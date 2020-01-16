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
        public Boolean isCallFromSC2 = false;
        public Boolean isConsequence = false;
        public Boolean IsCallFromSC2
        {
            get => isCallFromSC2;
            set
            {
                if (value != isCallFromSC2)
                {
                    isCallFromSC2 = value;
                    OnPropertyChanged("IsCallFromSC2");
                }
            }
        }

        public Boolean IsConsequence
        {
            get => isCallFromSC2;
            set
            {
                if (value != isConsequence)
                {
                    isConsequence = value;
                    OnPropertyChanged("IsConsequence");
                }
            }
        }

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
            List<Item> allList = new List<Item>(Items);


            Debug.WriteLine(" \n *********** selection 2 changé");

            GridView gridview = sender as GridView;
            Debug.WriteLine(" removed size:  "+ e.RemovedItems.Count());
            Debug.WriteLine(" added size:  "+e.AddedItems.Count());
            if(e.RemovedItems.Count() == 1)
            {
                if (Slitems.Count == 1 && IsConsequence) { }
                else {

                    IsCallFromSC2 = true;
                    Debug.WriteLine(" Premier if : e.R==1 && Slitem.count !=1");
                    Item removedItem = e.RemovedItems.Last() as Item;
                    Debug.WriteLine(" removeditem:  " + removedItem.Text);
                    updatedList.Remove(removedItem);
                    IsConsequence = true;
                    Slitems = updatedList;
                    IsConsequence = false;
                    allList.Remove(removedItem);
                    Items = allList;
                    allList.Add(removedItem);
                    Items = allList;
                    IsCallFromSC2 = false;
                    Debug.WriteLine("\n On repasse FromSC2 à false");
                }
            }
            Debug.WriteLine("\n");
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
            Debug.WriteLine(" \n Je suis Dans le selectionChanged 1 ");
            // Peut etre selecteditem.remove(i)
            Debug.WriteLine(" removed size:  " + e.RemovedItems.Count());
            Debug.WriteLine(" added size:  " + e.AddedItems.Count());
            Debug.WriteLine("isFromSC2"+ IsCallFromSC2);
            List<Item> updatedList = new List<Item>(Slitems);
            GridView gridview = sender as GridView;

            if (e.RemovedItems.Count() == 1 && !IsCallFromSC2)
            {
                Debug.WriteLine(" Premier if : e.R==1 && fromSc2=false");
                Item removedItem = e.RemovedItems.First() as Item;
                Debug.WriteLine(" removeditem:  " + removedItem.Text);
                updatedList.Remove(removedItem);
                Slitems = updatedList;
            }
            else
            {
                if (e.AddedItems.Count() == 1 && !IsCallFromSC2) {
                    Debug.WriteLine(" Deuxieme if : e.A==1 ");
                    Item addedItem = e.AddedItems.First() as Item;
                    Debug.WriteLine(" addeditem:  " + addedItem.Text);
                    updatedList.Add(addedItem);
                    Slitems = updatedList;
                }
                else
                {
                    if (e.RemovedItems.Count() > 0)
                    {
                        Debug.WriteLine(" Troisieme if : e.R>0");
                        foreach (Item item in e.RemovedItems)
                        {
                            gridview.SelectedItems.Add(item);
                        }
                    }
                }
            }

        }
    }
}
