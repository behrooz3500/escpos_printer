using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsolePrinter
{
    public class Constants
    {
        // public string configPrinterName { get {return "printer_name";}}
        public string configPrinterName => "printer_name";
        public string configReportPath => "report_path";
        public string configFileName => "//app.config";
        public string licenseRelativePath => "//license.key";

        public string stimulsoftBussinessObject => "receiptItems";
        public string stimulsoftTimeVariable => "time";
        public string stimulsoftDateVariavle => "date";


        public enum ExitCode : int
        {
            /// <summary>
            ///  Printing job exit codes
            /// </summary>
            
            Success = 0,
            PrinterNotFound = 11,
            JsonNotFound = 12,
            ReportNotFound = 13,
            LicenseKeyMissed = 14,
            UnknownError = 15,
            NoArgumentFound = 16,
        }
    }
}
