using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Diagnostics;
using Windows.UI.Xaml.Input;
using humanlab.Models;
using humanlab.Helpers.Models;

namespace humanlab.ViewModels
{
    class ElementsPositioningViewModel : BaseViewModel
    {
  
        private double marginWindow_ScrollViewer;

        private double scrollViewerHeigth;

        private double scrollViewerWidth;

        private List<Element> elements;
        private List<ElementPlaced> elementsPlaced;


        public ElementsPositioningViewModel()
        {
            this.marginWindow_ScrollViewer = 0;
            this.elementsPlaced = new List<ElementPlaced>();
            this.elements = new List<Element>();

        }

        private void Grid_Loading(FrameworkElement sender, object args)
        {
            foreach(Element element in Elements)
            {
                ElementPlaced ep = new ElementPlaced(element, 0, 0, 0, 0);
            }
        }

 
    public List<Element> Elements
        {
            get => elements;
            set
            {
                if (value != elements)
                {
                    elements = value;
                    OnPropertyChanged("Elements");
                }
            }
        }
        public List<ElementPlaced> ElementsPlaced
        {
            get => elementsPlaced;
            set
            {
                if (value != elementsPlaced)
                {
                    elementsPlaced = value;
                    OnPropertyChanged("ElementsPlaced");
                }
            }
        }
        public Double MarginWindow_ScrollViewer
        {
            get => marginWindow_ScrollViewer;
            set
            {
                if (value != marginWindow_ScrollViewer)
                {
                    marginWindow_ScrollViewer = value;
                    OnPropertyChanged("MarginWindow_ScrollViewer");
                }
            }
        }

        public Double ScrollViewerHeigth
        {
            get => scrollViewerHeigth;
            set
            {
                if (value != scrollViewerHeigth)
                {
                    scrollViewerHeigth = value;
                    OnPropertyChanged("ScrollViewerHeigth");
                }
            }
        }

        public Double ScrollViewerWidth
        {
            get => scrollViewerWidth;
            set
            {
                if (value != scrollViewerWidth)
                {
                    scrollViewerWidth = value;
                    OnPropertyChanged("ScrollViewerWidth");
                }
            }
        }
        public void Image_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            Image image = sender as Image;
      
        }

        public void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double windowHeigth = Window.Current.Bounds.Height;
            MarginWindow_ScrollViewer = windowHeigth - ScrollViewerHeigth;
        }

        public void Scrollview_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ScrollViewer scrollViewer = sender as ScrollViewer;

            // Get Scrollviewer size
            ScrollViewerHeigth = scrollViewer.ViewportHeight;
            ScrollViewerWidth = scrollViewer.ViewportWidth;

            // Get Window App size
            double windowHeigth = Window.Current.Bounds.Height;

            // Calculate difference between scrollViewer and Frame Title 
            MarginWindow_ScrollViewer = windowHeigth - ScrollViewerHeigth;
        }

    }
}
