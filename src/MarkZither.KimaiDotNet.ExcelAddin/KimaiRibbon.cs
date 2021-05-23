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

        private void btnSync_Click(object sender, RibbonControlEventArgs e)
        {
            var userName = Globals.ThisAddIn.ApiUsername;
            var password = Globals.ThisAddIn.ApiPassword;

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(Globals.ThisAddIn.ApiUrl);
            client.DefaultRequestHeaders.Add("X-AUTH-USER", userName);
            client.DefaultRequestHeaders.Add("X-AUTH-TOKEN", password);

            Kimai2APIDocs docs = new Kimai2APIDocs(client, false);
            var timesheet = docs.ListTimesheetsRecordsUsingGet();

            lblVersionNo.Label = timesheet.Count.ToString();

            // https://docs.microsoft.com/en-us/visualstudio/vsto/how-to-programmatically-display-a-string-in-a-worksheet-cell?view=vs-2019
            // https://stackoverflow.com/questions/856196/vsto-write-to-a-cell-in-excel
            Excel.Application excel = new Excel.Application();
            Excel.Workbook workbook = excel.Workbooks.Add(Type.Missing);
            Excel.Worksheet sheet = (Excel.Worksheet)workbook.ActiveSheet; 
            ((Excel.Range)sheet.Cells[1, 1]).Value = "Hello";

            int endRow = 5;
            int endCol = 6;         
            for (int idxRow = 1; idxRow <= endRow; idxRow++)
            {
                for (int idxCol = 1; idxCol <= endCol; idxCol++)
                {
                    ((Excel.Range)sheet.Cells[idxRow, idxCol]).Value2 = "Kilroy wuz here";
                }
            }
        }
    }
}
