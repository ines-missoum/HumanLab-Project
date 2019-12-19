using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace humanlab.Models
{
    public class GridElements
    {
        public int GridId { get; set; }
        public Grid Grid { get; set; }

        public int ElementId { get; set; }
        public Element Element { get; set; }

        public int Xposition { get; set; }
        public int Yposition { get; set; }
    }
}
