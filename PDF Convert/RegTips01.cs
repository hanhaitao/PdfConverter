using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.Threading;
using System.Runtime.InteropServices; //运行Windows的API,引用DLL专用
using System.Diagnostics; //Process引用时

namespace PDF_Convert
{
    public partial class RegTips01 : Form
    {
        [DllImport("user32")]
        public static extern int ReleaseCapture();//用鼠标拖动窗体,移动窗体
        [DllImport("user32")]
        public static extern int SendMessage(IntPtr hwnd, int msg, int wp, int lp);//用鼠标拖动窗体,移动窗体

        private string iniLanguage = string.Empty; //读取Config.ini文件里面的language
        private string spath01 = string.Empty;
        ini_config ini = new ini_config("config.ini");

        public RegTips01()
        {
            //引入多语言
            iniLanguage = ini.read_ini("language");
            if (string.IsNullOrEmpty(iniLanguage))
                iniLanguage = System.Globalization.CultureInfo.InstalledUICulture.Name;

            InitializeComponent();

            this.Size = new Size(419, 240);//窗体界面固定大小(防止程序员不小心改变了大小,因为是多语言界面,窗体大小是固定的)

            switch (Program.iniLanguage.ToLower())
            {
                case "zh-cn":
                    spath01 = Application.StartupPath + "\\zh-CN\\RegTips01\\";
                    break;
                case "en":
                    spath01 = Application.StartupPath + "\\en\\RegTips01\\";
                    break;
                case "ja":
                    spath01 = Application.StartupPath + "\\Ja\\RegTips01\\";
                    break;
                case "zh-tw":
                    spath01 = Application.StartupPath + "\\zh-TW\\RegTips01\\";
                    break;
                default:
                    spath01 = Application.StartupPath + "\\zh-CN\\RegTips01\\";
                    break;
            }
        }

        private void RegTips01_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 27)//KongMengyuan增加,2015-11-11,Esc键27,如果按了回车键就把窗体关闭,按Esc键也把窗体关闭
            {
                this.pbConfirm.BackgroundImage = Image.FromFile(spath01 + "cancel_03.png");
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
            if (e.KeyValue == 13 || e.KeyValue == 32)//KongMengyuan增加,2015-11-11,回车键13空格键32,如果按了回车键就把窗体关闭,按Esc键也把窗体关闭
            {
                this.pbConfirm.BackgroundImage = Image.FromFile(spath01 + "sure_03.png");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void pbConfirm_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.pbConfirm.BackgroundImage = Image.FromFile(spath01 + "sure_03.png");
                try
                {
                    Process.Start("http://www.xjpdf.com/software/pdfConvert/buy/?version=" + Program.httpVersion + "&machine=" + Program.httpMachineCode);
                }
                catch
                {
                    Process.Start("iexplore.exe", "http://www.xjpdf.com/software/pdfConvert/buy/?version=" + Program.httpVersion + "&machine=" + Program.httpMachineCode);

                }
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
                this.pbOver.BackgroundImage = Image.FromFile(spath01 + "cancel_01.png");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void RegTips01_Load(object sender, EventArgs e)
        {
            this.Refresh();
            this.Text = Program.appProgName;
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

        private void RegTips01_MouseDown(object sender, MouseEventArgs e)
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
                this.BackgroundImage = Image.FromFile(spath01 + "RegTips_01.png"); //窗体背景
            }
            else
            {
                this.BackgroundImage = Image.FromFile(spath01 + "RegTips_02.png"); //窗体背景
            }

            this.Size = new System.Drawing.Size(419, 240);
            this.pbConfirm.Location = new Point(66, 188);
            this.pbConfirm.Size = new System.Drawing.Size(125, 36);
            this.pbOver.Location = new Point(234, 188);
            this.pbOver.Size = new System.Drawing.Size(125, 36);
            this.pbClose.Location = new Point(393, 12);

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
                this.BackgroundImage = Image.FromFile(spath01 + "RegTips_01.png"); //窗体背景
            }
            else
            {
                this.BackgroundImage = Image.FromFile(spath01 + "RegTips_02.png"); //窗体背景
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
                this.BackgroundImage = Image.FromFile(spath01 + "RegTips_01.png"); //窗体背景
            }
            else
            {
                this.BackgroundImage = Image.FromFile(spath01 + "RegTips_02.png"); //窗体背景
            }

            this.Size = new System.Drawing.Size(480, 240);
            this.pbConfirm.Location = new Point(50, 188);
            this.pbConfirm.Size = new System.Drawing.Size(145, 36);
            this.pbOver.Location = new Point(230, 188);
            this.pbOver.Size = new System.Drawing.Size(220, 36);
            this.pbClose.Location = new Point(450, 12);
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
                this.BackgroundImage = Image.FromFile(spath01 + "RegTips_01.png"); //窗体背景
            }
            else
            {
                this.BackgroundImage = Image.FromFile(spath01 + "RegTips_02.png"); //窗体背景
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

        private void pbClose_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

    }
}
