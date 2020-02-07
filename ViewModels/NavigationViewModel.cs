﻿using humanlab.Views;
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
            contentFrame = typeof(ActivityLoadingView);
            title = "Lancer une activité";
            ColorNav = ColorTheme;
        }

        public String ColorNav { get; set; }
        /// <summary>
        /// the current view
        /// </summary>
        public Type contentFrame;
        /// <summary>
        /// the current view title
        /// </summary>
        public String title;
        public Object parameterToPass;

        /*** GETTERS AND SETTERS ***/
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
        public Object ParameterToPass
        {
            get => parameterToPass;
            set => SetProperty(ref parameterToPass, value, "ParameterToPass");
        }


        /*** METHODS ***/
        /// <summary>
        /// Method called when the navigation view is loaded (ie : in our case when the application is loaded)
        /// </summary>
        /// <param name="sender">navigation view</param>
        /// <param name="args">arguments</param>
        public void nvTopLevelNav_Loaded(object sender, RoutedEventArgs e)
        {
            NavigationView NavView = sender as NavigationView;
            NavView.SelectedItem = NavView.MenuItems[1];
        }

        public void nvTopLevelNav_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            // we use itemInvoked instead
        }

        /// <summary>
        /// Method called when we click on a menu item
        /// </summary>
        /// <param name="sender">navigation view</param>
        /// <param name="args">arguments</param>
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
                            this.ContentFrame = typeof(ActivityLoadingView);
                            break;
                        case "Nouvelle activité":
                            this.ContentFrame = typeof(ActivityFormView);
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
                        case "Gestion des catégories":
                            this.ContentFrame = typeof(CategoriesManagementView);
                            break;
                    }
                }
                ParameterToPass = null;
            }
        }
    }
}
