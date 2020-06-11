using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Taurit.Toolkit.ProcessTodoistInbox.Stats.Services;

namespace Taurit.Toolkit.ProcessTodoistInbox.Stats.Tests
{
    [TestClass]
    public class KindleMateStatsReaderTests
    {
        private const String ExampleValidStatsFileContents = @"dateUtc;numClippings;numWords
2019-11-03 13:28:58Z;2300;1087
2019-11-03 13:29:10Z;2500;1087
";

        private readonly KindleMateStatsReader _sut;

        public KindleMateStatsReaderTests()
        {
            _sut = new KindleMateStatsReader(KindleMateStatsReaderTests.ExampleValidStatsFileContents.Split(Environment.NewLine));
        }

        [TestMethod]
        public void GetEstimatedTimeNeededToProcessHighlight_WhenExactDateCanBeMatched_ExpectExceptionIsNotThrown()
        {
            // Arrange
            var exactDateTime = new DateTime(2019, 11, 03, 13, 28, 58, DateTimeKind.Utc);

            // Act
            _sut.GetEstimatedTimeNeededToProcessHighlight(exactDateTime);

            // Assert
        }

        [TestMethod]
        public void
            GetEstimatedTimeNeededToProcessHighlight_WhenExactDateCanBeMatched_ExpectAtLeastAMinuteEstimatedForEachItem()
        {
            // Arrange
            var exactDateTime = new DateTime(2019, 11, 03, 13, 28, 58, DateTimeKind.Utc);

            // Act
            TimeSpan estimate = _sut.GetEstimatedTimeNeededToProcessHighlight(exactDateTime);

            // Assert
            Assert.IsTrue(estimate.TotalMinutes >= 2300);
        }


        [TestMethod]
        public void
            GetEstimatedTimeNeededToProcessVocabularyWords_WhenExactDateCanBeMatched_ExpectExceptionIsNotThrown()
        {
            // Arrange
            var exactDateTime = new DateTime(2019, 11, 03, 13, 28, 58, DateTimeKind.Utc);

            // Act
            _sut.GetEstimatedTimeNeededToProcessVocabularyWords(exactDateTime);

            // Assert
        }

        [TestMethod]
        public void
            GetEstimatedTimeNeededToProcessVocabularyWords_WhenExactDateCanBeMatched_ExpectAtLeastAMinuteEstimatedForEachItem()
        {
            // Arrange
            var exactDateTime = new DateTime(2019, 11, 03, 13, 28, 58, DateTimeKind.Utc);

            // Act
            TimeSpan estimate = _sut.GetEstimatedTimeNeededToProcessVocabularyWords(exactDateTime);

            // Assert
            Assert.IsTrue(estimate.TotalMinutes >= 1087);
        }

        [TestMethod]
        public void
            GetEstimatedTimeNeededToProcessHighlight_WhenNoExactDateCanBeMatched_ExpectExceptionIsNotThrown() // some extrapolation is expected
        {
            // Arrange
            var dateOutOfScope = new DateTime(2018, 11, 03, 13, 28, 58, DateTimeKind.Utc);

            // Act
            _sut.GetEstimatedTimeNeededToProcessHighlight(dateOutOfScope);

            // Assert
        }

        [TestMethod]
        public void
            GetEstimatedTimeNeededToProcessVocabularyWords_WhenNoExactDateCanBeMatched_ExpectExceptionIsNotThrown() // some extrapolation is expected
        {
            // Arrange
            var dateOutOfScope = new DateTime(2018, 11, 03, 13, 28, 58, DateTimeKind.Utc);

            // Act
            _sut.GetEstimatedTimeNeededToProcessVocabularyWords(dateOutOfScope);

            // Assert
        }
    }
}