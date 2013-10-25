using System;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace C6.Globals.Tools
{
    public partial class LittleHelpers
    {
        /// <summary>
        ///
        /// </summary>
        public class CSVReader
        {
            #region Global Variables

            #region Variable definition

            private string csvSeparator = Properties.Settings.Default.DefaultCSVSeparator;
            private LittleHelpers littleHelpers = new LittleHelpers();

            #endregion Variable definition

            #region Properties

            /// <summary>
            /// The Separator used as default if no Separator is used for all CSV related functions in this class!
            /// </summary>
            public string CSVSeparator
            {
                get
                {
                    return csvSeparator;
                }
                set
                {
                    csvSeparator = value.ToString();
                }
            }

            #endregion Properties

            #endregion Global Variables

            #region Global Delegates and Events

            public delegate void EventHandler(object sender, EventArgs eA);

            /// <summary>
            ///
            /// </summary>
            public event EventHandler csvRowImported;

            #endregion Global Delegates and Events

            #region CSV- Handling

            #region Seperated HeaderColumnHandler

            /// <summary>
            /// HeaderColumnHandler delivers a string[] or DataTable (with setup columns) depending on Headline.
            /// Delivers a null if an error occured (usually this is a doubled column within the Headline.
            ///
            /// Note1:  DataTable.Clear() & DataTable.Columns.Clear() is executed, so an created but empty table is
            ///         enough for the reference table :)
            ///
            /// Note2:  In the case that a Column is repeadeately existing within a Headline,
            ///         we consequently add a numbering, starting with 1 for the first doublette
            ///         to the name of the Column.
            /// </summary>
            /// <typeparam name="ReturnValueType">Either a DataTable or string[], anything else would result in an Exception!</typeparam>
            /// <param name="Path">The path to the CSV- or XML- file from which the header should be retrieved!</param>
            /// <param name="Headline">A string containing the header line.</param>
            /// <param name="Seperator">A string array containing the seperator data</param>
            /// <param name="Encode">The encoding of the file!</param>
            /// <returns>
            ///        Depending on ReturnValueType returns a DataTable or a string array contained in an object.
            ///        NULL if an error occours, usually this error means that a doubled header was contained in the Headline!
            /// </returns>

            public void GetCSVColumns<ReturnValueType>(string Path, string Headline, string[] Seperator, Encoding Encode, ref object ReturnValue)
            {
                // TODO: error handling has to be implemented !!
                // TODO: Handling XML- Files has to be implemented!!
                if (Path.ToLower().EndsWith(".xml"))
                    throw new Exception("XML Handling not yet implemented!");

                //MessageBox.Show("Checking whether the file exists!");
                if (File.Exists(Path))
                {
                    //MessageBox.Show("Opening CSV: " + Path);
                    string headLine = "";

                    using (StreamReader sr = new StreamReader(Path, Encode))
                    {
                        headLine = sr.ReadLine().TrimStart('\"').TrimEnd('\"');
                        // we have to remove the first and last " since it doesn't belong to the lines and was added just as start- and end- delimiter
                        sr.Close(); // we do not need the file anymore :)
                    }

                    if (typeof(ReturnValueType) == typeof(string[]))
                    {
                        string[] retValue = new string[1000];
                        GetCSVColumns(headLine, Seperator, ref retValue);

                        ReturnValue = retValue;
                    }

                    if (typeof(ReturnValueType) == typeof(DataTable))
                    {
                        DataTable retValue = new DataTable();
                        GetCSVColumns(headLine, Seperator, ref retValue);

                        ReturnValue = retValue;
                    }

                    if (typeof(ReturnValueType) == typeof(OrderedDictionary))
                    {
                        OrderedDictionary retValue = new OrderedDictionary();
                        GetCSVColumns(headLine, Seperator, ref retValue);

                        ReturnValue = retValue;
                    }
                }
                else
                {
                    //MessageBox.Show("File {0} doesn't exist!", Path);
                    throw new IOException(string.Format("File {0} doesn't exist!", Path));
                }
            }

            /// <summary>
            /// This function delivers the separated columns as an DataTable.
            ///
            /// Note1:  DataTable.Clear() & DataTable.Columns.Clear() is executed, so an created but empty table is
            ///         enough for the reference table :)
            ///
            /// Note2:  In the case that a Column is repeadeately existing within a Headline,
            ///         we consequently add a numbering, starting with 1 for the first doublette
            ///         to the name of the Column.
            /// </summary>
            /// <param name="Headline">The string with the headline itself.</param>
            /// <param name="Seperator">A string array with all separating parameters</param>
            /// <param name="ReturnValue">Returns the headline within an Hashtable</param>
            public void GetCSVColumns(string Headline, string[] Seperator, ref DataTable ReturnValue)
            {
                ReturnValue.Clear();            // clearing all data out of the referenced ReturnValue
                ReturnValue.Columns.Clear();    // clears all existing columns out of the referenced ReturnValue

                OrderedDictionary zw = new OrderedDictionary();
                GetCSVColumns(Headline, Seperator, ref zw);

                foreach (string KeyName in zw.Keys)
                    ReturnValue.Columns.Add(new DataColumn(zw[KeyName].ToString()));

                // and done :)
            }

            /// <summary>
            /// This function delivers the separated columns as an string[] array
            ///
            /// Note:   In the case that a Column is repeadeately existing within a Headline,
            ///         we consequently add a numbering, starting with 1 for the first doublette
            ///         to the name of the Column.
            /// </summary>
            /// <param name="Headline">The string with the headline itself.</param>
            /// <param name="Seperator">A string array with all separating parameters</param>
            /// <param name="ReturnValue">Returns the headline within an Hashtable</param>
            public void GetCSVColumns(string Headline, string[] Seperator, ref string[] ReturnValue)
            {
                OrderedDictionary zw = new OrderedDictionary();
                GetCSVColumns(Headline, Seperator, ref zw);

                string[] retValue = new string[zw.Count];

                int x = 0;
                foreach (string KeyName in zw.Keys)
                {
                    retValue[x] = zw[KeyName].ToString();
                    x++;
                }

                if (retValue.Length > ReturnValue.Length)
                    throw new Exception("Developer Error: GetCSVColumns - string[] ReturnValue is undersized! A size of " + retValue.Length.ToString());

                ReturnValue = retValue;
            }

            /// <summary>
            /// This function delivers the separated columns as an Hastable
            /// </summary>
            /// <param name="Headline">The string with the headline itself.</param>
            /// <param name="Seperator">A string array with all separating parameters</param>
            /// <param name="ReturnValue">Returns the headline within an Hashtable</param>
            /// <param name="RemoveEmptyEntries">Default value is StringSplitOptions.None</param>
            public void GetCSVColumns(string Headline, string[] Seperator, ref OrderedDictionary ReturnValue, StringSplitOptions RemoveEmptyEntries = StringSplitOptions.None)
            {
                if (Headline.Length == 0)
                    throw new Exception("Developer Error: Headline MUST NOT be empty!");

                if (Seperator.Length == 0)
                    throw new Exception("Developer Error: Separator MUST NOT be empty!");

                string[] HeaderColumns = Headline.Split(Seperator, RemoveEmptyEntries);

                OrderedDictionary HeadlineColumns = new OrderedDictionary(HeaderColumns.GetUpperBound(0));
                /*
                 * Note:    In the case that a Column is repeadeately existing within a Headline,
                 *          we consequently add a numbering, starting with 1 for the first doublette
                 *          to the name of the Column.
                 */
                try
                {
                    long ColumnCounter = 0;

                    foreach (string header in HeaderColumns)
                    {
                        string Header2Add = header;
                        ColumnCounter += 1;

                        if (HeadlineColumns.Contains(Header2Add))    // If the header exists, we do add a number to it :)
                        {
                            /*
                             * instead of handling the number for each header we do simpy run through a while-loop and increase
                             * the counter as much as needed. This is definetely not the nicest approach, but efficient since it
                             * isn't expected to approach a super large headline nor a developer entirely uncapable to handle his data ...
                             */
                            int orgHeaderCounter = 1;

                            while (HeadlineColumns.Contains(header))
                            {
                                if (HeadlineColumns.Contains(header + orgHeaderCounter.ToString()))
                                    orgHeaderCounter++;
                                else
                                {
                                    Header2Add += orgHeaderCounter.ToString();
                                    break;
                                }
                            }
                        }
                        HeadlineColumns.Add(Header2Add, Header2Add);
                    }
                }
                catch (Exception e)                // since we expect only one issue, we'll just make the table null and raise NO warning
                // TODO: Somehow an error should be raised to give the calling routine the chance to handle it.
                {
                    //MessageBox.Show(e.ToString());
                    ReturnValue = null; // ensures that the default value for the valuetype is delivered back in case of an error :)
                }

                //MessageBox.Show("HeadlineColumns.Values.Count = " + HeadlineColumns.Values.Count);
                ReturnValue = HeadlineColumns;
            }

            #endregion Seperated HeaderColumnHandler

            #region Read

            /// <summary>
            /// This method creates a DataTable from a CSV.
            ///
            /// If within the CSV headers are existing and if one column is doubled, this method returns an empty DataTable and raises NO error.
            ///
            /// This method assumes that Headlines, if any, aren't ignored per default!
            /// </summary>
            /// <param name="Path">The full path to the CSV.</param>
            /// <param name="Separator">A string array containing the separator(s).</param>
            /// <param name="Encode">The encoding of the file!</param>
            /// <param name="HasHeadline">true ~ has headlines; false ~ hos none</param>
            /// <param name="TableName">If wished, the data table object gets a table name :)</param>
            /// <returns>A DataTable object, representing the data from the CSV.
            /// </returns>
            public DataTable Read(string Path, string[] Separator, Encoding Encode, bool HasHeadline, string TableName)
            {
                return Read(Path, Separator, Encode, HasHeadline, TableName, false, true);
            }

            /// <summary>
            /// This method creates a DataTable from a CSV.
            ///
            /// If within the CSV headers are existing and if one column is doubled, this method returns an empty DataTable and raises NO error.
            ///
            /// This method assumes that Headlines, if any, aren't ignored per default!
            ///
            /// NOTE:	If csvRowImported != null, meaning the csvRowImported event has been consumed, DataTable will be empty but initialized
            ///			and the event handler will receive the string array to be handled at this place.
            /// </summary>
            /// <param name="Path">The full path to the CSV.</param>
            /// <param name="Separator">A string array containing the separator(s).</param>
            /// <param name="Encode">The encoding of the file!</param>
            /// <param name="HasHeadline">true ~ has headlines; false ~ hos none</param>
            /// <param name="TableName">If wished, the data table object gets a table name :)</param>
            /// <param name="ignoreHeadline">if true, the FIRST line of each file is ALWAYS ignored. If false && HasHeadline == true, the first line is interpreted as headline.</param>
            /// <param name="CreateDataTableAlways">if true, the DataTable (returnValue) is always created, if false, it is only created if LittleHelpers.csvRowImported is NOT consumed!</param>
            /// <returns>
            /// A DataTable object, representing the data from the CSV.
            /// </returns>
            public DataTable Read(string Path, string[] Separator, Encoding Encode, bool HasHeadline, string TableName, bool ignoreHeadline, bool CreateDataTableAlways)
            {
                return Read(Path, Separator, Encode, HasHeadline, TableName, ignoreHeadline, CreateDataTableAlways, 0);
            }

            /// <summary>
            /// This method creates a DataTable from a CSV.
            ///
            /// If within the CSV headers are existing and if one column is doubled, this method returns an empty DataTable and raises NO error.
            ///
            /// This method assumes that Headlines, if any, aren't ignored per default!
            ///
            /// NOTE:	If csvRowImported != null, meaning the csvRowImported event has been consumed, DataTable will be empty but initialized
            ///			and the event handler will receive the string array to be handled at this place.
            /// </summary>
            /// <param name="Path">The full path to the CSV.</param>
            /// <param name="Separator">A string array containing the separator(s).</param>
            /// <param name="Encode">The encoding of the file!</param>
            /// <param name="HasHeadline">true ~ has headlines; false ~ hos none</param>
            /// <param name="TableName">If wished, the data table object gets a table name :)</param>
            /// <param name="ignoreHeadline">if true, the FIRST line of each file is ALWAYS ignored. If false && HasHeadline == true, the first line is interpreted as headline.</param>
            /// <param name="CreateDataTableAlways">if true, the DataTable (returnValue) is always created, if false, it is only created if LittleHelpers.csvRowImported is NOT consumed!</param>
            /// <param name="csvColumnCounter">If HasHeadline is true, this parameter is ignored. If HasHeadline is true, this parameter must be greater than 0, else an exception is raised.</param>
            /// <returns>
            /// A DataTable object, representing the data from the CSV.
            /// </returns>
            public DataTable Read(string Path, string[] Separator, Encoding Encode, bool HasHeadline, string TableName, bool ignoreHeadline, bool CreateDataTableAlways, int ColumnCounter)
            {
                // initializing some worker varialbes.
                DataTable retValue = new DataTable();
                bool stop = false;

                bool isChildLine = false;						// this is used if the importer determined that a line is belonging to the line before (f.e. if CR(&/|)LF is not correctly escaperd);
                string[] csvColumns = new string[0];			// initialize the csvColumns placeholder
                string currentColumn = "";						// initialize the current column string
                int csvColumnCounter = 0;			            // initializing the column counter
                string fullSeperator = "";						// initialize the full seperator

                if (HasHeadline)                                // checking whether the HasHeadline :: ColumnCounter conditions are obeyed.
                    csvColumnCounter = 0;
                else
                    if (ColumnCounter <= 0)                     // TODO: Implement a function that is capable to figure out the number of columns automatically. A complicated task which should implement the probing on several rows - maybe by allowing to pass additional parameter(s), telling the number of probes and whether to take them per random or first lines ...
                        throw new Exception("LittleHelper.CSVReader:Developer Error: HasHeadline has been set to false and ColumnCounter is <= 0! Please deliver the number of columns or set HasHeadline to true!");
                    else
                        csvColumnCounter = ColumnCounter;

                for (int x = 0; x <= Separator.GetUpperBound(0); x++)		// get one seperator string
                    fullSeperator += Separator[x];

                if (TableName != "")        // well, we set the table name only if we got one :)
                    retValue.TableName = TableName;

                //MessageBox.Show("Reading data into table: " + TableName);

                // TODO: error handling has to be implemented !!
                //MessageBox.Show("Checking whether the file exists!");
                if (File.Exists(Path))
                {
                    //MessageBox.Show("Opening CSV: " + Path);

                    using (StreamReader sr = new StreamReader(Path, Encode))
                    {
#if DEBUG
                        littleHelpers.DebugPrint(true, "EndOfStream: " + sr.EndOfStream);
                        Thread.Sleep(2000);
#endif
                        if (!sr.EndOfStream)    // well, it may appear that the end of the stream is reached before we
                        // even start (which is if the file itself is empty!
                        {
                            if ((HasHeadline) && (!ignoreHeadline))        // if so we create the column headers in our table :)
                            {
                                string headLine = sr.ReadLine().TrimStart('\"').TrimEnd('\"');  // we have to remove the first and last " since it doesn't belong to the lines and was added just as start- and end- delimiter
                                GetCSVColumns(headLine, Separator, ref retValue);
#if DEBUG
                                littleHelpers.DebugPrint(true, "GetCSVColumn Result: " + retValue);
#endif
                            }

                            // well, now let's do the real, importing work

                            while ((sr.Peek() > -1) && (stop == false))        // if stop == true (which is the case if a double header was found, we do not go on here
                            {
                                // string csvRow = sr.ReadLine().TrimStart('\"').TrimEnd('\"');  // we have to remove the first and last " since it doesn't belong to the lines and was added just as start- and end- delimiter;
                                // string[] csvColumns = csvRow.Split(Separator, StringSplitOptions.None); // it may occour that we have empty columns - we can not delete them and have to empty insert them !

                                string csvRow = sr.ReadLine();		// we read it line by line

#if DEBUG
                                littleHelpers.DebugPrint(true, csvRow);
#endif

                                if (!isChildLine)					// the following is done ONLY if we are not working on a child line
                                    csvColumns = new string[0];		// initializng the base string containing the data

                                /*
                                    Figuring out whether we can simply split, use the RegExpression or need to go throught the hard way
                                */
                                if (!csvRow.Contains("\""))
                                {
                                    csvColumns = csvRow.Split(Separator, StringSplitOptions.None);              // it may occour that we have empty columns - we can not delete them (and ignore them therefore) but have
                                    // to insert them as empty as they are!
                                }
                                else
                                {
                                    /*
                                     * create the regular expression and compile it
                                     */
                                    Regex getColumnsRegEx = new Regex("(\"*?\"|[^,]+|'.*?')", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

                                    // now get all matches
                                    MatchCollection mc = getColumnsRegEx.Matches(csvRow);

                                    /*
                                     * Check whether the result of the regular expression is corresponding to the number of columns we do have to handle
                                     * Don't check if we handle a child line.
                                     *
                                     * NOTE:
                                     * If no header columns do exist, we simple handle each line without the chance to speed things up with Regex
                                     */
                                    if ((mc.Count == retValue.Columns.Count) && (!isChildLine))	// TODO: Check how to handle this if no Headercolumn exist!
                                    {
                                        // initialize the string array
                                        csvColumns = new string[mc.Count];
                                        int x = 0;

                                        // and fill it
                                        foreach (Match m in mc)
                                        {
                                            csvColumns[x] = m.Value.ToString();
                                            x++;
                                        }

#if DEBUG
                                        littleHelpers.DebugPrint(true, string.Format("\r\n\r\nReg-Expression result: {0} columns", csvColumns.GetUpperBound(0)));
                                        //Thread.Sleep(2000);
#endif
                                    }
                                    else
                                    {
                                        /*
                                         * Obviousely not, therefore we have to handle all cases step by step - this is the slowest process
                                         *
                                         * Now handling the following cases:
                                         *
                                         * 1)	__,__
                                         * 2)	"__","__"
                                         * 3)	"__",__
                                         * 4)	__,"__"
                                         * 5)	"_""_""_"				"" escaped within a "__"
                                         * 6)	""")					"" escaped with left or right side
                                         * 6.1)	"("")					(left side "" escaped)
                                         * 6.2)	("")"					(right side "" escaped)
                                         * 7)	"_"",""_"				escaped "" with comma included
                                         * 7.1)	"__""__,__""__"
                                         *
                                         */

                                        bool ignoreSeperator = false;		// true if the seperator should be ignored, false if not. The seperator is ignored f.e. if we are navigating within an identified string ;)

                                        if (!isChildLine)
                                        {
                                            currentColumn = "";				// only clear the current column if we are not working on a child line
                                            csvColumns = new string[0];		// initialize the array only if we are not working on a child line
                                            csvColumnCounter = 0;			// initialize the column counter only if we are not working on a child line
                                        }

                                        /*
                                         * TODO:    For now, ONLY seperators with 1 char are handled! Seperators might have MORE than 1 char... in terms of the so far handled rules, this could even be faster, e.g. we could
                                         *          rules based seperators into different tasks and work on the whole thing in parallel...
                                         */
                                        for (int x = 0; x < csvRow.Length; x++)
                                        {
                                            string currentChar = csvRow[x].ToString();
                                            CSVSeperatorHandler(ref x, ref csvRow, ref currentChar, fullSeperator, ref ignoreSeperator, ref csvColumnCounter, ref csvColumns, ref currentColumn, retValue.Columns.Count);

                                            // check whether we have a "
                                            if (currentChar == "\"")
                                            {
                                                if ((x + 1) >= csvRow.Length)		// is the next char end of line?
                                                {
                                                    ignoreSeperator = false;		// reset the ignoreSeperator to false
                                                    break;							// and break
                                                }
                                                else								// nope, we can go on, let's see what comes next
                                                {
                                                    //if (ignoreSeperator == false)	// well, the 'first' " we found, therefore we ignore the seperator from now on
                                                    //{
                                                    //ignoreSeperator = true;

                                                    if (csvRow[x + 1].ToString() == "\"")				// is the following char a " too?
                                                    {
                                                        if ((x + 2) <= csvRow.Length)					// yes? and is there another char behind it?
                                                        {
                                                            if (csvRow[x + 2].ToString() == "\"")		// and is the overnext a " as well?
                                                            {
                                                                // we've identified a """ :)
                                                                ignoreSeperator = ignoreSeperator == true ? false : true;	// well, than let's alternate the ignoreSeperator - this handles the left/right thematic automatically 8-)
                                                                currentColumn += "\"";										// and add a " to the current column

                                                                x = x + 2;													// and go on with the third char :)
                                                            }
                                                            else										// well, we found a double "" which means ...
                                                            {
                                                                if (ignoreSeperator)					// ... we found an " escape sequence ("") within a string column
                                                                {
                                                                    currentColumn += "\"";
                                                                    /*
                                                                     * Speed optimization. We are going to copy all chars up until the next "
                                                                     */
                                                                    x++;
                                                                    currentColumn += csvRow.IndexOf('\"', x + 1) - x - 1 > 0 ? csvRow.Substring(x + 1, csvRow.IndexOf('\"', x + 1) - x - 1) : "";
                                                                    //x = x + csvRow.Substring(x + 1, csvRow.IndexOf('\"', x + 1) - x - 1).Length;
                                                                    x = x + (csvRow.IndexOf('\"', x + 1) - x - 1 > 0 ? csvRow.Substring(x + 1, csvRow.IndexOf('\"', x + 1) - x - 1).Length : 0);
                                                                }
                                                                else
                                                                    x++;								// ... we found an empty column and therefore can go directly to the next char
                                                            }
                                                        }
                                                        else
                                                        {
                                                            ignoreSeperator = false;
#if DEBUG
                                                            littleHelpers.DebugPrint(true, "LittleHelper.CSV2DataTable: ERROR in CSV, line ends with \"\"");
                                                            Thread.Sleep(2000);
#endif
                                                            throw new Exception("LittleHelper.CSV2DataTable: ERROR in CSV, line ends with \"\"");
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ignoreSeperator = ignoreSeperator == true ? false : true;	// well, than let's alternate the ignoreSeperator
                                                        /*
                                                         * Speed optimization. We are going to copy all chars up until the next "
                                                         *
                                                         * If ignoreSeperator is true AND there is another char following x
                                                         */
                                                        if ((ignoreSeperator) && (((x + 1) <= csvRow.Length)))
                                                            /*
                                                             * first we'll figure out whether there is another " in the row or not (which can be the case if a Cr or CrLf does exist)
                                                             */
                                                            if (csvRow.IndexOf('\"', x + 1) > 0)
                                                            {
#if DEBUG
                                                                littleHelpers.DebugPrint(true, "\" detected in remaining row, copying everything up until this char");
#endif
                                                                currentColumn += csvRow.IndexOf('\"', x + 1) - x - 1 > 0 ? csvRow.Substring(x + 1, csvRow.IndexOf('\"', x + 1) - x - 1) : "";
                                                                //x = x + csvRow.Substring(x + 1, csvRow.IndexOf('\"', x + 1) - x - 1).Length;
                                                                x = x + (csvRow.IndexOf('\"', x + 1) - x - 1 > 0 ? csvRow.Substring(x + 1, csvRow.IndexOf('\"', x + 1) - x - 1).Length : 0);
                                                            }
                                                            else
                                                            {
                                                                // well, no " upcoming in this row, so we can simply copy the entire row ;)
#if DEBUG
                                                                littleHelpers.DebugPrint(true, "\" not detected in remaining row, copying everything to the end of the row");
#endif
                                                                currentColumn += csvRow.Substring(x + 1, csvRow.Length - x - 1);
                                                                x = csvRow.Length;
#if DEBUG
                                                                littleHelpers.DebugPrint(true, currentColumn);
#endif
                                                            }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                // TODO: if this doesn't result in an useful result, simply consider to take the substring to the end of line and check for the seperator.

                                                /*
                                                 * we might shortcut here - so first we copy now everything from this point to the next seperator
                                                 * if we can't shortcut simply since there are not enough chars or no , ... we simply return an empty ZW :)
                                                 */
                                                string zw = csvRow.IndexOf(fullSeperator, x + 1) - x - 1 > 0 ? csvRow.Substring(x + 1, csvRow.IndexOf(fullSeperator, x + 1) - x - 1) : "";
                                                /*
                                                 * now, if there is a " in zw, we need to use the current char (or TODO: optimize here again by copying to the next " ;))
                                                 */
                                                if (zw.IndexOf("\"") != -1)
                                                    if (zw.IndexOf("\"") != zw.Length - 1)	// check whether it the index is equal the last char in the string
                                                        currentColumn += currentChar;		// nope - well than we have to go through it char by char
                                                    else									// yes, if so, we copy everything except the last char
                                                    {
                                                        currentColumn += currentChar + zw.Substring(0, zw.Length - 1);
                                                        x = x + zw.Length;
                                                    }

                                                else									// since there is no " in zw, we can simply copy the whole string 8-)
                                                {
                                                    currentColumn += currentChar + zw;
                                                    x = x + zw.Length;
                                                }
                                            }
                                        }

                                        /*
                                         * If the current number of columns is below the number of columns
                                         * we originally determined, obviosely, the next line belongs to
                                         * this one.
                                         *
                                         * Rule:	We only expect a following line as ChildLine if before an " has been openend
                                         *			and triggered ignoreSeperator to true!
                                         */
                                        if ((csvColumnCounter < retValue.Columns.Count) && (ignoreSeperator))
                                        {
                                            isChildLine = true;
                                            currentColumn += "\r\n";					// add a carriage return, new line to the column
                                        }
                                        else
                                        {
                                            isChildLine = false;

                                            if (currentColumn != "")	// do we need to write the last column?
                                                // add a new column and store the current value in the current column
                                                CSVAddCSVColumnColumn(ref csvColumnCounter, retValue.Columns.Count, ref csvColumns, ref currentColumn);
                                        }
                                    }
                                }

                                //MessageBox.Show(csvRow);

                                if (!isChildLine)
                                {
                                    /*
                                     * Well, let's do some un-escaping (this is done independent on the following steps)
                                     * TODO: Consider whether this is done by an optional parameter in future.
                                     */
                                    for (int x = 0; x < csvColumns.GetUpperBound(0); x++)
                                    {
                                        csvColumns[x] = csvColumns[x].Replace("\"\"", "\"");
                                        csvColumns[x] = csvColumns[x].Replace("\\r", "\r");
                                        csvColumns[x] = csvColumns[x].Replace("\\n", "\n");
                                    }

                                    /*
                                     * now, let's handle the table creation and event thing.
                                     */
                                    if ((csvRowImported != null) && (CreateDataTableAlways == false))	// let's check whether someone got an abo only or more ;o)
                                    {
                                        csvRowImported(csvColumns, EventArgs.Empty);	                // we simply send the array :) - EventArgs are set to Empty for now.
                                    }
                                    else                                                                // well, we are definitely creating the returnValue DataTable
                                    {
                                        if (csvRowImported != null)
                                            csvRowImported(csvColumns, EventArgs.Empty);	            // we send the array if wished :) - EventArgs are set to Empty for now.

                                        if (retValue.Columns.Count == 0)						        // in the case we do not have a header row, we need to create the columns
                                            // once after we parsed the first line ...
                                            for (int x = 0; x <= csvColumns.GetUpperBound(0); x++)
                                                retValue.Columns.Add(new DataColumn());

                                        retValue.Rows.Add(csvColumns);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    //MessageBox.Show("File {0} doesn't exist!", Path);
                    throw new IOException(string.Format("File {0} doesn't exist!", Path));
                }

                //MessageBox.Show("CSV2Table... done");
                return retValue;
            }

            #region CSV2DataTable Helper

            #region Seperator Handler

            /// <summary>
            ///
            /// </summary>
            /// <param name="x"></param>
            /// <param name="csvRow"></param>
            /// <param name="currentChar"></param>
            /// <param name="fullSeperator"></param>
            /// <param name="ignoreSeperator"></param>
            /// <param name="csvColumnCounter"></param>
            /// <param name="csvColumns"></param>
            /// <param name="currentColumn"></param>
            private void CSVSeperatorHandler(ref int x, ref string csvRow, ref string currentChar, string fullSeperator, ref bool ignoreSeperator, ref int csvColumnCounter, ref string[] csvColumns, ref string currentColumn, int retValueColumnsCounter)
            {
                // check whether a Seperator is the current char - except if we do not need to check anyways :)
                if ((currentChar == fullSeperator) && (ignoreSeperator == false))
                {
                    // first add a new column and store the current value in the current column
                    CSVAddCSVColumnColumn(ref csvColumnCounter, retValueColumnsCounter, ref csvColumns, ref currentColumn);

                    // since we are already here, we can go on, so simply x++ :)
                    if ((x + 1) >= csvRow.Length)
                    {
                        ignoreSeperator = false;	// reset the ignoreSeperator to false
                    }
                    else
                    {
#if DEBUG
                        /*
					 * DEBUGGING ONLY	- use this if you want to break @ a specific column or row :)
					 */
                        if (currentColumn == "p.o.box 652")
                            littleHelpers.DebugPrint(true, "BREAKPOINT HERE :)");
#endif

                        // course we can empty the current column
                        currentColumn = "";

                        currentChar = csvRow[++x].ToString();		// and now we are going to take care about the next char :)
                        //					if (csvRow[x].ToString() == fullSeperator)	// to ensure that we do catch all possible empty columns we use kind of a recursion here :)
                        CSVSeperatorHandler(ref x, ref csvRow, ref currentChar, fullSeperator, ref ignoreSeperator, ref csvColumnCounter, ref csvColumns, ref currentColumn, retValueColumnsCounter);
                    }
                }
            }

            #endregion Seperator Handler

            #region Add a CSVColumn column

            /// <summary>
            ///
            /// </summary>
            /// <param name="csvColumnCounter"></param>
            /// <param name="retValueColumnsCounter"></param>
            /// <param name="csvColumns"></param>
            /// <param name="currentColumn"></param>
            private void CSVAddCSVColumnColumn(ref int csvColumnCounter, int retValueColumnsCounter, ref string[] csvColumns, ref string currentColumn)
            {
                // we only need to expand the csvColumns
                csvColumnCounter++;

                /*
                 * Error checking and throwing
                 * it might appear that the CSV or the developer made an error which has to be analyzed before the system can go on!
                */
                if ((csvColumnCounter > retValueColumnsCounter) && (retValueColumnsCounter != 0)) // if retValueColumnsCounter == 0, usually no headline has been given, therefore we need to believe that any column imported is a 'good' column.
                    // After the first row is added, the value is automatically consisting a number of given columns. Consequently more than the first row columns result in an error!
                    throw new Exception("LittleHelper.CSV2DataTable.CSVSeperatorHandler: created more columns than the CSV(-Header) allows. Developer or CSV error!");

                // well, let's go on!
                if (csvColumnCounter > csvColumns.GetUpperBound(0))
                    Array.Resize(ref csvColumns, csvColumnCounter);		// TODO: Rethink whether there is a faster way to handle this - maybe it is enough to set the array to the size we expect?

                // well, let's store the current column
                csvColumns[csvColumnCounter - 1] = currentColumn;
            }

            #endregion Add a CSVColumn column

            #endregion CSV2DataTable Helper

            #endregion Read

            #region Write

            /// <summary>
            ///
            /// </summary>
            /// <param name="Path"></param>
            /// <param name="DataTable4CSV"></param>
            /// <param name="Separator">if "" then "," is used.</param>
            /// <param name="Encode"></param>
            public void WriteDataTable2CSV(string Path, DataTable DataTable4CSV, string Separator, Encoding Encode)
            {
                // check Path
                if (Path.EndsWith("\\"))
                    throw new Exception("Error: Obviousely a path to an directory instead of a file path was given!");

                using (StreamWriter sw = new StreamWriter(Path, false, Encode))
                {
                    // for all the output, "," is the default column delimiter
                    string columnDelimiter = "\",\"";

                    if (Separator != "")                // well, if we need something else, here we go :)
                        columnDelimiter = Separator;

                    // tell us what you do
                    //MessageBox.Show("DataTable2CSV: Tablename: " + DataTable4CSV.TableName);
                    //MessageBox.Show("");

                    string outPut = "";    // helper variable

                    // then, deploy the column headers first
                    if ((Separator == "") || (Separator == "\",\""))
                        outPut += "\"";    // start the line with a "

                    for (int x = 0; x < DataTable4CSV.Columns.Count; x++)
                    {
                        outPut += DataTable4CSV.Columns[x].Caption.ToString() + columnDelimiter;
                    }

                    if ((Separator == "") || (Separator == "\",\""))
                        outPut = outPut.Substring(0, outPut.Length - 2);                        // remove the last ," to have a correct line end;
                    else
                        outPut = outPut.Substring(0, outPut.Length - columnDelimiter.Length);    // remove the entire last content delimiter to have a correct line end;

                    sw.WriteLine(outPut);
                    //MessageBox.Show(outPut);

                    // followed by the content :)
                    for (int x = 0; x < DataTable4CSV.Rows.Count; x++)
                    {
                        outPut = "";
                        if ((Separator == "") || (Separator == "\",\""))
                            outPut += "\"";    // start the line with a "

                        DataRow dr = DataTable4CSV.Rows[x];
                        for (int q = 0; q <= dr.ItemArray.GetUpperBound(0); q++)
                        {
                            string escaped = "";
                            // first get the data
                            escaped = dr.ItemArray.GetValue(q).ToString();
                            // now start escaping
                            escaped = escaped.Replace("\"", "\"\"");
                            escaped = escaped.Replace("\r", "\\r");
                            escaped = escaped.Replace("\n", "\\n");

                            outPut += escaped + columnDelimiter;
                        }

                        if ((Separator == "") || (Separator == "\",\""))
                            outPut = outPut.Substring(0, outPut.Length - 2);                        // remove the last ," to have a correct line end;
                        else
                            outPut = outPut.Substring(0, outPut.Length - columnDelimiter.Length);    // remove the entire last content delimiter to have a correct line end;

                        sw.WriteLine(outPut);
                        //MessageBox.Show(outPut);
                    }
                    // and done
                    sw.Close();        // would be done by the garbage collector, but we won't wait for it :)
                }
            }

            #endregion Write

            #endregion CSV- Handling
        }
    }
}