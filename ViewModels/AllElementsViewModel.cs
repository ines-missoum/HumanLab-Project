using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using humanlab.DAL;
using humanlab.Models;
using System.Diagnostics;
using Windows.UI.Xaml.Media.Imaging;

namespace humanlab.ViewModels
{
    class AllElementsViewModel : BaseViewModel
    {
        public List<Element> AllElements { get; set; }

    Repository repository;

        public AllElementsViewModel()
        {
            this.repository = new Repository();
            GetElementsAsync();

        }
        
   
        private async void GetElementsAsync() => AllElements = await repository.GetElementsAsync();
        
    }
}
