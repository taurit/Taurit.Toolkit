using Microsoft.VisualStudio.TestTools.UnitTesting;
using Taurit.Toolkit.ProcesTodoistInbox.Common.Models.Classification;
using Taurit.Toolkit.ProcesTodoistInbox.Common.Services;

namespace Taurit.Toolkit.ProcessTodoistInbox.Common.Tests
{
    [TestClass]
    public class ClassificationRulesFormatConverterTests
    {
        [TestMethod]
        public void StartsWithWordConditionIsCorrectlyRecognized()
        {
            // Arrange
            var sut = new ClassificationRulesFormatConverter();

            // Act
            ClassificationRule result = sut.Convert("if startsWith anki and numLabels 0 then setLabel nauka");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.If);
            Assert.IsNotNull(result.Then);
            Assert.AreEqual("anki", result.If.startsWith);
        }
    }
}