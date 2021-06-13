using MarkZither.KimaiDotNet.ExcelAddin.Services;
using MarkZither.KimaiDotNet.ExcelAddin.Sheets;
using MarkZither.KimaiDotNet.Models;

using Microsoft.Extensions.Logging;
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
        private string GetVersionNumber()
        {
            return ApplicationDeployment.IsNetworkDeployed
                ? ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString()
                : Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
        #region events
        private void KimaiRibbon_Load(object sender, RibbonUIEventArgs e)
        {
            lblAddinVersionNo.Label = GetVersionNumber();
        }
        private void tglApiCreds_Click(object sender, RibbonControlEventArgs e)
        {
            Globals.ThisAddIn.TaskPane.Visible = ((RibbonToggleButton)sender).Checked;
        }
        private async void btnConnect_Click(object sender, RibbonControlEventArgs e)
        {
            var mockWorksheet =
                Globals.ThisAddIn.Application.Worksheets.Cast<Worksheet>()
                       .SingleOrDefault(w => string.Equals(w.Name, "Mock", StringComparison.OrdinalIgnoreCase));

            IKimaiServices services;
            try
            {
                if (string.Equals(ConfigurationManager.AppSettings["UseMocks"], "true", StringComparison.OrdinalIgnoreCase)
                    || mockWorksheet is Worksheet)
                {
                    services = new MockKimaiServices();
                }
                else
                {
                    services = new KimaiServices();
                }
                var version = await services.GetVersion().ConfigureAwait(false);
                lblVersionNo.Label = version.VersionProperty;
                var user = await services.GetCurrentUser().ConfigureAwait(false);
                Globals.ThisAddIn.CurrentUser = user;
                btnSync.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                ExcelAddin.Globals.ThisAddIn.Logger.LogWarning($"Could not connect to Kimai server. {ex.Message}", ex);
            }
        }
        private async void btnSync_Click(object sender, RibbonControlEventArgs e)
        {
            await Sheets.Sheet1.Instance.SyncTimesheetsAsync().ConfigureAwait(false);
        }
        private void btnSettings_Click(object sender, RibbonControlEventArgs e)
        {
            // Method intentionally left empty.
        }
        private void btnCalendar_Click(object sender, RibbonControlEventArgs e)
        {
            MessageBox.Show("Coming Soon!");
            ExcelAddin.Globals.ThisAddIn.Logger.LogInformation("Calendar coming soon");
        }
        private void btnSyncPremuim_Click(object sender, RibbonControlEventArgs e)
        {
            MessageBox.Show("This is a premium feature please consider sponsoring the project.");
            ExcelAddin.Globals.ThisAddIn.Logger.LogInformation("Premium sync");
        }
        private void btnInfo_Click(object sender, RibbonControlEventArgs e)
        {
            //https://stackoverflow.com/questions/19458721/cant-type-on-a-wpf-window-in-a-vsto-addin
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.Show();
        }
        #endregion
    }
}
