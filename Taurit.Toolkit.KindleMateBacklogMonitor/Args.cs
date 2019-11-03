using System;
using CommandLine;

namespace Taurit.Toolkit.KindleMateBacklogMonitor
{
    public class Args
    {
        [Option('d', "database", Required = true, HelpText = "Path to a SQL database file")]
        public String DatabasePath { get; set; }
    }
}