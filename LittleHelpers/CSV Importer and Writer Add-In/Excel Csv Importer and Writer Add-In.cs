using System;
/*
 * some additional namespace we need to take some functionality from
 */

using System.Data;
using System.Text;
using System.Threading;
using Excel = Microsoft.Office.Interop.Excel;
using C6.Globals.Tools;

namespace CSV_Importer_and_Writer_Add_In
{
	public partial class ThisAddIn
	{
		private LittleHelpers littleHelper = new LittleHelpers();							// initialize LittleHelpers
		private LittleHelpers.CSVReader CSVReader = new LittleHelpers.CSVReader();			// initialize CSVReader

		/*
		 * This is going to be dirty - and may be replaced by a better solution - to ensure that we can create a lock
		 * on the CurrentImportedRowCounter
		 */
		private static string CurrentImportedRowCounter = "0";

		private Excel.Worksheet CurrentWorkSheet;

#if DEBUG
		private bool ShowDevToolsSetting = false;											// only for debugging
#endif

		/// <summary>
		///
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ThisAddIn_Startup(object sender, System.EventArgs e)
		{
#if DEBUG
			ShowDevToolsSetting = Application.ShowDevTools;         // we want to restore the setting afterwards
			Application.ShowDevTools = true;                        // but for now, please display the tools.
#endif
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
		{
#if DEBUG
			Application.ShowDevTools = ShowDevToolsSetting;         // well, let's restore the origin setting
#endif
			/*
			 * CleanUp
			 */
			CSVReader = null;
			littleHelper = null;
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void RowImported(object sender, EventArgs e)
		{
			string[] ImportedRow = (string[])sender;
			Application.ReferenceStyle = Excel.XlReferenceStyle.xlR1C1;			// It is easier to move through the table this way :)

			lock (CurrentImportedRowCounter)								// we are going to add 1 to the current row (marking the row we are going to work on), to assure 'thread' safety, we lock it first.
			{
				CurrentImportedRowCounter = (Convert.ToUInt64(CurrentImportedRowCounter) + 1).ToString();		// HACK: TODO: find a better method or check whether or not it is needed this way. During dev-tests errors didn't come up anymore after implementing it this way.
			}

			for (Int64 x = 0; x <= ImportedRow.GetUpperBound(0); x++)
			{
				((Excel.Range)CurrentWorkSheet.Cells[Convert.ToInt32(CurrentImportedRowCounter), (x + 1)]).Select();
				((Excel.Range)CurrentWorkSheet.Cells[Convert.ToInt32(CurrentImportedRowCounter), (x + 1)]).Value = ImportedRow[x];
			}
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="FileName"></param>
		/// <param name="RangeFrom"></param>
		/// <param name="RangeTo"></param>
		/// <param name="ColumnSeperator"></param>
		/// <param name="HasHeadline"></param>
		/// <param name="NoColumns"></param>
		/// <param name="Visible"></param>
		/// <param name="Verbose"></param>
		public void ImportCSV(string FileName, string RangeFrom, string RangeTo, string ColumnSeperator = ",", bool HasHeadline = false, int NoColumns = 0, bool Visible = true, bool Verbose = true)
		{
			/*
			 * Initialize some variables
			 */
			bool ScreenUpdating = Application.ScreenUpdating;							// backup current state
			Application.ScreenUpdating = Visible;										// assure that the screen is visible (or not) updated during work

			Excel.Worksheet currentWorksheet = this.Application.ActiveSheet;
			currentWorksheet.Activate();												// not neccessary, but nice to see :)
			CurrentWorkSheet = currentWorksheet;										// assure that RowImported could use the Worksheet
			/*
			 * TODO: remove the following two lines if implementation of RangeFrom - RangeTo should be implemented
			 */
			string column = "A";
			string row = "1";

			Application.ReferenceStyle = Excel.XlReferenceStyle.xlR1C1;			// It is easier to move through the table this way :)

			// well, we would like to import from row 1 for now
			// TODO: Change so RangeFrom is used here!
			long curColumn = 1;
			long curRow = 1;

			long maxColumn = 0;
			long maxRow = 0;
			/*
			 * check some preliminaries
			 */
			if ((HasHeadline == false) && (NoColumns == 0))
			{
				NoColumns = Convert.ToInt16(Application.InputBox("Columns: ", "You indicated that the file has no headline, please indicate the number of columns for proper import.", Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, 1));
			}
			/*
			 * start the import
			 */
			CSVReader.csvRowImported += new LittleHelpers.CSVReader.EventHandler(RowImported);	// let's handle each row after it was imported.
			Application.StatusBar = string.Format("Starting to import from {0}", FileName);
			CSVReader.Read(FileName, new string[] { ColumnSeperator }, Encoding.UTF8, HasHeadline, "", false, false, NoColumns);
			/*
			 * CleanUp
			 */
			Application.StatusBar = "";									// give it back to Excel :)
			Application.ScreenUpdating = ScreenUpdating;				// set it back to its original state
			CurrentImportedRowCounter = "0";							// set it back to start
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="FileName"></param>
		/// <param name="RangeFrom"></param>
		/// <param name="RangeTo"></param>
		/// <param name="ColumnSeperator"></param>
		/// <param name="HasHeadline"></param>
		/// <param name="Visible"></param>
		/// <param name="Verbose"></param>
		public void Export2CSV(string FileName, string RangeFrom, string RangeTo, string ColumnSeperator = ",", bool HasHeadline = false, bool Visible = true, bool Verbose = true)
		{
			/*
			 * Initialize some variables
			 */
			LittleHelpers.CSVReader CSVReader = new LittleHelpers.CSVReader();			// initialize CSVReader
			DataTable Table2Export = new DataTable();									// create the base data table expected by the exporting routine

			bool ScreenUpdating = Application.ScreenUpdating;							// backup current state
			Application.ScreenUpdating = Visible;										// assure that the screen is visible (or not) updated during work

			Excel.Worksheet currentWorksheet = this.Application.ActiveSheet;
			currentWorksheet.Activate();												// not neccessary, but nice to see :)

			/*
			* If no RangeFrom and/or RangeTo is set, we are going to create RangeFrom and RangeTo by
			* going from the first cell A1) to the right and figure out the last cell with a value != NULL or ""
			* as well as we are going down as long as the cells content isn't NULL or ""
			*/

			Excel.Range curRange;

			if ((RangeFrom != "") && (RangeTo != ""))
			{
				Application.StatusBar = string.Format("Choosing \"{0}\" as Range to import from..", RangeFrom + ":" + RangeTo);

				curRange = currentWorksheet.get_Range(RangeFrom, RangeTo);
				curRange.Select();
			}
			else
			{
				Application.StatusBar = "Choosing complete Worksheet as Range to import from..";
				curRange = currentWorksheet.Cells;
			}

			if (curRange != null)
			{
				Application.StatusBar = "Starting to figure out the number of columns to export...";
				/*
				 * TODO: remove the following two lines if implementation of RangeFrom - RangeTo should be implemented
				 */
				string column = "A";
				string row = "1";

				Application.ReferenceStyle = Excel.XlReferenceStyle.xlR1C1;			// It is easier to move through the table this way :)

				// well, we would like to import from row 1 for now
				// TODO: Change so RangeFrom is used here!
				long curColumn = 1;
				long curRow = 1;

				long maxColumn = 0;
				long maxRow = 0;

				DataRow dtCurRow;
				DataColumn dtCurColumn;

				// TODO: The following has to be changed so that RangeFrom / RangeTo is used - remember, this might mean that empty cells are exported !!
				// TODO: Implement Headline checking, if hasHeadline == True, we need to implement a check that Headlines (per column header!) isn't double existing nor has empty cell(s)...

				/*
				 * first we figure out the number of columns we have to import
				 *
				 * NOTE: If RangeFrom / RangeTo is implemented, this might have to be changed so it checks the range given as well as it than ignores empty cells within the range (except, HasHeadline is true, than EmptyCell handling must be implemented as well)
				 */
				string HeadLine = "";	// if there is a headline, we need this to ensure that the headers are unique, if there is no headline, we just add the columns to the table (which results automatically in named columns)

				while (curRange.Cells[curRow, curColumn].Value != null)
				{
					maxColumn += 1;

					if (HasHeadline == true)			// if we should get the first line as headline, we store it here first...
					{
						HeadLine += curRange.Cells[curRow, curColumn].Value.ToString() + ColumnSeperator;
					}
					else								// if not, we simply add empty columns to the table
					{
						Table2Export.Columns.Add("");	// we simply add empty columns to ensure the table has all columns it needs
					}

					curColumn += 1;
				}

				if (HasHeadline == true)				// well, we still have to create the tables columns - we use GetCSVColumns for this since it creates unique headers.
				{
					CSVReader.GetCSVColumns(HeadLine.TrimEnd((ColumnSeperator.ToCharArray())), new string[1] { ColumnSeperator }, ref Table2Export);		// TODO: Check whether new string[1] might result in an error if the Seperator is different... (importance: low)
				}

				Application.StatusBar = string.Format("{0} columns are seen as exportable", maxColumn);
				Thread.Sleep(2000);
				// create an array to store the cell data for one row
				string[] csvColumns = new string[maxColumn];

				/*
				 * now, let's fill the table
				 */
				// change the following two lines when implementing RangeFrom / RangeTo
				curColumn = 1;
				curRow = HasHeadline == true ? 2 : 1;		// NOTE: 2 because first line is already exported if HasHeadline is true, 1 since if the Headline hasn't been exported, the first line has to be exported as well.

				while (curRange.Cells[curRow, curColumn].Value != null)
				{
					Application.StatusBar = string.Format("Row {0} is currently been prepared to be exported.", curRow);
					dtCurRow = Table2Export.NewRow();				// well, we add a row here

					// TODO: exchange 1 against RangeFrom column when implementing RangeFrom / RangeTo
					for (curColumn = 1; curColumn <= maxColumn; curColumn++)							// we use a for instead of while since we can not be sure that within a row, more columns are used than in the headline - to ensure that we do not run into an error, we just take what we surely know exists.
					{
						if (Verbose == true)
						{
							Application.StatusBar = string.Format("Currently exporting row {0} and here column {1}.", curRow, curColumn);		// we only print this if Verbose Import was activated
						}
						csvColumns[curColumn - 1] = curRange.Cells[curRow, curColumn].Value != null ? curRange.Cells[curRow, curColumn].Value.ToString() : "";
					}

					// well, let's add the row
					Table2Export.Rows.Add(csvColumns);

					// change the following three lines when implementing RangeFrom / RangeTo
					curColumn = 1;				// reset curColumn
					curRow += 1;				// and go on with the next line :)
					curRange.Cells[curRow, curColumn].Select();
				}
				Application.StatusBar = string.Format("{0} rows have been prepared to be exported.", curRow - 1);
			}
			else
			{
				Application.StatusBar = "CSV Export: Nothing to Export or Cell A1 is empty";
			}

			Thread.Sleep(8000);             // Wait 8s to ensure a user can read the last Statusbar message and than
			Application.StatusBar = "";     // reset the statusbar

			// and now the easy part: Write the file :)
			//littleHelper.DataTable2CSV(FileName, Table2Export, ",", Encoding.UTF8);
			Application.StatusBar = "Begin to write the file...";

			try
			{
				CSVReader.WriteDataTable2CSV(FileName, Table2Export, ColumnSeperator, Encoding.UTF8);
			}
			catch (Exception)	// TODO: Implement error handling
			{
				throw;
			}
			Application.StatusBar = "Done writing the file, it can be found at " + FileName;
			Thread.Sleep(8000);
			/*
			 * clean up
			 */
			Application.StatusBar = "";									// give it back to Excel :)
			Application.ScreenUpdating = ScreenUpdating;				// set it back to its original state
			Table2Export = null;										// Dispose the table (not neccessary)
		}

		#region VSTO generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InternalStartup()
		{
			this.Startup += new System.EventHandler(ThisAddIn_Startup);
			this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
		}

		#endregion VSTO generated code
	}
}