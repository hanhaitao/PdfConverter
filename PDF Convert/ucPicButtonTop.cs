using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace PDF_Convert
{
    public partial class ucPicButtonTop : UserControl
    {
        public ucPicButtonTop()
        {
            InitializeComponent();
            BottomLine = true;
        }

        public string ButtonText
        {
            get { return lblText.Text; }
            set { lblText.Text = value; }
        }

        public Font ButtonTextFont
        {
            get { return lblText.Font; }
            set { lblText.Font = value; }
        }

        public bool BottomLine
        {
            get;
            set;
        }

        private Image btnSelected;

        public Image BtnSelected
        {
            get { return btnSelected; }
            set { btnSelected = value; }
        }

        private Image btnNoSelected;

        public Image BtnNoSelected
        {
            get { return btnNoSelected; }
            set { btnNoSelected = value; }
        }

        Pen bottomPen = new Pen(Color.FromArgb(0xfe, 0xfe, 0xff)); //0xe0, 0xe0, 0xe0

        private void ucPicButtonTop_Load(object sender, EventArgs e)
        {
            this.BackgroundImage = null;
            //BackColor = Color.FromArgb(0xdf, 0xdf, 0xdf); //#DFDFDF, 0xf0, 0xf0, 0xf0
            BackColor = Color.Transparent;
        }
    }
}
