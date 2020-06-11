using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;

namespace Taurit.Toolkit.FileProcessors.LocationProcessors
{
    public class MergeInboxProcessor : IMergeInboxProcessor
    {
        public void MergeInbox(
            ILogger<IMergeInboxProcessor> logger,
            String targetInboxPath,
            IEnumerable<String> alternativeInboxPaths,
            ISet<String> filesToNeverMove
        )
        {
            if (targetInboxPath == null) throw new ArgumentNullException(nameof(targetInboxPath));
            if (alternativeInboxPaths == null) throw new ArgumentNullException(nameof(alternativeInboxPaths));
            if (filesToNeverMove == null) throw new ArgumentNullException(nameof(filesToNeverMove));


            foreach (String alternativeInboxPath in alternativeInboxPaths)
            {
                String[] filesInAlternativeInbox = Directory.GetFiles(alternativeInboxPath);
                foreach (String filePath in filesInAlternativeInbox)
                {
                    String fileName = Path.GetFileName(filePath);
                    if (filesToNeverMove.Contains(fileName))
                        logger.LogInformation($"Skipped moving blacklisted file: {fileName}");
                    else
                    {
                        String targetFilePath = Path.Combine(targetInboxPath, fileName);
                        Boolean fileExistsInTargetInbox = File.Exists(targetFilePath);

                        if (fileExistsInTargetInbox)
                            logger.LogWarning(
                                $"Inbox merge: cannot move file {filePath} to {targetFilePath} because a file with the same name already exists in a target directory.");
                        else
                            File.Move(filePath, targetFilePath);
                    }
                }
            }
        }
    }
}