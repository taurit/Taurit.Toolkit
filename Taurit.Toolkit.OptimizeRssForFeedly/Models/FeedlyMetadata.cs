using System;
using System.Runtime.Serialization;

namespace Taurit.Toolkit.OptimizeRssForFeedly.Models
{
    [DataContract]
    internal class FeedlyMetadata
    {
        [DataMember] public String CoverImageUrl { get; set; }

        [DataMember] public String IconUrl { get; set; }

        [DataMember] public String LogoUrl { get; set; }

        [DataMember] public String AccentColor { get; set; }

        public Boolean IsValid => !String.IsNullOrWhiteSpace(CoverImageUrl) && !String.IsNullOrWhiteSpace(IconUrl) &&
                                  !String.IsNullOrWhiteSpace(LogoUrl) && !String.IsNullOrWhiteSpace(AccentColor);
    }
}