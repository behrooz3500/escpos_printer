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
        public string stimulsoftBussinessObject => "receiptItems";
        public string stimulsoftReceiptDate => "receipt_date";
        public string stimulsoftReceiptCode => "receipt_code";
        public string stimulsoftCompanyTitle => "company_title";
        public string stimulsoftCompanyPhone => "company_phone";
        public string stimulsoftCompanyAddress => "company_address";
        public string stimulsoftPersonName => "person_name";
        public string stimulsoftPersonPhone => "person_phone";
        public string stimulsoftPersonAddress => "person_address";
        public string stimulsoftInitialPrice => "initial_price";
        public string stimulsoftDiscount => "discount";
        public string stimulsoftTax => "tax";
        public string stimulsoftFinalPayment => "final_payment";
        public string stimulsoftSummaryText => "summary_text";


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
