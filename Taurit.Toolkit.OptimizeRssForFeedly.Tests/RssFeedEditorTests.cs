using System;
using System.Xml.Linq;
using Taurit.Toolkit.OptimizeRssForFeedly.Services;
using Xunit;

namespace Taurit.Toolkit.OptimizeRssForFeedly.Tests
{
    public class RssFeedEditorTests
    {
        [Fact]
        public void Constructor_DoesNotModifyDocument()
        {
            // Arrange
            XDocument originalRssDocument = XDocument.Load("sample-feed.xml");
            var rssDocument = new XDocument(originalRssDocument);

            // Act
            var sut = new RssFeedEditor(rssDocument);

            // Assert
            Assert.True(XNode.DeepEquals(sut.Feed, originalRssDocument));
        }

        [Fact]
        public void FeedlyNamespaceCanBeAdded()
        {
            // Arrange
            XDocument rssDocument = XDocument.Load("sample-feed.xml");
            var sut = new RssFeedEditor(rssDocument);

            // Act
            sut.AddFeedlyNamespace();

            // Assert
            Assert.NotNull(sut.Feed.Root);
            Assert.Contains(sut.Feed.Root.Attributes(),
                x => x.ToString() == "xmlns:webfeeds=\"http://webfeeds.org/rss/1.0\"");
        }

        [Fact]
        public void RssCoverImageCanBeAdded()
        {
            // Arrange
            XDocument rssDocument = XDocument.Load("sample-feed.xml");
            var sut = new RssFeedEditor(rssDocument);

            // Act
            sut.AddFeedlyNamespace();
            sut.AddCoverImage("https://example.com/image.png");

            // Assert
            Assert.NotNull(sut.Feed.Root);
            Assert.Contains("<webfeeds:cover image=\"https://example.com/image.png\" />",
                sut.Feed.Root.ToString()); // fix when I'm better at LINQ to XML
        }


        [Fact]
        public void RssIconCanBeAdded()
        {
            // Arrange
            XDocument rssDocument = XDocument.Load("sample-feed.xml");
            var sut = new RssFeedEditor(rssDocument);

            // Act
            sut.AddFeedlyNamespace();
            sut.AddIcon("https://example.com/image.png");

            // Assert
            Assert.NotNull(sut.Feed.Root);
            Assert.Contains("<webfeeds:icon>https://example.com/image.png</webfeeds:icon>",
                sut.Feed.Root.ToString()); // fix when I'm better at LINQ to XML
        }


        [Fact]
        public void RssLogoCanBeAdded()
        {
            // Arrange
            XDocument rssDocument = XDocument.Load("sample-feed.xml");
            var sut = new RssFeedEditor(rssDocument);

            // Act
            sut.AddFeedlyNamespace();
            sut.AddLogo("https://example.com/image.png");

            // Assert
            Assert.NotNull(sut.Feed.Root);
            Assert.Contains("<webfeeds:logo>https://example.com/image.png</webfeeds:logo>",
                sut.Feed.Root.ToString());
        }

        [Fact]
        public void RssAccentColorCanBeDefined()
        {
            // Arrange
            XDocument rssDocument = XDocument.Load("sample-feed.xml");
            var sut = new RssFeedEditor(rssDocument);

            // Act
            sut.AddFeedlyNamespace();
            sut.SetAccentColor("FF0000");

            // Assert
            Assert.NotNull(sut.Feed.Root);
            Assert.Contains("<webfeeds:accentColor>FF0000</webfeeds:accentColor>",
                sut.Feed.Root.ToString());
        }

        [Fact]
        public void RssinvalidAccentColorIsNotAccepted()
        {
            // Arrange
            XDocument rssDocument = XDocument.Load("sample-feed.xml");
            var sut = new RssFeedEditor(rssDocument);
            sut.AddFeedlyNamespace();

            // Act
            Assert.Throws<ArgumentException>(() => sut.SetAccentColor("#FF0000")); // hash is not expected
            
        }
    }
}