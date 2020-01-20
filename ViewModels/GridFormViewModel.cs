using humanlab.DAL;
using humanlab.Helpers.Models;
using humanlab.Models;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace humanlab.ViewModels
{
    public class GridFormViewModel : BaseViewModel
    {
        /*public attributes*/
        private List<ElementChecked> searchedElements;
        private List<ElementChecked> selectedElements;
        private List<ElementChecked> allElements;
        private bool isChooseElementsOpened;
        private bool isNextButtonShowing;
        public DelegateCommand ChoosePopUpVisibility { get; set; }
        private string buttonText;
        private List<string> categories;
        private bool isEmptySearchMessageShowing;
        private bool isEmptyElementMessageShowing;

        /*private attributes*/
        private bool searching;
        private bool selectionChangedFirstGridView;
        private GridView searchedGridView;
        private Repository repository;
        private string searchText { get; set; }
        private string searchCategory { get; set; }

        public GridFormViewModel()
        {
            repository = new Repository();
            InitialiseAllElements();
            SearchedElements = new List<ElementChecked>(AllElements.OrderByDescending(e => e.Element.ElementName.Length));
            SelectedElements = new List<ElementChecked>();
            searching = false;
            //pop up closed at the beginning
            isChooseElementsOpened = false;
            ChoosePopUpVisibility = new DelegateCommand(ChangeChoosePopUpVisibility);
            ButtonText = "Choisir";
            IsNextButtonShowing = false;
            searchText = "";
            searchCategory = "Tout";
            isEmptySearchMessageShowing = false;
            isEmptyElementMessageShowing = true;
        }

        private void ChangeChoosePopUpVisibility()
        {
            IsChooseElementsOpened = !IsChooseElementsOpened;
        }

        private async void InitialiseAllElements()
        {
            var elements = await repository.GetElementsAsync();
            AllElements = new List<ElementChecked>();
            elements.ForEach(e => AllElements.Add(new ElementChecked(e, false)));
            Categories = elements.Select(e => e.Category.CategoryName).Distinct().ToList();
            Categories.Add("Tout");
        }

        public string ButtonText
        {
            get => buttonText;
            set
            {
                if (value != buttonText)
                {
                    buttonText = value;
                    OnPropertyChanged("ButtonText");
                }
            }
        }
        public bool IsEmptyElementMessageShowing
        {
            get => isEmptyElementMessageShowing;
            set
            {
                if (value != isEmptyElementMessageShowing)
                {
                    isEmptyElementMessageShowing = value;
                    OnPropertyChanged("IsEmptyElementMessageShowing");
                }
            }
        }
        public bool IsEmptySearchMessageShowing
        {
            get => isEmptySearchMessageShowing;
            set
            {
                if (value != isEmptySearchMessageShowing)
                {
                    isEmptySearchMessageShowing = value;
                    OnPropertyChanged("IsEmptySearchMessageShowing");
                }
            }
        }
        public bool IsNextButtonShowing
        {
            get => isNextButtonShowing;
            set
            {
                if (value != isNextButtonShowing)
                {
                    isNextButtonShowing = value;
                    OnPropertyChanged("IsNextButtonShowing");
                }
            }
        }
        public bool IsChooseElementsOpened
        {
            get => isChooseElementsOpened;
            set
            {
                if (value != isChooseElementsOpened)
                {
                    isChooseElementsOpened = value;
                    OnPropertyChanged("IsChooseElementsOpened");
                    if (SelectedElements.Count() > 0)
                    {
                        ButtonText = "Modifier";
                        IsNextButtonShowing = true;
                        IsEmptyElementMessageShowing = false;
                    }
                    else
                    {
                        ButtonText = "Choisir";
                        IsNextButtonShowing = false;
                        IsEmptyElementMessageShowing = true;
                    }
                }
            }
        }
        public List<string> Categories
        {
            get => categories;
            set
            {
                if (value != categories)
                {
                    categories = value;
                    OnPropertyChanged("Categories");
                }
            }
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
            RefreshSelectionSearchedGrid();
        }

        public void GridView1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectionChangedFirstGridView = true;
            if (!searching)
            {
                List<ElementChecked> updatedList = new List<ElementChecked>(SelectedElements);

                if (e.AddedItems.Count() > 0)
                {
                    ElementChecked addedItem = e.AddedItems.First() as ElementChecked;
                    if (!SelectedElements.Contains(addedItem))
                    {
                        updatedList.Add(addedItem);
                        addedItem.IsSelected = true;
                        SelectedElements = updatedList.OrderByDescending(el => el.Element.ElementName.Length).ToList();
                    }

                }
                else
                {
                    if (e.RemovedItems.Count() > 0)
                    {
                        ElementChecked removedItem = e.RemovedItems.First() as ElementChecked;
                        updatedList.Remove(removedItem);
                        removedItem.IsSelected = false;
                        SelectedElements = updatedList.OrderByDescending(el => el.Element.ElementName.Length).ToList();
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
                if (SelectedElements.Contains(removedItem))
                    searchedGridView.SelectedItems.Remove(removedItem);
            }


            gridview.SelectAll();
        }

        public void GridView2_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            GridView gridview = sender as GridView;
            gridview.SelectAll();
        }

        private void RefreshSelectionSearchedGrid()
        {
            foreach (ElementChecked item in SearchedElements)
            {
                if (item.IsSelected)
                {
                    searchedGridView.SelectedItems.Add(item);
                }
            }
        }

        //search methods

        public void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            searching = true;
            searchText = sender.Text;
            Search();
            searching = false;
        }

        public void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            searching = true;

            ComboBox comboBox = sender as ComboBox;
            searchCategory = comboBox.SelectedItem.ToString();
            Search();
            searching = false;
        }

        private void Search()
        {
            List<ElementChecked> newSearchedCat = new List<ElementChecked>(AllElements);
            if (searchCategory.Equals("Tout"))
                SearchedElements = newSearchedCat.Where(e => e.Element.ElementName.Contains(searchText))
                                               .OrderByDescending(e => e.Element.ElementName.Length)
                                               .ToList(); 
            else
                SearchedElements = newSearchedCat.Where(e => e.Element.Category.CategoryName.Equals(searchCategory) && e.Element.ElementName.Contains(searchText))
                                                   .OrderByDescending(e => e.Element.ElementName.Length)
                                                   .ToList();

            IsEmptySearchMessageShowing = SearchedElements.Count() == 0;
            RefreshSelectionSearchedGrid();
        }


    }
}
