using System;
using System.Runtime.Serialization;

namespace Taurit.Toolkit.OptimizeRssForFeedly.Models
{
    [DataContract]
    internal class FeedlyMetadata
    {
        [DataMember]
        public String CoverImageUrl { get; set; }

        [DataMember]
        public String IconUrl { get; set; }

        [DataMember]
        public String LogoUrl { get; set; }

        [DataMember]
        public String AccentColor { get; set; }

        public Boolean IsValid => !string.IsNullOrWhiteSpace(CoverImageUrl) && !string.IsNullOrWhiteSpace(IconUrl) &&
                                  !string.IsNullOrWhiteSpace(LogoUrl) && !string.IsNullOrWhiteSpace(AccentColor);
    }
}