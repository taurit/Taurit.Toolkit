using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Taurit.Toolkit.FileProcessors.LocationProcessors
{
    public interface IMergeInboxProcessor
    {
        void MergeInbox(
            ILogger<IMergeInboxProcessor> logger,
            String targetInboxPath,
            IEnumerable<String> alternativeInboxPaths,
            ISet<String> filesToNeverMove
        );
    }
}