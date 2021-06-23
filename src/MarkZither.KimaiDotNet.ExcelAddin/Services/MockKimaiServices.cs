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
        private string Username { get; }
        private string Password { get; set; }
        private string ApiUrl { get; set; }
        private HttpClient Client { get; set; }

        readonly Random rnd = new Random(DateTime.Now.Millisecond); //DevSkim: ignore DS148264
        public MockKimaiServices()
        {
            Username = Globals.ThisAddIn.ApiUsername;
            Password = Globals.ThisAddIn.ApiPassword;
            ApiUrl = Globals.ThisAddIn.ApiUrl;
            Client = new HttpClient
            {
                BaseAddress = new Uri(ApiUrl),
            };
            Client.DefaultRequestHeaders.Add("X-AUTH-USER", Username);
            Client.DefaultRequestHeaders.Add("X-AUTH-TOKEN", Password);
        }
        public MockKimaiServices(string username, string password, string apiUrl)
        {
            Username = username;
            Password = password;
            ApiUrl = apiUrl;
            Client = new HttpClient
            {
                BaseAddress = new Uri(ApiUrl),
            };
            Client.DefaultRequestHeaders.Add("X-AUTH-USER", Username);
            Client.DefaultRequestHeaders.Add("X-AUTH-TOKEN", Password);
        }
        public async Task<IList<ProjectCollection>> GetProjects()
        {
            var projects = new List<ProjectCollection>
            {
                new ProjectCollection() { Id = 1, ParentTitle = "cust1", Name = "project 1", Customer = 1 },
                new ProjectCollection() { Id = 2, ParentTitle = "cust2", Name = "project 2", Customer = 2 },
                new ProjectCollection() { Id = 3, ParentTitle = "cust1", Name = "project 3", Customer = 1 },
                new ProjectCollection() { Id = 4, ParentTitle = "cust2", Name = "project 4", Customer = 2 },
            };

            return await Task.FromResult(projects.ToList()).ConfigureAwait(false);
        }
        public async Task<IList<CustomerCollection>> GetCustomers()
        {
            var customers = new List<CustomerCollection>
            {
                new CustomerCollection() { Id = 1, Name = "cust1" },
                new CustomerCollection() { Id = 2, Name = "cust2" },
            };

            return await Task.FromResult(customers).ConfigureAwait(false);
        }
        public async Task<IList<ActivityCollection>> GetActivities()
        {
            var activities = new List<ActivityCollection>
            {
                new ActivityCollection() { Id = 1, Name = "Act1", Project = 1, ParentTitle = "Project 1" },
                new ActivityCollection() { Id = 2, Name = "Act2", Project = 2, ParentTitle = "Project 2" },
                new ActivityCollection() { Id = 3, Name = "Act3NoProject"},
            };
            return await Task.FromResult(activities.ToList()).ConfigureAwait(false);
        }
#pragma warning disable MA0051 // Method is too long
        public async Task<IList<TimesheetCollection>> GetTimesheets()
#pragma warning restore MA0051 // Method is too long
        {
            var timesheets = new List<TimesheetCollection>
            {
                new TimesheetCollection()
                {
                    Id = 1,
                    Activity = 1,
                    Project = 1,
                    Begin = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 8, 0, 0),
                    End = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 9, 0, 0),
                    Duration = 60,
                    Description = "Test Timesheet 1",
                    User = 1,
                },
                new TimesheetCollection()
                {
                    Id = 2,
                    Activity = 2,
                    Project = 2,
                    Begin = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 9, 0, 0),
                    End = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 10, 0, 0),
                    Duration = 60,
                    Description = "Test Timesheet 2",
                    User = 1,
                },
                new TimesheetCollection()
                {
                    Id = 3,
                    Activity = 1,
                    Project = 1,
                    Begin = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 10, 0, 0),
                    End = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 11, 0, 0),
                    Duration = 60,
                    Description = "Test Timesheet 3",
                    User = 1,
                },
                new TimesheetCollection()
                {
                    Id = 4,
                    Activity = 2,
                    Project = 2,
                    Begin = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 11, 0, 0),
                    End = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 12, 0, 0),
                    Duration = 60,
                    Description = "Test Timesheet 4",
                    User = 1,
                },
                new TimesheetCollection()
                {
                    Id = 5,
                    Activity = 1,
                    Project = 1,
                    Begin = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 12, 0, 0),
                    End = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 13, 0, 0),
                    Duration = 60,
                    Description = "Test Timesheet 5",
                    User = 1,
                },
                new TimesheetCollection()
                {
                    Id = 6,
                    Activity = 2,
                    Project = 2,
                    Begin = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 14, 0, 0),
                    End = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 15, 0, 0),
                    Duration = 60,
                    Description = "Test Timesheet 6",
                    User = 1,
                },
            };

            return await Task.FromResult(timesheets).ConfigureAwait(false);
        }

        public Task<HttpOperationResponse> GetPing()
        {
            var ping = new HttpOperationResponse();

            return Task.FromResult(ping);
        }

        public Task<Models.Version> GetVersion()
        {
            var version = new Models.Version() { VersionProperty = "0.0.0.1" };
            return Task.FromResult(version);
        }

        public Task<TimesheetEntity> PostTimesheet(TimesheetEditForm timesheetEditForm)
        {
#pragma warning disable SCS0005 // Weak random number generator.
            int newId = rnd.Next(10, 10000);
#pragma warning restore SCS0005 // Weak random number generator.
            var timesheet = new TimesheetEntity()
            {
                Id = newId,
                Activity = timesheetEditForm.Activity,
                Project = timesheetEditForm.Project,
                User = timesheetEditForm.User,
                Begin = timesheetEditForm.Begin,
                End = timesheetEditForm.End,
                Description = timesheetEditForm.Description,
            };

            return Task.FromResult(timesheet);
        }

        public Task<UserEntity> GetCurrentUser()
        {
            var user = new UserEntity()
            {
                Id = 5,
            };

            return Task.FromResult(user);
        }
    }
}
