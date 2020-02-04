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
using System.Diagnostics;
using Windows.Media.Playback;
using humanlab.Views;
using humanlab.Services;

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
        private string[] authorizedPictureType = { "jpeg", "png", "jpg", "gif" };
        private string[] authorizedAudioType = { "mp4", "mp3", "wav" };
        //play/stop button
        private MediaPlayer playingSound;
        private string buttonIcon;

        //*** Form Validation Controls ***//
        private bool isNotAvailableName;
        public DelegateCommand SaveElementCommand { get; set; }
        public DelegateCommand PlayCommand { get; set; }
        public string DefaultColor { get; set; }

        public ElementFormViewModel()
        {
            this.elementName = "";
            this.elementSpeach = "";
            this.selectedCategory = null;
            this.isToggleChecked = false;
            this.isNotAvailableName = false;
            ElementsBorderBrush = InitializeColorDictionnary();
            SaveElementCommand = new DelegateCommand(SaveElementAsync);
            PlayCommand = new DelegateCommand(Play, CanPlay);
            DefaultColor = ColorTheme;
            this.repository = new Repository();

            // Retrieve data from db
            repository.CreateCategories();
            GetCategoriesAsync();
            GetElementsAsync();

            playingSound = null;
            ButtonIcon = "Play";
        }

        bool CanPlay()
        {
            if (IsToggleChecked)
            {
                return SelectedAudio != null;
            }
            else
            {
                return !ElementSpeach.Equals("");
            }

        }

        private async void Play()
        {
            if (IsToggleChecked)
            {
                if (playingSound == null)
                {
                    Debug.WriteLine("playing start");
                    playingSound = new MediaPlayer();
                    playingSound.Source = AudioSource;
                    playingSound.Play();
                    ButtonIcon = "Pause";
                    Debug.WriteLine("playing end");
                }
                else
                {
                    Debug.WriteLine("NOT playing start");
                    playingSound.Pause();
                    playingSound.Source = null;
                    playingSound = null;
                    ButtonIcon = "Play";
                    Debug.WriteLine("NOT playing end");
                }
            }
            else
            {
                Debug.WriteLine("vocal start");
                MediaElement mediaElement = new MediaElement();
                var synth = new Windows.Media.SpeechSynthesis.SpeechSynthesizer();
                Windows.Media.SpeechSynthesis.SpeechSynthesisStream stream = await synth.SynthesizeTextToStreamAsync(ElementSpeach);
                mediaElement.SetSource(stream, stream.ContentType);
                mediaElement.Play();
                Debug.WriteLine("vocal end");
            }

        }

        public string ButtonIcon
        {
            get => buttonIcon;
            set => SetProperty(ref buttonIcon, value, "ButtonIcon");
        }

        private async void GetCategoriesAsync(){
        var categorieNames = await repository.GetCategoriesNamesAsync();
            Categories = categorieNames.OrderBy(c => c).ToList();
    }
        private async void GetElementsAsync() => Elements = await repository.GetElementsNamesAsync();

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


        public List<string> Categories
        {
            get => categories;
            set => SetProperty(ref categories, value, "Categories");
        }

        public List<string> Elements
        {
            get => elements;
            set => SetProperty(ref elements, value, "Elements");
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
                        PlayCommand.RaiseCanExecuteChanged();
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
                    if (uiName.Equals("SelectedAudio")) { return "LightGray"; }
                    else return "Gray";

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
            set => SetProperty(ref isNotAvailableName, value, "IsNotAvailableName");
        }
        public BitmapImage Image
        {
            get => image;
            set => SetProperty(ref image, value, "ImageSource");
        }

        public MediaSource AudioSource
        {
            get => audioSource;
            set => SetProperty(ref audioSource, value, "AudioSource");
        }

        public StorageFile SelectedPicture
        {
            get => selectedPicture;
            set => SetProperty(ref selectedPicture, value, "SelectedPicture");
        }

        public StorageFile SelectedAudio
        {
            get => selectedAudio;
            set => SetProperty(ref selectedAudio, value, "SelectedAudio");
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
        public bool IsToggleChecked
        {
            get => isToggleChecked;
            set
            {
                if (value != isToggleChecked)
                {
                    isToggleChecked = value;

                    PlayCommand.RaiseCanExecuteChanged();
                    OnPropertyChanged("IsToggleChecked");
                    OnPropertyChanged("IsToggleNotChecked");

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
                PlayCommand.RaiseCanExecuteChanged();
            }

            else {
                Dictionary_SetValue("SelectedAudio", null); ; 
                OnPropertyChanged("SelectedAudioBorder"); }

        }

        public async void LoadMediaPlayer()
        {
            IRandomAccessStream stream = await SelectedAudio.OpenAsync(FileAccessMode.Read);
            AudioSource = MediaSource.CreateFromStream(stream, SelectedAudio.ContentType);
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
            try
            {
                await file.CopyAsync(assets);
            }
            catch { Debug.WriteLine("File already saved in Assets folder"); }
            
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

            Element model = new Element
            {
                ElementName = ElementName,
                SpeachText = speachText,
                Image = SelectedPicture.Name,
                Audio = audioFileName,
            };

            return model;
        }

        private async void SaveElementAsync()
        {

            if (Check_FormValidation())
            {
                //Saving element in db
                Element model = GenerateModel();
                repository.SaveElementAsync(model, SelectedCategory);

                // Success popup
                DisplayMessagesService.showSuccessMessage("élément", ElementName, ReloadElementFormView);

            }
            else { // Je met une alert
                DisplayMessagesService.showPersonalizedMessage("Des champs obligatoires à l'enregistrement d'un élément sont invalides ou manquants. Veuillez compléter les champs surlignés en rouge.");
            }
 
        }

        public void ReloadElementFormView()
        {
            // Here's the navigationView 
            var navigationView = GetNavigationView();
            var child = navigationView.Content as Frame;
            child.SourcePageType = typeof(BlankPage1);
            child.SourcePageType = typeof(ElementFormView);
            Debug.WriteLine("child" + child);


        }

    }
}
 
