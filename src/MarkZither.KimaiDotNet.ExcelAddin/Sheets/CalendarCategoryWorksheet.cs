using MarkZither.KimaiDotNet.ExcelAddin.Models.Calendar;
using MarkZither.KimaiDotNet.Models;

using Microsoft.Extensions.Logging;
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

            for (int idxRow = 1; idxRow <= categories.Category.Count; idxRow++)
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
                var column = Target.Column;
                string selectedValue = Target.Value2 as string;

                if (column.Equals(ExcelAddin.Constants.CalendarCategoryWorksheet.CustomerColumnIndex))
                {
                    try
                    {
                        CustomerCollection customer = Globals.ThisAddIn.Customers.Single(x => string.Equals(x.Name, selectedValue, StringComparison.OrdinalIgnoreCase));
                        List<ProjectCollection> projects = Globals.ThisAddIn.Projects.Where(x => x.Customer == customer.Id || !x.Customer.HasValue).ToList();
                        var projectFlatList = string.Join(ExcelAddin.Constants.FlatListDelimiter, projects.Select(i => i.Name));
                        AddDataValidationToColumnWithFlatList(projectFlatList, ExcelAddin.Constants.CalendarCategoryWorksheet.ProjectColumnIndex, Target.Row);
                        ExcelAddin.Globals.ThisAddIn.Logger.LogDebug("Customer changed, lets set the valid projects");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Could not find the selected customer. {ex.Message}");
                        ExcelAddin.Globals.ThisAddIn.Logger.LogWarning($"Could not find the selected customer. {ex.Message}", ex);
                    }
                }
                if (column.Equals(ExcelAddin.Constants.CalendarCategoryWorksheet.ProjectColumnIndex))
                {
                    try
                    {
                        string customerName = (string)((Range)Worksheet.Cells[Target.Row, ExcelAddin.Constants.CalendarCategoryWorksheet.CustomerColumnIndex]).Value2;
                        CustomerCollection customer = Globals.ThisAddIn.GetCustomerByName(customerName);
                        ProjectCollection project = Globals.ThisAddIn.GetProjectByName(selectedValue, customer.Id);
#pragma warning disable S1135 // Track uses of "TODO" tags
#pragma warning disable MA0026 // Fix TODO comment
                        // TODO: replace this with a call to a method in ThisAddIn
                        List<ActivityCollection> activities = Globals.ThisAddIn.Activities.Where(x => x.Project == project.Id || !x.Project.HasValue).ToList();
#pragma warning restore MA0026 // Fix TODO comment
#pragma warning restore S1135 // Track uses of "TODO" tags
                        var activitiesFlatList = string.Join(ExcelAddin.Constants.FlatListDelimiter, activities.Select(i => i.Name));
                        AddDataValidationToColumnWithFlatList(activitiesFlatList, ExcelAddin.Constants.CalendarCategoryWorksheet.ActivityColumnIndex, Target.Row);
                        ExcelAddin.Globals.ThisAddIn.Logger.LogDebug("Project changed, lets set the valid activities");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Could not find the selected project. {ex.Message}");
                        ExcelAddin.Globals.ThisAddIn.Logger.LogWarning($"Could not find the selected project. {ex.Message}", ex);
                    }
                }
                if (column.Equals(ExcelAddin.Constants.CalendarCategoryWorksheet.ActivityColumnIndex))
                {
                    // now we have a full customer, project and activity update the category mapping
                    var id = (string)((Range)Worksheet.Cells[Target.Row, ExcelAddin.Constants.CalendarCategoryWorksheet.IdColumnIndex]).Value2;
                    var cust = (string)((Range)Worksheet.Cells[Target.Row, ExcelAddin.Constants.CalendarCategoryWorksheet.CustomerColumnIndex]).Value2;
                    var custId = Globals.ThisAddIn.GetCustomerByName(cust); 
                    var projName = (string)((Range)Worksheet.Cells[Target.Row, ExcelAddin.Constants.CalendarCategoryWorksheet.ProjectColumnIndex]).Value2;
                    var project = Globals.ThisAddIn.GetProjectByName(projName, custId.Id);
                    var cat = Globals.ThisAddIn.Categories.Single(x => string.Equals(x.Guid, id, StringComparison.OrdinalIgnoreCase));
                    cat.ActivityId = Globals.ThisAddIn.GetActivityByName((string)Target.Value2, project.Id).Id.Value;
                    cat.ProjectId = project.Id.Value;
                    cat.CustomerId = custId.Id.Value;
                }
            }
        }
        private void AddDataValidationToColumnWithFlatList(string flatList, int columnIndex, int? row = null)
        {
            // might not use this method as there is a limit to the number of items that can be supported
            // however the other way has the limitaiton of needing to have a single range
            // if for example the a project has no customer there might be 2 ranges required
            // at which point we will have cell level validation rather than the whole column
            // and probably namy fewer items in the list
            // https://brandewinder.com/2011/01/23/Excel-In-Cell-DropDown-with-CSharp/
            // https://stackoverflow.com/questions/2333202/how-do-i-get-an-excel-range-using-row-and-column-numbers-in-vsto-c
            Range range;
            if (!row.HasValue)
            {
                range = (Range)Worksheet.Range[Worksheet.Cells[1, columnIndex], Worksheet.Cells[10000, columnIndex]];
            }
            else
            {
                range = (Range)Worksheet.Cells[row.Value, columnIndex];
            }

            try
            {
                // https://social.msdn.microsoft.com/Forums/vstudio/en-US/f89fe6b3-68c0-4a98-9522-953cc5befb34/how-to-make-a-excel-cell-readonly-by-c-code?forum=vsto
                Worksheet.Unprotect();
                Globals.ThisAddIn.Application.Cells.Locked = false;

                range.Validation.Delete();
                range.Validation.Add(
                   XlDVType.xlValidateList,
                   XlDVAlertStyle.xlValidAlertInformation,
                   XlFormatConditionOperator.xlBetween,
                   flatList,
                   Type.Missing);

                range.Validation.IgnoreBlank = true;
                range.Validation.InCellDropdown = true;

                range.Locked = false;
                Worksheet.Protect(Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                  Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to set validation on column index ${columnIndex}");
                ExcelAddin.Globals.ThisAddIn.Logger.LogWarning("Failed to set validation on column index ${columnIndex}", ex);
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