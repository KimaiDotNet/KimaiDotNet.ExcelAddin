using MarkZither.KimaiDotNet.ExcelAddin.Models.Calendar;

using Microsoft.Office.Interop.Excel;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MarkZither.KimaiDotNet.ExcelAddin.Sheets
{
    public sealed class CalendarCategoryWorksheet
    {
        private static readonly CalendarCategoryWorksheet instance = new CalendarCategoryWorksheet();
        public readonly Worksheet Worksheet;

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static CalendarCategoryWorksheet()
        {
        }

        private CalendarCategoryWorksheet()
        {
            Worksheet currentSheet = Globals.ThisAddIn.Application.ActiveSheet as Microsoft.Office.Interop.Excel.Worksheet;

            var calendarCategoryWorksheet =
                Globals.ThisAddIn.Application.Worksheets.Cast<Worksheet>()
                       .SingleOrDefault(w => string.Equals(w.Name, ExcelAddin.Constants.CalendarCategoryWorksheet.CalendarCategorySheetName, StringComparison.OrdinalIgnoreCase));
            if (calendarCategoryWorksheet is Worksheet)
            {
                Worksheet = calendarCategoryWorksheet;
            }
            else
            {
                calendarCategoryWorksheet = (Microsoft.Office.Interop.Excel.Worksheet)Globals.ThisAddIn.Application.Worksheets.Add(Missing.Value, Globals.ThisAddIn.Application.ActiveSheet as Worksheet);
                calendarCategoryWorksheet.Name = ExcelAddin.Constants.CalendarCategoryWorksheet.CalendarCategorySheetName;
#if !DEBUG
                calendarCategoryWorksheet.Visible = XlSheetVisibility.xlSheetVeryHidden;
#endif
                Worksheet = calendarCategoryWorksheet;
                SetupCalendarCategoryHeaderRow();
                // switch straight back to the current active sheet
                currentSheet.Select();
            }
        }

        public static CalendarCategoryWorksheet Instance
        {
            get
            {
                return instance;
            }
        }

        public void CreateOrUpdateCalendarCategoriesOnSheet(Categories categories)
        {
            Worksheet.Change -= new Microsoft.Office.Interop.Excel.
                DocEvents_ChangeEventHandler(changesRange_Change);
            SetupCalendarCategoryHeaderRow();

            for (int idxRow = 1; idxRow <= 5; idxRow++)
            {
                ((Range)Worksheet.Cells[idxRow + 1, ExcelAddin.Constants.CalendarCategoryWorksheet.IdColumnIndex]).Value2 = categories.Category[idxRow - 1].Guid;
                ((Range)Worksheet.Cells[idxRow + 1, ExcelAddin.Constants.CalendarCategoryWorksheet.IdColumnIndex]).Interior.Color = XlRgbColor.rgbAliceBlue;
                ((Range)Worksheet.Cells[idxRow + 1, ExcelAddin.Constants.CalendarCategoryWorksheet.NameColumnIndex]).Value2 = categories.Category[idxRow - 1].Name;
            }
            ((Range)Worksheet.Cells[2, ExcelAddin.Constants.CalendarCategoryWorksheet.CustomerColumnIndex]).AddDataValidationToColumn(Worksheet, ExcelAddin.Constants.CustomersSheet.CustomersSheetName, ExcelAddin.Constants.CustomersSheet.NameColumnIndex, ExcelAddin.Constants.CalendarCategoryWorksheet.CustomerColumnIndex);
            Worksheet.Change += new Microsoft.Office.Interop.Excel.DocEvents_ChangeEventHandler(changesRange_Change);

        }

        private void changesRange_Change(Range Target)
        {
            if (Target.Worksheet.Name.Equals(Constants.CalendarCategoryWorksheet.CalendarCategorySheetName, StringComparison.OrdinalIgnoreCase)
                && Target.Count == 1)
            {
                MessageBox.Show("Doing our change event on categories");
            }
        }

        public void SetupCalendarCategoryHeaderRow()
        {
            ((Range)Worksheet.Cells[1, ExcelAddin.Constants.CalendarCategoryWorksheet.IdColumnIndex]).Value2 = "Id";
            ((Range)Worksheet.Cells[1, ExcelAddin.Constants.CalendarCategoryWorksheet.NameColumnIndex]).Value2 = "Name";
            ((Range)Worksheet.Cells[1, ExcelAddin.Constants.CalendarCategoryWorksheet.NameColumnIndex]).EntireColumn.ColumnWidth = 14;
            ((Range)Worksheet.Cells[1, ExcelAddin.Constants.CalendarCategoryWorksheet.CustomerColumnIndex]).Value2 = "Customer";
            ((Range)Worksheet.Cells[1, ExcelAddin.Constants.CalendarCategoryWorksheet.ProjectColumnIndex]).Value2 = "Project";
            ((Range)Worksheet.Cells[1, ExcelAddin.Constants.CalendarCategoryWorksheet.ActivityColumnIndex]).Value2 = "Activity";
        }
    }
}