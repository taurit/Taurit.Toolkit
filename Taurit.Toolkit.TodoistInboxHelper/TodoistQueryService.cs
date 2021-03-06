﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using Polly;
using Polly.Retry;
using RestSharp;
using Taurit.Toolkit.TodoistInboxHelper.ApiModels;

namespace Taurit.Toolkit.TodoistInboxHelper
{
    public class TodoistQueryService : TodoistApiService, ITodoistQueryService
    {
        public TodoistQueryService(String authToken) : base(authToken)
        {
        }

        public IReadOnlyList<Label> GetAllLabels()
        {
            var client = new RestClient(TodoistApiService.ApiUrl);

            var request = new RestRequest("sync", Method.POST);
            request.AddParameter("token", AuthToken);
            request.AddParameter("seq_no", "0");
            request.AddParameter("resource_types", "[\"labels\"]");

            IRestResponse<TodoistSyncResponseLabels> response = client.Execute<TodoistSyncResponseLabels>(request);
            TodoistQueryService.AssertHttpRequestSucceeded(response, "list of labels");

            Contract.Assume(response != null);
            Contract.Assume(response.Data != null);
            Contract.Assume(response.Data.labels != null);
            return response.Data.labels.Where(x => x.is_deleted == 0).ToList();
        }

        /// <remarks>
        ///     This query failed few times a day on: "Status code=0 The operation has timed out."
        ///     Therefore a simple retry policy is now defined for the request.
        /// </remarks>
        public IReadOnlyList<Project> GetAllProjects()
        {
            var client = new RestClient(TodoistApiService.ApiUrl);

            var request = new RestRequest("sync", Method.POST);
            request.AddParameter("token", AuthToken);
            request.AddParameter("seq_no", "0");
            request.AddParameter("resource_types", "[\"projects\"]");

            RetryPolicy<IRestResponse<TodoistSyncResponseProjects>> retryPolicy = Policy
                .HandleResult<IRestResponse<TodoistSyncResponseProjects>>(r => r.StatusCode == 0)
                .Or<WebException>()
                .WaitAndRetry(3, attempt => TimeSpan.FromMilliseconds(10000)
                );

            IRestResponse<TodoistSyncResponseProjects>
                response = retryPolicy.Execute(() => client.Execute<TodoistSyncResponseProjects>(request));

            TodoistQueryService.AssertHttpRequestSucceeded(response, "list of projects");

            return response.Data.projects;
        }

        public IReadOnlyList<TodoTask> GetAllTasks(
            ILookup<Int64, Project> allProjectsIndexedById,
            ILookup<Int64, Label> allLabelsIndexedById
        )
        {
            var client = new RestClient(TodoistApiService.ApiUrl);

            var request = new RestRequest("sync", Method.POST);
            request.AddParameter("token", AuthToken);

            // Sequence number, used to allow client to perform incremental sync. Pass 0 to retrieve all active resource data.
            request.AddParameter("seq_no", "0");
            request.AddParameter("resource_types", "[\"items\"]");

            IRestResponse<TodoistSyncResponseTasks> response =
                client.Execute<TodoistSyncResponseTasks>(request);
            TodoistQueryService.AssertHttpRequestSucceeded(response, "list of tasks");

            foreach (TodoTask task in response.Data.items)
            {
                task.project_name = allProjectsIndexedById[task.project_id].Single().name;
                task.labelsNames = task.labels.Select(x => allLabelsIndexedById[x].Single().name).ToArray();
            }

            return response.Data.items
                .Where(x => x != null && x.is_deleted == 0 && x.@checked == 0 && x.is_archived == 0).ToList();
        }

        public IReadOnlyList<TodoTask> GetAllCompletedTasks()
        {
            var client = new RestClient(TodoistApiService.ApiUrl);
            var request = new RestRequest("completed/get_all", Method.GET);
            request.AddParameter("token", AuthToken);

            IRestResponse<TodoistGetCompletedItemsResponse> response =
                client.Execute<TodoistGetCompletedItemsResponse>(request);
            TodoistQueryService.AssertHttpRequestSucceeded(response, "list of tasks");

            return response.Data.items.ToList();
        }

        private static void AssertHttpRequestSucceeded<T>(IRestResponse<T> response, String typeOfResourceDisplayString,
            [CallerMemberName] String caller = null)
        {
            if (!response.IsSuccessful)
            {
                throw new TodoistApiWebException(
                    $"{caller} Failed to retrieve {typeOfResourceDisplayString} from the Todoist API. Status code={response.StatusCode}",
                    response.ErrorException);
            }

            if (response.Data == null)
            {
                throw new TodoistApiWebException(
                    $"{caller} Failed to retrieve {typeOfResourceDisplayString} from the Todoist API. Data is null. Status code={response.StatusCode}",
                    response.ErrorException);
            }
        }
    }
}