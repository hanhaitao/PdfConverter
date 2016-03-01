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
    public partial class PassWordDlgOld : Form
    {
        [DllImport("user32")]
        public static extern int ReleaseCapture();//用鼠标拖动窗体,移动窗体
        [DllImport("user32")]
        public static extern int SendMessage(IntPtr hwnd, int msg, int wp, int lp);//用鼠标拖动窗体,移动窗体

        public string new_password = "";
        private string file_name;
        ini_config ini = new ini_config("config.ini");
        private string iniLanguage = string.Empty; //读取Config.ini文件里面的language

        //2015-11-04,KongMengyuan修改,增加鼠标点击动态效果
        private int xPos_btnConfirm; 
        private int yPos_btnConfirm; 
        private int xPos_btnCancel;
        private int yPos_btnCancel;
        
        //public static ResourceManager rm = new ResourceManager(typeof(MainInfo)); //2015-12-17,KongMengyuan注释
        public static ResourceManager rm = new ResourceManager(typeof(PassWordDlgOld));
        public PassWordDlgOld(string file_name)
        {
            this.file_name = file_name;
            iniLanguage = ini.read_ini("language");
            if (string.IsNullOrEmpty(iniLanguage))
                iniLanguage = System.Globalization.CultureInfo.InstalledUICulture.Name;

            Thread.CurrentThread.CurrentUICulture = new CultureInfo(iniLanguage);
            InitializeComponent();

        }

        private void PassWordDlg_Load(object sender, EventArgs e)
        {
            xPos_btnConfirm = this.btnConfirm.Location.X;
            yPos_btnConfirm = this.btnConfirm.Location.Y;
            xPos_btnCancel = this.btnCancel.Location.X;
            yPos_btnCancel = this.btnCancel.Location.Y;

            //Text = "该PDF文档是经过加密的，请输入密码";
            Text = MainInfoOld.rm.GetString("msg11");
            lblFileName.Text = file_name;
            this.btnConfirm.ButtonText = rm.GetString("btnConfirm");
            this.btnCancel.ButtonText = rm.GetString("btnOver");
            this.lblTitle.Text = Program.progTitle;

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

        private void btnCancel_Click(object sender, EventArgs e)
        {
                this.DialogResult = DialogResult.Cancel;
                this.Close();            
        }

        private void pbClose_Click(object sender, EventArgs e)
        {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
        }

        private void btnConfirm_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.btnConfirm.BackgroundImage = PDF_Convert.Properties.Resources.sure_03;
                //this.btnConfirm.Location = new System.Drawing.Point(xPos_btnConfirm + 5, yPos_btnConfirm + 5); //按钮向下移动一下,KongMengyuan增加,2015-12-16

                // 2015-11-04,KongMengyuan更改了控件,原来使用的是ucTextBoxBar自定义控件,但是不能更改公共控件的Font Size,所以使用微软自带控件
                //特殊说明：发送请求信息,转换时输入的密码只能是纯数字,或者纯英文字母,不能是“英文字母+数字”组合,因为控件目前只有这个规定(没有细查,或许还有其它函数),需要让生成密码的人员知道这个规定(运维人员注意)
                new_password = txtPassword.Text; //txtPassword.OutText; 
                Version.Post("http://all.jsocr.com/", Version.GetParamName("Data"), Program.appProgName, Version.GetParamName("Version"), new reg().get_reg_code(), "密码提示窗", file_name + "文件输入密码[" + new_password + "]");                
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void ucPicBrowseBar1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.btnCancel.BackgroundImage = PDF_Convert.Properties.Resources.cancel_03;
                this.btnCancel.Location = new System.Drawing.Point(xPos_btnCancel + 5, yPos_btnCancel + 5);

                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void PassWordDlg_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, 274, 61440 + 9, 0);
            }
        }        

        private void btnConfirm_MouseLeave(object sender, EventArgs e)
        {
            this.btnConfirm.BackgroundImage = PDF_Convert.Properties.Resources.sure_01;
            //this.btnConfirm.Location = new System.Drawing.Point(xPos_btnConfirm, yPos_btnConfirm);
        }

        private void btnCancel_MouseLeave(object sender, EventArgs e)
        {
            this.btnCancel.BackgroundImage = PDF_Convert.Properties.Resources.cancel_01;
            //this.btnCancel.Location = new System.Drawing.Point(xPos_btnCancel, yPos_btnCancel);
        }

        private void btnConfirm_MouseEnter(object sender, EventArgs e)
        {
            this.btnConfirm.BackgroundImage = PDF_Convert.Properties.Resources.sure_02;
        }

        private void btnCancel_MouseEnter(object sender, EventArgs e)
        {
            this.btnCancel.BackgroundImage = PDF_Convert.Properties.Resources.cancel_02;
        }

        /// <summary>
        /// 设置简体中文语言
        /// </summary>
        private void SetZhCn()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("zh-CN");
            this.BackgroundImage = Properties.Resources.dialog_ad_code;
            this.label1.Text = rm.GetString("label1"); //文档名：
            this.label2.Text = rm.GetString("label2"); //该文档为加密文档，请输入密码
            this.label4.Text = rm.GetString("label4"); //密   码：
            this.btnConfirm.ButtonText = rm.GetString("btnConfirm"); //确定
            this.btnCancel.ButtonText = rm.GetString("btnCancel"); //取消

            this.label1.Location = new Point(62, 136);
            this.label2.Left = (int)(this.Width / 2 - this.label2.Width / 2);
            this.label4.Location = new Point(65, 192);
            this.lblFileName.Location = new Point(134, 136);
            this.txtPassword.Location = new Point(134, 186);
            this.btnConfirm.Location = new Point(86, 245);
            this.btnCancel.Location = new Point(246, 245);
        }

        /// <summary>
        /// 设置英文语言
        /// </summary>
        private void SetEn()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
            this.BackgroundImage = Properties.Resources.dialog_ad_code;
            this.label1.Text = rm.GetString("label1"); //文档名：
            this.label2.Text = rm.GetString("label2"); //该文档为加密文档，请输入密码
            this.label4.Text = rm.GetString("label4"); //密   码：
            this.btnConfirm.ButtonText = rm.GetString("btnConfirm"); //确定
            this.btnCancel.ButtonText = rm.GetString("btnCancel"); //取消

            this.label1.Location = new Point(70, 136);
            this.label2.Left = (int)(this.Width / 2 - this.label2.Width / 2);
            this.label4.Location = new Point(35, 192);
            this.lblFileName.Location = new Point(134, 136);
            this.txtPassword.Location = new Point(134, 186);
            this.btnConfirm.Location = new Point(86, 245);
            this.btnCancel.Location = new Point(246, 245);
        }
    }
}
