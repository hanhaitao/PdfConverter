using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace PDF_Convert
{
    public partial class ucBuy : UserControl
    {
        public ucBuy()
        {
            InitializeComponent();
        }

        public string Title
        {
            get { return labTile.Text; }
            set { labTile.Text = value; }
        }

        public string InTxt
        {
            get { return labIn.Text; }
            set { labIn.Text = value; }
        }

        public Image Img
        {
            get { return buyImg.Image; }
            set { buyImg.Image = value; }
        }

        public Color TextColor
        {
            get { return labTile.ForeColor; }
            set { labTile.ForeColor = value; }
        }

        private void buyImg_MouseClick(object sender, MouseEventArgs e)
        {
            if (this.Name == "ucBuy")
            {
                //Process.Start("http://www.xjpdf.com/software/pdfConvert/buy/?version=" + Version.version + "&machine=" + new reg().get_machine_code());
                try
                {
                    //Process.Start("http://www.xjpdf.com/software/pdfConvert/buy/?version=" + Version.version + "&machine=" + new reg().get_machine_code());
                    //KongMengyuan修改,郑总提出来：在点击右上角的“购买”时打开页面会卡一下,而点击右下角的"在线教程"就不卡,改变写法(提取机器码浪费了多余的时间)
                    Process.Start("http://www.xjpdf.com/software/pdfConvert/buy/?version=" + Program.httpVersion + "&machine=" + Program.httpMachineCode);
                }
                catch
                {
                    //Process.Start("iexplore.exe", "http://www.xjpdf.com/software/pdfConvert/buy/?version=" + Version.version + "&machine=" + new reg().get_machine_code());
                    //KongMengyuan修改,郑总提出来：在点击右上角的“购买”时打开页面会卡一下,而点击右下角的"在线教程"就不卡,改变写法(提取机器码浪费了多余的时间)
                    Process.Start("iexplore.exe", "http://www.xjpdf.com/software/pdfConvert/buy/?version=" + Program.httpVersion + "&machine=" + Program.httpMachineCode);
                }
                //发送请求信息 
                Version.Post("http://all.jsocr.com/", Version.GetParamName("Data"), Program.appProgName, Version.GetParamName("Version"), new reg().get_reg_code(), "注册窗口", "购买正式版");
            }
            if (this.Name == "ucQQ")
            {
                //KongMY,2015-10-30,增加判断"UcQQ"
                try
                {
                    //Process.Start("http://www.xjpdf.com/software/pdfConvert/qq/?version=" + Version.version + "&machine=" + new reg().get_machine_code());
                    //KongMengyuan修改,郑总提出来：在点击右上角的“购买”时打开页面会卡一下,而点击右下角的"在线教程"就不卡,改变写法(提取机器码浪费了多余的时间)
                    Process.Start("http://www.xjpdf.com/software/pdfConvert/qq/?version=" + Program.httpVersion + "&machine=" + Program.httpMachineCode);
                }
                catch
                {
                    //Process.Start("iexplore.exe", "http://www.xjpdf.com/software/pdfConvert/qq/?version=" + Version.version + "&machine=" + new reg().get_machine_code()); //必须使用IE9打开,IE10是打不开客服QQ的
                    //KongMengyuan修改,郑总提出来：在点击右上角的“购买”时打开页面会卡一下,而点击右下角的"在线教程"就不卡,改变写法(提取机器码浪费了多余的时间)
                    Process.Start("iexplore.exe", "http://www.xjpdf.com/software/pdfConvert/qq/?version=" + Program.httpVersion + "&machine=" + Program.httpMachineCode);
                }
            }
        }

        private void labTile_MouseClick(object sender, MouseEventArgs e)
        {
            if (this.Name == "ucBuy")
            {
                //Process.Start("http://www.xjpdf.com/software/pdfConvert/buy/?version=" + Version.version + "&machine=" + new reg().get_machine_code());
                try
                {
                    //Process.Start("http://www.xjpdf.com/software/pdfConvert/buy/?version=" + Version.version + "&machine=" + new reg().get_machine_code());
                    //KongMengyuan修改,郑总提出来：在点击右上角的“购买”时打开页面会卡一下,而点击右下角的"在线教程"就不卡,改变写法(提取机器码浪费了多余的时间)
                    Process.Start("http://www.xjpdf.com/software/pdfConvert/buy/?version=" + Program.httpVersion + "&machine=" + Program.httpMachineCode);
                }
                catch
                {
                    //Process.Start("iexplore.exe", "http://www.xjpdf.com/software/pdfConvert/buy/?version=" + Version.version + "&machine=" + new reg().get_machine_code());
                    //KongMengyuan修改,郑总提出来：在点击右上角的“购买”时打开页面会卡一下,而点击右下角的"在线教程"就不卡,改变写法(提取机器码浪费了多余的时间)
                    Process.Start("iexplore.exe", "http://www.xjpdf.com/software/pdfConvert/buy/?version=" + Program.httpVersion + "&machine=" + Program.httpMachineCode);
                }
                //发送请求信息 
                Version.Post("http://all.jsocr.com/", Version.GetParamName("Data"), Program.appProgName, Version.GetParamName("Version"), new reg().get_reg_code(), "注册窗口", "购买正式版");
            }
            
            if (this.Name == "ucQQ")
            {
                //KongMY,2015-10-30,增加判断"UcQQ"
                try
                {
                    //Process.Start("http://www.xjpdf.com/software/pdfConvert/qq/?version=" + Version.version + "&machine=" + new reg().get_machine_code());
                    //KongMengyuan修改,郑总提出来：在点击右上角的“购买”时打开页面会卡一下,而点击右下角的"在线教程"就不卡,改变写法(提取机器码浪费了多余的时间)
                    Process.Start("http://www.xjpdf.com/software/pdfConvert/qq/?version=" + Program.httpVersion + "&machine=" + Program.httpMachineCode);
                }
                catch
                {
                    //Process.Start("iexplore.exe", "http://www.xjpdf.com/software/pdfConvert/qq/?version=" + Version.version + "&machine=" + new reg().get_machine_code()); //必须使用IE9打开,IE10是打不开客服QQ的
                    //KongMengyuan修改,郑总提出来：在点击右上角的“购买”时打开页面会卡一下,而点击右下角的"在线教程"就不卡,改变写法(提取机器码浪费了多余的时间)
                    Process.Start("iexplore.exe", "http://www.xjpdf.com/software/pdfConvert/qq/?version=" + Program.httpVersion + "&machine=" + Program.httpMachineCode);
                }
            }
        }

        private void labIn_MouseClick(object sender, MouseEventArgs e)
        {
            if (this.Name == "ucBuy")
            {
                //Process.Start("http://www.xjpdf.com/software/pdfConvert/buy/?version=" + Version.version + "&machine=" + new reg().get_machine_code());
                try
                {
                    //Process.Start("http://www.xjpdf.com/software/pdfConvert/buy/?version=" + Version.version + "&machine=" + new reg().get_machine_code());
                    //KongMengyuan修改,郑总提出来：在点击右上角的“购买”时打开页面会卡一下,而点击右下角的"在线教程"就不卡,改变写法(提取机器码浪费了多余的时间)
                    Process.Start("http://www.xjpdf.com/software/pdfConvert/buy/?version=" + Program.httpVersion + "&machine=" + Program.httpMachineCode);
                }
                catch
                {
                    //Process.Start("iexplore.exe", "http://www.xjpdf.com/software/pdfConvert/buy/?version=" + Version.version + "&machine=" + new reg().get_machine_code());
                    //KongMengyuan修改,郑总提出来：在点击右上角的“购买”时打开页面会卡一下,而点击右下角的"在线教程"就不卡,改变写法(提取机器码浪费了多余的时间)
                    Process.Start("iexplore.exe", "http://www.xjpdf.com/software/pdfConvert/buy/?version=" + Program.httpVersion + "&machine=" + Program.httpMachineCode);
                }
                //发送请求信息 
                Version.Post("http://all.jsocr.com/", Version.GetParamName("Data"), Program.appProgName, Version.GetParamName("Version"), new reg().get_reg_code(), "注册窗口", "购买正式版");
            }
            if (this.Name == "ucQQ")
            {
                //KongMY,2015-10-30,增加判断"UcQQ"
                try
                {
                    //Process.Start("http://www.xjpdf.com/software/pdfConvert/qq/?version=" + Version.version + "&machine=" + new reg().get_machine_code());
                    //KongMengyuan修改,郑总提出来：在点击右上角的“购买”时打开页面会卡一下,而点击右下角的"在线教程"就不卡,改变写法(提取机器码浪费了多余的时间)
                    Process.Start("http://www.xjpdf.com/software/pdfConvert/qq/?version=" + Program.httpVersion + "&machine=" + Program.httpMachineCode);
                }
                catch
                {
                    //Process.Start("iexplore.exe", "http://www.xjpdf.com/software/pdfConvert/qq/?version=" + Version.version + "&machine=" + new reg().get_machine_code()); //必须使用IE9打开,IE10是打不开客服QQ的
                    //KongMengyuan修改,郑总提出来：在点击右上角的“购买”时打开页面会卡一下,而点击右下角的"在线教程"就不卡,改变写法(提取机器码浪费了多余的时间)
                    Process.Start("iexplore.exe", "http://www.xjpdf.com/software/pdfConvert/qq/?version=" + Program.httpVersion + "&machine=" + Program.httpMachineCode);
                }
            }
        }
    }
}
