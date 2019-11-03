using System;

namespace Taurit.Toolkit.KindleMateBacklogMonitor.Database
{
    public class Clippings
    {
        public String Key { get; set; }
        public String Content { get; set; }
        public String Bookname { get; set; }
        public String Authorname { get; set; }
        public Int64? Brieftype { get; set; }
        public String Clippingtypelocation { get; set; }
        public String Clippingdate { get; set; }
        public Int64? Read { get; set; }
        public String ClippingImportdate { get; set; }
        public String Tag { get; set; }
        public Int64? Sync { get; set; }
        public String Newbookname { get; set; }
        public Int64? ColorRgb { get; set; }
        public Int64? Pagenumber { get; set; }
    }
}