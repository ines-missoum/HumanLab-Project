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
using humanlab.Models;
using humanlab.Services;
using humanlab.Views;

namespace humanlab.ViewModels
{
    class ActivityFormViewModel : BaseViewModel
    {
        /*** PUBLIC ATTRIBUTES ***/

        private List<GridChecked> allGrids;
        private List<GridChecked> searchedGrids;
        private ObservableCollection<GridChecked> selectedGridsSource;
        private List<GridChecked> selectedGrids;
        private Activity activityToModify;
        public bool isEditModeActivated;

        //attributes of choosing elements view
        private bool isChooseGridsOpened;
        public DelegateCommand ChoosePopUpVisibility { get; set; }
        public DelegateCommand SaveOrUpdateActivityDelegate { get; set; }
        public DelegateCommand BackToWindow { get; set; }

        //attributs that handles the grid preview
        public DelegateCommand<object> ShowPreviewDelegate { get; set; }
        public DelegateCommand ClosePreviewDelegate { get; set; }

        /// <summary>
        /// Handle the visibility grid preview
        /// </summary>
        private bool isGridPreviewShowing;

        private List<ElementOfActivity> elements;


        //attributes linked to dynamic messages in front
        private bool isSaveButtonShowing;
        private string buttonText;
        private bool isEmptySearchMessageShowing;
        private bool isEmptyGridsMessageShowing;
        private bool isNoSelectedGrids;
        private bool isNoGrids;

        /*** PRIVATE ATTRIBUTES ***/

        // attributes linked to the management of the selection in the choose elements view :
        private bool fromSelectionChanged2;

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

            //attributs that handles the preview
            ShowPreviewDelegate = new DelegateCommand<object>(ShowGridPreview);
            ClosePreviewDelegate = new DelegateCommand(CloseGridPreview);
            IsGridPreviewShowing = false;
            Elements = new List<ElementOfActivity>();
            isEditModeActivated = false;


            //initialisation of al the lists
            InitialiseAllGrids();
            SearchedGrids = new List<GridChecked>(AllGrids.OrderByDescending(e => e.Grid.GridName.Length));
            SelectedGrids = new List<GridChecked>();
            SelectedGridsSource = new ObservableCollection<GridChecked>();

            IsNoGrids = AllGrids.Count() == 0;

            //the views are not displayed at the the beginning
            isChooseGridsOpened = false;
            ChoosePopUpVisibility = new DelegateCommand(ChangeChoosePopUpVisibility);
            SaveOrUpdateActivityDelegate = new DelegateCommand(SaveOrUpdateActivityIfAllowed);
            BackToWindow = new DelegateCommand(RedirectToAllActivitiesPage);

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

        private void SaveOrUpdateActivityIfAllowed()
        {
            if (ActivityToModify != null)
            {
                UpdateActivityIfAllowed();
            }
            else SaveActivityIfAllowed();
        }

        public async void PrepareEditModeAsync(FrameworkElement sender, object args)
        {
            NavigationView navigation = GetNavigationView();
            Frame child = navigation.Content as Frame;
            NavigationViewModel navigationViewModel = child.DataContext as NavigationViewModel;

            if (navigationViewModel.parameterToPass != null)
            {

                Activity activity = navigationViewModel.parameterToPass as Activity;
                ActivityToModify = activity;
                navigationViewModel.Title = "Modification de l'activité " + ActivityToModify.ActivityName;
                ButtonText = "Modifier";
                IsEmptyGridsMessageShowing = false;
                IsSaveButtonShowing = true;
                IsEditModeActivated = true;
                ActivityName = ActivityToModify.ActivityName;
                List<ActivityGrids> activityGridsId = await activityRepository.GetGridsOfActivity(ActivityToModify.ActivityId);
                List<int> gridsId = activityGridsId.Select(ag => ag.GridId).ToList();
                InitialiseAllGrids();
                SearchedGrids.ForEach(g =>
                {
                    if (gridsId.Contains(g.Grid.GridId))
                    {

                        g.IsSelected = true;
                        SelectedGrids.Add(g);
                    }
                });

                SelectedGridsSource = new ObservableCollection<GridChecked>(SelectedGrids);
                updateIndexOfItems();

            }
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
            grids.ForEach(g =>
            {

                AllGrids.Add(new GridChecked(g, false, index, ShowPreviewDelegate));
                index += 1;

            }
            );


        }

        /*** GETTERS AND SETTERS FOR PUBLIC ATTRIBUTES ***/

        public List<ElementOfActivity> Elements
        {
            get => elements;
            set => SetProperty(ref elements, value, "Elements");
        }

        public bool IsGridPreviewShowing
        {
            get => isGridPreviewShowing;
            set => SetProperty(ref isGridPreviewShowing, value, "IsGridPreviewShowing");
        }
        public bool IsEditModeActivated
        {
            get => isEditModeActivated;
            set => SetProperty(ref isEditModeActivated, value, "IsEditModeActivated");
        }
        public Activity ActivityToModify
        {
            get => activityToModify;
            set
            {
                if (value != activityToModify)
                {
                    activityToModify = value;
                    OnPropertyChanged("ActivityToModify");

                }
            }
        }
        public string ButtonText
        {
            get => buttonText;
            set => SetProperty(ref buttonText, value, "ButtonText");
        }
        public string ActivityName
        {
            get => activityName;
            set => SetProperty(ref activityName, value, "ActivityName");
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

        public bool IsNoGrids
        {
            get => isNoGrids;
            set => SetProperty(ref isNoGrids, value, "IsNoGrids");
        }

        public bool IsNoSelectedGrids
        {
            get => isNoSelectedGrids;
            set => SetProperty(ref isNoSelectedGrids, value, "IsNoSelectedGrids");

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
            set
            {
                if (value != selectedGrids)
                {
                    selectedGrids = value;
                    SelectedGridsSource = new ObservableCollection<GridChecked>(value);
                    IsNoSelectedGrids = selectedGrids.Count() == 0;
                    OnPropertyChanged("SelectedGridsSource");
                    OnPropertyChanged("SelectedGrids");
                    OnPropertyChanged("ButtonText");
                }
            }

        }



        public ObservableCollection<GridChecked> SelectedGridsSource
        {
            get => selectedGridsSource;
            set
            {
                SetProperty(ref selectedGridsSource, value, "SelectedGridsSource");
                IsNoSelectedGrids = selectedGrids.Count() == 0;
            }

        }
        public bool FromSelectionChanged2
        {
            get => fromSelectionChanged2;
            set => SetProperty(ref fromSelectionChanged2, value, "FromSelectionChanged2");

        }
        /*** METHODS ***/

        /// <summary>
        /// Change the visibility of the choose elements view
        /// </summary>
        private void ChangeChoosePopUpVisibility()
        {
            IsChooseGridsOpened = !IsChooseGridsOpened;
            updateIndexOfItems();
        }

        /// <summary>
        /// Check if the gridName is not empty are already existing. If so, display an error message, else open the organization view
        /// </summary>
        private async void UpdateActivityIfAllowed()
        {
            string errorMessage = null;
            string successMessage = "Votre activité " + ActivityToModify.ActivityName + " a été modifiée avec succès.";

            if (ActivityName.Equals(""))
                errorMessage = "Veuillez entrer un nom d'activité pour poursuivre.";
            else
            {
                //we check if the name is not already taken
                List<string> activitiesNames = await activityRepository.GetActivityNamesAsync();
                activitiesNames = activitiesNames.Select(a => a.ToLower()).ToList();
                //we check if the name is not already taken> a.ToLower()).ToList();
                if (activityToModify == null)
                {
                    Debug.WriteLine("1");
                    //if creation of new grid and name already exists
                    if (activitiesNames.Contains(ActivityName.ToLower()))
                        errorMessage = "Une activité porte déjà le nom que vous avez choisi. Veuillez le modifier pour poursuivre.";
                }
                else
                {
                    Debug.WriteLine("2");
                    //if update grid and name changed for one that already exists
                    Debug.WriteLine(activitiesNames.Contains(activityName.ToLower()));
                    Debug.WriteLine(activityName.ToLower().Equals(activityToModify.ActivityName.ToLower()));
                    Debug.WriteLine(activityName.ToLower());
                    Debug.WriteLine(activityToModify.ActivityName.ToLower());
                    if (!ActivityName.ToLower().Equals(activityToModify.ActivityName.ToLower()) && activitiesNames.Contains(ActivityName.ToLower()))
                    { errorMessage = "Une activité porte déjà le nom que vous avez choisi. Veuillez le modifier pour poursuivre.";
                        Debug.WriteLine("6");
                    }
                }

            }
            Debug.WriteLine("err" + errorMessage);
            //we show error if there is one
            if (errorMessage != null)
            {
                DisplayMessagesService.showPersonalizedMessage(errorMessage);
            }
            else
            {
                //Create new activity from activity form data 
                Activity modifiedActivity = new Activity
                {
                    ActivityId = ActivityToModify.ActivityId,
                    ActivityName = ActivityName,
                    FixingTime = Convert.ToInt32(FixingTime)
                };
                 try { activityRepository.UpdateActivityAsync(modifiedActivity, SelectedGridsSource); }
                 catch { Debug.WriteLine("Error while updating activity"); }

                DisplayMessagesService.showPersonalizedMessage(successMessage);
                RedirectToAllActivitiesPage();            }

        }
        /// <summary>
        /// Check if the gridName is not empty are already existing. If so, display an error message, else open the organization view
        /// </summary>
        private async void SaveActivityIfAllowed()
        {
            string errorMessage = null;
            string successMessage = "Votre activité " + ActivityName + " a été sauvegardée avec succès.";

            if (ActivityName.Equals(""))
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
                DisplayMessagesService.showPersonalizedMessage(errorMessage);
            }
            else
            {
                //Create new activity from activity form data 
                Activity newActivity = new Activity
                {
                    ActivityName = ActivityName,
                    FixingTime = Convert.ToInt32(FixingTime)
                };

                // Save Activity in db
                activityRepository.SaveActivityAsync(newActivity, SelectedGridsSource);

                DisplayMessagesService.showSuccessMessage("activité", ActivityName, ReloadActivityFormView);

            }

        }
        public void RedirectToAllActivitiesPage()
        {
            NavigationView nv = GetNavigationView();
            Frame child = nv.Content as Frame;
            NavigationViewModel navigationViewModel = child.DataContext as NavigationViewModel;
            navigationViewModel.ParameterToPass = null;
            navigationViewModel.Title = "Toutes les activités";
            child.SourcePageType = typeof(ActivityLoadingView);
        }

        public void ReloadActivityFormView()
        {
            // Here's the navigationView 
            var navigationView = GetNavigationView();
            var child = navigationView.Content as Frame;
            child.SourcePageType = typeof(BlankPage1);
            child.SourcePageType = typeof(ActivityFormView);
        }
        /*** METHODS THAT DEALS WITH GRIDVIEW SELECTION ISSUES IN THE CHOOSE GRID VIEW***/

        /// <summary>
        /// Method called at the loading time. Pre select all elements that have the isSelect attributes to true.
        /// </summary>
        /// <param name="sender">the first grid view</param>
        /// <param name="args">arguments</param>
        public void GridView1_Loading(FrameworkElement sender, object args)
        {
            Debug.WriteLine("Grid view 1 loading");
            GridView gridview1 = sender as GridView;
            searchedGridView = gridview1;
            RefreshSelectionSearchedGrid();
            Debug.WriteLine("Grid view 1 finish loading");

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

            if (!selectionChangedFirstGridView && e.RemovedItems.Count() == 1 && !FromSelectionChanged2)
            {
                FromSelectionChanged2 = true;
                GridChecked removedItem = e.RemovedItems.First() as GridChecked;
                if (SelectedGrids.Contains(removedItem))
                    searchedGridView.SelectedItems.Remove(removedItem);

                if (!searchedGridView.SelectedItems.Contains(removedItem))
                {
                    removedItem.IsSelected = false;
                    List<GridChecked> transitionList = new List<GridChecked>(SelectedGrids);
                    transitionList.Remove(removedItem);
                    SelectedGrids = transitionList.OrderByDescending(gc => gc.Grid.GridName.Length).ToList();
                    FromSelectionChanged2 = false;

                }
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
            ActivityName = name;
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
            SearchedGrids = newSearchedCat.Where(e => e.Grid.GridName.ToUpper().Contains(searchText.ToUpper()))
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
                }
            }
        }
        public void listView_DragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args) => updateIndexOfItems();

        public void timeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Slider slider = sender as Slider;
            if (slider != null && FixingTime != slider.Value)
            {
                FixingTime = slider.Value;

            }
        }

        //METHODS LINKED TO THE GRID PREVIEW
        /// <summary>
        /// This method is called to show the grid preview
        /// </summary>
        /// <param name="grid">the grid to show</param>
        public void ShowGridPreview(object gridParameter)
        {
            var grid = gridParameter as Models.Grid;
            //we retrieve the elements of the grid to show
            GetElementsOfGrid(grid.GridId);
            //we show the preview
            IsGridPreviewShowing = true;
            //we close the menu to have the preview in all screen
            NavigationView navView = GetNavigationView();
            navView.IsPaneVisible = false;
            navView.IsPaneOpen = false;
            navView.IsPaneToggleButtonVisible = false;
            //we change the title
            Frame child = navView.Content as Frame;
            NavigationViewModel navigationViewModel = child.DataContext as NavigationViewModel;
            navigationViewModel.Title = "Aperçu de la grille " + grid.GridName;
        }

        /// <summary>
        /// This method retrieves all the elements of a specific grid
        /// </summary>
        /// <param name="gridId">the id of the specific grid</param>
        private async void GetElementsOfGrid(int gridId)
        {
            List<ElementOfActivity> dbElements = await gridRepository.GetAllGridElements(gridId);
            Elements = dbElements;
        }

        /// <summary>
        /// This methods is called when we close the grid preview
        /// </summary>
        public void CloseGridPreview()
        {
            //we open back the menu
            NavigationView navView = GetNavigationView();
            navView.IsPaneVisible = true;
            navView.IsPaneToggleButtonVisible = true;
            //we change the title
            Frame child = navView.Content as Frame;
            NavigationViewModel navigationViewModel = child.DataContext as NavigationViewModel;
            if (ActivityToModify == null)
                navigationViewModel.Title = "Nouvelle activité";
            else
                navigationViewModel.Title = "Modification de l'activité " + ActivityToModify.ActivityName;
            //we call the preview
            IsGridPreviewShowing = false;
            //we reset the needed values
            Elements = new List<ElementOfActivity>();
        }

        /// <summary>
        /// This is called when the size of the window change. This close the menu that is open by default when the screen is bigger.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            NavigationView navView = GetNavigationView();
            navView.IsPaneOpen = false;
        }
    }
}
