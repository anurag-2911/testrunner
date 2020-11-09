using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace TestRunner
{
    public class OutputMessage
    {
        StringBuilder outMessage = new StringBuilder();
        private RichTextBox textBox;
        readonly string testResultFilePath = Environment.CurrentDirectory + "\\TestResult.txt";
        readonly string traceoutput = Environment.CurrentDirectory + "\\traceOutput.txt";

        public static OutputMessage Instance { get; } = new OutputMessage();
        private OutputMessage()
        {
            
            
        }

        public void SetTextBox(RichTextBox richTextBox)
        {
            this.textBox = richTextBox;
        }
        public void WriteMessage(string message)
        {
            outMessage.AppendLine(message);
        }

        public void CommitMessage()
        {
            try
            {
                if (File.Exists(testResultFilePath))
                {
                    string readMessageOutput = File.ReadAllText(testResultFilePath);
                    outMessage.AppendLine(readMessageOutput);
                }
            }
            catch (Exception)
            {

            }

            try
            {
                if(File.Exists(traceoutput))
                {
                    string readMessageOutput = File.ReadAllText(traceoutput);
                    outMessage.AppendLine(readMessageOutput);
                }
            }
            catch (Exception) { }
            WriteTextSafe(textBox, outMessage.ToString());
            
        }

        public void ResetMessage()
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
