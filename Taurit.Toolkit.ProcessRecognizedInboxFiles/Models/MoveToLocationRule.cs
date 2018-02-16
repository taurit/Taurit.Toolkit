﻿using System;
using System.Runtime.Serialization;

namespace Taurit.Toolkit.ProcessRecognizedInboxFiles.Models
{
    [DataContract]
    internal class MoveToLocationRule
    {
        [DataMember] public String Pattern { get; private set; }

        [DataMember] public String TargetLocation { get; private set; }
    }
}