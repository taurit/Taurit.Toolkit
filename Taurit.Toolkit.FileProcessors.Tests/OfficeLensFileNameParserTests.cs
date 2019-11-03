using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Taurit.Toolkit.FileProcessors.NameProcessors;
using Taurit.Toolkit.FileProcessors.NameProcessors.NameFormatProviders;
using Match = System.Text.RegularExpressions.Match;

namespace Taurit.Toolkit.FixDateFormatInFilenames.Tests
{
    [TestClass]
    public class OfficeLensFileNameParserTests
    {
        [TestMethod]
        public void PostJune2018FilenameFormatIsRecognized()
        {
            // Arrange
            IFileNameFormatProvider formatProviderMock = new Mock<IFileNameFormatProvider>().Object;
            Debug.Assert(formatProviderMock != null);
            var sut = new ChangeOfficeLensNameProcessor(formatProviderMock);

            // Act
            Match match = sut.GetMatch("2099_06_07 11_00 dm pko bp 3.jpg");

            // Assert
            Assert.IsNotNull(match);
            Assert.IsTrue(match.Success);
            Assert.AreEqual("2099", match.Groups["year"].Value);
            Assert.AreEqual("06", match.Groups["month"].Value);
            Assert.AreEqual("07", match.Groups["day"].Value);
            Assert.AreEqual("dm pko bp 3.jpg", match.Groups["description"].Value);
        }

        [TestMethod]
        public void PreJune2018FilenameFormatIsRecognized()
        {
            // Arrange
            IFileNameFormatProvider formatProviderMock = new Mock<IFileNameFormatProvider>().Object;
            Debug.Assert(formatProviderMock != null);
            var sut = new ChangeOfficeLensNameProcessor(formatProviderMock);

            // Act
            Match match = sut.GetMatch("01.02.2018 11 00 dm pko bp 3.jpg");

            // Assert
            Assert.IsNotNull(match);
            Assert.IsTrue(match.Success);
            Assert.AreEqual("2018", match.Groups["year"].Value);
            Assert.AreEqual("02", match.Groups["month"].Value);
            Assert.AreEqual("01", match.Groups["day"].Value);
            Assert.AreEqual("dm pko bp 3.jpg", match.Groups["description"].Value);
        }

        [TestMethod]
        public void HpScanToolFilenameFormatIsRecognized()
        {
            // Arrange
            IFileNameFormatProvider formatProviderMock = new Mock<IFileNameFormatProvider>().Object;
            Debug.Assert(formatProviderMock != null);
            var sut = new ChangeOfficeLensNameProcessor(formatProviderMock);

            // Act
            Match match = sut.GetMatch("514316-080118 BUW nauka Content Security Policy v2 features notatki 1.jpg");

            // Assert
            Assert.IsNotNull(match);
            Assert.IsTrue(match.Success);
            Assert.AreEqual("18", match.Groups["year"].Value);
            Assert.AreEqual("01", match.Groups["month"].Value);
            Assert.AreEqual("08", match.Groups["day"].Value);
            Assert.AreEqual("BUW nauka Content Security Policy v2 features notatki 1.jpg", match.Groups["description"].Value);
        }
    }
}