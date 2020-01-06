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

namespace humanlab.ViewModels
{
    public class TobiiTestViewModel : BaseViewModel
    {

        public TobiiTestViewModel()
        {
            StartGazeDeviceWatcher();

            FocusTime = 0;
            System.Diagnostics.Debug.WriteLine("started");

        }

        public double FocusTime { get; set; }
        private string myColor;
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
        /// <summary>
        /// Reference to the user's eyes and head as detected
        /// by the eye-tracking device.
        /// </summary>
        private GazeInputSourcePreview gazeInputSource;

        /// <summary>
        /// Dynamic store of eye-tracking devices.
        /// </summary>
        /// <remarks>
        /// Receives event notifications when a device is added, removed, 
        /// or updated after the initial enumeration.
        /// </remarks>
        private GazeDeviceWatcherPreview gazeDeviceWatcher;

        /// <summary>
        /// Eye-tracking device counter.
        /// </summary>
        private int deviceCounter = 0;

        /// <summary>
        /// Timer for gaze focus on RadialProgressBar.
        /// </summary>
        DispatcherTimer timerGaze = new DispatcherTimer();

        /// <summary>
        /// Tracker used to prevent gaze timer restarts.
        /// </summary>
        bool timerStarted = false;

        /// <summary>
        /// Start gaze watcher and declare watcher event handlers.
        /// </summary>
        private void StartGazeDeviceWatcher()
        {
            if (gazeDeviceWatcher == null)
            {
                gazeDeviceWatcher = GazeInputSourcePreview.CreateWatcher();
                gazeDeviceWatcher.Added += this.DeviceAdded;
                gazeDeviceWatcher.Updated += this.DeviceUpdated;
                gazeDeviceWatcher.Removed += this.DeviceRemoved;
                gazeDeviceWatcher.Start();
                System.Diagnostics.Debug.WriteLine("StartGazeDeviceWatcher");
            }
        }


        /// <summary>
        /// Eye-tracking device connected (added, or available when watcher is initialized).
        /// </summary>
        /// <param name="sender">Source of the device added event</param>
        /// <param name="e">Event args for the device added event</param>
        private void DeviceAdded(GazeDeviceWatcherPreview source,
            GazeDeviceWatcherAddedPreviewEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine("DeviceAdded");
            if (IsSupportedDevice(args.Device))
            {
                deviceCounter++;
            }
            // Set up gaze tracking.
            TryEnableGazeTrackingAsync(args.Device);
        }

        /// <summary>
        /// Initial device state might be uncalibrated, 
        /// but device was subsequently calibrated.
        /// </summary>
        /// <param name="sender">Source of the device updated event</param>
        /// <param name="e">Event args for the device updated event</param>
        private void DeviceUpdated(GazeDeviceWatcherPreview source,
            GazeDeviceWatcherUpdatedPreviewEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine("DeviceUpdated");
            // Set up gaze tracking.
            TryEnableGazeTrackingAsync(args.Device);
        }

        /// <summary>
        /// Handles disconnection of eye-tracking devices.
        /// </summary>
        /// <param name="sender">Source of the device removed event</param>
        /// <param name="e">Event args for the device removed event</param>
        private void DeviceRemoved(GazeDeviceWatcherPreview source,
            GazeDeviceWatcherRemovedPreviewEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine("DeviceRemoved");
            // Decrement gaze device counter and remove event handlers.
            if (IsSupportedDevice(args.Device))
            {
                deviceCounter--;

                if (deviceCounter == 0)
                {
                    gazeInputSource.GazeEntered -= this.GazeEntered;
                    gazeInputSource.GazeMoved -= this.GazeMoved;
                    gazeInputSource.GazeExited -= this.GazeExited;
                }
            }
        }

        /// <summary>
        /// Initialize gaze tracking.
        /// </summary>
        /// <param name="gazeDevice"></param>
        private async void TryEnableGazeTrackingAsync(GazeDevicePreview gazeDevice)
        {
            System.Diagnostics.Debug.WriteLine("TryEnableGazeTrackingAsync");
            // If eye-tracking device is ready, declare event handlers and start tracking.
            if (IsSupportedDevice(gazeDevice))
            {
                timerGaze.Interval = new TimeSpan(0, 0, 0, 0, 20);
                timerGaze.Tick += TimerGaze_Tick;

                // This must be called from the UI thread.
                gazeInputSource = GazeInputSourcePreview.GetForCurrentView();

                gazeInputSource.GazeEntered += GazeEntered;
                gazeInputSource.GazeMoved += GazeMoved;
                gazeInputSource.GazeExited += GazeExited;
            }
            // Notify if device calibration required.
            else if (gazeDevice.ConfigurationState ==
                        GazeDeviceConfigurationStatePreview.UserCalibrationNeeded ||
                        gazeDevice.ConfigurationState ==
                        GazeDeviceConfigurationStatePreview.ScreenSetupNeeded)
            {
                // Device isn't calibrated, so invoke the calibration handler.
                System.Diagnostics.Debug.WriteLine(
                    "Votre appareil est en cours de calibrage. Veuillez patientez s'il-vous-plait.");
                await gazeDevice.RequestCalibrationAsync();
            }
            // Notify if device calibration underway.
            else if (gazeDevice.ConfigurationState ==
                GazeDeviceConfigurationStatePreview.Configuring)
            {
                // Device is currently undergoing calibration.  
                // A device update is sent when calibration complete.
                System.Diagnostics.Debug.WriteLine(
                    "Votre appareil est en cours de configuration. Veuillez patientez s'il-vous-plait.");
            }
            // Device is not viable.
            else if (gazeDevice.ConfigurationState == GazeDeviceConfigurationStatePreview.Unknown)
            {
                // Notify if device is in unknown state.  
                // Reconfigure/recalbirate the device.  
                System.Diagnostics.Debug.WriteLine(
                    "Votre appareil n'est pas prêt. S'il-vous-plait installez votre appareil et reconfigurez le.");
            }
        }

        /// <summary>
        /// Check if eye-tracking device is viable.
        /// </summary>
        /// <param name="gazeDevice">Reference to eye-tracking device.</param>
        /// <returns>True, if device is viable; otherwise, false.</returns>
        private bool IsSupportedDevice(GazeDevicePreview gazeDevice)
        {
            System.Diagnostics.Debug.WriteLine("IsSupportedDevice");
            //TrackerState.Text = gazeDevice.ConfigurationState.ToString();
            return (gazeDevice.CanTrackEyes &&
                        gazeDevice.ConfigurationState ==
                        GazeDeviceConfigurationStatePreview.Ready);
        }

        /// <summary>
        /// GazeEntered handler.
        /// </summary>
        /// <param name="sender">Source of the gaze entered event</param>
        /// <param name="e">Event args for the gaze entered event</param>
        private void GazeEntered(
            GazeInputSourcePreview sender,
            GazeEnteredPreviewEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine("GazeEntered");
            ///TO DO 

            // Mark the event handled.
            args.Handled = true;
        }

        /// <summary>
        /// GazeExited handler.
        /// Call DisplayRequest.RequestRelease to conclude the 
        /// RequestActive called in GazeEntered.
        /// </summary>
        /// <param name="sender">Source of the gaze exited event</param>
        /// <param name="e">Event args for the gaze exited event</param>
        private void GazeExited(
            GazeInputSourcePreview sender,
            GazeExitedPreviewEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine("GazeExited");
            ///TO DO 

            // Mark the event handled.
            args.Handled = true;
        }

        /// <summary>
        /// GazeMoved handler translates the ellipse on the canvas to reflect gaze point.
        /// </summary>
        /// <param name="sender">Source of the gaze moved event</param>
        /// <param name="e">Event args for the gaze moved event</param>
        private void GazeMoved(GazeInputSourcePreview sender, GazeMovedPreviewEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine("GazeMoved");
            // Update the position of the ellipse corresponding to gaze point.
            if (args.CurrentPoint.EyeGazePosition != null)
            {
                double gazePointX = args.CurrentPoint.EyeGazePosition.Value.X;
                double gazePointY = args.CurrentPoint.EyeGazePosition.Value.Y;

                // The gaze point screen location.
                Point gazePoint = new Point(gazePointX, gazePointY);

                Image img = new Image();
                img.Name = "TestImage";
                // Basic hit test to determine if gaze point is on progress bar.
                bool hitRadialProgressBar =
                    DoesElementContainPoint(
                        gazePoint,
                        "TestImage",
                        img);

                // Use progress bar thickness for visual feedback.
                if (hitRadialProgressBar)
                {
                    ///TO DO 
                }
                else
                {
                    ///TO DO 
                }

                // Mark the event handled.
                args.Handled = true;
            }
        }

        /// <summary>
        /// Return whether the gaze point is over the progress bar.
        /// </summary>
        /// <param name="gazePoint">The gaze point screen location</param>
        /// <param name="elementName">The progress bar name</param>
        /// <param name="uiElement">The progress bar UI element</param>
        /// <returns></returns>
        private bool DoesElementContainPoint(
            Point gazePoint, string elementName, UIElement uiElement)
        {
            System.Diagnostics.Debug.WriteLine("DoesElementContainPoint");
            // Use entire visual tree of progress bar.
            IEnumerable<UIElement> elementStack =
                VisualTreeHelper.FindElementsInHostCoordinates(gazePoint, uiElement, true);
            foreach (UIElement item in elementStack)
            {
                //Cast to FrameworkElement and get element name.
                if (item is FrameworkElement feItem)
                {
                    if (feItem.Name.Equals(elementName))
                    {
                        if (!timerStarted)
                        {
                            // Start gaze timer if gaze over element.
                            timerGaze.Start();
                            timerStarted = true;
                        }
                        return true;
                    }
                }
            }

            // Stop gaze timer and reset progress bar if gaze leaves element.
            timerGaze.Stop();
            timerStarted = false;
            return false;
        }

        /// <summary>
        /// Tick handler for gaze focus timer.
        /// </summary>
        /// <param name="sender">Source of the gaze entered event</param>
        /// <param name="e">Event args for the gaze entered event</param>
        private void TimerGaze_Tick(object sender, object e)
        {
            System.Diagnostics.Debug.WriteLine("TimerGaze_Tick");
            // Increment progress bar.
            FocusTime += 1;

            // If progress bar reaches maximum value, reset and relocate.
            if (FocusTime == 100)
            {
                // Ensure the gaze timer restarts on new progress bar location.
                timerGaze.Stop();
                timerStarted = false;
                FocusTime = 0;
                MyColor = "Red";
            }
        }

    }
}
