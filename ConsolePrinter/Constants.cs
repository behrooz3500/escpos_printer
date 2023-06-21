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
        public string configFilePath => Environment.CurrentDirectory + "//config.json";
        public string licenseRelativePath => "//license.key";

        public string stimulsoftReceiptItemsKey => "items_list";
        public string stimulsoftReceiptInfoKey => "receipt_info";
        public string stimulsoftInvoicesBussinessObject => "receiptItems";
        public string stimulsoftReceiptBussinessObject => "receiptInfo";


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
            UnknownFileNotFound = 17,
        }
    }
}
