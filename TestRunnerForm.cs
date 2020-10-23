using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Windows.Forms;

namespace TestRunner
{

    public partial class TestRunnerForm : Form
    {
      
        AssemblyLoader assemblyLoader;
        TestClassHandler testClass;
        TestMethodHandler methodHandler;
        TreeNode currentTreeNode;         
                
        public TestRunnerForm()
        {
            InitializeComponent();
            assemblyLoader = new AssemblyLoader();
            testClass = new TestClassHandler();
            
        }
        private void attachToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AttachDll();

        }
        private void dropdownTestClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            object selectedItem = dropdownTestClass.SelectedItem;

            Type testclass;

            testClass.GetTestClasses.TryGetValue(selectedItem.ToString(), out testclass);

            PopulateMethodTree(testclass);

        }
        private void treeView_Tests_AfterSelect(object sender, TreeViewEventArgs treeViewEventArgs)
        {
            btn_Run.Enabled = true;
            currentTreeNode = treeViewEventArgs.Node;
            methodHandler.currentNode = currentTreeNode;
        }    
               
      
        private void btn_Run_Click(object sender, EventArgs e)
        {
            methodHandler.RunButtonClicked(sender, e);            
            
        }

        private void RunAllTests_Click(object sender, System.EventArgs e)
        {

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

        private void PopulateMethodTree(Type testclass)
        {
            methodHandler = new TestMethodHandler(textBoxParameters, lblParameters, progressBarTests, txtBox_OutPut);

            if (testclass != null)
            {
                PopulateMethods(testclass);
            }

        }
        private void PopulateMethods(Type testclass)
        {
            methodHandler.PopulateMethods(testclass);
            PopulateTestTree(methodHandler.MethodList);
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


        private void PopulateTestClasses(Assembly testDll)
        {
            testClass.PopulateTestClasses(testDll);

            if (testClass.GetTestClasses != null && testClass.GetTestClasses.Keys != null)
            {
                dropdownTestClass.Items.AddRange(testClass.GetTestClasses.Keys.ToArray());
            }
        }
    }
}
