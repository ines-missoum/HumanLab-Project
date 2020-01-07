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
using humanlab.DAL;
using humanlab.Models;
using Windows.Media.Core;

namespace humanlab.ViewModels
{


    class ElementFormViewModel : BaseViewModel
    {
        private string elementName;
        private string elementText;
        private bool isEmptyNameField;
        private string nameFieldBorderBrush;
        private string selectionBorderBrush;
        private string imageBorderBrush;
        private string audioBorderBrush;
        List<string> categories;
        List<string> elements;
        private string selectedCategorie;
        private bool isToggleChecked;
        private StorageFile selectedPicture;
        private StorageFile selectedAudio;
        private MediaSource audioSource;
        private BitmapImage image = new BitmapImage();
        private string[] authorizedPictureType = { "jpeg", "png", "jpg" };
        private string[] authorizedAudioType = { "mp4", "mp3" };

        public ElementFormViewModel()
        {

            this.elementName = "";
            this.elementText = "";
            this.nameFieldBorderBrush = "Gray";
            this.selectionBorderBrush = "Gray";
            this.imageBorderBrush = "Gray";
            this.audioBorderBrush = "Gray";
            this.selectedCategorie = null;
            this.isToggleChecked = false;
            Repository.CreateCategories();
            GetCategoriesAsync();
            GetElementsAsync();

        }

        private async void GetCategoriesAsync()
        {
            Categories = await Repository.GetCategoriesNamesAsync();

        }

        private async void GetElementsAsync()
        {
            Elements = await Repository.GetElementsNamesAsync();
        }

        public List<string> Categories

        {
            get => categories;
            set
            {
                if (value != categories)
                {
                    categories = value;
                    OnPropertyChanged("Categories");

                }
            }
        }


        public List<string> Elements

        {
            get => elements;
            set
            {
                if (value != elements)
                {
                    elements = value;
                    OnPropertyChanged("Elements");

                }
            }
        }


        public string ElementName
        {
            get => elementName;
            set
            {
                if (value != elementName)
                {
                    elementName = value;
                    if (value.Equals(""))
                    {
                        IsEmptyNameField = true;
                        NameFieldBorderBrush = "Red";
                        
                    }
                    else
                    {
                        IsEmptyNameField = false;
                        NameFieldBorderBrush = "Gray";
                    }
                    OnPropertyChanged("ElementName");
                    OnPropertyChanged("NameFieldBorderBrush");

                }
            }
        }

        public bool IsEmptyNameField
        {
            get => isEmptyNameField;
            set
            {
                if (value != isEmptyNameField)
                {
                    isEmptyNameField = value;
                    OnPropertyChanged("IsEmptyNameField");
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

        public MediaSource AudioSource
        {
            get => audioSource;
            set
            {
                if (value != audioSource)
                {
                    audioSource = value;
                    OnPropertyChanged("AudioSource");

                }
            }
        }

        public string NameFieldBorderBrush
        {
            get => nameFieldBorderBrush;
            set
            {
                if (value != nameFieldBorderBrush)
                {
                    nameFieldBorderBrush = value;
                    OnPropertyChanged("NameFieldBorderBrush");
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



        public string SelectionBorderBrush
        {
            get => selectionBorderBrush;
            set
            {
                if (value != selectionBorderBrush)
                {
                    selectionBorderBrush = value;
                    OnPropertyChanged("SelectionBorderBrush");

                }
            }
        }


        public string ImageBorderBrush
        {
            get => imageBorderBrush;
            set
            {
                if (value != imageBorderBrush)
                {
                    imageBorderBrush = value;
                    OnPropertyChanged("ImageBorderBrush");

                }
            }
        }

        public string AudioBorderBrush
        {
            get => audioBorderBrush;
            set
            {
                if (value != audioBorderBrush)
                {
                    audioBorderBrush = value;
                    OnPropertyChanged("ImageBorderBrush");

                }
            }
        }

        public void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch != null)
            {
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
                    
                    OnPropertyChanged("IsToggleChecked");
                    OnPropertyChanged("IsToggleNotChecked");

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
                LoadMediaPlayer();    
            }

        }

        public async void LoadMediaPlayer()
        {
                IRandomAccessStream stream = await SelectedAudio.OpenAsync(FileAccessMode.Read);
                AudioSource = MediaSource.CreateFromStream(stream, SelectedAudio.ContentType);  
        }

        public string SelectedCategorie
        {
            get => selectedCategorie;
            set
            {
                if (value != selectedCategorie)
                {
                    selectedCategorie = value;
                    OnPropertyChanged("SelectedPicture");

                }
            }
        }

        public void ComboBox_ChangeBorder()
        {
            if (ComboBox_SelectionEmpty())
            {
                SelectionBorderBrush = "Red";
            }
        }

        public void Image_ChangeBorder()
        {
            if (Image_FileEmpty())
            {
                ImageBorderBrush = "Red";
            }
        }

        public void Audio_ChangeBorder()
        {
            if (Audio_FileEmpty())
            {
                AudioBorderBrush = "Red";
            }
        }

        public bool ComboBox_SelectionEmpty()
        {
             return selectedCategorie != null ? false : true;
        }

        public bool Image_FileEmpty()
        {
            return selectedPicture != null ? false : true;
        }

        public bool Audio_FileEmpty()
        {
            return selectedAudio != null ? false : true;
        }

        public void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            string selected = comboBox.SelectedItem.ToString();
            if (selected != null && SelectedCategorie != selected)
            {
                SelectedCategorie = selected;
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