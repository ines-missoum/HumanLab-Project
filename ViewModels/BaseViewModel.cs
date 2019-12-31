using System.ComponentModel;

namespace humanlab.ViewModels
{
    /// <summary>
    /// Cette classe représente l'emetteur d'evenement sur les changements de propriétés. Elle va être utilisée comme base de tous les ViewModel
    /// </summary>
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
