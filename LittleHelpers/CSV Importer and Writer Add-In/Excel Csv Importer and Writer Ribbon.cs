using Microsoft.Office.Tools.Ribbon;

using System.Windows.Forms;

//using Application = Microsoft.Office.Interop.Excel.Application;

namespace CSV_Importer_and_Writer_Add_In
{
    public partial class Excel_Csv_Importer_and_Writer_Ribbon
    {
        private void Excel_Csv_Importer_and_Writer_Ribbon_Load(object sender, RibbonUIEventArgs e)
        {
        }

        private void ButtonCsvImport_Click(object sender, RibbonControlEventArgs e)
        {
            // First, we need a filename and path
            FileDialogImport.Filter = "CSV (.csv) |*.csv|TXT (.txt)|*.txt|TSV (*.tsv)|*.tsv|All files (*.*)|*.*";
            FileDialogImport.FilterIndex = 1;
            FileDialogImport.SupportMultiDottedExtensions = true;
            FileDialogImport.CheckFileExists = true;
            FileDialogImport.Title = "Please choose the file to be imported:";

            DialogResult FileDialogResult = FileDialogImport.ShowDialog();
            int NoColumns = 0;

            if (FileDialogResult == DialogResult.OK)
            {
                // TODO: Implement that a user can deliver RangeFrom and RangeTo
                string RangeFrom = "";
                string RangeTo = "";

                Globals.ThisAddIn.ImportCSV(FileDialogImport.FileName, RangeFrom, RangeTo, csvSeperator.Text, chkHasHeadline.Checked, NoColumns, chkVisible.Checked, chkVerbose.Checked);
            }
        }

        private void ButtonCsvExport_Click(object sender, RibbonControlEventArgs e)
        {
            // First, we need a filename and path
            FileDialogExport.Filter = "CSV (.csv) |*.csv|TXT (.txt)|*.txt|TSV (*.tsv)|*.tsv|All files (*.*)|*.*";
            FileDialogExport.FilterIndex = 1;
            FileDialogExport.SupportMultiDottedExtensions = true;
            FileDialogExport.CheckFileExists = false;
            FileDialogImport.Title = "Please choose the file to be exported to:";

            DialogResult FileDialogResult = FileDialogExport.ShowDialog();

            if (FileDialogResult == DialogResult.OK)
            {
                // TODO: Implement that a user can deliver RangeFrom and RangeTo
                string RangeFrom = "";
                string RangeTo = "";

                Globals.ThisAddIn.Export2CSV(FileDialogExport.FileName, RangeFrom, RangeTo, csvSeperator.Text, chkHasHeadline.Checked, chkVisible.Checked, chkVerbose.Checked);
            }
        }

        private void chkHasHeadline_Click(object sender, RibbonControlEventArgs e)
        {
        }
    }
}