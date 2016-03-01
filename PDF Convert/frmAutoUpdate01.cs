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
    public partial class frmAutoUpdate01 : Form
    {
        [DllImport("user32")]
        public static extern int ReleaseCapture();//用鼠标拖动窗体,移动窗体
        [DllImport("user32")]
        public static extern int SendMessage(IntPtr hwnd, int msg, int wp, int lp);//用鼠标拖动窗体,移动窗体

        private string iniLanguage = string.Empty; //读取Config.ini文件里面的language
        private string spath01 = string.Empty;
        ini_config ini = new ini_config("config.ini");

        public bool cancelShow; //是否自动更新
        public string newVersion; //最新版本

        public frmAutoUpdate01()
        {
            //自动更新右上角不允许有“关闭”,只能是确定或者取消,这样如果php网站定义了新版本必须更新时,用户无论如何都得去更新了(就要这种效果)
            //此界面的调试方法：
            //    1、打开MainInfo01.cs里面的注释“Thread trd = new Thread(statisticsPost)”,同时需要把MainInfo01.cs里面的“Program.httpVersion = "6.3"”版本变低,这样就会自动到PHP网站查找更新了(这种方法比较麻烦) 
            //    2、需要在MainInfo01.cs窗体里面增加一个按钮,然后用下面的程序调试它:
            /*             
                     private void test_frmAutoUpdate01(object sender, EventArgs e)
                    {
                        //测试额外frmAutoUpdate01专用,需增加一个测试按钮
                        //private void button1_Click(object sender, EventArgs e)
                        //{
                        //    test_frmAutoUpdate01(sender, e);
                        //}
                        frmAutoUpdate01 frm = new frmAutoUpdate01();
                        //frm.TopMost = true; //异步的窗口ShowDialog是不会阻塞主窗口的，你只能在主窗口线程创建。使用这个属性就可以把窗体一直放在所有窗体的最前面,这样也可以起到这种效果
                        frm.StartPosition = FormStartPosition.CenterParent;//放置于父窗体中间位置
                        frm.Location = this.PointToScreen(new Point(400, this.lstFile.Location.Y + 30));
                        if (MessageBox.Show("调试1个按钮还是2个按钮: Yes-1,No-2", "测试frmAutoUpdate窗体是否显示正常", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                        {
                            frm.cancelShow = true; //是否显示“跳过”按钮,在调试时要注意把它修改为false进行调试
                        }
                        DialogResult dr = frm.ShowDialog(); //此处代码的目的是
                    }

                    private void button1_Click(object sender, EventArgs e)
                    {
                        test_frmAutoUpdate01(sender, e);
                    }
             */

            //引入多语言
            iniLanguage = ini.read_ini("language");
            if (string.IsNullOrEmpty(iniLanguage))
                iniLanguage = System.Globalization.CultureInfo.InstalledUICulture.Name;

            InitializeComponent();

            this.Size = new Size(340, 199);//窗体界面固定大小(防止程序员不小心改变了大小,因为是多语言界面,窗体大小是固定的)

            switch (Program.iniLanguage.ToLower())
            {
                case "zh-cn":
                    spath01 = Application.StartupPath + "\\zh-CN\\frmAutoUpdate01\\";
                    this.Size = new System.Drawing.Size(340, 199);
                    break;
                case "en":
                    spath01 = Application.StartupPath + "\\en\\frmAutoUpdate01\\";
                    break;
                case "ja":
                    spath01 = Application.StartupPath + "\\Ja\\frmAutoUpdate01\\";
                    this.Size = new System.Drawing.Size(420, 199);
                    this.pbConfirm.Location = new Point(87, 147);
                    this.pbOver.Location = new Point(234, 147);
                    break;
                case "zh-tw":
                    spath01 = Application.StartupPath + "\\zh-TW\\frmAutoUpdate01\\";
                    break;
                default:
                    spath01 = Application.StartupPath + "\\zh-CN\\frmAutoUpdate01\\";
                    break;
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

        private void frmAutoUpdate01_Load(object sender, EventArgs e)
        {
            this.Refresh();
            this.Text = Program.appProgName;
            //this.Icon = new Icon(Image.FromFile(Application.StartupPath + "images\\logo.ico"));

            if (cancelShow == false)
            {
                //强制更新,不显示取消按钮
                this.pbOver.Enabled = false;
                this.pbOver.Visible = false;
                this.pbConfirm.Left = (this.Width - this.pbConfirm.Width) / 2; //显示在窗体中间
                //this.lblPrompt2.Text = "请更新到最新版本" + newVersion;
                //this.lblPrompt2.Left = (this.Width - this.lblPrompt2.Width) / 2; //显示在窗体中间
                this.BackgroundImage = Image.FromFile(spath01 + "frmAutoUpdate02.png");
            }

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

        private void frmAutoUpdate01_MouseDown(object sender, MouseEventArgs e)
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
            if (!MainInfo01.isReg)
            {
                this.BackgroundImage = Image.FromFile(spath01 + "frmAutoUpdate01.png"); //窗体背景
            }
            else
            {
                this.BackgroundImage = Image.FromFile(spath01 + "frmAutoUpdate02.png"); //窗体背景
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
            if (!MainInfo01.isReg)
            {
                this.BackgroundImage = Image.FromFile(spath01 + "frmAutoUpdate01.png"); //窗体背景
            }
            else
            {
                this.BackgroundImage = Image.FromFile(spath01 + "frmAutoUpdate02.png"); //窗体背景
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
            if (!MainInfo01.isReg)
            {
                this.BackgroundImage = Image.FromFile(spath01 + "frmAutoUpdate01.png"); //窗体背景
            }
            else
            {
                this.BackgroundImage = Image.FromFile(spath01 + "frmAutoUpdate02.png"); //窗体背景
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
            if (!MainInfo01.isReg)
            {
                this.BackgroundImage = Image.FromFile(spath01 + "frmAutoUpdate01.png"); //窗体背景
            }
            else
            {
                this.BackgroundImage = Image.FromFile(spath01 + "frmAutoUpdate02.png"); //窗体背景
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
            this.pbOver.BackgroundImage = Image.FromFile(spath01 + "cancel_01.png"); //确定
        }

        private void pbOver_MouseLeave(object sender, EventArgs e)
        {
            this.pbOver.BackgroundImage = Image.FromFile(spath01 + "cancel_01.png"); //确定
        }
    }
}
