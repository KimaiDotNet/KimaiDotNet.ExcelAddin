using Microsoft.Office.Interop.Excel;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MarkZither.KimaiDotNet.ExcelAddin.Sheets
{
    public sealed class ActivityWorksheet
    {
        private static readonly ActivityWorksheet instance = new ActivityWorksheet();
        public readonly Worksheet Worksheet;

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static ActivityWorksheet()
        {
        }

        private ActivityWorksheet()
        {
            Worksheet currentSheet = Globals.ThisAddIn.Application.ActiveSheet as Microsoft.Office.Interop.Excel.Worksheet;

            var activityWorksheet =
                Globals.ThisAddIn.Application.Worksheets.Cast<Worksheet>()
                       .SingleOrDefault(w => string.Equals(w.Name, ExcelAddin.Constants.ActivitiesSheet.ActivitiesSheetName, StringComparison.OrdinalIgnoreCase));
            if (activityWorksheet is Worksheet)
            {
                Worksheet = activityWorksheet;
            }
            else
            {
                activityWorksheet = (Microsoft.Office.Interop.Excel.Worksheet)Globals.ThisAddIn.Application.Worksheets.Add(Missing.Value, Globals.ThisAddIn.Application.ActiveSheet as Worksheet);
                activityWorksheet.Name = ExcelAddin.Constants.ActivitiesSheet.ActivitiesSheetName;
                activityWorksheet.Visible = XlSheetVisibility.xlSheetVeryHidden;
                Worksheet = activityWorksheet;
                // switch straight back to the current active sheet
                currentSheet.Select();
            }
        }

        public static ActivityWorksheet Instance
        {
            get
            {
                return instance;
            }
        }

        public void CreateOrUpdateActivitiesOnSheet(IList<Models.ActivityCollection> activities)
        {
            SetupActivityHeaderRow();

            for (int idxRow = 1; idxRow <= activities.Count; idxRow++)
            {
                ((Range)Worksheet.Cells[idxRow + 1, ExcelAddin.Constants.ActivitiesSheet.IdColumnIndex]).Value2 = activities[idxRow - 1].Id;
                ((Range)Worksheet.Cells[idxRow + 1, ExcelAddin.Constants.ActivitiesSheet.IdColumnIndex]).Interior.Color = XlRgbColor.rgbAliceBlue;
                ((Range)Worksheet.Cells[idxRow + 1, ExcelAddin.Constants.ActivitiesSheet.NameColumnIndex]).Value2 = activities[idxRow - 1].Name;
                if (activities[idxRow - 1].Project.HasValue)
                {
                    ((Range)Worksheet.Cells[idxRow + 1, ExcelAddin.Constants.ActivitiesSheet.ProjectNameColumnIndex]).Value2 = activities[idxRow - 1].ParentTitle;
                    ((Range)Worksheet.Cells[idxRow + 1, ExcelAddin.Constants.ActivitiesSheet.ProjectIdColumnIndex]).Value2 = activities[idxRow - 1].Project.Value;
                }
            }
        }

        public void SetupActivityHeaderRow()
        {
            ((Range)Worksheet.Cells[1, ExcelAddin.Constants.ActivitiesSheet.IdColumnIndex]).Value2 = "Id";
            ((Range)Worksheet.Cells[1, ExcelAddin.Constants.ActivitiesSheet.NameColumnIndex]).Value2 = "Name";
            ((Range)Worksheet.Cells[1, ExcelAddin.Constants.ActivitiesSheet.NameColumnIndex]).EntireColumn.ColumnWidth = 14;
        }
    }
}