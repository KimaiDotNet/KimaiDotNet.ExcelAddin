
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
            this.grpSettings = this.Factory.CreateRibbonGroup();
            this.tglApiCreds = this.Factory.CreateRibbonToggleButton();
            this.btnConnect = this.Factory.CreateRibbonButton();
            this.grpTimesheets = this.Factory.CreateRibbonGroup();
            this.btnSync = this.Factory.CreateRibbonButton();
            this.grpCalendar = this.Factory.CreateRibbonGroup();
            this.btnCalendar = this.Factory.CreateRibbonButton();
            this.btnSettings = this.Factory.CreateRibbonButton();
            this.box1 = this.Factory.CreateRibbonBox();
            this.lblStartDateTitle = this.Factory.CreateRibbonLabel();
            this.lblStartDate = this.Factory.CreateRibbonLabel();
            this.box2 = this.Factory.CreateRibbonBox();
            this.lblEndDateTitle = this.Factory.CreateRibbonLabel();
            this.lblEndDate = this.Factory.CreateRibbonLabel();
            this.btnSyncCalendarCategories = this.Factory.CreateRibbonButton();
            this.grpAdmin = this.Factory.CreateRibbonGroup();
            this.btnSyncCustomers = this.Factory.CreateRibbonButton();
            this.btnSyncProjects = this.Factory.CreateRibbonButton();
            this.grpSupport = this.Factory.CreateRibbonGroup();
            this.btnBuy = this.Factory.CreateRibbonButton();
            this.btnSponsor = this.Factory.CreateRibbonButton();
            this.btnInfo = this.Factory.CreateRibbonButton();
            this.btnBug = this.Factory.CreateRibbonButton();
            this.btnHelp = this.Factory.CreateRibbonButton();
            this.grpVersion = this.Factory.CreateRibbonGroup();
            this.lblVersion = this.Factory.CreateRibbonLabel();
            this.lblAddinVersionNo = this.Factory.CreateRibbonLabel();
            this.separator1 = this.Factory.CreateRibbonSeparator();
            this.lblServerVersion = this.Factory.CreateRibbonLabel();
            this.lblVersionNo = this.Factory.CreateRibbonLabel();
            this.tab1.SuspendLayout();
            this.grpSettings.SuspendLayout();
            this.grpTimesheets.SuspendLayout();
            this.grpCalendar.SuspendLayout();
            this.box1.SuspendLayout();
            this.box2.SuspendLayout();
            this.grpAdmin.SuspendLayout();
            this.grpSupport.SuspendLayout();
            this.grpVersion.SuspendLayout();
            this.SuspendLayout();
            // 
            // tab1
            // 
            this.tab1.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.tab1.Groups.Add(this.grpSettings);
            this.tab1.Groups.Add(this.grpTimesheets);
            this.tab1.Groups.Add(this.grpCalendar);
            this.tab1.Groups.Add(this.grpAdmin);
            this.tab1.Groups.Add(this.grpSupport);
            this.tab1.Groups.Add(this.grpVersion);
            this.tab1.Label = "Kimai";
            this.tab1.Name = "tab1";
            // 
            // grpSettings
            // 
            this.grpSettings.Items.Add(this.tglApiCreds);
            this.grpSettings.Items.Add(this.btnConnect);
            this.grpSettings.Label = "Settings";
            this.grpSettings.Name = "grpSettings";
            // 
            // tglApiCreds
            // 
            this.tglApiCreds.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.tglApiCreds.Label = "API Credentials";
            this.tglApiCreds.Name = "tglApiCreds";
            this.tglApiCreds.OfficeImageId = "PivotTableFieldSettings";
            this.tglApiCreds.ScreenTip = "test tip";
            this.tglApiCreds.ShowImage = true;
            this.tglApiCreds.SuperTip = "test super tip";
            this.tglApiCreds.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.tglApiCreds_Click);
            // 
            // btnConnect
            // 
            this.btnConnect.Label = "Connect";
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.OfficeImageId = "ConnectToServer";
            this.btnConnect.ShowImage = true;
            this.btnConnect.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnConnect_Click);
            // 
            // grpTimesheets
            // 
            this.grpTimesheets.Items.Add(this.btnSync);
            this.grpTimesheets.Label = "Timesheets";
            this.grpTimesheets.Name = "grpTimesheets";
            // 
            // btnSync
            // 
            this.btnSync.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnSync.Enabled = false;
            this.btnSync.Label = "Sync Data";
            this.btnSync.Name = "btnSync";
            this.btnSync.OfficeImageId = "ReplicationSynchronizeNow";
            this.btnSync.ShowImage = true;
            this.btnSync.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnSync_Click);
            // 
            // grpCalendar
            // 
            this.grpCalendar.Items.Add(this.btnCalendar);
            this.grpCalendar.Items.Add(this.btnSettings);
            this.grpCalendar.Items.Add(this.box1);
            this.grpCalendar.Items.Add(this.box2);
            this.grpCalendar.Items.Add(this.btnSyncCalendarCategories);
            this.grpCalendar.Label = "Calendar Import";
            this.grpCalendar.Name = "grpCalendar";
            // 
            // btnCalendar
            // 
            this.btnCalendar.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnCalendar.Label = "Import Calendar";
            this.btnCalendar.Name = "btnCalendar";
            this.btnCalendar.OfficeImageId = "CalendarImport";
            this.btnCalendar.ShowImage = true;
            this.btnCalendar.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnCalendar_Click);
            // 
            // btnSettings
            // 
            this.btnSettings.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnSettings.Label = "Settings";
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.OfficeImageId = "PivotTableFieldSettings";
            this.btnSettings.ShowImage = true;
            this.btnSettings.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnSettings_Click);
            // 
            // box1
            // 
            this.box1.Items.Add(this.lblStartDateTitle);
            this.box1.Items.Add(this.lblStartDate);
            this.box1.Name = "box1";
            // 
            // lblStartDateTitle
            // 
            this.lblStartDateTitle.Label = "Start Date";
            this.lblStartDateTitle.Name = "lblStartDateTitle";
            // 
            // lblStartDate
            // 
            this.lblStartDate.Label = "...";
            this.lblStartDate.Name = "lblStartDate";
            // 
            // box2
            // 
            this.box2.Items.Add(this.lblEndDateTitle);
            this.box2.Items.Add(this.lblEndDate);
            this.box2.Name = "box2";
            // 
            // lblEndDateTitle
            // 
            this.lblEndDateTitle.Label = "End Date   ";
            this.lblEndDateTitle.Name = "lblEndDateTitle";
            // 
            // lblEndDate
            // 
            this.lblEndDate.Label = "...";
            this.lblEndDate.Name = "lblEndDate";
            // 
            // btnSyncCalendarCategories
            // 
            this.btnSyncCalendarCategories.Label = "Import Categories";
            this.btnSyncCalendarCategories.Name = "btnSyncCalendarCategories";
            this.btnSyncCalendarCategories.OfficeImageId = "DataRefreshDialog";
            this.btnSyncCalendarCategories.ShowImage = true;
            this.btnSyncCalendarCategories.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnSyncCalendarCategories_Click);
            // 
            // grpAdmin
            // 
            this.grpAdmin.Items.Add(this.btnSyncCustomers);
            this.grpAdmin.Items.Add(this.btnSyncProjects);
            this.grpAdmin.Label = "Admin";
            this.grpAdmin.Name = "grpAdmin";
            // 
            // btnSyncCustomers
            // 
            this.btnSyncCustomers.Label = "Sync Customers";
            this.btnSyncCustomers.Name = "btnSyncCustomers";
            this.btnSyncCustomers.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnSyncPremuim_Click);
            // 
            // btnSyncProjects
            // 
            this.btnSyncProjects.Label = "Sync Projects";
            this.btnSyncProjects.Name = "btnSyncProjects";
            this.btnSyncProjects.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnSyncPremuim_Click);
            // 
            // grpSupport
            // 
            this.grpSupport.Items.Add(this.btnBuy);
            this.grpSupport.Items.Add(this.btnSponsor);
            this.grpSupport.Items.Add(this.btnInfo);
            this.grpSupport.Items.Add(this.btnBug);
            this.grpSupport.Items.Add(this.btnHelp);
            this.grpSupport.Label = "Support";
            this.grpSupport.Name = "grpSupport";
            // 
            // btnBuy
            // 
            this.btnBuy.Label = "Buy";
            this.btnBuy.Name = "btnBuy";
            this.btnBuy.OfficeImageId = "DataTypeCurrency";
            this.btnBuy.ShowImage = true;
            this.btnBuy.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnSyncPremuim_Click);
            // 
            // btnSponsor
            // 
            this.btnSponsor.Label = "Sponsor";
            this.btnSponsor.Name = "btnSponsor";
            this.btnSponsor.OfficeImageId = "DataTypeCurrency";
            this.btnSponsor.ShowImage = true;
            this.btnSponsor.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnSyncPremuim_Click);
            // 
            // btnInfo
            // 
            this.btnInfo.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnInfo.Label = "Information";
            this.btnInfo.Name = "btnInfo";
            this.btnInfo.OfficeImageId = "About";
            this.btnInfo.ShowImage = true;
            this.btnInfo.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnInfo_Click);
            // 
            // btnBug
            // 
            this.btnBug.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnBug.Label = "Report a bug";
            this.btnBug.Name = "btnBug";
            this.btnBug.OfficeImageId = "CodeEditor";
            this.btnBug.ShowImage = true;
            this.btnBug.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnBug_Click);
            // 
            // btnHelp
            // 
            this.btnHelp.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnHelp.Label = "View the forum";
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.OfficeImageId = "AccessOnlineLists";
            this.btnHelp.ShowImage = true;
            this.btnHelp.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnHelp_Click);
            // 
            // grpVersion
            // 
            this.grpVersion.Items.Add(this.lblVersion);
            this.grpVersion.Items.Add(this.lblAddinVersionNo);
            this.grpVersion.Items.Add(this.separator1);
            this.grpVersion.Items.Add(this.lblServerVersion);
            this.grpVersion.Items.Add(this.lblVersionNo);
            this.grpVersion.Label = "Version";
            this.grpVersion.Name = "grpVersion";
            // 
            // lblVersion
            // 
            this.lblVersion.Label = "Addin Version";
            this.lblVersion.Name = "lblVersion";
            // 
            // lblAddinVersionNo
            // 
            this.lblAddinVersionNo.Label = "0.0.0.0";
            this.lblAddinVersionNo.Name = "lblAddinVersionNo";
            // 
            // separator1
            // 
            this.separator1.Name = "separator1";
            // 
            // lblServerVersion
            // 
            this.lblServerVersion.Label = "Server Version";
            this.lblServerVersion.Name = "lblServerVersion";
            // 
            // lblVersionNo
            // 
            this.lblVersionNo.Label = "Not Connected";
            this.lblVersionNo.Name = "lblVersionNo";
            // 
            // KimaiRibbon
            // 
            this.Name = "KimaiRibbon";
            this.RibbonType = "Microsoft.Excel.Workbook";
            this.Tabs.Add(this.tab1);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.KimaiRibbon_Load);
            this.tab1.ResumeLayout(false);
            this.tab1.PerformLayout();
            this.grpSettings.ResumeLayout(false);
            this.grpSettings.PerformLayout();
            this.grpTimesheets.ResumeLayout(false);
            this.grpTimesheets.PerformLayout();
            this.grpCalendar.ResumeLayout(false);
            this.grpCalendar.PerformLayout();
            this.box1.ResumeLayout(false);
            this.box1.PerformLayout();
            this.box2.ResumeLayout(false);
            this.box2.PerformLayout();
            this.grpAdmin.ResumeLayout(false);
            this.grpAdmin.PerformLayout();
            this.grpSupport.ResumeLayout(false);
            this.grpSupport.PerformLayout();
            this.grpVersion.ResumeLayout(false);
            this.grpVersion.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab tab1;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup grpSettings;
        internal Microsoft.Office.Tools.Ribbon.RibbonToggleButton tglApiCreds;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnConnect;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup grpTimesheets;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup grpVersion;
        internal Microsoft.Office.Tools.Ribbon.RibbonLabel lblVersion;
        internal Microsoft.Office.Tools.Ribbon.RibbonLabel lblVersionNo;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnSync;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnSettings;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnCalendar;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup grpAdmin;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnSyncCustomers;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnSyncProjects;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup grpSupport;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnSponsor;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnBuy;
        internal Microsoft.Office.Tools.Ribbon.RibbonLabel lblAddinVersionNo;
        internal Microsoft.Office.Tools.Ribbon.RibbonSeparator separator1;
        internal Microsoft.Office.Tools.Ribbon.RibbonLabel lblServerVersion;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnInfo;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnBug;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnHelp;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup grpCalendar;
        internal Microsoft.Office.Tools.Ribbon.RibbonBox box1;
        internal Microsoft.Office.Tools.Ribbon.RibbonLabel lblStartDateTitle;
        internal Microsoft.Office.Tools.Ribbon.RibbonLabel lblStartDate;
        internal Microsoft.Office.Tools.Ribbon.RibbonBox box2;
        internal Microsoft.Office.Tools.Ribbon.RibbonLabel lblEndDateTitle;
        internal Microsoft.Office.Tools.Ribbon.RibbonLabel lblEndDate;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnSyncCalendarCategories;
    }

    partial class ThisRibbonCollection
    {
        internal KimaiRibbon KimaiRibbon
        {
            get { return this.GetRibbon<KimaiRibbon>(); }
        }
    }
}
