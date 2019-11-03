using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

// ReSharper disable InconsistentNaming

namespace Taurit.Toolkit.ProcessTodoistInbox.Common.Models.ClassificationRuleModels
{
    /// <summary>
    /// Represents part of the classification rule that defines the action
    /// </summary>
    [SuppressMessage("Microsoft.Design", "IDE1006", Justification = "Names are aligned with JSON property names")]
    [DataContract]
    public class ClassificationRuleThen
    {
        [Obsolete("Use only if you are JSON deserializer")]
        public ClassificationRuleThen()
        {
        }

        public ClassificationRuleThen(Int32? setPriority, String setLabel, String moveToProject, String setDuration)
        {
            this.setDuration = setDuration;
            this.setPriority = setPriority;
            this.setLabel = setLabel;
            this.moveToProject = moveToProject;
        }

        [DataMember]
        public Int32? setPriority { get; set; }

        [DataMember]
        public String setLabel { get; set; }

        [DataMember]
        public String moveToProject { get; set; }

        [DataMember]
        public String setDuration { get; set; }
    }
}