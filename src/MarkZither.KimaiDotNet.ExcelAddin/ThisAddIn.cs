using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools.Excel;
using System.IO.IsolatedStorage;
using System.IO;
using System.Xml.Serialization;
using MarkZither.KimaiDotNet.ExcelAddin.Properties;
using MarkZither.KimaiDotNet.Models;
using System.Diagnostics;
using Serilog;
using Microsoft.Extensions.Logging;
using MarkZither.KimaiDotNet.ExcelAddin.Sheets;
using System.Threading;
using System.Windows.Threading;
using PostSharp;
using PostSharp.Patterns.Diagnostics;
using PostSharp.Patterns.Diagnostics.Backends.Microsoft;
using System.Windows.Forms;
using MarkZither.KimaiDotNet.ExcelAddin.Models.Calendar;
using System.Security;
using Serilog.Sinks.InfluxDB;
using Serilog.Events;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System.Configuration;

namespace MarkZither.KimaiDotNet.ExcelAddin
{
    [VstoUnhandledException]
    public partial class ThisAddIn
    {
        private InfluxDBSinkOptions GetInfluxDBSinkOptions()
        {
            #pragma warning disable S1075 // URIs should not be hardcoded
            string InfluxCloudUrl = "https://westeurope-1.azure.cloud2.influxdata.com";
            #pragma warning restore S1075 // URIs should not be hardcoded   
            string kimaiExcelInfluxCloudOrg = Environment.GetEnvironmentVariable("KimaiExcelInfluxCloudOrg");
            string kimaiExcelInfluxCloudToken = Environment.GetEnvironmentVariable("KimaiExcelInfluxCloudToken");
            return new InfluxDBSinkOptions()
            {
                ApplicationName = "fluentSample",
                InstanceName = "fluentSampleInstance",
                ConnectionInfo = new InfluxDBConnectionInfo()
                {
                    Uri = new Uri(InfluxCloudUrl),
                    BucketName = "logs",
                    OrganizationId = kimaiExcelInfluxCloudOrg,  // Organization Id - unique id can be found under Profile > About > Common Ids
                                                          // To be set if bucket already created and give write permission and set CreateBucketIfNotExists to false
                    Token = kimaiExcelInfluxCloudToken,
                    CreateBucketIfNotExists = false,
                    //To specify if Bucket needs to be created and if token not known or without all access permissions
                    //AllAccessToken = "",
                    //BucketRetentionPeriod = TimeSpan.FromDays(1)
                }
            };
        }

        //https://docs.microsoft.com/en-us/visualstudio/vsto/how-to-create-and-modify-custom-document-properties?redirectedfrom=MSDN&view=vs-2019
        #region Properties
        public bool UseMocks { get; private set; }
        public string ApiUrl { get; set; }
        public string ApiUsername { get; set; }
        public string ApiPassword { get; set; }
        public string OWAUrl { get; set; }
        public string OWAUsername { get; set; }
        public string OWAPassword { get; set; }
        public DateTime CalSyncStartDate { get; set; }
        public DateTime CalSyncEndDate { get; set; }
        public UserEntity CurrentUser { get; set; }
        public IList<ProjectCollection> Projects { get; set; }
        public IList<ActivityCollection> Activities { get; set; }
        public IList<CustomerCollection> Customers { get; set; }
        public IList<TimesheetCollection> Timesheets { get; set; }
        public IList<Category> Categories { get; set; }

        //https://docs.microsoft.com/en-us/visualstudio/vsto/walkthrough-synchronizing-a-custom-task-pane-with-a-ribbon-button?view=vs-2019
        private Microsoft.Office.Tools.CustomTaskPane apiCredentialsTaskPane;
        public Microsoft.Office.Tools.CustomTaskPane TaskPane
        {
            get
            {
                return apiCredentialsTaskPane;
            }
        }

        public Microsoft.Extensions.Logging.ILogger Logger { get; private set; }

        #endregion

        public ProjectCollection GetProjectById(int id)
        {
            var project = Projects.SingleOrDefault(x => x.Id.Equals(id));
            if (project == default(ProjectCollection))
            {
                Debug.Write($"Id not found: {id}");
                ExcelAddin.Globals.ThisAddIn.Logger.LogInformation($"Project Id not found: {id}");
            }
            return project;
        }

        public ActivityCollection GetActivityById(int id)
        {
            var activity = Activities.SingleOrDefault(x => x.Id.Equals(id));
            if (activity == default(ActivityCollection))
            {
                Debug.Write($"Id not found: {id}");
                ExcelAddin.Globals.ThisAddIn.Logger.LogInformation($"Activity Id not found: {id}");
            }
            return activity;
        }
        public CustomerCollection GetCustomerById(int id)
        {
            var customer = Customers.SingleOrDefault(x => x.Id.Equals(id));
            if (customer == default(CustomerCollection))
            {
                Debug.Write($"Id not found: {id}");
                ExcelAddin.Globals.ThisAddIn.Logger.LogInformation($"Customer Id not found: {id}");
            }
            return customer;
        }
        public ActivityCollection GetActivityByName(string name, int? projectId)
        {
            var activity = Activities.SingleOrDefault(x => x.Name.Equals(name, StringComparison.Ordinal)
            && ((x.Project.HasValue && x.Project.Value == projectId) || !x.Project.HasValue));
            if (activity == default(ActivityCollection))
            {
                Debug.Write($"Activity Name not found: {name}");
                ExcelAddin.Globals.ThisAddIn.Logger.LogInformation($"Activity Name not found: {name}");
            }
            return activity;
        }

        public ProjectCollection GetProjectByName(string name, int? customerId)
        {
            try
            {
                var project = Projects.SingleOrDefault(x => x.Name.Equals(name, StringComparison.Ordinal)
                    && ((x.Customer.HasValue && customerId.Value == x.Customer) || !x.Customer.HasValue));
                if (project == default(ProjectCollection))
                {
                    Debug.WriteLine($"name not found: {name}");
                    ExcelAddin.Globals.ThisAddIn.Logger.LogInformation($"Project Name not found: {name}");
                }
                return project;
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"Single project not found: {ex}");
                MessageBox.Show("You appear to have duplicate projects for this customer");
                var project = Projects.First(x => x.Name.Equals(name, StringComparison.Ordinal)
                    && ((x.Customer.HasValue && customerId.Value == x.Customer) || !x.Customer.HasValue));
                if (project == default(ProjectCollection))
                {
                    Debug.WriteLine($"name not found: {name}");
                    ExcelAddin.Globals.ThisAddIn.Logger.LogInformation($"Project Name not found: {name}");
                }
                return project;
            }
        }
        public CustomerCollection GetCustomerByName(string name)
        {
            var customer = Customers.SingleOrDefault(x => x.Name.Equals(name, StringComparison.Ordinal));
            if (customer == default(CustomerCollection))
            {
                Debug.WriteLine($"name not found: {name}");
                ExcelAddin.Globals.ThisAddIn.Logger.LogInformation($"Customer Name not found: {name}");
            }
            return customer;
        }
        internal Category GetCategoryByName(string name)
        {
            var category = Categories.SingleOrDefault(x => x.Name.Equals(name, StringComparison.Ordinal));
            if (category == default(Category))
            {
                Debug.WriteLine($"name not found: {name}");
                ExcelAddin.Globals.ThisAddIn.Logger.LogInformation($"Category Name not found: {name}");
            }
            return category;
        }
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
#if DEBUG
            bool.TryParse(ConfigurationManager.AppSettings["UseMocks"], out bool useMocks);
            UseMocks = useMocks;
#endif
            // attempt to make a global exception handler to avoid crashes
            // https://social.msdn.microsoft.com/Forums/vstudio/en-US/c37599d9-21e8-4c32-b00e-926f97c8f639/global-exception-handler-for-vs-2008-excel-addin?forum=vsto
            // https://stackoverflow.com/questions/12115030/catch-c-sharp-wpf-unhandled-exception-in-word-add-in-before-microsoft-displays-e
            // https://exceptionalcode.wordpress.com/2010/02/17/centralizing-vsto-add-in-exception-management-with-postsharp/
            // https://www.add-in-express.com/forum/read.php?FID=5&TID=12667
            RegisterToExceptionEvents();

#pragma warning disable S125 // Sections of code should not be commented out
                            //var kimaiExcelAppCenterKey = Environment.GetEnvironmentVariable("KimaiExcelAppCenterKey");
                            //AppCenter.Start(kimaiExcelAppCenterKey, typeof(Analytics), typeof(Crashes));
#pragma warning restore S125 // Sections of code should not be commented out
            var myUserControl1 = new ucApiCredentials();

            apiCredentialsTaskPane = this.CustomTaskPanes.Add(myUserControl1, "API Credentials");
            apiCredentialsTaskPane.VisibleChanged +=
                new EventHandler(myCustomTaskPane_VisibleChanged);

            // instantiate and configure logging. Using serilog here, to log to console and a text-file.
            var loggerFactory = new Microsoft.Extensions.Logging.LoggerFactory();
            var loggerConfig = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
#pragma warning disable S1075 // URIs should not be hardcoded
                .WriteTo.File("c:\\temp\\logs\\myapp.txt", rollingInterval: RollingInterval.Day)
#pragma warning restore S1075 // URIs should not be hardcoded
           .WriteTo.InfluxDB(GetInfluxDBSinkOptions(), restrictedToMinimumLevel: LogEventLevel.Information)
                .CreateLogger();
            loggerFactory.AddSerilog(loggerConfig);
            Serilog.Debugging.SelfLog.Enable(Console.Error);

            // create logger and put it to work.
            var logProvider = loggerFactory.CreateLogger<ThisAddIn>();
            logProvider.LogDebug("debiggung");
            Logger = logProvider;

            // Configure PostSharp Logging to use Serilog
            LoggingServices.DefaultBackend = new MicrosoftLoggingBackend(loggerFactory);

            Globals.ThisAddIn.ApiUrl = Settings.Default?.ApiUrl;
            Globals.ThisAddIn.ApiUsername = Settings.Default?.ApiUsername;
            Globals.ThisAddIn.OWAUrl = Settings.Default?.OWAUrl;
            Globals.ThisAddIn.OWAUsername = Settings.Default?.OWAUsername;

            this.Application.WorkbookActivate += Application_WorkbookActivate;
            this.Application.WorkbookOpen += Application_WorkbookOpen;
        }
        private void Application_WorkbookOpen(Excel.Workbook Wb)
        {
            Logger.LogInformation("In Workbook Open", Wb);
            if (Sheets.KimaiWorksheet.Instance.isInitialized())
            {
                Logger.LogInformation("Opened a workbook with a Kimai hidden sheet", Wb);
                Sheets.Sheet1.Instance.AddSheetChangeEventHandler();
            }
        }

        private void Application_WorkbookActivate(Excel.Workbook Wb)
        {
            Logger.LogInformation("In Workbook Activate", Wb);
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            // https://docs.microsoft.com/en-us/dotnet/desktop/winforms/advanced/how-to-write-user-settings-at-run-time-with-csharp?view=netframeworkdesktop-4.8
            Settings.Default.ApiUrl = Globals.ThisAddIn.ApiUrl;
            Settings.Default.ApiUsername = Globals.ThisAddIn.ApiUsername;
            Settings.Default.OWAUrl = Globals.ThisAddIn.OWAUrl;
            Settings.Default.OWAUsername = Globals.ThisAddIn.OWAUsername;
            Settings.Default.Save();
        }

        private void myCustomTaskPane_VisibleChanged(object sender, System.EventArgs e)
        {
            Globals.Ribbons.KimaiRibbon.tglApiCreds.Checked =
                apiCredentialsTaskPane.Visible;
        }

        private void RegisterToExceptionEvents()
        {
            System.Windows.Forms.Application.ThreadException += ApplicationThreadException;

            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;

            Dispatcher.CurrentDispatcher.UnhandledExceptionFilter +=
  new DispatcherUnhandledExceptionFilterEventHandler(Dispatcher_UnhandledExceptionFilter);
        }

        private void Dispatcher_UnhandledExceptionFilter(object sender, DispatcherUnhandledExceptionFilterEventArgs e)
        {
            HandleUnhandledException(e.Exception);
        }

        private bool _handlingUnhandledException;
        private void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleUnhandledException((Exception)e.ExceptionObject);//there is small possibility that this wont be exception but only when interacting with code that can throw object that does not inherit from Exception
        }

        private void ApplicationThreadException(object sender, ThreadExceptionEventArgs e)
        {
            HandleUnhandledException(e.Exception);
        }

        private void HandleUnhandledException(Exception exception)
        {
            if (_handlingUnhandledException)
                return;
            try
            {
                _handlingUnhandledException = true;
                Logger.LogCritical(exception, "Unhandled exception occurred, plug-in will close.");
            }
            finally
            {
                _handlingUnhandledException = false;
            }
        }
        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }

        #endregion
    }
}
