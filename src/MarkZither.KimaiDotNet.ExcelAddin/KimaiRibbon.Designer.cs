
namespace MarkZither.KimaiDotNet.ExcelAddin
{
    partial class KimaiRibbon : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public KimaiRibbon()
            : base(Globals.Factory.GetRibbonFactory())
        {
            InitializeComponent();
        }

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
            this.tab1 = this.Factory.CreateRibbonTab();
            this.group1 = this.Factory.CreateRibbonGroup();
            this.tglApiCreds = this.Factory.CreateRibbonToggleButton();
            this.btnConnect = this.Factory.CreateRibbonButton();
            this.group2 = this.Factory.CreateRibbonGroup();
            this.grpVersion = this.Factory.CreateRibbonGroup();
            this.lblVersion = this.Factory.CreateRibbonLabel();
            this.lblVersionNo = this.Factory.CreateRibbonLabel();
            this.btnSync = this.Factory.CreateRibbonButton();
            this.tab1.SuspendLayout();
            this.group1.SuspendLayout();
            this.group2.SuspendLayout();
            this.grpVersion.SuspendLayout();
            this.SuspendLayout();
            // 
            // tab1
            // 
            this.tab1.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.tab1.Groups.Add(this.group1);
            this.tab1.Groups.Add(this.group2);
            this.tab1.Groups.Add(this.grpVersion);
            this.tab1.Label = "Kimai";
            this.tab1.Name = "tab1";
            // 
            // group1
            // 
            this.group1.Items.Add(this.tglApiCreds);
            this.group1.Items.Add(this.btnConnect);
            this.group1.Label = "group1";
            this.group1.Name = "group1";
            // 
            // tglApiCreds
            // 
            this.tglApiCreds.Label = "API Credentials";
            this.tglApiCreds.Name = "tglApiCreds";
            this.tglApiCreds.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.tglApiCreds_Click);
            // 
            // btnConnect
            // 
            this.btnConnect.Image = global::MarkZither.KimaiDotNet.ExcelAddin.Properties.Resources.noun_connect_3698648;
            this.btnConnect.Label = "Connect";
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.ShowImage = true;
            this.btnConnect.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnConnect_Click);
            // 
            // group2
            // 
            this.group2.Items.Add(this.btnSync);
            this.group2.Label = "group2";
            this.group2.Name = "group2";
            // 
            // grpVersion
            // 
            this.grpVersion.Items.Add(this.lblVersion);
            this.grpVersion.Items.Add(this.lblVersionNo);
            this.grpVersion.Label = "Version";
            this.grpVersion.Name = "grpVersion";
            // 
            // lblVersion
            // 
            this.lblVersion.Label = "Version Details";
            this.lblVersion.Name = "lblVersion";
            // 
            // lblVersionNo
            // 
            this.lblVersionNo.Label = "Not Connected";
            this.lblVersionNo.Name = "lblVersionNo";
            // 
            // btnSync
            // 
            this.btnSync.Label = "Sync Data";
            this.btnSync.Name = "btnSync";
            this.btnSync.OfficeImageId = "ReplicationSynchronizeNow";
            this.btnSync.ShowImage = true;
            this.btnSync.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnSync_Click);
            // 
            // KimaiRibbon
            // 
            this.Name = "KimaiRibbon";
            this.RibbonType = "Microsoft.Excel.Workbook";
            this.Tabs.Add(this.tab1);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.KimaiRibbon_Load);
            this.tab1.ResumeLayout(false);
            this.tab1.PerformLayout();
            this.group1.ResumeLayout(false);
            this.group1.PerformLayout();
            this.group2.ResumeLayout(false);
            this.group2.PerformLayout();
            this.grpVersion.ResumeLayout(false);
            this.grpVersion.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab tab1;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup group1;
        internal Microsoft.Office.Tools.Ribbon.RibbonToggleButton tglApiCreds;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnConnect;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup group2;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup grpVersion;
        internal Microsoft.Office.Tools.Ribbon.RibbonLabel lblVersion;
        internal Microsoft.Office.Tools.Ribbon.RibbonLabel lblVersionNo;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnSync;
    }

    partial class ThisRibbonCollection
    {
        internal KimaiRibbon KimaiRibbon
        {
            get { return this.GetRibbon<KimaiRibbon>(); }
        }
    }
}
