using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestRunner
{
    public class TestMethodHandler
    {
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

        public void RunMethod(MethodInfo methodInfo,TextBox textBoxParameters,TextBox outputBox,bool runAll=false)
        {
            Type type = methodInfo.ReflectedType;

            ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);

            object obj = constructor.Invoke(new object[] { });
            OutputMessage.Instance.ResetMessage(outputBox);


            RunTestMethod(methodInfo, textBoxParameters, obj);

            OutputMessage.Instance.CommitMessage(outputBox);
        }

        private void RunTestMethod(MethodInfo methodInfo, TextBox textBoxParameters, object obj)
        {
            
            MethodSetup(obj);

            RunMethod(methodInfo, obj, textBoxParameters);

            MethodTearDown(obj);
            
            
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
        private void RunMethod(MethodInfo methodInfo, object obj,TextBox textBoxParameters)
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
