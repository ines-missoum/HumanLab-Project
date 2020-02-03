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
        public double Height { get; set; }
        public double Width { get; set; }
        public double MarginLeft { get; set; }
        public double MarginTop { get; set; }
        public Element Element { get; set; }
        public DelegateCommand<object> ClickImage { get; set; }

        public ElementOfActivity(Element element, double marginLeft, double marginTop, double height, double width)
        {
            Element = element;
            FocusTime = 0;
            IsNotAnimated = true;
            Height = height;
            Width = width;
            MarginLeft = marginLeft;
            MarginTop = marginTop;
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

    }
}