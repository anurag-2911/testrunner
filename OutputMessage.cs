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

        public void CommitMessage(TextBox textBox)
        {
            if (File.Exists(testResultFilePath))
            {
                string readMessageOutput = File.ReadAllText(testResultFilePath);
                outMessage.AppendLine(readMessageOutput);
            }

            textBox.Text = outMessage.ToString();
        }

        public void ResetMessage(TextBox textBox)
        {
            textBox.Text = string.Empty;
            outMessage.Clear();
        }
    }
}
