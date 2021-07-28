using MarkZither.KimaiDotNet.ExcelAddin.Models.Calendar;

using Microsoft.Exchange.WebServices.Data;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkZither.KimaiDotNet.ExcelAddin.Services
{
    public interface ICalendarService
    {
        IList<Appointment> GetAppointments(DateTime startDate, DateTime endDate);
        Categories GetCategories();
    }
}
