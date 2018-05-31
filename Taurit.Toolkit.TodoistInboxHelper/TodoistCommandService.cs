using System;
using System.Collections.Generic;
using System.Text;
using RestSharp;
using Taurit.Toolkit.TodoistInboxHelper.ApiModels;

namespace Taurit.Toolkit.TodoistInboxHelper
{
    public class TodoistCommandService : TodoistApiService, ITodoistCommandService
    {
        private readonly List<String> commandsStrings = new List<String>();

        public TodoistCommandService(String apiKey) : base(apiKey)
        {
        }

        /// <inheritdoc />
        public void AddUpdateTaskCommand(Int64 taskId, Int32 priority, Int64 labelId, Int64 project)
        {
            Guid commandId = Guid.NewGuid();
            var labels = new List<Int64> {labelId};

            // JSON array with int64 ids
            String labelsArrayString = "[" + String.Join(",", labels) + "]";
            String commandString =
                $"{{\"type\": \"item_update\", \"uuid\": \"{commandId}\", \"args\": {{\"id\": {taskId}, \"priority\": {priority}, \"labels\": {labelsArrayString}}}}}";

            commandsStrings.Add(commandString);
        }


        public String ExecuteCommands()
        {
            var client = new RestClient(ApiUrl);

            var request = new RestRequest("sync", Method.POST);
            request.AddParameter("token", AuthToken);

            var commandsString = new StringBuilder();
            commandsString.Append("[");
            commandsString.Append(String.Join(", ", commandsStrings));
            commandsString.Append("]");

            request.AddParameter("commands", commandsString.ToString());

            IRestResponse<TodoistSyncResponseTasks> response = client.Execute<TodoistSyncResponseTasks>(request);
            String apiResponse = response.Content;
            return apiResponse;
        }
        
    }
}