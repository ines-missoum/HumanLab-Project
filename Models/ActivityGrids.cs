using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace humanlab.Models
{
    public class ActivityGrids
    {
        public int ActivityId { get; set; }
        public Activity Activity { get; set; }

        public int GridId { get; set; }
        public Grid Grid { get; set; }
        public int Order { get; set; }
    }
}
