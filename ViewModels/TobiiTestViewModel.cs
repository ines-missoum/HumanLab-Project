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

namespace humanlab.ViewModels
{
    public class TobiiTestViewModel : BaseViewModel
    {

        public TobiiTestViewModel()
        {
            TobiiSetUpService = new TobiiSetUpService(this.GazeEntered, this.GazeMoved, this.GazeExited, this.TimerGaze_Tick);
            TobiiSetUpService.StartGazeDeviceWatcher();
            //FocusTime = 0;
            MaxFocusTime = 5; //en sec
            //IsNotAnimated = true;
            ClickImage = new DelegateCommand<object>(ClickOnImage, CanClickOnImage);
            Elements = new List<ElementOfActivity>();
            Elements.Add(new ElementOfActivity("1", "/../UserAssets/chute.gif", MaxFocusTime, ClickImage, "ça doit faire mal !", null));
            Elements.Add(new ElementOfActivity("2", "/../UserAssets/ballon.gif", MaxFocusTime, ClickImage, null, "/../UserAssets/married-life.mp3"));
            playingSound = null;
            playingSpeech = null;
        }

        private List<ElementOfActivity> elements;
        private double maxFocusTime;
        private double focusTime1;
        private bool isNotAnimated1;
        private double focusTime2;
        private bool isNotAnimated2;
        private Transform transform;
        public DelegateCommand<object> ClickImage { get; set; }
        public TobiiSetUpService TobiiSetUpService { get; set; }

        private MediaPlayer playingSound;
        private MediaElement playingSpeech;

        public List<ElementOfActivity> Elements
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

        public bool IsNotAnimated1
        {
            get => isNotAnimated1;
            set
            {
                if (value != isNotAnimated1)
                {
                    isNotAnimated1 = value;
                    OnPropertyChanged("IsNotAnimated1");
                }
            }
        }
        public bool IsNotAnimated2
        {
            get => isNotAnimated2;
            set
            {
                if (value != isNotAnimated2)
                {
                    isNotAnimated2 = value;
                    OnPropertyChanged("IsNotAnimated2");
                }
            }
        }
        public double MaxFocusTime
        {
            get => maxFocusTime;
            set
            {
                if (value != maxFocusTime)
                {
                    maxFocusTime = value;
                }
            }
        }

        public double FocusTime1
        {
            get => focusTime1;
            set
            {
                if (value != focusTime1)
                {
                    focusTime1 = value;
                    OnPropertyChanged("FocusTime1");
                }
            }
        }

        public double FocusTime2
        {
            get => focusTime2;
            set
            {
                if (value != focusTime2)
                {
                    focusTime2 = value;
                    OnPropertyChanged("FocusTime2");
                }
            }
        }
        public Transform Transform
        {
            get => transform;
            set
            {
                if (value != transform)
                {
                    transform = value;
                    OnPropertyChanged("Transform");
                }
            }
        }



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
            // Update the position of the ellipse corresponding to gaze point.
            if (args.CurrentPoint.EyeGazePosition != null)
            {
                double gazePointX = args.CurrentPoint.EyeGazePosition.Value.X;
                double gazePointY = args.CurrentPoint.EyeGazePosition.Value.Y;

                //20 = width height !!!! to change corresponding to xaml
                //32 = taille de la progress bar (margin comprises)
                double ellipseLeft = gazePointX - (20 / 2.0f);
                double ellipseTop = gazePointY - (20 / 2.0f) - 32;

                // Translate transform for moving gaze ellipse.
                TranslateTransform translateEllipse = new TranslateTransform
                {
                    X = ellipseLeft,
                    Y = ellipseTop
                };

                Transform = translateEllipse;

                // The gaze point screen location.
                Point gazePoint = new Point(gazePointX, gazePointY);

                // Basic hit test to determine if gaze point is on progress bar.
                Image img = TobiiSetUpService.CurrentFocusImage as Image;
                bool hitRadialProgressBar = TobiiSetUpService.DoesElementContainPoint(gazePoint, null);
                if (img != null & !hitRadialProgressBar)
                {
                    ElementOfActivity current = Elements.Where(el => el.Id.Equals(img.Tag)).First();
                    current.FocusTime = 0;
                }


                // Mark the event handled.
                args.Handled = true;
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
                ElementOfActivity current = Elements.Where(el => el.Id.Equals(img.Tag)).First();
                BitmapImage source = img.Source as BitmapImage;

                // Increment progress bar.
                current.FocusTime += 0.02; //because the method is called each 20ms

                // If progress bar reaches maximum value, reset and relocate.
                if (current.FocusTime >= MaxFocusTime)//nb de sec
                {
                    // Ensure the gaze timer restarts on new progress bar location.
                    TobiiSetUpService.StopTimer();
                    current.FocusTime = 0;

                    Play(source, current);
                }
            }
            catch { }
        }

        private async void Play(BitmapImage source, ElementOfActivity current)
        {
            source.Play();
            current.IsNotAnimated = false;
            if (current.SoundName != null)
            {
                playingSound = new MediaPlayer();
                playingSound.Source = MediaSource.CreateFromUri(new Uri(current.SoundName));
                playingSound.Play();
            }
            else
            {
                playingSpeech = new MediaElement();
                var synt = new Windows.Media.SpeechSynthesis.SpeechSynthesizer();
                Windows.Media.SpeechSynthesis.SpeechSynthesisStream stream = await synt.SynthesizeTextToStreamAsync(current.SpeechText);
                playingSpeech.SetSource(stream, stream.ContentType);
                playingSpeech.Play();
            }

        }

        private void Stop(BitmapImage source, ElementOfActivity current)
        {
            source.Stop();
            current.IsNotAnimated = true;
            if (current.SoundName != null)
            {
                playingSound.Pause();
                playingSound.Source = null;
                playingSound = null;
            }
            else
            {
                playingSpeech.Stop();
                playingSpeech.Source = null;
                playingSpeech = null;
            }
        }

        ///MOUSE
        private void ClickOnImage(object args)
        {
            Image img = args as Image;
            ElementOfActivity current = Elements.Where(el => el.Id.Equals(img.Tag)).First();
            BitmapImage source = img.Source as BitmapImage;

            if (source.IsPlaying)
            {
                Stop(source, current);
            }
            else
            {
                Play(source, current);
            }

        }
        private bool CanClickOnImage(object args)
        {
            return true;
        }
    }
}
