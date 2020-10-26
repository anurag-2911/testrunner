using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace TestRunner
{
    public class OutputMessage
    {
        StringBuilder outMessage = new StringBuilder();

        readonly string testResultFilePath = Environment.CurrentDirectory + "\\TestResult.txt";

        public static OutputMessage Instance { get; } = new OutputMessage();
        private OutputMessage()
        {

        }
        public void WriteMessage(string message)
        {
            outMessage.AppendLine(message);
        }

        public void CommitMessage(RichTextBox textBox)
        {
            if (File.Exists(testResultFilePath))
            {
                string readMessageOutput = File.ReadAllText(testResultFilePath);
                outMessage.AppendLine(readMessageOutput);
            }
            WriteTextSafe(textBox, outMessage.ToString());
            
        }

        public void ResetMessage(RichTextBox textBox)
        {
            WriteTextSafe(textBox, string.Empty);
            
            outMessage.Clear();
        }
        private delegate void SafeCallDelegate(RichTextBox textBox,string text);
        private void WriteTextSafe(RichTextBox textbox,string text)
        {
            if (textbox.InvokeRequired)
            {
                var d = new SafeCallDelegate(WriteTextSafe);
                textbox.Invoke(d, new object[] { textbox,text });
            }
            else
            {
                textbox.Text = text;
            }
        }
    }
}
