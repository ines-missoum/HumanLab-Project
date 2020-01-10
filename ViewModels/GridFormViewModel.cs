using humanlab.Helpers.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace humanlab.ViewModels
{
    public class GridFormViewModel : BaseViewModel
    {
        public GridFormViewModel()
        {
            SearchedElements = new List<ElementChecked>();
            SearchedElements.Add(new ElementChecked("hello", false));
            SearchedElements.Add(new ElementChecked("salut", false));
            SearchedElements.Add(new ElementChecked("hola", true));
            SearchedElements.Add(new ElementChecked("test", false));

            SelectedElements = SearchedElements.Where(e => (e.IsSelected == true)).ToList();

        }

        private List<ElementChecked> searchedElements;
        private List<ElementChecked> selectedElements;

        public List<ElementChecked> SearchedElements
        {
            get => searchedElements;
            set
            {
                if (value != searchedElements)
                {
                    searchedElements = value;
                    OnPropertyChanged("SearchedElements");
                }
            }
        }
        public List<ElementChecked> SelectedElements
        {
            get => selectedElements;
            set
            {
                if (value != selectedElements)
                {
                    selectedElements = value;
                    OnPropertyChanged("SelectedElements");
                }
            }
        }
        public void OnElementSelection(object sender, SelectionChangedEventArgs e)
        {
           
            GridView gv = sender as GridView;
            Debug.WriteLine("ok");
            Debug.WriteLine(gv.SelectedItem);
            
            gv.SelectedItem = SearchedElements;

        }
    }
}
