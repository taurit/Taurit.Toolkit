using System;
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
        public String name { get; set; }

        [JsonProperty]
        public Int32 item_order { get; set; }
    }
}