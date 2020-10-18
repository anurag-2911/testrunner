namespace TestRunner
{
    partial class TestRunnerForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TestRunnerForm));
            this.treeView_Tests = new System.Windows.Forms.TreeView();
            this.btn_Run = new System.Windows.Forms.Button();
            this.txtBox_OutPut = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.FileUploadDailog = new System.Windows.Forms.OpenFileDialog();
            this.btnUploadTestProject = new System.Windows.Forms.Button();
           // CustomProgressBar customProgressBar = new CustomProgressBar();
            this.progressBarTests = new System.Windows.Forms.ProgressBar();//
            this.SuspendLayout();
            // 
            // treeView_Tests
            // 
            this.treeView_Tests.Location = new System.Drawing.Point(36, 99);
            this.treeView_Tests.Name = "treeView_Tests";
            this.treeView_Tests.Size = new System.Drawing.Size(360, 538);
            this.treeView_Tests.TabIndex = 0;
            this.treeView_Tests.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_Tests_AfterSelect);
            // 
            // btn_Run
            // 
            this.btn_Run.Location = new System.Drawing.Point(500, 42);
            this.btn_Run.Name = "btn_Run";
            this.btn_Run.Size = new System.Drawing.Size(152, 51);
            this.btn_Run.TabIndex = 1;
            this.btn_Run.Text = "Run";
            this.btn_Run.UseVisualStyleBackColor = true;
            this.btn_Run.Click += new System.EventHandler(this.btn_Run_Click);
            // 
            // txtBox_OutPut
            // 
            this.txtBox_OutPut.Location = new System.Drawing.Point(500, 99);
            this.txtBox_OutPut.Multiline = true;
            this.txtBox_OutPut.Name = "txtBox_OutPut";
            this.txtBox_OutPut.Size = new System.Drawing.Size(1277, 538);
            this.txtBox_OutPut.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(33, 657);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(111, 17);
            this.label1.TabIndex = 5;
            this.label1.Text = "NumberOfTimes";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(175, 654);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(221, 22);
            this.textBox1.TabIndex = 6;
            // 
            // FileUploadDailog
            // 
            this.FileUploadDailog.FileName = "FileUploadDialog";
            // 
            // btnUploadTestProject
            // 
            this.btnUploadTestProject.Location = new System.Drawing.Point(36, 52);
            this.btnUploadTestProject.Name = "btnUploadTestProject";
            this.btnUploadTestProject.Size = new System.Drawing.Size(146, 41);
            this.btnUploadTestProject.TabIndex = 7;
            this.btnUploadTestProject.Text = "Upload Test Project";
            this.btnUploadTestProject.UseVisualStyleBackColor = true;
            this.btnUploadTestProject.Click += new System.EventHandler(this.btnUploadTestProject_Click);
            // 
            // progressBarTests
            // 

            this.progressBarTests.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.progressBarTests.Location = new System.Drawing.Point(500, 657);
            this.progressBarTests.Name = "progressBarTests";
            this.progressBarTests.Size = new System.Drawing.Size(1277, 23);
            this.progressBarTests.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBarTests.TabIndex = 8;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1806, 795);
            this.Controls.Add(this.progressBarTests);
            this.Controls.Add(this.btnUploadTestProject);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtBox_OutPut);
            this.Controls.Add(this.btn_Run);
            this.Controls.Add(this.treeView_Tests);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "TestRunner";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeView_Tests;
        private System.Windows.Forms.Button btn_Run;
        private System.Windows.Forms.TextBox txtBox_OutPut;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.OpenFileDialog FileUploadDailog;
        private System.Windows.Forms.Button btnUploadTestProject;
        private System.Windows.Forms.ProgressBar progressBarTests;
        private CustomProgressBar progressBarTests;
    }
}

