using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;
using JetBrains.Annotations;
using MoreLinq.Extensions;
using Newtonsoft.Json;
using RestSharp;
using Taurit.Toolkit.TodoistInboxHelper.ApiModels;

namespace Taurit.Toolkit.TodoistInboxHelper
{
    public class TodoistCommandService : TodoistApiService, ITodoistCommandService
    {
        [NotNull] private readonly List<String> _commandsStrings = new List<String>(100);

        public TodoistCommandService(String apiKey) : base(apiKey)
        {
        }


        public String ExecuteCommands()
        {
            var resultMessages = new List<String>(1); // typically we fit in one batch
            var client = new RestClient(ApiUrl);

            // There is a limit of 100 commands when syncing. When exceeded, the whole request fails.
            // I keep the limit lower for a safe margin in case of future changes.
            var maxCommandsSentInOneApiRequests = 50;
            IEnumerable<IEnumerable<String>> commandsBatched = _commandsStrings.Batch(maxCommandsSentInOneApiRequests);

            var batchNumber = 1;
            foreach (IEnumerable<String> batch in commandsBatched)
            {
                var request = new RestRequest("sync", Method.POST);
                request.AddParameter("token", AuthToken);

                var commandsString = new StringBuilder();
                commandsString.Append("[");
                commandsString.Append(string.Join(", ", batch));
                commandsString.Append("]");

                request.AddParameter("commands", commandsString.ToString());

                IRestResponse<TodoistSyncResponseTasks> response = client.Execute<TodoistSyncResponseTasks>(request);
                String apiResponse = response.Content;
                String resultMessage =
                    $"Batch #{batchNumber} result: StatusCode {response.StatusCode}";
                resultMessages.Add(resultMessage);

                if (!response.IsSuccessful)
                {
                    // dont write the response in case of success because it's a big json with task guids, no value and lots of clutter
                    resultMessages.Add($"Details: {response.ErrorMessage} {response.ErrorException} {apiResponse}");
                }

                batchNumber++;
            }
            _commandsStrings.Clear();

            return resultMessages.Count == 0
                ? "there was nothing to send"
                : String.Join(Environment.NewLine, resultMessages);
        }

        /// <inheritdoc />
        public void AddUpdateProjectCommand(Int64 taskId, [CanBeNull] Int64? newProjectId)
        {
            if (!newProjectId.HasValue) return;

            // move task to another project
            Guid moveCommandId = Guid.NewGuid();
            String moveCommandString =
                $"{{\"type\": \"item_move\", \"uuid\": \"{moveCommandId}\", \"args\": {{\"id\": {taskId}, \"project_id\": {newProjectId.Value} }}}}";
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