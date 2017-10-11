namespace GraphTesting
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label2 = new System.Windows.Forms.Label();
            this.nodesTB = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.optionsTB = new System.Windows.Forms.NumericUpDown();
            this.outDegreeTB = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.singleSolutionCB = new System.Windows.Forms.CheckBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.generateBTN = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.graphNameTB = new System.Windows.Forms.TextBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.folderBrowserBTN = new System.Windows.Forms.Button();
            this.folderName = new System.Windows.Forms.Label();
            this.showFolderBTN = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nodesTB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.optionsTB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.outDegreeTB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 84);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Nodes Count:";
            // 
            // nodesTB
            // 
            this.nodesTB.Location = new System.Drawing.Point(112, 82);
            this.nodesTB.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nodesTB.Name = "nodesTB";
            this.nodesTB.Size = new System.Drawing.Size(100, 20);
            this.nodesTB.TabIndex = 3;
            this.nodesTB.Value = new decimal(new int[] {
            13,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 111);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Options Count:";
            // 
            // optionsTB
            // 
            this.optionsTB.Location = new System.Drawing.Point(112, 109);
            this.optionsTB.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.optionsTB.Name = "optionsTB";
            this.optionsTB.Size = new System.Drawing.Size(100, 20);
            this.optionsTB.TabIndex = 3;
            this.optionsTB.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // outDegreeTB
            // 
            this.outDegreeTB.Location = new System.Drawing.Point(112, 135);
            this.outDegreeTB.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.outDegreeTB.Name = "outDegreeTB";
            this.outDegreeTB.Size = new System.Drawing.Size(100, 20);
            this.outDegreeTB.TabIndex = 5;
            this.outDegreeTB.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(19, 137);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(88, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Max Out-Degree:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(19, 170);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(80, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "Single Solution:";
            // 
            // singleSolutionCB
            // 
            this.singleSolutionCB.AutoSize = true;
            this.singleSolutionCB.Location = new System.Drawing.Point(112, 170);
            this.singleSolutionCB.Name = "singleSolutionCB";
            this.singleSolutionCB.Size = new System.Drawing.Size(63, 17);
            this.singleSolutionCB.TabIndex = 7;
            this.singleSolutionCB.Text = "Enforce";
            this.singleSolutionCB.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(6, 19);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(466, 177);
            this.dataGridView1.TabIndex = 8;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.folderName);
            this.groupBox1.Controls.Add(this.folderBrowserBTN);
            this.groupBox1.Controls.Add(this.graphNameTB);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.singleSolutionCB);
            this.groupBox1.Controls.Add(this.nodesTB);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.optionsTB);
            this.groupBox1.Controls.Add(this.outDegreeTB);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Location = new System.Drawing.Point(10, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(307, 198);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "General Settings";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.dataGridView1);
            this.groupBox2.Location = new System.Drawing.Point(10, 216);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(481, 167);
            this.groupBox2.TabIndex = 14;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Restrictions";
            // 
            // generateBTN
            // 
            this.generateBTN.Location = new System.Drawing.Point(333, 54);
            this.generateBTN.Name = "generateBTN";
            this.generateBTN.Size = new System.Drawing.Size(149, 47);
            this.generateBTN.TabIndex = 15;
            this.generateBTN.Text = "Generate";
            this.generateBTN.UseVisualStyleBackColor = true;
            this.generateBTN.Click += new System.EventHandler(this.generateBTN_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Graph Name:";
            // 
            // graphNameTB
            // 
            this.graphNameTB.Location = new System.Drawing.Point(112, 23);
            this.graphNameTB.Name = "graphNameTB";
            this.graphNameTB.Size = new System.Drawing.Size(100, 20);
            this.graphNameTB.TabIndex = 9;
            // 
            // folderBrowserBTN
            // 
            this.folderBrowserBTN.Location = new System.Drawing.Point(22, 50);
            this.folderBrowserBTN.Name = "folderBrowserBTN";
            this.folderBrowserBTN.Size = new System.Drawing.Size(190, 23);
            this.folderBrowserBTN.TabIndex = 10;
            this.folderBrowserBTN.Text = "Export Destination...";
            this.folderBrowserBTN.UseVisualStyleBackColor = true;
            this.folderBrowserBTN.Click += new System.EventHandler(this.folderBrowserBTN_Click);
            // 
            // folderName
            // 
            this.folderName.AutoSize = true;
            this.folderName.Location = new System.Drawing.Point(231, 55);
            this.folderName.Name = "folderName";
            this.folderName.Size = new System.Drawing.Size(0, 13);
            this.folderName.TabIndex = 16;
            // 
            // showFolderBTN
            // 
            this.showFolderBTN.Enabled = false;
            this.showFolderBTN.Location = new System.Drawing.Point(333, 123);
            this.showFolderBTN.Name = "showFolderBTN";
            this.showFolderBTN.Size = new System.Drawing.Size(149, 47);
            this.showFolderBTN.TabIndex = 15;
            this.showFolderBTN.Text = "Open File";
            this.showFolderBTN.UseVisualStyleBackColor = true;
            this.showFolderBTN.Click += new System.EventHandler(this.showFolderBTN_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(499, 395);
            this.Controls.Add(this.showFolderBTN);
            this.Controls.Add(this.generateBTN);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.Text = "Geography Graph Generator";
            ((System.ComponentModel.ISupportInitialize)(this.nodesTB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.optionsTB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.outDegreeTB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nodesTB;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown optionsTB;
        private System.Windows.Forms.NumericUpDown outDegreeTB;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox singleSolutionCB;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button generateBTN;
        private System.Windows.Forms.Button folderBrowserBTN;
        private System.Windows.Forms.TextBox graphNameTB;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Label folderName;
        private System.Windows.Forms.Button showFolderBTN;
    }
}