using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace TestRunner
{
   public class AssemblyLoader
    {
        public Assembly LoadTestAssembly(string path)
        {
            Assembly testDll=null;
            try 
            {
                 testDll = Assembly.LoadFrom(path);
            }
            catch(Exception ex)
            {

            }
            return testDll;
        }
    }
}
