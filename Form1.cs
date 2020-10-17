using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestRunner
{
    public partial class Form1 : Form
    {
        TreeNode currentTreeNode;
        Dictionary<TreeNode, MethodInfo> methodTree = new Dictionary<TreeNode, MethodInfo>();
       
        public Form1()
        {
            InitializeComponent();
        }

        private void treeView_Tests_AfterSelect(object sender, TreeViewEventArgs treeViewEventArgs)
        {
            currentTreeNode = treeViewEventArgs.Node;
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
            RunTest();
        }

        private void RunTest()
        {
            MethodInfo methodInfo = null;
            if (methodTree.ContainsKey(currentTreeNode))
            {
                methodTree.TryGetValue(currentTreeNode, out methodInfo);
            }
            try
            {
                if (methodInfo != null)
                {
                    Type type = methodInfo.ReflectedType;
                    ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
                    object obj = constructor.Invoke(new object[] { });

                    object methodBase = methodInfo.Invoke(obj, new object[] { });
                    string output = Process.GetCurrentProcess().StandardOutput.ReadToEnd();

                }
            }
            catch (AssertionException ae)
            {
                Console.WriteLine(ae.ToString());
            }
            catch (Exception ex)
            {
                Exception innerException = ex.InnerException;

                if (innerException != null)
                {

                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}
