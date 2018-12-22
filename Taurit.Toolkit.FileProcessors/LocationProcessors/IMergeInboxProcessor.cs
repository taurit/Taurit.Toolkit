using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Taurit.Toolkit.FileProcessors.LocationProcessors
{
    public interface IMergeInboxProcessor
    {
        void MergeInbox([NotNull] String targetInboxPath, 
            [NotNull] IEnumerable<String> alternativeInboxPaths,
            [NotNull] ISet<String> filesToNeverMove);
    }
}