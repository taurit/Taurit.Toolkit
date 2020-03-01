using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Taurit.Toolkit.WeightMonitor.Common.Models;

namespace Taurit.Toolkit.WeightMonitor.Common.Tests
{
    [TestClass]
    public class TrainingPeriodTests
    {
        [TestMethod]
        public void When_BulkingPeriodIsTrimmed_Expect_TargetWeightIsLower()
        {
            // Arrange
            var bulkingPeriod = new BulkingPeriod(new DateTime(2020, 01, 01), new DateTime(2020, 01, 30), 90.0);
            Double expectedWeight = bulkingPeriod.ExpectedOptimumEndWeight;

            // Act
            bulkingPeriod.Trim(new DateTime(2020, 01, 14));

            // Assert
            Assert.IsTrue(bulkingPeriod.ExpectedMaximumEndWeight < expectedWeight);
        }

        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        [TestMethod]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
        public void When_CuttingPeriodIsConstructed_Expect_EndDateMustBeAfterStartDate()
        {
            // Arrange, Act, Assert
            new CuttingPeriod(new DateTime(2020, 01, 01), new DateTime(1990, 01, 01), 90.0);
        }

        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        [TestMethod]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
        public void When_BulkingPeriodIsConstructed_Expect_EndDateMustBeAfterStartDate()
        {
            // Arrange, Act, Assert
            new BulkingPeriod(new DateTime(2020, 01, 01), new DateTime(1990, 01, 01), 90.0);
        }

        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        [TestMethod]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
        public void When_MaintenancePeriodIsConstructed_Expect_EndDateMustBeAfterStartDate()
        {
            // Arrange, Act, Assert
            new MaintenancePeriod(new DateTime(2020, 01, 01), new DateTime(1990, 01, 01), 90.0);
        }

        [TestMethod]
        public void When_BulkingPeriodIsConstructed_Expect_TargetWeightIsHigherOnTheEndDateThanOnTheStartDate()
        {
            // Arrange
            var startWeight = 90.0;
            var bulkingPeriod = new BulkingPeriod(new DateTime(2020, 01, 01), new DateTime(2020, 01, 30), startWeight);

            // Act
            Double minimumExpectedWeight = bulkingPeriod.ExpectedMinimumEndWeight;
            Double optimumExpectedWeight = bulkingPeriod.ExpectedOptimumEndWeight;
            Double maximumExpectedWeight = bulkingPeriod.ExpectedMaximumEndWeight;

            // Assert
            Assert.IsTrue(minimumExpectedWeight > startWeight);
            Assert.IsTrue(optimumExpectedWeight > startWeight);
            Assert.IsTrue(maximumExpectedWeight > startWeight);

            Assert.IsTrue(maximumExpectedWeight > optimumExpectedWeight);
            Assert.IsTrue(optimumExpectedWeight > minimumExpectedWeight);
        }

        [TestMethod]
        public void When_CuttingPeriodIsConstructed_Expect_TargetWeightIsLowerOnTheEndDateThanOnTheStartDate()
        {
            // Arrange
            var startWeight = 90.0;
            var cuttingPeriod = new CuttingPeriod(new DateTime(2020, 01, 01), new DateTime(2020, 01, 30), startWeight);

            // Act
            Double minimumExpectedWeight = cuttingPeriod.ExpectedMinimumEndWeight;
            Double optimumExpectedWeight = cuttingPeriod.ExpectedOptimumEndWeight;
            Double maximumExpectedWeight = cuttingPeriod.ExpectedMaximumEndWeight;

            // Assert
            Assert.IsTrue(minimumExpectedWeight < startWeight);
            Assert.IsTrue(optimumExpectedWeight < startWeight);
            Assert.IsTrue(maximumExpectedWeight < startWeight);

            Assert.IsTrue(maximumExpectedWeight > optimumExpectedWeight);
            Assert.IsTrue(optimumExpectedWeight > minimumExpectedWeight);
        }

    }
}