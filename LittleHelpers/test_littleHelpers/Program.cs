using System;
using System.Data;
using System.Diagnostics;		// only needed for some stat's and encoding stuff
using System.Globalization;		// only needed for some stat's and encoding stuff

/*
 * the following is not mandatory and here used only for some stat's and encoding stuff
 */

using System.IO;				// only needed for some stat's and encoding stuff
using System.Text;
using System.Threading;			// only needed for some stat's and encoding stuff

namespace C6.Globals.Tools
{
    internal class myEventHandler                            // this class is only for testing purposes and shows how the sent events might be catched and handled.
    {
        private static int LineCounter = 0;
        private LittleHelpers.CSVReader CSVReader;

        public void EventListener(LittleHelpers.CSVReader lhCSVReader)
        {
            CSVReader = lhCSVReader;
            CSVReader.csvRowImported += new LittleHelpers.CSVReader.EventHandler(RowImported);
        }

        public void RowImported(object sender, EventArgs e)
        {
            Console.SetCursorPosition(0, 10);

            string[] Result = (string[])sender;

            Console.WriteLine("Currentline (" + LineCounter.ToString() + "):");

            ConsoleColor cc = Console.ForegroundColor;

            for (int x = 0; x < Result.GetUpperBound(0); x++)
                if (x < Result.GetUpperBound(0) - 1)
                {
                    Console.ForegroundColor = cc;
                    Console.Write(Result[x]);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(" || ");
                }
                else
                {
                    Console.ForegroundColor = cc;
                    Console.Write(Result[x]);
                }

            Console.WriteLine();

            //			Console.WriteLine(LineCounter++);
        }

        public void ClearLineCounter()
        {
            LineCounter = 0;
        }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            /*
             * some variables and settings
             */
            LittleHelpers.CSVReader CSVReader = new LittleHelpers.CSVReader();
            DataTable dt = new DataTable();

            myEventHandler eHandler = new myEventHandler();
            CSVReader.csvRowImported += new LittleHelpers.CSVReader.EventHandler(eHandler.RowImported);

            string Directory2Parse = @"c:\temp\CSV";
            int NoSampleDataLines = 4;
            bool HasHeadline = false;
            int TimeBetweenFiles = 15;

            bool ignoreHeadline = false;
            bool CreateDataTableAlways = false;
            int ColumnCounter = 0;
            bool UseCSVRowImported = true;

            PerformanceCounter freeMem = new PerformanceCounter("Memory", "Available Bytes");
            /*
             * setting default console settings, like green and black, position, buffer etc. pp. 8-)
             */
            //			Console.SetWindowSize(200, 75);
            //			Console.SetWindowPosition(0, 0);
            Console.SetBufferSize(200, Int16.MaxValue - 1);
            ConsoleColor cc = ConsoleColor.Green;
            Console.BackgroundColor = ConsoleColor.Black;		// default, everything is green and black 8-)
            Console.Clear();
            /*
             * allowing some 'customization' of the settings @ runtime
             */
            Console.Write("Path to search for CSV's in? {0}", Directory2Parse);
            Console.SetCursorPosition(Console.CursorLeft - Directory2Parse.Length, Console.CursorTop);
            Console.Beep(5000, 500);
            string zw = Console.ReadLine();
            Directory2Parse = zw == "" ? Directory2Parse : zw;

            Console.Write("Sample DB lines to output? {0}", NoSampleDataLines);
            Console.SetCursorPosition(Console.CursorLeft - NoSampleDataLines.ToString().Length, Console.CursorTop);
            Console.Beep(5000, 500);
            zw = Console.ReadLine();
            NoSampleDataLines = zw == "" ? NoSampleDataLines : Convert.ToInt32(int.Parse(zw));

            Console.Write("Has Headline? {0}", HasHeadline);
            Console.SetCursorPosition(Console.CursorLeft - HasHeadline.ToString().Length, Console.CursorTop);
            Console.Beep(5000, 500);
            zw = Console.ReadLine();
            HasHeadline = zw == "" ? HasHeadline : Convert.ToBoolean(bool.Parse(zw));

            if (HasHeadline)
            {
                Console.Write("Ignore Headline? {0}", ignoreHeadline);
                Console.SetCursorPosition(Console.CursorLeft - ignoreHeadline.ToString().Length, Console.CursorTop);
                Console.Beep(5000, 500);
                zw = Console.ReadLine();
                ignoreHeadline = zw == "" ? ignoreHeadline : Convert.ToBoolean(bool.Parse(zw));
            }

            if ((!HasHeadline) || (ignoreHeadline))
            {
                Console.Write("Number of columns? {0}", ColumnCounter);
                Console.SetCursorPosition(Console.CursorLeft - ColumnCounter.ToString().Length, Console.CursorTop);
                Console.Beep(5000, 500);
                zw = Console.ReadLine();
                ColumnCounter = zw == "" ? ColumnCounter : Convert.ToInt32(int.Parse(zw));
            }

            Console.Write("Time between files in [s]? {0}", TimeBetweenFiles);
            Console.SetCursorPosition(Console.CursorLeft - TimeBetweenFiles.ToString().Length, Console.CursorTop);
            Console.Beep(5000, 500);
            zw = Console.ReadLine();
            TimeBetweenFiles = zw == "" ? TimeBetweenFiles : Convert.ToInt32(int.Parse(zw));

            Console.Write("Use csvRowImportedEvent? {0}", UseCSVRowImported);
            Console.SetCursorPosition(Console.CursorLeft - UseCSVRowImported.ToString().Length, Console.CursorTop);
            Console.Beep(5000, 500);
            zw = Console.ReadLine();
            UseCSVRowImported = zw == "" ? UseCSVRowImported : Convert.ToBoolean(bool.Parse(zw));

            /*
             * let's handle this event thing directly
             */

            if (UseCSVRowImported == false)
                CSVReader.csvRowImported -= null;

            /*
             * and now really start
             */
            Console.Clear();
            Console.WriteLine("Starting to parse for CSV's in directory \"{0}\"", Directory2Parse);

            Console.WriteLine();
            Console.WriteLine("Settings are:");
            Console.WriteLine();
            Console.WriteLine("Searchpath            : {0}", Directory2Parse);
            Console.WriteLine("DB Samplelines        : {0}", NoSampleDataLines);
            Console.WriteLine("Has Headlines         : {0}", HasHeadline);
            Console.WriteLine("Time between files [s]: {0}", TimeBetweenFiles);
            Console.WriteLine("Seperator is          : ,");
            Console.WriteLine();

            if (Directory.Exists(Directory2Parse))
            {
                string[] csvFiles = Directory.GetFiles(Directory2Parse, "*.csv");
                Console.WriteLine("Found {0} CSV- files within the directory, going to parse them now!", csvFiles.GetUpperBound(0));
                Console.WriteLine();

                for (int x = 0; x <= csvFiles.GetUpperBound(0); x++)
                {
                    Console.Clear();
                    string File2Parse = csvFiles[x];
#if DEBUG
                    File2Parse = @"c:\temp\CSV\de_amazon_dvdvideo.csv";			// for debugging only - you might want to change, remove or outcomment this!
                    File2Parse = @"c:\temp\CSV\20070217.CSV";
                    HasHeadline = true;
#endif

                    if (File.Exists(File2Parse))
                    {
                        Console.WriteLine("Start handling file \"{0}\".", File2Parse);
                        FileInfo fi = new FileInfo(File2Parse);
                        Console.WriteLine("FileSize: {0} kb [~{1} MB]", (fi.Length / 1024).ToString("N2", CultureInfo.InvariantCulture), (fi.Length / 1024 / 1024).ToString("N1", CultureInfo.InvariantCulture));
                        Console.WriteLine();

                        /*
                         * count the lines in the File
                         */
                        Console.WriteLine("Getting lines in file...");
                        int FileLineCounter = 0;
                        Console.Write("Lines in file: {0}", FileLineCounter);

                        Encoding FileEncoding = Encoding.Default;                       // store the Encoding used in the file

                        using (StreamReader countReader = new StreamReader(File2Parse))
                        {
                            while (countReader.ReadLine() != null)
                            {
                                FileLineCounter++;
                                Console.SetCursorPosition(0, Console.CursorTop);
                                Console.Write("Lines in file: {0}", FileLineCounter);
                            }
                            FileEncoding = countReader.CurrentEncoding;                 // let's store the StreamReader detected file encoding here
                            Console.WriteLine();
                            Console.WriteLine("Encoding of file detected: {0}", FileEncoding.ToString().Replace("System.Text.", ""));
                        }

                        Console.WriteLine();
                        Console.WriteLine();

                        DateTime StartTime = DateTime.Now;	// we take this when we really start to import :)
                        Console.WriteLine("Start time:\t{0}", StartTime.ToString());
                        /*
                         * call the function itself :)
                         *
                         * this call expects , as seperator - it's hardcoded here...
                         */
                        dt = CSVReader.Read(File2Parse, new string[1] { "," }, FileEncoding, HasHeadline, File2Parse, ignoreHeadline, CreateDataTableAlways, ColumnCounter);        // TODO: Remove hardcoded delimiter, make it flexible for the test tool
                        /*
                         * get us some sample lines
                         */
                        Console.WriteLine("{0} lines of datasamples from DataTable", NoSampleDataLines);

                        for (int l = 0; l < NoSampleDataLines; l++)
                        {
                            string DataRow = "";
                            Console.WriteLine();
                            Console.Write("Showing row number: ");
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine(l + 1);
                            Console.ForegroundColor = cc;
                            Console.WriteLine();

                            if (l < dt.Rows.Count)
                            {
                                foreach (DataColumn dc in dt.Columns)
                                {
                                    DataRow += dt.Rows[l][dc.Caption].ToString() + " || ";
                                }
                                DataRow = DataRow.Substring(0, DataRow.Length - 4);	// remove the last " || "
                                /*
                                 * now output it, for better readability, we mark || as RED
                                 */
                                for (int c = 0; c < DataRow.Length; c++)
                                {
                                    if (DataRow[c] == '|')
                                        Console.ForegroundColor = ConsoleColor.Red;
                                    else
                                        Console.ForegroundColor = cc;

                                    Console.Write(DataRow[c]);
                                }
                                Console.WriteLine();
                            }
                        }
                        Console.WriteLine();
                        /*
                         * and do some statistics :)
                         */
                        DateTime EndTime = DateTime.Now;
                        Console.WriteLine("End time:\t{0}", EndTime.ToString());
                        Console.WriteLine("Time taken:\t{0}", EndTime.Subtract(StartTime).ToString());
                        Console.WriteLine();
                        Console.WriteLine();
                        Console.WriteLine("{0} rows in file, import took in average {1} seconds per row", dt.Rows.Count, ((double)EndTime.Subtract(StartTime).Seconds / (double)dt.Rows.Count).ToString("N4", CultureInfo.InvariantCulture));
                        Console.WriteLine();
                        Console.WriteLine();
                        /*
                         * roughly estimate the memory used by the DT
                         */
                        float FreeMemBefore = freeMem.NextValue();
                        dt.Clear();
                        float FreeMemAfter = freeMem.NextValue();
                        Console.WriteLine("The Datatable consumes ROUGHLY estimated {0} bytes in memory", FreeMemAfter - FreeMemBefore);
                        Console.WriteLine();
                        Console.WriteLine("Finished handling file \"{0}\".", File2Parse);
                        Console.WriteLine();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Waiting for {0}s before going on with next file", TimeBetweenFiles);
                        Console.ForegroundColor = cc;
                        Thread.Sleep(TimeBetweenFiles * 1000);
                        Console.WriteLine();
                    }
                    else
                        Console.WriteLine("File \"{0}\" does not exist!", File2Parse);
                }
            }
            else
                Console.WriteLine("Directory \"{0}\" does not exist!", Directory2Parse);
            Console.WriteLine();
            Console.WriteLine("Please press any key...");
            Console.ReadKey();
        }
    }
}