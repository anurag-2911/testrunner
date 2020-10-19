using System.Drawing;

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
            this.lblParameters = new System.Windows.Forms.Label();
            this.textBoxParameters = new System.Windows.Forms.TextBox();
            this.FileUploadDailog = new System.Windows.Forms.OpenFileDialog();
            this.btnUploadTestProject = new System.Windows.Forms.Button();
            this.progressBarTests = new TestRunner.CustomProgressBar();
            this.SuspendLayout();
            // 
            // treeView_Tests
            // 
            this.treeView_Tests.Location = new System.Drawing.Point(36, 99);
            this.treeView_Tests.Name = "treeView_Tests";
            this.treeView_Tests.Size = new System.Drawing.Size(447, 455);
            this.treeView_Tests.TabIndex = 0;
            this.treeView_Tests.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_Tests_AfterSelect);
            // 
            // btn_Run
            // 
            this.btn_Run.Location = new System.Drawing.Point(36, 49);
            this.btn_Run.Name = "btn_Run";
            this.btn_Run.Size = new System.Drawing.Size(152, 44);
            this.btn_Run.TabIndex = 1;
            this.btn_Run.Text = "Run Test";
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
            // lblParameters
            // 
            this.lblParameters.AutoSize = true;
            this.lblParameters.Location = new System.Drawing.Point(33, 574);
            this.lblParameters.Name = "lblParameters";
            this.lblParameters.Size = new System.Drawing.Size(81, 17);
            this.lblParameters.TabIndex = 5;
            this.lblParameters.Text = "Parameters";
            this.lblParameters.Visible = false;
            // 
            // textBoxParameters
            // 
            this.textBoxParameters.Location = new System.Drawing.Point(36, 626);
            this.textBoxParameters.Name = "textBoxParameters";
            this.textBoxParameters.Size = new System.Drawing.Size(447, 22);
            this.textBoxParameters.TabIndex = 6;
            this.textBoxParameters.Visible = false;
            // 
            // FileUploadDailog
            // 
            this.FileUploadDailog.FileName = "FileUploadDialog";
            // 
            // btnUploadTestProject
            // 
            this.btnUploadTestProject.Location = new System.Drawing.Point(463, 3);
            this.btnUploadTestProject.Name = "btnUploadTestProject";
            this.btnUploadTestProject.Size = new System.Drawing.Size(213, 41);
            this.btnUploadTestProject.TabIndex = 7;
            this.btnUploadTestProject.Text = "Upload Test Assembly";
            this.btnUploadTestProject.UseVisualStyleBackColor = true;
            this.btnUploadTestProject.Click += new System.EventHandler(this.btnUploadTestProject_Click);
            // 
            // progressBarTests
            // 
            this.progressBarTests.BackColor = System.Drawing.Color.Green;
            this.progressBarTests.ForeColor = System.Drawing.Color.Green;
            this.progressBarTests.Location = new System.Drawing.Point(500, 652);
            this.progressBarTests.Name = "progressBarTests";
            this.progressBarTests.Size = new System.Drawing.Size(1277, 23);
            this.progressBarTests.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBarTests.TabIndex = 8;
            // 
            // TestRunnerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1806, 795);
            this.Controls.Add(this.progressBarTests);
            this.Controls.Add(this.btnUploadTestProject);
            this.Controls.Add(this.textBoxParameters);
            this.Controls.Add(this.lblParameters);
            this.Controls.Add(this.txtBox_OutPut);
            this.Controls.Add(this.btn_Run);
            this.Controls.Add(this.treeView_Tests);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TestRunnerForm";
            this.Text = "TestRunner";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeView_Tests;
        private System.Windows.Forms.Button btn_Run;
        private System.Windows.Forms.TextBox txtBox_OutPut;
        private System.Windows.Forms.Label lblParameters;
        private System.Windows.Forms.TextBox textBoxParameters;
        private System.Windows.Forms.OpenFileDialog FileUploadDailog;
        private System.Windows.Forms.Button btnUploadTestProject;
        private CustomProgressBar progressBarTests;
        
    }
}

