using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class EBook
    {
        public string BookName { get; set; }
        public string Picture { get; set; }
        public string Path { get; set; }
        public DateTime? Datetime { get; set; }
        public int ID { get; set; }
        public string Author { get; set; }
        public string Classification { get; set; }
    }
}
