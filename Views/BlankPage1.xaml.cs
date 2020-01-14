using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace humanlab.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BlankPage1 : Page
    {
        public BlankPage1()
        {
            this.InitializeComponent();
        }

        private void t_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            FrameworkElement origin = sender as FrameworkElement;
            FrameworkElement parent = origin.Parent as FrameworkElement;

            var localCoords = e.Position;
            var relativeTransform = origin.TransformToVisual(parent);
            Point parentContainerCoords = relativeTransform.TransformPoint(localCoords);
            var center = parentContainerCoords;

            // translate/panning
            translateTransform.X += e.Delta.Translation.X;
            translateTransform.Y += e.Delta.Translation.Y;

            rotateTransform.CenterX = center.X;
            rotateTransform.CenterY = center.Y;
            rotateTransform.Angle += e.Delta.Rotation;

            scaleTransform.CenterX = center.X;
            scaleTransform.CenterY = center.Y;
            scaleTransform.ScaleX *= e.Delta.Scale;
            scaleTransform.ScaleY *= e.Delta.Scale;
        }
        private void scrollview_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            image.Width = scrollview.ViewportWidth;
            image.Height = scrollview.ViewportHeight;
        }
    }
}
