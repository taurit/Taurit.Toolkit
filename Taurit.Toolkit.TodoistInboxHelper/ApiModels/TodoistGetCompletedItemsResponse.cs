using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace Taurit.Toolkit.TodoistInboxHelper.ApiModels
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class TodoistGetCompletedItemsResponse
    {
        [JsonProperty]
        public List<TodoTask> items { get; set; }

        [JsonProperty]
        public List<Project> projects { get; set; }
    }
}