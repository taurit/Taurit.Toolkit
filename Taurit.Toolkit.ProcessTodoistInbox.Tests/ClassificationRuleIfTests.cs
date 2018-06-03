using System;
using System.Collections.Generic;
using Taurit.Toolkit.ProcessTodoistInbox.Models.Classification;
using Taurit.Toolkit.TodoistInboxHelper.ApiModels;
using Xunit;

namespace Taurit.Toolkit.ProcessTodoistInbox.Tests
{
    public class ClassificationRuleIfTests
    {
        [Fact]
        public void EmptyRuleMatchesAnything()
        {
            // Arrange
            var sut = new ClassificationRuleIf();
            var task = new TodoTask {content = "hello WORLD"};

            // Act
            Boolean match = sut.Matches(task);

            // Assert
            Assert.True(match);
        }

        [Fact]
        public void ContainsIsCaseInsensitive()
        {
            // Arrange
            var sut = new ClassificationRuleIf {taskContains = "world"};
            var task = new TodoTask {content = "hello WORLD"};

            // Act
            Boolean match = sut.Matches(task);

            // Assert
            Assert.True(match);
        }

        [Fact]
        public void ContainsReturnsFalseWhenTheresNoMatch()
        {
            // Arrange
            var sut = new ClassificationRuleIf {taskContains = "world"};
            var task = new TodoTask {content = "yo dawg"};

            // Act
            Boolean match = sut.Matches(task);

            // Assert
            Assert.False(match);
        }

        [Fact]
        public void StartsWithWordReturnsFalseWhenWordIsNotAFirstWord()
        {
            // Arrange
            var sut = new ClassificationRuleIf {taskStartsWithWord = new [] { "Buy" }};
            var task = new TodoTask {content = "Do not buy a TV, ever!"};

            // Act
            Boolean match = sut.Matches(task);

            // Assert
            Assert.False(match);
        }

        [Fact]
        public void StartsWithWordReturnsTrueWhenWordIsAFirstWord()
        {
            // Arrange
            var sut = new ClassificationRuleIf {taskStartsWithWord = new [] { "Buy" }};
            var task = new TodoTask {content = "Buy milk"};

            // Act
            Boolean match = sut.Matches(task);

            // Assert
            Assert.True(match);
        }

        [Fact]
        public void StartsWithWordIsNotCaseSensitive()
        {
            // Arrange
            var sut = new ClassificationRuleIf {taskStartsWithWord = new [] { "bUy" }};
            var task = new TodoTask {content = "BUY milk"};

            // Act
            Boolean match = sut.Matches(task);

            // Assert
            Assert.True(match);
        }

        [Fact]
        public void StartsWithWordIgnoresDifferenceInNationalCharacters()
        {
            // Arrange
            var sut = new ClassificationRuleIf {taskStartsWithWord = new [] { "Zapytać" }};
            var task = new TodoTask {content = "Zapytac telefonicznie o cene"};

            // Act
            Boolean match = sut.Matches(task);

            // Assert
            Assert.True(match);
        }

        [Fact]
        public void IfPriorityDoesNotMatchThereIsNoMatch()
        {
            // Arrange
            var sut = new ClassificationRuleIf {taskHasPriority = 3 };
            var task = new TodoTask {priority = 1};

            // Act
            Boolean match = sut.Matches(task);

            // Assert
            Assert.False(match);
        }

        [Fact]
        public void IfPriorityMatchesThereIsAMatch()
        {
            // Arrange
            var sut = new ClassificationRuleIf {taskHasPriority = 3 };
            var task = new TodoTask {priority = 3};

            // Act
            Boolean match = sut.Matches(task);

            // Assert
            Assert.True(match);
        }

        [Fact]
        public void ProjectCanBeMatchedByNameCaseInsensitive()
        {
            // Arrange
            var sut = new ClassificationRuleIf { taskProject = "Inbox" };
            var task = new TodoTask {project_name = "inbox"};

            // Act
            Boolean match = sut.Matches(task);

            // Assert
            Assert.True(match);
        }

        [Fact]
        public void WhenProjectDoesNotMatchTaskDoesNotMatch()
        {
            // Arrange
            var sut = new ClassificationRuleIf { taskProject = "Inbox" };
            var task = new TodoTask {project_name = "Obowiązki"};

            // Act
            Boolean match = sut.Matches(task);

            // Assert
            Assert.False(match);
        }

        [Fact]
        public void RuleOfNoLabelsDoesNotMatchWithTaskWithOneLabel()
        {
            // Arrange
            var sut = new ClassificationRuleIf { taskHasLabels = 0 };
            var task = new TodoTask { labels = new List<Int64>() { 123 }}; // one label with id "123"

            // Act
            Boolean match = sut.Matches(task);

            // Assert
            Assert.False(match);
        }
    }
}