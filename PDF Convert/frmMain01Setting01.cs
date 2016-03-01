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
using System.Collections; //ArrayList引用

namespace PDF_Convert
{
    public partial class frmMain01Setting01 : Form
    {
        //pdf分割,专用界面

        [DllImport("user32")]
        public static extern int ReleaseCapture();//用鼠标拖动窗体,移动窗体
        [DllImport("user32")]
        public static extern int SendMessage(IntPtr hwnd, int msg, int wp, int lp);//用鼠标拖动窗体,移动窗体

        private string spath01 = string.Empty;
        public int pageCount; //当前右键鼠标的总页数

        public frmMain01Setting01()
        {
            InitializeComponent();

            //放一个文本框目的是在初次显示窗体时把鼠标放在这里，否则tbPageSelect1会不显示提示信息
            rtbTemp.Left = this.Width + 10;
            rtbTemp.Top = 0;

            string sOld = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_03"); //请输入页码, 如 1-5;7,9;15-20,25
            this.tbPageSelect2.BackGroundText = sOld;

            this.Size = new Size(369, 292);//窗体界面固定大小(防止程序员不小心改变了大小,因为是多语言界面,窗体大小是固定的)

            Convert01.g_splitsettings.customizePages = new ArrayList();

            //syncContext = SynchronizationContext.Current; //利用SynchronizationContext.Current在线程间同步上下文,允许一个线程和另外一个线程进行通讯，SynchronizationContext在通讯中充当传输者的角色。另外这里有个地方需要清楚的，不是每个线程都附加SynchronizationContext这个对象，只有UI线程是一直拥有的。
            //Control.CheckForIllegalCrossThreadCalls = false;//容许子线程随时更新ui.这也是它的死穴：在同一个test函数体内,不能保证自身事务的一致性.给label1付了值,一回头,就已经被别人改了,这和超市的踩踏事件的后果一样严重.

            tbPageSelect1.MaxLength = 5; //最多可以输入5位长度数字99999(够用了)
            Convert01.g_splitsettings.split_page_mode = Convert01.tsplitpagemode.spEveryPage; // 默认"文件分割"鼠标右键的RadioButton默认为第1个
            this.pbConfirm.Location = new Point((int)((this.Width - pbConfirm.Width) / 2), 242); //通过cursorPosition取得位置坐标
            switch (Program.iniLanguage.ToLower())
            {
                case "zh-cn":
                    spath01 = Application.StartupPath + "\\zh-CN\\frmMain01Setting01\\";
                    break;
                case "en":
                    spath01 = Application.StartupPath + "\\en\\frmMain01Setting01\\";
                    break;
                case "ja":
                    spath01 = Application.StartupPath + "\\ja\\frmMain01Setting01\\";
                    break;
                case "zh-tw":
                    spath01 = Application.StartupPath + "\\zh-TW\\frmMain01Setting01\\";
                    break;
                default:
                    spath01 = Application.StartupPath + "\\zh-CN\\frmMain01Setting01\\";
                    break;
            }
        }

        private void frmMain01Setting01_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 27)//KongMengyuan增加,2015-11-11,Esc键27,如果按了回车键就把窗体关闭,按Esc键也把窗体关闭
            {
                this.Tag = ""; //取消时把它置空,这样页面选择就设置为"全部"
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }

            if (e.KeyValue == 13)//KongMengyuan增加,2015-11-11,回车键13空格键32,如果按了回车键就把窗体关闭,按Esc键也把窗体关闭
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
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

        private void frmMain01Setting01_Load(object sender, EventArgs e)
        {
            this.Refresh();
            this.Text = Program.appProgName;

            tbPageSelect1.Location = new Point(180, 140); //通过cursorPosition取得位置坐标
            tbPageSelect2.Location = new Point((int)((this.Width - tbPageSelect2.Width) / 2), 196); //通过cursorPosition取得位置坐标            

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
            this.pbAll.BackgroundImage = Image.FromFile(spath01 + "menu_1_1.png");

            this.tbPageSelect1.Location = new Point(174, 140);
            this.tbPageSelect2.Location = new Point((int)((this.Width - tbPageSelect2.Width) / 2), 199);
        }

        /// <summary>
        /// 设置英文语言.不同语言可以使用不同的控件定位(因为翻译长度不同)
        /// </summary>
        private void SetEn()
        {
            this.pbConfirm.BackgroundImage = Image.FromFile(spath01 + "sure_01.png"); //确定
            this.pbClose.BackgroundImage = Image.FromFile(Application.StartupPath + "\\images\\close_01.png"); //取消,窗体右上角关闭按钮
            this.pbAll.BackgroundImage = Image.FromFile(spath01 + "menu_1_1.png"); //取消

            this.tbPageSelect1.Location = new Point(185, 140);
        }

        /// <summary>
        /// 设置日文语言.不同语言可以使用不同的控件定位(因为翻译长度不同)
        /// </summary>
        private void SetJa()
        {
            this.pbConfirm.BackgroundImage = Image.FromFile(spath01 + "sure_01.png"); //确定
            this.pbClose.BackgroundImage = Image.FromFile(Application.StartupPath + "\\images\\close_01.png"); //取消,窗体右上角关闭按钮
            this.pbAll.BackgroundImage = Image.FromFile(spath01 + "menu_1_1.png");
            this.tbPageSelect1.Location = new Point(125, 140);
        }

        /// <summary>
        /// 设置繁体中文.不同语言可以使用不同的控件定位(因为翻译长度不同)
        /// </summary>
        private void SetZhTw()
        {
            this.pbConfirm.BackgroundImage = Image.FromFile(spath01 + "sure_01.png"); //确定
            this.pbClose.BackgroundImage = Image.FromFile(Application.StartupPath + "\\images\\close_01.png"); //取消,窗体右上角关闭按钮
            this.pbAll.BackgroundImage = Image.FromFile(spath01 + "menu_1_1.png");
        }

        private void pbAll_MouseDown(object sender, MouseEventArgs e)
        {
            //MouseDown和MouseClick两者不可同时共存,先发生前者,一般存在前者时后者就不触发了,切记
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, 274, 61440 + 9, 0);

                if (e.Location.X >= 100 && e.Location.X < 285 && e.Location.Y > 30 && e.Location.Y < 46) // 分割每一页
                {
                    Convert01.g_splitsettings.split_page_mode = Convert01.tsplitpagemode.spEveryPage;
                    this.pbAll.BackgroundImage = Image.FromFile(spath01 + "menu_1_1.png");
                    tbPageSelect2.Visible = false;
                }
                else if (e.Location.X >= 100 && e.Location.X < 285 && e.Location.Y > 58 && e.Location.Y < 77) // 分割奇数页
                {
                    Convert01.g_splitsettings.split_page_mode = Convert01.tsplitpagemode.spOddPage;
                    this.pbAll.BackgroundImage = Image.FromFile(spath01 + "menu_1_2.png");
                    tbPageSelect2.Visible = false;
                }
                else if (e.Location.X >= 100 && e.Location.X < 285 && e.Location.Y > 86 && e.Location.Y < 105) // 分割偶数页
                {
                    Convert01.g_splitsettings.split_page_mode = Convert01.tsplitpagemode.spEvenPage;
                    this.pbAll.BackgroundImage = Image.FromFile(spath01 + "menu_1_3.png");
                    tbPageSelect2.Visible = false;
                }
                else if (e.Location.X >= 100 && e.Location.X < 285 && e.Location.Y > 113 && e.Location.Y < 132) // 指定页面分割
                {
                    Convert01.g_splitsettings.split_page_mode = Convert01.tsplitpagemode.spCustomize;
                    this.pbAll.BackgroundImage = Image.FromFile(spath01 + "menu_1_4.png");
                    tbPageSelect2.Visible = true;
                    string sOld = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_03"); //请输入页码, 如 1-5;7,9;15-20,25
                    this.toolTip1.SetToolTip(this.tbPageSelect2, sOld);//增加TextBox的提示
                    tbPageSelect2.TextAlign = HorizontalAlignment.Left;
                    tbPageSelect2.Text = "";
                    rtbTemp.Focus();
                    //tbPageSelect2.Focus();
                }
                else if (e.Location.X >= 100 && e.Location.X < 285 && e.Location.Y > 139 && e.Location.Y < 161) // 每***分割
                {
                    Convert01.g_splitsettings.split_page_mode = Convert01.tsplitpagemode.spSpecifyInterval;
                    this.pbAll.BackgroundImage = Image.FromFile(spath01 + "menu_1_5.png");
                    tbPageSelect2.Visible = false;
                }
            }
        }

        private void pbAll_MouseMove(object sender, MouseEventArgs e)
        {

            //cursorPosition(); //显示当前坐标位置,在编程时定位控件坐标位置使用,正式使用之前将其注释掉

            //鼠标移动到不同的区域显示不同光标,通过cursorPosition取得位置坐标
            if (e.Location.X >= 100 && e.Location.X < 285 && e.Location.Y > 30 && e.Location.Y < 46) // 分割每一页
            {
                this.Cursor = System.Windows.Forms.Cursors.Hand;
            }
            else if (e.Location.X >= 100 && e.Location.X < 285 && e.Location.Y > 58 && e.Location.Y < 77) // 分割奇数页
            {
                this.Cursor = System.Windows.Forms.Cursors.Hand;
            }
            else if (e.Location.X >= 100 && e.Location.X < 285 && e.Location.Y > 86 && e.Location.Y < 105) // 分割偶数页
            {
                this.Cursor = System.Windows.Forms.Cursors.Hand;
            }
            else if (e.Location.X >= 100 && e.Location.X < 285 && e.Location.Y > 113 && e.Location.Y < 132) // 指定页面分割
            {
                this.Cursor = System.Windows.Forms.Cursors.Hand;
            }
            else if (e.Location.X >= 100 && e.Location.X < 285 && e.Location.Y > 139 && e.Location.Y < 161) // 每***分割
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

        private void tbPageSelect1_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                //只能输入数字，一般的做法就是在按键事件中处理，判断keychar的值。限制只能输入数字，小数点，Backspace，del这几个键。数字0~9所对应的keychar为48~57，小数点是46，Backspace是8
                int kc = (int)e.KeyChar;
                if ((kc < 48 || kc > 57) && kc != 8)
                {
                    e.Handled = true;
                }
                else
                {
                    //只要在此处输入数字,就把这个RadioButton点击上
                    if (Convert01.g_splitsettings.split_page_mode != Convert01.tsplitpagemode.spSpecifyInterval)
                    {
                        Convert01.g_splitsettings.split_page_mode = Convert01.tsplitpagemode.spSpecifyInterval;
                        this.pbAll.BackgroundImage = Image.FromFile(spath01 + "menu_1_5.png");
                        tbPageSelect2.Visible = false;
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void tbPageSelect1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                Convert01.g_splitsettings.split_page_mode = Convert01.tsplitpagemode.spSpecifyInterval;
                this.pbAll.BackgroundImage = Image.FromFile(spath01 + "menu_1_5.png");
                tbPageSelect2.Visible = false;
            }
        }

        private void tbPageSelect2_KeyDown(object sender, KeyEventArgs e)
        {
            if (Convert01.g_splitsettings.split_page_mode != Convert01.tsplitpagemode.spCustomize)
            {
                Convert01.g_splitsettings.split_page_mode = Convert01.tsplitpagemode.spCustomize;
                this.pbAll.BackgroundImage = Image.FromFile(spath01 + "menu_1_4.png");
            }
            allKeyPress(e);
        }

        private void allKeyPress(KeyEventArgs e)
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

        private void outputResult()
        {
            string str = tbPageSelect1.Text; //此位置通过Keypress事件控制只能输入0-9,其它不允许输入
            int num = 0x1869f; //0x1869F表示99999
            if (Convert01.g_splitsettings.split_page_mode == Convert01.tsplitpagemode.spSpecifyInterval)
            {
                if (str.Length <= 0)
                {
                    str = "1";
                    tbPageSelect1.Text = "1";
                }
                int pageTmp = System.Convert.ToInt32(str);
                tbPageSelect1.Text = pageTmp.ToString();

                if (pageTmp > 9999 && Convert01.g_splitsettings.split_page_mode == Convert01.tsplitpagemode.spSpecifyInterval)
                {
                    string sTip = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_01"); //提示
                    string sOld = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_02"); //分割页面不可大于99999
                    MessageBox.Show(sOld, sTip, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    tbPageSelect1.Text = num.ToString();
                }
                Convert01.g_splitsettings.split_page_interval = System.Convert.ToInt32(tbPageSelect1.Text);
            }
            if (Convert01.g_splitsettings.split_page_mode == Convert01.tsplitpagemode.spCustomize)
            {
                this.Tag = Convert01.strRemoveTrim(tbPageSelect2.Text, true); //去除杂乱符号
                if (Convert01.VerifyNumber01(this.Tag.ToString(), true) == false || verifyValidate(this.Tag.ToString()) == false) //校验数字是否正确
                {
                    tbPageSelect2.Focus();
                    return;
                }
                else
                {
                    this.Tag = Convert01.strMergeStr1(this.Tag.ToString());
                    parseCustomizePages(ref Convert01.g_splitsettings.customizePages, this.Tag.ToString());
                }
            }
            this.pbConfirm.BackgroundImage = Image.FromFile(spath01 + "sure_03.png");
            this.DialogResult = DialogResult.OK;
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

        internal static void parseCustomizePages(ref ArrayList customizePages, string pagestext)
        {
            char[] separator = new char[] { ';' };
            string[] strArray = pagestext.Split(separator);
            //g_last_parse_customizepage_result = true;
            customizePages.Clear();
            int num = 0x1869f; //0x1869F表示99999
            try
            {
                foreach (string str in strArray)
                {
                    if (str != "")
                    {
                        char[] chArray2 = new char[] { ',' };
                        string[] strArray3 = str.Split(chArray2);
                        Convert01.tpagesitem tpagesitem2 = new Convert01.tpagesitem
                        {
                            startIndex = 0,
                            endIndex = 0,
                            subPages = null
                        };
                        Convert01.tpagesitem tpagesitem = tpagesitem2;
                        if (strArray3.Length > 1)
                        {
                            tpagesitem.startIndex = -1;
                            tpagesitem.endIndex = -1;
                            tpagesitem.subPages = new ArrayList();
                        }
                        foreach (string str2 in strArray3)
                        {
                            if (str2 != "")
                            {
                                int num3;
                                if (!str2.Contains("-"))
                                {
                                    if (str2.Trim() != "")
                                    {
                                        num3 = int.Parse(str2.Trim());
                                        if (num3 > num)
                                        {
                                            num3 = num;
                                        }
                                        if (strArray3.Length > 1)
                                        {
                                            tpagesitem2 = new Convert01.tpagesitem
                                            {
                                                startIndex = num3,
                                                endIndex = num3,
                                                subPages = null
                                            };
                                            tpagesitem.subPages.Add(tpagesitem2);
                                        }
                                        else
                                        {
                                            tpagesitem.startIndex = num3;
                                            tpagesitem.endIndex = num3;
                                        }
                                    }
                                }
                                else
                                {
                                    char[] chArray3 = new char[] { '-' };
                                    string[] strArray5 = str2.Split(chArray3);
                                    if (strArray5.Length != 2)
                                    {
                                        //g_last_parse_customizepage_result = false;
                                        //MessageBox.Show("页码组合字符串格式错误：\n" + str2, "提示", MessageBoxButtons.OK,MessageBoxIcon.Warning);
                                        string msg01 = Program.Language_Load(Program.iniLanguage, "frmMain01Setting01", "MSG_01"); //提示
                                        string sOld = Program.Language_Load(Program.iniLanguage, "frmMain01Setting01", "MSG_04"); //您输入的密码错误，请重新输入
                                        MessageBox.Show(sOld + "\n" + str2, msg01, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    }
                                    else
                                    {
                                        num3 = int.Parse(strArray5[0].Trim());
                                        if (num3 > num)
                                        {
                                            num3 = num;
                                        }
                                        int num5 = int.Parse(strArray5[1].Trim());
                                        if (num5 > num)
                                        {
                                            num5 = num;
                                        }
                                        if (strArray3.Length > 1)
                                        {
                                            tpagesitem2 = new Convert01.tpagesitem
                                            {
                                                startIndex = num3,
                                                endIndex = num5,
                                                subPages = null
                                            };
                                            tpagesitem.subPages.Add(tpagesitem2);
                                        }
                                        else
                                        {
                                            tpagesitem.startIndex = num3;
                                            tpagesitem.endIndex = num5;
                                        }
                                    }
                                }
                            }
                        }
                        customizePages.Add(tpagesitem);
                    }
                }
            }
            catch
            {
                //g_last_parse_customizepage_result = false;
                //Logger.info("parseCustomizePages(),parse pages text failed.");
                string sTip = Program.Language_Load(Program.iniLanguage, "frmMain01Setting01", "MSG_01"); //提示
                string sOld = Program.Language_Load(Program.iniLanguage, "frmMain01Setting01", "MSG_04"); //页码组合字符串格式错误
                MessageBox.Show(sOld, sTip, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
