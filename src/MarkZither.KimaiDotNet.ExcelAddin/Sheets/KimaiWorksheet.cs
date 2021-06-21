using MarkZither.KimaiDotNet.ExcelAddin.Services;
using MarkZither.KimaiDotNet.Models;

using Microsoft.Extensions.Logging;
using Microsoft.Office.Interop.Excel;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MarkZither.KimaiDotNet.ExcelAddin.Sheets
{
    public sealed class KimaiWorksheet
    {
        private static readonly KimaiWorksheet instance = new KimaiWorksheet();
        public readonly Worksheet Worksheet;

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static KimaiWorksheet()
        {
        }

        private KimaiWorksheet()
        {
            Worksheet currentSheet = Globals.ThisAddIn.Application.ActiveSheet as Microsoft.Office.Interop.Excel.Worksheet;

            var kimaiWorksheet =
                Globals.ThisAddIn.Application.Worksheets.Cast<Worksheet>()
                       .SingleOrDefault(w => string.Equals(w.Name, ExcelAddin.Constants.KimaiSheet.KimaiSheetName, StringComparison.OrdinalIgnoreCase));
            if (kimaiWorksheet is Worksheet)
            {
                Worksheet = kimaiWorksheet;
            }
            else
            {
                kimaiWorksheet = (Microsoft.Office.Interop.Excel.Worksheet)Globals.ThisAddIn.Application.Worksheets.Add(Missing.Value, Globals.ThisAddIn.Application.ActiveSheet as Worksheet);
                kimaiWorksheet.Name = ExcelAddin.Constants.KimaiSheet.KimaiSheetName;
                kimaiWorksheet.Visible = XlSheetVisibility.xlSheetVeryHidden; 
                Worksheet = kimaiWorksheet;
                // switch straight back to the current active sheet
                currentSheet.Select();
            }
        }

        public static KimaiWorksheet Instance
        {
            get
            {
                return instance;
            }
        }

        public void SetSyncDate()
        {
            ((Range)Worksheet.Cells[1, 1]).Value2 = DateTime.Now.ToOADate();
        }

        public bool isInitialized()
        {
            if(((Range)Worksheet.Cells[1, 1]).Value2 is null)
            {
                return false;
            }
            return true;
        }
    }
}
