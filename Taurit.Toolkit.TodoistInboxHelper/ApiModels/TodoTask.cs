using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Taurit.Toolkit.TodoistInboxHelper.ApiModels
{
    /// <summary>
    ///     Field names as in: https://developer.todoist.com/#items
    /// </summary>
    public class TodoTask
    {
        [JsonProperty]
        public Int64 id { get; set; }

        [JsonProperty]
        public String content { get; set; }

        [JsonProperty]
        public List<Int64> labels { get; set; }

        /// <summary>
        ///     The priority of the task (a number between 1 and 4, 4 for very urgent and 1 for natural).
        /// </summary>
        [JsonProperty]
        public Int32 priority { get; set; }

        [JsonProperty]
        public Int64 project_id { get; set; }

        /// <summary>
        ///     Whether the task is marked as completed (where 1 is true and 0 is false).
        /// </summary>
        [JsonProperty]
        public Int32 @checked { get; set; }

        /// <summary>
        ///     Whether the task is marked as deleted (where 1 is true and 0 is false).
        /// </summary>
        [JsonProperty]
        public Int32 is_deleted { get; set; }
    }
}