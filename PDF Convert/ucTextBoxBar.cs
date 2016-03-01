using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace PDF_Convert
{
    [DefaultEvent("Click")]
    public partial class ucTextBoxBar : UserControl
    {
        //KongMengyuan增加封装,应该把密码封装出去
        public char PasswordChar
        {
            get { return this.txtOutPath.PasswordChar; }
            set { this.txtOutPath.PasswordChar =System.Convert.ToChar(value); }
        }

        public string OutText
        {
            get { return this.txtOutPath.Text; }
            set { this.txtOutPath.Text = value; }
        }

        public bool IsReadOnly
        {
            get { return this.txtOutPath.ReadOnly; }
            set { this.txtOutPath.ReadOnly = value; }
        }

        public Image BackImg
        {
            get { return this.BackgroundImage; }
            set { this.BackgroundImage = value; }
        }

        public ucTextBoxBar()
        {
            InitializeComponent();
        }
        
    }
}
