using MarkZither.KimaiDotNet.ExcelAddin.Services;

using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Tools.Ribbon;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

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

        void changesRange_Change(Excel.Range Target)
        {
            string cellAddress = Target.get_Address(
                Microsoft.Office.Interop.Excel.XlReferenceStyle.xlA1);
            System.Windows.Forms.MessageBox.Show("Cell " + cellAddress + " changed.");
        }
        private void KimaiRibbon_Load(object sender, RibbonUIEventArgs e)
        {

        }

        private void tglApiCreds_Click(object sender, RibbonControlEventArgs e)
        {
            Globals.ThisAddIn.TaskPane.Visible = ((RibbonToggleButton)sender).Checked;
        }

        private void btnConnect_Click(object sender, RibbonControlEventArgs e)
        {
            var userName = Globals.ThisAddIn.ApiUsername;
            var password = Globals.ThisAddIn.ApiPassword;

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(Globals.ThisAddIn.ApiUrl);
            client.DefaultRequestHeaders.Add("X-AUTH-USER", userName);
            client.DefaultRequestHeaders.Add("X-AUTH-TOKEN", password);

            Kimai2APIDocs docs = new Kimai2APIDocs(client, false);
            var version = docs.VersionMethod();

            lblVersionNo.Label = version.VersionProperty;
        }

        private async void btnSync_Click(object sender, RibbonControlEventArgs e)
        {
            KimaiServices service = new KimaiServices();
            var projects = await service.GetProjects();
            Globals.ThisAddIn.Projects = projects.ToList();

            var customers = await service.GetCustomers();
            Globals.ThisAddIn.Customers = customers.ToList();

            var activities = await service.GetActivities();
            Globals.ThisAddIn.Activities = activities.ToList();

            var userName = Globals.ThisAddIn.ApiUsername;
            var password = Globals.ThisAddIn.ApiPassword;

            var timesheets = await service.GetTimesheets();
            
            // https://docs.microsoft.com/en-us/visualstudio/vsto/how-to-programmatically-display-a-string-in-a-worksheet-cell?view=vs-2019
            // https://stackoverflow.com/questions/856196/vsto-write-to-a-cell-in-excel
            // https://docs.microsoft.com/en-us/previous-versions/office/troubleshoot/office-developer/automate-excel-using-visual-c-fill-data
            Worksheet sheet = Globals.ThisAddIn.Application.ActiveSheet;

            // https://social.msdn.microsoft.com/Forums/vstudio/en-US/f89fe6b3-68c0-4a98-9522-953cc5befb34/how-to-make-a-excel-cell-readonly-by-c-code?forum=vsto
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

            // https://stackoverflow.com/questions/3310800/how-to-make-correct-date-format-when-writing-data-to-excel
            var cell = (Range)sheet.Range[sheet.Cells[2, DateColumnIndex], sheet.Cells[10000, DateColumnIndex]];
            cell.NumberFormat = "dd-MMM-yyyy"; // e.g. dd-MMM-yyyy

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

            var projectFlatList = string.Join(",", projectList.ToArray());

            AddDataValidationToColumn(sheet, projectFlatList, ProjectColumnIndex);

            sheet.Change += new Microsoft.Office.Interop.Excel.
                DocEvents_ChangeEventHandler(changesRange_Change);

            var activityList = new List<string>();
            foreach (var activity in activities)
            {
                activityList.Add(activity.Name);
            }

            var activityFlatList = string.Join(",", activityList.ToArray());

            //AddDataValidationToColumn(sheet, activityFlatList, ActivityColumnIndex);

            // https://social.msdn.microsoft.com/Forums/vstudio/en-US/f89fe6b3-68c0-4a98-9522-953cc5befb34/how-to-make-a-excel-cell-readonly-by-c-code?forum=vsto
            Globals.ThisAddIn.Application.Cells.Locked = false;
            Globals.ThisAddIn.Application.get_Range("A1", $"A{timesheets.Count}").Locked = true;
            sheet.Protect(Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
              Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

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
