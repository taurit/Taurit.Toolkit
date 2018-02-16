using System.Diagnostics;
using System.Xml.Linq;
using System.Xml.XPath;
using JetBrains.Annotations;

namespace Taurit.Toolkit.OptimizeRssForFeedly.Services
{
    public class RssFeedEditor : FeedEditorBase
    {
        public RssFeedEditor([NotNull] XDocument rssDocument) : base(rssDocument)
        {
        }

        /// <inheritdoc />
        protected override XElement ChannelNode
        {
            get
            {
                Debug.Assert(Feed.Root != null);
                XElement channelNode = Feed.Root.XPathSelectElement("channel");
                Debug.Assert(channelNode != null);
                return channelNode;
            }
        }
    }
}