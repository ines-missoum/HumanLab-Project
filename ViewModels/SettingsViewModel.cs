using Prism.Commands;
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
        public String ColorButton { get; set; }
        public DelegateCommand SaveSettingsCommand { get; set; }

        public SettingsViewModel()
        {
            this.isManualChecked = false;
            this.isAutoChecked = false;
            ColorButton = ColorTheme;
            SaveSettingsCommand = new DelegateCommand(SaveSettings, CanSaveSettings);
        }
        private void SaveSettings()
        {

        }

        bool CanSaveSettings()
        {
            return IsAutoChecked || IsManualChecked;
        }
        public bool IsManualChecked
        {
            get => isManualChecked;
            set
            {
                if (value != isManualChecked)
                {
                    isManualChecked = value;
                    SaveSettingsCommand.RaiseCanExecuteChanged();
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
                    SaveSettingsCommand.RaiseCanExecuteChanged();
                }
            }
        }
    }
}
