using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        RichTextBox outputText;
        public object SelectedItem { get; set; }
        private readonly Type selectedType;
        private object reflectedObject;
        private const int Fill = 50;
        public TestMethodHandler(TextBox textBoxParameters,Label lblParameters,
            CustomProgressBar progressBar,RichTextBox outputText,Type selectedClass)
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
            
            StringBuilder message = new StringBuilder();
            string exceptioInfo = string.Empty;
            if (innerException != null)
            {
                exceptioInfo = innerException.ToString();

            }
            else
            {
                exceptioInfo = ex.ToString();
            }
            string fill = new String('*', Fill);
            message.AppendLine(fill);
            message.AppendLine("Method Name: " + methodInfo.Name);
            message.AppendLine("Result: Failed");
            if (!string.IsNullOrEmpty(exceptioInfo))
            {
                message.AppendLine(exceptioInfo);
            }
            //message.AppendLine("Time taken to run method: " + stopwatch.ElapsedMilliseconds + " milliseconds");
            message.AppendLine(fill);
            message.AppendLine();
            
            OutputMessage.Instance.WriteMessage(message.ToString());
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

        public static int Fill1 => Fill;

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
            
            GetParammeters(methodInfo, ref parameterInfo);

            string parametersInput = string.Empty;

            if (parameterInfo != null && parameterInfo.Length > 0)
            {
                parametersInput = textBoxParameters.Text;
            }

            if (parameterInfo.Length==0)
            {
                methodInfo.Invoke(reflectedObject, new object[] { });
            }

            else
            {
                object[] arguments = new object[parameterInfo.Length];

                object[] inputParamsArray = parametersInput.Split(',');
                
                for (int i = 0; i < parameterInfo.Length; i++)
                {
                    TypeConverter typeConverter = TypeDescriptor.GetConverter(parameterInfo[i].ParameterType);
                    if (!string.IsNullOrEmpty(textBoxParameters.Text))
                    {
                        arguments[i] = typeConverter.ConvertFrom(inputParamsArray[i]);
                    }
                    else
                    {
                        arguments[i] = GetDefaultValue(parameterInfo[i].ParameterType);
                    }
                }
                            
                               
                methodInfo.Invoke(reflectedObject, arguments);
            }


            stopwatch.Stop();
            string fill = new String('*', Fill);
            message.AppendLine(fill);
            message.AppendLine("Method Name: " + methodInfo.Name);
            message.AppendLine("Result: Passed");
            message.AppendLine("Time taken to run method: " + stopwatch.ElapsedMilliseconds + " milliseconds");
            message.AppendLine(fill);
            message.AppendLine();

            OutputMessage.Instance.WriteMessage(message.ToString());


        }

        private object GetDefaultValue(Type t)
        {
            if (t.IsValueType)
                return Activator.CreateInstance(t);

            return null;
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
