using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace GraphTesting
{
    public partial class Form1 : Form
    {

        DataTable restrictions;
        string destination;
        string name;

        public Form1()
        {
            InitializeComponent();

            restrictions = new DataTable("restrictionsDT");
            restrictions.Columns.Add("Out-Degree");
            restrictions.Columns.Add("Max Amount of Nodes");

            dataGridView1.DataSource = restrictions;
            
        }

        private void generateBTN_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(destination))
            {
                MessageBox.Show("Please select destination folder.", "Missing Input", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            try
            {
                this.name = graphNameTB.Text;
                int nodes = int.Parse(nodesTB.Value.ToString());
                int options = int.Parse(optionsTB.Value.ToString());
                int maxOut = int.Parse(outDegreeTB.Value.ToString());
                bool enforceSingle = singleSolutionCB.Checked;

                Dictionary<int, int> restrictionsCollection = new Dictionary<int, int>();

                foreach (DataRow row in this.restrictions.Rows)
                {
                    restrictionsCollection.Add(int.Parse(row[0].ToString()), int.Parse(row[1].ToString()));
                }

                Graph generated = GraphGenerator.Generate(name, nodes, options, maxOut, restrictionsCollection, enforceSingle);
                GraphML.Write(generated, destination);

                showFolderBTN.Enabled = true;

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void folderBrowserBTN_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                destination = folderBrowserDialog1.SelectedPath;
                folderName.Text = "(" + folderBrowserDialog1.SelectedPath.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries).Last<string>() + ")";
            }
        }

        private void showFolderBTN_Click(object sender, EventArgs e)
        {
            Process.Start(destination+"\\"+name+".xml");
        }
    }
}
