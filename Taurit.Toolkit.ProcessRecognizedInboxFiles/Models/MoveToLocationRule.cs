using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Taurit.Toolkit.ProcessRecognizedInboxFiles.Models
{
    [DataContract]
    internal class MoveToLocationRule
    {
        [DataMember]
        public String? TargetLocation { get; private set; }

        [DataMember]
        public List<String>? Patterns { get; private set; }
    }
}