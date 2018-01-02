namespace Car0
{
    partial class RobotData
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RobotData));
            this.RobotMatrixTextBox = new System.Windows.Forms.TextBox();
            this.RobotMatrixFileButton = new System.Windows.Forms.Button();
            this.SelectStationLabel = new System.Windows.Forms.Label();
            this.StationSelectListBox = new System.Windows.Forms.ListBox();
            this.SelectRobotLabel = new System.Windows.Forms.Label();
            this.RobotSelectListBox = new System.Windows.Forms.ListBox();
            this.RobotTypeLabel = new System.Windows.Forms.Label();
            this.ApplicationFramesCheckedListBox = new System.Windows.Forms.CheckedListBox();
            this.MakeFoldersButton = new System.Windows.Forms.Button();
            this.CheckRobotsButton = new System.Windows.Forms.Button();
            this.CheckProgressBar = new System.Windows.Forms.ProgressBar();
            this.BadApplicationsRichTextBox = new System.Windows.Forms.RichTextBox();
            this.BadAppLabel = new System.Windows.Forms.Label();
            this.CheckedLabel = new System.Windows.Forms.Label();
            this.WriteAcceptedChecksCheckBox = new System.Windows.Forms.CheckBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // RobotMatrixTextBox
            // 
            this.RobotMatrixTextBox.Location = new System.Drawing.Point(120, 12);
            this.RobotMatrixTextBox.Name = "RobotMatrixTextBox";
            this.RobotMatrixTextBox.ReadOnly = true;
            this.RobotMatrixTextBox.Size = new System.Drawing.Size(528, 20);
            this.RobotMatrixTextBox.TabIndex = 7;
            this.RobotMatrixTextBox.Text = "Not Set";
            // 
            // RobotMatrixFileButton
            // 
            this.RobotMatrixFileButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(225)))), ((int)(((byte)(225)))));
            this.RobotMatrixFileButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.RobotMatrixFileButton.Location = new System.Drawing.Point(12, 12);
            this.RobotMatrixFileButton.Name = "RobotMatrixFileButton";
            this.RobotMatrixFileButton.Size = new System.Drawing.Size(102, 20);
            this.RobotMatrixFileButton.TabIndex = 6;
            this.RobotMatrixFileButton.Text = "Robot Matrix File";
            this.RobotMatrixFileButton.UseVisualStyleBackColor = false;
            this.RobotMatrixFileButton.Click += new System.EventHandler(this.RobotMatrixFileButton_Click);
            // 
            // SelectStationLabel
            // 
            this.SelectStationLabel.AutoSize = true;
            this.SelectStationLabel.Location = new System.Drawing.Point(16, 44);
            this.SelectStationLabel.Name = "SelectStationLabel";
            this.SelectStationLabel.Size = new System.Drawing.Size(71, 13);
            this.SelectStationLabel.TabIndex = 8;
            this.SelectStationLabel.Text = "Select station";
            // 
            // StationSelectListBox
            // 
            this.StationSelectListBox.BackColor = System.Drawing.Color.PapayaWhip;
            this.StationSelectListBox.FormattingEnabled = true;
            this.StationSelectListBox.Location = new System.Drawing.Point(102, 44);
            this.StationSelectListBox.Name = "StationSelectListBox";
            this.StationSelectListBox.Size = new System.Drawing.Size(191, 69);
            this.StationSelectListBox.Sorted = true;
            this.StationSelectListBox.TabIndex = 10;
            this.StationSelectListBox.SelectedIndexChanged += new System.EventHandler(this.StationSelectListBox_SelectedIndexChanged);
            // 
            // SelectRobotLabel
            // 
            this.SelectRobotLabel.AutoSize = true;
            this.SelectRobotLabel.Location = new System.Drawing.Point(320, 44);
            this.SelectRobotLabel.Name = "SelectRobotLabel";
            this.SelectRobotLabel.Size = new System.Drawing.Size(64, 13);
            this.SelectRobotLabel.TabIndex = 11;
            this.SelectRobotLabel.Text = "Select robot";
            // 
            // RobotSelectListBox
            // 
            this.RobotSelectListBox.BackColor = System.Drawing.Color.PapayaWhip;
            this.RobotSelectListBox.FormattingEnabled = true;
            this.RobotSelectListBox.Location = new System.Drawing.Point(405, 44);
            this.RobotSelectListBox.Name = "RobotSelectListBox";
            this.RobotSelectListBox.Size = new System.Drawing.Size(243, 69);
            this.RobotSelectListBox.Sorted = true;
            this.RobotSelectListBox.TabIndex = 12;
            this.RobotSelectListBox.SelectedIndexChanged += new System.EventHandler(this.RobotSelectListBox_SelectedIndexChanged);
            // 
            // RobotTypeLabel
            // 
            this.RobotTypeLabel.AutoSize = true;
            this.RobotTypeLabel.Location = new System.Drawing.Point(16, 123);
            this.RobotTypeLabel.Name = "RobotTypeLabel";
            this.RobotTypeLabel.Size = new System.Drawing.Size(56, 13);
            this.RobotTypeLabel.TabIndex = 13;
            this.RobotTypeLabel.Text = "Robottype";
            this.RobotTypeLabel.Visible = false;
            // 
            // ApplicationFramesCheckedListBox
            // 
            this.ApplicationFramesCheckedListBox.BackColor = System.Drawing.Color.PapayaWhip;
            this.ApplicationFramesCheckedListBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ApplicationFramesCheckedListBox.FormattingEnabled = true;
            this.ApplicationFramesCheckedListBox.IntegralHeight = false;
            this.ApplicationFramesCheckedListBox.Location = new System.Drawing.Point(104, 123);
            this.ApplicationFramesCheckedListBox.Name = "ApplicationFramesCheckedListBox";
            this.ApplicationFramesCheckedListBox.Size = new System.Drawing.Size(544, 191);
            this.ApplicationFramesCheckedListBox.Sorted = true;
            this.ApplicationFramesCheckedListBox.TabIndex = 14;
            this.ApplicationFramesCheckedListBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.ApplicationFramesCheckedListBox_ItemCheck);
            // 
            // MakeFoldersButton
            // 
            this.MakeFoldersButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MakeFoldersButton.Image = ((System.Drawing.Image)(resources.GetObject("MakeFoldersButton.Image")));
            this.MakeFoldersButton.Location = new System.Drawing.Point(19, 161);
            this.MakeFoldersButton.Name = "MakeFoldersButton";
            this.MakeFoldersButton.Size = new System.Drawing.Size(44, 40);
            this.MakeFoldersButton.TabIndex = 15;
            this.MakeFoldersButton.Text = "Make Folders";
            this.MakeFoldersButton.UseVisualStyleBackColor = true;
            this.MakeFoldersButton.Click += new System.EventHandler(this.MakeFoldersButton_Click);
            // 
            // CheckRobotsButton
            // 
            this.CheckRobotsButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CheckRobotsButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CheckRobotsButton.ForeColor = System.Drawing.Color.Red;
            this.CheckRobotsButton.Image = ((System.Drawing.Image)(resources.GetObject("CheckRobotsButton.Image")));
            this.CheckRobotsButton.Location = new System.Drawing.Point(19, 216);
            this.CheckRobotsButton.Name = "CheckRobotsButton";
            this.CheckRobotsButton.Size = new System.Drawing.Size(44, 40);
            this.CheckRobotsButton.TabIndex = 16;
            this.CheckRobotsButton.Text = "Check Robots";
            this.CheckRobotsButton.UseVisualStyleBackColor = true;
            this.CheckRobotsButton.Visible = false;
            this.CheckRobotsButton.Click += new System.EventHandler(this.CheckRobotsButton_Click);
            // 
            // CheckProgressBar
            // 
            this.CheckProgressBar.Location = new System.Drawing.Point(19, 331);
            this.CheckProgressBar.Name = "CheckProgressBar";
            this.CheckProgressBar.Size = new System.Drawing.Size(629, 29);
            this.CheckProgressBar.TabIndex = 17;
            // 
            // BadApplicationsRichTextBox
            // 
            this.BadApplicationsRichTextBox.BackColor = System.Drawing.Color.PapayaWhip;
            this.BadApplicationsRichTextBox.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BadApplicationsRichTextBox.ForeColor = System.Drawing.Color.DarkRed;
            this.BadApplicationsRichTextBox.Location = new System.Drawing.Point(19, 390);
            this.BadApplicationsRichTextBox.Name = "BadApplicationsRichTextBox";
            this.BadApplicationsRichTextBox.Size = new System.Drawing.Size(629, 141);
            this.BadApplicationsRichTextBox.TabIndex = 18;
            this.BadApplicationsRichTextBox.Text = "";
            // 
            // BadAppLabel
            // 
            this.BadAppLabel.AutoSize = true;
            this.BadAppLabel.Location = new System.Drawing.Point(16, 374);
            this.BadAppLabel.Name = "BadAppLabel";
            this.BadAppLabel.Size = new System.Drawing.Size(113, 13);
            this.BadAppLabel.TabIndex = 19;
            this.BadAppLabel.Text = "Unknown Applications";
            this.BadAppLabel.Visible = false;
            // 
            // CheckedLabel
            // 
            this.CheckedLabel.AutoSize = true;
            this.CheckedLabel.Location = new System.Drawing.Point(37, 292);
            this.CheckedLabel.Name = "CheckedLabel";
            this.CheckedLabel.Size = new System.Drawing.Size(50, 13);
            this.CheckedLabel.TabIndex = 49;
            this.CheckedLabel.Text = "Checked";
            // 
            // WriteAcceptedChecksCheckBox
            // 
            this.WriteAcceptedChecksCheckBox.AutoSize = true;
            this.WriteAcceptedChecksCheckBox.Checked = true;
            this.WriteAcceptedChecksCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.WriteAcceptedChecksCheckBox.Location = new System.Drawing.Point(21, 278);
            this.WriteAcceptedChecksCheckBox.Name = "WriteAcceptedChecksCheckBox";
            this.WriteAcceptedChecksCheckBox.Size = new System.Drawing.Size(51, 17);
            this.WriteAcceptedChecksCheckBox.TabIndex = 48;
            this.WriteAcceptedChecksCheckBox.Text = "Write";
            this.WriteAcceptedChecksCheckBox.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Enabled = false;
            this.btnOK.Location = new System.Drawing.Point(389, 541);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(109, 33);
            this.btnOK.TabIndex = 50;
            this.btnOK.Text = "&Ok";
            this.btnOK.UseVisualStyleBackColor = false;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(513, 541);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(103, 33);
            this.btnCancel.TabIndex = 51;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            // 
            // RobotData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(225)))), ((int)(((byte)(225)))));
            this.CancelButton = this.CheckRobotsButton;
            this.ClientSize = new System.Drawing.Size(666, 576);
            this.ControlBox = false;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.CheckedLabel);
            this.Controls.Add(this.WriteAcceptedChecksCheckBox);
            this.Controls.Add(this.BadAppLabel);
            this.Controls.Add(this.BadApplicationsRichTextBox);
            this.Controls.Add(this.CheckProgressBar);
            this.Controls.Add(this.CheckRobotsButton);
            this.Controls.Add(this.MakeFoldersButton);
            this.Controls.Add(this.ApplicationFramesCheckedListBox);
            this.Controls.Add(this.RobotTypeLabel);
            this.Controls.Add(this.RobotSelectListBox);
            this.Controls.Add(this.SelectRobotLabel);
            this.Controls.Add(this.StationSelectListBox);
            this.Controls.Add(this.SelectStationLabel);
            this.Controls.Add(this.RobotMatrixTextBox);
            this.Controls.Add(this.RobotMatrixFileButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(630, 600);
            this.Name = "RobotData";
            this.ShowInTaskbar = false;
            this.Text = "Robot Data Collector";
            this.Deactivate += new System.EventHandler(this.RobotData_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RobotData_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox RobotMatrixTextBox;
        private System.Windows.Forms.Button RobotMatrixFileButton;
        private System.Windows.Forms.Label SelectStationLabel;
        private System.Windows.Forms.ListBox StationSelectListBox;
        private System.Windows.Forms.Label SelectRobotLabel;
        private System.Windows.Forms.ListBox RobotSelectListBox;
        private System.Windows.Forms.Label RobotTypeLabel;
        private System.Windows.Forms.CheckedListBox ApplicationFramesCheckedListBox;
        private System.Windows.Forms.Button MakeFoldersButton;
        private System.Windows.Forms.Button CheckRobotsButton;
        private System.Windows.Forms.ProgressBar CheckProgressBar;
        private System.Windows.Forms.RichTextBox BadApplicationsRichTextBox;
        private System.Windows.Forms.Label BadAppLabel;
        private System.Windows.Forms.Label CheckedLabel;
        private System.Windows.Forms.CheckBox WriteAcceptedChecksCheckBox;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}