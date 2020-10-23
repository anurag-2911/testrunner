using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TestRunner
{
    class TestClassHandler
    {
        private Dictionary<string, Type> testClasses = new Dictionary<string, Type>();
        public void PopulateTestClasses(Assembly testAssembly)
        {
            Type[] testclass = testAssembly.GetTypes();

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
        }

        public Dictionary<string,Type> GetTestClasses
        {
            get
            {
                return testClasses;
            }
        }
    }
}
