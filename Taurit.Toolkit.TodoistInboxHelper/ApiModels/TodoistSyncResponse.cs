using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Taurit.Toolkit.TodoistInboxHelper.ApiModels
{
    internal class TodoistSyncResponseTasks
    {
        [JsonProperty]
        public List<TodoTask> items { get; set; }

        [JsonProperty]
        public Int64 seq_no_global { get; set; }
    }

    internal class TodoistSyncResponseLabels
    {
        [JsonProperty]
        public List<Label> labels { get; set; }

        [JsonProperty]
        public Int64 seq_no_global { get; set; }
    }

    internal class TodoistSyncResponseProjects
    {
        [JsonProperty]
        public List<Project> projects { get; set; }

        [JsonProperty]
        public Int64 seq_no_global { get; set; }
    }
}