using humanlab.DAL;
using humanlab.Helpers.Models;
using humanlab.Models;
using humanlab.Views;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace humanlab.ViewModels
{
    public class AllGridsViewModel : BaseViewModel
    {
        //***ATTRIBUTS***//

        /// <summary>
        /// List of the all the grids in DB
        /// </summary>
        public List<Models.Grid> AllGrids { get; set; }

        /// <summary>
        /// Repository in charge of retrieving data linked to grids in db
        /// </summary>
        private GridRepository gridRepository;

        private bool isShowGridPreviewAlreadyCalled;
        private bool isEditModeActivated;
        private string editButton;


        /// <summary>
        /// Handle the visibility grid preview
        /// </summary>
        private bool isGridPreviewShowing;

        private List<ElementOfActivity> elements;

        public DelegateCommand CloseGridDelegate { get; set; }

        public DelegateCommand ChangeEditMode { get; set; }

        //***CONSTRUCTOR***//
        public AllGridsViewModel()
        {
            gridRepository = new GridRepository();
            GetAllGridsAsync();
            isShowGridPreviewAlreadyCalled = false;
            IsEditModeActivated = false;
            EditButton = "Modifier";
            ChangeEditMode = new DelegateCommand(SetEditMode);
            IsGridPreviewShowing = false;
            CloseGridDelegate = new DelegateCommand(CloseGridPreview);
        }

        /***GETTERS & SETTERS***/
        public List<ElementOfActivity> Elements
        {
            get => elements;
            set => SetProperty(ref elements, value, "Elements");
        }
        public bool IsEditModeActivated
        {
            get => isEditModeActivated;
            set => SetProperty(ref isEditModeActivated, value, "IsEditModeActivated");
        }
        public string EditButton
        {
            get => editButton;
            set => SetProperty(ref editButton, value, "EditButton");
        }

        public bool IsGridPreviewShowing
        {
            get => isGridPreviewShowing;
            set => SetProperty(ref isGridPreviewShowing, value, "IsGridPreviewShowing");
        }

        //***METHODS***//
        private async void GetAllGridsAsync()
        {
            var grids = await gridRepository.GetGridsAsync();
            AllGrids = grids.OrderByDescending(g => g.GridName.Length).ToList();
        }

        public void SetEditMode()
        {
            IsEditModeActivated = !IsEditModeActivated;
            if (EditButton.Equals("Modifier")) EditButton = "Fin Modification";
            else EditButton = "Modifier";
            
        }

        /// <summary>
        /// Method called when we click on a grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void GridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Check if the method is not calling itself
            if (!isShowGridPreviewAlreadyCalled)
            {
                GridView gv = sender as GridView;

                //Retrieve the grid we want to Play
                Models.Grid selected = gv.SelectedItem as Models.Grid;
                // Signal that we enter into the method once
                isShowGridPreviewAlreadyCalled = true;
                // Reset the grid selection
                gv.SelectedItem = null;

                if (!IsEditModeActivated)
                {
                    ShowGridPreview(selected);
                }
                else
                {
                    // Redirect toward the modification form
                    NavigationView navigation = GetNavigationView();
                    Frame child = navigation.Content as Frame;
                    NavigationViewModel navigationViewModel = child.DataContext as NavigationViewModel;
                    Object parameter = selected as Object;
                    // Passing parameter through the navigation viewmodel
                    navigationViewModel.ParameterToPass = parameter;
                    // Indicates which form we should open
                    child.SourcePageType = typeof(GridFormView);
                }
            }
        }

        /// <summary>
        /// This method is called to show the grid preview
        /// </summary>
        /// <param name="grid">the grid to show</param>
        public void ShowGridPreview(Models.Grid grid)
        {
            //we retrieve the elements of the grid to show
            GetElementsOfGrid(grid.GridId);
            //we show the preview
            IsGridPreviewShowing = true;
            //we close the menu to have the preview in all screen
            NavigationView navView = GetNavigationView();
            navView.IsPaneVisible = false;
            navView.IsPaneOpen = false;
            navView.IsPaneToggleButtonVisible = false;
            //we change the title of the page
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
            //we change the title of the page
            Frame child = navView.Content as Frame;
            NavigationViewModel navigationViewModel = child.DataContext as NavigationViewModel;
            navigationViewModel.Title = "Toutes les grilles";
            //we call the preview
            IsGridPreviewShowing = false;
            //we reset the needed values
            isShowGridPreviewAlreadyCalled = false;
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
