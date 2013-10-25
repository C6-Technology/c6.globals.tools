using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SocialCom.Globals.Tools
{
	public class Logging : IDisposable
	{

		#region Base class variables and functions
		#region Global Variables
		#region Internal variables
		/// <summary>
		/// Stores whether debugging information should be printed or not
		/// </summary>
		private bool debug = false;
		/// <summary>
		/// false ~ Dispose() not yet called, nothing happened; true ~ Dispose() called and executed
		/// </summary>
		private bool disposed = false;
		#endregion
		#region Properties		
		/// <summary>
		/// true ~ debugging
		/// false ~ no debugging
		/// </summary>
		public bool Debug
		{
			get { return debug; }
			set { debug = value; }
		}
		#endregion
		#endregion
		#region (De-)Constructor
		public Logging()
		{
		}

		~Logging()
		{
			if (!disposed)
				this.Dispose();
		}
		#endregion
		#region IDisposable implementation
		/// <summary>
		/// Implementation of IDisposable.
		/// </summary>
		/// <returns>nothing</returns>
		public void Dispose()
		{
			Dispose(true);
		}
		/// <summary>
		/// Implementation of IDisposable.
		/// </summary>
		/// <param name="disposing">true ~ do dispose; false ~ don't dispose</param>
		public void Dispose(bool disposing)
		{
			// Check to see if Dispose has already been called.
			if (!this.disposed)
				// If disposing equals true, dispose all managed
				// and unmanaged resources.
				if (disposing)
				{
					// Dispose managed resources.
				}

			// Note disposing has been done.
			disposed = true;
		}
		#endregion
		#endregion

		/// <summary>
		/// The internal representation of the function.
		/// </summary>
		/// <param name="condition">True ~ Debug.WriteLineIf will write; False ~ Debug.WriteLineIf won't write...</param>
		/// <param name="message">The string to be printed.</param>
		public void PrintLog(EventLogEntryType Condition, string Source, string Message)
		{

			/*
						if (!EventLog.SourceExists(Source))
							EventLog.CreateEventSource(Source, Source + "_log");
			
						if (PrintLogValue != null)	// if the messages are 'abonemented', we redirect all data to the message destination
							if ((Message.IndexOf(DateTime.Now.ToString() + " || ") == 0))
								PrintLogValue(Condition, Source, littleHelpers.CreateDebugMessageString(Message));
							else
								PrintLogValue(Condition, Source, Message);
						else                        // if not, we write everything to System.Diagnostics.Debug AND the EventLog	
						{
							//System.Diagnostics.Debug.WriteLineIf(true, Source + " - " + Message);
							//EventLog.WriteEntry(Source, Message, Condition);
						}
			 */
		}

	}
}
