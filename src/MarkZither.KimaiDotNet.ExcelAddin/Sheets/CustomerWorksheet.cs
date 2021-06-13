using Microsoft.Office.Interop.Excel;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MarkZither.KimaiDotNet.ExcelAddin.Sheets
{
    public sealed class CustomerWorksheet
    {
        private static readonly CustomerWorksheet instance = new CustomerWorksheet();
        public readonly Worksheet Worksheet;

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static CustomerWorksheet()
        {
        }

        private CustomerWorksheet()
        {
            Worksheet currentSheet = Globals.ThisAddIn.Application.ActiveSheet as Microsoft.Office.Interop.Excel.Worksheet;

            var customerWorksheet =
                Globals.ThisAddIn.Application.Worksheets.Cast<Worksheet>()
                       .SingleOrDefault(w => string.Equals(w.Name, ExcelAddin.Constants.CustomersSheet.CustomersSheetName, StringComparison.OrdinalIgnoreCase));
            if (customerWorksheet is Worksheet)
            {
                Worksheet = customerWorksheet;
            }
            else
            {
                customerWorksheet = (Microsoft.Office.Interop.Excel.Worksheet)Globals.ThisAddIn.Application.Worksheets.Add(Missing.Value, Globals.ThisAddIn.Application.ActiveSheet as Worksheet);
                customerWorksheet.Name = ExcelAddin.Constants.CustomersSheet.CustomersSheetName;
                customerWorksheet.Visible = XlSheetVisibility.xlSheetVeryHidden;
                Worksheet = customerWorksheet;
                // switch straight back to the current active sheet
                currentSheet.Select();
            }
        }

        public static CustomerWorksheet Instance
        {
            get
            {
                return instance;
            }
        }

        public void CreateOrUpdateCustomersOnSheet(IList<Models.CustomerCollection> customers)
        {
            SetupCustomerHeaderRow();

            //https://brandewinder.com/2011/01/23/Excel-In-Cell-DropDown-with-CSharp/
            for (int idxRow = 1; idxRow <= customers.Count; idxRow++)
            {
                ((Range)Worksheet.Cells[idxRow + 1, ExcelAddin.Constants.CustomersSheet.IdColumnIndex]).Value2 = customers[idxRow - 1].Id;
                ((Range)Worksheet.Cells[idxRow + 1, ExcelAddin.Constants.CustomersSheet.IdColumnIndex]).Interior.Color = XlRgbColor.rgbAliceBlue;
                ((Range)Worksheet.Cells[idxRow + 1, ExcelAddin.Constants.CustomersSheet.NameColumnIndex]).Value2 = customers[idxRow - 1].Name;
            }
        }

        public void SetupCustomerHeaderRow()
        {
            ((Range)Worksheet.Cells[1, ExcelAddin.Constants.CustomersSheet.IdColumnIndex]).Value2 = "Id";
            ((Range)Worksheet.Cells[1, ExcelAddin.Constants.CustomersSheet.NameColumnIndex]).Value2 = "Name";
            ((Range)Worksheet.Cells[1, ExcelAddin.Constants.CustomersSheet.NameColumnIndex]).EntireColumn.ColumnWidth = 14;
        }
    }
}