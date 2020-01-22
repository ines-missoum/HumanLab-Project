using humanlab.Views;
using System;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace humanlab.ViewModels
{
    public class NavigationViewModel : BaseViewModel
    {
        public NavigationViewModel()
        {
            contentFrame = typeof(AllActivitiesView);
            title = "Accueil";
            ColorNav = ColorTheme;
        }

        public String ColorNav { get; set; }
        public Type contentFrame;
        public String title;

        public String Title
        {
            get => title;
            set => SetProperty(ref title, value, "Title");
        }

        public Type ContentFrame
        {
            get => contentFrame;
            set => SetProperty(ref contentFrame, value, "ContentFrame");
        }


        public void nvTopLevelNav_Loaded(object sender, RoutedEventArgs e)
        {
            NavigationView NavView = sender as NavigationView;
            NavView.SelectedItem = NavView.MenuItems[1];
            Debug.WriteLine(NavView.MenuItems[1]);
        }

        public void nvTopLevelNav_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            // we use itemInvoked instead
        }

        public void nvTopLevelNav_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {

            if (args.IsSettingsInvoked)
            {
                this.Title = "Paramètres";
                this.ContentFrame = typeof(SettingsView);
            }
            else
            {
                string ItemContent = args.InvokedItem as string;

                if (ItemContent != null)
                {
                    this.Title = ItemContent;
                    switch (ItemContent)
                    {
                        
                        case "Toutes les activités":
                            this.ContentFrame = typeof(AllActivitiesView);
                            break;
                        case "Nouvelle activité":
                            break;
                        case "Toutes les grilles":
                            break;
                        case "Nouvelle grille":
                            this.ContentFrame = typeof(GridFormView);
                            break;
                        case "Tous les éléments":
                            this.ContentFrame = typeof(AllElementsView);
                            break;
                        case "Nouvel élément":
                            this.ContentFrame = typeof(ElementFormView);
                            break;
                    }
                }

            }
        }
    }
}
