using MarkZither.KimaiDotNet.Models;

using Microsoft.Rest;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MarkZither.KimaiDotNet.ExcelAddin.Services
{
    public class MockKimaiServices : IKimaiServices
    {
        private string Username { get; set; }
        private string Password { get; set; }
        private string ApiUrl { get; set; }
        private HttpClient Client { get; set; }

        readonly Random rnd = new Random(DateTime.Now.Millisecond); //DevSkim: ignore DS148264
        public MockKimaiServices()
        {
            Username = Globals.ThisAddIn.ApiUsername;
            Password = Globals.ThisAddIn.ApiPassword;
            ApiUrl = Globals.ThisAddIn.ApiUrl;
            Client = new HttpClient();
            Client.BaseAddress = new Uri(ApiUrl);
            Client.DefaultRequestHeaders.Add("X-AUTH-USER", Username);
            Client.DefaultRequestHeaders.Add("X-AUTH-TOKEN", Password);
        }
        public MockKimaiServices(string username, string password, string apiUrl)
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
            var projects = new List<ProjectCollection>();
            projects.Add(new ProjectCollection() { Id = 1, ParentTitle = "cust1", Name = "project 1", Customer = 1});
            projects.Add(new ProjectCollection() { Id = 2, ParentTitle = "cust2", Name = "project 2"});

            return await Task.FromResult(projects).ConfigureAwait(false);
        }

        public async Task<IList<CustomerCollection>> GetCustomers()
        {
            var customers = new List<CustomerCollection>();
            customers.Add(new CustomerCollection() { Id = 1, Name = "cust1" });
            customers.Add(new CustomerCollection() { Id = 2, Name = "cust2" });

            return await Task.FromResult(customers).ConfigureAwait(false);
        }

        public async Task<IList<ActivityCollection>> GetActivities()
        {
            var activities = new List<ActivityCollection>();
            activities.Add(new ActivityCollection() {Id = 1, Name = "Act1", Project = 1, ParentTitle = "Project 1" });
            activities.Add(new ActivityCollection() {Id = 2, Name = "Act2" });

            return await Task.FromResult(activities).ConfigureAwait(false);
        }
        public async Task<IList<TimesheetCollection>> GetTimesheets()
        {
            var timesheets = new List<TimesheetCollection>();
            timesheets.Add(new TimesheetCollection() {Id = 1,
                Activity = 1,
                Project = 1,
                Begin = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 8, 0, 0),
                End = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 9, 0, 0),
                Description = "Test Timesheet 1",
                User = 1 });
            timesheets.Add(new TimesheetCollection() {Id = 2,
                Activity = 2,
                Project = 2,
                Begin = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 9, 0, 0),
                End = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 10, 0, 0),
                Description = "Test Timesheet 2",
                User = 1 });

            return await Task.FromResult(timesheets).ConfigureAwait(false);
        }

        public Task<HttpOperationResponse> GetPing()
        {
            var ping = new HttpOperationResponse();

            return Task.FromResult(ping);
        }

        public Task<Models.Version> GetVersion()
        {
            Kimai2APIDocs docs = new Kimai2APIDocs(Client, false);
            var version = new Models.Version() {VersionProperty = "0.0.0.1" };

            return Task.FromResult(version);
        }

        public Task<TimesheetEntity> PostTimesheet(TimesheetEditForm timesheetEditForm)
        {
            int newId = rnd.Next(10, 10000);
            var timesheet = new TimesheetEntity() { Id = newId,
                Activity = timesheetEditForm.Activity,
                Project = timesheetEditForm.Project,
                User = timesheetEditForm.User,
                Begin = timesheetEditForm.Begin,
                End = timesheetEditForm.End,
            Description = timesheetEditForm.Description};

            return Task.FromResult(timesheet);
        }
    }
}
