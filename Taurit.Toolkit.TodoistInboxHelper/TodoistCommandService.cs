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
        public void AddUpdateTaskCommand(Int64 oldProjectId, Int64 taskId, Int32 priority, Int64 labelId, Int64 project)
        {
            // update task values
            Guid commandId = Guid.NewGuid();
            var labels = new List<Int64> {labelId};
            String labelsArrayString = "[" + String.Join(",", labels) + "]";
            String commandString =
                $"{{\"type\": \"item_update\", \"uuid\": \"{commandId}\", \"args\": {{\"id\": {taskId}, \"priority\": {priority}, \"labels\": {labelsArrayString}}}}}";
            
            commandsStrings.Add(commandString);

            // move task to another project
            Guid moveCommandId = Guid.NewGuid();
            String projectItems = $"{{\"{oldProjectId}\": [{taskId}]}}";
            String moveCommandString =
                $"{{\"type\": \"item_move\", \"uuid\": \"{moveCommandId}\", \"args\": {{\"project_items\": {projectItems}, \"to_project\": {project} }}}}";
            commandsStrings.Add(moveCommandString);
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
            commandsStrings.Clear();
            String apiResponse = response.Content;
            return apiResponse;
        }
        
    }
}