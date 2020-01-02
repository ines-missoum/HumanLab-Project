using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;


namespace humanlab.Services
{
    /// <summary>
    /// Service that deals with all the application parameters
    /// </summary>
    class ParametersService
    {
        public static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        public static void SaveManual()
        {
            localSettings.Values["isAutomatic"] = false;
        }

        public static void SaveAutomatic(double gridTime, string mode)
        {
            localSettings.Values["isAutomatic"] = true;
            localSettings.Values["gridTime"] = gridTime;
            localSettings.Values["mode"] = mode;
        }

        public static bool IsAutomatic()
        {
            try
            {
                return (bool)localSettings.Values["isAutomatic"];
            }
            catch
            {
                return false;
            }
        }

        public static double  GetGridTime()
        {
            try
            {
                return (double)localSettings.Values["gridTime"];
            }
            catch
            {
                return 60;
            }
        }

        public static string GetMode()
        {
            string paramValue = "";
            try
            {
                paramValue = (string)localSettings.Values["mode"];
            }
            catch
            {
            }
            if (paramValue == null || paramValue.Equals(""))
                return "Ordonné";
            else
                return paramValue;
        }
    }
}
