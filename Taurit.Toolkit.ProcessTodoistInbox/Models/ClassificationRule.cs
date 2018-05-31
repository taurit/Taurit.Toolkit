﻿using System;
using Newtonsoft.Json;
using Taurit.Toolkit.TodoistInboxHelper.ApiModels;

namespace Taurit.Toolkit.ProcessTodoistInbox.Models
{
    public class ClassificationRule
    {
        [JsonProperty]
        public String taskContains { get; set; }

        [JsonProperty]
        public Int32 setPriority { get; set; }

        [JsonProperty]
        public String setLabel { get; set; }

        [JsonProperty]
        public String moveToProject { get; set; }

        public Boolean Matches(TodoTask task)
        {
            return task.content.Contains(taskContains);
        }
    }
}