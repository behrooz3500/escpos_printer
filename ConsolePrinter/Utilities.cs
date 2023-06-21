using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ConsolePrinter
{
    public static class Utilities
    {
        public static string GetShamsiDate(this DateTime date)
        {
            /// An extension method for getting shamsi datetime
            
            PersianCalendar persianCalendar = new PersianCalendar();
            StringBuilder shamsiDate = new StringBuilder();

            shamsiDate.Append(persianCalendar.GetYear(date).ToString("0000"));
            shamsiDate.Append('/');
            shamsiDate.Append(persianCalendar.GetMonth(date).ToString("00"));
            shamsiDate.Append('/');
            shamsiDate.Append(persianCalendar.GetDayOfMonth(date).ToString("00"));
            return shamsiDate.ToString();
        }

        internal static Configuration LoadConfigurationFile(string configFileName)
        {
            /// Loading external configuration file
            ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
            configMap.ExeConfigFilename = Environment.CurrentDirectory + configFileName;

            return ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
        }

        [Serializable]
        internal class PrinterNotFoundException : ApplicationException
        {
            /// Custom exception fot managing printer not found
            public PrinterNotFoundException() { }
            public PrinterNotFoundException(string message) : base(message) { }
            public PrinterNotFoundException(string message, Exception innerException) : base(message, innerException) { }
        }

        internal static JObject LoadJsonFile(string jsonFilePath)
        {
            /// Load json object from given path
            string rawJsonContents = File.ReadAllText(jsonFilePath);
            JObject jsonContents = JsonConvert.DeserializeObject<JObject>(rawJsonContents);
            return jsonContents;
        }

        internal static void ErrorManager(Exception ex, Constants.ExitCode exitCode) {
            /// Managing return message based on the type of exception raised
            string errorMessage = ex.Message.ToLower();
            Console.WriteLine(exitCode.ToString());
            Console.Error.WriteLine(errorMessage);
            Environment.Exit((int)exitCode);
        }

    }
}
