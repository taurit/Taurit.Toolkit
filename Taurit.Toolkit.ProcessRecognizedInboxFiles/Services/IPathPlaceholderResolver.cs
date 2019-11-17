using System;

namespace Taurit.Toolkit.ProcessRecognizedInboxFiles.Services
{
    /// <summary>
    ///     Transforms folder path that might contain string placeholders into a real filesystem path.
    /// </summary>
    /// <example>
    ///     var result = Resolve("d://Photos//%YEAR%//WhatsApp");
    ///     Assert.IsTrue("d://Photos//2019//WhatsApp", result);
    /// </example>
    public interface IPathPlaceholderResolver
    {
        String Resolve(String templatePath);
    }
}