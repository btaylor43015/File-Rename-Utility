using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;


namespace FileRenameUtility
{
    public partial class frmMain : Form
    {
        string[]  lFiles;
        settings appSettings;

        public frmMain()
        {
            InitializeComponent();
            appSettings = new settings();
            
            // set the defaults here for now
            appSettings.sDefaultDir = "Desktop";
            appSettings.sSourcePattern = "IMG_*.*";
            appSettings.sTargetPattern = "<date>*.*";

        }
        private void showSettings()
        {

            // load the settings form

            frmSettings fm = new frmSettings();
            fm.Show();

        }
        private void loadSettings()
        {
            // load settings from xml file
            try
            {
                string sStartupPath = Application.StartupPath;
                //   clsSValidator objclsSValidator =  new clsSValidator(sStartupPath + @"settings.xml",
                //       sStartupPath + @"..\..\..\XMLFile1.xsd");
                // if (objclsSValidator.ValidateXMLFile()) return;
                XmlTextReader objXmlTextReader = new XmlTextReader(sStartupPath + @"\settings.xml");
                string sName = "";
                while (objXmlTextReader.Read())
                {
                    switch (objXmlTextReader.NodeType)
                    {
                        case XmlNodeType.Element:
                            sName = objXmlTextReader.Name;
                            break;
                        case XmlNodeType.Text:
                            switch (sName)
                            {
                                case "SourcePattern":
                                    appSettings.sSourcePattern = objXmlTextReader.Value;
                                   
                                    break;
                                case "TargetPattern":
                                   appSettings.sTargetPattern = objXmlTextReader.Value;
                                  
                                    break;
                                case "DefaultDirectory":
                                    appSettings.sDefaultDir = objXmlTextReader.Value;
                                    if (appSettings.sDefaultDir != "")
                                        txtFilesPath.Text = appSettings.sDefaultDir;

                                    break;
                            }
                            break;
                    }
                }
                objXmlTextReader.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void checkAllFiles(bool bChecked)
        {

            // set all items to checked or not
            for (int x = 0; x < lbFiles.Items.Count; x++)
            {
                lbFiles.SetItemChecked(x, bChecked);

            }

        }
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            string searchPattern = "*.*"; 
            string sDirectory;
            string[] fnParts;
         
            // clear the list box
            lbFiles.Items.Clear();
        
            // show folder browser dialog
            fbDialog.SelectedPath = appSettings.sDefaultDir;
            fbDialog.ShowDialog();

            // show the path in the text box
            txtFilesPath.Text = fbDialog.SelectedPath;

            // show the files in the list at this path         
            sDirectory= fbDialog.SelectedPath;
            appSettings.sDefaultDir = sDirectory;

            if (sDirectory != "")
            {
                lFiles = Directory.GetFiles(sDirectory, searchPattern);

                foreach (string file in lFiles)
                {
                    // add each file to the list box. But only the file name
                    fnParts = file.Split('\\');
                    lbFiles.Items.Add(fnParts[fnParts.GetUpperBound(0)]);
                }

                // set all files to be checked by default
                checkAllFiles(true);

                try
                {
                    string sStartupPath = Application.StartupPath;
                    XmlTextWriter objXmlTextWriter = new XmlTextWriter(sStartupPath + @"\settings.xml", null);
                    objXmlTextWriter.Formatting = Formatting.Indented;
                    objXmlTextWriter.WriteStartDocument();
                    objXmlTextWriter.WriteStartElement("ApplicationSettings");
                    objXmlTextWriter.WriteStartElement("SourcePattern");
                    objXmlTextWriter.WriteString(appSettings.sSourcePattern);
                    objXmlTextWriter.WriteEndElement();
                    objXmlTextWriter.WriteStartElement("TargetPattern");
                    objXmlTextWriter.WriteString(appSettings.sTargetPattern);
                    objXmlTextWriter.WriteEndElement();
                    objXmlTextWriter.WriteStartElement("DefaultDirectory");
                    objXmlTextWriter.WriteString(sDirectory);
                    objXmlTextWriter.WriteEndElement();
                    objXmlTextWriter.WriteEndElement();
                    objXmlTextWriter.WriteEndDocument();
                    objXmlTextWriter.Flush();
                    objXmlTextWriter.Close();

                   


                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }
        private string renameFile(string fileName)
        {
            // this routine will rename the file using the patterns in the application settings
            string[] filePath;
            string retFilename = "";
            string origFilename;
            string retPath;
            string[] remString;
            int insPos = 0;
            int astPos = 0;
       
            string strDate;

            // get the filename from the path
            filePath = fileName.Split('\\');
            retFilename = filePath[filePath.GetUpperBound(0)];
            origFilename = retFilename;


            remString = appSettings.sTargetPattern.Split('*');
            if (remString != null)
            {
                // remove the special fields from the template parts to leave any extra chars to be added
                for (int x = 0; x < remString.GetUpperBound(0) + 1; x++)
                    remString[x] = remString[x].Replace("<date>", "");

                // look for new characters to add to the filename
                for (int x = 0; x < remString.GetUpperBound(0) + 1; x++)
                {

                    if (remString[x] != "*" && remString[x] != "" && remString[x] != ".")
                    { // not a special field, add it to the filename
                        if (x == 0)
                            retFilename = retFilename.Insert(0, remString[0]);
                        else
                            retFilename = retFilename.Insert(retFilename.IndexOf(".") - 1, remString[x]);
                    }

                }

            } 
               
            // look for characters that are in the original pattern but not in the target pattern
            remString = appSettings.sSourcePattern.Split('*');
            if (remString != null)
            {
                if (!appSettings.sTargetPattern.Contains(remString[0]))
                    retFilename = retFilename.Replace(remString[0], "");


                astPos = appSettings.sTargetPattern.IndexOf("*");

                // look for special fields to be added
                if (appSettings.sTargetPattern.Contains("<date>"))
                {
                    // need to add the date to the filename
                    strDate = System.DateTime.Now.Date.Month.ToString() + System.DateTime.Now.Date.Day.ToString()
                        + System.DateTime.Now.Date.Year.ToString();


                    // figure out where to put it
                    insPos = appSettings.sTargetPattern.IndexOf("<date>");

                    //perPos = retFilename.IndexOf(".");

                    if (insPos == 0)
                        // add the date to the beginning of the filename
                        retFilename = retFilename.Insert(0, strDate);
                    else if (insPos > astPos)
                        // add the date after the asterick
                        retFilename = retFilename.Insert(retFilename.IndexOf(".") - 1, strDate);

                }
            }

            
           
           
            // add the file name to the rest of the path
            retPath = fileName.Replace(origFilename, retFilename);

            try
            {
                // rename the file on the disk
                FileInfo fi = new FileInfo(fileName);
                fi.MoveTo(retPath);
            }
            catch (Exception ex )
            {
                MessageBox.Show(ex.ToString());
            }


            return retPath;

        }
        
        private void btnSelectNone_Click(object sender, EventArgs e)
        {
            checkAllFiles(false);

        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            // set all items to checked
            checkAllFiles(true);

        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {

            showSettings();
           
        }

        private void btnRename_Click(object sender, EventArgs e)
        {
            // load the xml settings
            loadSettings();

            // rename all files that have been checked
            string[] sPath;
            string nFilename = "";

            if (MessageBox.Show("Rename selected files from: " + appSettings.sSourcePattern + " to: " + appSettings.sTargetPattern + "?", 
                "Confirm Rename", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                // go through all of the list to see which ones are checked.
                for (int x = 0; x < lbFiles.Items.Count; x++)
                {
                    if (lbFiles.CheckedIndices.Contains(x))
                    {
                        // if file checked, rename
                        nFilename = renameFile(lFiles[x]);
                        sPath = nFilename.Split('\\');

                        // change the name in the list box
                        lbFiles.Items.RemoveAt(x);
                        lbFiles.Items.Insert(x, sPath[sPath.GetUpperBound(0)]);

                        // update the file name array
                        lFiles[x] = nFilename;


                    }

                }
            }

            // rename complete, reload list box

        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            string searchPattern = "*.*";
            string[] fnParts;

            // load the settings
            loadSettings();

            // load up the initial list box if a default directory exists
            if (appSettings.sDefaultDir != "")
            {
                try
                {
                    lFiles = Directory.GetFiles(appSettings.sDefaultDir, searchPattern);

                    foreach (string file in lFiles)
                    {
                        // add each file to the list box. But only the file name
                        fnParts = file.Split('\\');
                        lbFiles.Items.Add(fnParts[fnParts.GetUpperBound(0)]);
                    }

                    // set all files to be checked by default
                    checkAllFiles(true);

                }
                catch 
                {
                    // problem getting files from default directory. reset to ""
                    appSettings.sDefaultDir = "";

                }

            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            showSettings();

        }

        private void lbFiles_SelectedIndexChanged(object sender, EventArgs e)
        {





        }
    }
}