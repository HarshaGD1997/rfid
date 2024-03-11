using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace rfid
{
    public partial class home : Form
    {
        public home()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Please enter a value in the text field.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; 
            }
            Form1 form1 = new Form1(textBox1.Text);

   
            form1.Show();

            this.Hide();
        }


        private void home_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.ApplicationExitCall)
            {
                e.Cancel = true;
                this.Hide();
            }
        }
    }
}
