using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Taurit.Toolkit.FileProcessors.NameProcessors.NameFormatProviders;

namespace Taurit.Toolkit.FileProcessors.Tests
{
    [TestClass]
    public class IsoDateFileNameProviderTests
    {
        [TestMethod]
        public void OneDigitDay_ShouldBePaddedWithZeroInOutput()
        {
            // Arrange
            var sut = new IsoDateFileNameFormatProvider();

            // Act
            String isoName1 = sut.FormatFileName("2000", "12", "01", "Test");
            String isoName2 = sut.FormatFileName("2000", "12", "1", "Test");

            // Assert
            Assert.AreEqual("2000-12-01 Test", isoName1);
            Assert.AreEqual("2000-12-01 Test", isoName2);
        }

        [TestMethod]
        public void OneDigitMonth_ShouldBePaddedWithZeroInOutput()
        {
            // Arrange
            var sut = new IsoDateFileNameFormatProvider();

            // Act
            String isoName1 = sut.FormatFileName("2000", "01", "11", "Test");
            String isoName2 = sut.FormatFileName("2000", "2", "11", "Test");

            // Assert
            Assert.AreEqual("2000-01-11 Test", isoName1);
            Assert.AreEqual("2000-02-11 Test", isoName2);
        }

        [TestMethod]
        public void TwoDigitYear_ShouldBeAssumedToBelongTo21thCentury()
        {
            // Arrange
            var sut = new IsoDateFileNameFormatProvider();

            // Act
            String isoName1 = sut.FormatFileName("19", "01", "11", "Test");

            // Assert
            Assert.AreEqual("2019-01-11 Test", isoName1);
        }
    }
}