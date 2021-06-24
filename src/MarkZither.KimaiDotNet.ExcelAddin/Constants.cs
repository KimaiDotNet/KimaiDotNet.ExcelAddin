using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkZither.KimaiDotNet.ExcelAddin
{
    public static class Constants
    {
        public const string FlatListDelimiter = ",";
        public const string Log = "TjKCwsfYmc3K0rDflmF4z5uCwwGxqPfJ73WJFKqPPFmIORwjcnVFlaeLcjm2qSZRZcWFP3cG7L18gTJSkAeYLQ==";
        public static class KimaiSheet
        {
            public const string KimaiSheetName = "Kimai";
        }
        public static class Sheet1
        {
            public const string TimesheetsSheetName = "Sheet1";
            public const int IdColumnIndex = 1;
            public const int DateColumnIndex = 2;
            public const int DurationColumnIndex = 3;
            public const int CustomerColumnIndex = 4;
            public const int ProjectColumnIndex = 5;
            public const int ActivityColumnIndex = 6;
            public const int DescColumnIndex = 7;
            public const int BeginTimeIndex = 8;
            public const int EndTimeIndex = 9;
        }
        public static class CustomersSheet
        {
            public const string CustomersSheetName = "Customers";
            public const int IdColumnIndex = 1;
            public const int NameColumnIndex = 2;
        }
        public static class ProjectsSheet
        {
            public const string ProjectsSheetName = "Projects";
            public const int IdColumnIndex = 1;
            public const int NameColumnIndex = 2;
            public const int CustomerNameColumnIndex = 3;
            public const int CustomerIdColumnIndex = 4;
        }
        public static class ActivitiesSheet
        {
            public const string ActivitiesSheetName = "Activities";
            public const int IdColumnIndex = 1;
            public const int NameColumnIndex = 2;
            public const int ProjectNameColumnIndex = 3;
            public const int ProjectIdColumnIndex = 4;
        }
    }
}
