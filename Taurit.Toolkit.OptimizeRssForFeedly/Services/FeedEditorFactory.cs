using System;
using System.Xml.Linq;

namespace Taurit.Toolkit.OptimizeRssForFeedly.Services
{
    internal static class FeedEditorFactory
    {
        public static FeedEditorBase GetForFile(String inputRssFilePath)
        {
            XDocument rssDocument = XDocument.Load(inputRssFilePath);

            if (inputRssFilePath.EndsWith("atom")) return new AtomFeedEditor(rssDocument);

            return new RssFeedEditor(rssDocument);
        }
    }
}