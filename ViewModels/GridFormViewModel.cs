using humanlab.DAL;
using humanlab.Helpers.Models;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using humanlab.Models;
using System.Threading.Tasks;
using humanlab.Views;
using humanlab.Services;

namespace humanlab.ViewModels
{
    /// <summary>
    /// View Model in charge of the grid form.
    /// </summary>
    public class GridFormViewModel : BaseViewModel
    {
        /*** PUBLIC ATTRIBUTES ***/

        private List<ElementChecked> allElements;
        private List<ElementChecked> searchedElements;
        private List<ElementChecked> selectedElements;
        private List<string> categories;
        private ElementPlaced elementTest;

        //attributes of choosing elements view
        private bool isChooseElementsOpened;
        public DelegateCommand ChoosePopUpVisibility { get; set; }

        //attributes of organize elements view
        private bool isOrganizeElementsOpened;
        public DelegateCommand OrganizePopUpVisibility { get; set; }

        private ScrollViewer scrollView;
        private ItemsControl itemsControl;
        private Boolean isPositionsSet;
        private List<ElementPlaced> elementsPlaced;
        public DelegateCommand SaveGridPlacementCommand { get; set; }
        public DelegateCommand ReturnToSelectionCommand { get; set; }
        //attributes linked to dynamic messages in front
        private bool isNextButtonShowing;
        private string buttonText;
        private bool isEmptySearchMessageShowing;
        private bool isEmptyElementMessageShowing;
        private bool fromSelectionChanged2;
        private bool isNoSelectedElements;
        private bool isNoElements;

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
        private Models.Grid gridToModify { get; set; }


        /// <summary>
        /// Responsible for looking in database
        /// </summary>
        private ElementRepository repository;
        private GridRepository gridRepository;

        /***CONSTRUCTOR***/

        public GridFormViewModel()
        {
            repository = new ElementRepository();
            gridRepository = new GridRepository();

            //initialisation of al the lists
            InitialiseAllElementsAndCategories();
            SearchedElements = new List<ElementChecked>(AllElements.OrderByDescending(e => e.Element.ElementName.Length));
            SelectedElements = new List<ElementChecked>();

            IsNoElements = AllElements.Count() == 0;

            //the views are not displayed at the the beginning
            isChooseElementsOpened = false;
            isOrganizeElementsOpened = false;
            ChoosePopUpVisibility = new DelegateCommand(ChangeChoosePopUpVisibility);
            OrganizePopUpVisibility = new DelegateCommand(OpenOrganizationPopUpIfAllowed);

            //no search has began
            searching = false;

            fromSelectionChanged2 = false;
            IsNextButtonShowing = false;
            isEmptySearchMessageShowing = false;
            isEmptyElementMessageShowing = true;
            ButtonText = "Choisir";


            //form default values
            searchCategory = "Tout";
            searchText = "";
            gridName = "";

            //scrollview attributes
            this.elementsPlaced = new List<ElementPlaced>();
            this.scrollView = new ScrollViewer();
            this.itemsControl = new ItemsControl();
            SaveGridPlacementCommand = new DelegateCommand(UpdateOrAddGridIfAllowed, CanSavOrUpdateGridPlacement);
            ReturnToSelectionCommand = new DelegateCommand(ReturnToSelection);
            isPositionsSet = false;

        }

        private void ReturnToSelection()
        {
            IsOrganizeElementsOpened = false;
            ElementsPlaced = new List<ElementPlaced>();
            NavigationView nv = GetNavigationView();
            nv.IsPaneToggleButtonVisible = true;
            nv.IsPaneVisible = true;


        }

        private bool CanSavOrUpdateGridPlacement()
        {
            return IsPositionsSet;
        }
        private async void UpdateOrAddGridIfAllowed()
        {
            string errorMessage = null;


            if (GridName.Equals(""))
                errorMessage = "Veuillez entrer un nom de grille pour poursuivre.";
            else
            {
                List<Models.Grid> grids = await gridRepository.GetGridsAsync();
                List<string> gridsNames = grids.Select(g => g.GridName).ToList();
                double size = (ScrollView.ViewportHeight / 2) * ScrollView.ZoomFactor;

                string successMessage = "";
                //we check if the name is not already taken
                if (gridToModify == null)
                {
                    //if creation of new grid and name already exists
                    if (gridsNames.Contains(GridName))
                        errorMessage = "Une grille porte déjà le nom que vous avez choisi. Veuillez le modifier pour poursuivre.";
                }
                else
                {
                    //if update grid and name changed for one that already exists
                    if (!GridName.Equals(gridToModify.GridName) && gridsNames.Contains(GridName))
                        errorMessage = "Une grille porte déjà le nom que vous avez choisi. Veuillez le modifier pour poursuivre.";
                }

                //we show error if there is one
                if (errorMessage != null)
                {
                    DisplayMessagesService.showPersonalizedMessage(errorMessage);
                }
                else
                {
                    if (gridToModify != null)
                    {
                        successMessage = "Votre grille " + GridName + " a été modifiée avec succès.";

                        Models.Grid modifiedGrid = new Models.Grid
                        {
                            GridId = gridToModify.GridId,
                            GridName = GridName,
                            ElementsHeight = size,
                            ElementsWidth = size,
                        };

                        try
                        {
                            gridRepository.UpdateGridAsync(modifiedGrid, ElementsPlaced);
                            DisplayMessagesService.showPersonalizedMessage(successMessage);
                            RedirectToAllGridsPage();

                        }
                        catch { Debug.WriteLine("Error upgrating element"); }
                        // Update Activity in db
                    }
                    else
                    {
                        Models.Grid newGrid = new Models.Grid
                        {
                            GridName = GridName,
                            ElementsHeight = size,
                            ElementsWidth = size,
                        };

                        try
                        {
                            gridRepository.SaveGridAsync(newGrid, ElementsPlaced);
                            DisplayMessagesService.showSuccessMessage("grille", gridName, ReloadGridFormView);
                            NavigationView nv = GetNavigationView();
                            nv.IsPaneToggleButtonVisible = true;
                            nv.IsPaneVisible = true;

                        }
                        catch { Debug.WriteLine("Error saving element"); }
                    }

                }
                //Create new activity from activity form data 

            }
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
        public async void PrepareEditModeAsync(FrameworkElement sender, object args)
        {
            NavigationView navigation = GetNavigationView();
            Frame child = navigation.Content as Frame;
            NavigationViewModel navigationViewModel = child.DataContext as NavigationViewModel;

            if (navigationViewModel.parameterToPass != null)
            {

                Models.Grid grid = navigationViewModel.parameterToPass as Models.Grid;
                gridToModify = grid;
                GridName = grid.GridName;
                navigationViewModel.Title = "Modification de la grille " + GridName;
                ButtonText = "Modifier";
                //     IsEmptyGridsMessageShowing = false;
                //     IsSaveButtonShowing = true;
                List<ElementOfActivity> dbElements = await gridRepository.GetAllGridElements(grid.GridId);
                IsEmptyElementMessageShowing = false;
                List<int> ElementsId = dbElements.Select(dbobj => dbobj.Element.ElementId).ToList();
                InitialiseAllElementsAndCategories();
                SearchedElements.ForEach(el =>
                {
                    if (ElementsId.Contains(el.Element.ElementId))
                    {

                        el.IsSelected = true;
                        SelectedElements.Add(el);
                    }
                });

            }
        }



        /*** GETTERS AND SETTERS FOR PUBLIC ATTRIBUTES ***/
        public string ButtonText
        {
            get => buttonText;
            set => SetProperty(ref buttonText, value, "ButtonText");
        }

        public string GridName
        {
            get => gridName;
            set => SetProperty(ref gridName, value, "GridName");
        }

        public ScrollViewer ScrollView
        {
            get => scrollView;
            set => SetProperty(ref scrollView, value, "ScrollView");

        }

        public ItemsControl ItemsControl
        {
            get => itemsControl;
            set => SetProperty(ref itemsControl, value, "ItemsControl");

        }
        public bool FromSelectionChanged2
        {
            get => fromSelectionChanged2;
            set => SetProperty(ref fromSelectionChanged2, value, "FromSelectionChanged2");

        }

        public bool IsNoElements
        {
            get => isNoElements;
            set => SetProperty(ref isNoElements, value, "IsNoElements");
        }

        public bool IsNoSelectedElements
        {
            get => isNoSelectedElements;
            set => SetProperty(ref isNoSelectedElements, value, "IsNoSelectedElements");

        }
        public bool IsPositionsSet
        {
            get => isPositionsSet;
            set => SetProperty(ref isPositionsSet, value, "IsPositionsSet");

        }
        public bool IsEmptyElementMessageShowing
        {
            get => isEmptyElementMessageShowing;
            set => SetProperty(ref isEmptyElementMessageShowing, value, "IsEmptyElementMessageShowing");

        }
        public bool IsEmptySearchMessageShowing
        {
            get => isEmptySearchMessageShowing;
            set => SetProperty(ref isEmptySearchMessageShowing, value, "IsEmptySearchMessageShowing");

        }
        public bool IsNextButtonShowing
        {
            get => isNextButtonShowing;
            set => SetProperty(ref isNextButtonShowing, value, "IsNextButtonShowing");
        }
        public bool IsOrganizeElementsOpened
        {
            get => isOrganizeElementsOpened;
            set => SetProperty(ref isOrganizeElementsOpened, value, "IsOrganizeElementsOpened");
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
            set => SetProperty(ref categories, value, "Categories");
        }
        public ElementPlaced ElementTest
        {
            get => elementTest;
            set => SetProperty(ref elementTest, value, "ElementTest");

        }

        public List<ElementChecked> AllElements
        {
            get => allElements;
            set => SetProperty(ref allElements, value, "AllElements");

        }
        public List<ElementChecked> SearchedElements
        {
            get => searchedElements;
            set => SetProperty(ref searchedElements, value, "SearchedElements");

        }
        public List<ElementChecked> SelectedElements
        {
            get => selectedElements;
            set
            {
                SetProperty(ref selectedElements, value, "SelectedElements");
                IsNoSelectedElements = selectedElements.Count() == 0;
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

            if (GridName.Equals(""))
                errorMessage = "Veuillez entrer un nom de grille pour poursuivre.";
            else
            {
                //we check if the name is not already taken
                List<string> gridsNames = await repository.GetGridsNamesAsync();
                if (gridToModify == null)
                {
                    //if creation of new grid and name already exists
                    if (gridsNames.Contains(GridName))
                        errorMessage = "Une grille porte déjà le nom que vous avez choisi. Veuillez le modifier pour poursuivre.";
                }
                else
                {
                    //if update grid and name changed for one that already exists
                    if (!GridName.Equals(gridToModify.GridName) && gridsNames.Contains(GridName))
                        errorMessage = "Une grille porte déjà le nom que vous avez choisi. Veuillez le modifier pour poursuivre.";
                }
            }

            //we show error if there is one
            if (errorMessage != null)
            {
                DisplayMessagesService.showPersonalizedMessage(errorMessage);
            }
            else
            {
                List<ElementPlaced> listBis = new List<ElementPlaced>(ElementsPlaced);
                //else we display the organization view
                // Retrieve checked elements and create new ElementPlaced item
                foreach (ElementChecked element in SelectedElements)
                {
                    //Initialize values to 0 before setting them to their real value
                    ElementPlaced ep = new ElementPlaced(element.Element, 0, 0, 0, 0);
                    listBis.Add(ep);
                }
                ElementsPlaced = listBis;
                IsOrganizeElementsOpened = !IsOrganizeElementsOpened;
                NavigationView nv = GetNavigationView();
                nv.IsPaneOpen = false;
                nv.IsPaneToggleButtonVisible = false;
                nv.IsPaneVisible = false;
                AddDelegatesToItems(ItemsControl);
                SetInitialWidthToElements();
            }

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

                        SelectedElements = updatedList.OrderByDescending(el => el.Element.ElementName.Length).ToList();
                        removedItem.IsSelected = false;
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
                ElementChecked removedItem = e.RemovedItems.First() as ElementChecked;

                if (SelectedElements.Contains(removedItem))
                    searchedGridView.SelectedItems.Remove(removedItem);
                if (!searchedGridView.SelectedItems.Contains(removedItem))
                {
                    removedItem.IsSelected = false;
                    List<ElementChecked> transitionList = new List<ElementChecked>(SelectedElements);
                    transitionList.Remove(removedItem);
                    SelectedElements = transitionList.OrderByDescending(el => el.Element.ElementName.Length).ToList();
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
            GridName = name;
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
                SearchedElements = newSearchedCat.Where(e => e.Element.ElementName.ToUpper().Contains(searchText.ToUpper()))
                                               .OrderByDescending(e => e.Element.ElementName.Length)
                                               .ToList();
            else
                //else we search by name (not strict => just checking if the name contains the search) AND by category
                SearchedElements = newSearchedCat.Where(e => e.Element.Category.CategoryName.Equals(searchCategory) && e.Element.ElementName.ToUpper().Contains(searchText.ToUpper()))
                                                   .OrderByDescending(e => e.Element.ElementName.Length)
                                                   .ToList();

            IsEmptySearchMessageShowing = AllElements.Count() > 0 && SearchedElements.Count() == 0;
            RefreshSelectionSearchedGrid();
        }



        /*********************************************************************************************************************************/

        public void ItemsControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Collection of Selected Elements to be placed in the grid
            ItemsControl = sender as ItemsControl;
            AddDelegatesToItems(ItemsControl);

        }

        public void AddDelegatesToItems(ItemsControl itemsControl)
        {
            var items = itemsControl.Items;

            foreach (var item in items)
            {

                // Retrieve all UIElements (images) from the DataTemplate 
                // Here 'element' refers to a ContentPresenter object which wraps the image we need for translation calculs
                itemsControl.UpdateLayout();
                UIElement element = (UIElement)itemsControl.ItemContainerGenerator.ContainerFromItem(item);

                // Add to each element their own delegate method manipulationDelta
                element.ManipulationDelta += new ManipulationDeltaEventHandler(Image_ManipulationDelta);
                element.ManipulationStarted += new ManipulationStartedEventHandler(Image_ManipulationStarted);
                element.ManipulationCompleted += new ManipulationCompletedEventHandler(Image_ManipulationCompleted);
            }

        }

        public void SetInitialWidthToElements()
        {
            double initialWidth = ScrollView.ViewportHeight / 2;
            double initialHeigth = ScrollView.ViewportHeight / 2;
            {
                // Set UIElements object width/heigth
                foreach (ElementPlaced ep in ElementsPlaced)
                {

                    ep.WidthString = initialWidth.ToString();
                    ep.HeigthString = initialHeigth.ToString();

                    OnPropertyChanged("WidthString");
                    OnPropertyChanged("HeigthString");

                }
                float initialZoomFactor = 0.7F;
                ScrollView.ChangeView(0, 0, initialZoomFactor);
            }
        }


        private void Image_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            var contentPresenter = sender as ContentPresenter;

            //Get image wrapped in the sender of type ContentPresenter
            var child = VisualTreeHelper.GetChild(contentPresenter, 0);


            //Cast object to image 
            Image image = child as Image;
            image.Opacity = 1;
        }

        private void Image_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            var contentPresenter = sender as ContentPresenter;

            //Get image wrapped in the sender of type ContentPresenter
            var child = VisualTreeHelper.GetChild(contentPresenter, 0);


            //Cast object to image 
            Image image = child as Image;
            image.Opacity = 0.8;
        }
        public List<ElementPlaced> ElementsPlaced
        {
            get => elementsPlaced;
            set => SetProperty(ref elementsPlaced, value, "ElementsPlaced");

        }

        public void Image_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {

            var contentPresenter = sender as ContentPresenter;

            //Get image wrapped in the sender of type ContentPresenter
            var child = VisualTreeHelper.GetChild(contentPresenter, 0);


            //Cast object to image 
            Image image = child as Image;

            //Retrieve image's tag to check on which element('current') we should apply the translation
            string tagImage = image.Tag.ToString();

            ElementPlaced current = ElementsPlaced.Select(element => element)
                                                 .Where(element => element.Element.ElementName.Equals(tagImage)).First();

            //Get Position of the current inside the scrollViewer
            var results = GetItemPositionInScrollViewer(image);
            /***CALCULS FOR LIMITATIONS***/

            //Distance between ScrollViewer's left border and the Image's left Border
            var LeftBorder = results["LeftBorder"];

            //Distance between ScrollViewer's left border and the Image's rigth Border
            var RightBorder = results["RightBorder"];

            //Distance between ScrollViewer's top border and the Image's top
            var TopBorder = results["TopBorder"];

            //Distance between ScrollViewer's top border and the Image's bottom
            var BottomBorder = results["BottomBorder"];

            //Small shift(delta) on the horizontal axis 
            var xAdjustment = e.Delta.Translation.X;

            //Small shift(delta) on the vertical axis 
            var yAdjustment = e.Delta.Translation.Y;



            //Conditions before object's translation

            if (LeftBorder + xAdjustment >= 0 && RightBorder + xAdjustment <= ScrollView.ViewportWidth)
            {
                current.DeltaOnX += xAdjustment * (1 / ScrollView.ZoomFactor);

                current.XPosition = LeftBorder + xAdjustment;

            }

            if (TopBorder + yAdjustment >= 0 && BottomBorder + yAdjustment <= ScrollView.ViewportHeight)
            {
                current.DeltaOnY += yAdjustment * (1 / ScrollView.ZoomFactor);
                current.YPosition = TopBorder + yAdjustment;
            }

            IsPositionsSet = true;
            SaveGridPlacementCommand.RaiseCanExecuteChanged();

        }

        public Image GetImageFromUIElement(object item)
        {
            // Retrieve all UIElements (images) from the DataTemplate 
            // Here 'element' refers to a ContentPresenter object which wraps the image we need for translation calculs
            UIElement element = (UIElement)itemsControl.ItemContainerGenerator.ContainerFromItem(item);
            var contentPresenter = element as ContentPresenter;

            //Get image wrapped in the sender of type ContentPresenter
            var child2 = VisualTreeHelper.GetChild(contentPresenter, 0);

            //Cast object to image 
            Image image = child2 as Image;

            return image;


        }
        public Dictionary<string, double> GetItemPositionInScrollViewer(Image image)
        {

            Dictionary<string, double> results = new Dictionary<string, double>();


            //Get Position of the current inside the scrollViewer
            var position = image.TransformToVisual(ScrollView);
            Point p = position.TransformPoint(new Point(0, 0));

            /***CALCULS FOR LIMITATIONS***/

            //Distance between ScrollViewer's left border and the Image's left Border
            var LeftBorder = p.X;
            results.Add("LeftBorder", LeftBorder);

            //Distance between ScrollViewer's left border and the Image's rigth Border
            var RightBorder = p.X + (image.Width * ScrollView.ZoomFactor);
            results.Add("RightBorder", RightBorder);

            //Distance between ScrollViewer's top border and the Image's top
            var TopBorder = p.Y;
            results.Add("TopBorder", TopBorder);

            //Distance between ScrollViewer's top border and the Image's bottom
            var BottomBorder = p.Y + (image.Height * ScrollView.ZoomFactor);
            results.Add("BottomBorder", BottomBorder);

            return results;
        }



        public void Scrollview_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {

            ScrollViewer scrollViewer = sender as ScrollViewer;
            ScrollView = scrollViewer;

            var child = VisualTreeHelper.GetChild(scrollView, 0);
            var nb1 = VisualTreeHelper.GetChild(child, 0);
            var nb2 = VisualTreeHelper.GetChild(nb1, 0);
            var nb = VisualTreeHelper.GetChild(nb2, 0);
            ItemsControl itemsControl = nb as ItemsControl;
            var items = itemsControl.Items;

            foreach (var item in items)
            {
                var image = GetImageFromUIElement(item);

                var results = GetItemPositionInScrollViewer(image);
                /***CALCULS FOR LIMITATIONS***/

                //Distance between ScrollViewer's left border and the Image's left Border
                var LeftBorder = results["LeftBorder"];

                //Distance between ScrollViewer's left border and the Image's rigth Border
                var RightBorder = results["RightBorder"];

                //Distance between ScrollViewer's top border and the Image's top
                var TopBorder = results["TopBorder"];

                //Distance between ScrollViewer's top border and the Image's bottom
                var BottomBorder = results["BottomBorder"];


                //Retrieve image's tag to check on which element('current') we should apply the translation
                string tagImage = image.Tag.ToString();

                ElementPlaced current = ElementsPlaced.Select(el => el)
                                                      .Where(el => el.Element.ElementName.Equals(tagImage))
                                                      .First();

                if (LeftBorder < 0) { current.DeltaOnX += (-LeftBorder); }


                if (RightBorder > ScrollView.ViewportWidth) { current.DeltaOnX -= RightBorder; }


                if (TopBorder < 0) { current.DeltaOnY += (-TopBorder); }


                if (BottomBorder > ScrollView.ViewportHeight) { current.DeltaOnY -= BottomBorder; }

            }
        }

        public void Scrollview_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ScrollViewer scrollViewer = sender as ScrollViewer;
            // Set Scrollviewer size
            ScrollView = scrollViewer;
            SetInitialWidthToElements();
            NavigationView navigationView = GetNavigationView();
            navigationView.IsPaneOpen = false;
        }
        public void RedirectToAllGridsPage()
        {
            NavigationView nv = GetNavigationView();
            Frame child = nv.Content as Frame;
            NavigationViewModel navigationViewModel = child.DataContext as NavigationViewModel;
            navigationViewModel.ParameterToPass = null;
            navigationViewModel.Title = "Toutes les grilles";
            child.SourcePageType = typeof(AllGridsView);
        }

        public void ReloadGridFormView()
        {
            var navigationView = GetNavigationView();
            var child = navigationView.Content as Frame;
            child.SourcePageType = typeof(BlankPage1);
            child.SourcePageType = typeof(GridFormView);

        }
    }
}
