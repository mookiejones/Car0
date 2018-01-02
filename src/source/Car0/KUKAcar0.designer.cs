 
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using CarZero;

namespace CarZero
{
    public partial class KUKAcar0: Form
    {
              #region Members
        private ComponentResourceManager manager;
        private Button CalculateRobot321Button;
        private IContainer components = null;
        private static OpenFileDialog FileBrowser = new OpenFileDialog();
        private static FolderBrowserDialog folderBrowser1 = new FolderBrowserDialog();
        private TextBox FolderDisplayTextBox;
        private Button FolderSelectButton;
        private Button GetRobotDataButton;
        private RadioButton Initial321RadioButton;
        private static bool Initializing = false;
        private TextBox J1_TextBox;
        private TextBox J2_TextBox;
        private TextBox J3_TextBox;
        private TextBox J4_J5_TextBox;
        private TextBox J4_J6_TextBox;
        private Button J4setJ5ValueButton;
        private Button J4setJ6ValueButton;
        private TextBox J5_J4_TextBox;
        private TextBox J5_J6_TextBox;
        private Button J5setJ4ValueButton;
        private Button J5setJ6ValueButton;
        private TextBox J6_J4_TextBox;
        private TextBox J6_J5_TextBox;
        private Button J6setJ4ValueButton;
        private Button J6setJ5ValueButton;
        private static List<Vector> MyJ4Data = null;
        private static List<Vector> MyJ5Data = null;
        private static List<Vector> MyJ6Data = null;
        public static RobotData MyRobotData = null;
        private RichTextBox ProblemsTextBox;
        private RadioButton RadialToolDataRadioButton;
        private Brand Rbrand = new Brand();
        private Button ReadJointCoordsButton;
        private Button ReadRobotBaseDataButton;
        private RadioButton RobotBasePointsRadioButton;
        private ListBox RobotBrandListBox;
        private TextBox RobotDataDisplayTextBox;
        private Button SetAllJointValuesButton;
        private Button SetJ123Button;
        private TextBox TrackerDataFileTextBox;
        private Button ViewTrackerFileBbutton;
        private CheckBox WriteDebugDataCheckBox;
        #endregion
        
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KUKAcar0));
            this.FolderDisplayTextBox = new System.Windows.Forms.TextBox();
            this.FolderSelectButton = new System.Windows.Forms.Button();
            this.ViewTrackerFileBbutton = new System.Windows.Forms.Button();
            this.TrackerDataFileTextBox = new System.Windows.Forms.TextBox();
            this.CalculateRobot321Button = new System.Windows.Forms.Button();
            this.Initial321RadioButton = new System.Windows.Forms.RadioButton();
            this.RobotBasePointsRadioButton = new System.Windows.Forms.RadioButton();
            this.GetRobotDataButton = new System.Windows.Forms.Button();
            this.RobotDataDisplayTextBox = new System.Windows.Forms.TextBox();
            this.ProblemsTextBox = new System.Windows.Forms.RichTextBox();
            this.RobotBrandListBox = new System.Windows.Forms.ListBox();
            this.RadialToolDataRadioButton = new System.Windows.Forms.RadioButton();
            this.ReadRobotBaseDataButton = new System.Windows.Forms.Button();
            this.ReadJointCoordsButton = new System.Windows.Forms.Button();
            this.SetJ123Button = new System.Windows.Forms.Button();
            this.J3_TextBox = new System.Windows.Forms.TextBox();
            this.J2_TextBox = new System.Windows.Forms.TextBox();
            this.J1_TextBox = new System.Windows.Forms.TextBox();
            this.J4setJ5ValueButton = new System.Windows.Forms.Button();
            this.J4_J6_TextBox = new System.Windows.Forms.TextBox();
            this.J4_J5_TextBox = new System.Windows.Forms.TextBox();
            this.J4setJ6ValueButton = new System.Windows.Forms.Button();
            this.J5setJ6ValueButton = new System.Windows.Forms.Button();
            this.J5_J6_TextBox = new System.Windows.Forms.TextBox();
            this.J5_J4_TextBox = new System.Windows.Forms.TextBox();
            this.J5setJ4ValueButton = new System.Windows.Forms.Button();
            this.J6setJ5ValueButton = new System.Windows.Forms.Button();
            this.J6_J5_TextBox = new System.Windows.Forms.TextBox();
            this.J6_J4_TextBox = new System.Windows.Forms.TextBox();
            this.J6setJ4ValueButton = new System.Windows.Forms.Button();
            this.SetAllJointValuesButton = new System.Windows.Forms.Button();
            this.WriteDebugDataCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // FolderDisplayTextBox
            // 
            this.FolderDisplayTextBox.BackColor = System.Drawing.Color.PapayaWhip;
            this.FolderDisplayTextBox.Location = new System.Drawing.Point(120, 9);
            this.FolderDisplayTextBox.Name = "FolderDisplayTextBox";
            this.FolderDisplayTextBox.ReadOnly = true;
            this.FolderDisplayTextBox.Size = new System.Drawing.Size(496, 20);
            this.FolderDisplayTextBox.TabIndex = 3;
            this.FolderDisplayTextBox.Text = "Not Set";
            // 
            // FolderSelectButton
            // 
            this.FolderSelectButton.AccessibleRole = System.Windows.Forms.AccessibleRole.TitleBar;
            this.FolderSelectButton.Location = new System.Drawing.Point(12, 9);
            this.FolderSelectButton.Name = "FolderSelectButton";
            this.FolderSelectButton.Size = new System.Drawing.Size(102, 20);
            this.FolderSelectButton.TabIndex = 2;
            this.FolderSelectButton.Text = "Set Root Folder";
            this.FolderSelectButton.UseVisualStyleBackColor = true;
            this.FolderSelectButton.Click += new System.EventHandler(this.FolderSelectButton_Click);
            // 
            // ViewTrackerFileBbutton
            // 
            this.ViewTrackerFileBbutton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(225)))), ((int)(((byte)(225)))));
            this.ViewTrackerFileBbutton.Location = new System.Drawing.Point(12, 82);
            this.ViewTrackerFileBbutton.Name = "ViewTrackerFileBbutton";
            this.ViewTrackerFileBbutton.Size = new System.Drawing.Size(102, 20);
            this.ViewTrackerFileBbutton.TabIndex = 4;
            this.ViewTrackerFileBbutton.Text = "View Tracker File";
            this.ViewTrackerFileBbutton.UseVisualStyleBackColor = false;
            this.ViewTrackerFileBbutton.Visible = false;
            this.ViewTrackerFileBbutton.Click += new System.EventHandler(this.ViewTrackerFileBbutton_Click);
            // 
            // TrackerDataFileTextBox
            // 
            this.TrackerDataFileTextBox.BackColor = System.Drawing.Color.PapayaWhip;
            this.TrackerDataFileTextBox.Location = new System.Drawing.Point(120, 82);
            this.TrackerDataFileTextBox.Name = "TrackerDataFileTextBox";
            this.TrackerDataFileTextBox.ReadOnly = true;
            this.TrackerDataFileTextBox.Size = new System.Drawing.Size(496, 20);
            this.TrackerDataFileTextBox.TabIndex = 5;
            this.TrackerDataFileTextBox.Text = "Not Set";
            // 
            // CalculateRobot321Button
            // 
            this.CalculateRobot321Button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(225)))), ((int)(((byte)(225)))));
            this.CalculateRobot321Button.Location = new System.Drawing.Point(12, 278);
            this.CalculateRobot321Button.Name = "CalculateRobot321Button";
            this.CalculateRobot321Button.Size = new System.Drawing.Size(604, 24);
            this.CalculateRobot321Button.TabIndex = 22;
            this.CalculateRobot321Button.Text = "Calculate radial tool data";
            this.CalculateRobot321Button.UseVisualStyleBackColor = false;
            this.CalculateRobot321Button.Click += new System.EventHandler(this.CalculateRobot321Button_Click);
            // 
            // Initial321RadioButton
            // 
            this.Initial321RadioButton.AutoSize = true;
            this.Initial321RadioButton.Location = new System.Drawing.Point(496, 61);
            this.Initial321RadioButton.Name = "Initial321RadioButton";
            this.Initial321RadioButton.Size = new System.Drawing.Size(81, 17);
            this.Initial321RadioButton.TabIndex = 25;
            this.Initial321RadioButton.Text = "Initial 321 fit";
            this.Initial321RadioButton.UseVisualStyleBackColor = true;
            this.Initial321RadioButton.Visible = false;
            this.Initial321RadioButton.CheckedChanged += new System.EventHandler(this.Initial321RadioButton_CheckedChanged);
            // 
            // RobotBasePointsRadioButton
            // 
            this.RobotBasePointsRadioButton.AutoSize = true;
            this.RobotBasePointsRadioButton.Location = new System.Drawing.Point(120, 61);
            this.RobotBasePointsRadioButton.Name = "RobotBasePointsRadioButton";
            this.RobotBasePointsRadioButton.Size = new System.Drawing.Size(111, 17);
            this.RobotBasePointsRadioButton.TabIndex = 26;
            this.RobotBasePointsRadioButton.Text = "Robot base points";
            this.RobotBasePointsRadioButton.UseVisualStyleBackColor = true;
            this.RobotBasePointsRadioButton.CheckedChanged += new System.EventHandler(this.RobotBasePointsRadioButton_CheckedChanged);
            // 
            // GetRobotDataButton
            // 
            this.GetRobotDataButton.Location = new System.Drawing.Point(12, 35);
            this.GetRobotDataButton.Name = "GetRobotDataButton";
            this.GetRobotDataButton.Size = new System.Drawing.Size(85, 20);
            this.GetRobotDataButton.TabIndex = 39;
            this.GetRobotDataButton.Text = "Robot data";
            this.GetRobotDataButton.UseVisualStyleBackColor = true;
            this.GetRobotDataButton.Visible = false;
            this.GetRobotDataButton.Click += new System.EventHandler(this.GetRobotDataButton_Click);
            // 
            // RobotDataDisplayTextBox
            // 
            this.RobotDataDisplayTextBox.BackColor = System.Drawing.Color.PapayaWhip;
            this.RobotDataDisplayTextBox.Location = new System.Drawing.Point(120, 35);
            this.RobotDataDisplayTextBox.Name = "RobotDataDisplayTextBox";
            this.RobotDataDisplayTextBox.ReadOnly = true;
            this.RobotDataDisplayTextBox.Size = new System.Drawing.Size(496, 20);
            this.RobotDataDisplayTextBox.TabIndex = 40;
            this.RobotDataDisplayTextBox.Text = "Not Set";
            this.RobotDataDisplayTextBox.Visible = false;
            // 
            // ProblemsTextBox
            // 
            this.ProblemsTextBox.BackColor = System.Drawing.Color.PapayaWhip;
            this.ProblemsTextBox.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ProblemsTextBox.Location = new System.Drawing.Point(12, 193);
            this.ProblemsTextBox.Name = "ProblemsTextBox";
            this.ProblemsTextBox.Size = new System.Drawing.Size(604, 80);
            this.ProblemsTextBox.TabIndex = 41;
            this.ProblemsTextBox.Text = "";
            // 
            // RobotBrandListBox
            // 
            this.RobotBrandListBox.BackColor = System.Drawing.Color.PapayaWhip;
            this.RobotBrandListBox.DisplayMember = "2";
            this.RobotBrandListBox.FormattingEnabled = true;
            this.RobotBrandListBox.Items.AddRange(new object[] {
            "ABB",
            "Fanuc",
            "FanucRJ",
            "KUKA",
            "Nachi"});
            this.RobotBrandListBox.Location = new System.Drawing.Point(12, 117);
            this.RobotBrandListBox.Name = "RobotBrandListBox";
            this.RobotBrandListBox.Size = new System.Drawing.Size(85, 69);
            this.RobotBrandListBox.TabIndex = 42;
            this.RobotBrandListBox.ValueMember = "2";
            this.RobotBrandListBox.SelectedIndexChanged += new System.EventHandler(this.RobotBrandListBox_SelectedIndexChanged);
            // 
            // RadialToolDataRadioButton
            // 
            this.RadialToolDataRadioButton.AutoSize = true;
            this.RadialToolDataRadioButton.Checked = true;
            this.RadialToolDataRadioButton.Location = new System.Drawing.Point(15, 61);
            this.RadialToolDataRadioButton.Name = "RadialToolDataRadioButton";
            this.RadialToolDataRadioButton.Size = new System.Drawing.Size(99, 17);
            this.RadialToolDataRadioButton.TabIndex = 43;
            this.RadialToolDataRadioButton.TabStop = true;
            this.RadialToolDataRadioButton.Text = "Radial tool data";
            this.RadialToolDataRadioButton.UseVisualStyleBackColor = true;
           // 
            // ReadRobotBaseDataButton
            // 
            this.ReadRobotBaseDataButton.Location = new System.Drawing.Point(120, 141);
            this.ReadRobotBaseDataButton.Name = "ReadRobotBaseDataButton";
            this.ReadRobotBaseDataButton.Size = new System.Drawing.Size(151, 20);
            this.ReadRobotBaseDataButton.TabIndex = 44;
            this.ReadRobotBaseDataButton.Text = "Read Robot Base Program";
            this.ReadRobotBaseDataButton.UseVisualStyleBackColor = true;
            this.ReadRobotBaseDataButton.Visible = false;
            this.ReadRobotBaseDataButton.Click += new System.EventHandler(this.ReadRobotBaseDataButton_Click);
            // 
            // ReadJointCoordsButton
            // 
            this.ReadJointCoordsButton.Location = new System.Drawing.Point(120, 116);
            this.ReadJointCoordsButton.Name = "ReadJointCoordsButton";
            this.ReadJointCoordsButton.Size = new System.Drawing.Size(151, 20);
            this.ReadJointCoordsButton.TabIndex = 45;
            this.ReadJointCoordsButton.Text = "Read Robot Tool Program";
            this.ReadJointCoordsButton.UseVisualStyleBackColor = true;
            this.ReadJointCoordsButton.Visible = false;
            this.ReadJointCoordsButton.Click += new System.EventHandler(this.ReadJointCoordsButton_Click);
            // 
            // SetJ123Button
            // 
            this.SetJ123Button.Location = new System.Drawing.Point(302, 116);
            this.SetJ123Button.Name = "SetJ123Button";
            this.SetJ123Button.Size = new System.Drawing.Size(75, 20);
            this.SetJ123Button.TabIndex = 46;
            this.SetJ123Button.Text = "J123 Values";
            this.SetJ123Button.UseVisualStyleBackColor = true;
            this.SetJ123Button.Visible = false;
            this.SetJ123Button.Click += new System.EventHandler(this.SetJ123Button_Click);
            // 
            // J3_TextBox
            // 
            this.J3_TextBox.Location = new System.Drawing.Point(541, 117);
            this.J3_TextBox.Name = "J3_TextBox";
            this.J3_TextBox.Size = new System.Drawing.Size(75, 20);
            this.J3_TextBox.TabIndex = 49;
            this.J3_TextBox.Visible = false;
            // 
            // J2_TextBox
            // 
            this.J2_TextBox.Location = new System.Drawing.Point(461, 117);
            this.J2_TextBox.Name = "J2_TextBox";
            this.J2_TextBox.Size = new System.Drawing.Size(75, 20);
            this.J2_TextBox.TabIndex = 48;
            this.J2_TextBox.Visible = false;
            // 
            // J1_TextBox
            // 
            this.J1_TextBox.Location = new System.Drawing.Point(380, 117);
            this.J1_TextBox.Name = "J1_TextBox";
            this.J1_TextBox.Size = new System.Drawing.Size(75, 20);
            this.J1_TextBox.TabIndex = 47;
            this.J1_TextBox.Visible = false;
            // 
            // J4setJ5ValueButton
            // 
            this.J4setJ5ValueButton.Location = new System.Drawing.Point(303, 115);
            this.J4setJ5ValueButton.Name = "J4setJ5ValueButton";
            this.J4setJ5ValueButton.Size = new System.Drawing.Size(75, 20);
            this.J4setJ5ValueButton.TabIndex = 50;
            this.J4setJ5ValueButton.Text = "J4-J5 Value";
            this.J4setJ5ValueButton.UseVisualStyleBackColor = true;
            this.J4setJ5ValueButton.Visible = false;
            this.J4setJ5ValueButton.Click += new System.EventHandler(this.J4setJ5ValueButton_Click);
            // 
            // J4_J6_TextBox
            // 
            this.J4_J6_TextBox.Location = new System.Drawing.Point(542, 113);
            this.J4_J6_TextBox.Name = "J4_J6_TextBox";
            this.J4_J6_TextBox.Size = new System.Drawing.Size(75, 20);
            this.J4_J6_TextBox.TabIndex = 52;
            this.J4_J6_TextBox.Visible = false;
            // 
            // J4_J5_TextBox
            // 
            this.J4_J5_TextBox.Location = new System.Drawing.Point(381, 113);
            this.J4_J5_TextBox.Name = "J4_J5_TextBox";
            this.J4_J5_TextBox.Size = new System.Drawing.Size(75, 20);
            this.J4_J5_TextBox.TabIndex = 51;
            this.J4_J5_TextBox.Visible = false;
            // 
            // J4setJ6ValueButton
            // 
            this.J4setJ6ValueButton.Location = new System.Drawing.Point(461, 113);
            this.J4setJ6ValueButton.Name = "J4setJ6ValueButton";
            this.J4setJ6ValueButton.Size = new System.Drawing.Size(75, 20);
            this.J4setJ6ValueButton.TabIndex = 53;
            this.J4setJ6ValueButton.Text = "J4-J6 Value";
            this.J4setJ6ValueButton.UseVisualStyleBackColor = true;
            this.J4setJ6ValueButton.Visible = false;
            this.J4setJ6ValueButton.Click += new System.EventHandler(this.J4setJ6ValueButton_Click);
            // 
            // J5setJ6ValueButton
            // 
            this.J5setJ6ValueButton.Location = new System.Drawing.Point(462, 141);
            this.J5setJ6ValueButton.Name = "J5setJ6ValueButton";
            this.J5setJ6ValueButton.Size = new System.Drawing.Size(75, 20);
            this.J5setJ6ValueButton.TabIndex = 57;
            this.J5setJ6ValueButton.Text = "J5-J6 Value";
            this.J5setJ6ValueButton.UseVisualStyleBackColor = true;
            this.J5setJ6ValueButton.Visible = false;
            this.J5setJ6ValueButton.Click += new System.EventHandler(this.J5setJ6ValueButton_Click);
            // 
            // J5_J6_TextBox
            // 
            this.J5_J6_TextBox.Location = new System.Drawing.Point(542, 141);
            this.J5_J6_TextBox.Name = "J5_J6_TextBox";
            this.J5_J6_TextBox.Size = new System.Drawing.Size(75, 20);
            this.J5_J6_TextBox.TabIndex = 56;
            this.J5_J6_TextBox.Visible = false;
            // 
            // J5_J4_TextBox
            // 
            this.J5_J4_TextBox.Location = new System.Drawing.Point(381, 141);
            this.J5_J4_TextBox.Name = "J5_J4_TextBox";
            this.J5_J4_TextBox.Size = new System.Drawing.Size(75, 20);
            this.J5_J4_TextBox.TabIndex = 55;
            this.J5_J4_TextBox.Visible = false;
            // 
            // J5setJ4ValueButton
            // 
            this.J5setJ4ValueButton.Location = new System.Drawing.Point(303, 141);
            this.J5setJ4ValueButton.Name = "J5setJ4ValueButton";
            this.J5setJ4ValueButton.Size = new System.Drawing.Size(75, 20);
            this.J5setJ4ValueButton.TabIndex = 54;
            this.J5setJ4ValueButton.Text = "J5-J4 Value";
            this.J5setJ4ValueButton.UseVisualStyleBackColor = true;
            this.J5setJ4ValueButton.Visible = false;
            this.J5setJ4ValueButton.Click += new System.EventHandler(this.J5setJ4ValueButton_Click);
            // 
            // J6setJ5ValueButton
            // 
            this.J6setJ5ValueButton.Location = new System.Drawing.Point(460, 167);
            this.J6setJ5ValueButton.Name = "J6setJ5ValueButton";
            this.J6setJ5ValueButton.Size = new System.Drawing.Size(75, 20);
            this.J6setJ5ValueButton.TabIndex = 61;
            this.J6setJ5ValueButton.Text = "J6-J5 Value";
            this.J6setJ5ValueButton.UseVisualStyleBackColor = true;
            this.J6setJ5ValueButton.Visible = false;
            this.J6setJ5ValueButton.Click += new System.EventHandler(this.J6setJ5ValueButton_Click);
            // 
            // J6_J5_TextBox
            // 
            this.J6_J5_TextBox.Location = new System.Drawing.Point(541, 167);
            this.J6_J5_TextBox.Name = "J6_J5_TextBox";
            this.J6_J5_TextBox.Size = new System.Drawing.Size(75, 20);
            this.J6_J5_TextBox.TabIndex = 60;
            this.J6_J5_TextBox.Visible = false;
            // 
            // J6_J4_TextBox
            // 
            this.J6_J4_TextBox.Location = new System.Drawing.Point(380, 167);
            this.J6_J4_TextBox.Name = "J6_J4_TextBox";
            this.J6_J4_TextBox.Size = new System.Drawing.Size(75, 20);
            this.J6_J4_TextBox.TabIndex = 59;
            this.J6_J4_TextBox.Visible = false;
            // 
            // J6setJ4ValueButton
            // 
            this.J6setJ4ValueButton.Location = new System.Drawing.Point(302, 167);
            this.J6setJ4ValueButton.Name = "J6setJ4ValueButton";
            this.J6setJ4ValueButton.Size = new System.Drawing.Size(75, 20);
            this.J6setJ4ValueButton.TabIndex = 58;
            this.J6setJ4ValueButton.Text = "J6-J4 Value";
            this.J6setJ4ValueButton.UseVisualStyleBackColor = true;
            this.J6setJ4ValueButton.Visible = false;
            this.J6setJ4ValueButton.Click += new System.EventHandler(this.J6setJ4ValueButton_Click);
            // 
            // SetAllJointValuesButton
            // 
            this.SetAllJointValuesButton.Location = new System.Drawing.Point(120, 167);
            this.SetAllJointValuesButton.Name = "SetAllJointValuesButton";
            this.SetAllJointValuesButton.Size = new System.Drawing.Size(151, 20);
            this.SetAllJointValuesButton.TabIndex = 62;
            this.SetAllJointValuesButton.Text = "Set All Joint Values";
            this.SetAllJointValuesButton.UseVisualStyleBackColor = true;
            this.SetAllJointValuesButton.Visible = false;
            this.SetAllJointValuesButton.Click += new System.EventHandler(this.SetAllJointValuesButton_Click);
            // 
            // WriteDebugDataCheckBox
            // 
            this.WriteDebugDataCheckBox.AutoSize = true;
            this.WriteDebugDataCheckBox.Location = new System.Drawing.Point(237, 62);
            this.WriteDebugDataCheckBox.Name = "WriteDebugDataCheckBox";
            this.WriteDebugDataCheckBox.Size = new System.Drawing.Size(112, 17);
            this.WriteDebugDataCheckBox.TabIndex = 63;
            this.WriteDebugDataCheckBox.Text = "Write Debug Data";
            this.WriteDebugDataCheckBox.UseVisualStyleBackColor = true;
            // 
            // KUKAcar0
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(225)))), ((int)(((byte)(225)))));
            this.ClientSize = new System.Drawing.Size(619, 302);
            this.Controls.Add(this.WriteDebugDataCheckBox);
            this.Controls.Add(this.SetAllJointValuesButton);
            this.Controls.Add(this.J6setJ5ValueButton);
            this.Controls.Add(this.J6_J5_TextBox);
            this.Controls.Add(this.J6_J4_TextBox);
            this.Controls.Add(this.J6setJ4ValueButton);
            this.Controls.Add(this.J5setJ6ValueButton);
            this.Controls.Add(this.J5_J6_TextBox);
            this.Controls.Add(this.J5_J4_TextBox);
            this.Controls.Add(this.J5setJ4ValueButton);
            this.Controls.Add(this.J4setJ6ValueButton);
            this.Controls.Add(this.J4_J6_TextBox);
            this.Controls.Add(this.J4_J5_TextBox);
            this.Controls.Add(this.J4setJ5ValueButton);
            this.Controls.Add(this.J3_TextBox);
            this.Controls.Add(this.J2_TextBox);
            this.Controls.Add(this.J1_TextBox);
            this.Controls.Add(this.SetJ123Button);
            this.Controls.Add(this.ReadJointCoordsButton);
            this.Controls.Add(this.ReadRobotBaseDataButton);
            this.Controls.Add(this.RadialToolDataRadioButton);
            this.Controls.Add(this.RobotBrandListBox);
            this.Controls.Add(this.ProblemsTextBox);
            this.Controls.Add(this.RobotDataDisplayTextBox);
            this.Controls.Add(this.GetRobotDataButton);
            this.Controls.Add(this.RobotBasePointsRadioButton);
            this.Controls.Add(this.Initial321RadioButton);
            this.Controls.Add(this.CalculateRobot321Button);
            this.Controls.Add(this.TrackerDataFileTextBox);
            this.Controls.Add(this.ViewTrackerFileBbutton);
            this.Controls.Add(this.FolderDisplayTextBox);
            this.Controls.Add(this.FolderSelectButton);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(635, 340);
            this.Name = "KUKAcar0";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "KUKA Car 0 (6-29-12)";
            this.TransparencyKey = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.Activated += new System.EventHandler(this.KUKAcar0_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.KUKAcar0_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
