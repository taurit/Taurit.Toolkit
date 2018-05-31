using System;
using System.Collections.Generic;
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

            return response.Data.labels;
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

            return response.Data.projects;
        }

        public IReadOnlyList<TodoTask> GetAllTasks()
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

        public IReadOnlyList<TodoTask> GetTasks()
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
        
    }
}