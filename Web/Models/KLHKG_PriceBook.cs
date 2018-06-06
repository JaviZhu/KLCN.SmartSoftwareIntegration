using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class KLHKG_PriceBook
    {
        public string KLHKG_PriceBookID { get; set; }
        public string UniqueCode { get; set; }
        public string CustType { get; set; }
        public string SKU { get; set; }
        public string Currency { get; set; }
        public decimal Qty { get; set; }
        public decimal ListPrice { get; set; }
        public decimal MgrPrice { get; set; }
        public decimal SDPrice { get; set; }
        public int ID { get; set; }
    }
}
