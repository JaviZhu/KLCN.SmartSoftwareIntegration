using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class HardDisk
    {
        public int ID { get; set; }
        public string BarCode { get; set; }
        public string Bank { get; set; }
        public string Tag { get; set; }
        public string Category { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
