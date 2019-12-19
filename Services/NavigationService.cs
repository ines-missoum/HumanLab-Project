using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using humanlab.Views;

namespace humanlab.Services
{
    public class NavigationService
    {
        private NavigationView Nav = null;
        private Frame ContentFrame = null;
        public String text;

        private void nvTopLevelNav_Loaded(object sender, RoutedEventArgs e)
        {
            // set the initial SelectedItem
            foreach (NavigationViewItemBase item in Nav.MenuItems)
            {
                if (item is NavigationViewItem && item.Tag.ToString() == "Nav_Activity_All")
                {
                    Nav.SelectedItem = item;
                    break;
                }
            }
            ContentFrame.Navigate(typeof(AllActivitiesView));
        }

        private void nvTopLevelNav_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            // we use itemInvoked instead
        }

        private void nvTopLevelNav_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                ContentFrame.Navigate(typeof(SettingsView));
            }
            else
            {
                TextBlock ItemContent = args.InvokedItem as TextBlock;
                if (ItemContent != null)
                {
                    switch (ItemContent.Tag)
                    {
                        case "Nav_Element_New":
                            text = "blabla";
                            //ContentFrame.Navigate(typeof(CreateElementView));
                            break;
                    }
                }
            }
        }
      
    }
}
