using humanlab.Services;
using Prism.Commands;
using System;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace humanlab.ViewModels
{
    class SettingsViewModel : BaseViewModel
    {
        private bool isAutoChecked;
        private double gridTime;
        private string selectedMode;
        private bool isSomethingChanged;
        public String ColorButton { get; set; }
        public DelegateCommand SaveSettingsCommand { get; set; }

        public SettingsViewModel()
        {
            this.isAutoChecked = ParametersService.IsAutomatic();
            this.gridTime = ParametersService.GetGridTime();
            this.selectedMode = ParametersService.GetMode();
            this.isSomethingChanged = false;
            ColorButton = ColorTheme;
            SaveSettingsCommand = new DelegateCommand(SaveSettingsAsync, CanSaveSettings);
        }

        public void Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Slider slider = sender as Slider;
            if (slider != null && GridTime != slider.Value)
            {
                isSomethingChanged = true;
                GridTime = slider.Value;

            }
        }

        public void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            string selected = comboBox.SelectedItem.ToString();
            if (selected != null && SelectedMode != selected)
            {
                isSomethingChanged = true;
                SelectedMode = selected;
            }
        }
        private async void SaveSettingsAsync()
        {
            MessageDialog messageDialog;
            try
            {
                if (IsAutoChecked)
                {
                    ParametersService.SaveAutomatic(GridTime, SelectedMode);
                }
                else
                {
                    ParametersService.SaveManual();
                }
                // Create the message dialog and set its content
                messageDialog = new MessageDialog("Vos modifications ont été sauvegardées avec succès.");

            }
            catch
            {
                messageDialog = new MessageDialog("Une erreur s'est produite lors de la sauvegarde. Veuillez réessayer.");
            }
            // Show the message dialog
            await messageDialog.ShowAsync();

        }

        bool CanSaveSettings()
        {
            return isSomethingChanged;
        }

        public bool IsAutoChecked
        {
            get => isAutoChecked;
            set
            {
                if (value != isAutoChecked)
                {
                    isAutoChecked = value;
                    isSomethingChanged = true;
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
