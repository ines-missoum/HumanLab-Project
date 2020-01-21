using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using humanlab.Models;
using humanlab.ViewModels;

namespace humanlab.Helpers.Models
{
    class ElementPlaced : BaseViewModel
    {

        public Element Element { get; set; }
        public double positionX;

        public double positionY;

        public double Width { get; set; }
        public double Heigth { get; set; }


        public ElementPlaced(Element element, double x, double y, double width, double heigth){
            this.Element = element;
            this.positionX = x;
            this.positionY = y;
            this.Width = width;
            this.Heigth = heigth;
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
