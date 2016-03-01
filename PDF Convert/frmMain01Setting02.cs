using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.Threading;
using System.Runtime.InteropServices; //运行Windows的API,引用DLL专用
using System.Text.RegularExpressions; //Regex调用

namespace PDF_Convert
{
    public partial class frmMain01Setting02 : Form
    {
        //pdf分割,专用界面

        [DllImport("user32")]
        public static extern int ReleaseCapture();//用鼠标拖动窗体,移动窗体
        [DllImport("user32")]
        public static extern int SendMessage(IntPtr hwnd, int msg, int wp, int lp);//用鼠标拖动窗体,移动窗体

        private string spath01 = string.Empty;
        private int radioSelect; //当前选择的是哪一个RadioButton按钮
        public int pageCount; //当前右键鼠标的总页数

        public frmMain01Setting02()
        {
            InitializeComponent();

            //放一个文本框目的是在初次显示窗体时把鼠标放在这里，否则tbPageSelect1会不显示提示信息
            rtbTemp.Left = this.Width + 10;
            rtbTemp.Top = 0;

            string sOld = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_03"); //请输入页码, 如 1-5;7,9;15-20,25
            this.tbPageSelect1.BackGroundText = sOld;

            this.Size = new Size(369, 245);//窗体界面固定大小(防止程序员不小心改变了大小,因为是多语言界面,窗体大小是固定的)

            //syncContext = SynchronizationContext.Current; //利用SynchronizationContext.Current在线程间同步上下文,允许一个线程和另外一个线程进行通讯，SynchronizationContext在通讯中充当传输者的角色。另外这里有个地方需要清楚的，不是每个线程都附加SynchronizationContext这个对象，只有UI线程是一直拥有的。
            //Control.CheckForIllegalCrossThreadCalls = false;//容许子线程随时更新ui.这也是它的死穴：在同一个test函数体内,不能保证自身事务的一致性.给label1付了值,一回头,就已经被别人改了,这和超市的踩踏事件的后果一样严重.

            tbPageSelect1.MaxLength = 32767; //最多可以输入32767位长度的数字(够用了)
            radioSelect = 1; // 默认RadioButton显示第1个
            switch (Program.iniLanguage.ToLower())
            {
                case "zh-cn":
                    spath01 = Application.StartupPath + "\\zh-CN\\frmMain01Setting02\\";
                    break;
                case "en":
                    spath01 = Application.StartupPath + "\\en\\frmMain01Setting02\\";
                    break;
                case "ja":
                    spath01 = Application.StartupPath + "\\ja\\frmMain01Setting02\\";
                    break;
                case "zh-tw":
                    spath01 = Application.StartupPath + "\\zh-TW\\frmMain01Setting02\\";
                    break;
                default:
                    spath01 = Application.StartupPath + "\\zh-CN\\frmMain01Setting02\\";
                    break;
            }
        }

        private void frmMain01Setting02_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 27)//KongMengyuan增加,2015-11-11,Esc键27,如果按了回车键就把窗体关闭,按Esc键也把窗体关闭
            {
                this.Tag = ""; //取消时把它置空,这样页面选择就设置为"全部"
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }

            if (e.KeyValue == 13)//KongMengyuan增加,2015-11-11,回车键13空格键32,如果按了回车键就把窗体关闭,按Esc键也把窗体关闭
            {
                outputResult();
            }
        }

        private void pbConfirm_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                outputResult();
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

        private void pbClose_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.Tag = ""; //取消时把它置空,这样页面选择就设置为"全部"
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void pbClose_MouseEnter(object sender, EventArgs e)
        {
            this.pbClose.BackgroundImage = Image.FromFile(Application.StartupPath + "\\images\\close_02.png"); //取消,窗体右上角关闭按钮
        }

        private void pbClose_MouseLeave(object sender, EventArgs e)
        {
            this.pbClose.BackgroundImage = Image.FromFile(Application.StartupPath + "\\images\\close_01.png"); //取消,窗体右上角关闭按钮
        }

        private void frmMain01Setting02_Load(object sender, EventArgs e)
        {
            this.Refresh();
            this.Text = Program.appProgName;

            tbPageSelect1.Location = new Point((int)((this.Width - tbPageSelect1.Width) / 2), 196); //通过cursorPosition取得位置坐标
            pbConfirm.Location = new Point((int)((this.Width - pbConfirm.Width) / 2), 240); //通过cursorPosition取得位置坐标

            string sOld = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_03"); //请输入页码, 如 1-5;7,9;15-20,25
            this.toolTip1.SetToolTip(this.tbPageSelect1, sOld);//增加TextBox的提示
            tbPageSelect1.TextAlign = HorizontalAlignment.Left;

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

        /// <summary>
        /// 设置简体中文语言.不同语言可以使用不同的控件定位(因为翻译长度不同)
        /// </summary>
        private void SetZhCn()
        {
            this.pbConfirm.BackgroundImage = Image.FromFile(spath01 + "sure_01.png");
            this.pbClose.BackgroundImage = Image.FromFile(Application.StartupPath + "\\images\\close_01.png"); //取消,窗体右上角关闭按钮
            this.pbAll.BackgroundImage = Image.FromFile(spath01 + "menu_2_1.png");

            this.pbConfirm.Location = new Point((int)((this.Width - pbConfirm.Width) / 2), 185);
            this.tbPageSelect1.Location = new Point((int)((this.Width - tbPageSelect1.Width) / 2), 105);
        }

        /// <summary>
        /// 设置英文语言.不同语言可以使用不同的控件定位(因为翻译长度不同)
        /// </summary>
        private void SetEn()
        {
            this.pbConfirm.BackgroundImage = Image.FromFile(spath01 + "sure_01.png"); //确定
            this.pbClose.BackgroundImage = Image.FromFile(Application.StartupPath + "\\images\\close_01.png"); //取消,窗体右上角关闭按钮
            this.pbAll.BackgroundImage = Image.FromFile(spath01 + "menu_2_1.png");
        }

        /// <summary>
        /// 设置日文语言.不同语言可以使用不同的控件定位(因为翻译长度不同)
        /// </summary>
        private void SetJa()
        {
            this.pbConfirm.BackgroundImage = Image.FromFile(spath01 + "sure_01.png"); //确定
            this.pbClose.BackgroundImage = Image.FromFile(Application.StartupPath + "\\images\\close_01.png"); //取消,窗体右上角关闭按钮
            this.pbAll.BackgroundImage = Image.FromFile(spath01 + "menu_2_1.png");
        }

        /// <summary>
        /// 设置繁体中文.不同语言可以使用不同的控件定位(因为翻译长度不同)
        /// </summary>
        private void SetZhTw()
        {
            this.pbConfirm.BackgroundImage = Image.FromFile(spath01 + "sure_01.png"); //确定
            this.pbClose.BackgroundImage = Image.FromFile(Application.StartupPath + "\\images\\close_01.png"); //取消,窗体右上角关闭按钮
            this.pbAll.BackgroundImage = Image.FromFile(spath01 + "menu_2_1.png");
        }

        private void pbAll_MouseDown(object sender, MouseEventArgs e)
        {
            //MouseDown和MouseClick两者不可同时共存,先发生前者,一般存在前者时后者就不触发了,切记
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, 274, 61440 + 9, 0);

                int locationSpecify_X_1 = 70; //指定页面,X1的位置
                int locationSpecify_X_2 = 177; //指定页面,X2的位置
                int locationSpecify_Y_1 = 54; //指定页面,Y1的位置
                int locationSpecify_Y_2 = 68; //指定页面,Y2的位置
                int locationAll_X_1 = 195; //所有页面,X1的位置
                int locationAll_X_2 = 281; //所有页面,X2的位置
                int locationAll_Y_1 = 48; //所有页面,Y1的位置
                int locationAll_Y_2 = 70; //所有页面,Y2的位置
                switch (Program.iniLanguage.ToLower())
                {
                    case "zh-cn":
                        break;
                    case "en":
                        locationSpecify_X_1 = 84; //指定页面,X1的位置
                        locationSpecify_X_2 = 195; //指定页面,X2的位置
                        locationSpecify_Y_1 = 48; //指定页面,Y1的位置
                        locationSpecify_Y_2 = 70; //指定页面,Y2的位置
                        locationAll_X_1 = 207; //所有页面,X1的位置
                        locationAll_X_2 = 292; //所有页面,X2的位置
                        locationAll_Y_1 = 48; //所有页面,Y1的位置
                        locationAll_Y_2 = 70; //所有页面,Y2的位置

                        tbPageSelect1.Location = new Point((int)((this.Width - tbPageSelect1.Width) / 2), 103); //通过cursorPosition取得位置坐标
                        pbConfirm.Location = new Point((int)((this.Width - pbConfirm.Width) / 2), 182); //通过cursorPosition取得位置坐标

                        //tbPageSelect1.Location = new Point(38, 103);
                        //pbConfirm.Location = new Point(139, 182);
                        break;
                    case "ja":
                        break;
                    case "zh-tw":
                        break;
                    default:
                        break;
                }

                if (e.Location.X >= locationSpecify_X_1 && e.Location.X < locationSpecify_X_2 && e.Location.Y > locationSpecify_Y_1 && e.Location.Y < locationSpecify_Y_2) // 指定页面
                {
                    radioSelect = 1;
                    this.pbAll.BackgroundImage = Image.FromFile(spath01 + "menu_2_1.png");
                    tbPageSelect1.Visible = true;
                    string sOld = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_03"); //请输入页码, 如 1-5;7,9;15-20,25
                    this.toolTip1.SetToolTip(this.tbPageSelect1, sOld);//增加TextBox的提示
                    tbPageSelect1.TextAlign = HorizontalAlignment.Left;
                    //tbPageSelect1.Focus();
                    rtbTemp.Focus();
                }
                else if (e.Location.X >= locationAll_X_1 && e.Location.X < locationAll_X_2 && e.Location.Y > locationAll_Y_1 && e.Location.Y < locationAll_Y_2) // 所有页面
                {
                    radioSelect = 2;
                    this.pbAll.BackgroundImage = Image.FromFile(spath01 + "menu_2_2.png");
                    tbPageSelect1.Text = "";
                    rtbTemp.Focus();
                    tbPageSelect1.Visible = false;
                }
            }
        }

        private void pbAll_MouseMove(object sender, MouseEventArgs e)
        {
            //cursorPosition(); //显示当前坐标位置,在编程时定位控件坐标位置使用,正式使用之前将其注释掉

            int locationSpecify_X_1 = 70; //指定页面,X1的位置
            int locationSpecify_X_2 = 177; //指定页面,X2的位置
            int locationSpecify_Y_1 = 54; //指定页面,Y1的位置
            int locationSpecify_Y_2 = 68; //指定页面,Y2的位置
            int locationAll_X_1 = 195; //所有页面,X1的位置
            int locationAll_X_2 = 281; //所有页面,X2的位置
            int locationAll_Y_1 = 48; //所有页面,Y1的位置
            int locationAll_Y_2 = 70; //所有页面,Y2的位置
            tbPageSelect1.Location = new Point((int)((this.Width - tbPageSelect1.Width) / 2), 103); //通过cursorPosition取得位置坐标
            pbConfirm.Location = new Point((int)((this.Width - pbConfirm.Width) / 2), 182); //通过cursorPosition取得位置坐标
            switch (Program.iniLanguage.ToLower())
            {
                case "zh-cn":
                    break;
                case "en":
                    locationSpecify_X_1 = 84; //指定页面,X1的位置
                    locationSpecify_X_2 = 195; //指定页面,X2的位置
                    locationSpecify_Y_1 = 48; //指定页面,Y1的位置
                    locationSpecify_Y_2 = 70; //指定页面,Y2的位置
                    locationAll_X_1 = 207; //所有页面,X1的位置
                    locationAll_X_2 = 292; //所有页面,X2的位置
                    locationAll_Y_1 = 48; //所有页面,Y1的位置
                    locationAll_Y_2 = 70; //所有页面,Y2的位置                    
                    break;
                case "ja":
                    break;
                case "zh-tw":
                    break;
                default:
                    break;
            }

            //鼠标移动到不同的区域显示不同光标,通过cursorPosition取得位置坐标
            if (e.Location.X >= locationSpecify_X_1 && e.Location.X < locationSpecify_X_2 && e.Location.Y > locationSpecify_Y_1 && e.Location.Y < locationSpecify_Y_2) // 指定页面
            {
                this.Cursor = System.Windows.Forms.Cursors.Hand;
            }
            else if (e.Location.X >= locationAll_X_1 && e.Location.X < locationAll_X_2 && e.Location.Y > locationAll_Y_1 && e.Location.Y < locationAll_Y_2) // 所有页面
            {
                this.Cursor = System.Windows.Forms.Cursors.Hand;
            }
            else
            {
                this.Cursor = System.Windows.Forms.Cursors.Default;
            }
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

        private void tbPageSelect1_KeyDown(object sender, KeyEventArgs e)
        {
            if (radioSelect != 1)
            {
                radioSelect = 1;
                this.pbAll.BackgroundImage = Image.FromFile(spath01 + "menu_2_1.png");
            }
            allKeyPress(e);
        }

        private void tbPageSelect1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                radioSelect = 1;
                this.pbAll.BackgroundImage = Image.FromFile(spath01 + "menu_2_1.png");
            }
        }

        private void allKeyPress(KeyEventArgs e)
        {
            if (e.KeyValue == 27)//KongMengyuan增加,2015-11-11,Esc键27,如果按了回车键就把窗体关闭,按Esc键也把窗体关闭
            {
                this.Tag = ""; //取消时把它置空,这样页面选择就设置为"全部"
                this.AccessibleName = "";
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }

            if (e.KeyValue == 13)//KongMengyuan增加,2015-11-11,回车键13空格键32,如果按了回车键就把窗体关闭,按Esc键也把窗体关闭
            {
                outputResult();
            }
        }

        private void outputResult()
        {
            this.pbConfirm.BackgroundImage = Image.FromFile(spath01 + "sure_03.png");

            if (radioSelect == 1)
            {
                this.Tag = tbPageSelect1.Text;
                string tag = this.Tag.ToString();
            }
            else
            {
                this.Tag = "";
                this.DialogResult = DialogResult.OK; //必须放在最后,否则无论如何此窗体都会关闭的
                this.Close();
                return;
            }
            if (this.Tag.ToString().Trim() == ""
                || (Convert01.VerifyNumber01(this.Tag.ToString(), true) == false && this.Tag.ToString().Length > 0)
               )
            {
                this.Tag = "";

                string sTip = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_01"); //提示
                string sOld = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_04"); //输入的页码不符合条件,是否直接退出
                DialogResult dr = MessageBox.Show(sOld, sTip, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dr.ToString() != "Yes") //DialogResult.Cancel
                {
                    if (radioSelect == 1)
                    {
                        //tbPageSelect1.Focus();
                        rtbTemp.Focus();
                    }
                    return;
                }
            }
            else
            {
                //int pageTmp = System.Convert.ToInt32(str);
                if (radioSelect == 1)
                {
                    if (Convert01.VerifyNumber01(this.Tag.ToString(), true) == false || verifyValidate(this.Tag.ToString()) == false) //校验数字是否正确
                    {
                        //tbPageSelect1.Focus();
                        rtbTemp.Focus();
                        return;
                    }
                    else
                    {
                        string s = tbPageSelect1.Text;
                        s = Convert01.strMergeStr1(s);
                        this.Tag = Convert01.strRemoveTrim(s, false); //去除杂乱符号
                    }
                }
                else
                {
                    this.Tag = "";
                }
            }
            this.DialogResult = DialogResult.OK; //必须放在最后,否则无论如何此窗体都会关闭的
            this.Close();
        }

        #region 校验是否超过最大值,同时是否从小到大排列
        /// <summary>
        /// 2016-01-27,KongMengyuan
        /// 校验是否超过最大值,同时是否从小到大排列
        /// 测试样例:
        /// 1,5,7
        /// 1,7,5
        /// </summary>
        /// <param name="enterNumber">输入的组合字符串</param>
        /// <returns>是否正确</returns>
        private bool verifyValidate(string enterNumber)
        {
            int inputMaxCount = 0;
            //KongMengyuan,2016-01-08,微软的Select的Bug,快速点击切换时,可能会把上条选中的数字切换过来,如果慢速就没有这个问题了(这个和当初产品经理夏勇设计的鼠标移到ListView上面显示全称,有时会因为移动快而定位不准,是一个道理)
            int pageCount = this.pageCount;

            if (!MainInfo01.isReg) //免费版只转换前5页
            {
                if (pageCount >= Program.pageFreeConvert)
                {
                    pageCount = Program.pageFreeConvert;
                }
            }

            //利用正则表达式将字符串中的数字提取出来分别保存在整型数组中,提取字符串中的数字
            string s = enterNumber;  //测试数据"1 2234 34";

            s = Convert01.strRemoveTrim(s, false); //去除字符串前后两边的多余符号和空格"  ；;,，-?*  "

            MatchCollection vMatchs = Regex.Matches(s, @"(\d+)"); //需要引用 using System.Text.RegularExpressions; //Regex调用
            int[] vInts = new int[vMatchs.Count];
            for (int i = 0; i < vMatchs.Count; i++)
            {
                if (vMatchs[i].Value.Length > "99999".Length)
                {
                    string sTip = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_01"); //提示
                    string sOld = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_07"); //所输入的数字 $S 大于99999,目前不支持这么大的数字
                    sOld = Program.strReplace(sOld, "$S", new string[] { vMatchs[i].ToString() }); //替换Language.ini字符串里面的通用代替符号
                    MessageBox.Show(sOld + "\r\n" + "\r\n" + this.AccessibleName, sTip, MessageBoxButtons.OK, MessageBoxIcon.Error);  // 字符之间加入回车符
                    return false;
                }
                vInts[i] = int.Parse(vMatchs[i].Value);
            }
            int uBound = vInts.GetUpperBound(0); //取得数组的最大上标

            string errString = string.Empty;

            for (int i = 0; i <= uBound; i++)
            {
                inputMaxCount = vInts[i];
                //Console.WriteLine(vInts[i]);
                if (inputMaxCount > pageCount)
                {
                    errString += ", " + inputMaxCount.ToString();
                }

                //所输入的数字必须是从小到大排列的
                try
                {
                    if (System.Convert.ToInt32(vInts[i + 1]) < System.Convert.ToInt32(vInts[i]))
                    {
                        string sTip = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_01"); //提示
                        string sOld = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_06"); //所输入的数字必须是从小到大排列的，请重新输入！                            
                        MessageBox.Show(sOld + "\r\n" + "\r\n" + this.AccessibleName, sTip, MessageBoxButtons.OK, MessageBoxIcon.Error);  // 字符之间加入回车符
                        return false;
                    }
                }
                catch
                { }
            }
            if (errString != string.Empty)
            {
                //去掉字符串左边的逗号和空格
                errString = errString.Substring(2, errString.Length - 2);
                string sTip = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_01"); //提示
                string sOld = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_05"); //输入的页码 $S 已经超过可以转换的最大页数 $S ,请检查输入的页码范围
                sOld = Program.strReplace(sOld, "$S", new string[] { errString, pageCount.ToString() }); //替换Language.ini字符串里面的通用代替符号
                MessageBox.Show(sOld + "\r\n" + "\r\n" + this.AccessibleName, sTip, MessageBoxButtons.OK, MessageBoxIcon.Error);  // 字符之间加入回车符
                return false;

            }
            return true;
        }
        #endregion
    }
}
