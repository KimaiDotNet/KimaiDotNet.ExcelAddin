using Microsoft.Office.Interop.Excel;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MarkZither.KimaiDotNet.ExcelAddin.Sheets
{
    public sealed class Sheet1
    {
        private static readonly Sheet1 instance = new Sheet1();
        public readonly Worksheet Worksheet;

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static Sheet1()
        {
        }

        private Sheet1()
        {
            var sheet1Worksheet =
                Globals.ThisAddIn.Application.Worksheets.Cast<Worksheet>()
                       .SingleOrDefault(w => string.Equals(w.Name, ExcelAddin.Constants.Sheet1.TimesheetsSheetName, StringComparison.OrdinalIgnoreCase));
            if(sheet1Worksheet is Worksheet)
            {
                Worksheet = sheet1Worksheet;
            }
            else
            {
                sheet1Worksheet = (Microsoft.Office.Interop.Excel.Worksheet)Globals.ThisAddIn.Application.Worksheets.Add(Missing.Value, Globals.ThisAddIn.Application.ActiveSheet as Worksheet);
                sheet1Worksheet.Name = ExcelAddin.Constants.Sheet1.TimesheetsSheetName;
                Worksheet = sheet1Worksheet;
            }
        }

        public static Sheet1 Instance
        {
            get
            {
                return instance;
            }
        }
    }
}
