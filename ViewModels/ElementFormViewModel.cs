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
using Windows.UI.Popups;
using Windows.ApplicationModel;

namespace humanlab.ViewModels
{


    class ElementFormViewModel : BaseViewModel
    {

        //*** Main Attributes ***//
        private string elementName;
        private string elementSpeach;
        private string selectedCategory;
        private StorageFile selectedPicture;
        private StorageFile selectedAudio;
        private bool isToggleChecked;


        //*** Data ***//
        List<string> categories;
        List<string> elements;
        Repository repository;

        //*** Media ***//
        private MediaSource audioSource;
        private BitmapImage image = new BitmapImage();
        private string[] authorizedPictureType = { "jpeg", "png", "jpg" };
        private string[] authorizedAudioType = { "mp4", "mp3" };

        //*** Form Validation Controls ***//
        Dictionary<string, string> elementsBorderBrush;
        private bool isNotAvailableName;


        public DelegateCommand SaveElementCommand { get; set; }
        public ElementFormViewModel()
        {
            this.elementName = "";
            this.elementSpeach = "";
            this.selectedCategory = null;
            this.isToggleChecked = false;
            this.isNotAvailableName = false;
            ElementsBorderBrush = InitializeColorDictionnary();
            SaveElementCommand = new DelegateCommand(SaveElementAsync, CanSaveElement);
            this.repository = new Repository();

            repository.CreateCategories();
            GetCategoriesAsync();
            GetElementsAsync();

        }

        private bool CanSaveElement() => true;


        public Dictionary<string, string> InitializeColorDictionnary()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            string[] xamlElements = { "ElementName", "ElementSpeach", "SelectedCategory", "SelectedPicture", "SelectedAudio" };
            foreach (string element in xamlElements)
            {
                result.Add(element, "default");
            }
            return result;
        }

        private async void GetCategoriesAsync() => Categories = await repository.GetCategoriesNamesAsync();
        private async void GetElementsAsync() => Elements = await repository.GetElementsNamesAsync();


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
                        Dictionary_SetValue("ElementName", null);
                    }
                    else
                    {
                        IsNotAvailableName = CheckAvailability(value);
                        Dictionary_SetValue("ElementName", value);
                    }
                    OnPropertyChanged("ElementName");
                    OnPropertyChanged("ElementNameBorder");
                    OnPropertyChanged("IsNotAvailableName");

                }
            }
        }

        public string ElementSpeach
        {
            get => elementSpeach;
            set
            {
                if (value != elementSpeach)
                {
                    elementSpeach = value;

                    if (value.Equals(""))
                    {
                        Dictionary_SetValue("ElementSpeach", null);
                    }
                    else
                    {
                        Dictionary_SetValue("ElementSpeach", value);
                    }

                    OnPropertyChanged("ElementSpeach");
                    OnPropertyChanged("ElementsBorderBrush");
                    OnPropertyChanged("ElementSpeachBorder");
                }
            }
        }

        public string GetBorderColor(string uiName)
        {
            if (ElementsBorderBrush != null) {
                if (ElementsBorderBrush[uiName] != null)
                {
                    OnPropertyChanged(uiName);
                    return "Gray";

                }
                else {
                    OnPropertyChanged(uiName);
                    return "Red"; }

            }
            else return "Gray";
        }

        public string ElementNameBorder
        {
            get => GetBorderColor("ElementName");
        }

        public string ElementSpeachBorder
        {
            get => GetBorderColor("ElementSpeach");
        }

        public string SelectedCategoryBorder
        {
            get => GetBorderColor("SelectedCategory");
        }

        public string SelectedPictureBorder
        {
            get => GetBorderColor("SelectedPicture");
        }

        public string SelectedAudioBorder
        {
            get => GetBorderColor("SelectedAudio");
        }

        public Dictionary<string, string> ElementsBorderBrush { get; set; }

        public void Dictionary_SetValue(string key, string value) {
            ElementsBorderBrush[key] = value;
            OnPropertyChanged("ElementsBorderBrush"); }

        public void Dictionary_SetValuesToNull(List<string> keys)
        {

            if (keys.Capacity != 0) {
                System.Diagnostics.Debug.WriteLine("capacity not nul");
                foreach (string key in keys) {
                    ElementsBorderBrush[key] = null;
                    OnPropertyChanged(key + "Border");
                }
                OnPropertyChanged("ElementsBorderBrush");

            }
        }

        public bool CheckAvailability(string name)
        {
            return Elements.Contains(name);

        }

        public bool IsNotAvailableName
        {
            get => isNotAvailableName;
            set
            {
                if (value != isNotAvailableName)
                {
                    isNotAvailableName = value;
                    OnPropertyChanged("IsNotAvailableName");
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
                SelectedPicture = file;
                IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read);

                await Image.SetSourceAsync(stream);
                Dictionary_SetValue("SelectedPicture", file.Name);
                OnPropertyChanged("SelectedPictureBorder");

            }
            else {
                Dictionary_SetValue("SelectedPicture", null);
                OnPropertyChanged("SelectedPictureBorder");
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
                Dictionary_SetValue("SelectedAudio", file.Name);
                OnPropertyChanged("SelectedAudioBorder");
            }

            else {
                System.Diagnostics.Debug.WriteLine("Je suis passé dans le else file null ");
                Dictionary_SetValue("SelectedAudio", null); ; OnPropertyChanged("SelectedAudioBorder"); }

        }

        public async void LoadMediaPlayer()
        {
            IRandomAccessStream stream = await SelectedAudio.OpenAsync(FileAccessMode.Read);
            AudioSource = MediaSource.CreateFromStream(stream, SelectedAudio.ContentType);
        }

        public string SelectedCategory
        {
            get => selectedCategory;
            set
            {
                if (value != selectedCategory)
                {
                    selectedCategory = value;
                    Dictionary_SetValue("SelectedCategory", value);
                    OnPropertyChanged("SelectedCategory");
                    OnPropertyChanged("SelectedCategoryBorder");


                }
            }
        }

        public bool ComboBox_SelectionEmpty()
        {
            return selectedCategory != null ? false : true;
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
            if (selected != null && SelectedCategory != selected)
            {
                SelectedCategory = selected;
            }
        }

        public StorageFile SelectedPicture
        {
            get => selectedPicture;
            set
            {
                if (value != selectedPicture)
                {
                    selectedPicture = value;
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

        public bool Check_FormValidation()
        {
            string notMandatoryElement = "";
            bool validation = true;
            List<string> valuesToChange = new List<string>();

            if (IsToggleChecked) { notMandatoryElement = "ElementSpeach"; }
            else { notMandatoryElement = "SelectedAudio"; }

            foreach (KeyValuePair<string, string> pair in ElementsBorderBrush)
            {

                if (!pair.Key.Equals(notMandatoryElement))
                {
                    if (pair.Value == null)
                    {
                        validation = false;
                        OnPropertyChanged("ElementsBorderBrush");
                    }
                    else if (pair.Value.Equals("default")) {
                        valuesToChange.Add(pair.Key);
                        validation = false; }
                    OnPropertyChanged("ElementsBorderBrush");
                }
            }

            Dictionary_SetValuesToNull(valuesToChange);
            return validation;

        }

        public async void SaveFileInFolder(StorageFile file)
        {
            StorageFolder assets = await Package.Current.InstalledLocation.GetFolderAsync("Assets");
            await file.CopyAsync(assets);
        }

        public Element GenerateModel()
        {
            string audioFileName = "";
            string speachText = "";
            
            if (IsToggleChecked) {
                audioFileName = SelectedAudio.Name;
                SaveFileInFolder(SelectedAudio);
            }
            else {
                speachText = ElementSpeach;
                SaveFileInFolder(SelectedPicture);
            }
            Category category = repository.GetCategoryByName(SelectedCategory);

            Element model = new Element
            {
                ElementName = ElementName,
                SpeachText = speachText,
                Image = SelectedPicture.Name,
                Audio = audioFileName,
                Category = category

            };

            return model;
        }




        private async void SaveElementAsync()
        {
            MessageDialog messageDialog;
            if (Check_FormValidation())
            {
                //J'enregistre l'élément
                Category category = repository.GetCategoryByName(SelectedCategory);
                System.Diagnostics.Debug.WriteLine(" *****Categorie retournée plus bas " + category + category.CategoryName + category.CategoryId);
                Element model = GenerateModel();
                repository.SaveElementAsync(model, category);
                messageDialog = new MessageDialog("Votre element " + ElementName +" a été sauvegardé avec succès.");

                System.Diagnostics.Debug.WriteLine("check true");

                // Je met un succes
            }
            else { // Je met une alert
                System.Diagnostics.Debug.WriteLine("check false");

                messageDialog = new MessageDialog("Des champs obligatoires à l'enregistrement d'un élément sont invalides ou manquants. Veuillez compléter les champs surlignés en rouge.");
            }
            await messageDialog.ShowAsync();
        }
    }
}
 
