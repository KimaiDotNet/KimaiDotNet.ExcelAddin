using MarkZither.KimaiDotNet.Models;

using Microsoft.Rest;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MarkZither.KimaiDotNet.ExcelAddin.Services
{
    public class KimaiServices : IKimaiServices
    {
        private string Username { get; set; }
        private string Password { get; set; }
        private string ApiUrl { get; set; }
        private HttpClient Client { get; set; }
        public KimaiServices()
        {
            Username = Globals.ThisAddIn.ApiUsername;
            Password = Globals.ThisAddIn.ApiPassword;
            ApiUrl = Globals.ThisAddIn.ApiUrl;
            Client = new HttpClient();
            Client.BaseAddress = new Uri(ApiUrl);
            Client.DefaultRequestHeaders.Add("X-AUTH-USER", Username);
            Client.DefaultRequestHeaders.Add("X-AUTH-TOKEN", Password);
        }
        public KimaiServices(string username, string password, string apiUrl)
        {
            Username = username;
            Password = password;
            ApiUrl = apiUrl;
            Client = new HttpClient();
            Client.BaseAddress = new Uri(ApiUrl);
            Client.DefaultRequestHeaders.Add("X-AUTH-USER", Username);
            Client.DefaultRequestHeaders.Add("X-AUTH-TOKEN", Password);
        }
        public async Task<IList<ProjectCollection>> GetProjects()
        {
            Kimai2APIDocs docs = new Kimai2APIDocs(Client, false);
            var projects = await docs.ListProjectUsingGetAsync(null, null, "3");

            return projects;
        }

        public async Task<IList<CustomerCollection>> GetCustomers()
        {
            Kimai2APIDocs docs = new Kimai2APIDocs(Client, false);
            var customers = await docs.ListCustomersUsingGetAsync("3");

            return customers;
        }

        public async Task<IList<ActivityCollection>> GetActivities()
        {
            Kimai2APIDocs docs = new Kimai2APIDocs(Client, false);
            var activities = await docs.ListActivitiesUsingGetAsync(null, null, "3");

            return activities;
        }
        public async Task<IList<TimesheetCollection>> GetTimesheets()
        {
            Kimai2APIDocs docs = new Kimai2APIDocs(Client, false);
            var timesheets = await docs.ListTimesheetsRecordsUsingGetAsync(null, null, null, null, null, null, null, null, "150", null, "begin", "ASC", DateTime.Now.AddDays(-21).ToString("yyyy-MM-ddT00:00:00")).ConfigureAwait(false);

            return timesheets;
        }

        public async Task<HttpOperationResponse> GetPing()
        {
            Kimai2APIDocs docs = new Kimai2APIDocs(Client, false);
            var ping = await docs.PingWithHttpMessagesAsync();

            return ping;
        }

        public async Task<Models.Version> GetVersion()
        {
            Kimai2APIDocs docs = new Kimai2APIDocs(Client, false);
            var version = await docs.VersionMethodAsync();

            return version;
        }

        public async Task<TimesheetEntity> PostTimesheet(TimesheetEditForm timesheetEditForm)
        {
            Kimai2APIDocs docs = new Kimai2APIDocs(Client, false);
            var timesheet = await docs.CreateTimesheetRecordUsingPostAsync(timesheetEditForm);

            return timesheet;
        }
        public async Task<UserEntity> GetCurrentUser()
        {
            Kimai2APIDocs docs = new Kimai2APIDocs(Client, false);
            var user = await docs.GetCurrentUserUsingGetAsync();

            return user;
        }
    }
}
