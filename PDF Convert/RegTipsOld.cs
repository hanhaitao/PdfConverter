using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.InteropServices; //运行Windows的API,引用DLL专用
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace PDF_Convert
{
    public partial class RegTipsOld : Form
    {
        //KongMengyuan增加,郑总提出来：在点击右上角的“购买”时打开页面会卡一下,而点击右下角的"在线教程"就不卡,改变写法(提取机器码浪费了多余的时间)
        private string httpVersion = "";
        private string httpMachineCode = "";
        private string httpRegCode = "";

        [DllImport("user32")]
        public static extern int ReleaseCapture();//用鼠标拖动窗体,移动窗体
        [DllImport("user32")]
        public static extern int SendMessage(IntPtr hwnd, int msg, int wp, int lp);//用鼠标拖动窗体,移动窗体

        ini_config ini = new ini_config("config.ini");
        private string iniLanguage = string.Empty; //读取Config.ini文件里面的language
        //public static ResourceManager rm = new ResourceManager(typeof(MainInfo)); //2015-12-17,KongMengyuan注释
        public static ResourceManager rm = new ResourceManager(typeof(RegTipsOld));
        public RegTipsOld()
        {
            //引入多语言
            iniLanguage = ini.read_ini("language");
            if (string.IsNullOrEmpty(iniLanguage))
                iniLanguage = System.Globalization.CultureInfo.InstalledUICulture.Name;
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(iniLanguage);
            InitializeComponent();

        }
        private void ClearListTips_Load(object sender, EventArgs e)
        {
            //KongMengyuan增加,郑总提出来：在点击右上角的“购买”时打开页面会卡一下,而点击右下角的"在线教程"就不卡,改变写法(提取机器码浪费了多余的时间)
            httpVersion = Program.httpVersion;
            httpMachineCode = Program.httpMachineCode;
            httpRegCode = Program.httpRegCode;
            this.lblTitle.Text = Program.progTitle;

            //this.btnConfirm.ButtonText = rm.GetString("Buy");
            //this.btnOver.ButtonText = rm.GetString("RegActive");
            switch (iniLanguage.ToLower())
            {
                case "zh-cn":
                    SetZhCn();
                    break;
                case "en":
                    SetEn();
                    break;
                default:
                    SetZhCn();
                    break;
            }

            this.Text = Program.appProgName; //窗体的快捷方式显示,任务栏的显示内容
        }
        private void pbClose_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.pbClose.BackgroundImage = PDF_Convert.Properties.Resources.close_03;
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void btnConfirm_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.btnConfirm.BackgroundImage=Properties.Resources.RegTips_btn_dialog_buy_03;
                //Process.Start("http://www.xjpdf.com/software/pdfConvert/buy/?version=" + Version.version + "&machine=" + new reg().get_machine_code());
                try
                {
                    //KongMengyuan修改,郑总提出来：在点击右上角的“购买”时打开页面会卡一下,而点击右下角的"在线教程"就不卡,改变写法(提取机器码浪费了多余的时间)
                    //Process.Start("http://www.xjpdf.com/software/pdfConvert/buy/?version=" + Version.version + "&machine=" + new reg().get_machine_code());
                    Process.Start("http://www.xjpdf.com/software/pdfConvert/buy/?version=" + httpVersion + "&machine=" + httpMachineCode);
                }
                catch
                {
                    //KongMengyuan修改,郑总提出来：在点击右上角的“购买”时打开页面会卡一下,而点击右下角的"在线教程"就不卡,改变写法(提取机器码浪费了多余的时间)
                    //Process.Start("iexplore.exe", "http://www.xjpdf.com/software/pdfConvert/buy/?version=" + Version.version + "&machine=" + new reg().get_machine_code());
                    Process.Start("iexplore.exe", "http://www.xjpdf.com/software/pdfConvert/buy/?version=" + httpVersion + "&machine=" + httpMachineCode);

                }
                //this.DialogResult = DialogResult.Cancel;
                //this.Close();

                //KongMengyuan修改,郑总提出来：在点击右上角的“购买”时打开页面会卡一下,而点击右下角的"在线教程"就不卡,改变写法(提取机器码浪费了多余的时间)
                //发送请求信息 
                //Version.Post("http://all.jsocr.com/", Version.GetParamName("Data"), Program.appProgName, Version.GetParamName("Version"), new reg().get_reg_code(), "免费试用版提示窗口",  "购买正式版");
            }
        }

        private void btnOver_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.btnConfirm.BackgroundImage = Properties.Resources.RegTips_btn_dialog_reg_03;
                //KongMengyuan修改,郑总提出来：在点击右上角的“购买”时打开页面会卡一下,而点击右下角的"在线教程"就不卡,改变写法(提取机器码浪费了多余的时间)
                //发送请求信息 
                //Version.Post("http://all.jsocr.com/", Version.GetParamName("Data"), Program.appProgName, Version.GetParamName("Version"), new reg().get_reg_code(), "免费试用版提示窗口", "注册并激活");

                this.DialogResult = DialogResult.OK;
                this.Close();

            }
        }

        private void ClearListTips_Paint(object sender, PaintEventArgs e)
        {
            //ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle,
            //           Color.FromArgb(45, 164, 244), 2, ButtonBorderStyle.Solid,
            //           Color.FromArgb(45, 164, 244), 2, ButtonBorderStyle.Solid,
            //           Color.FromArgb(45, 164, 244), 2, ButtonBorderStyle.Solid,
            //           Color.FromArgb(45, 164, 244), 2, ButtonBorderStyle.Solid);
            //DrawBroder画不上底部边框 用DrawLine补充上
            //e.Graphics.DrawLine(new Pen(Color.FromArgb(45, 164, 244), 3), 0, panel1.Height - 2, panel1.Width, panel1.Height - 2);

        }


        private void RegTips_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, 274, 61440 + 9, 0);
            }
        }

        private void btnConfirm_MouseEnter(object sender, EventArgs e)
        {
            this.btnConfirm.BackgroundImage = Properties.Resources.RegTips_btn_dialog_buy_02;
        }

        private void btnConfirm_MouseLeave(object sender, EventArgs e)
        {
            this.btnConfirm.BackgroundImage = Properties.Resources.RegTips_btn_dialog_buy_01;
        }

        private void btnOver_MouseEnter(object sender, EventArgs e)
        {
            this.btnOver.BackgroundImage = Properties.Resources.RegTips_btn_dialog_reg_02;
        }

        private void btnOver_MouseLeave(object sender, EventArgs e)
        {
            this.btnOver.BackgroundImage = Properties.Resources.RegTips_btn_dialog_reg_01;
        }

        private void pbClose_MouseEnter(object sender, EventArgs e)
        {
            this.pbClose.BackgroundImage = Properties.Resources.close_02;
        }

        private void pbClose_MouseLeave(object sender, EventArgs e)
        {
            this.pbClose.BackgroundImage = Properties.Resources.close_01;
        }

        /// <summary>
        /// 设置简体中文语言
        /// </summary>
        private void SetZhCn()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("zh-CN");
            this.BackgroundImage = Properties.Resources.RegTips_bg;
            this.label1.Text = rm.GetString("label1"); //免费试用版仅支持文档前5页的转换
            this.label2.Text = rm.GetString("label2"); //免费试用版仅支持文档前5页的转换
            this.btnConfirm.ButtonText = rm.GetString("btnConfirm"); //购买正式版
            this.btnOver.ButtonText = rm.GetString("btnOver"); //注册并激活

            this.label1.Left = (int)(this.Width / 2 - this.label1.Width / 2);
            this.label2.Left = (int)(this.Width / 2 - this.label2.Width / 2);
            //this.label1.Location = new Point(59, 76);
            //this.label2.Location = new Point(84, 137);
            this.btnConfirm.Location = new Point(55, 183);
            this.btnOver.Location = new Point(231, 183);
        }

        /// <summary>
        /// 设置英文语言
        /// </summary>
        private void SetEn()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
            this.BackgroundImage = Properties.Resources.RegTips_bg;
            this.label1.Text = rm.GetString("label1"); //免费试用版仅支持文档前5页的转换
            this.label2.Text = rm.GetString("label2"); //免费试用版仅支持文档前5页的转换
            this.btnConfirm.ButtonText = rm.GetString("btnConfirm"); //购买正式版
            this.btnOver.ButtonText = rm.GetString("btnOver"); //注册并激活

            this.label1.Left = (int)(this.Width / 2 - this.label1.Width / 2);
            this.label2.Left = (int)(this.Width / 2 - this.label2.Width / 2);
            //this.label1.Location = new Point(59, 76);
            //this.label2.Location = new Point(84, 137);
            this.btnConfirm.Location = new Point(55, 183);
            this.btnOver.Location = new Point(231, 183);
        }

    }
}
