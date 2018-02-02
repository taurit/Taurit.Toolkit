using System;
using System.Xml.Linq;

namespace Taurit.Toolkit.OptimizeRssForFeedly.Services
{
    public class AtomFeedEditor : FeedEditorBase
    {
        public AtomFeedEditor(XDocument atomDocument) : base(atomDocument)
        {
        }

        protected override XElement ChannelNode =>
            Feed.Root ?? throw new InvalidOperationException("XML document doesn't contain root node");
    }
}