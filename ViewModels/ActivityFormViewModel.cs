using humanlab.DAL;
using humanlab.Helpers.Models;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Diagnostics;
using Windows.UI.Xaml.Controls.Primitives;

namespace humanlab.ViewModels
{
    class ActivityFormViewModel : BaseViewModel
    {
        /*** PUBLIC ATTRIBUTES ***/

        private List<GridChecked> allGrids;
        private List<GridChecked> searchedGrids;
        private ObservableCollection<GridChecked> selectedGridsSource;
        private List<GridChecked> selectedGrids;

        //attributes of choosing elements view
        private bool isChooseGridsOpened;
        public DelegateCommand ChoosePopUpVisibility { get; set; }
        public DelegateCommand SaveActivityDelegate { get; set; }

        //attributes linked to dynamic messages in front
        private bool isSaveButtonShowing;
        private string buttonText;
        private bool isEmptySearchMessageShowing;
        private bool isEmptyGridsMessageShowing;

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
        private string activityName;
        private double fixingTime;

        /// <summary>
        /// Responsible for looking in database
        /// </summary>
        private GridRepository gridRepository;
        private ActivityRepository activityRepository;

        /***CONSTRUCTOR***/

        public ActivityFormViewModel()
        {
            gridRepository = new GridRepository();
            activityRepository = new ActivityRepository();

            //initialisation of al the lists
            InitialiseAllGrids();
            SearchedGrids = new List<GridChecked>(AllGrids.OrderByDescending(e => e.Grid.GridName.Length));
            SelectedGrids = new List<GridChecked>();
            SelectedGridsSource = new ObservableCollection<GridChecked>();

            //A supprimer just test


            //the views are not displayed at the the beginning
            isChooseGridsOpened = false;
            ChoosePopUpVisibility = new DelegateCommand(ChangeChoosePopUpVisibility);
            SaveActivityDelegate = new DelegateCommand(SaveActivityIfAllowed);

            //no search has began
            searching = false;

            IsSaveButtonShowing = false;
            isEmptySearchMessageShowing = false;
            isEmptyGridsMessageShowing = true;
            ButtonText = "Choisir";

            //form default values
            searchText = "";
            activityName = "";
            fixingTime = 0;

        }

        /// <summary>
        /// Method that retrieve all the elements from the database and save them as ElementChecked (that has in addition a IsSelected attribute to handle the selection in the grid view)
        /// It also retrieve all the categories to set the combo box in the view for the search
        /// </summary>
        private async void InitialiseAllGrids()
        {
            //retrieve all the elements
            var grids = await gridRepository.GetGridsAsync();
            AllGrids = new List<GridChecked>();
            grids.OrderByDescending(g => g.GridName.Length).ToList();
            int index = 1;
            grids.ForEach(g => {

                AllGrids.Add(new GridChecked(g, false, index));
                index += 1;
         
                }
            );
            

        }

        /*** GETTERS AND SETTERS FOR PUBLIC ATTRIBUTES ***/
        public string ButtonText
        {
            get => buttonText;
            set => SetProperty(ref buttonText, value, "ButtonText");
        }
        public double FixingTime
        {
            get => fixingTime;
            set => SetProperty(ref fixingTime, value, "FixingTime");
        }

        public bool IsEmptyGridsMessageShowing
        {
            get => isEmptyGridsMessageShowing;
            set => SetProperty(ref isEmptyGridsMessageShowing, value, "IsEmptyGridsMessageShowing");

        }
        public bool IsEmptySearchMessageShowing
        {
            get => isEmptySearchMessageShowing;
            set => SetProperty(ref isEmptySearchMessageShowing, value, "IsEmptySearchMessageShowing");
        }

        public bool IsSaveButtonShowing
        {
            get => isSaveButtonShowing;
            set => SetProperty(ref isSaveButtonShowing, value, "IsSaveButtonShowing");
        }

        public bool IsChooseGridsOpened
        {
            get => isChooseGridsOpened;
            set
            {
                if (value != isChooseGridsOpened)
                {
                    isChooseGridsOpened = value;
                    OnPropertyChanged("IsChooseGridsOpened");
                    if (SelectedGrids.Count() > 0)
                    {
                        ButtonText = "Modifier";
                        IsSaveButtonShowing = true;
                        IsEmptyGridsMessageShowing = false;
                    }
                    else
                    {
                        ButtonText = "Choisir";
                        IsSaveButtonShowing = false;
                        IsEmptyGridsMessageShowing = true;
                    }
                }
            }
        }

        public List<GridChecked> AllGrids
        {
            get => allGrids;
            set => SetProperty(ref allGrids, value, "AllGrids");
        }

        public List<GridChecked> SearchedGrids
        {
            get => searchedGrids;
            set => SetProperty(ref searchedGrids, value, "SearchedGrids");

        }
        public List<GridChecked> SelectedGrids
        {
            get => selectedGrids;
            set {
                if (value != selectedGrids)
                {
                    selectedGrids = value;
                    SelectedGridsSource = new ObservableCollection<GridChecked>(value);
                    OnPropertyChanged("SelectedGridsSource");
                    OnPropertyChanged("SelectedGrids");
                    OnPropertyChanged("ButtonText");
                }
            }

        }



        public ObservableCollection<GridChecked> SelectedGridsSource
        {
            get => selectedGridsSource;
            set => SetProperty(ref selectedGridsSource, value, "SelectedGridsSource");

        }

        /*** METHODS ***/

        /// <summary>
        /// Change the visibility of the choose elements view
        /// </summary>
        private void ChangeChoosePopUpVisibility()
        {
            IsChooseGridsOpened = !IsChooseGridsOpened;
        }

        /// <summary>
        /// Check if the gridName is not empty are already existing. If so, display an error message, else open the organization view
        /// </summary>
        private async void SaveActivityIfAllowed()
        {
            string errorMessage = null;

            if (activityName.Equals(""))
                errorMessage = "Veuillez entrer un nom d'activité pour poursuivre.";
            else
            {
                //we check if the name is not already taken
                List<string> activitiesNames = await activityRepository.GetActivityNamesAsync();
                if (activitiesNames.Contains(activityName))
                    errorMessage = "Une activité porte déjà le nom que vous avez choisi. Veuillez le modifier pour poursuivre.";
            }

            //we show error if there is one
            if (errorMessage != null)
            {
                MessageDialog messageDialog = new MessageDialog(errorMessage);
                // display the message dialog with the proper error 
                await messageDialog.ShowAsync();
            }
            else
            {
                //SAVE => TODO
            }

        }

        /*** METHODS THAT DEALS WITH GRIDVIEW SELECTION ISSUES IN THE CHOOSE GRID VIEW***/

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
                List<GridChecked> updatedList = new List<GridChecked>(SelectedGrids);

                if (e.AddedItems.Count() > 0)
                {
                    GridChecked addedItem = e.AddedItems.First() as GridChecked;
                    if (!SelectedGrids.Contains(addedItem))
                    {
                        updatedList.Add(addedItem);
                        addedItem.IsSelected = true;
                        SelectedGrids = updatedList.OrderByDescending(el => el.Grid.GridName.Length).ToList();
                    }

                }
                else
                {
                    if (e.RemovedItems.Count() > 0)
                    {
                        GridChecked removedItem = e.RemovedItems.First() as GridChecked;
                        updatedList.Remove(removedItem);
                        removedItem.IsSelected = false;
                        SelectedGrids = updatedList.OrderByDescending(el => el.Grid.GridName.Length).ToList();
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
                GridChecked removedItem = e.RemovedItems.First() as GridChecked;
                if (SelectedGrids.Contains(removedItem))
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
            foreach (GridChecked item in SearchedGrids)
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
            activityName = name;
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
        /// Update the searchedElements list taking care of the category and name searched.
        /// </summary>
        private void Search()
        {
            List<GridChecked> newSearchedCat = new List<GridChecked>(AllGrids);
            SearchedGrids = newSearchedCat.Where(e => e.Grid.GridName.Contains(searchText))
                                           .OrderByDescending(e => e.Grid.GridName.Length)
                                           .ToList();

            IsEmptySearchMessageShowing = SearchedGrids.Count() == 0;
            RefreshSelectionSearchedGrid();
        }

        public void updateIndexOfItems()
        {
            foreach (GridChecked item in SelectedGridsSource)
            {
                int currentIndex = SelectedGridsSource.IndexOf(item) + 1;
                if (item.IndexInListView != currentIndex)
                {
                    item.IndexInListView = currentIndex;
                    Debug.WriteLine("nom " + item.Grid.GridName + "  => index =" + item.IndexInListView);
                }
            }
        }
        public void listView_DragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)=> updateIndexOfItems();

        public void timeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Slider slider = sender as Slider;
            if (slider != null && FixingTime != slider.Value)
            {
                FixingTime = slider.Value;
                Debug.WriteLine("fix time " + FixingTime);

            }
        }
    }
}
