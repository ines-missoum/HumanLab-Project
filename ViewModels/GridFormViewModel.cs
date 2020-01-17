using humanlab.Helpers.Models;
using humanlab.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace humanlab.ViewModels
{
    public class GridFormViewModel : BaseViewModel
    {

        private List<ElementChecked> searchedElements;
        private List<ElementChecked> selectedElements;
        private List<ElementChecked> allElements;

        public GridFormViewModel()
        {
            SearchedElements = new List<ElementChecked>();
            SearchedElements.Add(new ElementChecked(new Element { ElementName = "hello" }, false));
            SearchedElements.Add(new ElementChecked(new Element { ElementName = "salut" }, false));
            SearchedElements.Add(new ElementChecked(new Element { ElementName = "hola" }, true));
            SearchedElements.Add(new ElementChecked(new Element { ElementName = "test" }, false));

            SelectedElements = SearchedElements.Where(e => (e.IsSelected == true)).ToList();

        }

        public List<ElementChecked> AllElements
        {
            get => allElements;
            set
            {
                if (value != allElements)
                {
                    allElements = value;
                    OnPropertyChanged("AllElements");
                }
            }
        }
        public List<ElementChecked> SearchedElements
        {
            get => searchedElements;
            set
            {
                if (value != searchedElements)
                {
                    searchedElements = value;
                    OnPropertyChanged("SearchedElements");
                }
            }
        }
        public List<ElementChecked> SelectedElements
        {
            get => selectedElements;
            set
            {
                Debug.WriteLine("setter");
                if (value != selectedElements)
                {
                    Debug.WriteLine("changed");
                    selectedElements = value;
                    OnPropertyChanged("SelectedElements");
                }
            }
        }

        public void GridView1_Loading(FrameworkElement sender, object args)
        {
            GridView gridview1 = sender as GridView;
            foreach (ElementChecked item in SearchedElements)
            {
                if (item.IsSelected)
                {
                    gridview1.SelectedItems.Add(item);
                }
            }
        }

        public void GridView1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Debug.WriteLine("enter");
            List<ElementChecked> updatedList = new List<ElementChecked>(SelectedElements);

            if (e.AddedItems.Count() > 0)
            {
                Debug.WriteLine("add");
                ElementChecked addedItem = e.AddedItems.First() as ElementChecked;
                if (!SelectedElements.Contains(addedItem))
                    updatedList.Add(addedItem);
            }
            else
            {
                Debug.WriteLine("remove");
                ElementChecked removedItem = e.RemovedItems.First() as ElementChecked;
                updatedList.Remove(removedItem);
            }
            SelectedElements = updatedList;
        }

        public void GridView2_Loading(FrameworkElement sender, object args)
        {
            GridView gridview2 = sender as GridView;
            gridview2.SelectAll();
        }

        public void GridView2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GridView gridview = sender as GridView;
            gridview.SelectAll();
        }

        public void GridView2_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            GridView gridview = sender as GridView;
            gridview.SelectAll();
        }

    }
}
