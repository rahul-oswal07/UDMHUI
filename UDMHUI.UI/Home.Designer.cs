namespace UDMHUI.UI
{
    partial class Home
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.lblMessage = new System.Windows.Forms.Label();
            this.button9 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.txtDBPath = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.button6 = new System.Windows.Forms.Button();
            this.txtLookupPath = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.button5 = new System.Windows.Forms.Button();
            this.txtPackagePath = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.button4 = new System.Windows.Forms.Button();
            this.txtDTExecPath = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.txtSQLUtilityPath = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.txtXO2Path = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtDatabaseName = new System.Windows.Forms.TextBox();
            this.txtSQLServerInstanceName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.btnReset = new System.Windows.Forms.Button();
            this.lstMessageBox = new System.Windows.Forms.ListBox();
            this.button2 = new System.Windows.Forms.Button();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.button10 = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.Location = new System.Drawing.Point(40, 27);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1120, 645);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.Transparent;
            this.tabPage1.Controls.Add(this.lblMessage);
            this.tabPage1.Controls.Add(this.button9);
            this.tabPage1.Controls.Add(this.button8);
            this.tabPage1.Controls.Add(this.button7);
            this.tabPage1.Controls.Add(this.txtDBPath);
            this.tabPage1.Controls.Add(this.label8);
            this.tabPage1.Controls.Add(this.button6);
            this.tabPage1.Controls.Add(this.txtLookupPath);
            this.tabPage1.Controls.Add(this.label7);
            this.tabPage1.Controls.Add(this.button5);
            this.tabPage1.Controls.Add(this.txtPackagePath);
            this.tabPage1.Controls.Add(this.label6);
            this.tabPage1.Controls.Add(this.button4);
            this.tabPage1.Controls.Add(this.txtDTExecPath);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.button3);
            this.tabPage1.Controls.Add(this.txtSQLUtilityPath);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.button1);
            this.tabPage1.Controls.Add(this.txtXO2Path);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.txtDatabaseName);
            this.tabPage1.Controls.Add(this.txtSQLServerInstanceName);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Location = new System.Drawing.Point(4, 29);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(4);
            this.tabPage1.Size = new System.Drawing.Size(1112, 612);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Configuration";
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Location = new System.Drawing.Point(35, 525);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(0, 20);
            this.lblMessage.TabIndex = 25;
            this.lblMessage.Visible = false;
            // 
            // button9
            // 
            this.button9.Location = new System.Drawing.Point(478, 525);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(107, 41);
            this.button9.TabIndex = 24;
            this.button9.Text = "Reset";
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.OnResetButtonClick);
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(347, 525);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(109, 41);
            this.button8.TabIndex = 23;
            this.button8.Text = "Update";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.OnUpdateButtonClick);
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(804, 419);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(98, 29);
            this.button7.TabIndex = 22;
            this.button7.Text = "Browse";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.OnDBPathBrowseButtonClick);
            // 
            // txtDBPath
            // 
            this.txtDBPath.AllowDrop = true;
            this.txtDBPath.Location = new System.Drawing.Point(234, 419);
            this.txtDBPath.Margin = new System.Windows.Forms.Padding(4);
            this.txtDBPath.Name = "txtDBPath";
            this.txtDBPath.Size = new System.Drawing.Size(529, 26);
            this.txtDBPath.TabIndex = 21;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(31, 425);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(181, 20);
            this.label8.TabIndex = 20;
            this.label8.Text = "DB Path (LDF,MDF)";
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(804, 369);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(98, 29);
            this.button6.TabIndex = 19;
            this.button6.Text = "Browse";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.OnLookupBrowseButtonClick);
            // 
            // txtLookupPath
            // 
            this.txtLookupPath.AllowDrop = true;
            this.txtLookupPath.Location = new System.Drawing.Point(234, 369);
            this.txtLookupPath.Margin = new System.Windows.Forms.Padding(4);
            this.txtLookupPath.Name = "txtLookupPath";
            this.txtLookupPath.Size = new System.Drawing.Size(529, 26);
            this.txtLookupPath.TabIndex = 18;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(31, 375);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(113, 20);
            this.label7.TabIndex = 17;
            this.label7.Text = "Lookup Path";
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(804, 320);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(98, 29);
            this.button5.TabIndex = 16;
            this.button5.Text = "Browse";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.OnPackagePathBrowseButtonClick);
            // 
            // txtPackagePath
            // 
            this.txtPackagePath.AllowDrop = true;
            this.txtPackagePath.Location = new System.Drawing.Point(234, 320);
            this.txtPackagePath.Margin = new System.Windows.Forms.Padding(4);
            this.txtPackagePath.Name = "txtPackagePath";
            this.txtPackagePath.Size = new System.Drawing.Size(529, 26);
            this.txtPackagePath.TabIndex = 15;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(31, 320);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(124, 20);
            this.label6.TabIndex = 14;
            this.label6.Text = "Package Path";
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(804, 264);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(98, 29);
            this.button4.TabIndex = 13;
            this.button4.Text = "Browse";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.OnDTExeBrowseButtonClick);
            // 
            // txtDTExecPath
            // 
            this.txtDTExecPath.AllowDrop = true;
            this.txtDTExecPath.Location = new System.Drawing.Point(234, 264);
            this.txtDTExecPath.Margin = new System.Windows.Forms.Padding(4);
            this.txtDTExecPath.Name = "txtDTExecPath";
            this.txtDTExecPath.Size = new System.Drawing.Size(529, 26);
            this.txtDTExecPath.TabIndex = 12;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(31, 270);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(119, 20);
            this.label5.TabIndex = 11;
            this.label5.Text = "DTExec Path";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(804, 208);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(98, 29);
            this.button3.TabIndex = 10;
            this.button3.Text = "Browse";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.OnSQLUtilityPathBrowseButtonClick);
            // 
            // txtSQLUtilityPath
            // 
            this.txtSQLUtilityPath.AllowDrop = true;
            this.txtSQLUtilityPath.Location = new System.Drawing.Point(234, 208);
            this.txtSQLUtilityPath.Margin = new System.Windows.Forms.Padding(4);
            this.txtSQLUtilityPath.Name = "txtSQLUtilityPath";
            this.txtSQLUtilityPath.Size = new System.Drawing.Size(529, 26);
            this.txtSQLUtilityPath.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(31, 214);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(145, 20);
            this.label4.TabIndex = 8;
            this.label4.Text = "SQL Utility Path";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(804, 154);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(98, 29);
            this.button1.TabIndex = 7;
            this.button1.Text = "Browse";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.OnXO2PathBrowseButtonClick);
            // 
            // txtXO2Path
            // 
            this.txtXO2Path.Location = new System.Drawing.Point(234, 154);
            this.txtXO2Path.Margin = new System.Windows.Forms.Padding(4);
            this.txtXO2Path.Name = "txtXO2Path";
            this.txtXO2Path.Size = new System.Drawing.Size(529, 26);
            this.txtXO2Path.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(31, 154);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(97, 20);
            this.label3.TabIndex = 5;
            this.label3.Text = "X02 Path: ";
            // 
            // txtDatabaseName
            // 
            this.txtDatabaseName.Location = new System.Drawing.Point(234, 103);
            this.txtDatabaseName.Margin = new System.Windows.Forms.Padding(4);
            this.txtDatabaseName.Name = "txtDatabaseName";
            this.txtDatabaseName.Size = new System.Drawing.Size(529, 26);
            this.txtDatabaseName.TabIndex = 4;
            // 
            // txtSQLServerInstanceName
            // 
            this.txtSQLServerInstanceName.Location = new System.Drawing.Point(234, 39);
            this.txtSQLServerInstanceName.Margin = new System.Windows.Forms.Padding(4);
            this.txtSQLServerInstanceName.Name = "txtSQLServerInstanceName";
            this.txtSQLServerInstanceName.Size = new System.Drawing.Size(529, 26);
            this.txtSQLServerInstanceName.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(31, 103);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(155, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "Database Name: ";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(31, 39);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(194, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "SQL server Instance: ";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.button10);
            this.tabPage2.Controls.Add(this.btnReset);
            this.tabPage2.Controls.Add(this.lstMessageBox);
            this.tabPage2.Controls.Add(this.button2);
            this.tabPage2.Location = new System.Drawing.Point(4, 29);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(4);
            this.tabPage2.Size = new System.Drawing.Size(1112, 612);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "DB Setup";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(157, 258);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(120, 46);
            this.btnReset.TabIndex = 3;
            this.btnReset.Text = "RESET";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.OnLogResetButtonClick);
            // 
            // lstMessageBox
            // 
            this.lstMessageBox.AccessibleDescription = "";
            this.lstMessageBox.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.lstMessageBox.FormattingEnabled = true;
            this.lstMessageBox.ItemHeight = 20;
            this.lstMessageBox.Location = new System.Drawing.Point(7, 7);
            this.lstMessageBox.Name = "lstMessageBox";
            this.lstMessageBox.Size = new System.Drawing.Size(723, 224);
            this.lstMessageBox.TabIndex = 2;
            this.lstMessageBox.SelectedIndexChanged += new System.EventHandler(this.lstMessageBox_SelectedIndexChanged);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(8, 258);
            this.button2.Margin = new System.Windows.Forms.Padding(4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(120, 46);
            this.button2.TabIndex = 1;
            this.button2.Text = "CreateDB";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.OnCreateDBButtonClick);
            // 
            // tabPage3
            // 
            this.tabPage3.Location = new System.Drawing.Point(4, 29);
            this.tabPage3.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(1112, 612);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "tabPage3";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            this.tabPage4.Location = new System.Drawing.Point(4, 29);
            this.tabPage4.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(1112, 612);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "tabPage4";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // tabPage5
            // 
            this.tabPage5.Location = new System.Drawing.Point(4, 29);
            this.tabPage5.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Size = new System.Drawing.Size(1112, 612);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "tabPage5";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // button10
            // 
            this.button10.Location = new System.Drawing.Point(313, 258);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(134, 46);
            this.button10.TabIndex = 4;
            this.button10.Text = "LoadLookup";
            this.button10.UseVisualStyleBackColor = true;
            this.button10.Click += new System.EventHandler(this.OnLoadLookupButtonClick);
            // 
            // Home
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1673, 950);
            this.Controls.Add(this.tabControl1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Home";
            this.Text = "UDMH Run";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        /// <summary>
        /// Loads the default values from Config file
        /// </summary>

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtXO2Path;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtDatabaseName;
        private System.Windows.Forms.TextBox txtSQLServerInstanceName;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox txtSQLUtilityPath;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.TextBox txtDTExecPath;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.TextBox txtLookupPath;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.TextBox txtPackagePath;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.TextBox txtDBPath;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.ListBox lstMessageBox;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button button10;
    }
}

