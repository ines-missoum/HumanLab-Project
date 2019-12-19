using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace humanlab.Models
{
    class Element
    {
        public int IdElement { get; set; }
        public string ElementName { get; set; }
        public string Image { get; set; }
        public string Audio { get; set; }
        public string SpeachText { get; set; }

        public int CategoryId { get; }
        public Category Category { get; }
        public ICollection<Grid> Grids { get; } = new List<Grid>();
    }
}
