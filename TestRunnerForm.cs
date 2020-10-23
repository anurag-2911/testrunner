using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestRunner
{
    
    public partial class TestRunnerForm : Form
    {
        [DllImport("user32.dll")]
        private static extern bool SendMessage(IntPtr hWnd, Int32 msg, Int32 wParam, Int32 lParam);

        private const Int32 WM_USER = 0x0400;
        private const Int32 CCM_FIRST = 0x2000;
        private const Int32 PBM_SETBARCOLOR = WM_USER + 9;
        private const Int32 PBM_SETBKCOLOR = CCM_FIRST + 1;

        AssemblyLoader assemblyLoader;
        TestClassHandler testClass;
        TestMethodHandler methodHandler;
        TreeNode currentTreeNode;         
                
        public TestRunnerForm()
        {
            InitializeComponent();
            assemblyLoader = new AssemblyLoader();
            testClass = new TestClassHandler();
            methodHandler = new TestMethodHandler();
        }

        private void treeView_Tests_AfterSelect(object sender, TreeViewEventArgs treeViewEventArgs)
        {
            currentTreeNode = treeViewEventArgs.Node;
            this.ResetResultWindow();
            string parameters=methodHandler.CheckParameters(currentTreeNode,textBoxParameters,lblParameters);
            if(string.IsNullOrEmpty(parameters))
            {
                textBoxParameters.Visible = false;
                lblParameters.Visible = false;
            }
        }    
               
        private void PopulateTestTree(List<MethodInfo> testMethods)
        {
            try
            {
                treeView_Tests.Nodes.Clear();
                methodHandler.MethodTree.Clear();

                foreach (var item in testMethods)
                {
                    TreeNode treeNode = new TreeNode(item.Name);
                    methodHandler.MethodTree.Add(treeNode, item);
                }

                treeView_Tests.Nodes.AddRange(methodHandler.MethodTree.Keys.ToArray());

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
                
            }
            
        }

        /// <summary>
        /// This method will be called to load the test library
        /// </summary>
        private void AttachDll()
        {
            
            if (FileUploadDailog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Assembly testDll = assemblyLoader.LoadTestAssembly(FileUploadDailog.FileName);

                    lblTestClass.Visible = true;
                    dropdownTestClass.Visible = true;

                    PopulateTestClasses(testDll);
                    
                   
                }
                catch (SecurityException ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private void PopulateMethods(Type testclass)
        {
            methodHandler.PopulateMethods(testclass);
            PopulateTestTree(methodHandler.MethodList);
        }

        private void PopulateTestClasses(Assembly testDll)
        {
            testClass.PopulateTestClasses(testDll);

            if (testClass.GetTestClasses != null && testClass.GetTestClasses.Keys != null)
            {
                dropdownTestClass.Items.AddRange(testClass.GetTestClasses.Keys.ToArray());
            }
        }

        private void btn_Run_Click(object sender, EventArgs e)
        {
            if(currentTreeNode==null)
            {
                MessageBox.Show("No test is selected!!");
                return;
            }
            string message = methodHandler.CheckParameters(currentTreeNode,textBoxParameters,lblParameters);
            if(!string.IsNullOrEmpty(message) && string.IsNullOrEmpty(textBoxParameters.Text))
            {
                MessageBox.Show(message);
                return;
            }
            
            RunTest();
        }

        private void RunTest()
        {
            if (methodHandler.MethodTree == null || currentTreeNode == null)
            {
                return;
            }

            MethodInfo methodInfo = null;
            if (methodHandler.MethodTree.ContainsKey(currentTreeNode))
            {
                methodHandler.MethodTree.TryGetValue(currentTreeNode, out methodInfo);
            }
            RunTest(methodInfo);
        }

        private void RunTest(MethodInfo methodInfo)
        {
            try
            {
                if (methodInfo != null)
                {
                    RunTestMethod(methodInfo);

                }
            }
            catch (AssertionException ex)
            {
                HandleFailure(methodInfo, ex);
            }
            catch (Exception ex)
            {
                HandleFailure(methodInfo, ex);
            }
        }

        private void RunTestMethod(MethodInfo methodInfo)
        {
            progressBarTests.Maximum = 100;
            progressBarTests.Step = 10;
            progressBarTests.Value = 0;
            SetColor(Color.Yellow);
            methodHandler.RunMethod(methodInfo,textBoxParameters,txtBox_OutPut);
            SetColor(Color.Green);
        }

        private void SetColor(Color color)
        {
            this.progressBarTests.ForeColor = color;
            this.progressBarTests.BackColor = color;
            currentTreeNode.BackColor = color;
           // currentTreeNode.ForeColor = color;
        }

        private void HandleFailure(MethodInfo methodInfo, Exception ex)
        {
            txtBox_OutPut.Text = string.Empty;
            
            SendMessage(progressBarTests.Handle, 1040, 2, 0);
            SetColor(Color.Red);

            Exception innerException = ex.InnerException;
            string message = string.Empty;
            if (innerException != null)
            {
                message = innerException.ToString();

            }
            message = message + Environment.NewLine + methodInfo.Name + " Test Failed";
            OutputMessage.Instance.WriteMessage(message);
            OutputMessage.Instance.CommitMessage(txtBox_OutPut);
            //txtBox_OutPut.Text = message;
            textBoxParameters.Visible = false;
            lblParameters.Visible = false;
        }                             

       
        private void ResetResultWindow()
        {
            txtBox_OutPut.Text = string.Empty;
        }

        private void attachToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AttachDll();
            btn_Run.Enabled = true;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void howToUseToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
            MessageBox.Show(InformationMessage.Instance.HowToUseMessage());
        }

        private void contactToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(InformationMessage.Instance.ContactMessage());
        }

        private void dropdownTestClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            object selectedItem = dropdownTestClass.SelectedItem;
            Type testclass;
            testClass.GetTestClasses.TryGetValue(selectedItem.ToString(), out testclass);
           
            if (testclass!=null)
            {
                PopulateMethods(testclass);
            }

        }
    }
}
