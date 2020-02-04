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
        /// <summary>
        /// Indicayes if we go to the next grid automatically or not
        /// </summary>
        private bool isAutoChecked;
        /// <summary>
        /// Time after wich we go automatically to go the next grid
        /// </summary>
        private double gridTime;
        /// <summary>
        /// Indicates how we go automatically to the next grid (ie : random, loop ...)
        /// </summary>
        private string selectedMode;
        /// <summary>
        /// Indicates if something changed in the form from the previous saved settings
        /// </summary>
        private bool isSomethingChanged;
        public String ColorButton { get; set; }
        public DelegateCommand SaveSettingsCommand { get; set; }

        public SettingsViewModel()
        {
            // we retrieve the previous saved settings
            this.isAutoChecked = ParametersService.IsAutomatic();
            this.gridTime = ParametersService.GetGridTime();
            this.selectedMode = ParametersService.GetMode();
            this.isSomethingChanged = false;
            ColorButton = ColorTheme;
            SaveSettingsCommand = new DelegateCommand(SaveSettingsAsync, CanSaveSettings);
        }

        //***GETTERS AND SETTERS***//
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
                    isSomethingChanged = true;
                    OnPropertyChanged("GridTime");
                    SaveSettingsCommand.RaiseCanExecuteChanged();
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
                    isSomethingChanged = true;
                    OnPropertyChanged("SelectedMode");
                    SaveSettingsCommand.RaiseCanExecuteChanged();
                }
            }
        }

        /*** METHODS ***/

        /// <summary>
        /// Retrieve and save the value of the slider
        /// </summary>
        /// <param name="sender">the slider</param>
        /// <param name="e">arguments</param>
        public void Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Slider slider = sender as Slider;
            if (slider != null && GridTime != slider.Value)
            {
                isSomethingChanged = true;
                GridTime = slider.Value;

            }
        }

        /// <summary>
        /// Retrieve and save the value of the combo box
        /// </summary>
        /// <param name="sender">the comboo box</param>
        /// <param name="e">arguments</param>
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

        /// <summary>
        /// Save the new parameters not in data base but in the application data.
        /// </summary>
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
                    ParametersService.SaveManual(SelectedMode);
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

        /// <summary>
        /// Indicates if the saved button is allowed.
        /// </summary>
        /// <returns></returns>
        bool CanSaveSettings()
        {
            //while nothing changed we can't saved
            return isSomethingChanged;
        }

    }
}
