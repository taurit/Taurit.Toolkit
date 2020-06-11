using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using JetBrains.Annotations;
using MoreLinq.Extensions;
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
            var client = new RestClient(TodoistApiService.ApiUrl);

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

                Boolean responseSeemsToIndicateError = response.Content?.Contains("error_code") ?? false;
                if (!response.IsSuccessful || responseSeemsToIndicateError)
                {
                    // don't write the response in case of success because it's a big json with task guids, no value and lots of clutter
                    resultMessages.Add($"Details: {response.ErrorMessage} {response.ErrorException} {apiResponse}");
                }

                batchNumber++;
            }

            _commandsStrings.Clear();

            return resultMessages.Count == 0
                ? "there was nothing to send"
                : string.Join(Environment.NewLine, resultMessages);
        }

        /// <inheritdoc />
        public void AddUpdateProjectCommand(Int64 taskId, [CanBeNull] Int64? newProjectId)
        {
            if (!newProjectId.HasValue) return;

            // move task to another project
            var moveCommandId = Guid.NewGuid();
            String moveCommandString =
                $"{{\"type\": \"item_move\", \"uuid\": \"{moveCommandId}\", \"args\": {{\"id\": {taskId}, \"project_id\": {newProjectId.Value} }}}}";
            _commandsStrings.Add(moveCommandString);
        }

        /// <inheritdoc />
        public void AddUpdateLabelCommand(Int64 taskId, [CanBeNull] Int64? newLabelId)
        {
            if (!newLabelId.HasValue) return;

            var commandId = Guid.NewGuid();
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

            var commandId = Guid.NewGuid();
            String commandString =
                $"{{\"type\": \"item_update\", \"uuid\": \"{commandId}\", \"args\": {{\"id\": {taskId}, \"priority\": {newPriority.Value}}}}}";

            _commandsStrings.Add(commandString);
        }

        public void AddUpdateTextCommand(Int64 taskId, String newName)
        {
            if (newName is null) return;

            var commandId = Guid.NewGuid();

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
            String commandStringAsJson = JsonSerializer.Serialize(commandStringAsObject);

            _commandsStrings.Add(commandStringAsJson);
        }

        public void AddRemoveTasksCommands([NotNull] IReadOnlyList<TodoTask> completedTasks)
        {
            if (completedTasks == null) throw new ArgumentNullException(nameof(completedTasks));
            List<Int64> idsOfTasksToRemove = completedTasks.Select(x => x.id).ToList();

            foreach (Int64 idOfTaskToRemove in idsOfTasksToRemove)
            {
                // first we need to uncomplete the item (at least in API v8). Otherwise delete returns ok but does not work
                String uncompleteCommand = TodoistCommandService.CreateCommandForUncompleteItem(idOfTaskToRemove);
                _commandsStrings.Add(uncompleteCommand);

                // then we can delete
                String deleteCommand = TodoistCommandService.CreateCommandForDeleteItem(idOfTaskToRemove);
                _commandsStrings.Add(deleteCommand);
            }
        }

        private static String CreateCommandForUncompleteItem(Int64 idOfTask)
        {
            var commandId = Guid.NewGuid();

            var commandStringAsObject = new
            {
                type = "item_uncomplete",
                uuid = $"{commandId}",
                args = new
                {
                    // used to be 'ids', but since API v8 we can't pass more than one id in a request
                    id = idOfTask
                }
            };
            String commandStringAsJson = JsonSerializer.Serialize(commandStringAsObject);
            return commandStringAsJson;
        }

        private static String CreateCommandForDeleteItem(Int64 idOfTaskToRemove)
        {
            var commandId = Guid.NewGuid();

            var commandStringAsObject = new
            {
                type = "item_delete",
                uuid = $"{commandId}",
                args = new
                {
                    // used to be 'ids', but since API v8 we can't pass more than one id in a request
                    id = idOfTaskToRemove
                }
            };
            String commandStringAsJson = JsonSerializer.Serialize(commandStringAsObject);
            return commandStringAsJson;
        }
    }
}