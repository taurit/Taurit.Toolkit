﻿using System;
using System.Runtime.Serialization;

namespace Taurit.Toolkit.ProcessRecognizedInboxFiles.Models
{
    [DataContract]
    public class ConvertToWebPRule
    {
        [DataMember]
        public String? Directory { get; private set; }

        [DataMember]
        public String? Pattern { get; private set; }

        [DataMember]
        public Int32 Quality { get; private set; }

        [DataMember]
        public Int32 PreserveOriginalThresholdBytes { get; private set; }
    }
}