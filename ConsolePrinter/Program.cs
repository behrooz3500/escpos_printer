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
using Newtonsoft.Json.Linq;
using NLog;
using System.Drawing.Text;
using System.Collections.Specialized;

namespace ConsolePrinter
{
    internal class Program
    {
        
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        [STAThread]
        static void Main(string[] args)
        {
            /// Printing json contents
            /// args: full path to the json file
            
            Constants constants = new Constants();
            

            try
            {
                Logger.Info(">>>>>>>>>>>>>>> New Print Job Started!<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
                var installedPrinters = PrinterSettings.InstalledPrinters;
                string installedPrintersString = "";
                Logger.Info("Currently installed printer on the system are:");
                int counter = 0;

                foreach(string installedPrinter in installedPrinters) {
                    installedPrintersString += ($"{counter+1}-{installedPrinter.ToString()}, ");
                    counter++;
                }
                Logger.Info(installedPrintersString);

                // Load stimulsoft license
                Stimulsoft.Base.StiLicense.LoadFromFile(Environment.CurrentDirectory + constants.licenseRelativePath);
                Logger.Info("License found!");

                // Load Config File and fetch parameters from it
                JObject config = Utilities.LoadJsonFile(constants.configFilePath);
                Logger.Info("Config file found!");

                string printerName = config[constants.configPrinterName].ToString();
                string reportPath = config[constants.configReportPath].ToString();
                Logger.Info($"Printer name is: {printerName}");
                Logger.Info($"Report path is: {reportPath}");

                // Validating printer name
                if (!PrinterSettings.InstalledPrinters.Cast<string>().Contains(printerName, StringComparer.OrdinalIgnoreCase))
                {
                    Logger.Error("Printer not found! Check if it is fully installed and turned on.");
                    throw new Utilities.PrinterNotFoundException("Printer name not found in installed printers!");
                }

                JObject jsonData = Utilities.LoadJsonFile(args[0]);
                var invoicesBusinessObject = jsonData[constants.stimulsoftReceiptItemsKey];
                var infoBusinessObject = jsonData[constants.stimulsoftReceiptInfoKey];

                // Prepare stimulsoft report
                StiReport report = new StiReport();
                report.RegBusinessObject(name: constants.stimulsoftInvoicesBussinessObject, value:invoicesBusinessObject);
                Logger.Info("Receipt Items registered.");
                report.RegBusinessObject(name: constants.stimulsoftReceiptBussinessObject, value: infoBusinessObject);
                Logger.Info("Receipt info registered.");
                report.Load(reportPath);
                report.Compile();
                Logger.Info("Report compilation completed.");


                // Set printer settings
                PrinterSettings xprinter = new PrinterSettings();
                xprinter.PrinterName = printerName;
                report.Print(showPrintDialog:false, printerSettings:xprinter);

                Environment.Exit((int)Constants.ExitCode.Success);
            }

            catch (FileNotFoundException ex)
            {
                string error_message = ex.Message.ToLower();
                Logger.Error(ex);

                if (error_message.Contains("License")) Utilities.ErrorManager(ex, Constants.ExitCode.LicenseKeyMissed);
                else if (error_message.Contains(".mrt")) Utilities.ErrorManager(ex, Constants.ExitCode.ReportNotFound);
                else if (error_message.Contains(".json")) Utilities.ErrorManager(ex, Constants.ExitCode.JsonNotFound);
                else Utilities.ErrorManager(ex, Constants.ExitCode.UnknownFileNotFound);
            }

            catch (Utilities.PrinterNotFoundException ex)
            {
                Logger.Error(ex);
                Utilities.ErrorManager(ex, Constants.ExitCode.PrinterNotFound);
            }

            catch (IndexOutOfRangeException ex)
            {
                Logger.Error(ex);
                Utilities.ErrorManager(ex, Constants.ExitCode.NoArgumentFound);
            }

            catch (Exception ex) {
                string error_message = ex.Message.ToLower();
                Logger.Error(ex);
                if (error_message.Contains("definition")) Utilities.ErrorManager(ex, Constants.ExitCode.ErrorInReportCompilation);
                else Utilities.ErrorManager(ex, Constants.ExitCode.UnknownError);
            }
        }

    }
}
