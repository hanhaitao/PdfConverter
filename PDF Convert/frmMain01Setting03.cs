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
    public partial class frmMain01Setting03 : Form
    {
        //pdf分割,专用界面

        [DllImport("user32")]
        public static extern int ReleaseCapture();//用鼠标拖动窗体,移动窗体
        [DllImport("user32")]
        public static extern int SendMessage(IntPtr hwnd, int msg, int wp, int lp);//用鼠标拖动窗体,移动窗体

        private string spath01 = string.Empty;
        private int radioSelect; //当前选择的是哪一个RadioButton按钮
        public int pageCount; //当前右键鼠标的总页数

        public frmMain01Setting03()
        {
            InitializeComponent();

            //放一个文本框目的是在初次显示窗体时把鼠标放在这里，否则tbPageSelect1会不显示提示信息
            rtbTemp.Left = this.Width + 10;
            rtbTemp.Top = 0;

            string sOld = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_02"); //请输入页码, 如 1-5;7,9;15-20,25
            this.tbPageSelect1.BackGroundText = sOld;
            sOld = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_03"); //请输入页码, 如 1-5,7,9
            this.tbPageSelect2.BackGroundText = sOld;

            this.Size = new Size(369, 292);//窗体界面固定大小(防止程序员不小心改变了大小,因为是多语言界面,窗体大小是固定的)

            //syncContext = SynchronizationContext.Current; //利用SynchronizationContext.Current在线程间同步上下文,允许一个线程和另外一个线程进行通讯，SynchronizationContext在通讯中充当传输者的角色。另外这里有个地方需要清楚的，不是每个线程都附加SynchronizationContext这个对象，只有UI线程是一直拥有的。
            //Control.CheckForIllegalCrossThreadCalls = false;//容许子线程随时更新ui.这也是它的死穴：在同一个test函数体内,不能保证自身事务的一致性.给label1付了值,一回头,就已经被别人改了,这和超市的踩踏事件的后果一样严重.

            tbPageSelect1.MaxLength = 32767; //最多可以输入32767位长度的数字(够用了)
            radioSelect = 1; // 默认RadioButton显示第1个
            switch (Program.iniLanguage.ToLower())
            {
                case "zh-cn":
                    spath01 = Application.StartupPath + "\\zh-CN\\frmMain01Setting03\\";
                    break;
                case "en":
                    spath01 = Application.StartupPath + "\\en\\frmMain01Setting03\\";
                    break;
                case "ja":
                    spath01 = Application.StartupPath + "\\ja\\frmMain01Setting03\\";
                    break;
                case "zh-tw":
                    spath01 = Application.StartupPath + "\\zh-TW\\frmMain01Setting03\\";
                    break;
                default:
                    spath01 = Application.StartupPath + "\\zh-CN\\frmMain01Setting03\\";
                    break;
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

        private void frmMain01Setting03_Load(object sender, EventArgs e)
        {
            this.Refresh();
            this.Text = Program.appProgName;

            tbPageSelect1.Location = new Point(25, 73); //通过cursorPosition取得位置坐标
            tbPageSelect2.Location = new Point(25, 182); //通过cursorPosition取得位置坐标
            pbConfirm.Location = new Point((int)((this.Width - pbConfirm.Width) / 2), 247); //通过cursorPosition取得位置坐标

            string sOld = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_02"); //请输入页码, 如 1-5;7,9;15-20,25
            this.toolTip1.SetToolTip(this.tbPageSelect1, sOld);//增加TextBox的提示
            sOld = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_03"); //请输入页码, 如 1-5,7,9
            this.toolTip1.SetToolTip(this.tbPageSelect2, sOld);//增加TextBox的提示
            tbPageSelect1.TextAlign = HorizontalAlignment.Left;
            tbPageSelect2.TextAlign = HorizontalAlignment.Left;

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

            //tbPageSelect1.Focus();
        }

        /// <summary>
        /// 设置简体中文语言.不同语言可以使用不同的控件定位(因为翻译长度不同)
        /// </summary>
        private void SetZhCn()
        {
            this.pbConfirm.BackgroundImage = Image.FromFile(spath01 + "sure_01.png");
            this.pbClose.BackgroundImage = Image.FromFile(Application.StartupPath + "\\images\\close_01.png"); //取消,窗体右上角关闭按钮
            this.pbAll.BackgroundImage = Image.FromFile(spath01 + "menu_3_1.png");
        }

        /// <summary>
        /// 设置英文语言.不同语言可以使用不同的控件定位(因为翻译长度不同)
        /// </summary>
        private void SetEn()
        {
            this.pbConfirm.BackgroundImage = Image.FromFile(spath01 + "sure_01.png"); //确定
            this.pbClose.BackgroundImage = Image.FromFile(Application.StartupPath + "\\images\\close_01.png"); //取消,窗体右上角关闭按钮
            this.pbAll.BackgroundImage = Image.FromFile(spath01 + "menu_3_1.png");
        }

        /// <summary>
        /// 设置日文语言.不同语言可以使用不同的控件定位(因为翻译长度不同)
        /// </summary>
        private void SetJa()
        {
            this.pbConfirm.BackgroundImage = Image.FromFile(spath01 + "sure_01.png"); //确定
            this.pbClose.BackgroundImage = Image.FromFile(Application.StartupPath + "\\images\\close_01.png"); //取消,窗体右上角关闭按钮
            this.pbAll.BackgroundImage = Image.FromFile(spath01 + "menu_3_1.png");
        }

        /// <summary>
        /// 设置繁体中文.不同语言可以使用不同的控件定位(因为翻译长度不同)
        /// </summary>
        private void SetZhTw()
        {
            this.pbConfirm.BackgroundImage = Image.FromFile(spath01 + "sure_01.png"); //确定
            this.pbClose.BackgroundImage = Image.FromFile(Application.StartupPath + "\\images\\close_01.png"); //取消,窗体右上角关闭按钮
            this.pbAll.BackgroundImage = Image.FromFile(spath01 + "menu_3_1.png");
        }

        private void pbAll_MouseDown(object sender, MouseEventArgs e)
        {
            //MouseDown和MouseClick两者不可同时共存,先发生前者,一般存在前者时后者就不触发了,切记
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, 274, 61440 + 9, 0);

                if (e.Location.X >= 25 && e.Location.X < 250 && e.Location.Y > 30 && e.Location.Y < 55) // 分割每一页
                {
                    radioSelect = 1;
                    this.pbAll.BackgroundImage = Image.FromFile(spath01 + "menu_3_1.png");
                    tbPageSelect1.Text = "";
                    //tbPageSelect1.Focus();
                    rtbTemp.Focus();
                }
                else if (e.Location.X >= 25 && e.Location.X < 250 && e.Location.Y > 138 && e.Location.Y < 165) // 分割奇数页
                {
                    radioSelect = 2;
                    this.pbAll.BackgroundImage = Image.FromFile(spath01 + "menu_3_2.png");
                    tbPageSelect2.Text = "";
                    //tbPageSelect2.Focus();
                    rtbTemp.Focus();
                }
            }
        }

        private void pbAll_MouseMove(object sender, MouseEventArgs e)
        {
            //cursorPosition(); //显示当前坐标位置,在编程时定位控件坐标位置使用,正式使用之前将其注释掉

            //鼠标移动到不同的区域显示不同光标,通过cursorPosition取得位置坐标
            if (e.Location.X >= 25 && e.Location.X < 250 && e.Location.Y > 30 && e.Location.Y < 55) // 分割每一页
            {
                this.Cursor = System.Windows.Forms.Cursors.Hand;
            }
            else if (e.Location.X >= 25 && e.Location.X < 250 && e.Location.Y > 138 && e.Location.Y < 165) // 分割奇数页
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
                this.pbAll.BackgroundImage = Image.FromFile(spath01 + "menu_3_1.png");
            }
            allKeyPress(e);
        }

        private void tbPageSelect1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                radioSelect = 1;
                this.pbAll.BackgroundImage = Image.FromFile(spath01 + "menu_3_1.png");
            }
        }

        private void tbPageSelect2_KeyDown(object sender, KeyEventArgs e)
        {
            if (radioSelect != 2)
            {
                radioSelect = 2;
                this.pbAll.BackgroundImage = Image.FromFile(spath01 + "menu_3_2.png");
            }
            allKeyPress(e);
        }

        private void tbPageSelect2_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                radioSelect = 2;
                this.pbAll.BackgroundImage = Image.FromFile(spath01 + "menu_3_2.png");
            }
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
            this.pbConfirm.BackgroundImage = Image.FromFile(spath01 + "sure_03.png");

            if (radioSelect == 1)
            {
                this.Tag = tbPageSelect1.Text;
            }
            else
            {
                this.Tag = tbPageSelect2.Text;
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
                        tbPageSelect1.Focus();
                    }
                    else
                    {
                        tbPageSelect2.Focus();
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
                        tbPageSelect1.Focus();
                        return;
                    }
                    else
                    {
                        this.Tag = Convert01.strRemoveTrim(tbPageSelect1.Text, true); //去除杂乱符号
                    }
                }
                else
                {
                    if (Convert01.VerifyNumber01(this.Tag.ToString(), false) == false)
                    {
                        tbPageSelect2.Focus();
                        return;
                    }
                    else
                    {
                        //转义成为“提取指定页数”
                        if (!MainInfo01.isReg) //免费版只转换前5页
                        {
                            if (pageCount >= Program.pageFreeConvert)
                            {
                                pageCount = Program.pageFreeConvert;
                            }
                        }
                        this.Tag = numberDeduction(this.Tag.ToString(), pageCount);
                    }
                }
            }
            this.Tag = Convert01.strMergeStr1(this.Tag.ToString());
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

        #region 数字反转,数字抠除,数字扣除
        /// <summary>
        /// 2016-01-27,KongMengyuan
        /// 由需要"删除指定页数"转义成为“提取指定页数”
        /// 数字抠除,数字反转:
        ///      比如"1-5,7,9"(最大数字为20),则反转为"6,8,10-20"
        ///      再如"5,8,11-13"(最大数字为20),则反转为"1-4,6-7,9-10,14-20"
        /// </summary>
        /// <param name="numberText">输入的组合字符串"需要删除的指定页数"</param>
        /// <param name="maxPageCount">所转换文件的最大页数</param>
        /// <returns>抠除后的剩余数字,为空则已经全部抠除</returns>
        private string numberDeduction(string numberText, int maxPageCount)
        {
            //测试数据:
            //    numberDeduction("1,2,2-13", 20);                     //    1,2,2-13 对应的是 14-20
            //    numberDeduction("3,3,3,3", 20);                      //    3,3,3,3 对应的是 1-2,4-20
            //    numberDeduction("1,2,11-13", 20);                    //    1,2,11-13 对应的是 3-10,14-20
            //    numberDeduction("5,8,11-13", 20);                    //    5,8,11-13 最大页 20 对应的是 1-4,6-7,9-10,14-20
            //    numberDeduction("1-2,11-13", 20);                    //    1-2,11-13 最大页 20 对应的是 3-10,14-20
            //    numberDeduction("1-2,2-3,11-13", 20);                //    1-2,2-3,11-13 最大页 20 对应的是 4-10,14-20
            //    numberDeduction("1-5,11-13", 20);                    //    1-5,11-13 最大页 20 对应的是 6-10,14-20
            //    numberDeduction("1,8,11-13", 20);                    //    1,8,11-13 最大页 20 对应的是 2-7,9-10,14-20
            //    numberDeduction("2,3,11-13", 20);                    //    2,3,11-13 最大页 20 对应的是 1,4-10,14-20
            //    numberDeduction("5,8,18-19", 20);                    //    5,8,18-19 最大页 20 对应的是 1-4,6-7,9-17,20
            //    numberDeduction("5,8,19-20", 20);                    //    5,8,19-20 最大页 20 对应的是 1-4,6-7,9-18
            //    numberDeduction("5,8,10,12,20", 20);                 //    5,8,10,20 最大页 20 对应的是 1-4,6-7,9,11,13-19
            //    numberDeduction("5,8,10,20", 20);                    //    5,8,10,20 最大页 20 对应的是 1-4,6-7,9,11-19
            //    numberDeduction("5,8,20", 20);                       //    5,8,20 最大页 20 对应的是 1-4,6-7,9-19
            //    numberDeduction("5,8,9,20", 20);                     //    5,8,9,20 最大页 20 对应的是 1-4,6-7,10-19
            //    numberDeduction("5,8,9,15-17,18-19,20", 20);         //    5,8,9,15-17,18-19,20 最大页 20 对应的是 1-4,6-7,10-14
            //    numberDeduction("5,8-9,15-17,18-19,20", 20);         //    5,8-9,15-17,18-19,20 最大页 20 对应的是 1-4,6-7,10-14
            //    numberDeduction("5-8,9,15-17,18-19,20", 20);         //    5-8,9,15-17,18-19,20 最大页 20 对应的是 1-4,10-14
            //    numberDeduction("5-8,9,15-17,18,19-20", 20);         //    5-8,9,15-17,18,19-20 最大页 20 对应的是 1-4,10-14
            //    numberDeduction("5-8,9,15-；17，,18，,19-20", 20);   //    5-8,9,15-；17，,18，,19-20 最大页 20 对应的是 1-4,10-14
            //    numberDeduction("5-8,9,15-17;，,18，,19-20", 20);    //    5-8,9,15-17;，,18，,19-20 最大页 20 对应的是 1-4,10-14
            //    numberDeduction("5-8,9,15-17;;,18,,19-20", 20);      //    5-8,9,15-17;;,18,,19-20 最大页 20 对应的是 1-4,10-14
            //    numberDeduction("5-8,9,15-16-17,18,19-20", 20);      //    5-8,9,15-16-17,18,19-20 最大页 20 对应的是 1-4,10-14
            //    numberDeduction("5-8,9,15-16;,18,19-20", 20);        //    5-8,9,15-16,;18,19-20 最大页 20 对应的是 1-4,10-14,17
            //    numberDeduction("5-8,9,15-16,;18,19-20", 20);        //    5-8,9,15-16;,18,19-20 最大页 20 对应的是 1-4,10-14,17
            //    numberDeduction("5—8,9,15-16,;18,19-20", 20);       //    5—8,9,15-16;,18,19-20 最大页 20 对应的是 1-4,10-14,17
            //    numberDeduction("5——8,9,15-16,;18,19-20", 20);     //    5——8,9,15-16;,18,19-20 最大页 20 对应的是 1-4,10-14,17 (——是按组合键"Shift和-"出来的)
            //    numberDeduction("-1,2,5", 20);                       //    -1,2,5 最大页 20 对应的是 3-4,6-20
            //    numberDeduction("5-8,9,15-17,18,19-20", 2000);       //    5-8,9,15-17,18,19-20 最大页 2000 对应的是 1-4,10-14,21-2000
            //    numberDeduction("0,0", 20);                          //    0,0 最大页 20 对应的是 1-20
            //    numberDeduction("0,00,0", 20);                       //    0,00,0 最大页 20 对应的是 1-20
            //    numberDeduction("0;00,0", 20);                       //    0;00,0 最大页 20 对应的是 1-20
            //    numberDeduction("1-5,6-20", 20);                     //    1-5,6-20 最大页 20 对应的是               , 空白
            //    numberDeduction("1-5,6-10,11-18,18-20", 20);         //    1-5,6-10,11-18,18-20 最大页 20 对应的是   , 空白
            //    numberDeduction("；；；；,,", 20);                   //    ；；；；,, 最大页 20 对应的是
            //    numberDeduction("", 20);                             //     最大页 20 对应的是
            //    numberDeduction("1", 1);                             //    1 最大页 1                                , 弹出错误提示
            //    numberDeduction("1,2,555", 20);                      //    1,2,555 最大页 20                         , 弹出错误提示
            //    numberDeduction("1,2,555,20", 20);                   //    1,2,5张三,20 最大页 20                    , 弹出错误提示
            //    numberDeduction("5-8,9,aaa,15-17,18,19-20", 2000);   //    5-8,9,aaa,15-17,18,19-20 最大页 2000      , 弹出错误提示
            //    numberDeduction("5-8,9a,aaa,15-17,18,19-20", 2000);  //    5-8,9a,aaa,15-17,18,19-20 最大页 2000     , 弹出错误提示
            //    numberDeduction("5-8,9a,15-17,18,19-20", 2000);      //    5-8,9a,15-17,18,19-20 最大页 2000         , 弹出错误提示
            //    numberDeduction("1aaa", 1);                          //    1aaa 最大页 1                             , 弹出错误提示
            //    numberDeduction("1aaa,2,5", 20);                     //    1aaa,2,5 最大页 20                        , 弹出错误提示
            //    numberDeduction("1,2,5,20aaa", 20);                  //    1,2,5,20aaa 最大页 20                     , 弹出错误提示
            //    numberDeduction("1,2,5,20aaa", 20);                  //    1,2,5,20aaa 最大页 20                     , 弹出错误提示
            //    numberDeduction("1,2,5张,20", 20);                   //    1,2,5张,20 最大页 20                      , 弹出错误提示
            //    numberDeduction("1,2,5张三,20", 20);                 //    1,2,5张三,20 最大页 20                    , 弹出错误提示 
            //    numberDeduction("1,2,520", 20);                      //    1,2,520 最大页 20                         , 弹出错误提示
            //    numberDeduction("333333333333333333333333", 20);     //    333333333333333333333333 最大页 20        , 弹出错误提示

            int inputMaxCount = 0;
            int pageCount = maxPageCount;
            string s = numberText;
            s = Convert01.strRemoveTrim(s, false);

            //检查数字是否正确
            if (s.Trim().Length > 0)
            {
                //将非数字和字母换为逗号,或者弹出错误
                string s1 = s.Replace("-", ","); //中间的“破折号+逗号”代替为1个破折号
                string[] newStr1 = s1.Split(',');
                for (int i = 0; i <= newStr1.GetUpperBound(0); i++)
                {
                    //s = Regex.Replace(s, @"[^0-9a-ZA-Z]+", ",", RegexOptions.IgnoreCase).Trim();

                    //所输入的不全是数字,可能包含字母或汉字
                    int numberOne;
                    if (!int.TryParse(newStr1[i], out numberOne))
                    {
                        string sTip = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_01"); //提示
                        string sOld = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_08"); //所输入的数据 $S 不全是数字，请重新输入！
                        sOld = Program.strReplace(sOld, "$S", new string[] { newStr1[i] }); //替换Language.ini字符串里面的通用代替符号
                        MessageBox.Show(sOld + "\r\n" + "\r\n" + this.AccessibleName, sTip, MessageBoxButtons.OK, MessageBoxIcon.Error);  // 字符之间加入回车符
                        return string.Empty;
                    }
                    //所输入的数字必须是从小到大排列的
                    try
                    {
                        if (System.Convert.ToInt32(newStr1[i + 1]) < System.Convert.ToInt32(newStr1[i]))
                        {
                            string sTip = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_01"); //提示
                            string sOld = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_06"); //所输入的数字必须是从小到大排列的，请重新输入！                            
                            MessageBox.Show(sOld + "\r\n" + "\r\n" + this.AccessibleName, sTip, MessageBoxButtons.OK, MessageBoxIcon.Error);  // 字符之间加入回车符
                            return string.Empty;
                        }
                    }
                    catch
                    { }
                }
            }
            else
            {
                return string.Empty;
            }

            //所输入的数字不能超过最大范围
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
                    return string.Empty;
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
            }

            //如果只有1页,则不可以删除,直接返回错误
            if (pageCount == 1)
            {
                string sTip = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_01"); //提示
                string sOld = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_10"); //总页数只有1页,不允许再删除(用户不选择它即可)
                MessageBox.Show(sOld + "\r\n" + "\r\n" + this.AccessibleName, sTip, MessageBoxButtons.OK, MessageBoxIcon.Error);  // 字符之间加入回车符
                return string.Empty;
            }

            string rtnStr = string.Empty;
            if (errString != string.Empty)
            {
                string sTip = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_01"); //提示
                string sOld = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_05"); //输入的页码 $S 已经超过可以转换的最大页数 $S ,请检查输入的页码范围
                sOld = Program.strReplace(sOld, "$S", new string[] { errString, pageCount.ToString() }); //替换Language.ini字符串里面的通用代替符号
                MessageBox.Show(sOld + "\r\n" + "\r\n" + this.AccessibleName, sTip, MessageBoxButtons.OK, MessageBoxIcon.Error);  //字符之间加入回车符
                return string.Empty;
            }
            else
            {
                string[] newStr1 = s.Split(',');

                //int[,] arr = new int[2, 2] { { 1, 2 }, { 3, 4 } }; //二维数组
                int[,] arr = new int[newStr1.GetUpperBound(0) + 1, 2]; //动态二维数组

                for (int i = 0; i <= newStr1.GetUpperBound(0); i++)
                {
                    string[] newStr2 = newStr1[i].Split('-');
                    if (newStr2.GetUpperBound(0) == 0)
                    {
                        arr[i, 0] = System.Convert.ToInt32(newStr2[0]) - 1;
                        arr[i, 1] = System.Convert.ToInt32(newStr2[0]) + 1;
                    }
                    else
                    {
                        arr[i, 0] = System.Convert.ToInt32(newStr2[0]) - 1;
                        arr[i, 1] = System.Convert.ToInt32(newStr2[1]) + 1;
                    }
                    //System.Console.WriteLine(arr[i, 0].ToString() + " 第1次整理 " + arr[i, 1].ToString());
                }

                for (int j = 0; j < arr.GetUpperBound(0); j++)
                {
                    //把没有用的数据全部变为0
                    try
                    {
                        if (arr[j + 1, 0] < arr[j, 1]) //如果后面的值比前面的末尾值大,两者全部置0
                        {
                            arr[j, 1] = 0;
                            arr[j + 1, 0] = 0;
                        }
                        if (arr[j + 1, 0] == arr[j, 1]) //如果两者相等,则取1个值即可
                        {
                            //arr[j, 1] = 0;
                            arr[j + 1, 0] = 0;
                        }
                    }
                    catch
                    { }
                }
                //把去掉0的数字重新连接起来
                for (int j = 0; j <= arr.GetUpperBound(0); j++)
                {
                    //把不是0的数据两两连接起来
                    //System.Console.WriteLine(arr[j, 0].ToString() + " 第2次整理 " + arr[j, 1].ToString());

                    if (arr[j, 0] != 0)
                    {
                        rtnStr = rtnStr + "-" + arr[j, 0];
                    }
                    if (arr[j, 1] != 0)
                    {
                        if (arr[j, 1] <= pageCount)
                        {
                            rtnStr = rtnStr + "," + arr[j, 1];
                        }
                    }
                    //处理第1个值
                    if (j == 0 && arr[0, 0] > 1)
                    {
                        rtnStr = "1-" + rtnStr;
                    }
                    //处理最后一个值
                    if (j == arr.GetUpperBound(0) && arr[j, 1] < pageCount)
                    {
                        rtnStr = rtnStr + "-" + pageCount.ToString();
                    }
                }
                //处理多余符号
                while (rtnStr.Contains("--"))
                {
                    rtnStr = rtnStr.Replace("--", "-"); //中间的多个破折号代替为1个破折号
                }
                char[] MyChar1 = { ';', '；', ',', '，', '-', '?', '*' }; //去除字符串前后两边的多余符号和空格"  ；;,，-?*  "
                rtnStr = rtnStr.Trim(MyChar1).Trim();
            }

            //System.Console.WriteLine(s + " 对应的是 " + rtnStr);
            if (rtnStr == "")
            {
                string sTip = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_01"); //提示
                string sOld = Program.Language_Load(Program.iniLanguage, this.Name, "MSG_09"); //输入的数字 $S 全部删除了,请检查输入的数字范围
                sOld = Program.strReplace(sOld, "$S", new string[] { numberText }); //替换Language.ini字符串里面的通用代替符号
                MessageBox.Show(sOld + "\r\n" + "\r\n" + this.AccessibleName, sTip, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);  // 字符之间加入回车符
            }
            return rtnStr;
        }
        #endregion

    }
}
