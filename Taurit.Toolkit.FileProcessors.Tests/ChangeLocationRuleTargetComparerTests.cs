using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Taurit.Toolkit.FileProcessors.LocationProcessors;

namespace Taurit.Toolkit.FileProcessors.Tests
{
    [TestClass]
    public class ChangeLocationRuleTargetComparerTests
    {
        [TestMethod]
        public void WhenTargetLocationIsDifferent_AllRulesArePreservedInASet()
        {
            // Arrange
            var rules = new List<ChangeLocationRule>
            {
                new ChangeLocationRule("asdf", "c:\\"),
                new ChangeLocationRule("sdfg", "d:\\")
            };
            var sut = new ChangeLocationRuleTargetComparer();

            // Act
            var uniqueRules = new HashSet<ChangeLocationRule>(rules, sut);

            // Assert
            Assert.AreEqual(rules.Count, uniqueRules.Count);
        }

        [TestMethod]
        public void WhenTargetLocationIsTheSame_OnlyOneRuleIsPreservedInASet()
        {
            // Arrange
            var rules = new List<ChangeLocationRule>
            {
                new ChangeLocationRule("asdf", "c:\\"),
                new ChangeLocationRule("sdfg", "c:\\")
            };
            var sut = new ChangeLocationRuleTargetComparer();

            // Act
            var uniqueRules = new HashSet<ChangeLocationRule>(rules, sut);

            // Assert
            Assert.AreEqual(1, uniqueRules.Count);
        }
    }
}