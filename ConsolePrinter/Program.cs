﻿using System;
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

namespace ConsolePrinter
{
    internal class Program
    {

        [STAThread]
        static void Main(string[] args)
        {
            /// Printing json contents
            /// args: full path to the json file

            Constants constants = new Constants();

            try
            {
                // Load stimulsoft license
                Stimulsoft.Base.StiLicense.LoadFromFile(Environment.CurrentDirectory + constants.licenseRelativePath);

                // Load Config File and fetch parameters from it
                JObject config = Utilities.LoadJsonFile(constants.configFilePath);
                string printerName = config[constants.configPrinterName].ToString();
                string reportPath = config[constants.configReportPath].ToString();

                // Validating printer name
                if (!PrinterSettings.InstalledPrinters.Cast<string>().Contains(printerName, StringComparer.OrdinalIgnoreCase))
                {
                    throw new Utilities.PrinterNotFoundException("Printer name not found in installed printers!");
                }

                JObject jsonData = Utilities.LoadJsonFile(args[0]);
                var invoicesBusinessObject = jsonData[constants.stimulsoftReceiptItemsKey];
                var infoBusinessObject = jsonData[constants.stimulsoftReceiptInfoKey];
                
                // Prepare stimulsoft report
                StiReport report = new StiReport();
                report.RegBusinessObject(name: constants.stimulsoftInvoicesBussinessObject, value:invoicesBusinessObject);
                report.RegBusinessObject(name: constants.stimulsoftReceiptBussinessObject, value: infoBusinessObject);
                report.Load(reportPath);
                report.Compile();

                // Set printer settings
                PrinterSettings xprinter = new PrinterSettings();
                xprinter.PrinterName = printerName;
                report.Print(showPrintDialog:false, printerSettings:xprinter);

                Environment.Exit((int)Constants.ExitCode.Success);
            }

            catch (FileNotFoundException ex)
            {
                string error_message = ex.Message.ToLower();
                if (error_message.Contains("License")) Environment.Exit((int)Constants.ExitCode.LicenseKeyMissed);
                else if (error_message.Contains(".mrt")) Environment.Exit((int)Constants.ExitCode.ReportNotFound);
                else if (error_message.Contains(".json")) Environment.Exit((int)Constants.ExitCode.JsonNotFound);
                else Environment.Exit((int)Constants.ExitCode.UnknownError);
            }

            catch (Utilities.PrinterNotFoundException)
            {
                Environment.Exit((int)Constants.ExitCode.PrinterNotFound);
            }

            catch (IndexOutOfRangeException)
            {
                Environment.Exit((int)Constants.ExitCode.NoArgumentFound);
            }

            catch (Exception ex) {
                Console.WriteLine(ex.GetType());
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.ToString());

                Environment.Exit((int)Constants.ExitCode.UnknownError);
                              
            }
        }

    }
}
