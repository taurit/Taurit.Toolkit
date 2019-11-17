using System;
using System.Diagnostics.CodeAnalysis;
using CommandLine;
using JetBrains.Annotations;

namespace Taurit.Toolkit.ProcessRecognizedInboxFiles
{
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    [UsedImplicitly]
    internal class Args
    {
        [Option('c', "config", Required = true,
            HelpText = "Path to the JSON configuration file (defining what to move and where).")]
        [DisallowNull]

        public String ConfigurationFilePath { get; set; }
    }
#pragma warning restore CS8618
}