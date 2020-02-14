using System.Collections.Generic;
using System.Linq;
using Windows.Devices.Input.Preview;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Controls;
using Windows.UI.Core;
using Windows.UI.Xaml.Input;
using Prism.Commands;
using humanlab.Services;
using Windows.UI.Xaml.Media.Imaging;
using humanlab.Helpers.Models;
using System.Diagnostics;
using Windows.Media.Playback;
using Windows.Media.Core;
using System;
using Windows.Storage;
using humanlab.DAL;
using humanlab.Models;
using System.Threading.Tasks;
using humanlab.Views;
using System.Collections.ObjectModel;
using System.Threading;

namespace humanlab.ViewModels
{
    public class ActivityLoadingViewModel : BaseViewModel
    {

        /*** ATTRIBUTS ***/
        private List<ElementOfActivity> elements;
        private double maxFocusTime;
        private Transform transform;
        public DelegateCommand<object> ClickImage { get; set; }
        public TobiiSetUpService TobiiSetUpService { get; set; }

        private MediaPlayer playingSound;
        private MediaElement playingSpeech;

        //repositoryies => responsible for retrieving data in db
        private GridRepository gridRepository;
        private ActivityRepository activityRepository;

        public List<ActivityUpdated> allActivities;
        public ObservableCollection<ActivityUpdated> allActivitiesObserver;
        private bool isActivityLoading;

        private bool openActivityAlreadyCalled;
        private bool isEditModeActivated;
        private string editButton;
        private string selectionMode;

        ScrollViewer scrollViewer;
        public DelegateCommand CloseActivityDelegate { get; set; }
        private DelegateCommand<object> DeleteActivityDelegate { get; set; }
        private DelegateCommand<object> UpdateActivityDelegate { get; set; }

        private List<(int GridOrder, int GridId)> listGridIds;
        private (int GridOrder, int GridId) currentGrid;
        public DelegateCommand NextGrid { get; set; }
        public DelegateCommand PreviousGrid { get; set; }

        private (BitmapImage source, ElementOfActivity element) activatedElement;
        public DelegateCommand ChangeEditMode { get; set; }
        private Random random;
        public string Mode { get; set; }
        public bool IsShowingNextArrow { get; set; }
        public bool IsShowingPreviousArrow { get; set; }
        public bool IsShowingTimeLeft { get; set; }
        //for automatic mode
        private DispatcherTimer timerForAtomaticGrid;
        private double secondsLeftBeforeNextGrid;
        private double timeToWaitBeforeChangingGrid;

        /*** CONSTRUCTOR ***/

        public ActivityLoadingViewModel()
        {
            gridRepository = new GridRepository();
            activityRepository = new ActivityRepository();
            DeleteActivityDelegate = new DelegateCommand<object>(DeleteActivity);
            UpdateActivityDelegate = new DelegateCommand<object>(UpdateActivity);
            GetAllActivitiesAsync();

            Elements = new List<ElementOfActivity>();
            ClickImage = new DelegateCommand<object>(ClickOnImage);
            listGridIds = new List<(int GridOrder, int GridId)>();

            NextGrid = new DelegateCommand(ClickOnNext, CanClickOnNext);
            PreviousGrid = new DelegateCommand(ClickOnPrevious, CanClickOnPrevious);
            ChangeEditMode = new DelegateCommand(SetEditMode);
            SelectionMode = "Single";
            ScrollViewer = new ScrollViewer();

            TobiiSetUpService = new TobiiSetUpService(this.GazeEntered, this.GazeMoved, this.GazeExited, this.TimerGaze_Tick);

            MaxFocusTime = 5; //en sec
            IsActivityLoading = false;

            openActivityAlreadyCalled = false;
            IsEditModeActivated = false;
            EditButton = "Modifier";

            CloseActivityDelegate = new DelegateCommand(CloseActivity);

            playingSound = null;
            playingSpeech = null;
            activatedElement = (null, null);

            random = new Random();

            InitValuesDependingOnsettings();

        }
        private void InitValuesDependingOnsettings()
        {

            //set values depending of settings
            if (ParametersService.IsAutomatic())
            {
                Mode = "MODE : Automatique " + " " + ParametersService.GetMode() + " ";
                timeToWaitBeforeChangingGrid = ParametersService.GetGridTime();
                InitTimer();
            }
            else
                Mode = "MODE : Manuel " + " " + ParametersService.GetMode();

            IsShowingNextArrow = !ParametersService.IsAutomatic();
            IsShowingPreviousArrow = !ParametersService.IsAutomatic() && !ParametersService.GetMode().ToLower().Equals("aléatoire");
            IsShowingTimeLeft = ParametersService.IsAutomatic() ;
            Debug.WriteLine(IsShowingNextArrow);
        }
        private void InitTimer()
        {
            //timerForAtomaticGrid_Tick will be called each 1 sec
            timerForAtomaticGrid = new DispatcherTimer();
            timerForAtomaticGrid.Tick += new EventHandler<object>(timerForAtomaticGrid_Tick);
            timerForAtomaticGrid.Interval = new TimeSpan(0, 0, 0, 1); //1sec
            SecondsLeftBeforeNextGrid = timeToWaitBeforeChangingGrid;
        }
        private void timerForAtomaticGrid_Tick(object sender, object e)
        {
            SecondsLeftBeforeNextGrid = SecondsLeftBeforeNextGrid - 1;

            if (SecondsLeftBeforeNextGrid < 0)
            {
                ClickOnNext();
                SecondsLeftBeforeNextGrid = timeToWaitBeforeChangingGrid;
            }

        }
        public void DeleteActivity(object activityObject)
        {
            try
            {
                Activity activity = activityObject as Activity;
                activityRepository.DeleteActivity(activity);
                ActivityUpdated actUpdated = AllActivities.Find(a => a.Activity == activity);
                AllActivitiesObserver.Remove(actUpdated);
            }
            catch { Debug.WriteLine("Error while deleting activity"); }
        }

        public void UpdateActivity(object activityObject)
        {
            // Redirect toward the modification form
            NavigationView navigation = GetNavigationView();
            Frame child = navigation.Content as Frame;
            NavigationViewModel navigationViewModel = child.DataContext as NavigationViewModel;
            Object parameter = activityObject as Object;
            // Passing parameter through the navigation viewmodel
            navigationViewModel.ParameterToPass = parameter;
            // Indicates which form we should open
            child.SourcePageType = typeof(ActivityFormView);
        }

        private async void GetAllActivitiesAsync()
        {
            var activities = await activityRepository.GetActivitiesAsync();
            List<ActivityUpdated> activitiesUpdated = new List<ActivityUpdated>();
            activities.ForEach(activity =>
            {
                var a = new ActivityUpdated(activity, DeleteActivityDelegate, UpdateActivityDelegate);
                activitiesUpdated.Add(a);
            });
            AllActivities = activitiesUpdated.OrderByDescending(e => e.Activity.ActivityName.Length).ToList();
        }

        private async void GetElementsOfGrid(int gridId)
        {
            List<ElementOfActivity> dbElements = await gridRepository.GetAllGridElements(gridId);
            dbElements.ForEach(e =>
            {
                e.MaxFocusTime = MaxFocusTime;
                e.ClickImage = ClickImage;
            });

            Elements = dbElements;

        }

        /***GETTERS & SETTERS***/

        public double SecondsLeftBeforeNextGrid
        {
            get => secondsLeftBeforeNextGrid;
            set => SetProperty(ref secondsLeftBeforeNextGrid, value, "SecondsLeftBeforeNextGrid");
        }
        public ScrollViewer ScrollViewer
        {
            get => scrollViewer;
            set => SetProperty(ref scrollViewer, value, "ScrollViewer");
        }
        public List<ActivityUpdated> AllActivities
        {
            get => allActivities;
            set
            {
                if (value != allActivities)
                {
                    allActivities = value;
                    AllActivitiesObserver = new ObservableCollection<ActivityUpdated>(value);
                }
            }
        }
        public ObservableCollection<ActivityUpdated> AllActivitiesObserver
        {
            get => allActivitiesObserver;
            set => SetProperty(ref allActivitiesObserver, value, "AllActivitiesObserver");

        }
        public List<ElementOfActivity> Elements
        {
            get => elements;
            set => SetProperty(ref elements, value, "Elements");
        }

        public double MaxFocusTime
        {
            get => maxFocusTime;
            set => SetProperty(ref maxFocusTime, value, "MaxFocusTime");
        }

        public Transform Transform
        {
            get => transform;
            set => SetProperty(ref transform, value, "Transform");
        }

        public bool IsActivityLoading
        {
            get => isActivityLoading;
            set => SetProperty(ref isActivityLoading, value, "IsActivityLoading");
        }

        public string SelectionMode
        {
            get => selectionMode;
            set => SetProperty(ref selectionMode, value, "SelectionMode");
        }

        public bool OpenActivityAlreadyCalled
        {
            get => openActivityAlreadyCalled;
            set => SetProperty(ref openActivityAlreadyCalled, value, "OpenActivityAlreadyCalled");
        }

        public bool IsEditModeActivated
        {
            get => isEditModeActivated;
            set => SetProperty(ref isEditModeActivated, value, "IsEditModeActivated");
        }

        public string EditButton
        {
            get => editButton;
            set => SetProperty(ref editButton, value, "EditButton");
        }

        /*** METHODS ***/

        /// <summary>
        /// GazeEntered handler.
        /// </summary>
        /// <param name="sender">Source of the gaze entered event</param>
        /// <param name="e">Event args for the gaze entered event</param>
        private void GazeEntered(GazeInputSourcePreview sender, GazeEnteredPreviewEventArgs args)
        {
            args.Handled = true;

        }

        /// <summary>
        /// GazeExited handler.
        /// Call DisplayRequest.RequestRelease to conclude the 
        /// RequestActive called in GazeEntered.
        /// </summary>
        /// <param name="sender">Source of the gaze exited event</param>
        /// <param name="e">Event args for the gaze exited event</param>
        private void GazeExited(GazeInputSourcePreview sender, GazeExitedPreviewEventArgs args)
        {
            args.Handled = true;
        }

        /// <summary>
        /// GazeMoved handler translates the ellipse on the canvas to reflect gaze point.
        /// </summary>
        /// <param name="sender">Source of the gaze moved event</param>
        /// <param name="e">Event args for the gaze moved event</param>
        private void GazeMoved(GazeInputSourcePreview sender, GazeMovedPreviewEventArgs args)
        {
            if (TobiiSetUpService.IsActiveDevice())
            {
                // Update the position of the ellipse corresponding to gaze point.
                if (args.CurrentPoint.EyeGazePosition != null)
                {
                    // we retrieve the position of the gaze
                    double gazePointX = args.CurrentPoint.EyeGazePosition.Value.X;
                    double gazePointY = args.CurrentPoint.EyeGazePosition.Value.Y;

                    // xaml data => to change if the xaml changes
                    int eyesEllipseSize = 20;
                    int progressBarSize = 24; //(margin included)

                    double ellipseLeft = gazePointX - (eyesEllipseSize / 2.0f);
                    double ellipseTop = gazePointY - (eyesEllipseSize / 2.0f) - progressBarSize;

                    // Translate transform for moving gaze ellipse.
                    TranslateTransform translateEllipse = new TranslateTransform
                    {
                        X = ellipseLeft,
                        Y = ellipseTop
                    };

                    Transform = translateEllipse;

                    // The gaze point screen location.
                    Point gazePoint = new Point(gazePointX, gazePointY);

                    // Basic hit test to determine if gaze point is on image.
                    Image img = TobiiSetUpService.CurrentFocusImage as Image;
                    bool hitImage = TobiiSetUpService.DoesElementContainPoint(gazePoint, null);
                    if (img != null & !hitImage)
                    {
                        ElementOfActivity current = Elements.Where(el => el.Element.ElementId.Equals(img.Tag)).First();
                        current.FocusTime = 0;
                    }

                    // Mark the event handled.
                    args.Handled = true;
                }
            }

        }



        /// <summary>
        /// Tick handler for gaze focus timer.
        /// </summary>
        /// <param name="sender">Source of the gaze entered event</param>
        /// <param name="e">Event args for the gaze entered event</param>
        private void TimerGaze_Tick(object sender, object e)
        {
            try
            {
                Image img = TobiiSetUpService.CurrentFocusImage as Image;
                ElementOfActivity current = Elements.Where(el => el.Element.ElementId.Equals(img.Tag)).First();
                BitmapImage source = img.Source as BitmapImage;

                // Increment progress bar.
                current.FocusTime += 0.025; //because the method is called each 25ms

                // If progress bar reaches maximum value, reset and relocate.
                if (current.FocusTime >= MaxFocusTime)//nb de sec
                {
                    // we animate the element and reset all needed values
                    TobiiSetUpService.StopTimer();
                    current.FocusTime = 0;
                    Play(source, current);
                }
            }
            catch { }
        }

        private async void Play(BitmapImage source, ElementOfActivity current)
        {
            string path = "ms-appx:///Assets/";

            //if an element is already playing we stop it
            if (this.activatedElement != (null, null))
                Stop(this.activatedElement.source, this.activatedElement.element);

            source.Play();
            current.IsNotAnimated = false;
            if (current.Element.Audio != "")
            {
                playingSound = new MediaPlayer();
                playingSound.Source = MediaSource.CreateFromUri(new Uri(path + current.Element.Audio));
                playingSound.Play();
            }
            else
            {
                playingSpeech = new MediaElement();
                var synt = new Windows.Media.SpeechSynthesis.SpeechSynthesizer();
                Windows.Media.SpeechSynthesis.SpeechSynthesisStream stream = await synt.SynthesizeTextToStreamAsync(current.Element.SpeachText);
                playingSpeech.SetSource(stream, stream.ContentType);
                playingSpeech.Play();
            }

            //we save the new playing element
            this.activatedElement = (source, current);

        }

        private void Stop(BitmapImage source, ElementOfActivity current)
        {
            source.Stop();
            current.IsNotAnimated = true;
            if (current.Element.Audio != "" && playingSound != null)
            {
                playingSound.Pause();
                playingSound.Source = null;
                playingSound = null;
            }
            else if (playingSpeech != null)
            {
                playingSpeech.Stop();
                playingSpeech.Source = null;
                playingSpeech = null;
            }
        }

        //MOUSE
        private void ClickOnImage(object args)
        {
            Image img = args as Image;
            ElementOfActivity current = Elements.Where(el => el.Element.ElementId.Equals(img.Tag)).First();
            current.FocusTime = 0;
            BitmapImage source = img.Source as BitmapImage;

            if (current.IsNotAnimated)
            {
                Play(source, current);
            }
            else
            {
                //else we stop the only running element
                this.activatedElement = (null, null);
                Stop(source, current);
            }
        }

        //NAVIGATION GRID

        /// <summary>
        /// Checks if the next button should be allowed
        /// </summary>
        /// <returns>Returns true if the mode is loop or if this is note the last grid, else false</returns>
        private bool CanClickOnNext()
        {
            return ParametersService.GetMode().ToLower().Equals("boucle") || ParametersService.GetMode().ToLower().Equals("aléatoire")
                || ParametersService.GetMode().ToLower().Equals("ordonné") && currentGrid.GridOrder < listGridIds.Count;
        }
        /// <summary>
        /// Checks if the previous button should be allowed
        /// </summary>
        /// <returns>Returns true if the mode is loop or if this is note the first grid, else false</returns>
        private bool CanClickOnPrevious()
        {
            return ParametersService.GetMode().ToLower().Equals("boucle")
                || ParametersService.GetMode().ToLower().Equals("ordonné") && currentGrid.GridOrder > 1;
        }

        private void ClickOnNext()
        {
            ChangeCurrentGrid(getGridOrderToDisplay(true));
        }

        private void ClickOnPrevious()
        {
            ChangeCurrentGrid(getGridOrderToDisplay(false));
        }

        private void ChangeCurrentGrid(int newGridOrder)
        {
            currentGrid = listGridIds.Find(tuple => tuple.GridOrder == newGridOrder);
            GetElementsOfGrid(currentGrid.GridId);
            NextGrid.RaiseCanExecuteChanged();
            PreviousGrid.RaiseCanExecuteChanged();
        }

        private int getGridOrderToDisplay(bool nextGridWanted)
        {
            string mode = ParametersService.GetMode().ToLower();

            int gridOrder;
            if (mode.Equals("aléatoire"))
            {
                gridOrder = random.Next(1, listGridIds.Count + 1);
            }
            else
            {
                if (nextGridWanted)
                    gridOrder = currentGrid.GridOrder + 1;
                else
                    gridOrder = currentGrid.GridOrder - 1;
            }


            //we check if the limits are reached
            switch (mode)
            {
                case "boucle":
                    if (gridOrder > listGridIds.Count)
                        gridOrder = 1;
                    else if (gridOrder < 1)
                        gridOrder = listGridIds.Count;
                    break;
                case "aléatoire":
                    while (gridOrder == currentGrid.GridOrder)
                        gridOrder = random.Next(1, listGridIds.Count + 1);
                    break;
            }
            return gridOrder;
        }

        public void GridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Check if the method is not calling itself
            if (!OpenActivityAlreadyCalled)
            {
                GridView gv = sender as GridView;

                //Retrieve the activity we want to Play
                ActivityUpdated selectedItem = gv.SelectedItem as ActivityUpdated;
                Activity selected = selectedItem.Activity;
                // Signal that we enter into the method once
                OpenActivityAlreadyCalled = true;
                // Reset the activity selection
                gv.SelectedItem = null;

                if (!IsEditModeActivated)
                {
                    OpenActivity(selected);
                }
                else
                {
                    // Redirect toward the modification form
                    NavigationView navigation = GetNavigationView();
                    Frame child = navigation.Content as Frame;
                    NavigationViewModel navigationViewModel = child.DataContext as NavigationViewModel;
                    Object parameter = selected as Object;
                    // Passing parameter through the navigation viewmodel
                    navigationViewModel.ParameterToPass = parameter;
                    // Indicates which form we should open
                    child.SourcePageType = typeof(ActivityFormView);
                }
            }

        }

        public void OpenActivity(Activity activity)
        {
            IsActivityLoading = true;
            MaxFocusTime = activity.FixingTime;
            GetAllGridsOfLoadingActivity(activity.ActivityId);
            TobiiSetUpService.StartGazeDeviceWatcher();

            if (ParametersService.IsAutomatic())
                timerForAtomaticGrid.Start();

            NavigationView navView = GetNavigationView();
            navView.IsPaneVisible = false;
            navView.IsPaneOpen = false;
            navView.IsPaneToggleButtonVisible = false;

            NextGrid.RaiseCanExecuteChanged();
            PreviousGrid.RaiseCanExecuteChanged();

        }

        private async void GetAllGridsOfLoadingActivity(int idActivity)
        {
            List<ActivityGrids> grids = await activityRepository.GetGridsOfActivity(idActivity);
            listGridIds = new List<(int GridOrder, int GridId)>();
            grids.ForEach(g => listGridIds.Add((g.Order, g.GridId)));

            currentGrid = listGridIds.Find(tuple => tuple.GridOrder == 1);
            //retrieve list of elements
            GetElementsOfGrid(currentGrid.GridId);
        }

        public void CloseActivity()
        {

            TobiiSetUpService.StartGazeDeviceWatcher();
            NavigationView navView = GetNavigationView();
            navView.IsPaneVisible = true;
            navView.IsPaneToggleButtonVisible = true;
            IsActivityLoading = false;
            TobiiSetUpService.RemoveDevice();
            OpenActivityAlreadyCalled = false;

            if (ParametersService.IsAutomatic())
                timerForAtomaticGrid.Stop();

            SecondsLeftBeforeNextGrid = timeToWaitBeforeChangingGrid;

            //if an element is playing we stop it
            if (this.activatedElement != (null, null))
                Stop(this.activatedElement.source, this.activatedElement.element);
        }

        public void SetEditMode()
        {
            IsEditModeActivated = !IsEditModeActivated;
            if (EditButton.Equals("Modifier"))
            {
                EditButton = "Fin Modification";
                SelectionMode = "None";
                AllActivities.ForEach(a => a.EditMode = true);
            }
            else
            {
                EditButton = "Modifier";
                SelectionMode = "Single";
                AllActivities.ForEach(a => a.EditMode = false);
            }
        }
        public void GridView_Loading(FrameworkElement sender, object args)
        {
            ScrollViewer sc = sender as ScrollViewer;
            ScrollViewer = sc;
        }

        public void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            NavigationView navView = GetNavigationView();
            navView.IsPaneOpen = false;
        }

    }
}
