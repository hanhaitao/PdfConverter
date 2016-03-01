using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.InteropServices; //运行Windows的API,引用DLL专用
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace PDF_Convert
{
    public partial class frmAutoUpdateOld : Form
    {
        [DllImport("user32")]
        public static extern int ReleaseCapture();//用鼠标拖动窗体,移动窗体
        [DllImport("user32")]
        public static extern int SendMessage(IntPtr hwnd, int msg, int wp, int lp);//用鼠标拖动窗体,移动窗体

        ini_config ini = new ini_config("config.ini");
        private string iniLanguage = string.Empty; //读取Config.ini文件里面的language
        //public static ResourceManager rm = new ResourceManager(typeof(MainInfo)); //2015-12-17,KongMengyuan注释
        public static ResourceManager rm = new ResourceManager(typeof(frmAutoUpdateOld));
        public bool cancelShow; //是否自动更新
        public string newVersion; //最新版本
        public frmAutoUpdateOld()
        {
            iniLanguage = ini.read_ini("language");
            if (string.IsNullOrEmpty(iniLanguage))
                iniLanguage = System.Globalization.CultureInfo.InstalledUICulture.Name;
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(iniLanguage);
            InitializeComponent();            
        }
        private void ClearListTips_Load(object sender, EventArgs e)
        {
            this.Refresh();
            this.btnConfirm.ButtonText = rm.GetString("btnConfirm");
            this.btnOver.ButtonText = rm.GetString("btnOver");
            this.lblTitle.Text = Program.progTitle;
            if (cancelShow == false)
            {
                //强制更新,不显示取消按钮
                this.btnOver.Enabled = false;
                this.btnOver.Visible = false;
                this.btnConfirm.Left = (this.Width - this.btnConfirm.Width) / 2; //显示在窗体中间
                this.lblPrompt2.Text = "请更新到最新版本" + newVersion;
                this.lblPrompt2.Left = (this.Width - this.lblPrompt2.Width) / 2; //显示在窗体中间
            }

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
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void btnConfirm_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.btnConfirm.BackgroundImage = PDF_Convert.Properties.Resources.sure_03;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void btnOver_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.btnOver.BackgroundImage = PDF_Convert.Properties.Resources.cancel_03;
                this.DialogResult = DialogResult.Cancel;
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
            ////DrawBroder画不上底部边框 用DrawLine补充上
            //e.Graphics.DrawLine(new Pen(Color.FromArgb(45, 164, 244), 3), 0, panel1.Height - 2, panel1.Width, panel1.Height - 2);
        }

        private void ClearListTips_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, 274, 61440 + 9, 0);
            }
        }

        private void ClearListTips_Activated(object sender, EventArgs e)
        {
            btnConfirm.Focus();//KongMengyuan增加,2015-11-11,窗体打开后把默认焦点放在确认上面
        }

        private void btnConfirm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)//KongMengyuan增加,2015-11-11,如果按了回车键就把窗体关闭
            {
                this.btnConfirm.BackgroundImage = PDF_Convert.Properties.Resources.sure_03;
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void btnConfirm_MouseEnter(object sender, EventArgs e)
        {
            this.btnConfirm.BackgroundImage = PDF_Convert.Properties.Resources.sure_02;
        }

        private void btnConfirm_MouseLeave(object sender, EventArgs e)
        {
            this.btnConfirm.BackgroundImage = PDF_Convert.Properties.Resources.sure_01;
        }

        private void btnOver_MouseLeave(object sender, EventArgs e)
        {
            this.btnOver.BackgroundImage = PDF_Convert.Properties.Resources.cancel_01;
        }

        private void btnOver_MouseEnter(object sender, EventArgs e)
        {
            this.btnOver.BackgroundImage = PDF_Convert.Properties.Resources.cancel_02;
        }

        /// <summary>
        /// 设置简体中文语言
        /// </summary>
        private void SetZhCn()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("zh-CN");
            this.BackgroundImage = Properties.Resources.dialog_clear;
            this.lblPrompt1.Text = rm.GetString("lblPrompt1"); //最新版本的安装包已经下载完成
            this.lblPrompt2.Text = rm.GetString("lblPrompt2"); //是否退出程序立刻更新？
            this.btnConfirm.ButtonText = rm.GetString("btnConfirm"); //确定
            this.btnOver.ButtonText = rm.GetString("btnOver"); //取消

            this.lblPrompt1.Left = (int)(this.Width / 2 - this.lblPrompt1.Width / 2);
            this.lblPrompt2.Left = (int)(this.Width / 2 - this.lblPrompt2.Width / 2);
            //this.lblPrompt1.Location = new Point(54, 59);
            //this.lblPrompt2.Location = new Point(71, 86);
            //this.btnConfirm.Location = new Point(48, 124); //这两个按钮不能设置Location,因为它依据网站index.php返回的参数不同,此按钮的位置也是不同的
            //this.btnOver.Location = new Point(195, 124);
        }

        /// <summary>
        /// 设置英文语言
        /// </summary>
        private void SetEn()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
            this.BackgroundImage = Properties.Resources.dialog_clear;
            this.lblPrompt1.Text = rm.GetString("lblPrompt1"); //最新版本的安装包已经下载完成
            this.lblPrompt2.Text = rm.GetString("lblPrompt2"); //是否退出程序立刻更新？
            this.btnConfirm.ButtonText = rm.GetString("btnConfirm"); //确定
            this.btnOver.ButtonText = rm.GetString("btnOver"); //取消

            this.lblPrompt1.Left = (int)(this.Width / 2 - this.lblPrompt1.Width / 2);
            this.lblPrompt2.Left = (int)(this.Width / 2 - this.lblPrompt2.Width / 2);
            //this.lblPrompt1.Location = new Point(54, 59);
            //this.lblPrompt2.Location = new Point(71, 86);
            //this.btnConfirm.Location = new Point(48, 124); //这两个按钮不能设置Location,因为它依据网站index.php返回的参数不同,此按钮的位置也是不同的
            //this.btnOver.Location = new Point(195, 124);
        }
    }
}
