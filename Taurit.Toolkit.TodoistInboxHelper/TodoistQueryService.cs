using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
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
            var client = new RestClient(ApiUrl);

            var request = new RestRequest("sync", Method.POST);
            request.AddParameter("token", AuthToken);
            request.AddParameter("seq_no", "0");
            request.AddParameter("resource_types", "[\"labels\"]");

            IRestResponse<TodoistSyncResponseLabels> response = client.Execute<TodoistSyncResponseLabels>(request);
            AssertHttpRequestSucceeded(response, "list of labels");

            Contract.Assume(response != null);
            Contract.Assume(response.Data != null);
            Contract.Assume(response.Data.labels != null);
            return response.Data.labels.Where(x => x.is_deleted == 0).ToList();
        }

        public IReadOnlyList<Project> GetAllProjects()
        {
            var client = new RestClient(ApiUrl);

            var request = new RestRequest("sync", Method.POST);
            request.AddParameter("token", AuthToken);
            request.AddParameter("seq_no", "0");
            request.AddParameter("resource_types", "[\"projects\"]");

            IRestResponse<TodoistSyncResponseProjects>
                response = client.Execute<TodoistSyncResponseProjects>(request);
            AssertHttpRequestSucceeded(response, "list of projects");

            return response.Data.projects;
        }

        public IReadOnlyList<TodoTask> GetAllTasks(ILookup<Int64, Project> allProjectsIndexedById)
        {
            var client = new RestClient(ApiUrl);

            var request = new RestRequest("sync", Method.POST);
            request.AddParameter("token", AuthToken);

            // Sequence number, used to allow client to perform incremental sync. Pass 0 to retrieve all active resource data. 
            request.AddParameter("seq_no", "0");
            request.AddParameter("resource_types", "[\"items\"]");

            IRestResponse<TodoistSyncResponseTasks> response =
                client.Execute<TodoistSyncResponseTasks>(request);
            AssertHttpRequestSucceeded(response, "list of tasks");

            foreach (TodoTask task in response.Data.items)
                task.project_name = allProjectsIndexedById[task.project_id].Single().name;

            return response.Data.items
                .Where(x => x != null && x.is_deleted == 0 && x.@checked == 0 && x.is_archived == 0).ToList();
        }
        
        private void AssertHttpRequestSucceeded<T>(IRestResponse<T> response, String typeOfResourceDisplayString, [CallerMemberName] String caller = null)
        {
            if (!response.IsSuccessful)
            {
                throw new TodoistApiWebException(
                    $"{caller} Failed to retrieve {typeOfResourceDisplayString} from the Todoist API. Status code={response.StatusCode}", response.ErrorException);
            }

            if (response.Data == null)
            {
                throw new TodoistApiWebException(
                    $"{caller} Failed to retrieve {typeOfResourceDisplayString} from the Todoist API. Data is null. Status code={response.StatusCode}", response.ErrorException);
            }
        }
    }
}