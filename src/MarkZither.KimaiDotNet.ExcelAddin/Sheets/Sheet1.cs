using MarkZither.KimaiDotNet.ExcelAddin.Services;
using MarkZither.KimaiDotNet.Models;

using Microsoft.Extensions.Logging;
using Microsoft.Office.Interop.Excel;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MarkZither.KimaiDotNet.ExcelAddin.Sheets
{
    public sealed class Sheet1
    {
        private static readonly Sheet1 instance = new Sheet1();
        public readonly Worksheet Worksheet;

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static Sheet1()
        {
        }

        private Sheet1()
        {
            var sheet1Worksheet =
                Globals.ThisAddIn.Application.Worksheets.Cast<Worksheet>()
                       .SingleOrDefault(w => string.Equals(w.Name, ExcelAddin.Constants.Sheet1.TimesheetsSheetName, StringComparison.OrdinalIgnoreCase));
            if (sheet1Worksheet is Worksheet)
            {
                Worksheet = sheet1Worksheet;
            }
            else
            {
                sheet1Worksheet = (Microsoft.Office.Interop.Excel.Worksheet)Globals.ThisAddIn.Application.Worksheets.Add(Missing.Value, Globals.ThisAddIn.Application.ActiveSheet as Worksheet);
                sheet1Worksheet.Name = ExcelAddin.Constants.Sheet1.TimesheetsSheetName;
                Worksheet = sheet1Worksheet;
            }
        }

        public static Sheet1 Instance
        {
            get
            {
                return instance;
            }
        }

        public void SetupTimesheetsHeaderRow()
        {
            ((Range)Worksheet.Cells[1, ExcelAddin.Constants.Sheet1.IdColumnIndex]).Value2 = "Id";
            ((Range)Worksheet.Cells[1, ExcelAddin.Constants.Sheet1.DateColumnIndex]).Value2 = "Date";
            ((Range)Worksheet.Cells[1, ExcelAddin.Constants.Sheet1.DateColumnIndex]).EntireColumn.ColumnWidth = 14;
            ((Range)Worksheet.Cells[1, ExcelAddin.Constants.Sheet1.DurationColumnIndex]).Value2 = "Duration (mins)";
            ((Range)Worksheet.Cells[1, ExcelAddin.Constants.Sheet1.DurationColumnIndex]).EntireColumn.ColumnWidth = 14;
            ((Range)Worksheet.Cells[1, ExcelAddin.Constants.Sheet1.CustomerColumnIndex]).Value2 = "Customer";
            ((Range)Worksheet.Cells[1, ExcelAddin.Constants.Sheet1.CustomerColumnIndex]).EntireColumn.ColumnWidth = 25;
            ((Range)Worksheet.Cells[1, ExcelAddin.Constants.Sheet1.ProjectColumnIndex]).Value2 = "Project";
            ((Range)Worksheet.Cells[1, ExcelAddin.Constants.Sheet1.ProjectColumnIndex]).EntireColumn.ColumnWidth = 30;
            ((Range)Worksheet.Cells[1, ExcelAddin.Constants.Sheet1.ActivityColumnIndex]).Value2 = "Activity";
            ((Range)Worksheet.Cells[1, ExcelAddin.Constants.Sheet1.ActivityColumnIndex]).EntireColumn.ColumnWidth = 30;
            ((Range)Worksheet.Cells[1, ExcelAddin.Constants.Sheet1.DescColumnIndex]).Value2 = "Description";
            ((Range)Worksheet.Cells[1, ExcelAddin.Constants.Sheet1.DescColumnIndex]).EntireColumn.ColumnWidth = 80;
            ((Range)Worksheet.Cells[1, ExcelAddin.Constants.Sheet1.BeginTimeIndex]).Value2 = "Begin Time";
            ((Range)Worksheet.Cells[1, ExcelAddin.Constants.Sheet1.BeginTimeIndex]).EntireColumn.ColumnWidth = 25;
            ((Range)Worksheet.Cells[1, ExcelAddin.Constants.Sheet1.EndTimeIndex]).Value2 = "End Time";
            ((Range)Worksheet.Cells[1, ExcelAddin.Constants.Sheet1.EndTimeIndex]).EntireColumn.ColumnWidth = 25;

            // https://stackoverflow.com/questions/3310800/how-to-make-correct-date-format-when-writing-data-to-excel
            var cell = Worksheet.Range[Worksheet.Cells[2, ExcelAddin.Constants.Sheet1.DateColumnIndex], Worksheet.Cells[10000, ExcelAddin.Constants.Sheet1.DateColumnIndex]];
            cell.NumberFormat = "dd-MMM-yyyy"; // e.g. dd-MMM-yyyy
            var cellBegin = Worksheet.Range[Worksheet.Cells[2, ExcelAddin.Constants.Sheet1.BeginTimeIndex], Worksheet.Cells[10000, ExcelAddin.Constants.Sheet1.BeginTimeIndex]];
            cellBegin.NumberFormat = "hh:mm:ss"; // e.g. dd-MMM-yyyy
            var cellEnd = Worksheet.Range[Worksheet.Cells[2, ExcelAddin.Constants.Sheet1.EndTimeIndex], Worksheet.Cells[10000, ExcelAddin.Constants.Sheet1.EndTimeIndex]];
            cellEnd.NumberFormat = "hh:mm:ss"; // e.g. dd-MMM-yyyy
        }

        public void WriteTimesheetRows(IList<Models.TimesheetCollection> timesheets)
        {
            for (int idxRow = 1; idxRow <= timesheets.Count; idxRow++)
            {
                ((Range)Worksheet.Cells[idxRow + 1, ExcelAddin.Constants.Sheet1.IdColumnIndex]).Value2 = timesheets[idxRow - 1].Id;
                ((Range)Worksheet.Cells[idxRow + 1, ExcelAddin.Constants.Sheet1.IdColumnIndex]).Interior.Color = XlRgbColor.rgbAliceBlue;
                ((Range)Worksheet.Cells[idxRow + 1, ExcelAddin.Constants.Sheet1.DateColumnIndex]).Value2 = timesheets[idxRow - 1].Begin.Date.ToOADate();
                ((Range)Worksheet.Cells[idxRow + 1, ExcelAddin.Constants.Sheet1.DurationColumnIndex]).Value2 = timesheets[idxRow - 1].Duration / 60;
                if (timesheets[idxRow - 1].Project.HasValue)
                {
                    var project = Globals.ThisAddIn.GetProjectById(timesheets[idxRow - 1].Project.Value);
                    ((Range)Worksheet.Cells[idxRow + 1, ExcelAddin.Constants.Sheet1.CustomerColumnIndex]).Value2 = project.ParentTitle;
                    ((Range)Worksheet.Cells[idxRow + 1, ExcelAddin.Constants.Sheet1.ProjectColumnIndex]).Value2 = project.Name;
                }
                if (timesheets[idxRow - 1].Activity.HasValue)
                {
                    var activity = Globals.ThisAddIn.GetActivityById(timesheets[idxRow - 1].Activity.Value);
                    ((Range)Worksheet.Cells[idxRow + 1, ExcelAddin.Constants.Sheet1.ActivityColumnIndex]).Value2 = activity.Name;
                }
                            ((Range)Worksheet.Cells[idxRow + 1, ExcelAddin.Constants.Sheet1.DescColumnIndex]).Value2 = timesheets[idxRow - 1].Description;
                ((Range)Worksheet.Cells[idxRow + 1, ExcelAddin.Constants.Sheet1.BeginTimeIndex]).Value2 = timesheets[idxRow - 1].Begin.ToOADate();
                if (timesheets[idxRow - 1].End.HasValue)
                {
                    ((Range)Worksheet.Cells[idxRow + 1, ExcelAddin.Constants.Sheet1.EndTimeIndex]).Value2 = timesheets[idxRow - 1].End.Value.ToOADate();
                }
            }
        }
        public async Task SyncNewRowsToKimai(IKimaiServices service, IList<Models.TimesheetCollection> timesheets)
        {
            try
            {
                //find a row with no id to post
                for (int i = timesheets.Count; i < 10000; i++)
                {
                    dynamic id = ((Range)Worksheet.Cells[i, ExcelAddin.Constants.Sheet1.IdColumnIndex]).Value2;
                    dynamic oADate = ((Range)Worksheet.Cells[i, ExcelAddin.Constants.Sheet1.DateColumnIndex]).Value2;
                    if (id is null && (oADate is int || oADate is double))
                    {
                        int duration = (int)((Range)Worksheet.Cells[i, ExcelAddin.Constants.Sheet1.DurationColumnIndex]).Value2;
                        string customerName = (string)((Range)Worksheet.Cells[i, ExcelAddin.Constants.Sheet1.CustomerColumnIndex]).Value2;
                        int customerId = Globals.ThisAddIn.GetCustomerByName(customerName).Id.Value;
                        string projectName = (string)((Range)Worksheet.Cells[i, ExcelAddin.Constants.Sheet1.ProjectColumnIndex]).Value2;
                        int projectId = Globals.ThisAddIn.GetProjectByName(projectName, customerId).Id.Value;
                        string activityName = (string)((Range)Worksheet.Cells[i, ExcelAddin.Constants.Sheet1.ActivityColumnIndex]).Value2;
                        int activityId = Globals.ThisAddIn.GetActivityByName(activityName, projectId).Id.Value;
                        string description = (string)((Range)Worksheet.Cells[i, ExcelAddin.Constants.Sheet1.DescColumnIndex]).Value2;

                        if (oADate is Double)//this is really probably an OADate 
                        // https://docs.microsoft.com/en-us/dotnet/api/system.datetime.tooadate?view=net-5.0
                        {
                            DateTime date = DateTime.FromOADate(oADate).Date; //make sure any time is ignored, it shouldn't be there
                            int addMinutes = GetNextAvailableTimeInMinutes(date);

                            var timesheet = await service.PostTimesheet(new Models.TimesheetEditForm()
                            {
                                Project = projectId,
                                Activity = activityId,
                                Begin = date.AddMinutes(addMinutes).ToUniversalTime(),
                                End = date.AddMinutes(addMinutes).AddMinutes(duration).ToUniversalTime(),
                                User = Globals.ThisAddIn.CurrentUser.Id.Value,
                                Description = description,
                            }).ConfigureAwait(false);
                            Globals.ThisAddIn.Timesheets.Add(new Models.TimesheetCollection() { Id = timesheet.Id, Begin = timesheet.Begin, End = timesheet.End, Activity = timesheet.Activity, Project = timesheet.Project, Duration = duration, User = timesheet.User, Description = timesheet.Description, Tags = timesheet.Tags, });
                            UpdateNewTimesheetRecordAfterServerSync(i, timesheet);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                ExcelAddin.Globals.ThisAddIn.Logger.LogWarning("Failed to sync new rows to kimai", ex);
            }
        }

        private void UpdateNewTimesheetRecordAfterServerSync(int rowNo, TimesheetEntity timesheet)
        {
            // https://social.msdn.microsoft.com/Forums/vstudio/en-US/f89fe6b3-68c0-4a98-9522-953cc5befb34/how-to-make-a-excel-cell-readonly-by-c-code?forum=vsto
            Worksheet.Unprotect();
            Globals.ThisAddIn.Application.Cells.Locked = false;
            Worksheet.Change -= new Microsoft.Office.Interop.Excel.
            DocEvents_ChangeEventHandler(changesRange_Change);

            ((Range)Worksheet.Cells[rowNo, ExcelAddin.Constants.Sheet1.IdColumnIndex]).Value2 = timesheet.Id;

#pragma warning disable S125 // Sections of code should not be commented out
            //((Excel.Range)sheet.Cells[timesheets.Count + 2, IdColumnIndex]).Interior.Color = Excel.XlRgbColor.rgbAliceBlue;
#pragma warning restore S125 // Sections of code should not be commented out
            ((Range)Worksheet.Cells[rowNo, ExcelAddin.Constants.Sheet1.DateColumnIndex]).Value2 = timesheet.Begin.Date.ToOADate();
            if (timesheet.Project.HasValue)
            {
                var project = Globals.ThisAddIn.GetProjectById(timesheet.Project.Value);
                ((Range)Worksheet.Cells[rowNo, ExcelAddin.Constants.Sheet1.CustomerColumnIndex]).Value2 = project.ParentTitle;
                ((Range)Worksheet.Cells[rowNo, ExcelAddin.Constants.Sheet1.ProjectColumnIndex]).Value2 = project.Name;
            }
            if (timesheet.Activity.HasValue)
            {
                var activity = Globals.ThisAddIn.GetActivityById(timesheet.Activity.Value);
                ((Range)Worksheet.Cells[rowNo, ExcelAddin.Constants.Sheet1.ActivityColumnIndex]).Value2 = activity.Name;
            }
                                    ((Range)Worksheet.Cells[rowNo, ExcelAddin.Constants.Sheet1.DescColumnIndex]).Value2 = timesheet.Description;
            ((Range)Worksheet.Cells[rowNo, ExcelAddin.Constants.Sheet1.BeginTimeIndex]).Value2 = timesheet.Begin.ToOADate();
            if (timesheet.End.HasValue)
            {
                ((Range)Worksheet.Cells[rowNo, ExcelAddin.Constants.Sheet1.EndTimeIndex]).Value2 = timesheet.End.Value.ToOADate();
            }

            Worksheet.Change += new Microsoft.Office.Interop.Excel.
            DocEvents_ChangeEventHandler(changesRange_Change);

            // https://social.msdn.microsoft.com/Forums/vstudio/en-US/f89fe6b3-68c0-4a98-9522-953cc5befb34/how-to-make-a-excel-cell-readonly-by-c-code?forum=vsto
            Globals.ThisAddIn.Application.Cells.Locked = false;
            Globals.ThisAddIn.Application.get_Range("A1", $"A{rowNo}").Locked = true;
            Worksheet.Protect(Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
              Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
        }

        private int GetNextAvailableTimeInMinutes(DateTime date)
        {
            var lastTimeEntry = Globals.ThisAddIn.Timesheets.OrderByDescending(x => x.End).FirstOrDefault(x => x.Begin.Date.Equals(date.Date) && x.End != null)?.End.Value;
            if (lastTimeEntry is null)
            { return 8 * 60; }
            return lastTimeEntry.Value.Hour * 60 + lastTimeEntry.Value.Minute;
        }

        void changesRange_Change(Range Target)
        {
            if (Target.Worksheet.Name.Equals(Constants.Sheet1.TimesheetsSheetName, StringComparison.OrdinalIgnoreCase)
                && Target.Count == 1)
            {
                var column = Target.Column;
                string selectedValue = Target.Value2 as string;

                if (column.Equals(ExcelAddin.Constants.Sheet1.CustomerColumnIndex))
                {
                    try
                    {
                        CustomerCollection customer = Globals.ThisAddIn.Customers.Single(x => string.Equals(x.Name, selectedValue, StringComparison.OrdinalIgnoreCase));
                        List<ProjectCollection> projects = Globals.ThisAddIn.Projects.Where(x => x.Customer == customer.Id || !x.Customer.HasValue).ToList();
                        var projectFlatList = string.Join(ExcelAddin.Constants.FlatListDelimiter, projects.Select(i => i.Name));
                        AddDataValidationToColumnWithFlatList(projectFlatList, ExcelAddin.Constants.Sheet1.ProjectColumnIndex, Target.Row);
                        ExcelAddin.Globals.ThisAddIn.Logger.LogDebug("Customer changed, lets set the valid projects");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Could not find the selected customer. {ex.Message}");
                        ExcelAddin.Globals.ThisAddIn.Logger.LogWarning($"Could not find the selected customer. {ex.Message}", ex);
                    }
                }
                if (column.Equals(ExcelAddin.Constants.Sheet1.ProjectColumnIndex))
                {
                    try
                    {
                        ProjectCollection project = Globals.ThisAddIn.Projects.Single(x => string.Equals(x.Name, selectedValue, StringComparison.OrdinalIgnoreCase));
                        List<ActivityCollection> activities = Globals.ThisAddIn.Activities.Where(x => x.Project == project.Id || !x.Project.HasValue).ToList();
                        var activitiesFlatList = string.Join(ExcelAddin.Constants.FlatListDelimiter, activities.Select(i => i.Name));
                        AddDataValidationToColumnWithFlatList(activitiesFlatList, ExcelAddin.Constants.Sheet1.ActivityColumnIndex, Target.Row);
                        ExcelAddin.Globals.ThisAddIn.Logger.LogDebug("Project changed, lets set the valid activities");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Could not find the selected project. {ex.Message}");
                        ExcelAddin.Globals.ThisAddIn.Logger.LogWarning($"Could not find the selected project. {ex.Message}", ex);
                    }
                }
            }
        }
        public async Task SyncTimesheetsAsync()
        {
            var mockWorksheet = Globals.ThisAddIn.Application.Worksheets.Cast<Worksheet>()
                                   .SingleOrDefault(w => string.Equals(w.Name, "Mock", StringComparison.OrdinalIgnoreCase));
            IKimaiServices services = !(string.Equals(ConfigurationManager.AppSettings["UseMocks"], "true", StringComparison.OrdinalIgnoreCase) || mockWorksheet is Worksheet) ? (IKimaiServices)new KimaiServices() : new MockKimaiServices();

            var projects = await services.GetProjects().ConfigureAwait(false);
            Globals.ThisAddIn.Projects = projects.ToList();
            Sheets.ProjectWorksheet.Instance.CreateOrUpdateProjectsOnSheet(projects);

            var customers = await services.GetCustomers().ConfigureAwait(false);
            Globals.ThisAddIn.Customers = customers.ToList();
            Sheets.CustomerWorksheet.Instance.CreateOrUpdateCustomersOnSheet(customers);

            var activities = await services.GetActivities().ConfigureAwait(false);
            Globals.ThisAddIn.Activities = activities.ToList();
            Sheets.ActivityWorksheet.Instance.CreateOrUpdateActivitiesOnSheet(activities);

            // https://docs.microsoft.com/en-us/visualstudio/vsto/how-to-programmatically-display-a-string-in-a-worksheet-cell?view=vs-2019
            // https://stackoverflow.com/questions/856196/vsto-write-to-a-cell-in-excel
            // https://docs.microsoft.com/en-us/previous-versions/office/troubleshoot/office-developer/automate-excel-using-visual-c-fill-data
            Worksheet sheet = Globals.ThisAddIn.Application.ActiveSheet as Microsoft.Office.Interop.Excel.Worksheet;
            if (Globals.ThisAddIn.Timesheets != null)
            {
                await Sheets.Sheet1.Instance.SyncNewRowsToKimai(services, Globals.ThisAddIn.Timesheets).ConfigureAwait(false);
            }
            var timesheets = await services.GetTimesheets().ConfigureAwait(false);
            Globals.ThisAddIn.Timesheets = timesheets.ToList();

            // https://social.msdn.microsoft.com/Forums/vstudio/en-US/f89fe6b3-68c0-4a98-9522-953cc5befb34/how-to-make-a-excel-cell-readonly-by-c-code?forum=vsto
            sheet.Unprotect();
            Globals.ThisAddIn.Application.Cells.Locked = false;

            sheet.Change -= new Microsoft.Office.Interop.Excel.
                DocEvents_ChangeEventHandler(changesRange_Change);

            Sheets.Sheet1.Instance.SetupTimesheetsHeaderRow();
            Sheets.Sheet1.Instance.WriteTimesheetRows(timesheets);

            //https://stackoverflow.com/questions/2414591/how-to-create-validation-from-name-range-on-another-worksheet-in-excel-using-c
            //https://docs.microsoft.com/en-us/visualstudio/vsto/how-to-add-namedrange-controls-to-worksheets?view=vs-2019
            //https://stackoverflow.com/questions/10373561/convert-a-number-to-a-letter-in-c-sharp-for-use-in-microsoft-excel
            //https://stackoverflow.com/a/10373827
            AddDataValidationToColumnByRange(ExcelAddin.Constants.CustomersSheet.CustomersSheetName, ExcelAddin.Constants.CustomersSheet.NameColumnIndex, ExcelAddin.Constants.Sheet1.CustomerColumnIndex);

            sheet.Change += new Microsoft.Office.Interop.Excel.DocEvents_ChangeEventHandler(changesRange_Change);

            var activitiesFlatList = string.Join(ExcelAddin.Constants.FlatListDelimiter, activities.Select(i => i.Name));
            AddDataValidationToColumnWithFlatList(activitiesFlatList, ExcelAddin.Constants.Sheet1.ActivityColumnIndex);
            sheet.Unprotect();
            // https://social.msdn.microsoft.com/Forums/vstudio/en-US/f89fe6b3-68c0-4a98-9522-953cc5befb34/how-to-make-a-excel-cell-readonly-by-c-code?forum=vsto
            if (Globals.ThisAddIn.Application.Cells.Locked is bool && (bool)Globals.ThisAddIn.Application.Cells.Locked)
            {
                Globals.ThisAddIn.Application.Cells.Locked = false;
            }
            Globals.ThisAddIn.Application.get_Range("A1", $"I{timesheets.Count + 1}").Locked = true;
            sheet.Protect(Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
              Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
        }
        private void AddDataValidationToColumnByRange(string validationListSheetName, int validationListColumnIndex, int validatedColumnIndex)
        {
            string forumula = $"='{validationListSheetName}'!{(validationListColumnIndex - 1).ColumnLetterFromIndex()}:{(validationListColumnIndex - 1).ColumnLetterFromIndex()}";
            Range cell = Worksheet.Range[$"{(validatedColumnIndex - 1).ColumnLetterFromIndex()}2:{(validatedColumnIndex - 1).ColumnLetterFromIndex()}10000"];
            cell.Validation.Delete();
            cell.Validation.Add(XlDVType.xlValidateList, XlDVAlertStyle.xlValidAlertInformation, XlFormatConditionOperator.xlEqual, forumula, Type.Missing);
            cell.Validation.IgnoreBlank = true;
            cell.Validation.InCellDropdown = true;
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
    }
}
