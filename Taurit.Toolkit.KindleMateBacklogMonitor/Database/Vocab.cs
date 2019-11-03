using System;

namespace Taurit.Toolkit.KindleMateBacklogMonitor.Database
{
    public class Vocab
    {
        public String Id { get; set; }
        public String WordKey { get; set; }
        public String Word { get; set; }
        public String Stem { get; set; }
        public Int64? Category { get; set; }
        public String Translation { get; set; }
        public String Timestamp { get; set; }
        public Int64? Frequency { get; set; }
        public Int64? Sync { get; set; }
        public Int64? ColorRgb { get; set; }
    }
}