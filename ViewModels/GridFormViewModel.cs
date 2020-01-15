using humanlab.Helpers.Models;
using humanlab.Models;
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

        private List<ElementChecked> searchedElements;
        private List<ElementChecked> selectedElements;
        private List<ElementChecked> allElements;

        public GridFormViewModel()
        {
            SearchedElements = new List<ElementChecked>();
            SearchedElements.Add(new ElementChecked(new Element{ ElementName = "hello"}, false));
            SearchedElements.Add(new ElementChecked(new Element { ElementName = "salut" }, false));
            SearchedElements.Add(new ElementChecked(new Element { ElementName = "hola" }, true));
            SearchedElements.Add(new ElementChecked(new Element { ElementName = "test" }, false));

            SelectedElements = SearchedElements.Where(e => (e.IsSelected == true)).ToList();

        }

        public List<ElementChecked> AllElements
        {
            get => allElements;
            set
            {
                if (value != allElements)
                {
                    allElements = value;
                    OnPropertyChanged("AllElements");
                }
            }
        }
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
