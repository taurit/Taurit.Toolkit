using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

// ReSharper disable InconsistentNaming
#pragma warning disable IDE1006 // Naming Styles
namespace Taurit.Toolkit.TodoistInboxHelper.ApiModels
{
    [DataContract]
    internal class TodoistSyncResponseTasks
    {
        [DataMember]
        public List<TodoTask> items { get; set; }

        [DataMember]
        public Int64 seq_no_global { get; set; }
    }

    [DataContract]
    internal class TodoistSyncResponseLabels
    {
        [DataMember]
        public List<Label> labels { get; set; }

        [DataMember]

        public Int64 seq_no_global { get; set; }
    }

    [DataContract]
    internal class TodoistSyncResponseProjects
    {
        [DataMember]
        public List<Project> projects { get; set; }

        [DataMember]
        public Int64 seq_no_global { get; set; }
    }
}
#pragma warning restore IDE1006 // Naming Styles