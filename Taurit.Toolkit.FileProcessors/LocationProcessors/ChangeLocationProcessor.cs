using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;

namespace Taurit.Toolkit.FileProcessors.LocationProcessors
{
    public class ChangeLocationProcessor : IFileProcessor
    {
        private readonly IList<ChangeLocationRule> _rules;

        public ChangeLocationProcessor(IEnumerable<ChangeLocationRule> rules)
        {
            _rules = rules.ToList();
        }

        /// <inheritdoc />
        public void ProcessMatchingFiles(String directoryPath)
        {
            Contract.Assert(Directory.Exists(directoryPath), "Inbox directory must exist to enumerate files");

            String[] filesInDirectory = Directory.GetFiles(directoryPath);
            foreach (String filePath in filesInDirectory)
            {
                String fileName = Path.GetFileName(filePath);
                Debug.Assert(fileName != null);

                ChangeLocationRule ruleToApply = _rules.FirstOrDefault(rule => rule.CanBeAppliedTo(fileName));
                if (ruleToApply != null)
                {
                    String targetFilePath = Path.Combine(ruleToApply.TargetLocation, fileName);
                    Console.WriteLine($"Moving {fileName} to {targetFilePath}");
                    File.Move(filePath, targetFilePath);
                }
            }
        }
    }
}