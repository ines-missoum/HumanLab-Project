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

namespace humanlab.ViewModels
{
    public class TobiiTestViewModel : BaseViewModel
    {

        public TobiiTestViewModel()
        {
            TobiiSetUpService = new TobiiSetUpService(this.GazeEntered, this.GazeMoved, this.GazeExited, this.TimerGaze_Tick);
            TobiiSetUpService.StartGazeDeviceWatcher();
            FocusTime = 0;
            ClickImage = new DelegateCommand(ClickOnImage, CanClickOnImage);
        }

        public double focusTime;
        private string myColor;
        private Transform transform;
        public DelegateCommand ClickImage { get; set; }
        public TobiiSetUpService TobiiSetUpService { get; set; }

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
        public String MyColor
        {
            get => myColor;
            set
            {
                if (value != myColor)
                {
                    myColor = value;
                    OnPropertyChanged("MyColor");
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
                double ellipseLeft = gazePointX - (20 / 2.0f);
                double ellipseTop = gazePointY - (20 / 2.0f);

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
                bool hitRadialProgressBar = TobiiSetUpService.DoesElementContainPoint(gazePoint,"TestImage",null);

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
            FocusTime += 1;

            // If progress bar reaches maximum value, reset and relocate.
            if (FocusTime == 100)
            {
                // Ensure the gaze timer restarts on new progress bar location.
                TobiiSetUpService.StopTimer();
                FocusTime = 0;
                MyColor = "Red";
            }
        }

        ///MOUSE
        private void ClickOnImage()
        {
            if (MyColor != null && MyColor.Equals("Red"))
                MyColor = "Blue";
            else
                MyColor = "Red";
        }
        private bool CanClickOnImage()
        {
            return true;
        }
    }
}
