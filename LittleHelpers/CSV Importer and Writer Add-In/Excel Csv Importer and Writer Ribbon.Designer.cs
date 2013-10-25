namespace CSV_Importer_and_Writer_Add_In
{
	partial class Excel_Csv_Importer_and_Writer_Ribbon : Microsoft.Office.Tools.Ribbon.RibbonBase
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		public Excel_Csv_Importer_and_Writer_Ribbon()
			: base(Globals.Factory.GetRibbonFactory())
		{
			InitializeComponent();
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Excel_Csv_Importer_and_Writer_Ribbon));
			this.CSVImportExport = this.Factory.CreateRibbonTab();
			this.Action = this.Factory.CreateRibbonGroup();
			this.CsvSettings = this.Factory.CreateRibbonGroup();
			this.chkHasHeadline = this.Factory.CreateRibbonCheckBox();
			this.chkVisible = this.Factory.CreateRibbonCheckBox();
			this.csvSeperator = this.Factory.CreateRibbonEditBox();
			this.chkVerbose = this.Factory.CreateRibbonCheckBox();
			this.FileDialogImport = new System.Windows.Forms.OpenFileDialog();
			this.FileDialogExport = new System.Windows.Forms.SaveFileDialog();
			this.ButtonCsvImport = this.Factory.CreateRibbonButton();
			this.ButtonCsvExport = this.Factory.CreateRibbonButton();
			this.CSVImportExport.SuspendLayout();
			this.Action.SuspendLayout();
			this.CsvSettings.SuspendLayout();
			// 
			// CSVImportExport
			// 
			this.CSVImportExport.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
			this.CSVImportExport.Groups.Add(this.Action);
			this.CSVImportExport.Groups.Add(this.CsvSettings);
			resources.ApplyResources(this.CSVImportExport, "CSVImportExport");
			this.CSVImportExport.Name = "CSVImportExport";
			// 
			// Action
			// 
			this.Action.Items.Add(this.ButtonCsvImport);
			this.Action.Items.Add(this.ButtonCsvExport);
			resources.ApplyResources(this.Action, "Action");
			this.Action.Name = "Action";
			// 
			// CsvSettings
			// 
			this.CsvSettings.Items.Add(this.chkHasHeadline);
			this.CsvSettings.Items.Add(this.chkVisible);
			this.CsvSettings.Items.Add(this.csvSeperator);
			this.CsvSettings.Items.Add(this.chkVerbose);
			resources.ApplyResources(this.CsvSettings, "CsvSettings");
			this.CsvSettings.Name = "CsvSettings";
			// 
			// chkHasHeadline
			// 
			resources.ApplyResources(this.chkHasHeadline, "chkHasHeadline");
			this.chkHasHeadline.Name = "chkHasHeadline";
			this.chkHasHeadline.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.chkHasHeadline_Click);
			// 
			// chkVisible
			// 
			this.chkVisible.Checked = true;
			resources.ApplyResources(this.chkVisible, "chkVisible");
			this.chkVisible.Name = "chkVisible";
			// 
			// csvSeperator
			// 
			resources.ApplyResources(this.csvSeperator, "csvSeperator");
			this.csvSeperator.Name = "csvSeperator";
			// 
			// chkVerbose
			// 
			this.chkVerbose.Checked = true;
			resources.ApplyResources(this.chkVerbose, "chkVerbose");
			this.chkVerbose.Name = "chkVerbose";
			// 
			// FileDialogImport
			// 
			this.FileDialogImport.FileName = "Please choose a file";
			resources.ApplyResources(this.FileDialogImport, "FileDialogImport");
			// 
			// FileDialogExport
			// 
			this.FileDialogExport.CreatePrompt = true;
			resources.ApplyResources(this.FileDialogExport, "FileDialogExport");
			// 
			// ButtonCsvImport
			// 
			this.ButtonCsvImport.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
			this.ButtonCsvImport.Image = global::CSV_Importer_and_Writer_Add_In.Properties.Resources.CSVImport;
			resources.ApplyResources(this.ButtonCsvImport, "ButtonCsvImport");
			this.ButtonCsvImport.Name = "ButtonCsvImport";
			this.ButtonCsvImport.ShowImage = true;
			this.ButtonCsvImport.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.ButtonCsvImport_Click);
			// 
			// ButtonCsvExport
			// 
			this.ButtonCsvExport.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
			this.ButtonCsvExport.Image = global::CSV_Importer_and_Writer_Add_In.Properties.Resources.CSVExport;
			resources.ApplyResources(this.ButtonCsvExport, "ButtonCsvExport");
			this.ButtonCsvExport.Name = "ButtonCsvExport";
			this.ButtonCsvExport.ShowImage = true;
			this.ButtonCsvExport.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.ButtonCsvExport_Click);
			// 
			// Excel_Csv_Importer_and_Writer_Ribbon
			// 
			this.Name = "Excel_Csv_Importer_and_Writer_Ribbon";
			this.RibbonType = "Microsoft.Excel.Workbook";
			this.Tabs.Add(this.CSVImportExport);
			this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.Excel_Csv_Importer_and_Writer_Ribbon_Load);
			this.CSVImportExport.ResumeLayout(false);
			this.CSVImportExport.PerformLayout();
			this.Action.ResumeLayout(false);
			this.Action.PerformLayout();
			this.CsvSettings.ResumeLayout(false);
			this.CsvSettings.PerformLayout();

		}

		#endregion

		internal Microsoft.Office.Tools.Ribbon.RibbonTab CSVImportExport;
		internal Microsoft.Office.Tools.Ribbon.RibbonGroup Action;
		internal Microsoft.Office.Tools.Ribbon.RibbonGroup CsvSettings;
		internal Microsoft.Office.Tools.Ribbon.RibbonButton ButtonCsvImport;
		internal Microsoft.Office.Tools.Ribbon.RibbonButton ButtonCsvExport;
		private System.Windows.Forms.OpenFileDialog FileDialogImport;
		private System.Windows.Forms.SaveFileDialog FileDialogExport;
		internal Microsoft.Office.Tools.Ribbon.RibbonCheckBox chkHasHeadline;
		internal Microsoft.Office.Tools.Ribbon.RibbonCheckBox chkVisible;
		internal Microsoft.Office.Tools.Ribbon.RibbonEditBox csvSeperator;
		internal Microsoft.Office.Tools.Ribbon.RibbonCheckBox chkVerbose;
	}

	partial class ThisRibbonCollection
	{
		internal Excel_Csv_Importer_and_Writer_Ribbon Excel_Csv_Imporeter_and_Writer_Ribbon
		{
			get
			{
				return this.GetRibbon<Excel_Csv_Importer_and_Writer_Ribbon>();
			}
		}
	}
}
