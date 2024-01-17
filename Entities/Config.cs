using System;
using System.Collections.Generic;
using System.Text;

namespace Mini.Entities
{
    public class Config
    {
        public string Engine { get; set; }
        public string ConString { get; set; }
        public string Query { get; set; }
        public string DashEndpoint { get; set; }
        public string GoaEndpoint { get; set; }
        public string Path { get; set; }
        public string SheetName { get; set; }
        public string HeaderCode { get; set; }
        public string HeaderName { get; set; }
        public string HeaderQty { get; set; }
        public string HeaderPrice { get; set; }
        public string Delimiter { get; set; }
        public bool with2digit { get; set; }
        public bool QtyDigit { get; set; }
    }
}
