using System;

using System.Diagnostics;

namespace C6.Globals.Tools
{
    public partial class LittleHelpers
    {
        /// <summary>
        ///
        /// </summary>
        public class ExcelTools
        {
            #region Global Variables

            #region Enum definition

            /// <summary>
            ///
            /// </summary>
            public enum GetColumnNameDirection
            {
                Left,
                Right
            }

            /// <summary>
            ///
            /// </summary>
            public enum ExcelVersion
            {
                Excel2007,
                Excel2010
            }

            #endregion Enum definition

            #region Variable definition

            // load the default variables
            private string maxColumns2007 = Properties.Settings.Default.MaxColumnExcel2007.ToString();

            private string maxColumns2010 = Properties.Settings.Default.MaxColumnExcel2010.ToString();

            #endregion Variable definition

            #region Properties

            #endregion Properties

            #region Structs

            private struct MoveColumnRetValue
            {
                public int Move;
                public char Result;
            }

            #endregion Structs

            #endregion Global Variables

            private MoveColumnRetValue MoveColumn(char minValue, char MaxValue, char CurrentValue, int AddNumberOfColumns, GetColumnNameDirection Direction)
            {
                MoveColumnRetValue returnValue = new MoveColumnRetValue();

                int direction = (Direction == GetColumnNameDirection.Left) ? -AddNumberOfColumns : AddNumberOfColumns;		// If -, we go left, if + we go right. We always go the number of columns set in AddNumberOfColumns.
                CurrentValue = Convert.ToChar(CurrentValue.ToString().ToUpper());											// let's ensure we work on upper cases only. The way we do this is some kind of through the back to the head... TODO: Find a better method!
                /*
                 * now let's get some ASCII to work with
                 */
                int currentASCII = Convert.ToInt16(CurrentValue);
                int minASCII = Convert.ToInt16(minValue);
                int maxASCII = Convert.ToInt16(MaxValue);
                /*
                 * and now let's go to work
                 */
#if DEBUG
                Debug.Print("");
#endif

                return returnValue;
            }

            /// <summary>
            /// Returns the maximal available columns for Excel 2007 or 2010.
            /// </summary>
            /// <param name="EV">The Excel Version the maxcolumn should be returned for</param>
            /// <returns>The max-column in form of "XFD" or similiar</returns>
            public string MaxColumn(ExcelVersion EV)
            {
                if (EV == ExcelVersion.Excel2007)
                    return maxColumns2007;
                else
                    return maxColumns2010;
            }

            /// <summary>
            /// This method returns, depending on the given parameter values, the name (header) of an Excel based column
            /// </summary>
            /// <param name="CurColumn">The current used column. If the column is not given in uppercase, it is converted to. </param>
            /// <param name="Version">The Excel Version</param>
            /// <param name="Direction">Is the cursor moving to the Right or Left?</param>
            /// <param name="AddNumberOfColumns ">The number of columns the cursor should move.</param>
            /// <returns>The resulting column as a string.</returns>
            public string GetNextColumnName(string CurColumn, ExcelVersion Version, GetColumnNameDirection Direction, byte AddNumberOfColumns = 1)
            {
                string result = CurColumn;																					// If we change nothing or throw an error, at least the current column will be returned
                string MaxColumns = "";
                int direction = (Direction == GetColumnNameDirection.Left) ? -AddNumberOfColumns : AddNumberOfColumns;		// If -, we go left, if + we go right. We always go the number of columns set in AddNumberOfColumns.

                CurColumn = CurColumn.ToUpper();																			// let's ensure we work on upper cases only.

                if (Version == ExcelVersion.Excel2007)																		// setup the MaxColumns version conform.
                    MaxColumns = maxColumns2007.ToUpper();
                else
                    MaxColumns = maxColumns2010.ToUpper();

                char currentChar = CurColumn.ToUpper()[CurColumn.Length - 1];																// We always need the right most char to start from.
                int currentASCII = Convert.ToInt16(currentChar);

                /*
                                if ((CurColumn == MaxColumns) && (direction == 1))						// check whether we already reached the end to the right
                                {
                                    throw new Exception(string.Format("Max. column for Excelversion {0} reached. The MaxColumn is {1}", Version.ToString(), MaxColumns.ToString()));
                                }
                                else
                                {
                                    if ((CurColumn == "A") && (direction == -1))						// check whether we already reached the beginning to the left
                                    {
                                        throw new Exception(string.Format("Min. column for Excelversion {0} reached. The MinColumn is \"A\"", Version.ToString()));
                                    }
                                    else																							// well, we've work todo
                                    {
                                        for (int x = CurColumn.Length; x > 0; x--)													// we start from the last char in the string
                                        {
                                            string wrkChar = CurColumn[CurColumn.Length].ToString();								// we always work on the currently last char first
                                            int ascWrkChar = Convert.ToInt16(wrkChar.ToUpper());									// the ASCII code of the char. "A" is 65, "Z" is 90

                                            if (ascWrkChar - 1 < 65)
                                            {
                                                // at this point, it is clear that we have two columns, therefore it is easy to act and no further measurement is needed
                                                result = CurColumn.Substring(1, CurColumn.Length - 1) ;
                                            }
                                            else
                                                if (ascWrkChar + 1 > 90)
                                                {
                                                }
                                        }
                                    }
                                }
                 */
                return result;
            }
        }
    }
}