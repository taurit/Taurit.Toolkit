using System;

namespace Taurit.Toolkit.ProcessTodoistInbox.Stats
{
    // todo move to some external configuration file
    internal class StatsAppSettings
    {
        public String SnapshotsRootFolderPath =>
            @"f:\\Mirrors\\RaspberryPiWindows10\\"; // SSD path is much faster than network path to Raspberry Pi

        /// <summary>
        /// Only every N-th snapshot will be read, this property's value being N
        /// </summary>
        public static Int32 ReductionRatio => 8;
    }
}