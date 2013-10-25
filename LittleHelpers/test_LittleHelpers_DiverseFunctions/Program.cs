using System;

/*
 * the following is not mandatory and here used only for some stat's and encoding stuff
 */
// only needed for some stat's and encoding stuff
// only needed for some stat's and encoding stuff
// only needed for some stat's and encoding stuff
// only needed for some stat's and encoding stuff

namespace C6.Globals.Tools
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            LittleHelpers littleHelpers = new LittleHelpers();
            LittleHelpers.ExcelTools excelTools = new LittleHelpers.ExcelTools();

            Console.WriteLine(excelTools.GetNextColumnName("AA", LittleHelpers.ExcelTools.ExcelVersion.Excel2010, LittleHelpers.ExcelTools.GetColumnNameDirection.Right));
            Console.WriteLine(excelTools.GetNextColumnName("AAA", LittleHelpers.ExcelTools.ExcelVersion.Excel2010, LittleHelpers.ExcelTools.GetColumnNameDirection.Left));
        }
    }
}