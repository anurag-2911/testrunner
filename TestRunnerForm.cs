using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestRunner
{

    public partial class TestRunnerForm : Form
    {
      
        AssemblyLoader assemblyLoader;
        TestClassHandler testClass;
        TestMethodHandler methodHandler;
        TreeNode currentTreeNode;
        CancellationTokenSource cancellationTokenSource_buttonRunMethod;
        CancellationToken runbuttonTask;
        CancellationTokenSource cancellationTokenSource_buttonRunAllMethod;
        CancellationToken runAllbuttonTask;
        readonly string traceoutput = Environment.CurrentDirectory + "\\traceOutput.txt";
        TextWriterTraceListener textWriterTraceListener; 
        

        public TestRunnerForm()
        {
            InitializeComponent();
            assemblyLoader = new AssemblyLoader();
            testClass = new TestClassHandler();
            OutputMessage.Instance.SetTextBox(txtBoxOutput);
            
        }

        private void attachToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AttachDll();

        }
        private void dropdownTestClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            object selectedItem = dropdownTestClass.SelectedItem;
            btnRunAllTests.Enabled = true;
            Type testclass;

            testClass.GetTestClasses.TryGetValue(selectedItem.ToString(), out testclass);

            PopulateMethodTree(testclass);

        }
        private void treeView_Tests_AfterSelect(object sender, TreeViewEventArgs treeViewEventArgs)
        {
            btn_Run.Enabled = true;
            currentTreeNode = treeViewEventArgs.Node;
            methodHandler.currentNode = currentTreeNode;
            OutputMessage.Instance.ResetMessage();
            methodHandler.CheckParameters(currentTreeNode, textBoxParameters, lblParameters);
            
        }    
      
        private void btn_Run_Click(object sender, EventArgs e)
        {
            StartTraceListener();
            
            SwitchButtonState(btnRunAllTests, false);
            SwitchButtonState(btnStopTests, true);
            
            cancellationTokenSource_buttonRunMethod = new CancellationTokenSource();

            runbuttonTask = cancellationTokenSource_buttonRunMethod.Token;
            try
            {
                Task runmethodThread = Task.Factory.StartNew(() =>
                {
                    methodHandler.RunButtonClicked(sender, e);
                }, runbuttonTask);

                runmethodThread.ContinueWith(RunMethodCompleted);
            }
            catch (Exception ex)
            {

            }

            EndTraceListener();
        }

        private static void EndTraceListener()
        {
            try
            {
                Trace.Flush();
            }
            catch { }
        }

        private void StartTraceListener()
        {
            try
            {
                if(File.Exists(traceoutput))
                {
                    File.Delete(traceoutput);
                }
                Thread.Sleep(10);
                textWriterTraceListener = new TextWriterTraceListener(traceoutput);
                Trace.Listeners.Add(textWriterTraceListener);
            }
            catch { }
        }

        private void RunMethodCompleted(Task obj)
        {
            SwitchButtonState(btnStopTests, false);
            SwitchButtonState(btnRunAllTests, true);
        }


        private void btnRunAllTests_Click(object sender, EventArgs e)
        {
            StartTraceListener();
            SwitchButtonState(btn_Run, false);
            SwitchButtonState(btnStopTests, true);
            cancellationTokenSource_buttonRunAllMethod = new CancellationTokenSource();
            runAllbuttonTask = cancellationTokenSource_buttonRunAllMethod.Token;
            try
            {
                Task runallmethodsThread = Task.Factory.StartNew(() =>
                 {
                     methodHandler.RunAllTestsButtonClicked(sender, e);
                 },runAllbuttonTask);
                runallmethodsThread.ContinueWith(RunAllMethodCompleted);
            }
            catch(Exception ex)
            {

            }
            EndTraceListener();
        }

        private void RunAllMethodCompleted(Task obj)
        {
            SwitchButtonState(btnStopTests, false);
            SwitchButtonState(btn_Run, true);
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
            methodHandler = new TestMethodHandler(textBoxParameters, lblParameters, progressBarTests, txtBoxOutput,testclass);
            
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
        private delegate void SafeCallDelegate(Button stopButton, bool value);
        private void SwitchButtonState(Button button, bool value)
        {
            if (button.InvokeRequired)
            {
                var d = new SafeCallDelegate(SwitchButtonState);
                button.Invoke(d, new object[] { button, value });
            }
            else
            {
                button.Enabled = value;
            }
        }
       
        private void btnStopTests_Click(object sender, EventArgs e)
        {
            //todo: find a better way
            if (cancellationTokenSource_buttonRunMethod != null)
            {
                cancellationTokenSource_buttonRunMethod.Cancel();
                
            }
            if (cancellationTokenSource_buttonRunAllMethod != null)
            {
                cancellationTokenSource_buttonRunAllMethod.Cancel();
            }

            MessageBox.Show("Test execution stopped");
            
        }
    }
}
