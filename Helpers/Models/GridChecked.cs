using humanlab.Models;
using humanlab.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace humanlab.Helpers.Models
{
    class GridChecked : BaseViewModel
    {
        public Grid Grid { get; set; }
        public bool IsSelected { get; set; }

        public int indexInListView;

        public GridChecked(Grid grid, bool isSelected, int index)
        {
            Grid = grid;
            IsSelected = isSelected;
            indexInListView = index;
        }

        public int IndexInListView
        {
            get => indexInListView;
            set => SetProperty(ref indexInListView, value, "IndexInListView");

        }
    }
}
