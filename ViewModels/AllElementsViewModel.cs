using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using humanlab.DAL;
using humanlab.Models;
using System.Diagnostics;
using Windows.UI.Xaml.Media.Imaging;
using Prism.Commands;
using Windows.UI.Xaml.Controls;
using humanlab.Views;
using Windows.UI.Xaml;

namespace humanlab.ViewModels
{
    class AllElementsViewModel : BaseViewModel
    {
        public List<Element> AllElements { get; set; }
        private bool isEditModeActivated;
        private string editButton;
        private string selectionMode;

        public DelegateCommand ChangeEditMode { get; set; }

        Repository repository;

        public AllElementsViewModel()
        {
            this.repository = new Repository();
            GetElementsAsync();
            IsEditModeActivated = false;
            EditButton = "Modifier";
            ChangeEditMode = new DelegateCommand(SetEditMode);
            selectionMode = "None";
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
        public string SelectionMode
        {
            get => selectionMode;
            set => SetProperty(ref selectionMode, value, "SelectionMode");
        }

        public void SetEditMode()
        {
            IsEditModeActivated = !IsEditModeActivated;
            if (EditButton.Equals("Modifier"))
            {
                EditButton = "Fin Modification";
                SelectionMode = "Single";
            }
            else { EditButton = "Modifier";
                SelectionMode = "None";
            }
            Debug.WriteLine(" is edit " + IsEditModeActivated);
        }


        private async void GetElementsAsync()
        {
            var elements = await repository.GetElementsAsync();
            AllElements = elements.OrderByDescending(e => e.ElementName.Length).ToList(); 
        }

        public void GridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (IsEditModeActivated)
            {
                GridView gv = sender as GridView;
                Element selected = gv.SelectedItem as Element;
                Debug.WriteLine("selected " + selected);
                NavigationView navigation = GetNavigationView();
                Frame child = navigation.Content as Frame;
                NavigationViewModel navigationViewModel = child.DataContext as NavigationViewModel;
                Object parameter = selected as Object;
                navigationViewModel.ParameterToPass = parameter;
                child.SourcePageType = typeof(ElementFormView);
                
            }
        }

    }
}
