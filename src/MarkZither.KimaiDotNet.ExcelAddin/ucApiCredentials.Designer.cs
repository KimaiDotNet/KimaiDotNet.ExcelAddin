
namespace MarkZither.KimaiDotNet.ExcelAddin
{
    partial class ucApiCredentials
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.txtApiUsername = new System.Windows.Forms.TextBox();
            this.txtApiPassword = new System.Windows.Forms.TextBox();
            this.lblUsername = new System.Windows.Forms.Label();
            this.lblApiPassword = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.lblAPIUrl = new System.Windows.Forms.Label();
            this.txtAPIUrl = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnTestConnection = new System.Windows.Forms.Button();
            this.lblConnectionStatus = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // txtApiUsername
            // 
            this.txtApiUsername.Location = new System.Drawing.Point(19, 98);
            this.txtApiUsername.Name = "txtApiUsername";
            this.txtApiUsername.Size = new System.Drawing.Size(153, 20);
            this.txtApiUsername.TabIndex = 0;
            // 
            // txtApiPassword
            // 
            this.txtApiPassword.Location = new System.Drawing.Point(19, 148);
            this.txtApiPassword.Name = "txtApiPassword";
            this.txtApiPassword.Size = new System.Drawing.Size(153, 20);
            this.txtApiPassword.TabIndex = 1;
            this.txtApiPassword.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // lblUsername
            // 
            this.lblUsername.AutoSize = true;
            this.lblUsername.Location = new System.Drawing.Point(16, 73);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(55, 13);
            this.lblUsername.TabIndex = 2;
            this.lblUsername.Text = "Username";
            // 
            // lblApiPassword
            // 
            this.lblApiPassword.AutoSize = true;
            this.lblApiPassword.Location = new System.Drawing.Point(16, 121);
            this.lblApiPassword.Name = "lblApiPassword";
            this.lblApiPassword.Size = new System.Drawing.Size(73, 13);
            this.lblApiPassword.TabIndex = 3;
            this.lblApiPassword.Text = "API Password";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(19, 261);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 23);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lblAPIUrl
            // 
            this.lblAPIUrl.AutoSize = true;
            this.lblAPIUrl.Location = new System.Drawing.Point(16, 15);
            this.lblAPIUrl.Name = "lblAPIUrl";
            this.lblAPIUrl.Size = new System.Drawing.Size(40, 13);
            this.lblAPIUrl.TabIndex = 6;
            this.lblAPIUrl.Text = "API Url";
            // 
            // txtAPIUrl
            // 
            this.txtAPIUrl.Location = new System.Drawing.Point(19, 40);
            this.txtAPIUrl.Name = "txtAPIUrl";
            this.txtAPIUrl.Size = new System.Drawing.Size(153, 20);
            this.txtAPIUrl.TabIndex = 5;
            this.txtAPIUrl.Validating += new System.ComponentModel.CancelEventHandler(this.txtAPIUrl_Validating);
            this.txtAPIUrl.Validated += new System.EventHandler(this.txtAPIUrl_Validated);
            // 
            // toolTip1
            // 
            this.toolTip1.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTip1.ToolTipTitle = "API Url";
            // 
            // btnTestConnection
            // 
            this.btnTestConnection.Location = new System.Drawing.Point(19, 174);
            this.btnTestConnection.Name = "btnTestConnection";
            this.btnTestConnection.Size = new System.Drawing.Size(100, 23);
            this.btnTestConnection.TabIndex = 7;
            this.btnTestConnection.Text = "Test Connection";
            this.btnTestConnection.UseVisualStyleBackColor = true;
            this.btnTestConnection.Click += new System.EventHandler(this.btnTestConnection_Click);
            // 
            // lblConnectionStatus
            // 
            this.lblConnectionStatus.AutoSize = true;
            this.lblConnectionStatus.Location = new System.Drawing.Point(3, 9);
            this.lblConnectionStatus.Name = "lblConnectionStatus";
            this.lblConnectionStatus.Size = new System.Drawing.Size(101, 13);
            this.lblConnectionStatus.TabIndex = 8;
            this.lblConnectionStatus.Text = "lblConnectionStatus";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblConnectionStatus);
            this.panel1.Location = new System.Drawing.Point(19, 203);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(153, 52);
            this.panel1.TabIndex = 9;
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // ucApiCredentials
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnTestConnection);
            this.Controls.Add(this.lblAPIUrl);
            this.Controls.Add(this.txtAPIUrl);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.lblApiPassword);
            this.Controls.Add(this.lblUsername);
            this.Controls.Add(this.txtApiPassword);
            this.Controls.Add(this.txtApiUsername);
            this.Name = "ucApiCredentials";
            this.Size = new System.Drawing.Size(194, 311);
            this.Validating += new System.ComponentModel.CancelEventHandler(this.ucApiCredentials_Validating);
            this.Validated += new System.EventHandler(this.ucApiCredentials_Validated);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtApiUsername;
        private System.Windows.Forms.TextBox txtApiPassword;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.Label lblApiPassword;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label lblAPIUrl;
        private System.Windows.Forms.TextBox txtAPIUrl;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnTestConnection;
        private System.Windows.Forms.Label lblConnectionStatus;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ErrorProvider errorProvider;
    }
}
