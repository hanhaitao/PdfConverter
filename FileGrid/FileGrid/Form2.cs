using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FileGrid
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            FolderBrowserDialog o = new FolderBrowserDialog();
            o.ShowDialog();
            this.Close();
        }
    }
}
