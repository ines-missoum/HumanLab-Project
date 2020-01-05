using System;
using System.Collections.Generic;
using Prism.Commands;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.Storage.Pickers;
using Windows.Storage;

namespace humanlab.ViewModels
{


    class ElementFormViewModel : BaseViewModel
    {
        private bool isToggleChecked;
        private StorageFile selectedPicture;
        public string selectedPictureName;
        private string[] authorizedFileType1 = { "jpeg", "png", "jpg" };
        private string[] authorizedFileType2 = { "mp4", "mp3" };
        public ElementFormViewModel()
        {
            this.isToggleChecked = false;
            this.selectedPictureName= "example.png";

        }

        public void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch != null)
            {
                System.Diagnostics.Debug.WriteLine("toooo" + toggleSwitch.IsOn);
                IsToggleChecked = toggleSwitch.IsOn;

            }
        }

        public bool IsToggleChecked
        {
            get => isToggleChecked;
            set
            {
                if (value != isToggleChecked)
                {
                    isToggleChecked = value;
                    
                    System.Diagnostics.Debug.WriteLine("dedans o" + value);
                    OnPropertyChanged("IsToggleChecked");

                }
            }
        }

        public bool IsToggleNotChecked => IsToggleChecked;

        public async void ChoosePicture(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            foreach (string type in authorizedFileType1)
            {
                string typeFile = "." + type;
                openPicker.FileTypeFilter.Add(typeFile);
            }
            StorageFile file = await openPicker.PickSingleFileAsync();

            if (file != null)
            {
                SelectedPicture= file;
                SelectedPictureName= "d";
                System.Diagnostics.Debug.WriteLine("aaaaaaaaaaaaoo" + file.Name);

            }

        }

        public StorageFile SelectedPicture
        {
            get => selectedPicture;
            set
            {
                if (value != selectedPicture)
                {
                    selectedPicture= value;
                    OnPropertyChanged("SelectedPictureChanged");

                }
            }
        }

        public string SelectedPictureName
        {
            get => selectedPictureName;
            set
            {
                if (value != selectedPictureName)
                {
                    selectedPictureName= value;
                    System.Diagnostics.Debug.WriteLine("value" + this.selectedPictureName);
                    OnPropertyChanged("SelectedPictureNameChanged");

                }
            }
        }

        
/*
        public string GetPictureName()
        {

            if (this.selectedPicture != null)
            {
                return this.selectedPicture.Name;
            }
            else return "example.png";
        }
        */
    }
    
}