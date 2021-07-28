using Microsoft.Office.Interop.Excel;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MarkZither.KimaiDotNet.ExcelAddin.Sheets
{
    public sealed class ProjectWorksheet
    {
        private static readonly ProjectWorksheet instance = new ProjectWorksheet();
        public readonly Worksheet Worksheet;

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static ProjectWorksheet()
        {
        }

        private ProjectWorksheet()
        {
            Worksheet currentSheet = Globals.ThisAddIn.Application.ActiveSheet as Microsoft.Office.Interop.Excel.Worksheet;

            var projectWorksheet =
                Globals.ThisAddIn.Application.Worksheets.Cast<Worksheet>()
                       .SingleOrDefault(w => string.Equals(w.Name, ExcelAddin.Constants.ProjectsSheet.ProjectsSheetName, StringComparison.OrdinalIgnoreCase));
            if (projectWorksheet is Worksheet)
            {
                Worksheet = projectWorksheet;
            }
            else
            {
                projectWorksheet = (Microsoft.Office.Interop.Excel.Worksheet)Globals.ThisAddIn.Application.Worksheets.Add(Missing.Value, Globals.ThisAddIn.Application.ActiveSheet as Worksheet);
                projectWorksheet.Name = ExcelAddin.Constants.ProjectsSheet.ProjectsSheetName;
                projectWorksheet.Visible = XlSheetVisibility.xlSheetVeryHidden;
                Worksheet = projectWorksheet;
                // switch straight back to the current active sheet
                currentSheet.Select();
            }
        }

        public static ProjectWorksheet Instance
        {
            get
            {
                return instance;
            }
        }

        public void CreateOrUpdateProjectsOnSheet(IList<KimaiDotNet.Models.ProjectCollection> projects)
        {
            SetupProjectHeaderRow();

            //https://brandewinder.com/2011/01/23/Excel-In-Cell-DropDown-with-CSharp/
            for (int idxRow = 1; idxRow <= projects.Count; idxRow++)
            {
                ((Range)Worksheet.Cells[idxRow + 1, ExcelAddin.Constants.ProjectsSheet.IdColumnIndex]).Value2 = projects[idxRow - 1].Id;
                ((Range)Worksheet.Cells[idxRow + 1, ExcelAddin.Constants.ProjectsSheet.IdColumnIndex]).Interior.Color = XlRgbColor.rgbAliceBlue;
                ((Range)Worksheet.Cells[idxRow + 1, ExcelAddin.Constants.ProjectsSheet.NameColumnIndex]).Value2 = projects[idxRow - 1].Name;
                if (projects[idxRow - 1].Customer.HasValue)
                {
                    ((Range)Worksheet.Cells[idxRow + 1, ExcelAddin.Constants.ProjectsSheet.CustomerNameColumnIndex]).Value2 = projects[idxRow - 1].ParentTitle;
                    ((Range)Worksheet.Cells[idxRow + 1, ExcelAddin.Constants.ProjectsSheet.CustomerIdColumnIndex]).Value2 = projects[idxRow - 1].Customer.Value;
                }
            }
        }

        public void SetupProjectHeaderRow()
        {
            ((Range)Worksheet.Cells[1, ExcelAddin.Constants.ProjectsSheet.IdColumnIndex]).Value2 = "Id";
            ((Range)Worksheet.Cells[1, ExcelAddin.Constants.ProjectsSheet.NameColumnIndex]).Value2 = "Name";
            ((Range)Worksheet.Cells[1, ExcelAddin.Constants.ProjectsSheet.NameColumnIndex]).EntireColumn.ColumnWidth = 14;
            ((Range)Worksheet.Cells[1, ExcelAddin.Constants.ProjectsSheet.CustomerNameColumnIndex]).Value2 = "Parent Title";
            ((Range)Worksheet.Cells[1, ExcelAddin.Constants.ProjectsSheet.CustomerNameColumnIndex]).EntireColumn.ColumnWidth = 25;
            ((Range)Worksheet.Cells[1, ExcelAddin.Constants.ProjectsSheet.CustomerIdColumnIndex]).Value2 = "Customer Id";
        }
    }
}