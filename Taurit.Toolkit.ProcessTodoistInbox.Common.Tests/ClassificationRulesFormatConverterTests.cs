using System;
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
        [ExpectedException(typeof(ArgumentNullException))]
        public void WhenNullRuleIsPassedThenExceptionIsThrown()
        {
            // Arrange
            var sut = new ClassificationRulesFormatConverter();

            // Act
            // ReSharper disable once AssignNullToNotNullAttribute
            sut.Convert(null);

            // Assert
            Assert.Fail("Exception should have been thrown before this line");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void WhenIfOrThenClauseIsNotPresentThenExceptionIsThrown()
        {
            // Arrange
            var sut = new ClassificationRulesFormatConverter();

            // Act
            // ReSharper disable once AssignNullToNotNullAttribute
            sut.Convert("yo dawg");

            // Assert
            Assert.Fail("Exception should have been thrown before this line");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void WhenIfClauseIsEmptyExceptionIsThrown()
        {
            // Arrange
            var sut = new ClassificationRulesFormatConverter();

            // Act
            // ReSharper disable once AssignNullToNotNullAttribute
            sut.Convert("if then setLabel(nauka)");

            // Assert
            Assert.Fail("Exception should have been thrown before this line");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void WhenThenClauseIsEmptyExceptionIsThrown()
        {
            // Arrange
            var sut = new ClassificationRulesFormatConverter();

            // Act
            // ReSharper disable once AssignNullToNotNullAttribute
            sut.Convert("if startsWith(anki) then ");

            // Assert
            Assert.Fail("Exception should have been thrown before this line");
        }

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
            Assert.AreEqual("nauka", result.Then.setLabel);
        }

        [TestMethod]
        public void StartsWithConditionIsCorrectlyRecognizedWithMultipleArguments()
        {
            // Arrange
            var sut = new ClassificationRulesFormatConverter();

            // Act
            ClassificationRule result =
                sut.Convert("if startsWith(anki|supermemo|angielski) and numLabelsIs(0) then setLabel(nauka)");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.If);
            Assert.IsNotNull(result.Then);

            Assert.IsNull(result.If.project);
            Assert.IsNull(result.If.containsWord);
            Assert.IsNull(result.If.priority);
            Assert.AreEqual(0, result.If.numLabels);

            Assert.IsNotNull(result.If.startsWith);
            Assert.AreEqual(3, result.If.startsWith.Length);
            Assert.IsTrue(result.If.startsWith.Contains("anki"));
            Assert.IsTrue(result.If.startsWith.Contains("supermemo"));
            Assert.IsTrue(result.If.startsWith.Contains("angielski"));

            Assert.IsNull(result.Then.moveToProject);
            Assert.IsNull(result.Then.setPriority);
            Assert.IsNotNull(result.Then.setLabel);
            Assert.AreEqual("nauka", result.Then.setLabel);
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
            Assert.AreEqual("nauka", result.Then.setLabel);
        }

        [TestMethod]
        public void ContainsWordConditionIsCorrectlyRecognizedWithMultipleArguments()
        {
            // Arrange
            var sut = new ClassificationRulesFormatConverter();

            // Act
            ClassificationRule result =
                sut.Convert("if containsWord(anki|supermemo|angielski) and numLabelsIs(0) then setLabel(nauka)");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.If);
            Assert.IsNotNull(result.Then);

            Assert.IsNull(result.If.project);
            Assert.IsNull(result.If.startsWith);
            Assert.IsNull(result.If.priority);

            Assert.IsNotNull(result.If.containsWord);
            Assert.AreEqual(3, result.If.containsWord.Length);
            Assert.IsTrue(result.If.containsWord.Contains("anki"));
            Assert.IsTrue(result.If.containsWord.Contains("supermemo"));
            Assert.IsTrue(result.If.containsWord.Contains("angielski"));

            Assert.AreEqual(0, result.If.numLabels);

            Assert.IsNull(result.Then.moveToProject);
            Assert.IsNull(result.Then.setPriority);
            Assert.IsNotNull(result.Then.setLabel);
            Assert.AreEqual("nauka", result.Then.setLabel);
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
            Assert.AreEqual("Inbox", result.If.project);

            Assert.IsNull(result.Then.moveToProject);
            Assert.IsNull(result.Then.setPriority);
            Assert.IsNotNull(result.Then.setLabel);
            Assert.AreEqual("nauka", result.Then.setLabel);
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
            Assert.AreEqual("nauka", result.Then.setLabel);
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
            Assert.AreEqual("nauka", result.Then.setLabel);
        }

        [TestMethod]
        public void SetLabelActionIsCorrectlyRecognized()
        {
            // Arrange
            var sut = new ClassificationRulesFormatConverter();

            // Act
            ClassificationRule result = sut.Convert("if numLabelsIs(0) then setLabel(nauka)");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.If);
            Assert.IsNotNull(result.Then);

            Assert.IsNull(result.Then.moveToProject);
            Assert.IsNull(result.Then.setPriority);
            Assert.IsNotNull(result.Then.setLabel);
            Assert.AreEqual("nauka", result.Then.setLabel);
        }

        [TestMethod]
        public void MoveToProjectActionIsCorrectlyRecognized()
        {
            // Arrange
            var sut = new ClassificationRulesFormatConverter();

            // Act
            ClassificationRule result = sut.Convert("if numLabelsIs(0) then moveToProject(Obowiązki)");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.If);
            Assert.IsNotNull(result.Then);

            Assert.IsNull(result.Then.setLabel);
            Assert.IsNull(result.Then.setPriority);
            Assert.IsNotNull(result.Then.moveToProject);
            Assert.AreEqual("Obowiązki", result.Then.moveToProject);
        }

        [TestMethod]
        public void SetPriorityActionIsCorrectlyRecognized()
        {
            // Arrange
            var sut = new ClassificationRulesFormatConverter();

            // Act
            ClassificationRule result = sut.Convert("if numLabelsIs(0) then setPriority(4)");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.If);
            Assert.IsNotNull(result.Then);

            Assert.IsNull(result.Then.setLabel);
            Assert.IsNull(result.Then.moveToProject);
            Assert.IsNotNull(result.Then.setPriority);
            Assert.AreEqual(4, result.Then.setPriority.Value);
        }


        [TestMethod]
        public void PriorityInActionCanBeDefinedInNaturalLanguage()
        {
            // Arrange
            var sut = new ClassificationRulesFormatConverter();

            // Act
            ClassificationRule resultUppercase = sut.Convert("if numLabelsIs(0) then setPriority(HIGH)");
            ClassificationRule resultLowercase = sut.Convert("if numLabelsIs(0) then setPriority(high)");

            // Assert
            Assert.IsNotNull(resultUppercase);
            Assert.IsNotNull(resultUppercase.If);
            Assert.IsNotNull(resultUppercase.Then);
            Assert.IsNotNull(resultLowercase);
            Assert.IsNotNull(resultLowercase.If);
            Assert.IsNotNull(resultLowercase.Then);

            Assert.IsNull(resultUppercase.Then.setLabel);
            Assert.IsNull(resultUppercase.Then.moveToProject);
            Assert.IsNotNull(resultUppercase.Then.setPriority);
            Assert.AreEqual(4, resultUppercase.Then.setPriority.Value);

            Assert.IsNull(resultLowercase.Then.setLabel);
            Assert.IsNull(resultLowercase.Then.moveToProject);
            Assert.IsNotNull(resultLowercase.Then.setPriority);
            Assert.AreEqual(4, resultLowercase.Then.setPriority.Value);
        }

        [TestMethod]
        public void MultipleActionsAreRecognizedInASingleRule()
        {
            // Arrange
            var sut = new ClassificationRulesFormatConverter();

            // Act
            ClassificationRule result =
                sut.Convert("if numLabelsIs(0) then setPriority(4) and setLabel(home) and moveToProject(Nauka)");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.If);
            Assert.IsNotNull(result.Then);

            Assert.IsNotNull(result.Then.setLabel);
            Assert.AreEqual("home", result.Then.setLabel);

            Assert.IsNotNull(result.Then.moveToProject);
            Assert.AreEqual("Nauka", result.Then.moveToProject);

            Assert.IsNotNull(result.Then.setPriority);
            Assert.AreEqual(4, result.Then.setPriority.Value);
        }

        [TestMethod]
        public void MultipleConditionsAreRecognizedInASingleRule()
        {
            // Arrange
            var sut = new ClassificationRulesFormatConverter();

            // Act
            ClassificationRule result =
                sut.Convert(
                    "if numLabelsIs(0) and projectIs(Inbox) and containsWord(aaa) and priorityIs(low) and startsWith(abc) then setPriority(4)");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.If);
            Assert.IsNotNull(result.Then);

            Assert.IsNotNull(result.If.project);
            Assert.IsNotNull(result.If.containsWord);
            Assert.IsNotNull(result.If.priority);
            Assert.IsNotNull(result.If.startsWith);
            Assert.IsNotNull(result.If.numLabels);

            Assert.AreEqual("Inbox", result.If.project);
            Assert.AreEqual("aaa", result.If.containsWord.Single());
            Assert.AreEqual(2, result.If.priority);
            Assert.AreEqual("abc", result.If.startsWith.Single());
            Assert.AreEqual(0, result.If.numLabels);
        }
    }
}