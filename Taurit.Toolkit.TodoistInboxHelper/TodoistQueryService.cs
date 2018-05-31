﻿using System;
using System.Collections.Generic;
using RestSharp;
using Taurit.Toolkit.TodoistInboxHelper.ApiModels;

namespace Taurit.Toolkit.TodoistInboxHelper
{
    public class TodoistQueryService : TodoistApiService
    {
        public TodoistQueryService(String authToken) : base(authToken)
        {
        }

        public IList<Label> GetAllLabels()
        {
            var client = new RestClient(ApiUrl);

            var request = new RestRequest("sync", Method.POST);
            request.AddParameter("token", AuthToken);
            request.AddParameter("seq_no", "0");
            request.AddParameter("resource_types", "[\"labels\"]");

            IRestResponse<TodoistSyncResponseLabels> response = client.Execute<TodoistSyncResponseLabels>(request);

            return response.Data.labels;
        }

        public IList<Project> GetAllProjects()
        {
            var client = new RestClient(ApiUrl);

            var request = new RestRequest("sync", Method.POST);
            request.AddParameter("token", AuthToken);
            request.AddParameter("seq_no", "0");
            request.AddParameter("resource_types", "[\"projects\"]");

            IRestResponse<TodoistSyncResponseProjects>
                response = client.Execute<TodoistSyncResponseProjects>(request);

            return response.Data.projects;
        }

        public IList<TodoTask> GetAllTasks()
        {
            var client = new RestClient(ApiUrl);

            var request = new RestRequest("sync", Method.POST);
            request.AddParameter("token", AuthToken);

            // Sequence number, used to allow client to perform incremental sync. Pass 0 to retrieve all active resource data. 
            request.AddParameter("seq_no", "0");
            request.AddParameter("resource_types", "[\"items\"]");

            IRestResponse<TodoistSyncResponseTasks> response =
                client.Execute<TodoistSyncResponseTasks>(request);

            return response.Data.items;
        }

        public IList<TodoTask> GetTasks()
        {
            var client = new RestClient(ApiUrl);

            var request = new RestRequest("sync", Method.POST);
            request.AddParameter("token", AuthToken);

            // Sequence number, used to allow client to perform incremental sync. Pass 0 to retrieve all active resource data. 
            request.AddParameter("seq_no", "0");
            request.AddParameter("resource_types", "[\"items\"]");

            IRestResponse<TodoistSyncResponseTasks> response =
                client.Execute<TodoistSyncResponseTasks>(request);

            return response.Data.items;
        }

        //    public String UpdateTasks(List<TodoTask> tasksToUpdate)
        //    {
        //        if (tasksToUpdate.Count == 0)
        //        {
        //            return "Empty list of tasks";
        //        }
        //        if (tasksToUpdate.Any(task => task.labels == null))
        //        {
        //            return "List of tasks contains at least one invalid item";
        //        }

        //        var client = new RestClient(ApiUrl);

        //        var request = new RestRequest("sync", Method.POST);
        //        request.AddParameter("token", _authToken);

        //        // build json command as string (a shortcut)
        //        var commandsString = new StringBuilder();
        //        commandsString.Append("[");
        //        for (var i = 0; i < tasksToUpdate.Count; i++)
        //        {
        //            String commandString = GetUpdateCommandString(tasksToUpdate[i]);
        //            commandsString.Append(commandString);

        //            if (i != tasksToUpdate.Count - 1)
        //            {
        //                commandsString.Append(",");
        //            }
        //        }

        //        commandsString.Append("]");


        //        request.AddParameter("commands", commandsString.ToString());

        //        IRestResponse<TodoistTasksResponse> response = client.Execute<TodoistTasksResponse>(request);
        //        String apiResponse = response.Content;
        //        return apiResponse;
        //    }

        //    private String GetUpdateCommandString(TodoTask task)
        //    {
        //        String commandString;
        //        Guid commandId = Guid.NewGuid();

        //        if (task.IsToBeDeleted)
        //        {
        //            // as in documentation, https://developer.todoist.com/sync/v7/#delete-items
        //            commandString =
        //                $"{{\"type\": \"item_delete\", \"uuid\": \"{commandId}\", \"args\": {{\"ids\": [{task.id}] }}}}";
        //        }
        //        else
        //        {
        //            // typical use case: update labels
        //            List<Int64> specialLabelsIds = Label.SpecialLabels.Select(x => x.id).ToList();
        //            IEnumerable<Int64> labelsExcludingSpecial = task.labels.Where(x => !specialLabelsIds.Contains(x));
        //            String labelsArrayString =
        //                "[" + String.Join(",", labelsExcludingSpecial) + "]"; // JSON array with int64 ids

        //            commandString =
        //                $"{{\"type\": \"item_update\", \"uuid\": \"{commandId}\", \"args\": {{\"id\": {task.id}, \"priority\": {task.priority}, \"labels\": {labelsArrayString}}}}}";
        //        }

        //        return commandString;
        //}
    }
}