using System;
using System.Collections.Generic;
using Windows.Devices.Input.Preview;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace humanlab.Services
{
    public class TobiiSetUpService
    {
        public TobiiSetUpService(
            TypedEventHandler<GazeInputSourcePreview, GazeEnteredPreviewEventArgs> GazeEntered,
            TypedEventHandler<GazeInputSourcePreview, GazeMovedPreviewEventArgs> gazeMoved,
            TypedEventHandler<GazeInputSourcePreview, GazeExitedPreviewEventArgs> GazeExited,
            EventHandler<object> tick)
        {
            GazeMovedHandler = gazeMoved;
            GazeExitedHandler = GazeExited;
            GazeEnteredHandler = GazeEntered;
            TickHandler = tick;
            CurrentFocusImage = null;
        }

        //public delegate void TickDelegate(object sender, object args);
        private TypedEventHandler<GazeInputSourcePreview, GazeMovedPreviewEventArgs> GazeMovedHandler { get; }
        private TypedEventHandler<GazeInputSourcePreview, GazeExitedPreviewEventArgs> GazeExitedHandler { get; }
        private TypedEventHandler<GazeInputSourcePreview, GazeEnteredPreviewEventArgs> GazeEnteredHandler { get; }
        private EventHandler<object> TickHandler { get; }

        public UIElement CurrentFocusImage { get; set; }
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
        DispatcherTimer TimerGaze { get; } = new DispatcherTimer();

        /// <summary>
        /// Tracker used to prevent gaze timer restarts.
        /// </summary>
        bool timerStarted = false;

        /// <summary>
        /// Start gaze watcher and declare watcher event handlers.
        /// </summary>
        public void StartGazeDeviceWatcher()
        {
            if (gazeDeviceWatcher == null)
            {
                gazeDeviceWatcher = GazeInputSourcePreview.CreateWatcher();
                gazeDeviceWatcher.Added += this.DeviceAdded;
                gazeDeviceWatcher.Updated += this.DeviceUpdated;
                gazeDeviceWatcher.Removed += this.DeviceRemoved;
                gazeDeviceWatcher.Start();
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
            // Decrement gaze device counter and remove event handlers.
            if (IsSupportedDevice(args.Device))
            {
                deviceCounter--;

                if (deviceCounter == 0)
                {
                    gazeInputSource.GazeEntered -= this.GazeEnteredHandler;
                    gazeInputSource.GazeMoved -= this.GazeMovedHandler;
                    gazeInputSource.GazeExited -= this.GazeExitedHandler;
                }
            }
        }

        /// <summary>
        /// Initialize gaze tracking.
        /// </summary>
        /// <param name="gazeDevice"></param>
        private async void TryEnableGazeTrackingAsync(GazeDevicePreview gazeDevice)
        {
            // If eye-tracking device is ready, declare event handlers and start tracking.
            if (IsSupportedDevice(gazeDevice))
            {
                TimerGaze.Interval = new TimeSpan(0, 0, 0, 0, 20); //20ms
                TimerGaze.Tick += this.TickHandler;

                // This must be called from the UI thread.
                gazeInputSource = GazeInputSourcePreview.GetForCurrentView();

                gazeInputSource.GazeEntered += this.GazeEnteredHandler;
                gazeInputSource.GazeMoved += this.GazeMovedHandler;
                gazeInputSource.GazeExited += this.GazeExitedHandler;
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
            return (gazeDevice.CanTrackEyes && gazeDevice.ConfigurationState == GazeDeviceConfigurationStatePreview.Ready);
        }

        /// <summary>
        /// Return whether the gaze point is over the progress bar.
        /// </summary>
        /// <param name="gazePoint">The gaze point screen location</param>
        /// <param name="elementName">The progress bar name</param>
        /// <param name="uiElement">The progress bar UI element</param>
        /// <returns></returns>
        public bool DoesElementContainPoint(Point gazePoint, string elementName, UIElement uiElement)
        {
            // Use entire visual tree of progress bar.
            IEnumerable<UIElement> elementStack = VisualTreeHelper.FindElementsInHostCoordinates(gazePoint, uiElement, true);
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
                            StartTimer();
                        }
                        CurrentFocusImage = feItem;
                        return true;
                    }
                }
            }

            // Stop gaze timer and reset progress bar if gaze leaves element.
            StopTimer();
            CurrentFocusImage = null;
            return false;
        }
        public void StopTimer()
        {
            TimerGaze.Stop();
            timerStarted = false;
        }

        private void StartTimer()
        {
            TimerGaze.Start();
            timerStarted = true;
        }
    }
}
