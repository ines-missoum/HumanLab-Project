using humanlab.Models;
using humanlab.ViewModels;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace humanlab.Helpers.Models
{
    public class ElementOfActivity : BaseViewModel
    {

        private double focusTime;
        private bool isNotAnimated;
        public double MaxFocusTime { get; set; }
        public Element Element { get; set; }
        public DelegateCommand<object> ClickImage { get; set; }

        public ElementOfActivity(Element element, double focusTime, DelegateCommand<object> clickImage)
        {
            Element = element;
            FocusTime = 0;
            IsNotAnimated = true;
            MaxFocusTime = focusTime;
            ClickImage = clickImage;
        }

        public double FocusTime
        {
            get => focusTime;
            set => SetProperty(ref focusTime, value, "FocusTime");
        }

        public bool IsNotAnimated
        {
            get => isNotAnimated;
            set => SetProperty(ref isNotAnimated, value, "IsNotAnimated");
        }

        /*public Element Element
        {
            get => element;
            set => SetProperty(ref element, value, "Element");
        }*/


    }
}