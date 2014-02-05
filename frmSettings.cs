using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;


namespace FileRenameUtility
{
    public partial class frmSettings : Form
    {

        private string defaultDirectory;

         public frmSettings()
        {
            InitializeComponent();
        

        }

        private void frmSettings_Load(object sender, EventArgs e)
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
                                    txtSourcePattern.Text = objXmlTextReader.Value;
                                    break;
                                case "TargetPattern":
                                    txtTargetPattern.Text = objXmlTextReader.Value;
                                    break;
                                case "DefaultDirectory":
                                    defaultDirectory = objXmlTextReader.Value;
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

        private void btnSave_Click(object sender, EventArgs e)
        {

            // save values to an xml file

            try
            {
                string sStartupPath = Application.StartupPath;
                XmlTextWriter objXmlTextWriter =  new XmlTextWriter(sStartupPath + @"\settings.xml",null);
                objXmlTextWriter.Formatting = Formatting.Indented;
                objXmlTextWriter.WriteStartDocument();
                objXmlTextWriter.WriteStartElement("ApplicationSettings");
                objXmlTextWriter.WriteStartElement("SourcePattern");
                objXmlTextWriter.WriteString(txtSourcePattern.Text);
                objXmlTextWriter.WriteEndElement();
                objXmlTextWriter.WriteStartElement("TargetPattern");
                objXmlTextWriter.WriteString(txtTargetPattern.Text);
                objXmlTextWriter.WriteEndElement();
                objXmlTextWriter.WriteStartElement("DefaultDirectory");
                objXmlTextWriter.WriteString( defaultDirectory);
                objXmlTextWriter.WriteEndElement();

                objXmlTextWriter.WriteEndElement();
                objXmlTextWriter.WriteEndDocument();
                objXmlTextWriter.Flush();
                objXmlTextWriter.Close();

                this.Dispose();

                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }




        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();

        }
    }
}