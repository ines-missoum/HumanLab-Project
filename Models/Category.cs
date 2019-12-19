using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace humanlab.Models
{
    class Category
    {
        public int IdCategory { get; set; }
        public string CategoryName { get; set; }

        public List<Element> Elements { get; set; }
    }
}
