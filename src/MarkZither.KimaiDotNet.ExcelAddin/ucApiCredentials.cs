using MarkZither.KimaiDotNet.ExcelAddin.Services;

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

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if(this.ValidateChildren())
            {
                Globals.ThisAddIn.ApiUrl = txtAPIUrl.Text;
                Globals.ThisAddIn.ApiUsername = txtApiUsername.Text;
                Globals.ThisAddIn.ApiPassword = txtApiPassword.Text;

                IKimaiServices services;
                if (string.Equals(ConfigurationManager.AppSettings["UseMocks"], "true", StringComparison.OrdinalIgnoreCase))
                {
                    services = new MockKimaiServices();
                }
                else
                {
                    services = new KimaiServices();
                }
                var version = await services.GetVersion();
                Globals.Ribbons.GetRibbon<KimaiRibbon>().lblVersionNo.Label = version.VersionProperty;
                Globals.Ribbons.GetRibbon<KimaiRibbon>().btnSync.Enabled = true;
            }
        }

        private async void btnTestConnection_Click(object sender, EventArgs e)
        {
            if (this.ValidateChildren())
            {
                var APIUrl = txtAPIUrl.Text;
                var userName = txtApiUsername.Text;
                var password = txtApiPassword.Text;

                IKimaiServices services;
                if (string.Equals(ConfigurationManager.AppSettings["UseMocks"], "true", StringComparison.OrdinalIgnoreCase))
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
                    Do(lblConnectionStatus, rb => rb.Text = $"Success");
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
                //MessageBox.Show("API Url must be a valid Url");
                e.Cancel = true;
            }
        }

        private void ucApiCredentials_Validating(object sender, CancelEventArgs e)
        {
            //e.Cancel = true;
        }

        private void ucApiCredentials_Validated(object sender, EventArgs e)
        {
            //this.errorProvider.Clear();
        }

        private void txtAPIUrl_Validated(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            errorProvider.SetError(textBox, "");
        }
    }
}
