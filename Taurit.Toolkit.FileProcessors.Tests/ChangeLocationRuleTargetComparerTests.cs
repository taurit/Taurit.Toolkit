using System.Collections.Generic;
using Taurit.Toolkit.FileProcessors.LocationProcessors;
using Xunit;

namespace Taurit.Toolkit.FileProcessors.Tests
{
    public class ChangeLocationRuleTargetComparerTests
    {
        [Fact]
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
            Assert.Equal(rules.Count, uniqueRules.Count);
        }

        [Fact]
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
            Assert.Single(uniqueRules);
        }
    }
}