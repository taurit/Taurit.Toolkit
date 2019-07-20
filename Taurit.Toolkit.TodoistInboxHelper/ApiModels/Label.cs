using System;
using System.Diagnostics;
using Newtonsoft.Json;
// ReSharper disable InconsistentNaming

namespace Taurit.Toolkit.TodoistInboxHelper.ApiModels
{
    /// <summary>
    ///     This class is returned to client-side code. Be cautious with name changes.
    /// </summary>
    [DebuggerDisplay("Label `{name}`")]
    public class Label
    {
        [Obsolete("Should only be used for deserialization")]
        public Label()
        {
        }

        public Label(Int64 id, Int32 isDeleted, String name)
        {
            this.id = id;
            is_deleted = isDeleted;
            this.name = name;
        }

        [JsonProperty]
        public Int64 id { get; set; }

        [JsonProperty]
        public Int32 is_deleted { get; set; }

        [JsonProperty]
        public Int32 is_favorite { get; set; }

        [JsonProperty]
        public String name { get; set; }

        [JsonProperty]
        public Int32 item_order { get; set; }

        [JsonProperty]
        public Int32 color { get; set; }

        public override String ToString()
        {
            return name;
        }
    }
}