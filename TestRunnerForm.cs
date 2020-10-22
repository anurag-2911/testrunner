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


        TreeNode currentTreeNode;
        Dictionary<TreeNode, MethodInfo> methodTree = new Dictionary<TreeNode, MethodInfo>();
        string testResultFilePath = Environment.CurrentDirectory + "\\TestResult.txt";
        MethodInfo testFixtureSetup = null;
        MethodInfo testFixtureTearDown = null;
        MethodInfo testMethodSetup = null;
        MethodInfo testMethodTearDown = null;
        Dictionary<string,Type> testClasses = new Dictionary<string, Type>();

        public TestRunnerForm()
        {
            InitializeComponent();
        }

        private void treeView_Tests_AfterSelect(object sender, TreeViewEventArgs treeViewEventArgs)
        {
            currentTreeNode = treeViewEventArgs.Node;
            this.ResetResultWindow();
            string parameters=CheckParameters();
            if(string.IsNullOrEmpty(parameters))
            {
                textBoxParameters.Visible = false;
                lblParameters.Visible = false;
            }
        }

        private string CheckParameters()
        {
            MethodInfo methodInfo;
            string message = string.Empty;
           
            methodTree.TryGetValue(currentTreeNode, out methodInfo);
            if (methodInfo != null)
            {
                try
                {
                    ParameterInfo[] parameters = null;
                    message= GetParammeters(methodInfo, ref parameters);
                    if (parameters.Length > 0)
                    {
                        textBoxParameters.Visible = true;
                        lblParameters.Visible = true;
                        lblParameters.Width = parameters.Length + 10;
                        lblParameters.Text = message;
                    }
                }
                catch (Exception)
                {

                }

            }
            return message;
        }

        private static string GetParammeters(MethodInfo methodInfo,ref ParameterInfo[] parameters)
        {
            StringBuilder parameterMessage = new StringBuilder();
            parameters = methodInfo.GetParameters();
            if (parameters.Length > 0)
            {
                
                parameterMessage.Append("Enter parameter(s) ");
                foreach (var item in parameters)
                {
                    parameterMessage.Append(item.Name);
                    parameterMessage.Append(",");
                }
                parameterMessage.Remove(parameterMessage.Length-1, 1);
                
            }
            return parameterMessage.ToString();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void PopulateTestTree(List<MethodInfo> testMethods)
        {
            try
            {
                treeView_Tests.Nodes.Clear();
                methodTree.Clear();

                foreach (var item in testMethods)
                {
                    TreeNode treeNode = new TreeNode(item.Name);
                    methodTree.Add(treeNode, item);
                }

                treeView_Tests.Nodes.AddRange(methodTree.Keys.ToArray());

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
                
            }
            
        }

        private void AttachDll()
        {
            

            if (FileUploadDailog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var filePath = FileUploadDailog.FileName;

                    Assembly testDll = Assembly.LoadFrom(filePath);

                    lblTestClass.Visible = true;
                    dropdownTestClass.Visible = true;

                    PopulateTestClasses(testDll);
                    
                   
                }
                catch (SecurityException ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        private void PopulateMethods(Type testclass)
        {
            List<MethodInfo> methodsList = new List<MethodInfo>();
            MethodInfo[] methods = testclass.GetMethods();

            foreach (var item in methods)
            {
                IEnumerable<CustomAttributeData> customAttributes = item.CustomAttributes;
                foreach (var attribute in customAttributes)
                {
                    if (attribute.AttributeType.Name == "TestFixtureSetUpAttribute")
                    {
                        testFixtureSetup = item;
                        break;
                    }
                    if (attribute.AttributeType.Name == "TestFixtureTearDownAttribute")
                    {
                        testFixtureTearDown = item;
                        break;
                    }
                    if (attribute.AttributeType.Name == "SetUpAttribute")
                    {
                        testMethodSetup = item;
                        break;
                    }
                    if (attribute.AttributeType.Name == "TearDownAttribute")
                    {
                        testMethodTearDown = item;
                        break;
                    }
                    if (attribute.AttributeType.Name == "TestAttribute")
                    {
                        methodsList.Add(item);
                        break;
                    }
                }


            }

            PopulateTestTree(methodsList);
        }

        private void PopulateTestClasses(Assembly testDll)
        {
            Type[] testclass = testDll.GetTypes();
            
            foreach (var item in testclass)
            {
                IEnumerable<CustomAttributeData> customAttributes = item.CustomAttributes;
                foreach (var attribute in customAttributes)
                {
                    if (attribute.AttributeType.Name == "TestFixtureAttribute")
                    {
                        testClasses.Add(item.Name, item);
                        break;
                    }
                    
                }

            }

            if (testClasses != null && testClasses.Keys != null)
            {
                dropdownTestClass.Items.AddRange(testClasses.Keys.ToArray());
            }
        }

        private void btn_Run_Click(object sender, EventArgs e)
        {
            if(currentTreeNode==null)
            {
                MessageBox.Show("No test is selected!!");
                return;
            }
            string message = CheckParameters();
            if(!string.IsNullOrEmpty(message) && string.IsNullOrEmpty(textBoxParameters.Text))
            {
                MessageBox.Show(message);
                return;
            }
            
            RunTest();
        }

        private void RunTest()
        {
            if (methodTree == null || currentTreeNode == null)
            {
                return;
            }

            MethodInfo methodInfo = null;
            if (methodTree.ContainsKey(currentTreeNode))
            {
                methodTree.TryGetValue(currentTreeNode, out methodInfo);
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
            RunMethod(methodInfo);
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
            txtBox_OutPut.Text = message;
            textBoxParameters.Visible = false;
            lblParameters.Visible = false;
        }

        private void PopulateOutputMessage(string msg)
        {
            txtBox_OutPut.Text = string.Empty;

            txtBox_OutPut.Text = msg;
        }

        private void RunMethod(MethodInfo methodInfo)
        {
            Type type = methodInfo.ReflectedType;

            ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);

            object obj = constructor.Invoke(new object[] { });

            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();
            Setup(obj);

            RunMethod(methodInfo, obj, stopwatch);

            TearDown(obj);

            textBoxParameters.Visible = false;

            lblParameters.Visible = false;

        }

        private void TearDown(object obj)
        {
            if (testMethodTearDown != null)
            {
                testMethodTearDown.Invoke(obj, new object[] { });
            }
            if (testFixtureTearDown != null)
            {
                testFixtureTearDown.Invoke(obj, new object[] { });
            }
        }

        private void Setup(object obj)
        {
            if (testFixtureSetup != null)
            {
                testFixtureSetup.Invoke(obj, new object[] { });
            }
            if (testMethodSetup != null)
            {
                testMethodSetup.Invoke(obj, new object[] { });
            }
        }

        private void RunMethod(MethodInfo methodInfo, object obj, Stopwatch stopwatch)
        {
            ParameterInfo[] parameterInfo = null;
            StringBuilder methodResult = new StringBuilder();

            GetParammeters(methodInfo, ref parameterInfo);

            string parameters = string.Empty;

            if (parameterInfo != null && parameterInfo.Length > 0)
            {
                parameters = textBoxParameters.Text;
            }

            if (string.IsNullOrEmpty(parameters))
            {
                methodInfo.Invoke(obj, new object[] { });
            }

            else
            {
                //Todo: make it generic
                object[] args = parameters.Split(',');
                int[] counters = new int[args.Length];
                for (int i = 0; i < args.Length; i++)
                {
                    counters[i] = Convert.ToInt32(args[i]);
                }
                methodInfo.Invoke(obj, new object[] { counters[0] });
            }



            txtBox_OutPut.Text = string.Empty;
            stopwatch.Stop();
            progressBarTests.Value = 100;
            long timeTakentoRunMethod = stopwatch.ElapsedMilliseconds;
            methodResult.AppendLine();
            methodResult.AppendLine("Method " + methodInfo.Name);
            methodResult.AppendLine("Passed");
            methodResult.AppendLine("Time taken to run method: " + stopwatch.ElapsedMilliseconds +" milliseconds");

            if (File.Exists(testResultFilePath))
            {
                string[] output = File.ReadAllLines(testResultFilePath);
                foreach (var item in output)
                {
                    methodResult.AppendLine(item);
                }
            }

            
            methodResult.AppendLine();
            methodResult.AppendLine();

            PopulateOutputMessage(methodResult.ToString());
            
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
            StringBuilder message = new StringBuilder();
            message.AppendLine("1. Attach the test dll ");
            message.AppendLine("2. All dependent dll which test dll needs should be in same folder as test dll");
            message.AppendLine("3. Select test class");
            message.AppendLine("4. Run test(s)");
            MessageBox.Show(message.ToString());
        }

        private void contactToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("anurag.kumar@microfocus.com");
        }

        private void dropdownTestClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            object selectedItem = dropdownTestClass.SelectedItem;
            Type testclass = null;
            testClasses.TryGetValue(selectedItem.ToString(), out testclass);
           
            if (testclass!=null)
            {
                PopulateMethods(testclass);
            }

        }
    }
}
