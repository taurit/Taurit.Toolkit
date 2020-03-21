using System;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using Taurit.Toolkit.OptimizeRssForFeedly.Models;
using Taurit.Toolkit.OptimizeRssForFeedly.Services;

namespace Taurit.Toolkit.OptimizeRssForFeedly
{
    /// <summary>
    ///     This app allows to enrich RSS feed with some metadata specific to Feedly.
    /// </summary>
    /// <remarks>
    ///     I use it as a part of build process for my statically generated blog. Documentation on Feedly RSS extensions can be
    ///     found eg on <see href="https://blog.feedly.com/10-ways-to-optimize-your-feed-for-feedly/" /> and
    ///     <see href="https://alligator.io/rss/optimizing-rss-feed-feedly/" />
    /// </remarks>
    internal class Program
    {
        private static void Main(String[] args)
        {
            Debug.Assert(args.Length == 2, "Expected two arguments: [feed-location.rss] [feed-metadata.json]");

            String inputRssFileName = args[0];
            String metadataFileName = args[1];

            // Load metadata from configuration file
            String metadataJsonContent = File.ReadAllText(metadataFileName);
            var metadata = JsonConvert.DeserializeObject<FeedlyMetadata>(metadataJsonContent);
            Debug.Assert(metadata.IsValid, "some required metadata values were not found");

            // Edit RSS feed
            FeedEditorBase feedEditor = FeedEditorFactory.GetForFile(inputRssFileName);
            if (feedEditor.Feed.ToString().Contains("http://webfeeds.org/rss/1.0"))
            {
                Console.WriteLine("WebFeeds extensions were already found in a feed, skipping.");
                return;
            }

            Program.AddMetadataToFeed(feedEditor, metadata);

            // Update RSS feed file
            feedEditor.Feed.Save(inputRssFileName);
        }

        private static void AddMetadataToFeed(FeedEditorBase rssFeedEditor, FeedlyMetadata metadata)
        {
            rssFeedEditor.AddFeedlyNamespace();
            rssFeedEditor.AddCoverImage(metadata.CoverImageUrl);
            rssFeedEditor.AddIcon(metadata.IconUrl);
            rssFeedEditor.AddLogo(metadata.LogoUrl);
            rssFeedEditor.SetAccentColor(metadata.AccentColor);
        }
    }
}