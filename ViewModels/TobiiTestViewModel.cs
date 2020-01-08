using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

namespace humanlab.ViewModels
{
    public class TobiiTestViewModel : BaseViewModel
    {

        public TobiiTestViewModel()
        {
            TobiiSetUpService = new TobiiSetUpService(this.GazeEntered, this.GazeMoved, this.GazeExited, this.TimerGaze_Tick);
            TobiiSetUpService.StartGazeDeviceWatcher();
            FocusTime = 0;
            MaxFocusTime = 5; //en sec
            IsNotAnimated = true;
            ClickImage = new DelegateCommand<object>(ClickOnImage, CanClickOnImage);
        }

        private double maxFocusTime;
        private double focusTime;
        private bool isNotAnimated;
        private Transform transform;
        public DelegateCommand<object> ClickImage { get; set; }
        public TobiiSetUpService TobiiSetUpService { get; set; }

        public bool IsNotAnimated
        {
            get => isNotAnimated;
            set
            {
                if (value != isNotAnimated)
                {
                    isNotAnimated = value;
                    OnPropertyChanged("IsNotAnimated");
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
       
        public double FocusTime
        {
            get => focusTime;
            set
            {
                if (value != focusTime)
                {
                    focusTime = value;
                    OnPropertyChanged("FocusTime");
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
                bool hitRadialProgressBar = TobiiSetUpService.DoesElementContainPoint(gazePoint, "TestImage", null);
                if (!hitRadialProgressBar)
                    this.FocusTime = 0;

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
            // Increment progress bar.
            FocusTime += 0.02; //because the method is called each 20ms

            // If progress bar reaches maximum value, reset and relocate.
            if (FocusTime >= MaxFocusTime)//nb de sec
            {
                // Ensure the gaze timer restarts on new progress bar location.
                TobiiSetUpService.StopTimer();
                FocusTime = 0;
                try
                {
                    Image img = TobiiSetUpService.CurrentFocusImage as Image;
                    BitmapImage source = img.Source as BitmapImage;
                    source.Play();
                    IsNotAnimated = false;
                }
                catch { }


            }
        }

        ///MOUSE
        private void ClickOnImage(object args)
        {
            Image img = args as Image;
            BitmapImage source = img.Source as BitmapImage;

            if (source.IsPlaying)
            {
                source.Stop();
                IsNotAnimated = true;
            }
            else
            {
                source.Play();
                IsNotAnimated = false;
            }

        }
        private bool CanClickOnImage(object args)
        {
            return true;
        }
    }
}
