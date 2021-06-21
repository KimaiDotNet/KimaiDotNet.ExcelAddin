using MarkZither.KimaiDotNet.ExcelAddin.Services;

using Microsoft.Extensions.Logging;
using Microsoft.Office.Interop.Excel;
using Microsoft.Rest;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MarkZither.KimaiDotNet.ExcelAddin
{
    public partial class ucApiCredentials : UserControl
    {
        public ucApiCredentials()
        {
            InitializeComponent();
            txtAPIUrl.Text = Settings.Default.ApiUrl;
            txtApiUsername.Text = Settings.Default.ApiUsername;
            lblConnectionStatus.Text = "";
        }
        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (this.ValidateChildren())
            {
                Globals.ThisAddIn.ApiUrl = txtAPIUrl.Text;
                Globals.ThisAddIn.ApiUsername = txtApiUsername.Text;
                Globals.ThisAddIn.ApiPassword = txtApiPassword.Text;

                var mockWorksheet =
                Globals.ThisAddIn.Application.Worksheets.Cast<Worksheet>()
                       .SingleOrDefault(w => string.Equals(w.Name, "Mock", StringComparison.OrdinalIgnoreCase));
                IKimaiServices services;
                if (string.Equals(ConfigurationManager.AppSettings["UseMocks"], "true", StringComparison.OrdinalIgnoreCase)
                    || mockWorksheet is Worksheet)
                {
                    services = new MockKimaiServices();
                }
                else
                {
                    services = new KimaiServices();
                }
                try
                {
                    var version = await services.GetVersion();
                    var user = await services.GetCurrentUser();
                    ExcelAddin.Globals.ThisAddIn.Logger.LogInformation("Connected to Kimai", version);
                    Globals.Ribbons.GetRibbon<KimaiRibbon>().lblVersionNo.Label = version.VersionProperty;
                    Globals.Ribbons.GetRibbon<KimaiRibbon>().btnSync.Enabled = true;
                    Globals.ThisAddIn.CurrentUser = user;
                    Do(lblConnectionStatus, rb => rb.ForeColor = Color.Green);
                    Do(lblConnectionStatus, rb => rb.Text = $"Success");
                }
                catch (HttpOperationException hoex) when (hoex.Response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    Do(lblConnectionStatus, rb => rb.ForeColor = Color.Red);
                    Do(lblConnectionStatus, rb => rb.Text = $"Failed: Username or Password Incorrect {Environment.NewLine} {hoex.Message}");
                }
                catch (HttpOperationException hoex)
                {
                    Do(lblConnectionStatus, rb => rb.ForeColor = Color.Red);
                    Do(lblConnectionStatus, rb => rb.Text = $"Failed: {hoex.Message}");
                }
                catch (HttpRequestException hrex)
                {
                    Do(lblConnectionStatus, rb => rb.ForeColor = Color.Red);
                    Do(lblConnectionStatus, rb => rb.Text = $"Failed: Server name incorrect {Environment.NewLine} {hrex.Message}");
                }
                catch (Exception ex)
                {
                    // i'm using a little helper method here...
                    // https://stackoverflow.com/questions/31007145/asynchronous-ui-updates-in-winforms
                    Do(lblConnectionStatus, rb => rb.ForeColor = Color.Red);
                    Do(lblConnectionStatus, rb => rb.Text = $"Failed: {ex.Message}");
                }
            }
        }

        private async void btnTestConnection_Click(object sender, EventArgs e)
        {
            if (this.ValidateChildren())
            {
                var APIUrl = txtAPIUrl.Text;
                var userName = txtApiUsername.Text;
                var password = txtApiPassword.Text;

                var mockWorksheet =
                Globals.ThisAddIn.Application.Worksheets.Cast<Worksheet>()
                       .SingleOrDefault(w => string.Equals(w.Name, "Mock", StringComparison.OrdinalIgnoreCase));
                IKimaiServices services;
                if (string.Equals(ConfigurationManager.AppSettings["UseMocks"], "true", StringComparison.OrdinalIgnoreCase)
                    || mockWorksheet is Worksheet)
                {
                    services = new MockKimaiServices(userName, password, APIUrl);
                }
                else
                {
                    services = new KimaiServices(userName, password, APIUrl);
                }
                try
                {
                    _ = await services.GetPing().ConfigureAwait(false);
                    Do(lblConnectionStatus, rb => rb.ForeColor = Color.Green);
                    Do(lblConnectionStatus, rb => rb.Text = $"Success: API ping call ");
                }
                catch (HttpOperationException hoex) when (hoex.Response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    Do(lblConnectionStatus, rb => rb.ForeColor = Color.Red);
                    Do(lblConnectionStatus, rb => rb.Text = $"Failed: Username or Password Incorrect {Environment.NewLine} {hoex.Message}");
                }
                catch (HttpOperationException hoex)
                {
                    Do(lblConnectionStatus, rb => rb.ForeColor = Color.Red);
                    Do(lblConnectionStatus, rb => rb.Text = $"Failed: {hoex.Message}");
                }
                catch (HttpRequestException hrex)
                {
                    Do(lblConnectionStatus, rb => rb.ForeColor = Color.Red);
                    Do(lblConnectionStatus, rb => rb.Text = $"Failed: Server name incorrect {Environment.NewLine} {hrex.Message}");
                }
                catch (Exception ex)
                {
                    // i'm using a little helper method here...
                    // https://stackoverflow.com/questions/31007145/asynchronous-ui-updates-in-winforms
                    Do(lblConnectionStatus, rb => rb.ForeColor = Color.Red);
                    Do(lblConnectionStatus, rb => rb.Text = $"Failed: {ex.Message}");
                }
            }
        }

        public static void Do<TControl>(TControl control, Action<TControl> action) where TControl : Control
        {
            if (control.InvokeRequired)
            {
                control.Invoke(action, control);
            }
            else
            {
                action(control);
            }
        }

        private void txtAPIUrl_Validating(object sender, CancelEventArgs e)
        {
            //make sure it is a url         
            if (txtAPIUrl.Text.Length > 0 && Uri.TryCreate(txtAPIUrl.Text, UriKind.Absolute, out Uri _))
            {
                txtAPIUrl.BackColor = Color.White;
                e.Cancel = false;
            }
            else
            {
                txtAPIUrl.BackColor = Color.PaleVioletRed;
                this.errorProvider.SetError(this.txtAPIUrl, "Invalid value!");
                e.Cancel = true;
            }
        }

        private void ucApiCredentials_Validating(object sender, CancelEventArgs e)
        {
            // Method intentionally left empty.
        }

        private void ucApiCredentials_Validated(object sender, EventArgs e)
        {
            this.errorProvider.Clear();
        }

        private void txtAPIUrl_Validated(object sender, EventArgs e)
        {
            System.Windows.Forms.TextBox textBox = sender as System.Windows.Forms.TextBox;
            errorProvider.SetError(textBox, "");
        }

        private void ucApiCredentials_SizeChanged(object sender, EventArgs e)
        {
            lblConnectionStatus.Size = new Size(((ucApiCredentials)sender).Size.Width, lblConnectionStatus.Size.Height);
            panel1.Size = new Size(((ucApiCredentials)sender).Size.Width, panel1.Size.Height);
        }
    }
}
