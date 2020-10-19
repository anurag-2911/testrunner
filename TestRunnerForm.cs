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

        public TestRunnerForm()
        {
            InitializeComponent();
        }

        private void treeView_Tests_AfterSelect(object sender, TreeViewEventArgs treeViewEventArgs)
        {
            currentTreeNode = treeViewEventArgs.Node;
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
                catch (Exception exception)
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

        private void btnUploadTestProject_Click(object sender, EventArgs e)
        {
            string[] testclasses = ConfigurationManager.AppSettings.GetValues("TestClasses");
            List<MethodInfo> methodsList = new List<MethodInfo>();
            if (FileUploadDailog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var filePath = FileUploadDailog.FileName;
                    Assembly testDll = Assembly.LoadFrom(filePath);
                    List<Type> classes = new List<Type>();
                    MethodInfo[] methods=null;
                    foreach (var item in testclasses)
                    {
                        classes.Add(testDll.GetType(item));
                    }
                    foreach (var item in classes)
                    {
                       methods = item.GetMethods();
                        
                    }
                    foreach (var item in methods)
                    {
                        IEnumerable<CustomAttributeData> customAttributes = item.CustomAttributes;
                        foreach (var attribute in customAttributes)
                        {
                            if(attribute.AttributeType.Name== "TestFixtureSetUpAttribute")
                            {
                                testFixtureSetup = item;
                                break;
                            }
                            if(attribute.AttributeType.Name== "TestFixtureTearDownAttribute")
                            {
                                testFixtureTearDown = item;
                                break;
                            }
                            if(attribute.AttributeType.Name== "SetUpAttribute")
                            {
                                testMethodSetup = item;
                                break;
                            }
                            if(attribute.AttributeType.Name== "TearDownAttribute")
                            {
                                testMethodTearDown = item;
                                break;
                            }
                            if(attribute.AttributeType.Name =="TestAttribute")
                            {
                                methodsList.Add(item);
                                break;
                            }
                        }
                                             
                       
                    }
                    PopulateTestTree(methodsList);
                }
                catch (SecurityException ex)
                {
                    Console.WriteLine(ex.ToString()); 
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        private void btn_Run_Click(object sender, EventArgs e)
        {
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
            if(methodTree ==null || currentTreeNode ==null)
            {
                return;
            }

            MethodInfo methodInfo = null;
            if (methodTree.ContainsKey(currentTreeNode))
            {
                methodTree.TryGetValue(currentTreeNode, out methodInfo);
            }
            try
            {
                if (methodInfo != null)
                {
                    progressBarTests.Maximum = 100;
                    progressBarTests.Step = 10;
                    progressBarTests.Value = 0;

                    RunMethod(methodInfo);
                    this.progressBarTests.ForeColor = Color.Green;
                    this.progressBarTests.BackColor = Color.Green;
                    //currentTreeNode.BackColor = Color.Green;
                    currentTreeNode.ForeColor = Color.Green;

                }
            }
            catch (AssertionException ae)
            {
                txtBox_OutPut.Text = string.Empty;
                txtBox_OutPut.Text=ae.ToString();
            }
            catch (Exception ex)
            {
                txtBox_OutPut.Text = string.Empty;
                //// 1040 - PBM_SETSTATE
                //// 2 - red (error), 3 - yellow (paused), 1 - green (in progress) 
                //SendMessage(progressBarTests.Handle, 1040, 2, 0);
                progressBarTests.ForeColor = Color.Red;
                progressBarTests.BackColor = Color.Red;
                //currentTreeNode.BackColor = Color.Red;
                currentTreeNode.ForeColor = Color.Red;

                Exception innerException = ex.InnerException;
                string message = string.Empty;
                if (innerException != null)
                {
                    message = innerException.ToString();
                    
                }
                message = message +Environment.NewLine+ methodInfo.Name + " Test Failed";
                txtBox_OutPut.Text = message;
                textBoxParameters.Visible = false;
                lblParameters.Visible = false;
            }
        }

        private void RunMethod(MethodInfo methodInfo)
        {
            Type type = methodInfo.ReflectedType;
            
            ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
            
            object obj = constructor.Invoke(new object[] { });
           
            Stopwatch stopwatch = new Stopwatch();
            
            stopwatch.Start();
            
            if (testFixtureSetup != null)
            {
                testFixtureSetup.Invoke(obj, new object[] { });
            }
            if (testMethodSetup != null)
            {
                testMethodSetup.Invoke(obj, new object[] { });
            }
            ParameterInfo[] parameterInfo = null;
            GetParammeters(methodInfo, ref parameterInfo);
            string parameters = string.Empty;
            if(parameterInfo != null && parameterInfo.Length>0)
            {
                parameters = textBoxParameters.Text;
            }
            if (string.IsNullOrEmpty(parameters))
            {
                methodInfo.Invoke(obj, new object[] { });
            }
            else
            {
                object[] args = parameters.Split(',');
                int[] counters = new int[args.Length];
                for (int i = 0; i < args.Length; i++)
                {
                    counters[i] = Convert.ToInt32(args[i]);
                }
                methodInfo.Invoke(obj, new object[] {counters[0] });
            }
            
            
            
            txtBox_OutPut.Text = string.Empty;
            stopwatch.Stop();
            progressBarTests.Value = 100;
            long timeTakentoRunMethod = stopwatch.ElapsedMilliseconds;
            
            string message = string.Format("{0} is Pass in {1} milliseconds ", methodInfo.Name, timeTakentoRunMethod);

            string[] output = File.ReadAllLines(testResultFilePath);
            
            StringBuilder stringBuilder = new StringBuilder();
            
            stringBuilder.AppendLine(message);
            
            foreach (var item in output)
            {
                stringBuilder.AppendLine(item);
            }
            
            txtBox_OutPut.Text = stringBuilder.ToString();
            
            if (testMethodTearDown != null)
            {
                testMethodTearDown.Invoke(obj, new object[] { });
            }
            if (testFixtureTearDown != null)
            {
                testFixtureTearDown.Invoke(obj, new object[] { });
            }
            textBoxParameters.Visible = false;
            lblParameters.Visible = false;

        }
    }
}
