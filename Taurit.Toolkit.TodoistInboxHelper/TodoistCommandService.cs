using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;
using JetBrains.Annotations;
using Newtonsoft.Json;
using RestSharp;
using Taurit.Toolkit.TodoistInboxHelper.ApiModels;

namespace Taurit.Toolkit.TodoistInboxHelper
{
    public class TodoistCommandService : TodoistApiService, ITodoistCommandService
    {
        [NotNull] private readonly List<String> _commandsStrings = new List<String>();

        public TodoistCommandService(String apiKey) : base(apiKey)
        {
        }


        public String ExecuteCommands()
        {
            var client = new RestClient(ApiUrl);

            var request = new RestRequest("sync", Method.POST);
            request.AddParameter("token", AuthToken);

            var commandsString = new StringBuilder();
            commandsString.Append("[");
            commandsString.Append(string.Join(", ", _commandsStrings));
            commandsString.Append("]");

            request.AddParameter("commands", commandsString.ToString());

            IRestResponse<TodoistSyncResponseTasks> response = client.Execute<TodoistSyncResponseTasks>(request);
            _commandsStrings.Clear();
            Contract.Assume(response != null);
            String apiResponse = response.Content;
            return apiResponse;
        }

        /// <inheritdoc />
        public void AddUpdateProjectCommand(Int64 taskId, Int64 oldProjectId, [CanBeNull] Int64? newProjectId)
        {
            if (!newProjectId.HasValue) return;

            // move task to another project
            Guid moveCommandId = Guid.NewGuid();
            String projectItems = $"{{\"{oldProjectId}\": [{taskId}]}}";
            String moveCommandString =
                $"{{\"type\": \"item_move\", \"uuid\": \"{moveCommandId}\", \"args\": {{\"project_items\": {projectItems}, \"to_project\": {newProjectId.Value} }}}}";
            _commandsStrings.Add(moveCommandString);
        }

        /// <inheritdoc />
        public void AddUpdateLabelCommand(Int64 taskId, [CanBeNull] Int64? newLabelId)
        {
            if (!newLabelId.HasValue) return;

            Guid commandId = Guid.NewGuid();
            var labels = new List<Int64> {newLabelId.Value};
            String labelsArrayString = "[" + string.Join(",", labels) + "]";
            String commandString =
                $"{{\"type\": \"item_update\", \"uuid\": \"{commandId}\", \"args\": {{\"id\": {taskId}, \"labels\": {labelsArrayString}}}}}";

            _commandsStrings.Add(commandString);
        }

        /// <inheritdoc />
        public void AddUpdatePriorityCommand(Int64 taskId, [CanBeNull] Int32? newPriority)
        {
            if (!newPriority.HasValue) return;

            Guid commandId = Guid.NewGuid();
            String commandString =
                $"{{\"type\": \"item_update\", \"uuid\": \"{commandId}\", \"args\": {{\"id\": {taskId}, \"priority\": {newPriority.Value}}}}}";

            _commandsStrings.Add(commandString);
        }

        public void AddUpdateTextCommand(Int64 taskId, String newName)
        {
            if (newName is null) return;

            Guid commandId = Guid.NewGuid();

            var commandStringAsObject = new
            {
                type = "item_update",
                uuid = $"{commandId}",
                args = new
                {
                    id = taskId,
                    content = newName
                }
            };
            String commandStringAsJson = JsonConvert.SerializeObject(commandStringAsObject);

            _commandsStrings.Add(commandStringAsJson);
        }
    }
}