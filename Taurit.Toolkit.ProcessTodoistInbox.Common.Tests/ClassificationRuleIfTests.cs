using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Taurit.Toolkit.ProcesTodoistInbox.Common.Models.Classification;
using Taurit.Toolkit.TodoistInboxHelper.ApiModels;

#pragma warning disable 618 // I'm not using constructor for ClassificationRuleIf to make tests brief

namespace Taurit.Toolkit.ProcessTodoistInbox.Common.Tests
{
    [TestClass]
    public class ClassificationRuleIfTests
    {
        [TestMethod]
        public void EmptyRuleMatchesAnything()
        {
            // Arrange
            var sut = new ClassificationRuleIf();
            var task = new TodoTask {content = "hello WORLD"};

            // Act
            Boolean match = sut.Matches(task);

            // Assert
            Assert.IsTrue(match);
        }

        [TestMethod]
        public void ContainsIsCaseInsensitive()
        {
            // Arrange
            var sut = new ClassificationRuleIf {contains = new[] {"world"}};
            var task = new TodoTask {content = "hello WORLD"};

            // Act
            Boolean match = sut.Matches(task);

            // Assert
            Assert.IsTrue(match);
        }

        [TestMethod]
        public void ContainsIgnoresDiacritics()
        {
            // Arrange
            var sut = new ClassificationRuleIf {contains = new[] {"Umyć"}};
            var task = new TodoTask {content = "umyc"};

            // Act
            Boolean match = sut.Matches(task);

            // Assert
            Assert.IsTrue(match);
        }

        [TestMethod]
        public void ItIsEnoughForOneElementInContainsListToMatch()
        {
            // Arrange
            var sut = new ClassificationRuleIf {contains = new[] {"hola", "world"}};
            var task = new TodoTask {content = "hello WORLD"};

            // Act
            Boolean match = sut.Matches(task);

            // Assert
            Assert.IsTrue(match);
        }

        [TestMethod]
        public void ContainsReturnsFalseWhenTheresNoMatch()
        {
            // Arrange
            var sut = new ClassificationRuleIf {contains = new[] {"world"}};
            var task = new TodoTask {content = "yo dawg"};

            // Act
            Boolean match = sut.Matches(task);

            // Assert
            Assert.IsFalse(match);
        }

        [TestMethod]
        public void StartsWithWordReturnsFalseWhenWordIsNotAFirstWord()
        {
            // Arrange
            var sut = new ClassificationRuleIf {startsWith = new[] {"Buy"}};
            var task = new TodoTask {content = "Do not buy a TV, ever!"};

            // Act
            Boolean match = sut.Matches(task);

            // Assert
            Assert.IsFalse(match);
        }

        [TestMethod]
        public void ContainsWordReturnsTrueEvenWhenWordIsNotAFirstWord()
        {
            // Arrange
            var sut = new ClassificationRuleIf {containsWord = new[] {"xyz", "Buy"}};
            var task = new TodoTask {content = "Do not buy a TV, ever!"};

            // Act
            Boolean match = sut.Matches(task);

            // Assert
            Assert.IsTrue(match);
        }

        [TestMethod]
        public void ContainsWordReturnsFalseWhenWordIsNotFound()
        {
            // Arrange
            var sut = new ClassificationRuleIf {containsWord = new[] {"xyz", "zyx"}};
            var task = new TodoTask {content = "Do not buy a TV, ever!"};

            // Act
            Boolean match = sut.Matches(task);

            // Assert
            Assert.IsFalse(match);
        }

        [TestMethod]
        public void StartsWithWordReturnsTrueWhenWordIsAFirstWord()
        {
            // Arrange
            var sut = new ClassificationRuleIf {startsWith = new[] {"Buy"}};
            var task = new TodoTask {content = "Buy milk"};

            // Act
            Boolean match = sut.Matches(task);

            // Assert
            Assert.IsTrue(match);
        }

        [TestMethod]
        public void StartsWithWordIsNotCaseSensitive()
        {
            // Arrange
            var sut = new ClassificationRuleIf {startsWith = new[] {"bUy"}};
            var task = new TodoTask {content = "BUY milk"};

            // Act
            Boolean match = sut.Matches(task);

            // Assert
            Assert.IsTrue(match);
        }

        [TestMethod]
        public void StartsWithWordIgnoresDifferenceInNationalCharacters()
        {
            // Arrange
            var sut = new ClassificationRuleIf {startsWith = new[] {"Zapytać"}};
            var task = new TodoTask {content = "Zapytac telefonicznie o cene"};

            // Act
            Boolean match = sut.Matches(task);

            // Assert
            Assert.IsTrue(match);
        }

        [TestMethod]
        public void IfPriorityDoesNotMatchThereIsNoMatch()
        {
            // Arrange
            var sut = new ClassificationRuleIf {priority = 3};
            var task = new TodoTask {priority = 1};

            // Act
            Boolean match = sut.Matches(task);

            // Assert
            Assert.IsFalse(match);
        }

        [TestMethod]
        public void IfPriorityMatchesThereIsAMatch()
        {
            // Arrange
            var sut = new ClassificationRuleIf {priority = 3};
            var task = new TodoTask {priority = 3};

            // Act
            Boolean match = sut.Matches(task);

            // Assert
            Assert.IsTrue(match);
        }

        [TestMethod]
        public void ProjectCanBeMatchedByNameCaseInsensitive()
        {
            // Arrange
            var sut = new ClassificationRuleIf {project = "Inbox"};
            var task = new TodoTask {project_name = "inbox"};

            // Act
            Boolean match = sut.Matches(task);

            // Assert
            Assert.IsTrue(match);
        }

        [TestMethod]
        public void WhenProjectDoesNotMatchTaskDoesNotMatch()
        {
            // Arrange
            var sut = new ClassificationRuleIf {project = "Inbox"};
            var task = new TodoTask {project_name = "Obowiązki"};

            // Act
            Boolean match = sut.Matches(task);

            // Assert
            Assert.IsFalse(match);
        }

        [TestMethod]
        public void RuleOfNoLabelsDoesNotMatchWithTaskWithOneLabel()
        {
            // Arrange
            var sut = new ClassificationRuleIf {numLabels = 0};
            var task = new TodoTask {labels = new List<Int64> {123}}; // one label with id "123"

            // Act
            Boolean match = sut.Matches(task);

            // Assert
            Assert.IsFalse(match);
        }

        [TestMethod]
        public void RuleOfUndefinedDurationDoesNotMatchWithTaskWithDefinedDuration()
        {
            // Arrange
            var sut = new ClassificationRuleIf {duration = "undefined"};
            var task = new TodoTask {content = "buy milk (10 minutes)"};

            // Act
            Boolean match = sut.Matches(task);

            // Assert
            Assert.IsFalse(match);
        }

        
        [TestMethod]
        public void RuleOfUndefinedDurationDoesMatchWithTaskWithUndefinedDuration()
        {
            // Arrange
            var sut = new ClassificationRuleIf {duration = "undefined"};
            var task = new TodoTask {content = "buy milk"};

            // Act
            Boolean match = sut.Matches(task);

            // Assert
            Assert.IsTrue(match);
        }

        
        [TestMethod]
        public void RuleOfDefinedDurationDoesMatchWithTaskWithDefinedSameDuration()
        {
            // Arrange
            var sut = new ClassificationRuleIf {duration = "about 5 minutes"};
            var task = new TodoTask {content = "buy milk (5 min)"};

            // Act
            Boolean match = sut.Matches(task);

            // Assert
            Assert.IsTrue(match);
        }

        
        [TestMethod]
        public void RuleOfDefinedDurationDoesNotMatchWithTaskWithDefinedDifferentDuration()
        {
            // Arrange
            var sut = new ClassificationRuleIf {duration = "about 5 minutes"};
            var task = new TodoTask {content = "buy milk (7 min)"};

            // Act
            Boolean match = sut.Matches(task);

            // Assert
            Assert.IsFalse(match);
        }
    }
}