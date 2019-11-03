namespace Taurit.Toolkit.KindleMateBacklogMonitor.Database
{
    public partial class Clippings
    {
        public string Key { get; set; }
        public string Content { get; set; }
        public string Bookname { get; set; }
        public string Authorname { get; set; }
        public long? Brieftype { get; set; }
        public string Clippingtypelocation { get; set; }
        public string Clippingdate { get; set; }
        public long? Read { get; set; }
        public string ClippingImportdate { get; set; }
        public string Tag { get; set; }
        public long? Sync { get; set; }
        public string Newbookname { get; set; }
        public long? ColorRgb { get; set; }
        public long? Pagenumber { get; set; }
    }
}
