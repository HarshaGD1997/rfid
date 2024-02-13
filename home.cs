﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
                return; // Stop the execution of the method
            }
            Form1 form1 = new Form1(textBox1.Text);

            // Show Form1
            form1.Show();

            // Optionally, you can hide the current form
            this.Hide();
        }
    }
}