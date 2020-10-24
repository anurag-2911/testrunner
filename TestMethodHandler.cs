using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace TestRunner
{

    public class TestMethodHandler
    {
        [DllImport("user32.dll")]
        private static extern bool SendMessage(IntPtr hWnd, Int32 msg, Int32 wParam, Int32 lParam);

        private const Int32 WM_USER = 0x0400;
        private const Int32 CCM_FIRST = 0x2000;
        private const Int32 PBM_SETBARCOLOR = WM_USER + 9;
        private const Int32 PBM_SETBKCOLOR = CCM_FIRST + 1;


        public TreeNode currentNode { get; set; }
        TextBox textBoxParameters;
        Label lblParameters;
        CustomProgressBar progressBar;
        TextBox outputText;
        public object SelectedItem { get; set; }
        private readonly Type selectedType;
        private object reflectedObject;

        public TestMethodHandler(TextBox textBoxParameters,Label lblParameters,
            CustomProgressBar progressBar,TextBox outputText,Type selectedClass)
        {
            this.textBoxParameters = textBoxParameters;
            this.lblParameters = lblParameters;
            this.progressBar = progressBar;
            this.outputText = outputText;
            this.selectedType = selectedClass;
            
        }
        public void RunAllTestsButtonClicked(object sender,EventArgs e)
        {           
            OutputMessage.Instance.ResetMessage(outputText);
            
            if (reflectedObject == null)
            {
                reflectedObject = GetReflectedObject(selectedType);
            }
            
            FixtureSetup(reflectedObject);
            
            foreach (var item in MethodList)
            {
                TreeNode treeNode = MethodTree.FirstOrDefault(x => x.Value == item).Key;
                if(treeNode!=null)
                {
                    currentNode = treeNode;
                }
                
                RunTest(item);
            }
            FixtureTearDown(reflectedObject);

            OutputMessage.Instance.CommitMessage(outputText);
        }
        public void RunButtonClicked(object sender, EventArgs e)
        {
            if (currentNode == null)
            {
                MessageBox.Show("No test method is selected!!");
                return;
            }
            if(reflectedObject==null)
            {
                reflectedObject = GetReflectedObject(MethodTree[currentNode]);
            }
            string message = CheckParameters(currentNode, textBoxParameters, lblParameters);
            if (!string.IsNullOrEmpty(message) && string.IsNullOrEmpty(textBoxParameters.Text))
            {
                MessageBox.Show(message);
                return;
            }

            OutputMessage.Instance.ResetMessage(outputText);

            FixtureSetup(reflectedObject);
            
            RunTest();

            FixtureTearDown(reflectedObject);
        }
        private void SetColor(Color color)
        {
            this.progressBar.ForeColor = color;
            this.progressBar.BackColor = color;
            this.currentNode.ForeColor = color;
            // currentTreeNode.ForeColor = color;
        }

        private void HandleFailure(MethodInfo methodInfo, Exception ex)
        {
            SetColor(Color.Red);
            
            Exception innerException = ex.InnerException;
            
            string message = string.Empty;
            if (innerException != null)
            {
                message = innerException.ToString();

            }
            message = message + Environment.NewLine + methodInfo.Name + " Test Failed";
            OutputMessage.Instance.WriteMessage(message);
            textBoxParameters.Visible = false;
            lblParameters.Visible = false;
        }
        private void RunTest()
        {
            if (MethodTree == null || this.currentNode == null)
            {
                return;
            }

            MethodInfo methodInfo = null;
            if (MethodTree.ContainsKey(this.currentNode))
            {
                MethodTree.TryGetValue(this.currentNode, out methodInfo);
            }
           
            RunTest(methodInfo);
            
            OutputMessage.Instance.CommitMessage(outputText);
        }
       

        private void SetProgressBar()
        {
            progressBar.Maximum = 100;
            progressBar.Step = 10;
            progressBar.Value = 0;
        }

        private void RunTest(MethodInfo methodInfo)
        {
            try
            {
                if (methodInfo != null)
                {
                    SetColor(Color.Orange);

                    MethodSetup(reflectedObject);

                    RunMethod(methodInfo,textBoxParameters);

                    MethodTearDown(reflectedObject);

                    SetColor(Color.Green);
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
        public void PopulateMethods(Type testclass)
        {
            
            MethodInfo[] methods = testclass.GetMethods();

            foreach (var item in methods)
            {
                IEnumerable<CustomAttributeData> customAttributes = item.CustomAttributes;
                foreach (var attribute in customAttributes)
                {
                    if (attribute.AttributeType.Name == "TestFixtureSetUpAttribute")
                    {
                        TestFixtureSetup = item;
                        break;
                    }
                    if (attribute.AttributeType.Name == "TestFixtureTearDownAttribute")
                    {
                        TestFixtureTearDown = item;
                        break;
                    }
                    if (attribute.AttributeType.Name == "SetUpAttribute")
                    {
                        TestMethodSetup = item;
                        break;
                    }
                    if (attribute.AttributeType.Name == "TearDownAttribute")
                    {
                        TestMethodTearDown = item;
                        break;
                    }
                    if (attribute.AttributeType.Name == "TestAttribute")
                    {
                        MethodList.Add(item);
                        break;
                    }
                }

            }

        }
        public MethodInfo TestFixtureSetup { get; private set; } = null;

        public MethodInfo TestFixtureTearDown { get; private set; } = null;

        public MethodInfo TestMethodSetup { get; private set; } = null;

        public MethodInfo TestMethodTearDown { get; private set; } = null;

        public List<MethodInfo> MethodList { get; } = new List<MethodInfo>();

        public void PopulateMethodTree(TreeNode node, MethodInfo methodInfo)
        {
            MethodTree.Add(node, methodInfo);
        }

        public Dictionary<TreeNode, MethodInfo> MethodTree { get; } = new Dictionary<TreeNode, MethodInfo>();

       
        private void FixtureTearDown(object obj)
        {
            if (this.TestFixtureTearDown != null)
            {
                this.TestFixtureTearDown.Invoke(obj, new object[] { });
            }
        }

        private void MethodTearDown(object obj)
        {
            if (this.TestMethodTearDown != null)
            {
                this.TestMethodTearDown.Invoke(obj, new object[] { });
            }
        }

        

        private void MethodSetup(object obj)
        {
            if (this.TestMethodSetup != null)
            {
                this.TestMethodSetup.Invoke(obj, new object[] { });
            }
        }

        private void FixtureSetup(object obj)
        {
            if (this.TestFixtureSetup != null)
            {
                this.TestFixtureSetup.Invoke(obj, new object[] { });
            }
        }

        private object GetReflectedObject(Type type)
        {
            ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);

            object obj = constructor.Invoke(new object[] { });
            return obj;
        }
        
        private object GetReflectedObject(MethodInfo methodInfo)
        {
            Type type = methodInfo.ReflectedType;

            ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);

            object obj = constructor.Invoke(new object[] { });
            return obj;
        }
                
        private static string GetParammeters(MethodInfo methodInfo, ref ParameterInfo[] parameters)
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
                parameterMessage.Remove(parameterMessage.Length - 1, 1);

            }
            return parameterMessage.ToString();
        }
        private void RunMethod(MethodInfo methodInfo, TextBox textBoxParameters)
        {
            
            StringBuilder message = new StringBuilder();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

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
                methodInfo.Invoke(reflectedObject, new object[] { });
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
                methodInfo.Invoke(reflectedObject, new object[] { counters[0] });
            }


            stopwatch.Stop();
           
            message.AppendLine();
            message.AppendLine("Method " + methodInfo.Name);
            message.AppendLine("Passed");
            message.AppendLine("Time taken to run method: " + stopwatch.ElapsedMilliseconds + " milliseconds");

            OutputMessage.Instance.WriteMessage(message.ToString());


        }

        public string CheckParameters(TreeNode currentTreeNode,TextBox textBoxParameters, Label lblParameters)
        {
            MethodInfo methodInfo;
            string message = string.Empty;

            MethodTree.TryGetValue(currentTreeNode, out methodInfo);
            if (methodInfo != null)
            {
                try
                {
                    ParameterInfo[] parameters = null;
                    message = GetParammeters(methodInfo, ref parameters);
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

        


    }
}
