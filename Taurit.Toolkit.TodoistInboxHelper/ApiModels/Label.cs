using System;
using System.Diagnostics;
using System.Runtime.Serialization;

// ReSharper disable InconsistentNaming
#pragma warning disable IDE1006 // Naming Styles
namespace Taurit.Toolkit.TodoistInboxHelper.ApiModels
{
    /// <summary>
    ///     This class is returned to client-side code. Be cautious with name changes.
    /// </summary>
    [DebuggerDisplay("Label `{name}`")]
    [DataContract]
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

        [DataMember]
        public Int64 id { get; set; }

        [DataMember]
        public Int32 is_deleted { get; set; }

        [DataMember]
        public Int32 is_favorite { get; set; }

        [DataMember]
        public String name { get; set; }

        [DataMember]
        public Int32 item_order { get; set; }

        [DataMember]
        public Int32 color { get; set; }

        public override String ToString()
        {
            return name;
        }
    }
}
#pragma warning restore IDE1006 // Naming Styles