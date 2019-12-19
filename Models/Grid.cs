using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace humanlab.Models
{
    class Grid
    {
        public int IdGrid { get; set; }
        public string GridName { get; set; }
        public int ElementsSize { get; set; }

        public ICollection<Activity> Activities { get; } = new List<Activity>();
        public ICollection<Element> Elements { get; } = new List<Element>();
    }
}
