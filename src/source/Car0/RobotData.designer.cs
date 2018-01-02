using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarZero
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Windows.Forms;
    using CarZero.Classes;
    public partial class RobotData : Form
    {
        #region · Members ·
        private CheckedListBox ApplicationFramesCheckedListBox;
        private const int AppTypePadWidth = 0x25;
        private Label BadAppLabel;
        private RichTextBox BadApplicationsRichTextBox;
        private Label CheckedLabel;
        private ProgressBar CheckProgressBar;
        private Button CheckRobotsButton;
        private IContainer components = null;
        private static OpenFileDialog FileBrowser = new OpenFileDialog();
        private static DialogResult FileReady = DialogResult.None;
     
        private Button MakeFoldersButton;
       
        private Button RobotMatrixFileButton;
        private TextBox RobotMatrixTextBox;
        private ListBox RobotSelectListBox;
       
        private Label RobotTypeLabel;
        private Label SelectRobotLabel;
        private Label SelectStationLabel;
      
       
        private ListBox StationSelectListBox;
     
        private CheckBox WriteAcceptedChecksCheckBox;

        #endregion
        
        private void InitializeComponent()
        {
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(RobotData));
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
            this.SuspendLayout();
            // 
            // RobotMatrixTextBox
            // 
            this.RobotMatrixTextBox.Location = new System.Drawing.Point(120, 12);
            this.RobotMatrixTextBox.Name = "RobotMatrixTextBox";
            this.RobotMatrixTextBox.ReadOnly = true;
            this.RobotMatrixTextBox.Size = new System.Drawing.Size(476, 20);
            this.RobotMatrixTextBox.TabIndex = 7;
            this.RobotMatrixTextBox.Text = "Not Set";
            // 
            // RobotMatrixFileButton
            // 
            this.RobotMatrixFileButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(225)))), ((int)(((byte)(225)))));
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
            this.RobotSelectListBox.Size = new System.Drawing.Size(191, 69);
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
            this.ApplicationFramesCheckedListBox.Size = new System.Drawing.Size(494, 191);
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
            this.CheckRobotsButton.Image = global::CarZero.Properties.Resources.CheckRobotsButton_Image;
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
            this.CheckProgressBar.Size = new System.Drawing.Size(578, 29);
            this.CheckProgressBar.TabIndex = 17;
            // 
            // BadApplicationsRichTextBox
            // 
            this.BadApplicationsRichTextBox.BackColor = System.Drawing.Color.PapayaWhip;
            this.BadApplicationsRichTextBox.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BadApplicationsRichTextBox.ForeColor = System.Drawing.Color.DarkRed;
            this.BadApplicationsRichTextBox.Location = new System.Drawing.Point(19, 390);
            this.BadApplicationsRichTextBox.Name = "BadApplicationsRichTextBox";
            this.BadApplicationsRichTextBox.Size = new System.Drawing.Size(577, 141);
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
            // RobotData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(225)))), ((int)(((byte)(225)))));
            this.CancelButton = this.CheckRobotsButton;
            this.ClientSize = new System.Drawing.Size(624, 576);
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
            this.MinimumSize = new System.Drawing.Size(630, 600);
            this.Name = "RobotData";
            this.ShowInTaskbar = false;
            this.Text = "Robot Data Collector";
            this.Deactivate += new System.EventHandler(this.RobotData_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RobotData_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        
    }
}
