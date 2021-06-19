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

namespace MarkZither.KimaiDotNet.ExcelAddin
{
    [VstoUnhandledException]
    public partial class ThisAddIn
    {
        //https://docs.microsoft.com/en-us/visualstudio/vsto/how-to-create-and-modify-custom-document-properties?redirectedfrom=MSDN&view=vs-2019
        #region Properties
        public string ApiUrl { get; set; }
        public string ApiUsername { get; set; }
        public string ApiPassword { get; set; }
        public UserEntity CurrentUser { get; set; }
        public IList<ProjectCollection> Projects { get; set; }
        public IList<ActivityCollection> Activities { get; set; }
        public IList<CustomerCollection> Customers { get; set; }
        public IList<TimesheetCollection> Timesheets { get; set; }
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

        public ActivityCollection GetActivityByName(string name, int? projectId)
        {
            var activity = Activities.SingleOrDefault(x => x.Name.Equals(name, StringComparison.Ordinal)
            && x.Project.Value == projectId);
            if (activity == default(ActivityCollection))
            {
                Debug.Write($"Activity Name not found: {name}");
                ExcelAddin.Globals.ThisAddIn.Logger.LogInformation($"Activity Name not found: {name}");
            }
            return activity;
        }

        public ProjectCollection GetProjectByName(string name, int? customerId)
        {
            var project = Projects.SingleOrDefault(x => x.Name.Equals(name, StringComparison.Ordinal)
                && customerId.Value == x.Customer);
            if (project == default(ProjectCollection))
            {
                Debug.WriteLine($"name not found: {name}");
                ExcelAddin.Globals.ThisAddIn.Logger.LogInformation($"Project Name not found: {name}");
            }
            return project;
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

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            // attempt to make a global exception handler to avoid crashes
            // https://social.msdn.microsoft.com/Forums/vstudio/en-US/c37599d9-21e8-4c32-b00e-926f97c8f639/global-exception-handler-for-vs-2008-excel-addin?forum=vsto
            // https://stackoverflow.com/questions/12115030/catch-c-sharp-wpf-unhandled-exception-in-word-add-in-before-microsoft-displays-e
            // https://exceptionalcode.wordpress.com/2010/02/17/centralizing-vsto-add-in-exception-management-with-postsharp/
            // https://www.add-in-express.com/forum/read.php?FID=5&TID=12667
            RegisterToExceptionEvents();

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
                .CreateLogger();
            loggerFactory.AddSerilog(loggerConfig);

            // create logger and put it to work.
            var logProvider = loggerFactory.CreateLogger<ThisAddIn>();
            logProvider.LogDebug("debiggung");
            Logger = logProvider;

            // Configure PostSharp Logging to use Serilog
            LoggingServices.DefaultBackend = new MicrosoftLoggingBackend(loggerFactory);

            Globals.ThisAddIn.ApiUrl = Settings.Default?.ApiUrl;
            Globals.ThisAddIn.ApiUsername = Settings.Default?.ApiUsername;
            if (true)
            {
                throw new InvalidCastException("test exception");
            }
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            // https://docs.microsoft.com/en-us/dotnet/desktop/winforms/advanced/how-to-write-user-settings-at-run-time-with-csharp?view=netframeworkdesktop-4.8
            Settings.Default.ApiUrl = Globals.ThisAddIn.ApiUrl;
            Settings.Default.ApiUsername = Globals.ThisAddIn.ApiUsername;
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
