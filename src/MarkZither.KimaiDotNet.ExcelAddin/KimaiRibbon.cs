using MarkZither.KimaiDotNet.ExcelAddin.Services;

using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Tools.Ribbon;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Excel = Microsoft.Office.Interop.Excel;

namespace MarkZither.KimaiDotNet.ExcelAddin
{
    public partial class KimaiRibbon
    {
        private const int IdColumnIndex = 1;
        private const int DateColumnIndex = 2;
        private const int CustomerColumnIndex = 3;
        private const int ProjectColumnIndex = 4;
        private const int ActivityColumnIndex = 5;
        private const int DescColumnIndex = 6;
        private const int BeginTimeIndex = 7;
        private const int EndTimeIndex = 8;

        void changesRange_Change(Excel.Range Target)
        {
            string cellAddress = Target.get_Address(
                Microsoft.Office.Interop.Excel.XlReferenceStyle.xlA1);
        }
        private void KimaiRibbon_Load(object sender, RibbonUIEventArgs e)
        {

        }

        private void tglApiCreds_Click(object sender, RibbonControlEventArgs e)
        {
            Globals.ThisAddIn.TaskPane.Visible = ((RibbonToggleButton)sender).Checked;
        }

        private async void btnConnect_Click(object sender, RibbonControlEventArgs e)
        {
            IKimaiServices services;
            if (string.Equals(ConfigurationManager.AppSettings["UseMocks"], "true", StringComparison.OrdinalIgnoreCase))
            {
                services = new MockKimaiServices();
            }
            else
            {
                services = new KimaiServices();
            }
            var version = await services.GetVersion().ConfigureAwait(false);

            lblVersionNo.Label = version.VersionProperty;
        }

        private async void btnSync_Click(object sender, RibbonControlEventArgs e)
        {
            IKimaiServices services;
            if (string.Equals(ConfigurationManager.AppSettings["UseMocks"], "true", StringComparison.OrdinalIgnoreCase)) {
                services = new MockKimaiServices();
            }
            else {
                services = new KimaiServices();
            }

            var projects = await services.GetProjects().ConfigureAwait(false);
            Globals.ThisAddIn.Projects = projects.ToList();

            var customers = await services.GetCustomers().ConfigureAwait(false);
            Globals.ThisAddIn.Customers = customers.ToList();

            var activities = await services.GetActivities().ConfigureAwait(false);
            Globals.ThisAddIn.Activities = activities.ToList();

            var timesheets = await services.GetTimesheets().ConfigureAwait(false);
            Globals.ThisAddIn.Timesheets = timesheets.ToList();
            // https://docs.microsoft.com/en-us/visualstudio/vsto/how-to-programmatically-display-a-string-in-a-worksheet-cell?view=vs-2019
            // https://stackoverflow.com/questions/856196/vsto-write-to-a-cell-in-excel
            // https://docs.microsoft.com/en-us/previous-versions/office/troubleshoot/office-developer/automate-excel-using-visual-c-fill-data
            Worksheet sheet = Globals.ThisAddIn.Application.ActiveSheet;

            // https://social.msdn.microsoft.com/Forums/vstudio/en-US/f89fe6b3-68c0-4a98-9522-953cc5befb34/how-to-make-a-excel-cell-readonly-by-c-code?forum=vsto
            sheet.Unprotect();
            Globals.ThisAddIn.Application.Cells.Locked = false;

            sheet.Change -= new Microsoft.Office.Interop.Excel.
                DocEvents_ChangeEventHandler(changesRange_Change);

            ((Excel.Range)sheet.Cells[1, IdColumnIndex]).Value2 = "Id";
            ((Excel.Range)sheet.Cells[1, DateColumnIndex]).Value2 = "Date";
            ((Excel.Range)sheet.Cells[1, DateColumnIndex]).EntireColumn.ColumnWidth = 14;
            ((Excel.Range)sheet.Cells[1, CustomerColumnIndex]).Value2 = "Customer";
            ((Excel.Range)sheet.Cells[1, CustomerColumnIndex]).EntireColumn.ColumnWidth = 25;
            ((Excel.Range)sheet.Cells[1, ProjectColumnIndex]).Value2 = "Project";
            ((Excel.Range)sheet.Cells[1, ProjectColumnIndex]).EntireColumn.ColumnWidth = 30;
            ((Excel.Range)sheet.Cells[1, ActivityColumnIndex]).Value2 = "Activity";
            ((Excel.Range)sheet.Cells[1, ActivityColumnIndex]).EntireColumn.ColumnWidth = 30;
            ((Excel.Range)sheet.Cells[1, DescColumnIndex]).Value2 = "Description";
            ((Excel.Range)sheet.Cells[1, DescColumnIndex]).EntireColumn.ColumnWidth = 80;
            ((Excel.Range)sheet.Cells[1, BeginTimeIndex]).Value2 = "Begin Time";
            ((Excel.Range)sheet.Cells[1, BeginTimeIndex]).EntireColumn.ColumnWidth = 25;
            ((Excel.Range)sheet.Cells[1, EndTimeIndex]).Value2 = "End Time";
            ((Excel.Range)sheet.Cells[1, EndTimeIndex]).EntireColumn.ColumnWidth = 25;

            // https://stackoverflow.com/questions/3310800/how-to-make-correct-date-format-when-writing-data-to-excel
            var cell = (Range)sheet.Range[sheet.Cells[2, DateColumnIndex], sheet.Cells[10000, DateColumnIndex]];
            cell.NumberFormat = "dd-MMM-yyyy"; // e.g. dd-MMM-yyyy
            var cellBegin = (Range)sheet.Range[sheet.Cells[2, BeginTimeIndex], sheet.Cells[10000, BeginTimeIndex]];
            cellBegin.NumberFormat = "hh:mm:ss"; // e.g. dd-MMM-yyyy
            var cellEnd = (Range)sheet.Range[sheet.Cells[2, EndTimeIndex], sheet.Cells[10000, EndTimeIndex]];
            cellEnd.NumberFormat = "hh:mm:ss"; // e.g. dd-MMM-yyyy

            //https://brandewinder.com/2011/01/23/Excel-In-Cell-DropDown-with-CSharp/
            for (int idxRow = 1; idxRow <= timesheets.Count; idxRow++)
            {
                ((Excel.Range)sheet.Cells[idxRow + 1, IdColumnIndex]).Value2 = timesheets[idxRow - 1].Id;
                ((Excel.Range)sheet.Cells[idxRow + 1, IdColumnIndex]).Interior.Color = Excel.XlRgbColor.rgbAliceBlue;
                ((Excel.Range)sheet.Cells[idxRow + 1, DateColumnIndex]).Value2 = timesheets[idxRow - 1].Begin.Date.ToOADate();
                if (timesheets[idxRow - 1].Project.HasValue)
                {
                    var project = Globals.ThisAddIn.GetProjectById(timesheets[idxRow - 1].Project.Value);
                    ((Excel.Range)sheet.Cells[idxRow + 1, CustomerColumnIndex]).Value2 = project.ParentTitle; // timesheets[idxRow - 1].cu;
                    ((Excel.Range)sheet.Cells[idxRow + 1, ProjectColumnIndex]).Value2 = project.Name;
                }
                if (timesheets[idxRow - 1].Activity.HasValue)
                {
                    var activity = Globals.ThisAddIn.GetActivityById(timesheets[idxRow - 1].Activity.Value);
                    ((Excel.Range)sheet.Cells[idxRow + 1, ActivityColumnIndex]).Value2 = activity.Name;
                }
                ((Excel.Range)sheet.Cells[idxRow + 1, DescColumnIndex]).Value2 = timesheets[idxRow - 1].Description;
                ((Excel.Range)sheet.Cells[idxRow + 1, BeginTimeIndex]).Value2 = timesheets[idxRow - 1].Begin.ToOADate();
                if (timesheets[idxRow - 1].End.HasValue)
                {
                    ((Excel.Range)sheet.Cells[idxRow + 1, EndTimeIndex]).Value2 = timesheets[idxRow - 1].End.Value.ToOADate();
                }
            }

            var customerList = new List<string>();
            foreach (var customer in customers)
            {
                customerList.Add(customer.Name);
            }

            var customerFlatList = string.Join(",", customerList.ToArray());

            AddDataValidationToColumn(sheet, customerFlatList, CustomerColumnIndex);

            var projectList = new List<string>();
            foreach (var project in projects)
            {
                projectList.Add(project.Name);
            }
            const string delimiter = ",";
            var projectFlatList = string.Join(",", projectList.ToArray());
            var projectFlatList2 = string.Join(delimiter, projects.Select(i => i.Name));
            AddDataValidationToColumn(sheet, projectFlatList, ProjectColumnIndex);

            sheet.Change += new Microsoft.Office.Interop.Excel.
                DocEvents_ChangeEventHandler(changesRange_Change);

            var activityList = new List<string>();
            foreach (var activity in activities)
            {
                activityList.Add(activity.Name);
            }

            var activityFlatList = string.Join(",", activityList.ToArray());

            AddDataValidationToColumn(sheet, activityFlatList, ActivityColumnIndex);

            // https://social.msdn.microsoft.com/Forums/vstudio/en-US/f89fe6b3-68c0-4a98-9522-953cc5befb34/how-to-make-a-excel-cell-readonly-by-c-code?forum=vsto
            Globals.ThisAddIn.Application.Cells.Locked = false;
            Globals.ThisAddIn.Application.get_Range("A1", $"A{timesheets.Count + 1}").Locked = true;
            sheet.Protect(Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
              Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

            await SyncNewRowsToKimai(services, timesheets, sheet).ConfigureAwait(false);
        }

        private async Task SyncNewRowsToKimai(IKimaiServices service, IList<Models.TimesheetCollection> timesheets, Worksheet sheet)
        {
            try
            {
                //find a row with no id to post
                for (int i = timesheets.Count; i < 10000; i++)
                {
                    dynamic id = ((Excel.Range)sheet.Cells[i, IdColumnIndex]).Value2;
                    if (id is int || id is double)
                    {

                    }
                    if (id is string)
                    {

                    }
                    if (id is null)
                    {
                        dynamic oADate = ((Excel.Range)sheet.Cells[i, DateColumnIndex]).Value2;

                        if (oADate is Double)//this is really probably an OADate 
                        // https://docs.microsoft.com/en-us/dotnet/api/system.datetime.tooadate?view=net-5.0
                        {
                            DateTime date = DateTime.FromOADate(oADate).Date; //make sure any time is ignored, it shouldn't be there
                            int addMinutes = GetNextAvailableTimeInMinutes(date);
                            int duration = 60;
                            var timesheet = await service.PostTimesheet(new Models.TimesheetEditForm()
                            {
                                Project = 1,
                                Activity = 1,
                                Begin = date.AddMinutes(addMinutes),
                                End = date.AddMinutes(addMinutes).AddMinutes(duration),
                                User = 5,
                                Description = $"mark {DateTime.Now.Ticks}"
                            }).ConfigureAwait(false);
                            Globals.ThisAddIn.Timesheets.Add(new Models.TimesheetCollection()
                            {
                                Id = timesheet.Id,
                                Begin = timesheet.Begin,
                                End = timesheet.End,
                                Activity = timesheet.Activity,
                                Project = timesheet.Project,
                                Duration = duration,
                                User = timesheet.User,
                                Description = timesheet.Description,
                                Tags = timesheet.Tags
                            });
                            // https://social.msdn.microsoft.com/Forums/vstudio/en-US/f89fe6b3-68c0-4a98-9522-953cc5befb34/how-to-make-a-excel-cell-readonly-by-c-code?forum=vsto
                            sheet.Unprotect();
                            Globals.ThisAddIn.Application.Cells.Locked = false;
                            sheet.Change -= new Microsoft.Office.Interop.Excel.
                            DocEvents_ChangeEventHandler(changesRange_Change);

                            ((Excel.Range)sheet.Cells[i, IdColumnIndex]).Value2 = timesheet.Id;
                            //((Excel.Range)sheet.Cells[timesheets.Count + 2, IdColumnIndex]).Interior.Color = Excel.XlRgbColor.rgbAliceBlue;
                            ((Excel.Range)sheet.Cells[i, DateColumnIndex]).Value2 = timesheet.Begin.Date.ToOADate();
                            if (timesheet.Project.HasValue)
                            {
                                var project = Globals.ThisAddIn.GetProjectById(timesheet.Project.Value);
                                ((Excel.Range)sheet.Cells[i, CustomerColumnIndex]).Value2 = project.ParentTitle; // timesheets[idxRow - 1].cu;
                                ((Excel.Range)sheet.Cells[i, ProjectColumnIndex]).Value2 = project.Name;
                            }
                            if (timesheet.Activity.HasValue)
                            {
                                var activity = Globals.ThisAddIn.GetActivityById(timesheet.Activity.Value);
                                ((Excel.Range)sheet.Cells[i, ActivityColumnIndex]).Value2 = activity.Name;
                            }
                            ((Excel.Range)sheet.Cells[i, DescColumnIndex]).Value2 = timesheet.Description;
                            ((Excel.Range)sheet.Cells[i, BeginTimeIndex]).Value2 = timesheet.Begin.ToOADate();
                            if (timesheets[i].End.HasValue)
                            {
                                ((Excel.Range)sheet.Cells[i, EndTimeIndex]).Value2 = timesheet.End.Value.ToOADate();
                            }

                            sheet.Change += new Microsoft.Office.Interop.Excel.
                            DocEvents_ChangeEventHandler(changesRange_Change);

                            // https://social.msdn.microsoft.com/Forums/vstudio/en-US/f89fe6b3-68c0-4a98-9522-953cc5befb34/how-to-make-a-excel-cell-readonly-by-c-code?forum=vsto
                            Globals.ThisAddIn.Application.Cells.Locked = false;
                            Globals.ThisAddIn.Application.get_Range("A1", $"A{i}").Locked = true;
                            sheet.Protect(Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                              Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
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
                string errors = ex.Message;
            }
        }

        private int GetNextAvailableTimeInMinutes(DateTime date)
        {
            var lastTimeEntry = Globals.ThisAddIn.Timesheets.OrderByDescending(x => x.End).FirstOrDefault(x => x.Begin.Date.Equals(date.Date) && x.End != null)?.End.Value;
            if(lastTimeEntry is null)
            { return 8 * 60; }
            return lastTimeEntry.Value.Hour * 60 + lastTimeEntry.Value.Minute;
        }

        private void AddDataValidationToColumn(Worksheet sheet, string flatList, int ColumnIndex)
        {
            // https://stackoverflow.com/questions/2333202/how-do-i-get-an-excel-range-using-row-and-column-numbers-in-vsto-c
            var cell = (Range)sheet.Range[sheet.Cells[1, ColumnIndex], sheet.Cells[10000, ColumnIndex]];
            cell.Validation.Delete();
            cell.Validation.Add(
               XlDVType.xlValidateList,
               XlDVAlertStyle.xlValidAlertInformation,
               XlFormatConditionOperator.xlBetween,
               flatList,
               Type.Missing);

            cell.Validation.IgnoreBlank = true;
            cell.Validation.InCellDropdown = true;
        }
    }
}
