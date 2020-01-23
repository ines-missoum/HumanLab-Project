using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using humanlab.Models;
using humanlab.ViewModels;
using Windows.Foundation;
using Windows.UI.Xaml.Input;

namespace humanlab.Helpers.Models
{
    public class ElementPlaced : BaseViewModel
    {

        public Element Element { get; set; }
        public double positionX;

        public double positionY;

        public double width;
        public double heigth;
        public string widthString;
        public string heigthString;

       // public ManipulationDeltaEventHandler Image_ManipulationDelta { get; set; }


        public ElementPlaced(Element element, double x, double y, double width, double heigth)
        {
            this.Element = element;
            this.positionX = x;
            this.positionY = y;
            this.width = width;
            this.heigth = heigth;
            this.widthString = width.ToString();
            this.heigthString = heigth.ToString();
           // this.Image_ManipulationDelta = image_ManipulationDelta;
        }

        public double Heigth
        {
            get => heigth;
            set
            {
                if (value != heigth)
                {
                    heigth = value;
                    OnPropertyChanged("Heigth");


                }
            }
        }


        public string HeigthString
        {
            get => heigthString;
            set
            {
                if (value != heigthString)
                {
                    heigthString = value;
                    OnPropertyChanged("HeigthString");


                }
            }
        }
        public double Width
        {
            get => width;
            set
            {
                if (value != width)
                {
                    width = value;
                    OnPropertyChanged("Width");


                }
            }
        }
        public string WidthString
        {
            get => widthString;
            set
            {
                if (value != widthString)
                {
                    widthString = value;
                    OnPropertyChanged("WidthString");


                }
            }
        }


        public double PositionX
        {
            get => positionX;
            set
            {
                if (value != positionX)
                {
                    positionX = value;
                    OnPropertyChanged("PositionX");


                }
            }
        }

        public double PositionY
        {
            get => positionY;
            set
            {
                if (value != positionY)
                {
                    positionY = value;
                    OnPropertyChanged("PositionY");


                }
            }
        }
    }
}