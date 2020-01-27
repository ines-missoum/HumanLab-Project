using humanlab.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace humanlab.Helpers.Models
{
    class GridChecked
    {
        public Grid Grid { get; set; }
        public bool IsSelected { get; set; }

        public GridChecked(Grid grid, bool isSelected)
        {
            Grid = grid;
            IsSelected = isSelected;
        }
    }
}
