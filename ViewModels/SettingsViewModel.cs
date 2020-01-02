using Prism.Commands;
using System;
using System.Diagnostics;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace humanlab.ViewModels
{
    class SettingsViewModel : BaseViewModel
    {
        private bool isAutoChecked;
        private double gridTime;
        private string selectedMode;
        public String ColorButton { get; set; }
        public DelegateCommand SaveSettingsCommand { get; set; }

        public SettingsViewModel()
        {
            this.isAutoChecked = false;
            this.gridTime = 3000;
            this.selectedMode = "Boucle";
            ColorButton = ColorTheme;
            SaveSettingsCommand = new DelegateCommand(SaveSettings, CanSaveSettings);
        }

        public void Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Slider slider = sender as Slider;
            if (slider != null)
            {
                GridTime = slider.Value;
            }
        }

        public void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            string selected = comboBox.SelectedItem.ToString();
            if (selected != null)
            {
                SelectedMode = selected;
            }
        }
        private void SaveSettings()
        {

        }

        bool CanSaveSettings()
        {
            return true;
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
                    SaveSettingsCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public double GridTime
        {
            get => gridTime;
            set
            {
                if (value != gridTime)
                {
                    gridTime = value;
                    OnPropertyChanged("GridTime");
                }
            }
        }

        public string SelectedMode
        {
            get => selectedMode;
            set
            {
                if (value != selectedMode)
                {
                    selectedMode = value;
                    OnPropertyChanged("SelectedMode");
                }
            }
        }
    }
}
