using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace PDF_Convert
{
    public partial class ucPicLabel : UserControl
    {
        public ucPicLabel()
        {
            InitializeComponent();
        }

        public Image LabImg
        {
            get { return img.Image; }
            set { img.Image = value; }
        }

        public string LabContext
        {
            get { return txtIn.Text; }
            set { txtIn.Text = value; }
        }

        public Color TxtColor
        {
            get { return txtIn.ForeColor; }
            set { txtIn.ForeColor = value; }
        }
    }
}
