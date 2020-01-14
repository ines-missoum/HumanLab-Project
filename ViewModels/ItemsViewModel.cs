using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using humanlab.Helpers.Models;
using Windows.UI.Xaml.Controls;
using System.Diagnostics;
using Windows.UI.Xaml;

namespace humanlab.ViewModels
{
    public class ItemsViewModel : BaseViewModel
    {
        public List<Item> items = new List<Item>();
        public List<Item> slitems = new List<Item>();
        public List<Item> Items { 
            get => items;
            set
            {
                if (value != items)
                {
                    items = value;
                    OnPropertyChanged("Items");
                }
            }
        }

        public List<Item> Slitems
        {
            get => slitems;
            set
            {
                if (value != slitems)
                {
                    slitems = value;
                    OnPropertyChanged("Slitems");
                }
            }
        }

        private Item selectedItem;
        public Item SelectedItem
        {
            get { return selectedItem; }
            set
            {
                if (selectedItem != value)
                {
                    if (selectedItem != null)
                    {
                        selectedItem.IsSelected = false;
                    }

                    if (value != selectedItem)
                    {
                        selectedItem = value;
                        OnPropertyChanged("SelectedItem");
                    }

                    if (selectedItem != null)
                    {
                        selectedItem.IsSelected = true;
                    }
                }
            }
        }
        public ItemsViewModel()
        {
            Items = new List<Item>()
        {
            new Item() { Text = "Apple"},
            new Item() { Text = "Banana", IsSelected=true},
            new Item() { Text = "Orange", IsSelected=false},
            new Item() { Text = "Strawberry", IsSelected=false},
            new Item() { Text = "Else", IsSelected=true},
        };
        }



        public void GridView1_Loading(FrameworkElement sender, object args)
        {
            Debug.WriteLine(" Je suis Dans le 1er loading \n ");
            Slitems = new List<Item>();
            GridView gridview1 = sender as GridView;
            foreach(Item item in Items)
            {
                if(item.IsSelected)
                {
                    gridview1.SelectedItems.Add(item);
                    if (!Slitems.Contains(item)) { Slitems.Add(item); }


                }
            }

            Debug.WriteLine(" Dans sl on a " + Slitems.Count() +" items");

        }

        public void GridView2_Loading(FrameworkElement sender, object args)
        {
            Debug.WriteLine(" Je suis Dans le 2eme loading \n ");
            GridView gridview2 = sender as GridView;
            gridview2.SelectAll();
            }

        public void GridView2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<Item> updatedList = new List<Item>(Slitems);
            List<Item> allItemsList = new List<Item>(Items);

            Debug.WriteLine(" \n *********** selection 2 changé");

            GridView gridview = sender as GridView;

            gridview.SelectAll();

        }

        public void GridView2_SizeChanged(object sender, SizeChangedEventArgs e)
        {

            Debug.WriteLine(" \n *********** Taille à changé");
            GridView gridview = sender as GridView;
            gridview.SelectAll();
        }

        public void GridView1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Debug.WriteLine(" Je suis Dans le selectionChanged \n ");
            // Peut etre selecteditem.remove(i)

            List<Item> updatedList = new List<Item>(Slitems);

            if (e.AddedItems.Count()>0)
            {
                Item addedItem = e.AddedItems.First() as Item;
                updatedList.Add(addedItem);
            }
            else
            {
                Item removedItem = e.RemovedItems.First() as Item;
                updatedList.Remove(removedItem);
            }
            Slitems = updatedList; 
 
            /*
            GridView lv = sender as GridView;
            Item orange = new Item() { Text = "Orange"};
            Item strawberry= new Item() { Text = "Strawberry" };
            lv.Items.Last();

 
            List<Item> l2 = new List<Item>(Items);
            l2.Add(orange);
     
            Items = l2;
            //Items.Add(m);
            lv.SelectedItems.Add(lv.Items.First());
            lv.SelectedIndex = 1;
            foreach (Item item in e.RemovedItems)
            {
                item.IsSelected = false;
            }

            foreach (Item item in e.AddedItems)
            {
                item.IsSelected = true;
            }
            */
        }
    }
}
