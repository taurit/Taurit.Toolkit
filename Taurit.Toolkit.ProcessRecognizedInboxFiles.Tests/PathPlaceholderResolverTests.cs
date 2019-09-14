using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Taurit.Toolkit.ProcessRecognizedInboxFiles.Services;

namespace Taurit.Toolkit.ProcessRecognizedInboxFiles.Tests
{
    [TestClass]
    public class PathPlaceholderResolverTests
    {
        private readonly IPathPlaceholderResolver _sut = new PathPlaceholderResolver();


        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        public void When_NullIsPassedAsPath_Expect_Exception()
        {
            // Arrange

            // Act
            String resolvedPath = _sut.Resolve(null);

            // Assert
        }

        [TestMethod]
        public void When_PathDoesNotContainAnyTemplate_Expect_NothingIsChanged()
        {
            // Arrange
            var path = "d://Photos//SmartphonePhotos";

            // Act
            String resolvedPath = _sut.Resolve(path);

            // Assert
            Assert.AreEqual(path, resolvedPath);
        }

        [TestMethod]
        public void When_PathDoesContainYearTemplate_Expect_YearIsChangedToCurrentYear()
        {
            // Arrange
            var path = "d://Photos//%YEAR%//SmartphonePhotos";

            // Act
            String resolvedPath = _sut.Resolve(path);

            // Assert
            Assert.AreEqual($"d://Photos//{DateTime.UtcNow.Year}//SmartphonePhotos", resolvedPath);
        }
    }
}