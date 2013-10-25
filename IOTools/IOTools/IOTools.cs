using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;
using System.Windows.Forms;
using System.Web.UI.WebControls;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;


namespace C6.Globals.Tools
{
	/// <summary>
	/// This class includes diverse tools for IO related tasks, f.e. File and Directory etc.
	/// </summary>
	public class IOTools
	{
		/// <summary>
		/// stores whether debugging information should be printed or not
		/// </summary>
		private bool debug = true;
		/// <summary>
		/// public Property representing debugging status and opening the possibility to turn it off
		/// </summary>
		public bool Debug
		{
			get { return debug; }
			set { debug = value; }
		}

		/// <summary>
		/// We use some functionaly from this library so this is the reference to it ;)
		/// </summary>
		LittleHelpers littleHelper = new LittleHelpers();

		//TODO: Check whether IOTools needs a (de-)constructor and implement it if needed
		/// <summary>
		/// 
		/// </summary>
		public IOTools()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		#region LoadDirectories
		/// <summary>
		/// This function overload loads all Directory Information into one TreeNode.
		/// 
		/// searchPattern defaults to "*.*"
		/// subDirectories defaults to "true"
		/// </summary>
		/// <param name="parent">The TreeNode object to be filled
		/// The Object needs to be of type System.Windows.Forms.TreeNode() or System.Web.UI.WebControls.TreeNode()
		/// </param>
		/// <param name="path">The path, the directories should be retrieved from</param>
		public void LoadDirectories(Object parent, string path) 
		{
			LoadDirectories(parent, path, "*.*", true);
		}
		/// <summary>
		/// This function overload loads all Directory Information into one TreeNode.
		/// 
		/// subDirectories defaults to "true"
		/// </summary>
		/// <param name="parent">The TreeNode object to be filled
		/// The Object needs to be of type System.Windows.Forms.TreeNode() or System.Web.UI.WebControls.TreeNode()
		/// </param>
		/// <param name="path">The path, the directories should be retrieved from</param>
		/// <param name="searchPattern">The searchPattern to be searched by</param>
		public void LoadDirectories(Object parent, string path, string searchPattern)
		{
			LoadDirectories(parent, path, searchPattern, true);
		}
		/// <summary>
		/// This function overload loads all Directory Information into one TreeNode.
		/// 
		/// SearchPattern defaults to "*.*"
		/// </summary>
		/// <param name="parent">The TreeNode object to be filled
		/// The Object needs to be of type System.Windows.Forms.TreeNode() or System.Web.UI.WebControls.TreeNode()
		/// </param>
		/// <param name="path">The path, the directories should be retrieved from</param>
		/// <param name="SubDirectories">Should the function get the subdirectories as well (true) or not (false)</param>
		public void LoadDirectories(Object parent, string path, bool subDirectories)
		{
			LoadDirectories(parent, path, "*.*", subDirectories);
		}

		public void LoadDirectories(System.Windows.Forms.TreeNode parent, string path, string SearchPattern, bool SubDirectories)
		{
			LoadDirectories(parent, path, SearchPattern, SubDirectories);
		}

		public void LoadDirectories(System.Web.UI.WebControls.TreeNode parent, string path, string SearchPattern, bool SubDirectories)
		{
			LoadDirectories(parent, path, SearchPattern, SubDirectories);
		}


		/// <summary>
		/// This function loads all Directory Information into one TreeNode.
		/// </summary>
		/// <param name="parent">The TreeNode object to be filled
		/// The Object needs to be of type System.Windows.Forms.TreeNode() or System.Web.UI.WebControls.TreeNode()
		/// </param>
		/// <param name="path">The path, the directories should be retrieved from</param>
		/// <param name="SearchPattern">If needed a search pattern</param>
		/// <param name="SubDirectories">Should the function get the subdirectories as well (true) or not (false)</param>
		private void LoadDirectories(Object parent, string path, string SearchPattern, bool SubDirectories)
		{
			// initializing base Variables for returning data to both types of TreeNodes we
			// have to deal with.
			System.Windows.Forms.TreeNode       winNode = new System.Windows.Forms.TreeNode();
			System.Web.UI.WebControls.TreeNode  webNode = new System.Web.UI.WebControls.TreeNode();

			// initializing further needed variables
			DirectoryInfo directory = new DirectoryInfo(path);

			// now checking whether the developer provided us the data we where expecting or not!
			// if he didn't, we are throwing an exception
			if ((!Object.ReferenceEquals(parent.GetType(), winNode.GetType()) && (!Object.ReferenceEquals(parent.GetType(),webNode.GetType()))))
				throw new Exception("DEVELOPER ERROR: IOTools.LoadDirectories\r\nObject parent can be only of Type Windows.Forms.TreeNode() or Web.UI.WebControls.TreeNode()!");

/*
			Type parentType = typeof(Object);

			PropertyInfo parentText = parentType.GetProperty("Text");
 */

			//TODO: Implement SearchPattern
			AddLogMessage(debug, "SearchPattern not yet implemented");

			// write some information into the debug window
			AddLogMessage(debug, "LoadDirectories");
			AddLogMessage(debug, "Path: " + path);
			AddLogMessage(debug, "SearchPattern: " + SearchPattern);
			AddLogMessage(debug, "SubDirectories: " + SubDirectories.ToString());

			// now let's retrieve the directory information
			try
			{
				foreach (DirectoryInfo dirInfo in directory.GetDirectories())
				{
					// check which type of TreeNode we are dealing with and fill data into it.
					if (Object.ReferenceEquals(parent.GetType(), winNode.GetType()))
					{
						System.Windows.Forms.TreeNode node = new System.Windows.Forms.TreeNode();
						node.Text = dirInfo.Name;
						node.Tag = dirInfo.FullName;

						((System.Windows.Forms.TreeNode)parent).Nodes.Add(node);

						if (SubDirectories == true)
						{
							// Recurse the current directory
							LoadDirectories(node, dirInfo.FullName);
						}
					}
					else
					{
						System.Web.UI.WebControls.TreeNode node = new System.Web.UI.WebControls.TreeNode();
						node.Text = dirInfo.Name;

						((System.Web.UI.WebControls.TreeNode)parent).ChildNodes.Add(node);

						if (SubDirectories == true)
						{
							// Recurse the current directory
							LoadDirectories(node, dirInfo.FullName);
						}
					}
				}
			}
			catch (System.UnauthorizedAccessException eUnauthorized)
			{
				if (Object.ReferenceEquals(parent.GetType(), winNode.GetType()))
				{
					System.Windows.Forms.TreeNode node = new System.Windows.Forms.TreeNode();
					node.Text = " (Access Denied - " + eUnauthorized.Message + ")";
					
					((System.Windows.Forms.TreeNode)parent).Nodes.Add(node);
				}
				else
				{
					System.Web.UI.WebControls.TreeNode node = new System.Web.UI.WebControls.TreeNode();
					node.Text = " (Access Denied - " + eUnauthorized.Message + ")";

					((System.Web.UI.WebControls.TreeNode)parent).ChildNodes.Add(node);
				}
			}
			catch (IOException eIOException)
			{
				if (Object.ReferenceEquals(parent.GetType(), winNode.GetType()))
				{
					System.Windows.Forms.TreeNode node = new System.Windows.Forms.TreeNode();
					node.Text = " (IOException: " + eIOException.Message + ")";
					
					((System.Windows.Forms.TreeNode)parent).Nodes.Add(node);
				}
				else
				{
					System.Web.UI.WebControls.TreeNode node = new System.Web.UI.WebControls.TreeNode();
					node.Text = " (IOException: " + eIOException.Message + ")";

					((System.Web.UI.WebControls.TreeNode)parent).ChildNodes.Add(node);
				}
			}
			catch (Exception e)
			{
				if (Object.ReferenceEquals(parent.GetType(), winNode.GetType()))
				{
					System.Windows.Forms.TreeNode node = new System.Windows.Forms.TreeNode();
					node.Text = " (Unknown Error: " + e.Message + ")";
					
					((System.Windows.Forms.TreeNode)parent).Nodes.Add(node);
				}
				else
				{
					System.Web.UI.WebControls.TreeNode node = new System.Web.UI.WebControls.TreeNode();
					node.Text = " (Unknown Error: " + e.Message + ")";

					((System.Web.UI.WebControls.TreeNode)parent).ChildNodes.Add(node);
				}
			}
		}
		#endregion

		#region LoadDirectoriesAsStringArray
		/// <summary>
		/// This function returns a complete directory (and subdirectories) with all files as a "flat" string array
		/// </summary>
		/// <param name="path">The path to the directory from which the information should be retrieved</param>
		/// <param name="SearchPattern">*.* is the default pattern, any other - valid - pattern is allowed</param>
		/// <param name="SubDirectories">false is the default parameter, true means all subdirectories are returned as well.</param>
		/// <returns>This function returns a complete directory (and subdirectories) with all files as a "flat" string array</returns>
		public string[] LoadDirectoriesAsStringArray(string path = "", string SearchPattern = "*.*", bool SubDirectories = false, bool GetFiles = true, bool PrintDirsSeperately = true)
		{
			// initializing further needed variables
			DirectoryInfo directory = new DirectoryInfo(path);
			// initializes the array with elements representing the sum of Directories and Files within the path
			string[] retValue = new string[directory.GetDirectories().Count() + directory.GetFiles().Count()];
			// initialize the entry counter
			long x = 0; 

			try
			{                
				foreach (DirectoryInfo dirInfo in directory.GetDirectories())
				{
					retValue[x] = dirInfo.Name;
					//retValue[x] = dirInfo.FullName;                    

					// first let's check whether the array is still large enough ;)
					if (x + 1 < retValue.GetUpperBound(0))
						x++;
					else
					{
						// we resize the array with 100 
						Array.Resize<string>(ref retValue, retValue.GetUpperBound(0) + 100);
						x++;
					}
					
					if (SubDirectories == true)
					{
						// Recurse the current directory
                        LoadDirectoriesAsStringArray(dirInfo.FullName, SearchPattern, SubDirectories);
						//string[] subRetValue = LoadDirectoriesAsStringArray(dirInfo.FullName, SearchPattern, SubDirectories);                        
					}
				}                
			}
			catch (System.UnauthorizedAccessException eUnauthorized)
			{
				retValue[x] = " (Access Denied - " + eUnauthorized.Message + ")";
			}
			catch (IOException eIOException)
			{                    
				retValue[x] = " (IOException: " + eIOException.Message + ")";
			}
			catch (Exception e)
			{
				retValue[x] = " (Unknown Error: " + e.Message + ")";
			}

			return retValue;
		}
		#endregion
		#region FileMD5andSHA1
		/// <summary>
		/// Simple enum for computeFileHash
		/// </summary>
		public enum HashType
		{
			MD5,
			SHA1
		}

		/// <summary>
		/// Compares two files on base of their MD5 has.
		/// </summary>
		/// <param name="File1">The full path to file 1.</param>
		/// <param name="File2">The full path to file 2.</param>
		/// <returns>true if both files are equal, false if not.</returns>
		public bool CompareFiles(string File1, string File2)
		{
			return (computeFileHash(File1, HashType.MD5) == computeFileHash(File2, HashType.MD5));
		}

		/// <summary>
		/// computes either MD5 or SHA1 file hash from file given in Path
		/// </summary>
		/// <param name="Path">The path to the file the MD5 / SHA1 file cache should be created from.</param>
		/// <param name="Hash">The hash type, either MD5 oder SHA1.</param>
		/// <returns>A string with either MD5 or SHA1.</returns>
		public string computeFileHash(string Path, HashType Hash)
		{
			object CryptProvider;	// We use the CryptProvider object for either the MD5 or SHA1 CryptoProvider

			if (Hash == HashType.MD5)	// this is the switch we use for either MD5 or SHA1 to initialize the CryptProvider object
				CryptProvider = new MD5CryptoServiceProvider();
			else
				CryptProvider = new SHA1CryptoServiceProvider();

			// before we start, let's check if the file exist
			if (File.Exists(Path))
			{
				// well, let's try
				try
				{
					//MD5 md5 = new MD5CryptoServiceProvider();
					byte[] result;
					if (Hash == HashType.MD5)
						result = ((MD5CryptoServiceProvider)CryptProvider).ComputeHash(File.Open(Path, FileMode.Open));
					else
						result = ((SHA1CryptoServiceProvider)CryptProvider).ComputeHash(File.Open(Path, FileMode.Open));

					return result.ToString();
				}
				catch (IOException eIO)
				{
					EventLog.WriteEntry(Application.ProductName + "; V" + Application.ProductVersion, eIO.ToString());
					throw (eIO);
				}
				catch (Exception err)
				{
					EventLog.WriteEntry(Application.ProductName + "; V" + Application.ProductVersion, err.ToString());
					throw (err);
				}
			}
			else
			{
				throw new Exception(string.Format("computeFileHash: File {0} does not exist!", Path));
			}
		}
		#endregion

		#region GetFileEncoding
		/// <summary>
		/// This simply retrieves the file encoding and returns it to the user. If file encoding couldn't be retrieved,
		/// Encoding.Default is returned.
		/// </summary>
		/// <param name="FilePath">The path to the file from which we want to know which encoding it is using.</param>
		/// <returns>The Encoding as System.Text.Encoding</returns>
		public Encoding GetFileEncoding(string FilePath)
		{
			Encoding retValue = Encoding.Default;   // just the fall back if we can't figure out the right encoding

			if ((FilePath != "") && (File.Exists(FilePath)))
			{
				try
				{
					using (StreamReader sr = new StreamReader(FilePath))
					{
						int zw = sr.Read();                 // correct encoding can be retrieved only after initial read is done
						retValue = sr.CurrentEncoding;
					}
				}
				// for now, we just bubble the error - there is no need to handle it here...
				catch (System.UnauthorizedAccessException eUnauthorized)
				{
					throw eUnauthorized;
				}
				catch (IOException eIOException)
				{
					throw eIOException;
				}
				catch (Exception e)
				{
					throw e;
				}
			}

			return retValue;
		}
		#endregion

		#region Log
		public delegate void AddLogMessageDelegate(bool condition, string message);
		public AddLogMessageDelegate AddLogMessageValue;

		private void AddLogMessage(bool condition, string message)
		{
			if (AddLogMessageValue != null)
				AddLogMessageValue(condition, message);
			else
				System.Diagnostics.Debug.WriteLineIf(condition, message);
		}
		#endregion
	}
}