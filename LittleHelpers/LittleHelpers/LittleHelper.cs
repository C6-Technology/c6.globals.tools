using System;
using System.Data;
using System.Diagnostics;

/// <summary>
/// LittleHelpers is a class containing diverse "tools" and other "hacks" to make the developers life easier
/// </summary>

namespace C6.Globals.Tools
{
    /// <summary>
    ///
    /// </summary>
    public partial class LittleHelpers
    {
        #region Global Variables

        #region Variable definition

        #endregion Variable definition

        #region Properties

        #endregion Properties

        #endregion Global Variables

        #region Global Delegates and Events

        #endregion Global Delegates and Events

        #region (De-)Constructur

        /// <summary>
        ///
        /// </summary>
        public LittleHelpers()
        {
        }

        /// <summary>
        ///
        /// </summary>
        ~LittleHelpers()
        {
        }

        #endregion (De-)Constructur

        #region SQL related

        /// <summary>
        /// This method is depreceated since there are meanwhile better, native methods existing.
        /// </summary>
        /// <param name="EscapeMe">The string to be checked and escaped</param>
        /// <returns></returns>
        public string GetEscapedSQLValue(string EscapeMe)
        {
            string returnValue = EscapeMe.Replace("'", "''").Replace("’", "’’").Replace("‘", "‘‘");

            return returnValue;
        }

        #endregion SQL related

        #region DebugPrinting

        public void DebugPrint(bool condition, string Message)
        {
#if DEBUG
            Debug.WriteLineIf(condition, CreateDebugMessageString(Message));
#endif
        }

        public string CreateDebugMessageString(string Message)
        {
            return DateTime.Now.ToString() + " || " + Message;
        }

        #endregion DebugPrinting

        #region Log

        public delegate void AddLogMessageDelegate(bool condition, string message);

        public AddLogMessageDelegate AddLogMessageValue;

        private void AddLogMessage(bool condition, string message)
        {
            if (AddLogMessageValue != null)
                AddLogMessageValue(condition, message);
            else
                DebugPrint(condition, message);
        }

        #endregion Log

        #region Array related methods

        #region Method StringArray2String

        /// <summary>
        /// A simple function to "flaten" a string array into a single string. The function simply add's each field and delimites it by using ||.
        /// </summary>
        /// <param name="Array2Convert">A one dimensional string array.</param>
        /// <returns>A string with delimiter of || for each array field.</returns>
        public string StringArray2String(string[] Array2Convert)
        {
            if (Array2Convert.Rank > 1)
                throw new Exception("StringArray2String only handles one dimensional arrays");
            else
            {
                string returnValue = "";    // initializing returnValue

                foreach (string str in Array2Convert)
                    returnValue += str + "||";

                return returnValue.Substring(0, returnValue.Length - 2);    // return everything but the last two ||
            }
        }

        #endregion Method StringArray2String

        #endregion Array related methods

        #region DebugPrintDataTable

        // TODO: rewrite this to use it in conjunction with a filewriter.

        /// <summary>
        /// A simple "snippet" to DebugPrint DataTable content.
        ///
        /// Using this function does deliver the table content delimited by ","!
        /// </summary>
        /// <param name="PrintMe">The DataTable that should be outputted</param>
        public void DebugPrintDataTable(DataTable PrintMe)
        {
            this.DebugPrintDataTable(PrintMe, "");
        }

        /// <summary>
        /// A simple "snippet" to DebugPrint DataTable content
        /// </summary>
        /// <param name="PrintMe">The DataTable that should be outputted</param>
        /// <param name="ColumnDelimiter">The column delimiter, if any. Setting this to "" will result in "," as default delimiter!</param>
        public void DebugPrintDataTable(DataTable PrintMe, string ColumnDelimiter)
        {
            // for all the output, "," is the default column delimiter
            string columnDelimiter = "\",\"";

            if (ColumnDelimiter != "")				// well, if we need something else, here we go :)
                columnDelimiter = ColumnDelimiter;

            // tell us what you do
#if DEBUG
            Debug.Print("DebugPrintDataTable: Tablename: " + PrintMe.TableName);
            Debug.Print("");
#endif

            string outPut = "";	// helper variable

            // then, deploy the column headers first
            if (ColumnDelimiter == "")
                outPut += "\"";	// start the line with a "

            for (int x = 0; x < PrintMe.Columns.Count; x++)
            {
                outPut += PrintMe.Columns[x].Caption.ToString() + columnDelimiter;
            }

            if (ColumnDelimiter == "")
                outPut = outPut.Substring(0, outPut.Length - 2);						// remove the last ," to have a correct line end;
            else
                outPut = outPut.Substring(0, outPut.Length - columnDelimiter.Length);	// remove the entire last content delimiter to have a correct line end;
#if DEBUG
            Debug.WriteLine(outPut);
#endif

            // followed by the content :)
            outPut = "";
            if (ColumnDelimiter == "")
                outPut += "\"";	// start the line with a "

            for (int x = 0; x <= PrintMe.Rows.Count - 1; x++)
            {
                DataRow dr = PrintMe.Rows[x];
                for (int q = 0; q <= dr.ItemArray.GetUpperBound(0); q++)
                {
                    outPut += dr.ItemArray.GetValue(q).ToString() + columnDelimiter;
                }

                if (ColumnDelimiter == "")
                    outPut = outPut.Substring(0, outPut.Length - 2);						// remove the last ," to have a correct line end;
                else
                    outPut = outPut.Substring(0, outPut.Length - columnDelimiter.Length);	// remove the entire last content delimiter to have a correct line end;
#if DEBUG
                Debug.WriteLine(outPut);
#endif
            }
            // and done
        }

        #endregion DebugPrintDataTable
    }
}