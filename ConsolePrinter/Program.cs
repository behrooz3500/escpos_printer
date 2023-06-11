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
using Newtonsoft.Json.Linq;

namespace ConsolePrinter
{
    internal class Program
    {
        enum ExitCode : int
        {
            Success = 1,
            PrinterNotFound = 11,
            JsonNotFound = 12,
            ReportNotFound = 13,
            LicenseKeyMissed = 14,
            UnknownError = 15,
            NoArgumentFound = 16,

        }

        [Serializable]
        public class PrinterNotFoundException: ApplicationException
        {
            public PrinterNotFoundException() { }
            public PrinterNotFoundException(string message) : base(message) { }
            public PrinterNotFoundException(string message, Exception innerException): base(message, innerException) { }
        }

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
            try
            {
                Stimulsoft.Base.StiLicense.LoadFromFile(Environment.CurrentDirectory + "//license.key");
                //Stimulsoft.Base.StiLicense.LoadFromString("6vJhGtLLLz2GNviWmUTrhSqnOItdDwjBylQzQcAOiHkgpgFGkUl79uxVs8X+uspx6K+tqdtOB5G1S6PFPRrlVNvMUiSiNYl724EZbrUAWwAYHlGLRbvxMviMExTh2l9xZJ2xc4K1z3ZVudRpQpuDdFq+fe0wKXSKlB6okl0hUd2ikQHfyzsAN8fJltqvGRa5LI8BFkA/f7tffwK6jzW5xYYhHxQpU3hy4fmKo/BSg6yKAoUq3yMZTG6tWeKnWcI6ftCDxEHd30EjMISNn1LCdLN0/4YmedTjM7x+0dMiI2Qif/yI+y8gmdbostOE8S2ZjrpKsgxVv2AAZPdzHEkzYSzx81RHDzZBhKRZc5mwWAmXsWBFRQol9PdSQ8BZYLqvJ4Jzrcrext+t1ZD7HE1RZPLPAqErO9eo+7Zn9Cvu5O73+b9dxhE2sRyAv9Tl1lV2WqMezWRsO55Q3LntawkPq0HvBkd9f8uVuq9zk7VKegetCDLb0wszBAs1mjWzN+ACVHiPVKIk94/QlCkj31dWCg8YTrT5btsKcLibxog7pv1+2e4yocZKWsposmcJbgG0");
                //Stimulsoft.Base.StiLicense.Key = "6vJhGtLLLz2GNviWmUTrhSqnOItdDwjBylQzQcAOiHkgpgFGkUl79uxVs8X+uspx6K+tqdtOB5G1S6PFPRrlVNvMUiSiNYl724EZbrUAWwAYHlGLRbvxMviMExTh2l9xZJ2xc4K1z3ZVudRpQpuDdFq+fe0wKXSKlB6okl0hUd2ikQHfyzsAN8fJltqvGRa5LI8BFkA/f7tffwK6jzW5xYYhHxQpU3hy4fmKo/BSg6yKAoUq3yMZTG6tWeKnWcI6ftCDxEHd30EjMISNn1LCdLN0/4YmedTjM7x+0dMiI2Qif/yI+y8gmdbostOE8S2ZjrpKsgxVv2AAZPdzHEkzYSzx81RHDzZBhKRZc5mwWAmXsWBFRQol9PdSQ8BZYLqvJ4Jzrcrext+t1ZD7HE1RZPLPAqErO9eo+7Zn9Cvu5O73+b9dxhE2sRyAv9Tl1lV2WqMezWRsO55Q3LntawkPq0HvBkd9f8uVuq9zk7VKegetCDLb0wszBAs1mjWzN+ACVHiPVKIk94/QlCkj31dWCg8YTrT5btsKcLibxog7pv1+2e4yocZKWsposmcJbgG0";
                Console.WriteLine("here");
                ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
                configMap.ExeConfigFilename = Environment.CurrentDirectory + "//app.config";
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
                Boolean isPrinterValid = false;
                foreach (string printer in PrinterSettings.InstalledPrinters)
                {
                    Console.WriteLine(printer);
                    if (printerName.ToString().Equals(printer, StringComparison.OrdinalIgnoreCase))
                    {
                        isPrinterValid = true;
                        break;
                    }
                }
                if (!isPrinterValid)
                {
                    throw new PrinterNotFoundException("printer name not found in installed printers!");
                }
                report.Print(false, xprinter);
                Environment.Exit((int)ExitCode.Success);
                //return (int)ExitCode.Success;

            }
            catch (FileNotFoundException ex)
            {
                string error_message = ex.Message.ToLower();
                if (error_message.Contains("License")) Environment.Exit((int)ExitCode.LicenseKeyMissed);
                else if (error_message.Contains(".mrt")) Environment.Exit((int)ExitCode.ReportNotFound);
                else if (error_message.Contains(".json")) Environment.Exit((int)ExitCode.JsonNotFound);
          
            }
            catch (PrinterNotFoundException)
            {
                Environment.Exit((int)ExitCode.PrinterNotFound);
            }
            catch (System.IndexOutOfRangeException)
            {

                Environment.Exit((int)ExitCode.NoArgumentFound);
            }
            catch (Exception ex) {
                Console.WriteLine(ex.GetType());
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.ToString());

                Environment.Exit((int)ExitCode.UnknownError);

            }
        }
    }
}
