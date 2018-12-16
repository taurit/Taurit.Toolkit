using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Taurit.Toolkit.TodoistInboxHelper.Tests
{
    [TestClass]
    public class TaskDateParserTests
    {
        [TestMethod]
        public void When_ExampleDateStringFromTodoistDocumentationIsUsed_Expect_ItToBeCorrectlyParsed()
        {
            // Arrange 
            var sut = new TaskDateParser();
            var dateString = "Mon 07 Aug 2006 12:34:56 +0000";

            // Act
            DateTime? date = sut.TryParse(dateString);

            // Assert
            Assert.IsTrue(date.HasValue);
            DateTime dateValue = date.Value;
            Assert.AreEqual(2006, dateValue.Year);
            Assert.AreEqual(08, dateValue.Month);
            Assert.AreEqual(07, dateValue.Day);
            Assert.AreEqual(12, dateValue.Hour);
            Assert.AreEqual(34, dateValue.Minute);
            Assert.AreEqual(56, dateValue.Second);
        }

        [TestMethod]
        public void When_InvalidDateStringIsUsed_Expect_NoValueInResponse()
        {
            // Arrange 
            var sut = new TaskDateParser();
            var dateString = "This is NOT a valid date string";

            // Act
            DateTime? date = sut.TryParse(dateString);

            // Assert
            Assert.IsFalse(date.HasValue);
        }

        [TestMethod]
        public void When_DateStringFromRealAPIIsUsed_Expect_ItToBeCorrectlyParsed()
        {
            // Arrange 
            var sut = new TaskDateParser();
            var dateString = "Sun 16 Dec 2018 22:59:59 +0000";

            // Act
            DateTime? date = sut.TryParse(dateString);

            // Assert
            Assert.IsTrue(date.HasValue);
            DateTime dateValue = date.Value;
            Assert.AreEqual(2018, dateValue.Year);
            Assert.AreEqual(12, dateValue.Month);
            Assert.AreEqual(16, dateValue.Day);
            Assert.AreEqual(22, dateValue.Hour);
            Assert.AreEqual(59, dateValue.Minute);
            Assert.AreEqual(59, dateValue.Second);
        }
    }
}