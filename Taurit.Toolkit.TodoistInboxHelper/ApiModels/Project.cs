using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

// ReSharper disable InconsistentNaming

namespace Taurit.Toolkit.TodoistInboxHelper.ApiModels
{
    //{
    //    "id": 128501470,
    //    "name": "Project1",
    //    "color": 1,
    //    "indent": 1,
    //    "item_order": 36,
    //    "collapsed": 0,
    //    "shared": false,
    //    "is_deleted": 0,
    //    "is_archived": 0,
    //    "is_favorite": 0
    //}
    [DataContract]
    [DebuggerDisplay("Project `{" + nameof(Project.name) + "}`")]
    [SuppressMessage("Microsoft.Design", "IDE1006", Justification = "Names are aligned with JSON property names")]
    public class Project
    {
        [Obsolete("Should only be used for deserialization")]
        public Project()
        {
        }

        [DataMember]
        public Int64 id { get; set; }

        [DataMember]
        public Int32 is_deleted { get; set; }

        [DataMember]
        public Int32 is_archived { get; set; }

        [DataMember]
        public Int32 is_favorite { get; set; }

        [DataMember]
        public String name { get; set; }

        [DataMember]
        public Int32 item_order { get; set; }

        [DataMember]
        public Int32 color { get; set; }

        [DataMember]
        public Int32 indent { get; set; }

        [DataMember]
        public Int32 collapsed { get; set; }

        [DataMember]
        public Int32 shared { get; set; }

        [DataMember]
        public Boolean inbox_project { get; set; }

        [DataMember]
        public Boolean team_inbox { get; set; }

        public override String ToString()
        {
            return name;
        }
    }
}