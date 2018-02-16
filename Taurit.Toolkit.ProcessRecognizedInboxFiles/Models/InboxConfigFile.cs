using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Taurit.Toolkit.ProcessRecognizedInboxFiles.Models
{
    [DataContract]
    internal class InboxConfigFile
    {
        [DataMember] public String InboxFolder { get; private set; }

        [DataMember] public List<MoveToLocationRule> MoveToLocationRules { get; private set; }
    }
}