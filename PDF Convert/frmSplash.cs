using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.IO;
using System.Threading;//调用线程必须引用它

using System.Management;//项目下边的 引用--添加引用--框架--System.Management
using System.Drawing.Imaging;    

namespace PDF_Convert
{
    public partial class frmSplash : Form
    {
        //线程管理当前文件集合
        public Dictionary<string, int> dicThreadManagement = new Dictionary<string, int>();
        private string encodingCode = string.Empty;

        //private Thread[] thread;
        private int MainCount = 0; //解决主窗体同时打开多个的问题,郑侃炜提出的方法,比较实用,2015-11-12

        //private string iniLanguage = string.Empty; //读取Config.ini文件里面的language

        //ini_config ini = new ini_config("config.ini");

        public frmSplash()
        {
            ////引入多语言
            //iniLanguage = ini.read_ini("language");
            //if (string.IsNullOrEmpty(iniLanguage))
            //    iniLanguage = System.Globalization.CultureInfo.InstalledUICulture.Name;

            InitializeComponent();

            this.Size = new Size(484, 293);//窗体界面固定大小(防止程序员不小心改变了大小,因为是多语言界面,窗体大小是固定的)

            this.Text = Program.appProgName; //窗体的快捷方式显示,任务栏的显示内容
        }
        [STAThread]

        private void Animator()
        {
            MainCount++;
            if (MainCount > 1)
            {
                Console.WriteLine("查看当前的时间，跟踪为什么程序启动比较慢1： " + DateTime.Now.ToString());
                Console.WriteLine("正在尝试创建窗口，拦截");
                return;
            }
            //MainInfoOld frm = new MainInfoOld();
            MainInfo01 frm = new MainInfo01();
            // C# 判断窗体是否已经打开, 避免重复打开同一个窗体
            if (frm == null || frm.IsDisposed)
            {
                Console.WriteLine("查看当前的时间，跟踪为什么程序启动比较慢2： " + DateTime.Now.ToString());
                //frm = new MainInfoOld();
                frm = new MainInfo01();
                frm.Show();//如果之前未打开，则打开。
                Console.WriteLine("查看当前的时间，跟踪为什么程序启动比较慢3： " + DateTime.Now.ToString());                
                frm.Visible = true;
            }
            else
            {
                frm.Show();//如果之前未打开，则打开。                
                frm.Activate();//之前已打开，则给予焦点，置顶。
                frm.Visible = true;
            }

            pTimer_Elapsed(null, null); //当主窗体加载完成则移除本Form
            return;
        }

        private void frmSplash_Activated(object sender, EventArgs e)
        {
            pictureBox1.Visible = false;

            Thread tThread = new Thread(raceQuick);
            tThread.Start(); //开启了线程后,由于MainInfo窗体是在这个线程里面开启的,所以ListViewPlus.cs里面的OnHandleCreated的第一行“base.OnHandleCreated(e);”就会出错
            Animator(); //不启动线程gif图片会不动,会变成静态图片
            //Console.WriteLine("查看当前的时间，跟踪为什么程序启动比较慢7： " + DateTime.Now.ToString());
        }

        private void raceQuick()
        {
            //全部移到MainInfo01.cs的onlyRunOnce()里面了
        }

        private void frmSplash_Load(object sender, EventArgs e)
        {
            //自定义时间控件,避免加载Timer控件
            System.Timers.Timer pTimer = new System.Timers.Timer(8000);//每隔8秒执行一次，没用winfrom自带的
            pTimer.Elapsed += pTimer_Elapsed;//委托，要执行的方法
            pTimer.AutoReset = true;//获取该定时器自动执行
            pTimer.Enabled = true;//这个一定要写，要不然定时器不会执行的
            Control.CheckForIllegalCrossThreadCalls = false;//这个不太懂，有待研究

            switch (Program.iniLanguage.ToLower())
            {
                case "zh-cn":
                    this.BackgroundImage = Image.FromFile(Application.StartupPath + "\\zh-CN\\frmSplash\\" + "frmSplash.png");
                    break;
                case "en":
                    this.BackgroundImage = Image.FromFile(Application.StartupPath + "\\en\\frmSplash\\" + "frmSplash.png");
                    break;
                case "ja":
                    this.BackgroundImage = Image.FromFile(Application.StartupPath + "\\Ja\\frmSplash\\" + "frmSplash.png");
                    break;
                case "zh-tw":
                    this.BackgroundImage = Image.FromFile(Application.StartupPath + "\\zh-TW\\frmSplash\\" + "frmSplash.png");
                    break;
                default:
                    this.BackgroundImage = Image.FromFile(Application.StartupPath + "\\zh-CN\\frmSplash\\" + "frmSplash.png");
                    break;
            }
        }

        private void pTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //当主窗体加载完成则移除本Form
            while (Program.MainInfo_LoadFinish == "Finish")
            {
                //this.Visible = false;
                this.Hide(); //隐藏窗体1,入口是不能关的,除非窗体2是独立在项目外的
                break;
            }
        }

    }
}
