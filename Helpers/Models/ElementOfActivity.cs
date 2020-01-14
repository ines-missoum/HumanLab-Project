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


        private string imageName;
        private double focusTime;
        private bool isNotAnimated;
        public string Id { get; set; }
        public double MaxFocusTime { get; set; }
        public DelegateCommand<object>  ClickImage {get;set;}

    public ElementOfActivity(string id, string imageName, double focusTime, DelegateCommand<object> clickImage)
        {
            Id = id;
            ImageName = "ms-appx:///"+imageName;
            FocusTime = 0;
            IsNotAnimated = true;
            MaxFocusTime = focusTime;
            ClickImage = clickImage;

        }

        public string ImageName
        {
            get => imageName;
            set
            {
                if (value != imageName)
                {
                    imageName = value;
                    OnPropertyChanged("ImageName");
                }
            }
        }
        public double FocusTime
        {
            get => focusTime;
            set
            {
                if (value != focusTime)
                {
                    focusTime = value;
                    OnPropertyChanged("FocusTime");
                }
            }
        }
        public bool IsNotAnimated
        {
            get => isNotAnimated;
            set
            {
                if (value != isNotAnimated)
                {
                    isNotAnimated = value;
                    OnPropertyChanged("IsNotAnimated");
                }
            }
        }


    }
}
