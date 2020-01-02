using System;
using System.ComponentModel;
using Windows.UI.Xaml;

namespace humanlab.ViewModels
{
    /// <summary>
    /// Cette classe représente l'emetteur d'evenement sur les changements de propriétés. Elle va être utilisée comme base de tous les ViewModel
    /// </summary>
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        protected String ColorTheme = Application.Current.Resources["SystemAccentColor"].ToString();
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
