﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using MoreLinq;

namespace Taurit.Toolkit.FileProcessors.LocationProcessors
{
    public class ChangeLocationProcessor : FileProcessorBase
    {
        private readonly IList<ChangeLocationRule> _rules;

        public ChangeLocationProcessor(IEnumerable<ChangeLocationRule> rules)
        {
            _rules = rules.ToList();
        }

        /// <inheritdoc />
        public override void ProcessMatchingFile(String filePath)
        {
            String fileName = Path.GetFileName(filePath);
            Debug.Assert(fileName != null);

            List<ChangeLocationRule> applicableRules = _rules.Where(rule => rule.CanBeAppliedTo(fileName)).ToList();
            List<ChangeLocationRule> uniqueRules = applicableRules.ToHashSet(new ChangeLocationRuleTargetComparer()).ToList();
            if (uniqueRules.Count > 1)
            {
                Console.WriteLine($"Manual action required: conflicting rules were found for file {fileName}");
                foreach (ChangeLocationRule rule in applicableRules)
                    Console.WriteLine($"* Move to {rule.TargetLocation}");
            }
            else if (uniqueRules.Count == 1)
            {
                ChangeLocationRule ruleToApply = uniqueRules.Single();

                String targetFilePath = Path.Combine(ruleToApply.TargetLocation, fileName);
                Console.WriteLine($"Moving {fileName} to {targetFilePath}");
                File.Move(filePath, targetFilePath);
            }
        }
    }
}