using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Prism.Commands;
using Windows.UI.Popups;

namespace humanlab.Services
{
    public class DisplayMessagesService
    {
        public async static void showSuccessMessage(string type, string name,Action command)
        {
            string endVerb = "";
            if (!type.Equals("élément")) endVerb = "e";

            ContentDialog cd = new ContentDialog
            {
                Title = "Enregistrement de votre "+type,
                Content = "Votre "+type+" " + name + " a été sauvegardé"+ endVerb +" avec succès.",
                CloseButtonCommand = new DelegateCommand(command),
                CloseButtonText = "Fermer",
            };
            await cd.ShowAsync();
        }

        public async static void showErrorMessage()
        {
            MessageDialog errorMessageDialog = new MessageDialog("Une erreur s'est produite lors de l'enregistrement, Veuillez réessayer");
            await errorMessageDialog.ShowAsync();
        }
    }
}
