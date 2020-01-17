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
        private bool searching;
        private bool selectionChangedFirstGridView;
        private GridView searchedGridView;

        public GridFormViewModel()
        {
            AllElements = new List<ElementChecked>();
            AllElements.Add(new ElementChecked(new Element { ElementName = "hello" }, false));
            AllElements.Add(new ElementChecked(new Element { ElementName = "salut" }, false));
            AllElements.Add(new ElementChecked(new Element { ElementName = "hola" }, true));
            AllElements.Add(new ElementChecked(new Element { ElementName = "test" }, false));

            SearchedElements = new List<ElementChecked>(AllElements);

            SelectedElements = SearchedElements.Where(e => (e.IsSelected == true)).ToList();

            searching = false;

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
                if (value != selectedElements)
                {
                    selectedElements = value;
                    OnPropertyChanged("SelectedElements");
                }
            }
        }

        public void GridView1_Loading(FrameworkElement sender, object args)
        {
            GridView gridview1 = sender as GridView;
            searchedGridView = gridview1;
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
            Debug.WriteLine("GridView1_SelectionChanged");
            selectionChangedFirstGridView = true;
            if (!searching)
            {
                List<ElementChecked> updatedList = new List<ElementChecked>(SelectedElements);
                Debug.WriteLine(e.AddedItems.Count() + " " + e.RemovedItems.Count());

                if (e.AddedItems.Count() > 0)
                {
                    ElementChecked addedItem = e.AddedItems.First() as ElementChecked;
                    if (!SelectedElements.Contains(addedItem))
                    {
                        updatedList.Add(addedItem);
                        addedItem.IsSelected = true;
                        SelectedElements = updatedList;
                    }

                }
                else
                {
                    if (e.RemovedItems.Count() > 0)
                    {
                        ElementChecked removedItem = e.RemovedItems.First() as ElementChecked;
                        updatedList.Remove(removedItem);
                        removedItem.IsSelected = false;
                        SelectedElements = updatedList;
                    }

                }
            }
            selectionChangedFirstGridView = false;
        }

        public void GridView2_Loading(FrameworkElement sender, object args)
        {
            GridView gridview2 = sender as GridView;
            gridview2.SelectAll();
        }

        public void GridView2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GridView gridview = sender as GridView;

            if (!selectionChangedFirstGridView && e.RemovedItems.Count() == 1)
            {
                ElementChecked removedItem = e.RemovedItems.First() as ElementChecked;
                searchedGridView.SelectedItems.Remove(removedItem);
            }


            gridview.SelectAll();
        }

        public void GridView2_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            GridView gridview = sender as GridView;
            gridview.SelectAll();
        }

        public void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            searching = true;
            string text = sender.Text;
            List<ElementChecked> newSearchedNames = new List<ElementChecked>(AllElements);
            SearchedElements = newSearchedNames.Where(e => e.Element.ElementName.Contains(text)).ToList();

            foreach (ElementChecked item in SearchedElements)
            {
                if (item.IsSelected)
                {
                    searchedGridView.SelectedItems.Add(item);
                }
            }

            searching = false;
        }

    }
}
