using System;
using System.Diagnostics;
using Newtonsoft.Json;
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
    [DebuggerDisplay("Project `{name}`")]
    public class Project
    {
        [Obsolete("Should only be used for deserialization")]
        public Project()
        {
        }


        [JsonProperty]
        public Int64 id { get; set; }

        [JsonProperty]
        public Int32 is_deleted { get; set; }

        [JsonProperty]
        public Int32 is_archived { get; set; }

        [JsonProperty]
        public Int32 is_favorite { get; set; }

        [JsonProperty]
        public String name { get; set; }

        [JsonProperty]
        public Int32 item_order { get; set; }

        [JsonProperty]
        public Int32 color { get; set; }

        [JsonProperty]
        public Int32 indent { get; set; }

        [JsonProperty]
        public Int32 collapsed { get; set; }

        [JsonProperty]
        public Int32 shared { get; set; }

        [JsonProperty]
        public bool inbox_project { get; set; }

        [JsonProperty]
        public bool team_inbox { get; set; }
    }
}