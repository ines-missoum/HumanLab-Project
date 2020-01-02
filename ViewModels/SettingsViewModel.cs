using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace humanlab.ViewModels
{
    class SettingsViewModel : BaseViewModel
    {
        private bool isManualChecked = false;
        private bool isAutoChecked = false;

        public SettingsViewModel()
        {
            this.isManualChecked = false;
            this.isAutoChecked = false;
        }

        public bool IsManualChecked
        {
            get => isManualChecked;
            set
            {
                if (value != isManualChecked)
                {
                    isManualChecked = value;
                    OnPropertyChanged("IsManualChecked");
                }
            }
        }

        public bool IsAutoChecked
        {
            get => isAutoChecked;
            set
            {
                if (value != isAutoChecked)
                {
                    isAutoChecked = value;
                    OnPropertyChanged("IsAutoChecked");
                }
            }
        }
    }
}
