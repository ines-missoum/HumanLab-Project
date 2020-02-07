using humanlab.Models;
using humanlab.ViewModels;
using Prism.Commands;
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
        public DelegateCommand<object> ShowPreviewDelegate { get; set; }

        public GridChecked(Grid grid, bool isSelected, int index, DelegateCommand<object> showPreviewDelegate)
        {
            Grid = grid;
            IsSelected = isSelected;
            indexInListView = index;
            ShowPreviewDelegate = showPreviewDelegate;
        }

        public int IndexInListView
        {
            get => indexInListView;
            set => SetProperty(ref indexInListView, value, "IndexInListView");

        }
    }
}
