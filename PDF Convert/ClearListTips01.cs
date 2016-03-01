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
    public partial class ClearListTips01 : Form
    {
        [DllImport("user32")]
        public static extern int ReleaseCapture();//用鼠标拖动窗体,移动窗体
        [DllImport("user32")]
        public static extern int SendMessage(IntPtr hwnd, int msg, int wp, int lp);//用鼠标拖动窗体,移动窗体

        private string spath01 = string.Empty;

        public ClearListTips01()
        {
            InitializeComponent();

            //syncContext = SynchronizationContext.Current; //利用SynchronizationContext.Current在线程间同步上下文,允许一个线程和另外一个线程进行通讯，SynchronizationContext在通讯中充当传输者的角色。另外这里有个地方需要清楚的，不是每个线程都附加SynchronizationContext这个对象，只有UI线程是一直拥有的。
            //Control.CheckForIllegalCrossThreadCalls = false;//容许子线程随时更新ui.这也是它的死穴：在同一个test函数体内,不能保证自身事务的一致性.给label1付了值,一回头,就已经被别人改了,这和超市的踩踏事件的后果一样严重.

            this.pbConfirm.Location = new Point(38, 117);
            this.pbOver.Location = new Point(185, 117);
            this.pbClose.Location = new Point(290, 12);
            switch (Program.iniLanguage.ToLower())
            {
                case "zh-cn":
                    spath01 = Application.StartupPath + "\\zh-CN\\ClearListTips01\\";
                    this.Size = new Size(343, 173);//窗体界面固定大小(防止程序员不小心改变了大小,因为是多语言界面,窗体大小是固定的)
                    break;
                case "en":
                    spath01 = Application.StartupPath + "\\en\\ClearListTips01\\";
                    this.Size = new Size(343, 173);//窗体界面固定大小(防止程序员不小心改变了大小,因为是多语言界面,窗体大小是固定的)
                    break;
                case "ja":
                    spath01 = Application.StartupPath + "\\ja\\ClearListTips01\\";
                    this.Size = new Size(400, 173);//窗体界面固定大小(防止程序员不小心改变了大小,因为是多语言界面,窗体大小是固定的)
                    this.pbConfirm.Location = new Point(70, 117);
                    this.pbOver.Location = new Point(230, 117);
                    this.pbClose.Location = new Point(380, 5);
                    break;
                case "zh-tw":
                    spath01 = Application.StartupPath + "\\zh-TW\\ClearListTips01\\";
                    this.Size = new Size(343, 173);//窗体界面固定大小(防止程序员不小心改变了大小,因为是多语言界面,窗体大小是固定的)
                    break;
                default:
                    spath01 = Application.StartupPath + "\\zh-CN\\ClearListTips01\\";
                    break;
            }
        }

        private void ClearListTips01_KeyDown(object sender, KeyEventArgs e)
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

        private void ClearListTips01_Load(object sender, EventArgs e)
        {
            this.Refresh();
            this.Text = Program.appProgName;
            //this.Icon = new Icon(Image.FromFile(Application.StartupPath + "images\\logo.ico"));
            //switch (iniLanguage.ToLower())
            switch (Program.iniLanguage.ToLower())
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

        private void ClearListTips01_MouseDown(object sender, MouseEventArgs e)
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
                this.BackgroundImage = Image.FromFile(spath01 + "ClearListTips_01.png"); //窗体背景
            }
            else
            {
                this.BackgroundImage = Image.FromFile(spath01 + "ClearListTips_02.png"); //窗体背景
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
                this.BackgroundImage = Image.FromFile(spath01 + "ClearListTips_01.png");
            }
            else
            {
                this.BackgroundImage = Image.FromFile(spath01 + "ClearListTips_02.png");
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
                this.BackgroundImage = Image.FromFile(spath01 + "ClearListTips_01.png");
            }
            else
            {
                this.BackgroundImage = Image.FromFile(spath01 + "ClearListTips_02.png");
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
        /// 设置繁体中文
        /// </summary>
        private void SetZhTw()
        {
            this.pbConfirm.BackgroundImage = Image.FromFile(spath01 + "sure_01.png"); //确定
            this.pbOver.BackgroundImage = Image.FromFile(spath01 + "cancel_01.png"); //取消
            this.pbClose.BackgroundImage = Image.FromFile(Application.StartupPath + "\\images\\close_01.png"); //取消,窗体右上角关闭按钮
            if (!MainInfo01.isReg)
            {
                this.BackgroundImage = Image.FromFile(spath01 + "ClearListTips_01.png");
            }
            else
            {
                this.BackgroundImage = Image.FromFile(spath01 + "ClearListTips_02.png");
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

    }
}
