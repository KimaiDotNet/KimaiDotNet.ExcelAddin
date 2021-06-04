using MarkZither.KimaiDotNet.ExcelAddin.Services;

using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Tools.Ribbon;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Deployment.Application;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        private const string CustomersSheetName = "Customers";

        void changesRange_Change(Excel.Range Target)
        {
            string cellAddress = Target.get_Address(
                Microsoft.Office.Interop.Excel.XlReferenceStyle.xlA1);
        }
        private void KimaiRibbon_Load(object sender, RibbonUIEventArgs e)
        {
            lblAddinVersionNo.Label = GetVersionNumber();
        }

        private string GetVersionNumber()
        {
            string version = string.Empty;
            if(ApplicationDeployment.IsNetworkDeployed)
            {
                version = ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
            }
            else
            {
                version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
            return version;
        }

        private void tglApiCreds_Click(object sender, RibbonControlEventArgs e)
        {
            Globals.ThisAddIn.TaskPane.Visible = ((RibbonToggleButton)sender).Checked;
        }

        private async void btnConnect_Click(object sender, RibbonControlEventArgs e)
        {
            IKimaiServices services;
            try
            {
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

                btnSync.Enabled = true;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void btnSync_Click(object sender, RibbonControlEventArgs e)
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

            var projects = await services.GetProjects().ConfigureAwait(false);
            Globals.ThisAddIn.Projects = projects.ToList();
            CreateOrUpdateProjectsOnSheet(projects);

            var customers = await services.GetCustomers().ConfigureAwait(false);
            Globals.ThisAddIn.Customers = customers.ToList();
            CreateOrUpdateCustomersOnSheet(customers);

            var activities = await services.GetActivities().ConfigureAwait(false);
            Globals.ThisAddIn.Activities = activities.ToList();
            CreateOrUpdateActivitiesOnSheet(activities);

            // https://docs.microsoft.com/en-us/visualstudio/vsto/how-to-programmatically-display-a-string-in-a-worksheet-cell?view=vs-2019
            // https://stackoverflow.com/questions/856196/vsto-write-to-a-cell-in-excel
            // https://docs.microsoft.com/en-us/previous-versions/office/troubleshoot/office-developer/automate-excel-using-visual-c-fill-data
            Worksheet sheet = Globals.ThisAddIn.Application.ActiveSheet as Microsoft.Office.Interop.Excel.Worksheet;
            if (Globals.ThisAddIn.Timesheets != null)
            {
                await SyncNewRowsToKimai(services, Globals.ThisAddIn.Timesheets, sheet).ConfigureAwait(false);
            }
            var timesheets = await services.GetTimesheets().ConfigureAwait(false);
            Globals.ThisAddIn.Timesheets = timesheets.ToList();

            // https://social.msdn.microsoft.com/Forums/vstudio/en-US/f89fe6b3-68c0-4a98-9522-953cc5befb34/how-to-make-a-excel-cell-readonly-by-c-code?forum=vsto
            sheet.Unprotect();
            Globals.ThisAddIn.Application.Cells.Locked = false;

            sheet.Change -= new Microsoft.Office.Interop.Excel.
                DocEvents_ChangeEventHandler(changesRange_Change);

            SetupTimesheetsHeaderRow(sheet);
            WriteTimesheetRows(sheet, timesheets);

            //https://stackoverflow.com/questions/2414591/how-to-create-validation-from-name-range-on-another-worksheet-in-excel-using-c
            //https://docs.microsoft.com/en-us/visualstudio/vsto/how-to-add-namedrange-controls-to-worksheets?view=vs-2019
            
            Microsoft.Office.Interop.Excel.Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet)Globals.ThisAddIn.Application.ActiveWorkbook.Worksheets[1];
            Microsoft.Office.Tools.Excel.Worksheet extendedWorksheet = Globals.Factory.GetVstoObject(worksheet);

            //https://stackoverflow.com/questions/10373561/convert-a-number-to-a-letter-in-c-sharp-for-use-in-microsoft-excel
            //https://stackoverflow.com/a/10373827
            AddDataValidationToColumnByRange(customers.Count, sheet, CustomersSheetName, ExcelAddin.Constants.CustomersSheet.CustomerNameColumnIndex, CustomerColumnIndex);

            var projectList = new List<string>();
            foreach (var project in projects)
            {
                projectList.Add(project.Name);
            }
            const string delimiter = ",";
            var projectFlatList = string.Join(",", projectList.ToArray());
            var projectFlatList2 = string.Join(delimiter, projects.Select(i => i.Name));
            AddDataValidationToColumnWithFlatList(sheet, projectFlatList, ProjectColumnIndex);

            sheet.Change += new Microsoft.Office.Interop.Excel.
                DocEvents_ChangeEventHandler(changesRange_Change);

            var activityList = new List<string>();
            foreach (var activity in activities)
            {
                activityList.Add(activity.Name);
            }

            var activityFlatList = string.Join(",", activityList.ToArray());

            AddDataValidationToColumnWithFlatList(sheet, activityFlatList, ActivityColumnIndex);

            // https://social.msdn.microsoft.com/Forums/vstudio/en-US/f89fe6b3-68c0-4a98-9522-953cc5befb34/how-to-make-a-excel-cell-readonly-by-c-code?forum=vsto
            Globals.ThisAddIn.Application.Cells.Locked = false;
            Globals.ThisAddIn.Application.get_Range("A1", $"A{timesheets.Count + 1}").Locked = true;
            sheet.Protect(Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
              Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
        }

        private static void WriteTimesheetRows(Worksheet sheet, IList<Models.TimesheetCollection> timesheets)
        {
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
        }

        private static void AddDataValidationToColumnByRange(int rowCount, Worksheet sheet, string validationListSheetName, int validationListColumnIndex, int validatedColumnIndex)
        {
            //string forumula = "='" + "Customers" + "'!B2:B" + (rowCount + 1).ToString();
            //string forumula = "='" + "Customers" + "'!B:B";

            //string forumula = $"='{validationListSheetName}'!{GetColumnName(validationListColumnIndex - 1)}1:{GetColumnName(validationListColumnIndex - 1)}{(rowCount + 2).ToString()}";
            string forumula = $"='{validationListSheetName}'!{GetColumnName(validationListColumnIndex - 1)}:{GetColumnName(validationListColumnIndex - 1)}";
            Excel.Range cell = sheet.Range[$"{GetColumnName(validatedColumnIndex - 1)}2:{GetColumnName(validatedColumnIndex - 1)}10000"];
            cell.Validation.Delete();
            //cell6.Validation.Add(XlDVType.xlValidateList, XlDVAlertStyle.xlValidAlertInformation, XlFormatConditionOperator.xlBetween, flatlistuom, Type.Missing);
            cell.Validation.Add(XlDVType.xlValidateList, XlDVAlertStyle.xlValidAlertInformation, XlFormatConditionOperator.xlEqual, forumula, Type.Missing);
            cell.Validation.IgnoreBlank = true;
            cell.Validation.InCellDropdown = true;
        }

        private void CreateOrUpdateProjectsOnSheet(IList<Models.ProjectCollection> projects)
        {
            Worksheet sheet = Globals.ThisAddIn.Application.ActiveSheet as Microsoft.Office.Interop.Excel.Worksheet;
            var projectsWorksheet =
                Globals.ThisAddIn.Application.Worksheets.Cast<Worksheet>()
                                   .SingleOrDefault(w => string.Equals(w.Name, "Projects", StringComparison.OrdinalIgnoreCase));
            //Excel.Worksheet projectsWorksheet = Globals.ThisAddIn.Application.ThisWorkbook.Worksheets.Item["Foo"];
            if (projectsWorksheet == null)
            {
                projectsWorksheet = (Excel.Worksheet)Globals.ThisAddIn.Application.Worksheets.Add(Missing.Value, sheet);
                projectsWorksheet.Name = "Projects";
            }

            sheet.Select();
            SetupProjectHeaderRow(projectsWorksheet);

            //https://brandewinder.com/2011/01/23/Excel-In-Cell-DropDown-with-CSharp/
            for (int idxRow = 1; idxRow <= projects.Count; idxRow++)
            {
                ((Excel.Range)projectsWorksheet.Cells[idxRow + 1, IdColumnIndex]).Value2 = projects[idxRow - 1].Id;
                ((Excel.Range)projectsWorksheet.Cells[idxRow + 1, IdColumnIndex]).Interior.Color = Excel.XlRgbColor.rgbAliceBlue;
                ((Excel.Range)projectsWorksheet.Cells[idxRow + 1, DateColumnIndex]).Value2 = projects[idxRow - 1].Name;
                if (projects[idxRow - 1].Customer.HasValue)
                {
                    ((Excel.Range)projectsWorksheet.Cells[idxRow + 1, CustomerColumnIndex]).Value2 = projects[idxRow - 1].ParentTitle; // timesheets[idxRow - 1].cu;
                    ((Excel.Range)projectsWorksheet.Cells[idxRow + 1, ProjectColumnIndex]).Value2 = projects[idxRow - 1].Customer.Value;
                }
            }
        }

        private static void SetupProjectHeaderRow(Worksheet sheet)
        {
            ((Excel.Range)sheet.Cells[1, IdColumnIndex]).Value2 = "Id";
            ((Excel.Range)sheet.Cells[1, DateColumnIndex]).Value2 = "Name";
            ((Excel.Range)sheet.Cells[1, DateColumnIndex]).EntireColumn.ColumnWidth = 14;
            ((Excel.Range)sheet.Cells[1, CustomerColumnIndex]).Value2 = "Parent Title";
            ((Excel.Range)sheet.Cells[1, CustomerColumnIndex]).EntireColumn.ColumnWidth = 25;
            ((Excel.Range)sheet.Cells[1, ProjectColumnIndex]).Value2 = "Customer Id";
        }

        private void CreateOrUpdateActivitiesOnSheet(IList<Models.ActivityCollection> activities)
        {
            Worksheet sheet = Globals.ThisAddIn.Application.ActiveSheet as Microsoft.Office.Interop.Excel.Worksheet;
            var activitiesWorksheet =
                Globals.ThisAddIn.Application.Worksheets.Cast<Worksheet>()
                                   .SingleOrDefault(w => string.Equals(w.Name, "Activities", StringComparison.OrdinalIgnoreCase));
            //Excel.Worksheet projectsWorksheet = Globals.ThisAddIn.Application.ThisWorkbook.Worksheets.Item["Foo"];
            if (activitiesWorksheet == null)
            {
                activitiesWorksheet = (Excel.Worksheet)Globals.ThisAddIn.Application.Worksheets.Add(Missing.Value, sheet);
                activitiesWorksheet.Name = "Activities";
            }
            activitiesWorksheet.Visible = Excel.XlSheetVisibility.xlSheetVeryHidden;

            sheet.Select();
            SetupActivitiesHeaderRow(activitiesWorksheet);

            //https://brandewinder.com/2011/01/23/Excel-In-Cell-DropDown-with-CSharp/
            for (int idxRow = 1; idxRow <= activities.Count; idxRow++)
            {
                ((Excel.Range)activitiesWorksheet.Cells[idxRow + 1, IdColumnIndex]).Value2 = activities[idxRow - 1].Id;
                ((Excel.Range)activitiesWorksheet.Cells[idxRow + 1, IdColumnIndex]).Interior.Color = Excel.XlRgbColor.rgbAliceBlue;
                ((Excel.Range)activitiesWorksheet.Cells[idxRow + 1, DateColumnIndex]).Value2 = activities[idxRow - 1].Name;
                if (activities[idxRow - 1].Project.HasValue)
                {
                    ((Excel.Range)activitiesWorksheet.Cells[idxRow + 1, CustomerColumnIndex]).Value2 = activities[idxRow - 1].ParentTitle; // timesheets[idxRow - 1].cu;
                    ((Excel.Range)activitiesWorksheet.Cells[idxRow + 1, ProjectColumnIndex]).Value2 = activities[idxRow - 1].Project.Value;
                }
            }
        }

        private static void SetupActivitiesHeaderRow(Worksheet sheet)
        {
            ((Excel.Range)sheet.Cells[1, IdColumnIndex]).Value2 = "Id";
            ((Excel.Range)sheet.Cells[1, DateColumnIndex]).Value2 = "Name";
            ((Excel.Range)sheet.Cells[1, DateColumnIndex]).EntireColumn.ColumnWidth = 14;
        }
        private void CreateOrUpdateCustomersOnSheet(IList<Models.CustomerCollection> customers)
        {
            Worksheet sheet = Globals.ThisAddIn.Application.ActiveSheet as Microsoft.Office.Interop.Excel.Worksheet;
            var customersWorksheet =
                Globals.ThisAddIn.Application.Worksheets.Cast<Worksheet>()
                                   .SingleOrDefault(w => string.Equals(w.Name, CustomersSheetName, StringComparison.OrdinalIgnoreCase));
            //Excel.Worksheet projectsWorksheet = Globals.ThisAddIn.Application.ThisWorkbook.Worksheets.Item["Foo"];
            if (customersWorksheet == null)
            {
                customersWorksheet = (Excel.Worksheet)Globals.ThisAddIn.Application.Worksheets.Add(Missing.Value, sheet);
                customersWorksheet.Name = CustomersSheetName;
            }

            sheet.Select();
            SetupCustomerHeaderRow(customersWorksheet);

            //https://brandewinder.com/2011/01/23/Excel-In-Cell-DropDown-with-CSharp/
            for (int idxRow = 1; idxRow <= customers.Count; idxRow++)
            {
                ((Excel.Range)customersWorksheet.Cells[idxRow + 1, IdColumnIndex]).Value2 = customers[idxRow - 1].Id;
                ((Excel.Range)customersWorksheet.Cells[idxRow + 1, IdColumnIndex]).Interior.Color = Excel.XlRgbColor.rgbAliceBlue;
                ((Excel.Range)customersWorksheet.Cells[idxRow + 1, DateColumnIndex]).Value2 = customers[idxRow - 1].Name;
            }
        }

        private static void SetupCustomerHeaderRow(Worksheet sheet)
        {
            ((Excel.Range)sheet.Cells[1, IdColumnIndex]).Value2 = "Id";
            ((Excel.Range)sheet.Cells[1, DateColumnIndex]).Value2 = "Name";
            ((Excel.Range)sheet.Cells[1, DateColumnIndex]).EntireColumn.ColumnWidth = 14;
        }

        private static void SetupTimesheetsHeaderRow(Worksheet sheet)
        {
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

        private void AddDataValidationToColumnWithFlatList(Worksheet sheet, string flatList, int columnIndex)
        {
            // might not use this method as there is a limit to the number of items that can be supported
            // however the other way has the limitaiton of needing to have a single range
            // if for example the a project has no customer there might be 2 ranges required
            // at which point we will have cell level validation rather than the whole column
            // and probably namy fewer items in the list
            //https://brandewinder.com/2011/01/23/Excel-In-Cell-DropDown-with-CSharp/
            // https://stackoverflow.com/questions/2333202/how-do-i-get-an-excel-range-using-row-and-column-numbers-in-vsto-c
            var cell = (Range)sheet.Range[sheet.Cells[1, columnIndex], sheet.Cells[10000, columnIndex]];

            try
            {
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
            catch(Exception ex)
            {
                MessageBox.Show($"Failed to set validation on column index ${columnIndex}");
            }
        }

        private void btnSettings_Click(object sender, RibbonControlEventArgs e)
        {

        }

        private void btnCalendar_Click(object sender, RibbonControlEventArgs e)
        {
            MessageBox.Show("Coming Soon!");
        }

        private void btnSyncPremuim_Click(object sender, RibbonControlEventArgs e)
        {
            MessageBox.Show("This is a premium feature please consider sponsoring the project.");
        }

        private void btnInfo_Click(object sender, RibbonControlEventArgs e)
        {
            //https://stackoverflow.com/questions/19458721/cant-type-on-a-wpf-window-in-a-vsto-addin
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.Show();
        }

        static string GetColumnName(int index)
        {
            const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            var value = "";

            if (index >= letters.Length)
                value += letters[index / letters.Length - 1];

            value += letters[index % letters.Length];

            return value;
        }
    }
}
