using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Taurit.Toolkit.ProcesTodoistInbox.Common.Models.Classification;
using Taurit.Toolkit.ProcesTodoistInbox.Common.Services;

namespace Taurit.Toolkit.ProcessTodoistInbox.Common.Tests
{
    [TestClass]
    public class ClassificationRulesFormatConverterTests
    {
        [TestMethod]
        public void StartsWithConditionIsCorrectlyRecognized()
        {
            // Arrange
            var sut = new ClassificationRulesFormatConverter();

            // Act
            ClassificationRule result = sut.Convert("if startsWith(anki) and numLabelsIs(0) then setLabel(nauka)");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.If);
            Assert.IsNotNull(result.Then);
            
            Assert.IsNull(result.If.project);
            Assert.IsNull(result.If.containsWord);
            Assert.IsNull(result.If.priority);
            Assert.IsNotNull(result.If.startsWith);
            Assert.AreEqual("anki", result.If.startsWith.Single());
            Assert.AreEqual(0, result.If.numLabels);

            Assert.IsNull(result.Then.moveToProject);
            Assert.IsNull(result.Then.setPriority);
            Assert.IsNotNull(result.Then.setLabel);
            Assert.AreEqual("nauka", result.Then.setLabel.Single());
        }

        [TestMethod]
        public void ContainsWordConditionIsCorrectlyRecognized()
        {
            // Arrange
            var sut = new ClassificationRulesFormatConverter();

            // Act
            ClassificationRule result = sut.Convert("if containsWord(anki) and numLabelsIs(0) then setLabel(nauka)");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.If);
            Assert.IsNotNull(result.Then);
            
            Assert.IsNull(result.If.project);
            Assert.IsNull(result.If.startsWith);
            Assert.IsNull(result.If.priority);
            Assert.IsNotNull(result.If.containsWord); 
            Assert.AreEqual("anki", result.If.containsWord.Single());
            Assert.AreEqual(0, result.If.numLabels);

            Assert.IsNull(result.Then.moveToProject);
            Assert.IsNull(result.Then.setPriority);
            Assert.IsNotNull(result.Then.setLabel);
            Assert.AreEqual("nauka", result.Then.setLabel.Single());
        }

        [TestMethod]
        public void ProjectConditionIsCorrectlyRecognized()
        {
            // Arrange
            var sut = new ClassificationRulesFormatConverter();

            // Act
            ClassificationRule result = sut.Convert("if projectIs(Inbox) then setLabel(nauka)");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.If);
            Assert.IsNotNull(result.Then);
            
            Assert.IsNull(result.If.containsWord);
            Assert.IsNull(result.If.numLabels);
            Assert.IsNull(result.If.startsWith);
            Assert.IsNull(result.If.priority);
            Assert.IsNotNull(result.If.project);
            Assert.AreEqual("Inbox", result.If.project.Single());

            Assert.IsNull(result.Then.moveToProject);
            Assert.IsNull(result.Then.setPriority);
            Assert.IsNotNull(result.Then.setLabel);
            Assert.AreEqual("nauka", result.Then.setLabel.Single());
        }

        [TestMethod]
        public void PriorityConditionIsCorrectlyRecognized()
        {
            // Arrange
            var sut = new ClassificationRulesFormatConverter();

            // Act
            ClassificationRule result = sut.Convert("if priorityIs(1) then setLabel(nauka)");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.If);
            Assert.IsNotNull(result.Then);
            
            Assert.IsNull(result.If.project);
            Assert.IsNull(result.If.containsWord);
            Assert.IsNull(result.If.numLabels);
            Assert.IsNull(result.If.startsWith);
            Assert.IsNotNull(result.If.priority);
            Assert.AreEqual(1, result.If.priority.Value);

            Assert.IsNull(result.Then.moveToProject);
            Assert.IsNull(result.Then.setPriority);
            Assert.IsNotNull(result.Then.setLabel);
            Assert.AreEqual("nauka", result.Then.setLabel.Single());
        }

        
        [TestMethod]
        public void PriorityCanBeDefinedInNaturalLanguage()
        {
            // Arrange
            var sut = new ClassificationRulesFormatConverter();

            // Act
            ClassificationRule resultUndefined = sut.Convert("if priorityIs(Undefined) then setLabel(nauka)");
            ClassificationRule resultLow = sut.Convert("if priorityIs(Low) then setLabel(nauka)");
            ClassificationRule resultMedium = sut.Convert("if priorityIs(Medium) then setLabel(nauka)");
            ClassificationRule resultHigh = sut.Convert("if priorityIs(High) then setLabel(nauka)");

            ClassificationRule resultUndefinedLowercase = sut.Convert("if priorityIs(undefined) then setLabel(nauka)");
            ClassificationRule resultLowLowercase = sut.Convert("if priorityIs(low) then setLabel(nauka)");
            ClassificationRule resultMediumLowercase = sut.Convert("if priorityIs(medium) then setLabel(nauka)");
            ClassificationRule resultHighLowercase = sut.Convert("if priorityIs(high) then setLabel(nauka)");

            // Assert          
            Assert.IsTrue(resultUndefined.If.priority.HasValue);
            Assert.AreEqual(1, resultUndefined.If.priority.Value);

            Assert.IsTrue(resultUndefinedLowercase.If.priority.HasValue);
            Assert.AreEqual(1, resultUndefinedLowercase.If.priority.Value);

            Assert.IsTrue(resultLow.If.priority.HasValue);
            Assert.AreEqual(2, resultLow.If.priority.Value);

            Assert.IsTrue(resultLowLowercase.If.priority.HasValue);
            Assert.AreEqual(2, resultLowLowercase.If.priority.Value);

            Assert.IsTrue(resultMedium.If.priority.HasValue);
            Assert.AreEqual(3, resultMedium.If.priority.Value);

            Assert.IsTrue(resultMediumLowercase.If.priority.HasValue);
            Assert.AreEqual(3, resultMediumLowercase.If.priority.Value);

            Assert.IsTrue(resultHigh.If.priority.HasValue);
            Assert.AreEqual(4, resultHigh.If.priority.Value);

            Assert.IsTrue(resultHighLowercase.If.priority.HasValue);
            Assert.AreEqual(4, resultHighLowercase.If.priority.Value);
            
        }

        [TestMethod]
        public void NumLabelsConditionIsCorrectlyRecognized()
        {
            // Arrange
            var sut = new ClassificationRulesFormatConverter();

            // Act
            ClassificationRule result = sut.Convert("if numLabelsIs(1) then setLabel(nauka)");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.If);
            Assert.IsNotNull(result.Then);
            
            Assert.IsNull(result.If.project);
            Assert.IsNull(result.If.containsWord);
            Assert.IsNull(result.If.priority);
            Assert.IsNull(result.If.startsWith);
            Assert.IsNotNull(result.If.numLabels);
            Assert.AreEqual(1, result.If.numLabels.Value);

            Assert.IsNull(result.Then.moveToProject);
            Assert.IsNull(result.Then.setPriority);
            Assert.IsNotNull(result.Then.setLabel);
            Assert.AreEqual("nauka", result.Then.setLabel.Single());
        }

    }
}