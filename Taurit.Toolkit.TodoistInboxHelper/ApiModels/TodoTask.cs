﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;

// ReSharper disable InconsistentNaming

namespace Taurit.Toolkit.TodoistInboxHelper.ApiModels
{
    /// <summary>
    ///     Field names as in: https://developer.todoist.com/#items
    /// </summary>
    [DebuggerDisplay("Task `{content}`")]
    public class TodoTask
    {
        [JsonProperty] public Int32 is_archived;

        public String project_name;

        [JsonProperty]
        public Int64 id { get; set; }

        [JsonProperty]
        public Int64 user_id { get; set; }

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
        ///     The indent of the task (a number between 1 and 5, where 1 is top-level).
        /// </summary>
        [JsonProperty]
        public Int64 indent { get; set; }

        /// <summary>
        ///     The order of the task inside a project (the smallest value would place the task at the top).
        /// </summary>
        [JsonProperty]
        public Int64 item_order { get; set; }

        /// <summary>
        ///     The order of the task inside the Today or Next 7 days view (a number, where the smallest value would place the task
        ///     at the top).
        /// </summary>
        [JsonProperty]
        public Int64 day_order { get; set; }

        /// <summary>
        ///     Whether the task’s sub-tasks are collapsed (where 1 is true and 0 is false).
        /// </summary>
        [JsonProperty]
        public Int64 collapsed { get; set; }


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

        /// <summary>
        ///     Whether the task has been marked as completed and is marked to be moved to history, because all the child tasks of
        ///     its parent are also marked as completed (where 1 is true and 0 is false)
        /// </summary>
        [JsonProperty]
        public Int32 in_history { get; set; }

        /// <summary>
        ///     The date of the task, added in free form text, for example it can be <c>every day @ 10</c> (or <c>null</c> or an
        ///     empty string if not set).
        /// </summary>
        [JsonProperty]
        public String date_string { get; set; }

        /// <summary>
        ///     The language of the date_string (valid languages are: en, da, pl, zh, ko, de, pt, ja, it, fr, sv, ru, es, nl).
        /// </summary>
        [JsonProperty]
        public String date_lang { get; set; }

        /// <summary>
        ///     The date of the task in the format Mon 07 Aug 2006 12:34:56 +0000 (or null if not set). For all day task (i.e. task
        ///     due “Today”), the time part will be set as xx:xx:59.
        /// </summary>
        [JsonProperty]
        public String due_date_utc { get; set; }

        /// <summary>
        ///     The date when the task was created.
        /// </summary>
        [JsonProperty]
        public String date_added { get; set; }

        public Boolean HasDate => !string.IsNullOrWhiteSpace(date_string);
    }
}