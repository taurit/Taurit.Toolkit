using Xunit;

namespace Taurit.Toolkit.FixDateFormatInFilenames.Tests
{
    public class FileNameFixerTests
    {
        [Fact]
        public void OneDigitDay_ShouldBePaddedWithZeroInOutput()
        {
            // Arrange
            var sut = new FileNameFixer();

            // Act
            var isoName1 = sut.GetProperFileName("2000", "12", "01", "Test");
            var isoName2 = sut.GetProperFileName("2000", "12", "1", "Test");

            // Assert
            Assert.Equal("2000-12-01 Test", isoName1);
            Assert.Equal("2000-12-01 Test", isoName2);
        }

        [Fact]
        public void OneDigitMonth_ShouldBePaddedWithZeroInOutput()
        {
            // Arrange
            var sut = new FileNameFixer();

            // Act
            var isoName1 = sut.GetProperFileName("2000", "01", "11", "Test");
            var isoName2 = sut.GetProperFileName("2000", "2", "11", "Test");

            // Assert
            Assert.Equal("2000-01-11 Test", isoName1);
            Assert.Equal("2000-02-11 Test", isoName2);
        }
    }
}