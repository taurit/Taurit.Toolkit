using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

#pragma warning disable IDE1006 // Naming Styles
namespace Taurit.Toolkit.TodoistInboxHelper.ApiModels
{
    [DataContract]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class TodoistGetCompletedItemsResponse
    {
        [DataMember]

        public List<TodoTask> items { get; set; }


        [DataMember]
        public List<Project> projects { get; set; }
    }
}
#pragma warning restore IDE1006 // Naming Styles