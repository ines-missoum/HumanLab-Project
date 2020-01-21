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
using System.Diagnostics;

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
            translateTransform.X += e.Delta.Translation.X * 1 / scrollview.ZoomFactor;
            translateTransform.Y += e.Delta.Translation.Y * 1 / scrollview.ZoomFactor;

            

        }

        private void k_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            FrameworkElement origin = sender as FrameworkElement;
            FrameworkElement parent = origin.Parent as FrameworkElement;

            var localCoords = e.Position;
            var relativeTransform = origin.TransformToVisual(parent);
            Point parentContainerCoords = relativeTransform.TransformPoint(localCoords);
            var center = parentContainerCoords;

            Debug.WriteLine("center" + center);
            // translate/panning

            var m = sender as Image;
            
            var x=m.CenterPoint.X;
            var y = m.CenterPoint.Y;
            var z = m.CenterPoint.Z;
            var p = m.CenterPoint;
/*
            Debug.WriteLine("Ahdeig " + m.ActualHeight * scrollview.ZoomFactor);
            Debug.WriteLine("Awithd " + m.ActualWidth * scrollview.ZoomFactor);
            Debug.WriteLine("hdeig " + m.Height * scrollview.ZoomFactor);
            Debug.WriteLine("withd " + m.Width * scrollview.ZoomFactor);*/
        }


        private void scrollview_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {

            var l = scrollview.ZoomFactor;
        /*    Debug.WriteLine("extent " + scrollview.ActualWidth);
            Debug.WriteLine("extent " + scrollview.ActualHeight); */


        }
    }
}
