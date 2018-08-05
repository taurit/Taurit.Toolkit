using System.Diagnostics;
using Moq;
using Taurit.Toolkit.FileProcessors.NameProcessors;
using Taurit.Toolkit.FileProcessors.NameProcessors.NameFormatProviders;
using Xunit;
using Match = System.Text.RegularExpressions.Match;

namespace Taurit.Toolkit.FixDateFormatInFilenames.Tests
{
    public class OfficeLensFileNameParserTests
    {
        [Fact]
        public void PostJune2018FilenameFormatIsRecognized()
        {
            // Arrange
            IFileNameFormatProvider formatProviderMock = new Mock<IFileNameFormatProvider>().Object;
            Debug.Assert(formatProviderMock != null);
            var sut = new ChangeOfficeLensNameProcessor(formatProviderMock);

            // Act
            Match match = sut.GetMatch("2099_06_07 11_00 dm pko bp 3.jpg");

            // Assert
            Assert.NotNull(match);
            Assert.True(match.Success);
            Assert.Equal("2099", match.Groups["year"].Value);
            Assert.Equal("06", match.Groups["month"].Value);
            Assert.Equal("07", match.Groups["day"].Value);
            Assert.Equal("dm pko bp 3.jpg", match.Groups["description"].Value);
        }

        [Fact]
        public void PreJune2018FilenameFormatIsRecognized()
        {
            // Arrange
            IFileNameFormatProvider formatProviderMock = new Mock<IFileNameFormatProvider>().Object;
            Debug.Assert(formatProviderMock != null);
            var sut = new ChangeOfficeLensNameProcessor(formatProviderMock);

            // Act
            Match match = sut.GetMatch("01.02.2018 11 00 dm pko bp 3.jpg");

            // Assert
            Assert.NotNull(match);
            Assert.True(match.Success);
            Assert.Equal("2018", match.Groups["year"].Value);
            Assert.Equal("02", match.Groups["month"].Value);
            Assert.Equal("01", match.Groups["day"].Value);
            Assert.Equal("dm pko bp 3.jpg", match.Groups["description"].Value);
        }
    }
}