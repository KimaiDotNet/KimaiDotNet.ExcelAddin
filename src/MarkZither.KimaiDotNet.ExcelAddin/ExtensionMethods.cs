using Microsoft.Office.Interop.Excel;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkZither.KimaiDotNet.ExcelAddin
{
    public static class ExtensionMethods
    {
        public static string ColumnLetterFromIndex(this int columnId)
        {
            const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            var value = "";

            if (columnId >= letters.Length)
                value += letters[(columnId / letters.Length) - 1];

            value += letters[columnId % letters.Length];

            return value;
        }

        public static void AddDataValidationToColumn(this Range range, Worksheet worksheet, string validationListSheetName, int validationListColumnIndex, int validatedColumnIndex)
        {
            string forumula = $"='{validationListSheetName}'!{(validationListColumnIndex - 1).ColumnLetterFromIndex()}:{(validationListColumnIndex - 1).ColumnLetterFromIndex()}";
            Range cell = worksheet.Range[$"{(validatedColumnIndex - 1).ColumnLetterFromIndex()}2:{(validatedColumnIndex - 1).ColumnLetterFromIndex()}10000"];
            cell.Validation.Delete();
            cell.Validation.Add(XlDVType.xlValidateList, XlDVAlertStyle.xlValidAlertInformation, XlFormatConditionOperator.xlEqual, forumula, Type.Missing);
            cell.Validation.IgnoreBlank = true;
            cell.Validation.InCellDropdown = true;
        }
    }
}
