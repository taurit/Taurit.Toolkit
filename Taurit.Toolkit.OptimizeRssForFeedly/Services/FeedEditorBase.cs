using System;
using System.Diagnostics;
using System.Xml.Linq;
using JetBrains.Annotations;

namespace Taurit.Toolkit.OptimizeRssForFeedly.Services
{
    public abstract class FeedEditorBase
    {
        private const String WebFeedsNamespaceValue = "http://webfeeds.org/rss/1.0";

        private const String WebFeedsNamespaceKey = "webfeeds";

        private readonly XNamespace _webFeedsNamespace = FeedEditorBase.WebFeedsNamespaceValue;

        protected FeedEditorBase([NotNull] XDocument feedDocument)
        {
            Feed = new XDocument(feedDocument); // work on copy, so input parameter will not be modified
        }

        public XDocument Feed { get; }

        [NotNull]
        protected abstract XElement ChannelNode { get; }

        public void AddFeedlyNamespace()
        {
            Debug.Assert(Feed.Root != null);

            Feed.Root.Add(new XAttribute(XNamespace.Xmlns + FeedEditorBase.WebFeedsNamespaceKey,
                FeedEditorBase.WebFeedsNamespaceValue));
        }

        public void AddCoverImage(String imageUrl)
        {
            // expected: <webfeeds:cover image=”http://yoursite.com/a-large-cover-image.png“ />
            ChannelNode.AddFirst(new XElement(_webFeedsNamespace + "cover", new XAttribute("image", imageUrl)));
        }

        public void AddIcon(String iconUrl)
        {
            ChannelNode.AddFirst(new XElement(_webFeedsNamespace + "icon", iconUrl));
        }

        public void AddLogo(String logoUrl)
        {
            ChannelNode.AddFirst(new XElement(_webFeedsNamespace + "logo", logoUrl));
        }

        public void SetAccentColor([NotNull] String accentColor)
        {
            if (accentColor == null) throw new ArgumentNullException(nameof(accentColor));

            if (accentColor.Length != 6)
            {
                throw new ArgumentException(
                    "Color should be in a form of hex-encoded RGB, without hash (6 characters in total)",
                    nameof(accentColor));
            }

            ChannelNode.AddFirst(new XElement(_webFeedsNamespace + "accentColor", accentColor));
        }
    }
}