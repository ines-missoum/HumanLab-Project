﻿using humanlab.DAL;
using humanlab.Helpers.Models;
using humanlab.Models;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace humanlab.ViewModels
{
    public class GridFormViewModel : BaseViewModel
    {
        /*** PUBLIC ATTRIBUTES ***/

        private List<ElementChecked> allElements;
        private List<ElementChecked> searchedElements;
        private List<ElementChecked> selectedElements;
        private List<string> categories;

        //attributes of choosing elements view
        private bool isChooseElementsOpened;
        public DelegateCommand ChoosePopUpVisibility { get; set; }

        //attributes of organize elements view
        private bool isOrganizeElementsOpened;
        public DelegateCommand OrganizePopUpVisibility { get; set; }

        //attributes linked to dynamic messages in front
        private bool isNextButtonShowing;
        private string buttonText;
        private bool isEmptySearchMessageShowing;
        private bool isEmptyElementMessageShowing;

        /*** PRIVATE ATTRIBUTES ***/

        // attributes linked to the management of the selection in the choose elements view :

        /// <summary>
        /// Allows to know if a research is happening in the choose elements view
        /// </summary>
        private bool searching;

        /// <summary>
        /// Allows to know if the selection has just changed in the first grid view
        /// </summary>
        private bool selectionChangedFirstGridView;

        /// <summary>
        /// First grid view => contains the elements that are resulting of the search
        /// </summary>
        private GridView searchedGridView;

        //values of the form 
        private string searchText { get; set; }
        private string searchCategory { get; set; }
        private string gridName;

        /// <summary>
        /// Responsible for looking in database
        /// </summary>
        private Repository repository;

        /***CONSTRUCTOR***/

        public GridFormViewModel()
        {
            repository = new Repository();

            //initialisation of al the lists
            InitialiseAllElementsAndCategories();
            SearchedElements = new List<ElementChecked>(AllElements.OrderByDescending(e => e.Element.ElementName.Length));
            SelectedElements = new List<ElementChecked>();

            //the views are not displayed at the the beginning
            isChooseElementsOpened = false;
            isOrganizeElementsOpened = false;
            ChoosePopUpVisibility = new DelegateCommand(ChangeChoosePopUpVisibility);
            OrganizePopUpVisibility = new DelegateCommand(OpenOrganizationPopUpIfAllowed);

            //no search has began
            searching = false;

            IsNextButtonShowing = false;
            isEmptySearchMessageShowing = false;
            isEmptyElementMessageShowing = true;
            ButtonText = "Choisir";

            //form default values
            searchCategory = "Tout";
            searchText = "";
            gridName = "";
        }

        /// <summary>
        /// Method that retrieve all the elements from the database and save them as ElementChecked (that has in addition a IsSelected attribute to handle the selection in the grid view)
        /// It also retrieve all the categories to set the combo box in the view for the search
        /// </summary>
        private async void InitialiseAllElementsAndCategories()
        {
            //retrieve all the elements
            var elements = await repository.GetElementsAsync();
            AllElements = new List<ElementChecked>();
            elements.ForEach(e => AllElements.Add(new ElementChecked(e, false)));

            //retrieve all categories
            Categories = elements.Select(e => e.Category.CategoryName).Distinct().ToList();
            Categories.Add("Tout");
        }

        /*** GETTERS AND SETTERS FOR PUBLIC ATTRIBUTES ***/
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
        public bool IsOrganizeElementsOpened
        {
            get => isOrganizeElementsOpened;
            set
            {
                if (value != isOrganizeElementsOpened)
                {
                    isOrganizeElementsOpened = value;
                    OnPropertyChanged("IsOrganizeElementsOpened");
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

        /// <summary>
        /// Change the visibility of the choose elements view
        /// </summary>
        private void ChangeChoosePopUpVisibility()
        {
            IsChooseElementsOpened = !IsChooseElementsOpened;
        }

        /// <summary>
        /// Check if the gridName is not empty are already existing. If so, display an error message, else open the organization view
        /// </summary>
        private async void OpenOrganizationPopUpIfAllowed()
        {
            string errorMessage = null;

            if (gridName.Equals(""))
                errorMessage = "Veuillez entrer un nom de grille pour poursuivre.";
            else
            {
                //we check if the name is not already taken
                List<string> gridsNames = await repository.GetGridsNamesAsync();
                if (gridsNames.Contains(gridName))
                    errorMessage = "Une grille porte déjà le nom que vous avez choisi. Veuillez le modifier pour poursuivre.";
            }

            //we show error if there is one
            if (errorMessage != null)
            {
                MessageDialog messageDialog = new MessageDialog(errorMessage);
                // display the message dialog with the proper error 
                await messageDialog.ShowAsync();
            }
            else
                //else we display the organization view
                IsOrganizeElementsOpened = !IsOrganizeElementsOpened;
        }

        /*** METHODS THAT DEALS WITH GRIDVIEW SELECTION ISSUES IN THE CHOOSE ELEMENT VIEW***/

        /// <summary>
        /// Method called at the loading time. Pre select all elements that have the isSelect attributes to true.
        /// </summary>
        /// <param name="sender">the first grid view</param>
        /// <param name="args">arguments</param>
        public void GridView1_Loading(FrameworkElement sender, object args)
        {
            GridView gridview1 = sender as GridView;
            searchedGridView = gridview1;
            RefreshSelectionSearchedGrid();
        }

        /// <summary>
        /// Method called when the selection change in the first grid view.
        /// If an element is selected, it is added in the selected elements list.
        /// If an element is unselected, it is removed in the selected elements list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Method called at the loading time. Pre select all elements of the list.
        /// </summary>
        /// <param name="sender">the second grid view</param>
        /// <param name="args">arguments</param>
        public void GridView2_Loading(FrameworkElement sender, object args)
        {
            GridView gridview2 = sender as GridView;
            gridview2.SelectAll();
        }

        /// <summary>
        /// Method called when an element of the second gridview is unselect.
        /// We remove it from the slected elements list and unselect it from the search list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void GridView2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GridView gridview = sender as GridView;

            if (!selectionChangedFirstGridView && e.RemovedItems.Count() == 1)
            {
                ElementChecked removedItem = e.RemovedItems.First() as ElementChecked;
                if (SelectedElements.Contains(removedItem))
                    searchedGridView.SelectedItems.Remove(removedItem);
            }
            //because all the elements are automatically unselected
            gridview.SelectAll();
        }

        public void GridView2_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            GridView gridview = sender as GridView;
            gridview.SelectAll();
        }

        /// <summary>
        /// set all elements that have the isSelected attribute at true as selected in the first gridView.
        /// </summary>
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

        /*** FORM HANDLING METHODS ***/

        /// <summary>
        /// Method called at each changes in the text box og the gridName. We save the new value.
        /// </summary>
        /// <param name="sender"> gridName textbox</param>
        /// <param name="e">arguments</param>
        public void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox control = sender as TextBox;
            string name = control.Text;
            gridName = name;
        }

        /**Search hanling methods**/

        /// <summary>
        /// Retrieve the name searched, save it and run the search.
        /// </summary>
        /// <param name="sender">search by name field</param>
        /// <param name="args">arguments </param>
        public void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            searching = true;
            searchText = sender.Text;
            Search();
            searching = false;
        }

        /// <summary>
        /// Retrieve the name searched, save it and run the search.
        /// </summary>
        /// <param name="sender">search by name field</param>
        /// <param name="args">arguments </param>
        public void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            searching = true;

            ComboBox comboBox = sender as ComboBox;
            searchCategory = comboBox.SelectedItem.ToString();
            Search();
            searching = false;
        }

        /// <summary>
        /// Update the searchedElements list taking care of the category and name searched.
        /// </summary>
        private void Search()
        {
            List<ElementChecked> newSearchedCat = new List<ElementChecked>(AllElements);
            if (searchCategory.Equals("Tout"))
                //then we search only by names
                SearchedElements = newSearchedCat.Where(e => e.Element.ElementName.Contains(searchText))
                                               .OrderByDescending(e => e.Element.ElementName.Length)
                                               .ToList();
            else
                //else we search by name (not strict => just checking if the name contains the search) AND by category
                SearchedElements = newSearchedCat.Where(e => e.Element.Category.CategoryName.Equals(searchCategory) && e.Element.ElementName.Contains(searchText))
                                                   .OrderByDescending(e => e.Element.ElementName.Length)
                                                   .ToList();

            IsEmptySearchMessageShowing = SearchedElements.Count() == 0;
            RefreshSelectionSearchedGrid();
        }

    }
}
