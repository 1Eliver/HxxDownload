﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HxxServerDownloader
{
    public partial class MessageForm : Form
    {
        public MessageForm(string msg)
        {
            InitializeComponent();
            label1.Text = msg;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
