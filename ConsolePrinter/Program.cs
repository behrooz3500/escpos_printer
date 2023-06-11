using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Stimulsoft.Report;
using System.Drawing.Printing;
using System.Security.Cryptography.X509Certificates;
using System.Configuration;
using System.Globalization;

namespace ConsolePrinter
{
    internal class Program
    {
        public static string GetShamsiDate(DateTime date)
        {
            PersianCalendar persianCalendar = new PersianCalendar();
            StringBuilder shamsiDate = new StringBuilder();

            shamsiDate.Append(persianCalendar.GetYear(date).ToString("0000"));
            shamsiDate.Append('/');
            shamsiDate.Append(persianCalendar.GetMonth(date).ToString("00"));
            shamsiDate.Append('/');
            shamsiDate.Append(persianCalendar.GetDayOfMonth(date).ToString("00"));
            return shamsiDate.ToString();

        }

        [STAThread]
        static void Main(string[] args)
        {

            Stimulsoft.Base.StiLicense.LoadFromFile(Environment.CurrentDirectory+"//license.key");

            ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
            configMap.ExeConfigFilename = Environment.CurrentDirectory+"//app.config";
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
            var settings = config.AppSettings.Settings;
            string printerName = settings["printer_name"].Value;

            string jsonString = File.ReadAllText(args[0]);
            DataTable dataTable = JsonConvert.DeserializeObject<DataTable>(jsonString);

            StiReport report = new StiReport();
            report.RegBusinessObject("receiptItems", dataTable);
            report.Load(settings["report_path"].Value);
            report.Compile();
            report["time"] = DateTime.Now.ToString("HH:mm:ss");
            report["date"] = GetShamsiDate(DateTime.Now);

            PrinterSettings xprinter = new PrinterSettings();
            xprinter.PrinterName = printerName;
            report.Print(false, xprinter);
        }
    }
}
