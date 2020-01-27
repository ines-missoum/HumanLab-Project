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

        /// <summary>
        /// Method responsible for notifying the view for the change
        /// </summary>
        /// <param name="propertyName"> The public property name in the view model (the one linked in the view)</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// This is a generic setter. It checked if the value of the storage property has changed. If this is the case then it updates the value and notify the change.
        /// </summary>
        /// <typeparam name="T"> generic type </typeparam>
        /// <param name="storage">the private property in the view model</param>
        /// <param name="value">the new value of storage property </param>
        /// <param name="propertyName"> The public property name in the view model (the one linked in the view)</param>
        /// <returns></returns>
        protected virtual bool SetProperty<T>(ref T storage, T value,  string propertyName)
        {
            if (Equals(storage, value)) return false;

            storage = value;
            OnPropertyChanged(propertyName);

            return true;
        }
    }
}
