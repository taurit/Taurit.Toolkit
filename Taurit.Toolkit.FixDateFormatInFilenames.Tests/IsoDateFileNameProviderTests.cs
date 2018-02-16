using System;
using Taurit.Toolkit.FileProcessors.NameProcessors.NameFormatProviders;
using Taurit.Toolkit.FixDateFormatInFilenames.Domain;
using Xunit;

namespace Taurit.Toolkit.FixDateFormatInFilenames.Tests
{
    public class IsoDateFileNameProviderTests
    {
        [Fact]
        public void OneDigitDay_ShouldBePaddedWithZeroInOutput()
        {
            // Arrange
            var sut = new IsoDateFileNameFormatProvider();

            // Act
            String isoName1 = sut.FormatFileName("2000", "12", "01", "Test");
            String isoName2 = sut.FormatFileName("2000", "12", "1", "Test");

            // Assert
            Assert.Equal("2000-12-01 Test", isoName1);
            Assert.Equal("2000-12-01 Test", isoName2);
        }

        [Fact]
        public void OneDigitMonth_ShouldBePaddedWithZeroInOutput()
        {
            // Arrange
            var sut = new IsoDateFileNameFormatProvider();

            // Act
            String isoName1 = sut.FormatFileName("2000", "01", "11", "Test");
            String isoName2 = sut.FormatFileName("2000", "2", "11", "Test");

            // Assert
            Assert.Equal("2000-01-11 Test", isoName1);
            Assert.Equal("2000-02-11 Test", isoName2);
        }
    }
}