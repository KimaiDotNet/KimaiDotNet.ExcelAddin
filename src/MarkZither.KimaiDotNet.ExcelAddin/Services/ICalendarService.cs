using MarkZither.KimaiDotNet.ExcelAddin.Models.Calendar;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkZither.KimaiDotNet.ExcelAddin.Services
{
    public interface ICalendarService
    {
        Categories GetCategories();
    }
}
