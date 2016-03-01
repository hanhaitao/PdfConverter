using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.Threading;
using System.Runtime.InteropServices; //运行Windows的API,引用DLL专用

namespace PDF_Convert
{
    public partial class PassWordDlg01 : Form
    {
        [DllImport("user32")]
        public static extern int ReleaseCapture();//用鼠标拖动窗体,移动窗体
        [DllImport("user32")]
        public static extern int SendMessage(IntPtr hwnd, int msg, int wp, int lp);//用鼠标拖动窗体,移动窗体

        public string new_password = string.Empty;
        private string file_name = string.Empty;

        private string iniLanguage = string.Empty; //读取Config.ini文件里面的language
        private string spath01 = string.Empty;
        ini_config ini = new ini_config("config.ini");

        public PassWordDlg01(string file_name)
        {
            this.file_name = file_name;
            //引入多语言
            iniLanguage = ini.read_ini("language");
            if (string.IsNullOrEmpty(iniLanguage))
                iniLanguage = System.Globalization.CultureInfo.InstalledUICulture.Name;

            InitializeComponent();

            this.Size = new Size(420, 300);//窗体界面固定大小(防止程序员不小心改变了大小,因为是多语言界面,窗体大小是固定的)

            switch (Program.iniLanguage.ToLower())
            {
                case "zh-cn":
                    spath01 = Application.StartupPath + "\\zh-CN\\PassWordDlg01\\";
                    break;
                case "en":
                    spath01 = Application.StartupPath + "\\en\\PassWordDlg01\\";
                    break;
                case "ja":
                    spath01 = Application.StartupPath + "\\Ja\\PassWordDlg01\\";
                    break;
                case "zh-tw":
                    spath01 = Application.StartupPath + "\\zh-TW\\PassWordDlg01\\";
                    break;
                default:
                    spath01 = Application.StartupPath + "\\zh-CN\\PassWordDlg01\\";
                    break;
            }
        }
        private void pbConfirm_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.pbConfirm.BackgroundImage = Image.FromFile(spath01 + "sure_03.png");
                new_password = txtPassword.Text.Trim();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void pbConfirm_MouseEnter(object sender, EventArgs e)
        {
            if (((Control)sender).Enabled != false)
            {
                this.pbConfirm.BackgroundImage = Image.FromFile(spath01 + "sure_02.png");
            }
        }

        private void pbConfirm_MouseLeave(object sender, EventArgs e)
        {
            if (((Control)sender).Enabled != false)
            {
                this.pbConfirm.BackgroundImage = Image.FromFile(spath01 + "sure_01.png");
            }
        }

        private void pbOver_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.pbOver.BackgroundImage = Image.FromFile(spath01 + "cancel_03.png");
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void PassWordDlg01_Load(object sender, EventArgs e)
        {
            this.Refresh();
            this.Text = Program.appProgName;
            lblFileName.Text = file_name;
            //this.Icon = new Icon(Image.FromFile(Application.StartupPath + "images\\logo.ico"));
            switch (iniLanguage.ToLower())
            {
                case "zh-cn":
                    SetZhCn();
                    break;
                case "en":
                    SetEn();
                    break;
                case "ja":
                    SetJa();
                    break;
                case "zh-tw":
                    SetZhTw();
                    break;
                default:
                    SetZhCn();
                    break;
            }
        }

        private void PassWordDlg01_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, 274, 61440 + 9, 0);
            }
        }

        /// <summary>
        /// 设置简体中文语言
        /// </summary>
        private void SetZhCn()
        {
            this.pbConfirm.BackgroundImage = Image.FromFile(spath01 + "sure_01.png"); //确定
            this.pbOver.BackgroundImage = Image.FromFile(spath01 + "cancel_01.png"); //取消
            this.pbClose.BackgroundImage = Image.FromFile(Application.StartupPath + "\\images\\close_01.png"); //取消,窗体右上角关闭按钮
            if (!MainInfo01.isReg)
            {
                this.BackgroundImage = Image.FromFile(spath01 + "PassWordDlg_01.png"); //窗体背景
            }
            else
            {
                this.BackgroundImage = Image.FromFile(spath01 + "PassWordDlg_02.png"); //窗体背景
            }

            //Thread.CurrentThread.CurrentUICulture = new CultureInfo("zh-CN");
            //this.BackgroundImage = Properties.Resources.dialog_clear;
            //this.label2.Text = rm.GetString("label2.Text"); //您确定要清除列表中所有项目吗？
            //this.btnConfirm.ButtonText = rm.GetString("btnConfirm.ButtonText"); //确定            

            //this.label2.Left = (int)(this.Width / 2 - this.label2.Width / 2);
            //this.label2.Location = new Point(46, 75);
            //this.btnConfirm.Location = new Point(46, 117);
            //this.btnOver.Location = new Point(193, 117);
        }

        /// <summary>
        /// 设置英文语言
        /// </summary>
        private void SetEn()
        {
            this.pbConfirm.BackgroundImage = Image.FromFile(spath01 + "sure_01.png"); //确定
            this.pbOver.BackgroundImage = Image.FromFile(spath01 + "cancel_01.png"); //取消
            this.pbClose.BackgroundImage = Image.FromFile(Application.StartupPath + "\\images\\close_01.png"); //取消,窗体右上角关闭按钮
            if (!MainInfo01.isReg)
            {
                this.BackgroundImage = Image.FromFile(spath01 + "PassWordDlg_01.png"); //窗体背景
            }
            else
            {
                this.BackgroundImage = Image.FromFile(spath01 + "PassWordDlg_02.png"); //窗体背景
            }
            //Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
            //this.BackgroundImage = Properties.Resources.dialog_clear;
            //this.label2.Text = rm.GetString("label2.Text"); //您确定要清除列表中所有项目吗？
            //this.btnConfirm.ButtonText = rm.GetString("btnConfirm.ButtonText"); //确定
            //this.btnOver.ButtonText = rm.GetString("btnOver.ButtonText"); //取消

            //this.label2.Left = (int)(this.Width / 2 - this.label2.Width / 2);
            //this.label2.Location = new Point(46, 75);
            //this.btnConfirm.Location = new Point(46, 117);
            //this.btnOver.Location = new Point(193, 117);
        }

        /// <summary>
        /// 设置日文语言
        /// </summary>
        private void SetJa()
        {
            this.pbConfirm.BackgroundImage = Image.FromFile(spath01 + "sure_01.png"); //确定
            this.pbOver.BackgroundImage = Image.FromFile(spath01 + "cancel_01.png"); //取消
            this.pbClose.BackgroundImage = Image.FromFile(Application.StartupPath + "\\images\\close_01.png"); //取消,窗体右上角关闭按钮
            if (!MainInfo01.isReg)
            {
                this.BackgroundImage = Image.FromFile(spath01 + "PassWordDlg_01.png"); //窗体背景
            }
            else
            {
                this.BackgroundImage = Image.FromFile(spath01 + "PassWordDlg_02.png"); //窗体背景
            }
            //Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
            //this.BackgroundImage = Properties.Resources.dialog_clear;
            //this.label2.Text = rm.GetString("label2.Text"); //您确定要清除列表中所有项目吗？
            //this.btnConfirm.ButtonText = rm.GetString("btnConfirm.ButtonText"); //确定
            //this.btnOver.ButtonText = rm.GetString("btnOver.ButtonText"); //取消

            //this.label2.Left = (int)(this.Width / 2 - this.label2.Width / 2);
            //this.label2.Location = new Point(46, 75);
            //this.btnConfirm.Location = new Point(46, 117);
            //this.btnOver.Location = new Point(193, 117);
        }

        /// <summary>
        /// 设置繁体中文
        /// </summary>
        private void SetZhTw()
        {
            this.pbConfirm.BackgroundImage = Image.FromFile(spath01 + "sure_01.png"); //确定
            this.pbOver.BackgroundImage = Image.FromFile(spath01 + "cancel_01.png"); //取消
            this.pbClose.BackgroundImage = Image.FromFile(Application.StartupPath + "\\images\\close_01.png"); //取消,窗体右上角关闭按钮
            if (!MainInfo01.isReg)
            {
                this.BackgroundImage = Image.FromFile(spath01 + "PassWordDlg_01.png"); //窗体背景
            }
            else
            {
                this.BackgroundImage = Image.FromFile(spath01 + "PassWordDlg_02.png"); //窗体背景
            }
            //Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
            //this.BackgroundImage = Properties.Resources.dialog_clear;
            //this.label2.Text = rm.GetString("label2.Text"); //您确定要清除列表中所有项目吗？
            //this.btnConfirm.ButtonText = rm.GetString("btnConfirm.ButtonText"); //确定
            //this.btnOver.ButtonText = rm.GetString("btnOver.ButtonText"); //取消

            //this.label2.Left = (int)(this.Width / 2 - this.label2.Width / 2);
            //this.label2.Location = new Point(46, 75);
            //this.btnConfirm.Location = new Point(46, 117);
            //this.btnOver.Location = new Point(193, 117);
        }

        private void pbOver_MouseEnter(object sender, EventArgs e)
        {
            this.pbOver.BackgroundImage = Image.FromFile(spath01 + "cancel_02.png"); //取消
        }

        private void pbClose_MouseEnter(object sender, EventArgs e)
        {
            this.pbClose.BackgroundImage = Image.FromFile(Application.StartupPath + "\\images\\close_02.png"); //取消,窗体右上角关闭按钮
        }

        private void pbClose_MouseLeave(object sender, EventArgs e)
        {
            this.pbClose.BackgroundImage = Image.FromFile(Application.StartupPath + "\\images\\close_01.png"); //取消,窗体右上角关闭按钮
        }

        private void pbOver_MouseLeave(object sender, EventArgs e)
        {
            this.pbOver.BackgroundImage = Image.FromFile(spath01 + "cancel_01.png"); //取消
        }

        //显示当前坐标位置,在编程时定位控件坐标位置使用,正式使用之前将其注释掉
        private void cursorPosition()
        {
            lblCursorPosition.Visible = true;
            Point pt = this.PointToClient(Control.MousePosition); //将指定屏幕点的位置计算成工作区坐标
            string sPos = pt.X.ToString() + "," + pt.Y.ToString();
            lblCursorPosition.Text = sPos;
            //如果不需要光标跟随显示,就直接把下边两行注释掉(如果位置在最下边或最右边,可能会显示在屏幕外面)
            lblCursorPosition.Left = pt.X + 10;
            lblCursorPosition.Top = pt.Y + 20;
        }

        private void PassWordDlg01_MouseMove(object sender, MouseEventArgs e)
        {
            //cursorPosition(); //显示当前坐标位置,在编程时定位控件坐标位置使用,正式使用之前将其注释掉
        }
    }
}
