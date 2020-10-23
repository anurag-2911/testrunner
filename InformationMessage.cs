using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestRunner
{
    public class InformationMessage
    {
        private InformationMessage()
        {

        }
        public static InformationMessage Instance { get; } = new InformationMessage();

        public string HowToUseMessage()
        {
            StringBuilder howToUseMessage = new StringBuilder();

            howToUseMessage.AppendLine("1. Attach the test dll ");
            howToUseMessage.AppendLine("2. All dependent dll which test dll needs should be in same folder as test dll");
            howToUseMessage.AppendLine("3. Select test class");
            howToUseMessage.AppendLine("4. Run test(s)");

            return howToUseMessage.ToString();
        }

        public string ContactMessage()
        {
            StringBuilder contacts = new StringBuilder();
            contacts.Append("anurag.kumar@microfocus.com");
            return contacts.ToString();
        }
    }
}
