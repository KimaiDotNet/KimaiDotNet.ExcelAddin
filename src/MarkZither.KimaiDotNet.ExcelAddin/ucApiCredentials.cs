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
            }
        }

        private void btnTestConnection_Click(object sender, EventArgs e)
        {
            var userName = txtApiUsername.Text;
            var password = txtApiPassword.Text;

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://demo-plugins.kimai.org/");
            client.DefaultRequestHeaders.Add("X-AUTH-USER", userName);
            client.DefaultRequestHeaders.Add("X-AUTH-TOKEN", password);

            IKimaiServices services;
            if (string.Equals(ConfigurationManager.AppSettings["UseMocks"], "true", StringComparison.OrdinalIgnoreCase))
            {
                services = new MockKimaiServices();
            }
            else
            {
                services = new KimaiServices();
            }
            try
            {
                _ = services.GetPing();
                lblConnectionStatus.ForeColor = Color.Green;
                lblConnectionStatus.Text = "Success";
            }
            catch(Exception ex)
            {
                lblConnectionStatus.ForeColor = Color.Red;
                lblConnectionStatus.Text = $"Failed: {ex.Message}";
            }
        }

        private void txtAPIUrl_Validating(object sender, CancelEventArgs e)
        {
            //make sure it is a url         
            if (txtAPIUrl.Text.Length == 0)
            {
                e.Cancel = true;
            }
            else
            {
                e.Cancel = false;
            }
        }
    }
}
