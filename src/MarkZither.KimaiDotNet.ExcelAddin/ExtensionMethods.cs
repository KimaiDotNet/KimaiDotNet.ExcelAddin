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
                value += letters[columnId / letters.Length - 1];

            value += letters[columnId % letters.Length];

            return value;
        }
    }
}
