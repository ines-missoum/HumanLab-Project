using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using humanlab.Views;
using System.Diagnostics;

namespace humanlab.Services
{
    public class NavigationService
    {
        private NavigationView Nav = null;

        public NavigationService()
        {
            ContentFrame = typeof(CreateElementView);
        }

        public Type ContentFrame { get; set; }
        public string page { get; set; } = "using";
        public String Title { get; set; } = typeof(CreateElementView).ToString();

        
        public void nvTopLevelNav_Loaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("load");
            // set the initial SelectedItem
            /* foreach (NavigationViewItemBase item in Nav.MenuItems)
             {
                 if (item is NavigationViewItem && item.Tag.ToString() == "Nav_Activity_All")
                 {
                     Nav.SelectedItem = item;
                     break;
                 }
             }
             ContentFrame.Navigate(typeof(AllActivitiesView));*/
        }

        public void nvTopLevelNav_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            Debug.WriteLine("select");
            // we use itemInvoked instead
        }

        public void nvTopLevelNav_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            Debug.WriteLine("invoked");
            if (args.IsSettingsInvoked)
            {
                //ContentFrame.Navigate(typeof(SettingsView));
            }
            else
            {
                Debug.WriteLine(args.InvokedItem.GetType());
                string ItemContent = args.InvokedItem as string;
               
                if (ItemContent != null)
                {
                    Debug.WriteLine("not null");
                    switch (ItemContent)
                    {
                        case "Nouvel element":
                            Debug.WriteLine("testok");
                            Debug.WriteLine(ContentFrame);
                            Debug.WriteLine(ContentFrame);
                            ((Frame)Window.Current.Content).Navigate(typeof(CreateElementView));
                            //ContentFrame.Navigate(typeof(CreateElementView));
                            break;
                    }
                }
                else
                {
                    Debug.WriteLine("null");
                }
            }
        }
      
    }
}
