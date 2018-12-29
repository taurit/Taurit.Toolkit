using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Taurit.Toolkit.ProcessTodoistInbox.Stats.Models;

namespace Taurit.Toolkit.ProcessTodoistInbox.Stats.Tests
{
    [TestClass]
    public class SnapshotOnTimelineTests
    {
        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method)]
        public void When_EndOfQuarterPropertyIsRequested_Expect_ItIsValidForSnapshotDate(
            DateTime givenDate, DateTime expectedEndOfQuarter)
        {
            // Act
            var sut = new SnapshotOnTimeline(givenDate, null, null, null);

            // Assert
            Assert.AreEqual(expectedEndOfQuarter, sut.EndOfQuarter);
        }

        public static IEnumerable<Object[]> GetData()
        {
            yield return new Object[]
            {
                new DateTime(2018, 12, 30, 23, 37, 0),
                new DateTime(2019, 1, 1, 0, 0, 0)
            };

            yield return new Object[]
            {
                new DateTime(2018, 10, 1, 0, 0, 1),
                new DateTime(2019, 1, 1, 0, 0, 0)
            };

            yield return new Object[]
            {
                new DateTime(2018, 12, 31, 23, 59, 59),
                new DateTime(2019, 1, 1, 0, 0, 0)
            };

            yield return new Object[]
            {
                new DateTime(2018, 7, 15, 23, 59, 59),
                new DateTime(2018, 10, 1, 0, 0, 0)
            };
        }
    }
}