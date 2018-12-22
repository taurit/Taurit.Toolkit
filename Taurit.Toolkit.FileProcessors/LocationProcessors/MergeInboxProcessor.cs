using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;

namespace Taurit.Toolkit.FileProcessors.LocationProcessors
{
    public class MergeInboxProcessor : IMergeInboxProcessor
    {
        public void MergeInbox([NotNull] String targetInboxPath, 
            [NotNull] IEnumerable<String> alternativeInboxPaths,
            [NotNull] ISet<String> filesToNeverMove)
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
                        Console.WriteLine($"Skipped moving blacklisted file: {fileName}");
                    else
                    {
                        String targetFilePath = Path.Combine(targetInboxPath, fileName);
                        Boolean fileExistsInTargetInbox = File.Exists(targetFilePath);

                        if (fileExistsInTargetInbox)
                        {
                            Console.WriteLine(
                                $"Inbox merge: cannot move file {filePath} to {targetFilePath} because a file with the same name already exists in a target directory.");
                        }
                        else
                            File.Move(filePath, targetFilePath);
                    }
                }
            }
        }
    }
}