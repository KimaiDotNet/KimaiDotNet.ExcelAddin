using Flurl;

using MarkZither.KimaiDotNet.ExcelAddin.Models.Calendar;
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
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.Serialization;

using Excel = Microsoft.Office.Interop.Excel;

namespace MarkZither.KimaiDotNet.ExcelAddin
{
    [VstoUnhandledException]
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
            Globals.ThisAddIn.CalSyncStartDate = DateTime.Now.AddDays(-7);
            Globals.ThisAddIn.CalSyncEndDate = DateTime.Now.AddDays(7);
            lblStartDate.Label = Globals.ThisAddIn.CalSyncStartDate.Date.ToShortDateString();
            lblEndDate.Label = Globals.ThisAddIn.CalSyncEndDate.Date.ToShortDateString();
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
                if (Globals.ThisAddIn.UseMocks || mockWorksheet is Worksheet)
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
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.ShowDialog();
        }
        private void btnCalendar_Click(object sender, RibbonControlEventArgs e)
        {
            // the format of the EWS URL should be https://owa.youdomain.com/EWS/Exchange.asmx"
            string mbx = Globals.ThisAddIn.OWAUsername;
            ICalendarService calendarService = new EwsCalendarService(Globals.ThisAddIn.OWAUrl, mbx, Globals.ThisAddIn.OWAPassword);
            try
            {
                if (Globals.ThisAddIn.Categories is null)
                {
                    var categories = calendarService.GetCategories();
                    Globals.ThisAddIn.Categories = categories.Category;
                    Sheets.CalendarCategoryWorksheet.Instance.CreateOrUpdateCalendarCategoriesOnSheet(categories);
                }
                var appointments = calendarService.GetAppointments(Globals.ThisAddIn.CalSyncStartDate, Globals.ThisAddIn.CalSyncEndDate);
                Sheets.Sheet1.Instance.WriteCalendarRows(appointments);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private static bool RedirectionUrlValidationCallback(string redirectionUrl)
        {
            // The default for the validation callback is to reject the URL.
            bool result = false;
            Uri redirectionUri = new Uri(redirectionUrl);
            // Validate the contents of the redirection URL. In this simple validation
            // callback, the redirection URL is considered valid if it is using HTTPS
            // to encrypt the authentication credentials. 
            if (string.Equals(redirectionUri.Scheme, "https", StringComparison.OrdinalIgnoreCase))
            {
                result = true;
            }
            return result;
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
            if (true)
            {
                throw new InvalidCastException("test exception");
            }
        }
        #endregion

        private void btnBug_Click(object sender, RibbonControlEventArgs e)
        {
#pragma warning disable S1075 // URIs should not be hardcoded
            Process.Start("https://github.com/MarkZither/KimaiDotNet/issues");
#pragma warning restore S1075 // URIs should not be hardcoded
        }

        private void btnHelp_Click(object sender, RibbonControlEventArgs e)
        {
#pragma warning disable S1075 // URIs should not be hardcoded
            Process.Start("https://github.com/MarkZither/KimaiDotNet/discussions");
#pragma warning restore S1075 // URIs should not be hardcoded
        }

        private void btnSyncCalendarCategories_Click(object sender, RibbonControlEventArgs e)
        {
            // the format of the EWS URL should be https://owa.youdomain.com/EWS/Exchange.asmx"
            string mbx = Globals.ThisAddIn.OWAUsername;
            ICalendarService calendarService = new EwsCalendarService(Globals.ThisAddIn.OWAUrl, mbx, Globals.ThisAddIn.OWAPassword);

            var categories = calendarService.GetCategories();
            Globals.ThisAddIn.Categories = categories.Category;
            Sheets.CalendarCategoryWorksheet.Instance.CreateOrUpdateCalendarCategoriesOnSheet(categories);
        }
    }
}
