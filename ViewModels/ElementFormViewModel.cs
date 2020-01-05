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
using Windows.UI.Xaml.Media.Imaging;
using System.IO;
using Windows.Storage.Streams;

namespace humanlab.ViewModels
{


    class ElementFormViewModel : BaseViewModel
    {
        private string elementName;
        private string elementText;
        private bool isToggleChecked;
        private StorageFile selectedPicture;
        private StorageFile selectedAudio;
        private string selectedPictureName;
        private BitmapImage image = new BitmapImage();
        private MediaElement mediaPlayer = new MediaElement();
        private string[] authorizedPictureType = { "jpeg", "png", "jpg" };
        private string[] authorizedAudioType = { "mp4", "mp3" };
        public ElementFormViewModel()
        {
            this.elementName = "";
            this.elementText = "";
            this.isToggleChecked = false;
            this.selectedPictureName= "example.png";

        }

        public string ElementName
        {
            get => elementName;
            set
            {
                if (value != elementName)
                {
                    elementName = value;
                    OnPropertyChanged("ElementName");

                }
            }
        }

        public BitmapImage Image
        {
            get => image;
            set
            {
                if (value != image)
                {
                    image = value;
                    OnPropertyChanged("ImageSource");

                }
            }
        }

        public string ElementText
        {
            get => elementText;
            set
            {
                if (value != elementText)
                {
                    elementText = value;
                    OnPropertyChanged("ElementText");

                }
            }
        }



        public MediaElement MediaPlayer
        {
            get => mediaPlayer;
            set
            {
                if (value != mediaPlayer)
                {
                    mediaPlayer = mediaPlayer;
                    OnPropertyChanged("MediaPlayer");

                }
            }
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

        public bool IsToggleNotChecked => !IsToggleChecked;

        public async void ChoosePicture(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            foreach (string type in authorizedPictureType)
            {
                string typeFile = "." + type;
                openPicker.FileTypeFilter.Add(typeFile);
            }
            StorageFile file = await openPicker.PickSingleFileAsync();

            if (file != null)
            {
                SelectedPicture= file;
                SelectedPictureName= file.Name;
                IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read);

                await Image.SetSourceAsync(stream);

            }

        }

        public async void ChooseAudio(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.MusicLibrary;
            foreach (string type in authorizedAudioType)
            {
                string typeFile = "." + type;
                openPicker.FileTypeFilter.Add(typeFile);
            }
            StorageFile file = await openPicker.PickSingleFileAsync();

            if (file != null)
            {
                SelectedAudio = file;
                IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read);
                MediaPlayer.SetSource(stream, file.ContentType);
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
                    OnPropertyChanged("SelectedPicture");

                }
            }
        }

        public StorageFile SelectedAudio
        {
            get => selectedAudio;
            set
            {
                if (value != selectedAudio)
                {
                    selectedAudio = value;
                    OnPropertyChanged("SelectedAudio");

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
                    OnPropertyChanged("SelectedPictureName");

                }
            }
        }

        private async void SaveElementAsync()
        {

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