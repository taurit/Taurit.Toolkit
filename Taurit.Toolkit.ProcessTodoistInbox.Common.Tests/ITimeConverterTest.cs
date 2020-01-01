using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Taurit.Toolkit.ProcessTodoistInbox.Common.Services;

namespace Taurit.Toolkit.ProcessTodoistInbox.Common.Tests
{
    [TestClass]
    public class TimeConverterTest
    {
        // Arrange
        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method)]
        public void When_VariousTimeSpansAreConvertedToUnitOfWork_Expect_CorrectResult(
            TimeSpan input,
            Int32 expectedOutput
        )
        {
            // Arrange
            var sut = new TimeConverter();

            // Act
            Int32 output = sut.ConvertToUnitsOfWorkAndCeil(input);

            // Assert
            Assert.AreEqual(expectedOutput, output);
        }


        private static IEnumerable<Object[]> GetData()
        {
            yield return new Object[] { TimeSpan.FromMinutes(0), 0 };

            yield return new Object[] { TimeSpan.FromMinutes(45), 1 };
            yield return new Object[] { TimeSpan.FromMinutes(44), 1 };
            yield return new Object[] { TimeSpan.FromMinutes(1), 1 };

            yield return new Object[] { new TimeSpan(0, 0, 45, 1), 2 };
            yield return new Object[] { new TimeSpan(0, 0, 46, 0), 2 };
            yield return new Object[] { new TimeSpan(0, 0, 90, 0), 2 };

            yield return new Object[] { new TimeSpan(0, 0, 90, 1), 3 };
            
        }
    }
}