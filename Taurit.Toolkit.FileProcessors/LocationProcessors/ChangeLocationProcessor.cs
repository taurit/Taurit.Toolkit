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
                ProcessMatchingFile(filePath);
            }
        }

        /// <inheritdoc />
        public void ProcessMatchingFile(String filePath)
        {
            String fileName = Path.GetFileName(filePath);
            Debug.Assert(fileName != null);

            List<ChangeLocationRule> applicableRules = _rules.Where(rule => rule.CanBeAppliedTo(fileName)).ToList();
            if (applicableRules.Count > 1)
            {
                Console.WriteLine($"Manual action required: conflicting rules were found for file {fileName}");
                foreach (ChangeLocationRule rule in applicableRules)
                    Console.WriteLine($"* Move to {rule.TargetLocation}");
            }
            else if (applicableRules.Count == 1)
            {
                ChangeLocationRule ruleToApply = applicableRules.Single();

                String targetFilePath = Path.Combine(ruleToApply.TargetLocation, fileName);
                Console.WriteLine($"Moving {fileName} to {targetFilePath}");
                File.Move(filePath, targetFilePath);
            }
        }
    }
}