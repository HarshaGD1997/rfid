using System;
using System.Windows.Forms;
using System.IO.Ports;

namespace rfid
{
    public partial class Form1 : Form
    {
        SerialPort serialPort;

        public Form1()
        {
            InitializeComponent();
            serialPort = new SerialPort("COM4", 9600); // Change COM4 to the appropriate port
            serialPort.DataReceived += SerialPort_DataReceived;
            serialPort.Open();
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string data = serialPort.ReadLine();
            UpdateTextBox(data);
        }

        private void UpdateTextBox(string text)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(UpdateTextBox), text);
                return;
            }
            textBox1.Text = text;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            serialPort.Close();
        }
    }
}
