namespace Taurit.Toolkit.KindleMateBacklogMonitor.Database
{
    public partial class Vocab
    {
        public string Id { get; set; }
        public string WordKey { get; set; }
        public string Word { get; set; }
        public string Stem { get; set; }
        public long? Category { get; set; }
        public string Translation { get; set; }
        public string Timestamp { get; set; }
        public long? Frequency { get; set; }
        public long? Sync { get; set; }
        public long? ColorRgb { get; set; }
    }
}
