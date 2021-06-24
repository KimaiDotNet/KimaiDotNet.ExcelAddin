using MarkZither.KimaiDotNet.Models;

using Microsoft.Rest;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace MarkZither.KimaiDotNet.ExcelAddin.Services
{
    public interface IKimaiServices
    {
        Task<IList<ProjectCollection>> GetProjects();
        Task<IList<CustomerCollection>> GetCustomers();
        Task<IList<ActivityCollection>> GetActivities();
        Task<IList<TimesheetCollection>> GetTimesheets();
        Task<HttpOperationResponse> GetPing();
        Task<Models.Version> GetVersion();
        Task<TimesheetEntity> PostTimesheet(TimesheetEditForm timesheetEditForm);
        Task<UserEntity> GetCurrentUser();
    }
}