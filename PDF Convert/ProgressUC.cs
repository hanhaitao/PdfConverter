using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace PDF_Convert
{
    public partial class ProgressUC : UserControl
    {
        public ProgressUC()
        {
            InitializeComponent();
        }

        private string progressStr;

        public string ProgressStr
        {
            get { return labProgress.Text; }
            set { labProgress.Text = value; }
        }
    }
}
