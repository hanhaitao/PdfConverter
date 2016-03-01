using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Controls;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Resources;
using Aspose.Pdf;
using Aspose.Pdf.Text;
using Aspose.Slides;
using Aspose.Words;
using Aspose.Words.Drawing;
using Document = Aspose.Pdf.Document;
using Image = System.Drawing.Image;
using ImageFileType = Aspose.Pdf.Generator.ImageFileType;
using Point = System.Drawing.Point;
using SaveFormat = Aspose.Pdf.SaveFormat;
using Section = Aspose.Pdf.Generator.Section;

namespace PDF_Convert
{
    public class ConvertOld
    {
        private Aspose.Pdf.Document pdf_doc;
        private Aspose.Words.Document word_doc;
        private Aspose.Cells.Workbook excel_doc;
        private Aspose.Slides.Presentation ppt_doc;
        private Aspose.Pdf.Facades.PdfExtractor txt_doc;
        private FileStream fileStream;
        private int pages;
        private string outPath;
        public delegate void save_progress(int cur, int i);
        public bool CloseDirect; //KongMengyuan,2015-10-30,记录是否直接关闭屏幕
        //private FORMAT targetFormat;
        public FORMAT targetFormat;//KongMengyuan修改,2015-11-03,参考互盾PDFCon
        //是否弹出密码窗口
        private bool file_can_work = true;
        public string OtherMessage = "";
        private string err_msg = "";
        ResourceManager rm = new ResourceManager(typeof(MainInfoOld));
        public event ErrorMessageHandler ErrorMessageEvent;

        private bool SetCount = false;//KongMengyuan修改,2015-11-03,参考互盾PDFCon
        public int Count = 10;//KongMengyuan修改,2015-11-03,参考互盾PDFCon
        public enum FORMAT
        {
            File2WORD,
            File2EXCEL,
            File2PPT,
            File2HTML,
            IMG2PDF,
            File2TXT,
            File2IMG,
            DOC2PDF,
            PPT2PDF,
            Excel2PDF,
            //2015-06-16
            PDFSplit,
            PDFDecode,
            PDFMerge,
            PDFCompress,
            PDFGetImg
        };

        public ConvertOld(string file_path, string outPath, FORMAT format, MainInfoOld mainInfo)
        {
            try
            {
                fileStream = new FileStream(file_path, FileMode.Open, FileAccess.Read);
                this.outPath = outPath;
                this.targetFormat = format;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains(rm.GetString("msg6")))
                {
                    MessageBox.Show(
                        string.Format(rm.GetString("msg7"), file_path), rm.GetString("Tips"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                //if (ex.Message.Contains("正由另一进程使用"))
                //{
                //    MessageBox.Show("您的 " + file_path + " 文件已打开，请先关闭文件再进行转换！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //}

                return;
            }

            try
            {
                //pdf_doc = new Aspose.Pdf.Document(fileStream, global_config.password);
                string fileType = Path.GetExtension(file_path).ToLower();

                if (fileType.ToLower() == ".pdf")
                {
                    pdf_doc = new Aspose.Pdf.Document(fileStream);
                    pages = pdf_doc.Pages.Count;
                }
                else if (fileType.ToLower() == ".doc" || fileType.ToLower() == ".docx")
                {
                    word_doc = new Aspose.Words.Document(fileStream);
                    pages = word_doc.PageCount;
                }
                else if (fileType.ToLower() == ".ppt" || fileType.ToLower() == ".pptx")
                {
                    try
                    {
                        ppt_doc = new Presentation(fileStream);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.StackTrace);
                    }
                    pages = ppt_doc.Slides.Count;
                }
                else if (fileType.ToLower() == ".xls" || fileType.ToLower() == ".xlsx")
                {
                    excel_doc = new Aspose.Cells.Workbook(fileStream);
                    pages = excel_doc.Worksheets.Count;
                }
            }
            catch (Aspose.Pdf.Exceptions.InvalidPasswordException ex)
            {
                //UpdateTips frm = new UpdateTips();
                //frm.StartPosition = FormStartPosition.Manual;
                //frm.Location = this.PointToScreen(new Point(400, this.lstFile.Location.Y + 30));
                //DialogResult dr = frm.ShowDialog();
                PassWordDlgOld frm = new PassWordDlgOld(Path.GetFileName(file_path));
                frm.StartPosition = FormStartPosition.CenterParent;
                frm.Location = mainInfo.PointToScreen(new Point(350, mainInfo.lstFile.Location.Y + 30));
                DialogResult dr = frm.ShowDialog();
                bool ctn = true;
                while (dr == DialogResult.OK && ctn)
                {
                    try
                    {
                        fileStream = new FileStream(file_path, FileMode.Open, FileAccess.Read);
                        pdf_doc = new Aspose.Pdf.Document(fileStream, frm.new_password);
                        pages = pdf_doc.Pages.Count;
                        ctn = false;
                    }
                    catch (Aspose.Pdf.Exceptions.InvalidPasswordException)
                    {
                        MessageBox.Show("您输入的密码错误，请重新输入");
                        dr = frm.ShowDialog();
                    }
                }

                if (dr == DialogResult.Cancel || dr.ToString() == "Cancel")
                {
                    err_msg = "";
                    file_can_work = false;
                    OtherMessage = "文件已加密，请输入密码。";
                    CloseDirect = true; //KongMengyuan,2015-10-30,如果弹出窗体直接关闭则不理会它
                }
            }
            catch (Exception)
            {
                file_can_work = false;
                //err_msg = "发生未知错误";
                err_msg = rm.GetString("msg8");
            }

        }

        /// <summary>
        /// 转换方法
        /// </summary>
        /// <param name="progress">进度条委托</param>
        /// <param name="dlg">窗口</param>
        /// <param name="fileType">文档类型</param>
        /// <param name="index">列表索引</param>
        public void Save(Form dlg = null, string fileType = "", int index = 0, ListViewItem lv = null)
        {
            if (!file_can_work)
                return;
            try
            {
                switch (targetFormat)
                {
                    case FORMAT.File2WORD:
                        {
                            FileToWordMain(dlg, fileType, index, lv);
                        }
                        break;
                    case FORMAT.File2EXCEL:
                        {
                            FileToExcel(dlg, fileType, index, lv);
                        }
                        break;
                    case FORMAT.File2PPT:
                        {
                            FileToPPT(dlg, fileType, index, lv);
                        }
                        break;
                    case FORMAT.File2HTML:
                        {
                            FileToHTML(dlg, fileType, index, lv);
                        }
                        break;
                    case FORMAT.IMG2PDF:
                        {
                            IMGToPDF(dlg, fileType, index, lv);
                        }
                        break;
                    case FORMAT.File2TXT:
                        {
                            FiletToTXT(dlg, fileType, index, lv);
                        }
                        break;
                    case FORMAT.File2IMG:
                        {
                            FileToIMG(dlg, fileType, index, lv);
                        }
                        break;

                    case FORMAT.DOC2PDF:
                        {
                            DocToPDF(dlg, index, lv);
                        }
                        break;

                    case FORMAT.PPT2PDF:
                        {
                            PptToPDF(dlg, index, lv);
                        }
                        break;
                    case FORMAT.Excel2PDF:
                        {
                            XlsToPDF(dlg, index, lv);
                        }
                        break;

                    //2015-06-16
                    case FORMAT.PDFSplit:
                        {
                            //PDFSplit(dlg, fileType, index, lv);
                            //KongMengyuan增加,2015-11-10,参考互盾PDFCon
                            PDFSplitMain(dlg, fileType, index, lv);
                        }
                        break;
                    case FORMAT.PDFDecode:
                        {
                            PDFDecode(dlg, fileType, index, lv);
                        }
                        break;
                    case FORMAT.PDFCompress:
                        {
                            PDFCompress(dlg, fileType, index, lv);
                        }
                        break;
                    case FORMAT.PDFMerge:
                        {
                            PDFMerge(dlg, fileType, index, lv);
                        }
                        break;
                    case FORMAT.PDFGetImg:
                        {
                            PDFGetImg(dlg, fileType, index, lv);
                        }
                        break;
                }
            }
            catch (Exception)
            { }
        }

        private void PDFGetImg(Form dlg = null, string fileType = null, int index = 0, ListViewItem lv = null)
        {
            try
            {
                //outPath = outPath + "IMG\\";                
                outPath = outPath + "_IMG\\";//KongMengyuan修改,2015-11-10,原生成的路径不明显,加一个下划线区分一下

                ((ItemInfomation)lv.Tag).FileFullConvertPath = outPath;
                if (!Directory.Exists(outPath))
                {
                    Directory.CreateDirectory(outPath);
                }
                Document pdfDoc = null;
                MainInfoOld mainInfo = dlg as MainInfoOld;
                int startRate = 0;

                if (fileType.ToLower() == ".pdf")
                {
                    pdfDoc = pdf_doc;
                    startRate = 0;
                }
                else if (fileType.ToLower() == ".doc" || fileType.ToLower() == ".docx")
                {
                    pdfDoc = DocToPDF(dlg, lv.Index, lv);
                    startRate = 50;
                }
                else if (fileType.ToLower() == ".ppt" || fileType.ToLower() == ".pptx")
                {
                    pdfDoc = PptToPDF(dlg, lv.Index, lv);
                    startRate = 50;
                }
                else if (fileType.ToLower() == ".xls" || fileType.ToLower() == ".xlsx")
                {
                    pdfDoc = XlsToPDF(dlg, lv.Index, lv);
                    startRate = 50;
                }
                mainInfo.UpdateProcess(new TempClass(index, startRate));

                int count = pdfDoc.Pages.Count;

                //KongMengyuan修改,2015-11-03,参考互盾PDFCon
                if (SetCount == false)
                {
                    SetCount = true;
                    this.Count = count;
                }

                //int total = count;
                int total = count;// KongMengyuan注释,2015-12-15
                List<int> pageLst = GetPage(lv.SubItems[3].Text, pdfDoc.Pages.Count);
                if (pageLst == null || pageLst.Count < 1) //如果没有"选择页码",则把全部页码全部赋值给它,KongMengyuan注释,2015-12-16
                {
                    pageLst = new List<int>();
                    for (int i = 1; i <= count; i++)
                    {
                        pageLst.Add(i);
                    }
                }
                if (!MainInfoOld.isReg)
                {
                    if (pdfDoc.Pages.Count >= Program.pageFreeConvert)
                    {
                        total = Program.pageFreeConvert;
                    }
                }

                ErrorMessageArgs errorMsg = null; ;//KongMengyuan增加,2015-11-10,页面弹出提示专用
                bool pictureHave = false; //KongMengyuan增加,2015-11-11,当前PDF是否有图片

                for (int i = 0, c = 1; i < total; i++, c++)
                {
                    if (mainInfo.isClose) break;
                    try
                    {
                        while (((ItemInfomation)lv.Tag).Status == StatusType.Pause)
                        {
                            Application.DoEvents();
                        }
                        int page = pageLst[i];
                        if (pdfDoc.Pages[page].Resources.Images != null && pdfDoc.Pages[page].Resources.Images.Count > 0)
                        {
                            XImageCollection img = pdfDoc.Pages[page].Resources.Images;
                            for (int j = 1; j <= img.Count; j++)
                            {
                                try
                                {
                                    XImage xImage = img[j];
                                    MemoryStream imageStream = new MemoryStream();
                                    xImage.Save(imageStream, ImageFormat.Jpeg);
                                    Image newImg = Image.FromStream(imageStream);

                                    //newImg.Save(outPath + page.ToString() + "_" + j.ToString() + ".jpg"); 
                                    //((ItemInfomation)lv.Tag).FileFullConvertPath = outPath + page.ToString() + "_" + j.ToString() + ".jpg";
                                    //KongMenyuan修改,2015-11-10,原代码是使用“PDF图片所在页数_图片序号.jpg”作文件名的,用户看着不方便,现改为“文件名_PDF图片所在页数_图片序号.jpg”
                                    string file = ((ItemInfomation)mainInfo.lstFile.Items[index].Tag).FileFullPath;
                                    string name = getNameAndType(file);
                                    //去除扩展名
                                    int filenameStart = name.LastIndexOf(".");
                                    name = name.Substring(0, filenameStart); //只获取纯文件名(不带扩展名)
                                    newImg.Save(outPath + name + "_" + page.ToString() + "_" + j.ToString() + ".jpg");
                                    ((ItemInfomation)lv.Tag).FileFullConvertPath = outPath + name + "_" + page.ToString() + "_" + j.ToString() + ".jpg";
                                    pictureHave = true; //KongMengyuan增加,2015-11-11,原来的提示是只要PDF的首页没有图片,就会弹出提示,现在改为PDF任何页面有图片都不提示

                                    if (startRate == 50)
                                    {
                                        //mainInfo.UpdateProcess(new TempClass(lv.Index, (c * 50 / total) + 50));
                                        //mainInfo.UpdateProcess(new TempClass(lv.Index, 100));
                                        mainInfo.UpdateProcess(new TempClass(lv.Index, c * 100 / total));
                                    }
                                    else
                                    {
                                        //int cur = i * 100 / pdfDoc.Pages.Count;
                                        mainInfo.UpdateProcess(new TempClass(lv.Index, c * 100 / total));
                                    }
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e);
                                }
                            }
                            if (startRate == 50)
                            {
                                //mainInfo.UpdateProcess(new TempClass(lv.Index, (c * 50 / total) + 50));
                                //mainInfo.UpdateProcess(new TempClass(lv.Index, 100));
                                mainInfo.UpdateProcess(new TempClass(lv.Index, c * 100 / total));
                            }
                            else
                            {
                                //int cur = i * 100 / pdfDoc.Pages.Count;
                                mainInfo.UpdateProcess(new TempClass(lv.Index, c * 100 / total));
                            }
                        }
                        else
                        {
                            string file = ((ItemInfomation)mainInfo.lstFile.Items[index].Tag).FileFullPath;
                            string name = getNameAndType(file);

                            //ErrorMessageArgs args = new ErrorMessageArgs(name + "未包含图片！");
                            //if (ErrorMessageEvent != null)
                            //{
                            //    ErrorMessageEvent(this, args);
                            //}

                            //KongMengyuan修改,2015-11-10,如果有100页PDF就会弹出100个提示,这个提示放在这里不合理,要修改掉
                            errorMsg = null;
                            ErrorMessageArgs args = new ErrorMessageArgs(name + "  未包含图片！");
                            if (ErrorMessageEvent != null)
                            {
                                errorMsg = args;
                                //ErrorMessageEvent(this, args);
                            }
                        }

                    }
                    catch
                    {
                        if (startRate == 50)
                        {
                            //mainInfo.UpdateProcess(new TempClass(lv.Index, (c * 50 / total) + 50));
                            //mainInfo.UpdateProcess(new TempClass(lv.Index, 100));
                            mainInfo.UpdateProcess(new TempClass(lv.Index, c * 100 / total));
                        }
                        else
                        {
                            //int cur = i * 100 / pdfDoc.Pages.Count;
                            mainInfo.UpdateProcess(new TempClass(lv.Index, c * 100 / total));
                        }
                        continue;
                    }
                }

                //KongMengyuan修改,2015-11-10,如果有100页PDF就会弹出100个提示,这个提示应该放在这里
                if (errorMsg != null && pictureHave == false)
                {
                    ErrorMessageEvent(this, errorMsg);
                    errorMsg = null;

                    //KongMengyuan增加,2015-11-25,原来的情况是：没有图片则显示一个问号的图标. 新的修改为：没有图片就把这个文件移除(原代码已经把文件名移除,只是仍有一个picUnknow图片显示)
                    mainInfo.diclst.Remove(((ItemInfomation)lv.Tag).FileFullPath);
                    mainInfo.lstFile.RemoveFile(lv.Index);
                    if (mainInfo.lstFile.Items.Count < 1)
                    {
                        mainInfo.SetThreeButtonValidate(false);  //KongMengyuan增加,2015-12-16,开始转换,停止转换,清空列表,三个按钮的有效还是失效: true-有效,false-失效
                    }
                }

                mainInfo.UpdateProcess(new TempClass(lv.Index, 100));
                UpdateLstState(lv, mainInfo);
                pdfDoc.FreeMemory();//2015-12-21,KongMengyuan增加,释放内存,否则会出现"当文件转换完成后，在原始文件夹执行“删除”操作，发现原始PDF不能删除。转换完成后，仍然没释放对文件的控制。"
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void PDFCompress(Form dlg = null, string fileType = null, int index = 0, ListViewItem lv = null)
        {
            outPath = outPath + "_PDF压缩.pdf";
            ((ItemInfomation)lv.Tag).FileFullConvertPath = outPath;
            Aspose.Pdf.Document pdfDoc = null;
            MainInfoOld mainInfo = dlg as MainInfoOld;
            int startRate = 0;

            if (fileType.ToLower() == ".pdf")
            {
                pdfDoc = pdf_doc;
                startRate = 0;
            }

            Aspose.Pdf.Document new_pdf_doc = new Aspose.Pdf.Document();

            int count = pdfDoc.Pages.Count;
            List<int> pageLst = GetPage(lv.SubItems[3].Text, pdfDoc.Pages.Count);
            if (pageLst == null || pageLst.Count < 1) //如果没有"选择页码",则把全部页码全部赋值给它,KongMengyuan注释,2015-12-16
            {
                pageLst = new List<int>();
                for (int i = 1; i <= count; i++)
                {
                    pageLst.Add(i);
                }
            }
            //int total = pageLst.Count;
            int total = count;// KongMengyuan注释,2015-12-15
            if (!MainInfoOld.isReg)
            {
                if (pdfDoc.Pages.Count >= Program.pageFreeConvert)
                {
                    total = Program.pageFreeConvert;
                }
            }
            pdfDoc.Decrypt();
            for (int i = 0, c = 1; i < total; i++, c++)
            {
                if (mainInfo.isClose) break;
                try
                {
                    while (((ItemInfomation)lv.Tag).Status == StatusType.Pause)
                    {
                        Application.DoEvents();
                    }
                    int page = pageLst[i];
                    new_pdf_doc.Pages.Add(pdfDoc.Pages[page]);

                }
                catch
                {
                    continue;
                }
                finally
                {
                    if (startRate == 50)
                    {
                        //mainInfo.UpdateProcess(new TempClass(lv.Index, (c * 50 / total) + 50));
                        //mainInfo.UpdateProcess(new TempClass(lv.Index, 100));
                        mainInfo.UpdateProcess(new TempClass(lv.Index, c * 100 / total));
                    }
                    else
                    {
                        mainInfo.UpdateProcess(new TempClass(lv.Index, c * 100 / total));
                    }
                }
            }
            try
            {
                new_pdf_doc.OptimizeResources();
                new_pdf_doc.Save(outPath);
            }
            catch { }
            finally
            {
                UpdateLstState(lv, mainInfo);
                pdfDoc.FreeMemory();//2015-12-21,KongMengyuan增加,释放内存,否则会出现"当文件转换完成后，在原始文件夹执行“删除”操作，发现原始PDF不能删除。转换完成后，仍然没释放对文件的控制。"
            }
        }

        private void PDFDecode(Form dlg = null, string fileType = null, int index = 0, ListViewItem lv = null)
        {
            outPath = outPath + "_PDF密码解除.pdf";
            ((ItemInfomation)lv.Tag).FileFullConvertPath = outPath;
            Aspose.Pdf.Document pdfDoc = null;
            MainInfoOld mainInfo = dlg as MainInfoOld;
            int startRate = 0;

            if (fileType.ToLower() == ".pdf")
            {
                pdfDoc = pdf_doc;
                startRate = 0;
            }

            Aspose.Pdf.Document new_pdf_doc = new Aspose.Pdf.Document();

            int count = pdfDoc.Pages.Count;
            List<int> pageLst = GetPage(lv.SubItems[3].Text, pdfDoc.Pages.Count);
            if (pageLst == null || pageLst.Count < 1) //如果没有"选择页码",则把全部页码全部赋值给它,KongMengyuan注释,2015-12-16
            {
                pageLst = new List<int>();
                for (int i = 1; i <= count; i++)
                {
                    pageLst.Add(i);
                }
            }
            //int total = pageLst.Count;
            int total = count;// KongMengyuan注释,2015-12-15
            if (!MainInfoOld.isReg)
            {
                if (pdfDoc.Pages.Count >= Program.pageFreeConvert)
                {
                    total = Program.pageFreeConvert;
                }
            }
            pdfDoc.Decrypt();
            for (int i = 0, c = 1; i < total; i++, c++)
            {
                if (mainInfo.isClose) break;
                try
                {
                    while (((ItemInfomation)lv.Tag).Status == StatusType.Pause)
                    {
                        Application.DoEvents();
                    }
                    int page = pageLst[i];
                    new_pdf_doc.Pages.Add(pdfDoc.Pages[page]);
                }
                catch
                {
                    continue;
                }
                finally
                {
                    if (startRate == 50)
                    {
                        //mainInfo.UpdateProcess(new TempClass(lv.Index, (c * 50 / total) + 50));
                        //mainInfo.UpdateProcess(new TempClass(lv.Index, 100));
                        mainInfo.UpdateProcess(new TempClass(lv.Index, c * 100 / total));
                    }
                    else
                    {
                        mainInfo.UpdateProcess(new TempClass(lv.Index, c * 100 / total));
                    }
                }
            }
            try
            {
                new_pdf_doc.Save(outPath);
            }
            catch { }
            finally
            {
                UpdateLstState(lv, mainInfo);
                pdfDoc.FreeMemory();//2015-12-21,KongMengyuan增加,释放内存,否则会出现"当文件转换完成后，在原始文件夹执行“删除”操作，发现原始PDF不能删除。转换完成后，仍然没释放对文件的控制。"
            }
        }

        //KongMengyuan增加,2015-11-10,参考互盾PDFCon
        private void PDFSplitMain(Form dlg = null, string fileType = null, int index = 0, ListViewItem lv = null)
        {
            string outPathbackup = outPath;

            string pageSet = lv.SubItems[3].Text;
            if (pageSet.Contains("，"))
            {
                pageSet = pageSet.Replace("，", ",");
            }
            string[] pages = pageSet.Split(',');
            for (int i = 0; i < pages.Length; i++)
            {
                outPath = outPathbackup + pages[i];
                ListViewItem item = (ListViewItem)lv.Clone();
                item.SubItems[3].Text = pages[i];
                PDFSplit(dlg, fileType, index, item);
            }

            outPath = outPathbackup;
        }

        //KongMengyuan修改,2015-11-10,参考互盾PDFCon
        private void PDFSplit(Form dlg = null, string fileType = null, int index = 0, ListViewItem lv = null)
        {
            //outPath = outPath + "_split.pdf";
            //((ItemInfomation)lv.Tag).FileFullConvertPath = outPath;
            //Aspose.Pdf.Document pdfDoc = null;
            //MainInfo mainInfo = dlg as MainInfo;
            //int startRate = 0;

            //if (fileType == ".pdf" || fileType == ".PDF")
            //{
            //    pdfDoc = pdf_doc;
            //    startRate = 0;
            //}

            //Aspose.Pdf.Document new_pdf_doc = new Aspose.Pdf.Document();
            //MemoryStream ms;

            //int count = pdfDoc.Pages.Count;
            //List<int> pageLst = GetPage(lv.SubItems[3].Text, pdfDoc.Pages.Count);
            //if (pageLst == null || pageLst.Count < 1) //如果没有"选择页码",则把全部页码全部赋值给它,KongMengyuan注释,2015-12-16
            //{
            //    pageLst = new List<int>();
            //    for (int i = 1; i <= count; i++)
            //    {
            //        pageLst.Add(i);
            //    }
            //}
            //int total = pageLst.Count;
            //if (!MainInfoOld.isReg)
            //{
            //    if (pdfDoc.Pages.Count >= 5)
            //    {
            //        total = 5;
            //    }
            //}
            //for (int i = 0, c = 1; i < total; i++, c++)
            //{
            //    if (mainInfo.isClose) break;
            //    try
            //    {
            //        while (((ItemInfomation)lv.Tag).Status == StatusType.Pause)
            //        {
            //            Application.DoEvents();
            //        }
            //        int page = pageLst[i];
            //        //ms = new MemoryStream();
            //        new_pdf_doc.Pages.Add(pdfDoc.Pages[page]);
            //    }
            //    catch
            //    {
            //        continue;
            //    }
            //    finally
            //    {
            //        if (startRate == 50)
            //        {
            //            //mainInfo.UpdateProcess(new TempClass(lv.Index, (c * 50 / total) + 50));
            //            //mainInfo.UpdateProcess(new TempClass(lv.Index, 100));
            //            mainInfo.UpdateProcess(new TempClass(lv.Index, c * 100 / total));
            //        }
            //        else
            //        {
            //            mainInfo.UpdateProcess(new TempClass(lv.Index, c * 100 / total));
            //        }
            //    }
            //}
            //try
            //{
            //    new_pdf_doc.Save(outPath);
            //}
            //catch { }
            //finally
            //{
            //    UpdateLstState(lv, mainInfo);
            //}

            bool convertValid = false; //KongMengyuan增加,2015-11-11,记录当前转换是否有效

            outPath = outPath + "_split.pdf";
            ((ItemInfomation)lv.Tag).FileFullConvertPath = outPath;
            Aspose.Pdf.Document pdfDoc = null;
            MainInfoOld mainInfo = dlg as MainInfoOld;
            int startRate = 0;

            if (fileType.ToLower() == ".pdf")
            {
                pdfDoc = pdf_doc;
                startRate = 0;
            }

            Aspose.Pdf.Document new_pdf_doc = new Aspose.Pdf.Document();
            MemoryStream ms;

            int count = pdfDoc.Pages.Count;
            List<int> pageLst = GetPage(lv.SubItems[3].Text, pdfDoc.Pages.Count);//查看是否有“选择页码”,KongMengyuan注释,2015-11-19
            if (pageLst == null || pageLst.Count < 1) //如果没有"选择页码",则把全部页码全部赋值给它,KongMengyuan注释,2015-12-16
            {
                pageLst = new List<int>();
                for (int i = 1; i <= count; i++)
                {
                    pageLst.Add(i);
                }
            }
            //int total = pageLst.Count;
            int total = count;// KongMengyuan注释,2015-12-15
            if (SetCount == false)
            {
                SetCount = true;
                this.Count = count;
            }
            if (!MainInfoOld.isReg)
            {
                if (pdfDoc.Pages.Count >= Program.pageFreeConvert)
                {
                    total = Program.pageFreeConvert;
                }
            }
            for (int i = 0, c = 1; i < total; i++, c++)
            {
                convertValid = true; //KongMengyuan增加,2015-11-11,记录当前转换是否有效

                if (mainInfo.isClose) break;
                try
                {
                    while (((ItemInfomation)lv.Tag).Status == StatusType.Pause)
                    {
                        Application.DoEvents();
                    }
                    int page = pageLst[i];

                    //KongMengyuan增加,2015-11-11,超出了当前可以转换的最大页数
                    if (page > total)
                    {
                        string file = ((ItemInfomation)mainInfo.lstFile.Items[index].Tag).FileFullPath;
                        string name = getNameAndType(file);
                        //去除扩展名
                        //int filenameStart = name.LastIndexOf(".");
                        //name = name.Substring(0, filenameStart); //只获取纯文件名(不带扩展名)
                        MessageBox.Show("输入的页码已经超过可以转换的最大页数 " + total.ToString() + " ,请检查输入的页码范围" + "\r\n" + "\r\n" + name);  // 字符之间加入回车符
                        convertValid = false; //KongMengyuan增加,2015-11-11,记录当前转换是否有效
                        break;
                    }

                    //ms = new MemoryStream();
                    new_pdf_doc.Pages.Add(pdfDoc.Pages[page]);
                }
                catch
                {
                    continue;
                }
                finally
                {
                    if (startRate == 50)
                    {
                        //mainInfo.UpdateProcess(new TempClass(lv.Index, (c * 50 / total) + 50));
                        //mainInfo.UpdateProcess(new TempClass(lv.Index, 100));
                        mainInfo.UpdateProcess(new TempClass(lv.Index, c * 100 / total));
                    }
                    else
                    {
                        mainInfo.UpdateProcess(new TempClass(lv.Index, c * 100 / total));
                    }
                }
            }
            try
            {
                //new_pdf_doc.Save(outPath);
                if (convertValid) //KongMengyuan增加,2015-11-11,记录当前转换是否有效
                {
                    new_pdf_doc.Save(outPath);
                }
            }
            catch { }
            finally
            {
                UpdateLstState(lv, mainInfo);
                pdfDoc.FreeMemory();//2015-12-21,KongMengyuan增加,释放内存,否则会出现"当文件转换完成后，在原始文件夹执行“删除”操作，发现原始PDF不能删除。转换完成后，仍然没释放对文件的控制。"
            }
        }

        public List<int> GetPage(string pageSet, int count)
        {
            //查看是否有“选择页码”,KongMengyuan注释,2015-11-19
            if (pageSet.Trim() == "")
                return null;
            if (pageSet.Contains("全部"))
                return null;
            //if (!pageSet.Contains("-")) return null;
            List<int> lst = new List<int>();
            try
            {
                if (pageSet.Contains("，"))
                {
                    pageSet = pageSet.Replace("，", ",");
                }
                string[] pages = pageSet.Split(',');
                for (int i = 0; i < pages.Length; i++)
                {
                    if (!pages[i].Contains("-"))
                    {
                        int n = 0;
                        int.TryParse(pages[i], out n);
                        if (n <= count || n > 0)
                        {
                            lst.Add(n);
                        }
                    }
                    else
                    {
                        string[] page2 = pages[i].Split('-');
                        int start = 0;
                        int.TryParse(page2[0], out start);
                        int end = 0;
                        int.TryParse(page2[1], out end);
                        if (start >= end)
                        {
                            int temp;
                            temp = start;
                            start = end;
                            end = temp;
                        }
                        for (int j = start; j <= end; j++)
                        {
                            if (j <= count || j > 0)
                            {
                                lst.Add(j);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return lst;
        }

        private void FileToWordMain(Form dlg = null, string fileType = null, int index = 0, ListViewItem lv = null)
        {
            FileToWord_OneFile(dlg, fileType, index, lv); //所有的分部分转换,最后保存成一个文件,外包人员制作
            //FileToWord_ManyFiles(dlg, fileType, index, lv); //所有的分部分转换,最后保存成不同文件,KongMengyuan制作,2015-11-24
        }

        private void FileToWord_ManyFiles(Form dlg = null, string fileType = null, int index = 0, ListViewItem lv = null)
        {
            //新代码：所有的分部分转换,最后保存成不同文件,KongMengyuan制作,2015-11-24
            string outPathbackup = outPath;

            string pageSet = lv.SubItems[3].Text;
            if (pageSet.Contains("，"))
            {
                pageSet = pageSet.Replace("，", ",");
            }
            string[] pages = pageSet.Split(',');
            for (int i = 0; i < pages.Length; i++)
            {
                if (pages.Length > 1)
                {
                    outPath = outPathbackup + pages[i];
                }
                else
                {
                    outPath = outPathbackup;
                }
                //ListViewItem item = (ListViewItem)lv.Clone();
                ListViewItem item = lv; //此条语句是影响进度条的,如果想显示进度条就不要Clone,KongMengyuan修改,2015-11-26
                item.SubItems[3].Text = pages[i];
                FileToWord_OneFile(dlg, fileType, index, item); //把原代码的分节部分写在后面,就成了分文件保存了,非常容易的修改(但是发现这么写确实花了点时间),KongMengyuan,2015-11-24
            }

            outPath = outPathbackup;
        }

        private void FileToWord_OneFile(Form dlg = null, string fileType = null, int index = 0, ListViewItem lv = null)
        {
            //原始代码：所有的分部分转换,最后保存成一个文件,外包人员制作
            //KongMengyuan注释,2015-11-24,以下文件是把分部分转换的doc保存成一个文件(方法比较好),但是郑侃炜要求把分部分保存成不同的Doc(可能是有客户这么要求)
            outPath = outPath + ".doc";
            ((ItemInfomation)lv.Tag).FileFullConvertPath = outPath;
            Aspose.Pdf.Document pdfDoc = null;
            MainInfoOld mainInfo = dlg as MainInfoOld;
            int startRate = 0;

            if (fileType.ToLower() == ".pdf")
            {
                pdfDoc = pdf_doc;
                startRate = 0;
            }
            else if (fileType.ToLower() == ".ppt" || fileType.ToLower() == ".pptx")
            {
                pdfDoc = PptToPDF(dlg, lv.Index, lv);
                startRate = 50;
            }
            else if (fileType.ToLower() == ".xls" || fileType.ToLower() == ".xlsx")
            {
                pdfDoc = XlsToPDF(dlg, lv.Index, lv);
                startRate = 50;
            }
            Aspose.Words.Document new_word_doc = new Aspose.Words.Document();
            Aspose.Pdf.Document new_pdf_doc = new Aspose.Pdf.Document();
            MemoryStream ms;

            new_word_doc.ChildNodes.Clear();

            int count = pdfDoc.Pages.Count;
            if (count == 0)
            {
                mainInfo.UpdateProcess(new TempClass(lv.Index, 100));
                UpdateLstState(lv, mainInfo);
                string file = ((ItemInfomation)mainInfo.lstFile.Items[index].Tag).FileFullPath;
                string name = getNameAndType(file);
                string message = name + "转换错误！";
                ErrorMessageArgs args = new ErrorMessageArgs(message);
                if (ErrorMessageEvent != null)
                {
                    ErrorMessageEvent(this, args);
                }
                return;
            }
            List<int> pageLst = GetPage(lv.SubItems[3].Text, pdfDoc.Pages.Count);//查看是否有“选择页码”,KongMengyuan注释,2015-11-19
            if (pageLst == null || pageLst.Count < 1) //如果没有"选择页码",则把全部页码全部赋值给它,KongMengyuan注释,2015-12-16
            {
                pageLst = new List<int>();
                for (int i = 1; i <= count; i++)
                {
                    pageLst.Add(i);
                }
            }
            //int total = pageLst.Count;
            int total = count;// KongMengyuan注释,2015-12-15
            if (!MainInfoOld.isReg)
            {
                if (pdfDoc.Pages.Count >= Program.pageFreeConvert)
                {
                    total = Program.pageFreeConvert;
                }
            }
            for (int i = 0, c = 1; i < total; i++, c++)
            {
                if (mainInfo.isClose) break;
                try
                {
                    while (((ItemInfomation)lv.Tag).Status == StatusType.Pause)
                    {
                        Application.DoEvents();
                    }
                    int page = pageLst[i];
                    //KongMengyuan增加,2015-11-11,超出了当前可以转换的最大页数
                    if (page > total)
                    {
                        string file = ((ItemInfomation)mainInfo.lstFile.Items[index].Tag).FileFullPath;
                        string name = getNameAndType(file);
                        //去除扩展名
                        //int filenameStart = name.LastIndexOf(".");
                        //name = name.Substring(0, filenameStart); //只获取纯文件名(不带扩展名)
                        MessageBox.Show("输入的页码已经超过可以转换的最大页数 " + total.ToString() + " ,请检查输入的页码范围" + "\r\n" + "\r\n" + name);  // 字符之间加入回车符
                        //convertValid = false; //KongMengyuan增加,2015-11-11,记录当前转换是否有效
                        break;
                    }

                    //#region 添加图片印章或邮戳到PDF文件里
                    ////添加图片印章或邮戳到PDF文件里,KongMengyuan增加,2015-12-18
                    ////Create image stamp
                    //ImageStamp imageStamp = new ImageStamp("F:\\Work_Hudun\\SVN\\PdfCon\\PDF Convert\\images\\brower_loading1.jpg");
                    //imageStamp.Background = true;
                    //imageStamp.XIndent = 100;
                    //imageStamp.YIndent = 100;
                    //imageStamp.Height = 300;
                    //imageStamp.Width = 300;
                    ////imageStamp.Rotate = Rotation.on270; //旋转角度.不好用,加上它就不能转换了,但这个语句没有问题,奇怪吧
                    //imageStamp.RotateAngle = 35; ; //旋转角度.好用,但是270度就不能用,所以使用时要具体测试一下旋转角度
                    //imageStamp.Opacity = 0.5; //透明属性变化即渐隐效果,0-完全透明,1-完全显示
                    //imageStamp.Quality = 10; //图片质量                    
                    //pdfDoc.Pages[page].AddStamp(imageStamp);//Add stamp to particular page

                    //'set stamp alignment (place stamp on page top, centered horiznotally)
                    //stamp1.VerticalAlignment = Aspose.Pdf.VerticalAlignment.Top
                    //stamp1.HorizontalAlignment = Aspose.Pdf.HorizontalAlignment.Center
                    //' specify the font style as Bold
                    //stamp1.TextState.FontStyle = FontStyles.Bold
                    //' set the text fore ground color information as red
                    //stamp1.TextState.ForegroundColor = Color.Red
                    //' specify the font size as 14
                    //stamp1.TextState.FontSize = 14

                    //' now we need to set the vertical alignment of 2nd stamp object as Top
                    //stamp2.VerticalAlignment = Aspose.Pdf.VerticalAlignment.Top
                    //' set Horizontal alignment information for stamp as Center aligned
                    //stamp2.HorizontalAlignment = Aspose.Pdf.HorizontalAlignment.Center
                    //' set the zooming factor for stamp object
                    //stamp2.Zoom = 10

                    //' set the formatting of 3rd stamp object
                    //' specify the Vertical alignment information for stamp object as TOP
                    //stamp3.VerticalAlignment = Aspose.Pdf.VerticalAlignment.Top
                    //' Set the Horizontal alignment inforamtion for stamp object as Center aligned
                    //stamp3.HorizontalAlignment = Aspose.Pdf.HorizontalAlignment.Center
                    //' set the rotation angle for stamp object
                    //stamp3.RotateAngle = 35
                    //' set pink as background color for stamp
                    //stamp3.TextState.BackgroundColor = Color.Pink
                    //' change the font face information for stamp to Verdana
                    //stamp3.TextState.Font = FontRepository.FindFont("Verdana")
                    //#endregion

                    ms = new MemoryStream();
                    new_pdf_doc.Pages.Add(pdfDoc.Pages[page]);
                    new_pdf_doc.Save(ms, SaveFormat.Doc);
                    new_word_doc.AppendDocument(new Aspose.Words.Document(ms),
                        ImportFormatMode.KeepSourceFormatting);
                    new_pdf_doc.Pages.Delete();
                }
                catch
                {
                    continue;
                }
                finally
                {
                    if (startRate == 50)
                    {
                        mainInfo.UpdateProcess(new TempClass(lv.Index, (c * 50 / total) + 50));
                        //mainInfo.UpdateProcess(new TempClass(lv.Index, 100));
                        //mainInfo.UpdateProcess(new TempClass(lv.Index, c * 100 / total));
                    }
                    else
                    {
                        mainInfo.UpdateProcess(new TempClass(lv.Index, c * 100 / total));
                    }
                }
            }
            try
            {
                new_word_doc.Save(outPath);
            }
            catch { }
            finally
            {
                mainInfo.UpdateProcess(new TempClass(lv.Index, 100));
                UpdateLstState(lv, mainInfo);
            }

            //pdfDoc.FreeMemory();//2015-12-21,KongMengyuan增加,释放内存,否则会出现"当文件转换完成后，在原始文件夹执行“删除”操作，发现原始PDF不能删除。转换完成后，仍然没释放对文件的控制。"
        }

        /// <summary>
        ///更新列表状态
        /// </summary>
        /// <param name="lv"></param>
        /// <param name="mainInfo"></param>
        public void UpdateLstState(ListViewItem lv, MainInfoOld mainInfo)
        {
            ((ItemInfomation)lv.Tag).Status = StatusType.Done;
            mainInfo.diclst.Remove(((ItemInfomation)lv.Tag).FileFullPath);

            if (mainInfo.lstFile.IsAllFinished && mainInfo.fileQueue.Count == 0)
            {
                mainInfo.btnStart.Tag = "EnabledTrue";
                mainInfo.btnStart.BackgroundImage = Properties.Resources.btn_kszh_01;
                mainInfo.lstFile.Invalidate();
            }
        }

        private void FileToIMG(Form dlg = null, string fileType = null, int index = 0, ListViewItem lv = null)
        {
            //outPath = outPath + "IMG\\";
            outPath = outPath + "_IMG\\"; //KongMengyuan修改,2015-11-10,原生成的路径不明显,加一个下划线区分一下
            ((ItemInfomation)lv.Tag).FileFullConvertPath = outPath;
            if (!Directory.Exists(outPath))
            {
                Directory.CreateDirectory(outPath);
            }
            Aspose.Pdf.Document pdfDoc = null;
            MainInfoOld mainInfo = dlg as MainInfoOld;
            int startRate = 0;

            //if (fileType == ".Pdf" || fileType == ".PDF")
            if (fileType.ToLower() == ".pdf")
            {
                pdfDoc = pdf_doc;
                startRate = 0;
            }
            else if (fileType.ToLower() == ".doc" || fileType.ToLower() == ".docx")
            {
                pdfDoc = DocToPDF(dlg, lv.Index, lv);
                startRate = 50;
            }
            else if (fileType.ToLower() == ".ppt" || fileType.ToLower() == ".pptx")
            {
                pdfDoc = PptToPDF(dlg, lv.Index, lv);
                startRate = 50;
            }
            else if (fileType.ToLower() == ".xls" || fileType.ToLower() == ".xlsx")
            {
                pdfDoc = XlsToPDF(dlg, lv.Index, lv);
                startRate = 50;
            }
            Aspose.Pdf.Devices.JpegDevice jpg_device = new Aspose.Pdf.Devices.JpegDevice(new Aspose.Pdf.Devices.Resolution(300), 100);
            mainInfo.UpdateProcess(new TempClass(index, startRate));

            int count = pdfDoc.Pages.Count;
            if (count == 0)
            {
                mainInfo.UpdateProcess(new TempClass(lv.Index, 100));
                UpdateLstState(lv, mainInfo);
                string file = ((ItemInfomation)mainInfo.lstFile.Items[index].Tag).FileFullPath;
                string name = getNameAndType(file);
                string message = name + "转换错误！";
                ErrorMessageArgs args = new ErrorMessageArgs(message);
                if (ErrorMessageEvent != null)
                {
                    ErrorMessageEvent(this, args);
                }
                return;
            }
            List<int> pageLst = GetPage(lv.SubItems[3].Text, pdfDoc.Pages.Count);
            if (pageLst == null || pageLst.Count < 1) //如果没有"选择页码",则把全部页码全部赋值给它,KongMengyuan注释,2015-12-16
            {
                pageLst = new List<int>();
                for (int i = 1; i <= count; i++)
                {
                    pageLst.Add(i);
                }
            }
            //int total = pageLst.Count;
            int total = count;// KongMengyuan注释,2015-12-15
            if (!MainInfoOld.isReg)
            {
                if (pdfDoc.Pages.Count >= Program.pageFreeConvert)
                {
                    total = Program.pageFreeConvert;
                }
            }
            for (int i = 0, c = 1; i < total; i++, c++)
            {
                if (mainInfo.isClose) break;
                try
                {
                    while (((ItemInfomation)lv.Tag).Status == StatusType.Pause)
                    {
                        Application.DoEvents();
                    }
                    int page = pageLst[i];
                    jpg_device.Process(pdfDoc.Pages[page], outPath + page.ToString() + ".jpg");
                    ((ItemInfomation)lv.Tag).FileFullConvertPath = outPath + page.ToString() + ".jpg";
                    if (startRate == 50)
                    {
                        mainInfo.UpdateProcess(new TempClass(lv.Index, (c * 50 / total) + 50));
                        //mainInfo.UpdateProcess(new TempClass(lv.Index, 100));
                        //mainInfo.UpdateProcess(new TempClass(lv.Index, c * 100 / total));
                    }
                    else
                    {
                        //int cur = i * 100 / pdfDoc.Pages.Count;
                        mainInfo.UpdateProcess(new TempClass(lv.Index, c * 100 / total));
                    }
                }
                catch
                {
                    if (startRate == 50)
                    {
                        //mainInfo.UpdateProcess(new TempClass(lv.Index, (c * 50 / total) + 50));
                        mainInfo.UpdateProcess(new TempClass(lv.Index, c * 100 / total));
                    }
                    else
                    {
                        //int cur = i * 100 / pdfDoc.Pages.Count;
                        mainInfo.UpdateProcess(new TempClass(lv.Index, c * 100 / total));
                    }
                    continue;
                }
            }
            UpdateLstState(lv, mainInfo);
            pdfDoc.FreeMemory();//2015-12-21,KongMengyuan增加,释放内存,否则会出现"当文件转换完成后，在原始文件夹执行“删除”操作，发现原始PDF不能删除。转换完成后，仍然没释放对文件的控制。"
        }

        private void FiletToTXT(Form dlg = null, string fileType = null, int index = 0, ListViewItem lv = null)
        {
            outPath = outPath + ".txt";
            ((ItemInfomation)lv.Tag).FileFullConvertPath = outPath;
            Aspose.Pdf.Document pdfDoc = null;
            MainInfoOld mainInfo = dlg as MainInfoOld;
            int startRate = 0;

            //if (fileType == ".pdf" || fileType == ".PDF")
            if (fileType.ToLower() == ".pdf")
            {
                pdfDoc = pdf_doc;
                startRate = 0;
            }
            else if (fileType.ToLower() == ".ppt" || fileType.ToLower() == ".pptx")
            {
                pdfDoc = PptToPDF(dlg, lv.Index, lv);
                startRate = 50;
            }
            else if (fileType.ToLower() == ".xls" || fileType.ToLower() == ".xlsx")
            {
                pdfDoc = XlsToPDF(dlg, lv.Index, lv);
                startRate = 50;
            }
            else if (fileType.ToLower() == ".doc" || fileType.ToLower() == ".docx")
            {
                pdfDoc = DocToPDF(dlg, lv.Index, lv);
                startRate = 50;
            }
            Aspose.Pdf.Facades.PdfExtractor pdf_ex = new Aspose.Pdf.Facades.PdfExtractor(pdfDoc);

            FileStream fs = new FileStream(outPath, FileMode.Create);
            pdf_ex.ExtractTextMode = 0;

            int count = pdfDoc.Pages.Count;
            if (count == 0)
            {
                mainInfo.UpdateProcess(new TempClass(lv.Index, 100));
                UpdateLstState(lv, mainInfo);
                string file = ((ItemInfomation)mainInfo.lstFile.Items[index].Tag).FileFullPath;
                string name = getNameAndType(file);
                string message = name + "转换错误！";
                ErrorMessageArgs args = new ErrorMessageArgs(message);
                if (ErrorMessageEvent != null)
                {
                    ErrorMessageEvent(this, args);
                }
                return;
            }
            List<int> pageLst = GetPage(lv.SubItems[3].Text, pdfDoc.Pages.Count);
            if (pageLst == null || pageLst.Count < 1) //如果没有"选择页码",则把全部页码全部赋值给它,KongMengyuan注释,2015-12-16
            {
                pageLst = new List<int>();
                for (int i = 1; i <= count; i++)
                {
                    pageLst.Add(i);
                }
            }
            //int total = pageLst.Count;
            int total = count;// KongMengyuan注释,2015-12-15
            if (!MainInfoOld.isReg)
            {
                if (pdfDoc.Pages.Count >= Program.pageFreeConvert)
                {
                    total = Program.pageFreeConvert;
                }
            }
            for (int i = 0, c = 1; i < total; i++, c++)
            {
                if (mainInfo.isClose) break;
                try
                {
                    while (((ItemInfomation)lv.Tag).Status == StatusType.Pause)
                    {
                        Application.DoEvents();
                    }
                    int page = pageLst[i];
                    pdf_ex.StartPage = page;
                    pdf_ex.EndPage = page;
                    pdf_ex.ExtractText(Encoding.UTF8);
                    pdf_ex.GetText(fs);
                }
                catch
                {
                    if (startRate == 50)
                    {
                        mainInfo.UpdateProcess(new TempClass(lv.Index, (c * 50 / total) + 50));
                        //mainInfo.UpdateProcess(new TempClass(lv.Index, 100));
                        //mainInfo.UpdateProcess(new TempClass(lv.Index, c * 100 / total));
                    }
                    else
                    {
                        mainInfo.UpdateProcess(new TempClass(lv.Index, c * 100 / total));
                    }
                    continue;
                }
                finally
                {
                    mainInfo.UpdateProcess(new TempClass(lv.Index, c * 100 / total));
                }
                if (startRate == 50)
                {
                    mainInfo.UpdateProcess(new TempClass(lv.Index, (c * 50 / total) + 50));
                    //mainInfo.UpdateProcess(new TempClass(lv.Index, 100));
                    //mainInfo.UpdateProcess(new TempClass(lv.Index, c * 100 / total));
                }
                else
                {
                    mainInfo.UpdateProcess(new TempClass(lv.Index, c * 100 / total));
                }

            }

            fs.Close();
            pdfDoc.FreeMemory();//2015-12-21,KongMengyuan增加,释放内存,否则会出现"当文件转换完成后，在原始文件夹执行“删除”操作，发现原始PDF不能删除。转换完成后，仍然没释放对文件的控制。"
            UpdateLstState(lv, mainInfo);
        }

        private void IMGToPDF(Form dlg = null, string fileType = null, int index = 0, ListViewItem lv = null)
        {
            try
            {
                outPath = outPath + ".pdf";

                MainInfoOld mainInfo = dlg as MainInfoOld;

                //获取用户设置是否合并图片 1合并，0不合并
                //string isMerger = mainInfo.cbIsMerger.Checked ? "1" : "0";
                string isMerger = mainInfo.rbIsMerger1.Checked ? "1" : "0"; //单独到MainInfoOld.Designer.cs里面把 private System.Windows.Forms.RadioButton rbIsMerger1; 改为 public System.Windows.Forms.RadioButton rbIsMerger1;

                if (isMerger == "1")
                {
                    Aspose.Words.Document new_doc = new Aspose.Words.Document();
                    new_doc.RemoveAllChildren();
                    string directoryName = Path.GetDirectoryName(outPath);

                    outPath = directoryName + "\\" + "img转pdf.pdf";

                    ((ItemInfomation)lv.Tag).FileFullConvertPath = directoryName + "\\" + "img转pdf.pdf";

                    int count = mainInfo.lstFile.Items.Count;
                    if (!MainInfoOld.isReg)
                    {
                        if (count >= Program.pageFreeConvert)
                        {
                            count = Program.pageFreeConvert;
                        }
                    }
                    Aspose.Pdf.Generator.Pdf pdfGenerator = new Aspose.Pdf.Generator.Pdf();

                    #region 将所有图片合并到一个PDF中

                    //if (lv.Index == mainInfo.lstFile.Items.Count - 1)
                    //{


                    for (int i = 0; i < count; i++)
                    {
                        try
                        {
                            Aspose.Words.Document doc = new Aspose.Words.Document();
                            Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(doc);
                            string inputFileName = ((ItemInfomation)mainInfo.lstFile.Items[i].Tag).FileFullPath;
                            using (Image image = Image.FromFile(inputFileName))
                            {
                                // Find which dimension the frames in this image represent. For example
                                // the frames of a BMP or TIFF are "page dimension" whereas frames of a GIF image are "time dimension".
                                FrameDimension dimension = new FrameDimension(image.FrameDimensionsList[0]);

                                // Get the number of frames in the image.
                                int framesCount = image.GetFrameCount(dimension);

                                // Get the number of frames in the image.
                                //int framesCount = image.GetFrameCount(FrameDimension.Page);

                                // Loop through all frames.
                                for (int frameIdx = 0; frameIdx < framesCount; frameIdx++)
                                {
                                    // Insert a section break before each new page, in case of a multi-frame TIFF.
                                    if (frameIdx != 0)
                                        builder.InsertBreak(BreakType.SectionBreakNewPage);

                                    // Select active frame.
                                    image.SelectActiveFrame(dimension, frameIdx);

                                    // We want the size of the page to be the same as the size of the image.
                                    // Convert pixels to points to size the page to the actual image size.
                                    PageSetup ps = builder.PageSetup;
                                    ps.PageWidth = ConvertUtil.PixelToPoint(image.Width, image.HorizontalResolution);
                                    ps.PageHeight = ConvertUtil.PixelToPoint(image.Height, image.VerticalResolution);

                                    // Insert the image into the document and position it at the top left corner of the page.
                                    builder.InsertImage(
                                        image,
                                        RelativeHorizontalPosition.Page,
                                        0,
                                        RelativeVerticalPosition.Page,
                                        0,
                                        ps.PageWidth,
                                        ps.PageHeight,
                                        WrapType.None);
                                }
                            }
                            new_doc.AppendDocument(doc, ImportFormatMode.UseDestinationStyles);
                            //Section st = pdfGenerator.Sections.Add();

                            //Aspose.Pdf.Generator.Image img = new Aspose.Pdf.Generator.Image(st);

                            //st.Paragraphs.Add(img);

                            //img.ImageInfo.File = ((ItemInfomation) mainInfo.lstFile.Items[i].Tag).FileFullPath;

                            ////"图片文件(*.jpg,*.gif,*.bmp,*.png,*.tiff)|*.jpg;*.gif;*.bmp;*.png;*.tiff";
                            //switch (fileType.ToLower())
                            //{
                            //    case ".jpeg":
                            //    case ".jpg":
                            //        img.ImageInfo.ImageFileType = ImageFileType.Jpeg;
                            //        break;
                            //    case ".gif":
                            //        img.ImageInfo.ImageFileType = ImageFileType.Gif;
                            //        break;
                            //    case ".bmp":
                            //        img.ImageInfo.ImageFileType = ImageFileType.Bmp;
                            //        break;
                            //    case ".png":
                            //        img.ImageInfo.ImageFileType = ImageFileType.Png;
                            //        break;
                            //    case ".tif":
                            //    case ".tiff":
                            //        img.ImageInfo.ImageFileType = ImageFileType.Tiff;
                            //        break;
                            //    default:
                            //        img.ImageInfo.ImageFileType = ImageFileType.Jpeg;
                            //        break;
                            //}
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }

                    mainInfo.UpdateProcess(new TempClass(lv.Index, 100));

                    //}
                    //else
                    //{
                    //    mainInfo.UpdateProcess(new TempClass(lv.Index, 100));
                    //}

                    #endregion

                    //pdfGenerator.Save(outPath);
                    new_doc.Save(outPath);
                }
                else
                {

                    #region   将单张图片保存到一个PDF中

                    ((ItemInfomation)lv.Tag).FileFullConvertPath = outPath;
                    Aspose.Pdf.Generator.Pdf pdfGenerator = new Aspose.Pdf.Generator.Pdf();


                    Aspose.Pdf.Generator.Section st = pdfGenerator.Sections.Add();

                    Aspose.Pdf.Generator.Image img = new Aspose.Pdf.Generator.Image(st);

                    st.Paragraphs.Add(img);

                    img.ImageInfo.File = ((ItemInfomation)lv.Tag).FileFullPath;

                    //"图片文件(*.jpg,*.gif,*.bmp,*.png,*.tiff)|*.jpg;*.gif;*.bmp;*.png;*.tiff";
                    switch (fileType.ToLower())
                    {
                        case ".jpeg":
                        case ".jpg":

                            img.ImageInfo.ImageFileType = Aspose.Pdf.Generator.ImageFileType.Jpeg;
                            break;
                        case ".gif":

                            img.ImageInfo.ImageFileType = Aspose.Pdf.Generator.ImageFileType.Gif;
                            break;

                        case ".bmp":

                            img.ImageInfo.ImageFileType = Aspose.Pdf.Generator.ImageFileType.Bmp;
                            break;

                        case ".png":

                            img.ImageInfo.ImageFileType = Aspose.Pdf.Generator.ImageFileType.Png;
                            break;
                        case ".tif":
                        case ".tiff":
                            img.ImageInfo.ImageFileType = Aspose.Pdf.Generator.ImageFileType.Tiff;
                            break;

                        default:
                            img.ImageInfo.ImageFileType = Aspose.Pdf.Generator.ImageFileType.Jpeg;

                            break;
                    }
                    pdfGenerator.Save(outPath);
                    mainInfo.UpdateProcess(new TempClass(lv.Index, 100));

                    #endregion
                }

                UpdateLstState(lv, mainInfo);
            }
            catch
            {
                return;
            }
        }

        private void PDFMerge(Form dlg = null, string fileType = null, int index = 0, ListViewItem lv = null)
        {
            ////2015-12-16,KongMengYuan注释,原始代码,以下代码没有"选择页码",但全部合并也是好用的
            //try
            //{
            //    MainInfo mainInfo = dlg as MainInfo;
            //    string directoryName = Path.GetDirectoryName(outPath);
            //    outPath = directoryName + "\\" + "PDF合并";


            //    ((ItemInfomation)lv.Tag).FileFullConvertPath = directoryName + "\\" +
            //                                                    Path.GetFileNameWithoutExtension(
            //                                                        mainInfo.lstFile.Items[
            //                                                            mainInfo.lstFile.Items.Count - 1].SubItems[1]
            //                                                            .Text) + ".pdf";
            //    int count = mainInfo.lstFile.Items.Count;
            //    if (count > 3)
            //    {
            //        for (int i = 0; i < 3; i++)
            //        {
            //            string file = ((ItemInfomation)mainInfo.lstFile.Items[i].Tag).FileFullPath;
            //            outPath = outPath + "_" + getName(file);
            //        }
            //    }
            //    else
            //    {
            //        for (int i = 0; i < count; i++)
            //        {
            //            string file = ((ItemInfomation)mainInfo.lstFile.Items[i].Tag).FileFullPath;
            //            outPath = outPath + "_" + getName(file);
            //        }
            //    }
            //    outPath = outPath + ".pdf";
            //    ((ItemInfomation)lv.Tag).FileFullConvertPath = outPath;
            //    if (!MainInfoOld.isReg)
            //    {
            //        if (count >= Program.pageFreeConvert)
            //        {
            //            count = Program.pageFreeConvert;
            //        }
            //    }

            //    #region 将所有图片合并到一个PDF中

            //    //if (lv.Index == mainInfo.lstFile.Items.Count - 1)
            //    //{
            //    Aspose.Pdf.Document new_pdf_doc = new Aspose.Pdf.Document();
            //    int all = 0;
            //    int con = 0;
            //    for (int i = 0; i < count; i++)
            //    {
            //        string file = ((ItemInfomation)mainInfo.lstFile.Items[i].Tag).FileFullPath;
            //        Aspose.Pdf.Document doc = new Aspose.Pdf.Document(file);
            //        all = all + doc.Pages.Count;
            //    }
            //    for (int i = 0; i < count; i++)
            //    {
            //        try
            //        {
            //            string file = ((ItemInfomation)mainInfo.lstFile.Items[i].Tag).FileFullPath;
            //            Document doc = new Document(file);
            //            for (int j = 1; j <= doc.Pages.Count; j++)
            //            {
            //                try
            //                {
            //                    new_pdf_doc.Pages.Add(doc.Pages[j]);
            //                }
            //                catch (Exception e)
            //                {
            //                    Console.WriteLine(e);
            //                }
            //            }
            //            con = con + doc.Pages.Count;
            //            double num = (double)con / (double)all;
            //            int pro = (int)(num * 100);
            //            mainInfo.UpdateProgress(pro.ToString() + "%");
            //            mainInfo.UpdateProcess(new TempClass(lv.Index, 100));
            //        }
            //        catch (Exception e)
            //        {
            //            Console.WriteLine(e);
            //        }
            //    }
            //    new_pdf_doc.Save(outPath);
            //    mainInfo.UpdateProcess(new TempClass(lv.Index, 100));
            //    //}
            //    //else
            //    //{
            //    //    mainInfo.UpdateProcess(new TempClass(lv.Index, 100));
            //    //}

            //    #endregion


            //    UpdateLstState(lv, mainInfo);
            //    mainInfo.UpdateProgress("100%");
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.StackTrace);
            //}
            //finally
            //{
            //}

            //KongMengyuan重新写的代码,加入"选择页码",两个合并已经好用
            bool convertValid = false; //KongMengyuan增加,2015-11-11,记录当前转换是否有效
            try
            {
                MainInfoOld mainInfo = dlg as MainInfoOld;
                string directoryName = Path.GetDirectoryName(outPath);
                outPath = directoryName + "\\" + "PDF合并";

                ((ItemInfomation)lv.Tag).FileFullConvertPath = directoryName + "\\" +
                                                                Path.GetFileNameWithoutExtension(
                                                                    mainInfo.lstFile.Items[
                                                                        mainInfo.lstFile.Items.Count - 1].SubItems[1]
                                                                        .Text) + ".pdf";
                int count = mainInfo.lstFile.Items.Count;
                if (count > 3)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        string file = ((ItemInfomation)mainInfo.lstFile.Items[i].Tag).FileFullPath;
                        outPath = outPath + "_" + getName(file);
                    }
                }
                else
                {
                    for (int i = 0; i < count; i++)
                    {
                        string file = ((ItemInfomation)mainInfo.lstFile.Items[i].Tag).FileFullPath;
                        outPath = outPath + "_" + getName(file);
                    }
                }
                outPath = outPath + ".pdf";
                ((ItemInfomation)lv.Tag).FileFullConvertPath = outPath;

                if (!MainInfoOld.isReg)
                {
                    if (count >= Program.pageFreeConvert)
                    {
                        count = Program.pageFreeConvert;
                    }
                }

                #region 将所有图片合并到一个PDF中

                Aspose.Pdf.Document new_pdf_doc = new Aspose.Pdf.Document();
                int all = 0;
                int con = 0;
                for (int i = 0; i < count; i++)
                {
                    string file = ((ItemInfomation)mainInfo.lstFile.Items[i].Tag).FileFullPath;
                    Aspose.Pdf.Document doc = new Aspose.Pdf.Document(file);
                    all = all + doc.Pages.Count;
                }
                for (int i = 0; i < count; i++)
                {
                    convertValid = true;
                    try
                    {
                        Aspose.Pdf.Document pdfDoc = null;
                        if (fileType.ToLower() == ".pdf")
                        {
                            string file = ((ItemInfomation)mainInfo.lstFile.Items[i].Tag).FileFullPath;
                            Aspose.Pdf.Document doc = new Aspose.Pdf.Document(file);
                            pdfDoc = doc;
                        }

                        int total = pdfDoc.Pages.Count;

                        List<int> pageLst = GetPage(mainInfo.lstFile.Items[i].SubItems[3].Text, pdfDoc.Pages.Count);//查看是否有“选择页码”,KongMengyuan注释,2015-11-19
                        //if (pageLst == null || pageLst.Count < 1) //如果没有"选择页码",则把全部页码全部赋值给它,不需要这么麻烦,这种编程方法要看情况,KongMengyuan注释,2015-12-16
                        //{
                        //    pageLst = new List<int>();
                        //    for (int k = 1; k <= total; k++)
                        //    {
                        //        pageLst.Add(k);
                        //    }
                        //}

                        err_msg = "";
                        for (int j = 1; j <= pdfDoc.Pages.Count; j++)
                        {
                            try
                            {
                                if (pageLst == null || pageLst.Count < 1) //没有"选择页码",全部赋值,KongMengyuan注释,2015-12-16
                                {
                                    new_pdf_doc.Pages.Add(pdfDoc.Pages[j]);
                                }
                                else
                                {
                                    for (int m = 0; m < pageLst.Count; m++)
                                    {
                                        if (j == pageLst[m])//查看是否有“选择页码”,KongMengyuan注释,2015-11-19
                                        {
                                            new_pdf_doc.Pages.Add(pdfDoc.Pages[j]);
                                            break;
                                        }
                                        //if (pageLst[m] > total)//KongMengyuan增加,2015-12-16,超出了当前可以转换的最大页数
                                        //{
                                        //    //string file1 = ((ItemInfomation)mainInfo.lstFile.Items[index].Tag).FileFullPath;
                                        //    //string name = getNameAndType(file1);                                            
                                        //    //去除扩展名
                                        //    //int filenameStart = name.LastIndexOf(".");
                                        //    //name = name.Substring(0, filenameStart); //只获取纯文件名(不带扩展名)
                                        //    string file1 = ((ItemInfomation)mainInfo.lstFile.Items[i].Tag).FileFullPath;
                                        //    string name = getNameAndType(file1);
                                        //    err_msg = "输入的页码已经超过可以转换的最大页数 " + total.ToString() + " ,请检查输入的页码范围" + "\r\n" + "\r\n" + name;  // 字符之间加入回车符
                                        //    //MessageBox.Show("输入的页码已经超过可以转换的最大页数 " + total.ToString() + " ,请检查输入的页码范围" + "\r\n" + "\r\n" + name);  // 字符之间加入回车符
                                        //    convertValid = false; //KongMengyuan增加,2015-11-11,记录当前转换是否有效
                                        //    break;
                                        //}
                                    }
                                }

                                if (convertValid == false)
                                {
                                    break;
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }
                        }
                        con = con + pdfDoc.Pages.Count;
                        double num = (double)con / (double)all;
                        int pro = (int)(num * 100);
                        mainInfo.UpdateProgress(pro.ToString() + "%");
                        mainInfo.UpdateProcess(new TempClass(lv.Index, 100));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                    //if (err_msg != "")
                    //{
                    //    MessageBox.Show(err_msg); //此处代码已经不起作用了,如果同时有多个文档时,会显示多次提示,一直没有处理好。改进思路: 在MainInfoOld.cs里在comboBoxPage_Validated如果发现输入错误,重置“全部”
                    //    err_msg = "";
                    //}
                }
                new_pdf_doc.Save(outPath);
                mainInfo.UpdateProcess(new TempClass(lv.Index, 100));
                #endregion

                UpdateLstState(lv, mainInfo);
                mainInfo.UpdateProgress("100%");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
            }

        }

        public string getName(string file)
        {
            string name = "";
            try
            {
                int filenameStart = file.LastIndexOf("\\");
                int filemameEnd = file.LastIndexOf(".");
                name = file.Substring(filenameStart + 1, filemameEnd - filenameStart - 1);
            }
            catch (Exception ex)
            {

            }
            return name;
        }

        public string getNameAndType(string file)
        {
            string name = "";
            try
            {
                int filenameStart = file.LastIndexOf("\\");
                name = file.Substring(filenameStart + 1, file.Length - filenameStart - 1);
            }
            catch (Exception ex)
            {

            }
            return name;
        }

        private void pdf_to_html_callback(object outPath)
        {
            try
            {
                if (!MainInfoOld.isReg)
                {

                    if (pdf_doc.Pages.Count > Program.pageFreeConvert)
                    {
                        int[] delete_page;
                        delete_page = new int[pdf_doc.Pages.Count - Program.pageFreeConvert];
                        for (int i = 6, j = 1; i <= pdf_doc.Pages.Count; i++, j++)
                        {
                            delete_page[j - 1] = i;

                        }
                        pdf_doc.Pages.Delete(delete_page);
                    }

                }

                pdf_doc.Save(outPath.ToString(), Aspose.Pdf.SaveFormat.Html);
            }
            catch (Exception ex)
            {

            }
        }

        private void FileToHTML(Form dlg = null, string fileType = null, int index = 0, ListViewItem lv = null)
        {
            ((ItemInfomation)lv.Tag).FileFullConvertPath = outPath + ".html";
            Aspose.Pdf.Document pdfDoc = null;
            MainInfoOld mainInfo = dlg as MainInfoOld;
            int startRate = 0;

            //if (fileType == ".pdf" || fileType == ".PDF")
            if (fileType.ToLower() == ".pdf")            
            {
                pdfDoc = pdf_doc;
                startRate = 0;

            }
            else if (fileType.ToLower() == ".doc" || fileType.ToLower() == ".docx")
            {
                pdf_doc = DocToPDF(dlg, lv.Index, lv);
                startRate = 50;
            }
            else if (fileType.ToLower() == ".ppt" || fileType.ToLower() == ".pptx")
            {
                pdf_doc = PptToPDF(dlg, lv.Index, lv);
                startRate = 50;
            }
            else if (fileType.ToLower() == ".xls" || fileType.ToLower() == ".xlsx")
            {
                pdf_doc = XlsToPDF(dlg, lv.Index, lv);
                startRate = 50;
            }

            Thread cvrt = new Thread(pdf_to_html_callback);
            cvrt.Start(outPath + ".html");
            int cur = 1, old_cur = cur;

            int max = pdf_doc.Pages.Count;
            if (max == 0)
            {
                mainInfo.UpdateProcess(new TempClass(lv.Index, 100));
                UpdateLstState(lv, mainInfo);
                string file = ((ItemInfomation)mainInfo.lstFile.Items[index].Tag).FileFullPath;
                string name = getNameAndType(file);
                string message = name + "转换错误！";
                ErrorMessageArgs args = new ErrorMessageArgs(message);
                if (ErrorMessageEvent != null)
                {
                    ErrorMessageEvent(this, args);
                }
                return;
            }
            if (!MainInfoOld.isReg)
            {
                if (max >= Program.pageFreeConvert)
                {
                    max = Program.pageFreeConvert;
                }
            }

            mainInfo.UpdateProcess(new TempClass(lv.Index, startRate));


            while (true)
            {
                if (mainInfo.isClose) break;
                old_cur = cur;

                /*if (File.Exists(global_config.target_dic + Path.GetFileNameWithoutExtension(file_path) + "_files\\img_" + cur.ToString().PadLeft(2, '0') + ".*"))
                {
                    ++cur;
                }*/
                try
                {

                    if (Directory.GetFiles(outPath + "_files").Length != 0 || Directory.GetFiles(outPath + "_files",
                    "img_" + cur.ToString().PadLeft(2, '0') + ".*").Length != 0)
                    {
                        ++cur;
                    }
                }
                catch
                {

                }

                if (cur == max)
                    break;

                if (startRate == 50)
                {
                    if (old_cur != cur)
                    {
                        mainInfo.UpdateProcess(new TempClass(lv.Index, (cur * 50 / max) + 50));
                        //mainInfo.UpdateProcess(new TempClass(lv.Index, 100));
                        //mainInfo.UpdateProcess(new TempClass(lv.Index, cur * 100 / max));
                    }
                }
                else
                {
                    mainInfo.UpdateProcess(new TempClass(lv.Index, (cur * 100 / max)));
                }

                Thread.Sleep(30);
            }

            try
            {
                bool result = false;

                while (true)
                {
                    if (mainInfo.isClose) break;
                    if (Directory.GetFiles(outPath + "_files",
         "style.css").Length != 0)
                    {
                        result = true;
                        break;
                    }
                }
                if (result)
                {
                    mainInfo.UpdateProcess(new TempClass(lv.Index, 100));
                }

            }
            catch (Exception)
            {
                mainInfo.UpdateProcess(new TempClass(lv.Index, 100));
            }
            mainInfo.UpdateProcess(new TempClass(lv.Index, 100));
            UpdateLstState(lv, mainInfo);
            pdfDoc.FreeMemory();//2015-12-21,KongMengyuan增加,释放内存,否则会出现"当文件转换完成后，在原始文件夹执行“删除”操作，发现原始PDF不能删除。转换完成后，仍然没释放对文件的控制。"
        }

        private void FileToExcel(Form dlg = null, string fileType = null, int index = 0, ListViewItem lv = null)
        {
            try
            {
                outPath = outPath + ".xlsx";
                ItemInfomation info = ((ItemInfomation)lv.Tag);
                info.FileFullConvertPath = outPath;
                Aspose.Pdf.Document pdfDoc = null;
                MainInfoOld mainInfo = dlg as MainInfoOld;
                int startRate = 0;

                if (fileType.ToLower() == ".pdf")
                {
                    pdfDoc = pdf_doc;
                    startRate = 0;

                }
                else if (fileType.ToLower() == ".ppt" || fileType.ToLower() == ".pptx")
                {
                    pdfDoc = PptToPDF(dlg, lv.Index, lv);
                    startRate = 50;
                }
                else if (fileType.ToLower() == ".doc" || fileType.ToLower() == ".docx")
                {
                    pdfDoc = DocToPDF(dlg, lv.Index, lv);
                    startRate = 50;
                }
                Aspose.Cells.Workbook work_book = new Aspose.Cells.Workbook();
                Aspose.Cells.Workbook temp_book;
                MemoryStream ms;
                Aspose.Pdf.Document new_pdf_doc = new Aspose.Pdf.Document();

                work_book.Worksheets.Clear();

                int initial = 1;
                int count = pdfDoc.Pages.Count;
                int total = count;
                List<int> pageLst = GetPage(lv.SubItems[3].Text, pdfDoc.Pages.Count);
                if (pageLst != null && pageLst.Count > 0)
                {
                    initial = pageLst[0];
                    count = pageLst[pageLst.Count - 1];
                    if (count > pdfDoc.Pages.Count) count = pdfDoc.Pages.Count;
                    total = count - initial + 1;
                }
                if (!MainInfoOld.isReg) //文件转Excel,原代码免费版只转换前3页,KongMengyuan修改为转换前5页,2015-11-26
                {
                    //if (pdfDoc.Pages.Count >= 3) //Program.pageFreeConvert
                    if (pdfDoc.Pages.Count >= Program.pageFreeConvert)
                    {
                        count = Program.pageFreeConvert; //Program.pageFreeConvert //count = 3; 
                        initial = 1;
                        total = Program.pageFreeConvert; //Program.pageFreeConvert //total = 3;
                    }
                }
                for (int i = initial, c = 1; i <= count; i++, c++)
                {
                    if (mainInfo.isClose) break;
                    try
                    {
                        while (((ItemInfomation)lv.Tag).Status == StatusType.Pause)
                        {
                            Application.DoEvents();
                        }
                        ms = new MemoryStream();
                        new_pdf_doc.Pages.Add(pdfDoc.Pages[i]);
                        new_pdf_doc.Save(ms, Aspose.Pdf.SaveFormat.Excel);

                        temp_book = new Aspose.Cells.Workbook(ms);
                        work_book.Worksheets.Add(i.ToString());
                        work_book.Worksheets[c - 1].Copy(temp_book.Worksheets[0]);
                        new_pdf_doc.Pages.Delete();
                    }
                    catch (Exception e)
                    {
                        if (!mainInfo.diclst.ContainsKey(info.FileFullPath))
                            return;
                        if (startRate == 50)
                        {
                            mainInfo.UpdateProcess(new TempClass(lv.Index, (c * 50 / total) + 50));
                        }
                        else
                        {
                            int cur = c * 100 / total;
                            mainInfo.UpdateProcess(new TempClass(lv.Index, c * 100 / total));

                        }
                        continue;
                    }
                    if (!mainInfo.diclst.ContainsKey(info.FileFullPath))
                        return;
                    if (startRate == 50)
                    {
                        mainInfo.UpdateProcess(new TempClass(lv.Index, (c * 50 / total) + 50));
                    }
                    else
                    {
                        int cur = c * 100 / total;
                        mainInfo.UpdateProcess(new TempClass(lv.Index, c * 100 / total));

                    }
                }


                work_book.Save(outPath);

                UpdateLstState(lv, mainInfo);
                pdfDoc.FreeMemory();//2015-12-21,KongMengyuan增加,释放内存,否则会出现"当文件转换完成后，在原始文件夹执行“删除”操作，发现原始PDF不能删除。转换完成后，仍然没释放对文件的控制。"
            }
            catch (Exception ex)
            {

            }
            //outPath = outPath + ".xlsx";
            //ItemInfomation info = ((ItemInfomation)lv.Tag);
            //info.FileFullConvertPath = outPath;
            //Aspose.Pdf.Document pdfDoc = null;
            //MainInfo mainInfo = dlg as MainInfo;
            //int startRate = 0;

            //if (fileType == ".pdf" || fileType == ".PDF")
            //{
            //    pdfDoc = pdf_doc;
            //    startRate = 0;

            //}
            //else if (fileType == ".ppt" || fileType == ".pptx")
            //{
            //    pdfDoc = PptToPDF(dlg, lv.Index, lv);
            //    startRate = 50;
            //}
            //else if (fileType == ".doc" || fileType == ".docx")
            //{
            //    pdfDoc = DocToPDF(dlg, lv.Index, lv);
            //    startRate = 50;
            //}
            //Aspose.Cells.Workbook work_book = new Aspose.Cells.Workbook();
            //Aspose.Cells.Workbook temp_book;
            //MemoryStream ms;
            //Aspose.Pdf.Document new_pdf_doc = new Aspose.Pdf.Document();

            //work_book.Worksheets.Clear();

            //int count = pdfDoc.Pages.Count;
            //if (count == 0)
            //{
            //    mainInfo.UpdateProcess(new TempClass(lv.Index, 100));
            //    UpdateLstState(lv, mainInfo);
            //    string file = ((ItemInfomation)mainInfo.lstFile.Items[index].Tag).FileFullPath;
            //    string name = getNameAndType(file);
            //    string message = name + "转换错误！";
            //    ErrorMessageArgs args = new ErrorMessageArgs(message);
            //    if (ErrorMessageEvent != null)
            //    {
            //        ErrorMessageEvent(this, args);
            //    }
            //    return;
            //}
            //List<int> pageLst = GetPage(lv.SubItems[3].Text);
            //if (pageLst == null || pageLst.Count < 1) //如果没有"选择页码",则把全部页码全部赋值给它,KongMengyuan注释,2015-12-16
            //{
            //    pageLst = new List<int>();
            //    for (int i = 1; i <= count; i++)
            //    {
            //        pageLst.Add(i);
            //    }
            //}
            //int total = pageLst.Count;
            //if (!MainInfoOld.isReg)
            //{
            //    if (pdfDoc.Pages.Count >= 5)
            //    {
            //        total = 5;
            //    }
            //}
            //for (int i = 0, c = 1; i < total; i++, c++)
            //{
            //    int page = pageLst[i];
            //    if (mainInfo.isClose) break;
            //    try
            //    {
            //        while (((ItemInfomation)lv.Tag).Status == StatusType.Pause)
            //        {
            //            Application.DoEvents();
            //        }
            //        ms = new MemoryStream();
            //        new_pdf_doc.Pages.Add(pdfDoc.Pages[page]);
            //        new_pdf_doc.Save(ms, Aspose.Pdf.SaveFormat.Excel);

            //        temp_book = new Aspose.Cells.Workbook(ms);
            //        work_book.Worksheets.Add(page.ToString());
            //        work_book.Worksheets[c - 1].Copy(temp_book.Worksheets[0]);
            //        new_pdf_doc.Pages.Delete();
            //    }
            //    catch (Exception e)
            //    {
            //        if (!mainInfo.diclst.ContainsKey(info.FileFullPath))
            //            return;
            //        if (startRate == 50)
            //        {
            //            mainInfo.UpdateProcess(new TempClass(lv.Index, (c * 50 / total) + 50));
            //        }
            //        else
            //        {
            //            int cur = c * 100 / total;
            //            mainInfo.UpdateProcess(new TempClass(lv.Index, c * 100 / total));

            //        }
            //        continue;
            //    }
            //    if (!mainInfo.diclst.ContainsKey(info.FileFullPath))
            //        return;
            //    if (startRate == 50)
            //    {
            //        mainInfo.UpdateProcess(new TempClass(lv.Index, (c * 50 / total) + 50));
            //    }
            //    else
            //    {
            //        int cur = c * 100 / total;
            //        mainInfo.UpdateProcess(new TempClass(lv.Index, c * 100 / total));
            //    }


            //if (mainInfo.isClose) break;
            //try
            //{
            //    while (((ItemInfomation)lv.Tag).Status == StatusType.Pause)
            //    {
            //        Application.DoEvents();
            //    }
            //    int page = pageLst[i];
            //    ms = new MemoryStream();
            //    TextAbsorber textAbsorber = new TextAbsorber();
            //    pdfDoc.Pages[page].Accept(textAbsorber);
            //    string extractedText = textAbsorber.Text;
            //    string txtFile = "extracted_text1.txt";
            //    string pdfFile = "pdf1.pdf";
            //    TextWriter tw = new StreamWriter(txtFile);
            //    tw.WriteLine(extractedText);
            //    tw.Close();
            //    Aspose.Pdf.Generator.Pdf pdf1 = new Aspose.Pdf.Generator.Pdf();
            //    Aspose.Pdf.Generator.Section sec1 = pdf1.Sections.Add();
            //    System.IO.TextReader objReader = new System.IO.StreamReader("extracted_text.txt");
            //    do
            //    {
            //        //Create a new text paragraph & pass text to its constructor as argument
            //        Aspose.Pdf.Generator.Text t2 = new Aspose.Pdf.Generator.Text(objReader.ReadLine());
            //        t2.TextInfo.FontName = "宋体";
            //        // add the text object to paragraphs collection of section
            //        sec1.Paragraphs.Add(t2);
            //        // Read till the end of file

            //    } while (objReader.Peek() != -1);
            //    // Close the StreamReader object
            //    objReader.Close();
            //    pdf1.SetUnicode();
            //    string file = System.Environment.CurrentDirectory + "\\" + pdfFile;
            //    pdf1.Save(file);
            //    Aspose.Pdf.Document pdf2 = new Aspose.Pdf.Document(file);
            //    for (int j = 1; j <= pdf2.Pages.Count; j++)
            //    {
            //        TextAbsorber textAb = new TextAbsorber();
            //        pdf2.Pages[j].Accept(textAbsorber);
            //        string extrext = textAb.Text;
            //        new_pdf_doc.Pages.Add(pdf2.Pages[j]);
            //    }

            //    new_pdf_doc.Save(ms, Aspose.Pdf.SaveFormat.Excel);

            //    temp_book = new Aspose.Cells.Workbook(ms);
            //    work_book.Worksheets.Add(page.ToString());
            //    work_book.Worksheets[c - 1].Copy(temp_book.Worksheets[0]);
            //    new_pdf_doc.Pages.Delete();
            //}
            //catch (Exception e)
            //{
            //    if (!mainInfo.diclst.ContainsKey(info.FileFullPath))
            //        return;
            //    if (startRate == 50)
            //    {
            //        mainInfo.UpdateProcess(new TempClass(lv.Index, (c * 50 / total) + 50));
            //        //mainInfo.UpdateProcess(new TempClass(lv.Index, 100));
            //        //mainInfo.UpdateProcess(new TempClass(lv.Index, c * 100 / total));
            //    }
            //    else
            //    {
            //        int cur = c * 100 / total;
            //        mainInfo.UpdateProcess(new TempClass(lv.Index, c * 100 / total));
            //        //mainInfo.UpdateProcess(new TempClass(lv.Index, 100));
            //    }
            //    continue;
            //}
            //if (!mainInfo.diclst.ContainsKey(info.FileFullPath))
            //    return;
            //if (startRate == 50)
            //{
            //    mainInfo.UpdateProcess(new TempClass(lv.Index, (c * 50 / total) + 50));
            //    //mainInfo.UpdateProcess(new TempClass(lv.Index, 100));
            //    //mainInfo.UpdateProcess(new TempClass(lv.Index, c * 100 / total));
            //}
            //else
            //{
            //    int cur = c * 100 / total;
            //    mainInfo.UpdateProcess(new TempClass(lv.Index, c * 100 / total));
            //    //mainInfo.UpdateProcess(new TempClass(lv.Index, 100));
            //}
            ////mainInfo.UpdateProcess(new TempClass(lv.Index, c * 100 / total));

            //mainInfo.UpdateProcess(new TempClass(lv.Index, 100));
            //work_book.Save(outPath);
            //UpdateLstState(lv, mainInfo);
        }

        /// <summary>
        /// 返回指示文件是否已被其它程序使用的布尔值
        /// </summary>
        /// <param name="fileFullName">文件的完全限定名，例如：“C:\MyFile.txt”。</param>
        /// <returns>如果文件已被其它程序使用，则为 true；否则为 false。</returns>
        public static Boolean FileIsUsed(String fileFullName)
        {
            Boolean result = false;
            //判断文件是否存在，如果不存在，直接返回 false
            if (!System.IO.File.Exists(fileFullName))
            {
                result = false;
            }//end: 如果文件不存在的处理逻辑
            else
            {//如果文件存在，则继续判断文件是否已被其它程序使用
                //逻辑：尝试执行打开文件的操作，如果文件已经被其它程序使用，则打开失败，抛出异常，根据此类异常可以判断文件是否已被其它程序使用。
                System.IO.FileStream fileStream = null;
                try
                {
                    fileStream = System.IO.File.Open(fileFullName, System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite, System.IO.FileShare.None);
                    if (fileStream != null)
                    {
                        fileStream.Close();
                    }
                    result = false;
                }
                catch (System.IO.IOException ioEx)
                {
                    if (fileStream != null)
                    {
                        fileStream.Close();
                    }
                    result = true;
                }
                catch (System.Exception ex)
                {
                    if (fileStream != null)
                    {
                        fileStream.Close();
                    }
                    result = true;
                }
            }//end: 如果文件存在的处理逻辑
            //返回指示文件是否已被其它程序使用的值
            return result;
        }//end method FileIsUsed

        private void FileToPPT(Form dlg = null, string fileType = null, int index = 0, ListViewItem lv = null)
        {
            outPath = outPath + ".ppt";
            ((ItemInfomation)lv.Tag).FileFullConvertPath = outPath;
            Aspose.Pdf.Document pdfDoc = null;
            MainInfoOld mainInfo = dlg as MainInfoOld;
            int startRate = 0;

            if (fileType.ToLower() == ".pdf")
            {
                pdfDoc = pdf_doc;
                startRate = 0;

            }
            else if (fileType.ToLower() == ".xls" || fileType.ToLower() == ".xlsx")
            {
                pdfDoc = XlsToPDF(dlg, lv.Index, lv);
                startRate = 50;
            }
            else if (fileType.ToLower() == ".doc" || fileType.ToLower() == ".docx")
            {
                pdfDoc = DocToPDF(dlg, lv.Index, lv);
                startRate = 50;
            }
            Aspose.Slides.Presentation pres = new Aspose.Slides.Presentation();
            Aspose.Pdf.Devices.JpegDevice jpg_device = new Aspose.Pdf.Devices.JpegDevice(new Aspose.Pdf.Devices.Resolution(300), 100);
            Aspose.Slides.IPPImage img_ex;
            //pres.Slides.RemoveAt(0);
            MemoryStream ms;
            int sy;

            int count = pdfDoc.Pages.Count;
            if (count == 0)
            {
                mainInfo.UpdateProcess(new TempClass(lv.Index, 100));
                UpdateLstState(lv, mainInfo);
                string file = ((ItemInfomation)mainInfo.lstFile.Items[index].Tag).FileFullPath;
                string name = getNameAndType(file);
                string message = name + "转换错误！";
                ErrorMessageArgs args = new ErrorMessageArgs(message);
                if (ErrorMessageEvent != null)
                {
                    ErrorMessageEvent(this, args);
                }
                return;
            }
            List<int> pageLst = GetPage(lv.SubItems[3].Text, pdfDoc.Pages.Count);
            if (pageLst == null || pageLst.Count < 1) //如果没有"选择页码",则把全部页码全部赋值给它,KongMengyuan注释,2015-12-16
            {
                pageLst = new List<int>();
                for (int i = 1; i <= count; i++)
                {
                    pageLst.Add(i);
                }
            }
            //int total = pageLst.Count;
            int total = count;// KongMengyuan注释,2015-12-15
            if (!MainInfoOld.isReg)
            {
                if (pdfDoc.Pages.Count >= Program.pageFreeConvert)
                {
                    total = Program.pageFreeConvert;
                }
            }
            for (int i = 0, c = 1; i < total; i++, c++)
            {
                if (mainInfo.isClose) break;
                try
                {
                    while (((ItemInfomation)lv.Tag).Status == StatusType.Pause)
                    {
                        Application.DoEvents();
                    }
                    int page = pageLst[i];
                    pres.Slides.AddEmptySlide(pres.LayoutSlides[0]);
                    pres.Slides[c].Shapes.AddAutoShape(Aspose.Slides.ShapeType.Rectangle, 10, 20, System.Convert.ToSingle(mainInfo.txtWidth.Text)
                        , System.Convert.ToSingle(mainInfo.txtHeight.Text));
                    int scount = pres.Slides[c].Shapes.Count;
                    sy = scount - 1;
                    pres.Slides[c].Shapes[sy].FillFormat.FillType = Aspose.Slides.FillType.Picture;
                    pres.Slides[c].Shapes[sy].FillFormat.PictureFillFormat.PictureFillMode = Aspose.Slides.PictureFillMode.Stretch;
                    ms = new MemoryStream();
                    jpg_device.Process(pdfDoc.Pages[page], ms);
                    img_ex = pres.Images.AddImage(new Bitmap(ms));
                    pres.Slides[c].Shapes[sy].FillFormat.PictureFillFormat.Picture.Image = img_ex;
                }
                catch
                {
                    if (startRate == 50)
                    {
                        mainInfo.UpdateProcess(new TempClass(lv.Index, (c * 50 / total) + 50));
                        //mainInfo.UpdateProcess(new TempClass(lv.Index, 100));
                        //mainInfo.UpdateProcess(new TempClass(lv.Index, c * 100 / total));
                    }
                    else
                    {
                        mainInfo.UpdateProcess(new TempClass(lv.Index, c * 100 / total));

                    } continue;
                }
                if (startRate == 50)
                {
                    mainInfo.UpdateProcess(new TempClass(lv.Index, (c * 50 / total) + 50));
                    //mainInfo.UpdateProcess(new TempClass(lv.Index, 100));
                    //mainInfo.UpdateProcess(new TempClass(lv.Index, c * 100 / total));
                }
                else
                {
                    mainInfo.UpdateProcess(new TempClass(lv.Index, c * 100 / total));
                }
            }

            try
            {
                pres.Slides.RemoveAt(0);
                pres.Save(outPath, Aspose.Slides.Export.SaveFormat.Ppt);
            }
            catch { }
            mainInfo.UpdateProcess(new TempClass(lv.Index, 100));
            UpdateLstState(lv, mainInfo);
            pdfDoc.FreeMemory();//2015-12-21,KongMengyuan增加,释放内存,否则会出现"当文件转换完成后，在原始文件夹执行“删除”操作，发现原始PDF不能删除。转换完成后，仍然没释放对文件的控制。"
        }

        private Aspose.Pdf.Document DocToPDF(Form dlg = null, int index = 0, ListViewItem lv = null)
        {
            //if (word_doc == null) return null;
            //int EndRate = 50;
            //MainInfo mainInfo = dlg as MainInfo;
            //string file = ((ItemInfomation)mainInfo.lstFile.Items[index].Tag).FileFullPath;
            //Aspose.Words.Document new_word_doc;
            //Aspose.Pdf.Document new_pdf_doc = new Aspose.Pdf.Document();
            //int count = word_doc.PageCount;
            //if (count == 0)
            //{
            //    mainInfo.UpdateProcess(new TempClass(lv.Index, 100));
            //    UpdateLstState(lv, mainInfo);
            //    string fileAndType = ((ItemInfomation)mainInfo.lstFile.Items[index].Tag).FileFullPath;
            //    string name = getNameAndType(fileAndType);
            //    string message = name + "转换错误！";
            //    ErrorMessageArgs args = new ErrorMessageArgs(message);
            //    if (ErrorMessageEvent != null)
            //    {
            //        ErrorMessageEvent(this, args);
            //    }
            //    return new_pdf_doc;
            //}
            //Console.WriteLine("Page:" + file + "," + count);
            //List<int> pageLst = GetPage(lv.SubItems[3].Text, word_doc.PageCount);
            //if (pageLst == null || pageLst.Count < 1) //如果没有"选择页码",则把全部页码全部赋值给它,KongMengyuan注释,2015-12-16
            //{
            //    pageLst = new List<int>();
            //    for (int i = 1; i <= count; i++)
            //    {
            //        pageLst.Add(i);
            //    }
            //}
            //int total = pageLst.Count;
            //if (!MainInfoOld.isReg)
            //{
            //    if (word_doc.PageCount >= 5)
            //    {
            //        total = 5;
            //    }
            //}

            //if (targetFormat == FORMAT.DOC2PDF)
            //{
            //    outPath = outPath + ".pdf";
            //    EndRate = 100;
            //    ((ItemInfomation)lv.Tag).FileFullConvertPath = outPath;
            //}            

            //try
            //{
            //    MemoryStream ms;
            //    new_pdf_doc.Pages.Delete();
            //    for (int i = 0, c = 1; i < total; i++, c++)
            //    {
            //        if (mainInfo.isClose) break;
            //        try
            //        {
            //            while (((ItemInfomation) lv.Tag).Status == StatusType.Pause)
            //            {
            //                Application.DoEvents();
            //            }
            //            int page = pageLst[i] - 1;
            //            ms = new MemoryStream();
            //            new_word_doc = new Aspose.Words.Document();
            //            new_word_doc.ChildNodes.RemoveAt(0);
            //            new_word_doc.AppendChild(new_word_doc.ImportNode(word_doc.ChildNodes[page], true));

            //            new_word_doc.Save(ms, Aspose.Words.SaveFormat.Pdf);
            //            new_pdf_doc.Pages.Add((new Aspose.Pdf.Document(ms)).Pages);
            //            int cur = c*EndRate/total;
            //            mainInfo.UpdateProcess(new TempClass(lv.Index, cur));
            //        }
            //        catch(Exception ex)
            //        {
            //            string s = ex.StackTrace;
            //            Console.WriteLine("Convert:" + file + "," + c);
            //            mainInfo.UpdateProcess(new TempClass(lv.Index, c * EndRate / total));
            //        }
            //    }
            //    if (targetFormat == FORMAT.DOC2PDF)
            //    {
            //        Console.WriteLine("Save:" + file);
            //        new_pdf_doc.Save(outPath);
            //        UpdateLstState(lv, mainInfo);
            //    }
            //}
            //catch (Exception)
            //{

            //    return null;
            //}
            //return new_pdf_doc;

            //KongMengyuan修改,2015-11-03,参考互盾PDFCon,软件未注册用户仅能转换前5页，好像有时会失效(比如170页的Word文档会转成170页的PDF,也没有提示)。比如：word转pdf功能
            if (word_doc == null) return null;
            int EndRate = 50;
            MainInfoOld mainInfo = dlg as MainInfoOld;
            string file = ((ItemInfomation)mainInfo.lstFile.Items[index].Tag).FileFullPath;
            Aspose.Words.Document new_word_doc;
            Aspose.Pdf.Document new_pdf_doc = new Aspose.Pdf.Document();
            int count = word_doc.PageCount;
            if (SetCount == false)
            {
                SetCount = true;
                this.Count = count;
            }
            if (count == 0)
            {
                mainInfo.UpdateProcess(new TempClass(lv.Index, 100));
                UpdateLstState(lv, mainInfo);
                string fileAndType = ((ItemInfomation)mainInfo.lstFile.Items[index].Tag).FileFullPath;
                string name = getNameAndType(fileAndType);
                string message = name + "转换错误！";
                ErrorMessageArgs args = new ErrorMessageArgs(message);
                if (ErrorMessageEvent != null)
                {
                    ErrorMessageEvent(this, args);
                }
                return new_pdf_doc;
            }
            Console.WriteLine("Page:" + file + "," + count);
            List<int> pageLst = GetPage(lv.SubItems[3].Text, word_doc.PageCount);
            if (pageLst == null || pageLst.Count < 1) //如果没有"选择页码",则把全部页码全部赋值给它,KongMengyuan注释,2015-12-16
            {
                pageLst = new List<int>();
                for (int i = 1; i <= count; i++)
                {
                    pageLst.Add(i);
                }
            }
            //int total = pageLst.Count;
            int total = count;// KongMengyuan注释,2015-12-15
            if (!MainInfoOld.isReg)
            {
                if (word_doc.PageCount >= Program.pageFreeConvert)
                {
                    total = Program.pageFreeConvert;
                }
            }

            if (targetFormat == FORMAT.DOC2PDF)
            {
                outPath = outPath + ".pdf";
                EndRate = 100;
                ((ItemInfomation)lv.Tag).FileFullConvertPath = outPath;
            }

            try
            {
                MemoryStream ms;
                new_pdf_doc.Pages.Delete();
                for (int i = 0, c = 1; i < total; i++, c++)
                {
                    try
                    {
                        //while (((ItemInfomation) lv.Tag).Status == StatusType.Pause)
                        //{
                        //    Application.DoEvents();
                        //}
                        int page = pageLst[i] - 1;
                        ms = new MemoryStream();
                        new_word_doc = new Aspose.Words.Document();
                        new_word_doc.ChildNodes.RemoveAt(0);
                        new_word_doc.AppendChild(new_word_doc.ImportNode(word_doc.ChildNodes[page], true));

                        new_word_doc.Save(ms, Aspose.Words.SaveFormat.Pdf);
                        new_pdf_doc.Pages.Add((new Aspose.Pdf.Document(ms)).Pages);
                        int cur = c * EndRate / total;
                        mainInfo.UpdateProcess(new TempClass(lv.Index, cur));
                    }
                    catch (Exception ex)
                    {
                        string s = ex.StackTrace;
                        Console.WriteLine("Convert:" + file + "," + c);
                        mainInfo.UpdateProcess(new TempClass(lv.Index, c * EndRate / total));
                    }
                }
                if (targetFormat == FORMAT.DOC2PDF)
                {
                    Console.WriteLine("Save:" + file);
                    //
                    while (new_pdf_doc.Pages.Count > total)
                    {
                        new_pdf_doc.Pages.Delete(total + 1);
                    }
                    new_pdf_doc.Save(outPath);
                    UpdateLstState(lv, mainInfo);
                }
            }
            catch (Exception)
            {

                return null;
            }
            return new_pdf_doc;
        }

        private Aspose.Pdf.Document XlsToPDF(Form dlg = null, int index = 0, ListViewItem lv = null)
        {
            if (excel_doc == null) return null;
            int EndRate = 50;
            int initial = 1;
            int count = excel_doc.Worksheets.Count;
            int total = count;
            if (targetFormat == FORMAT.Excel2PDF)
            {
                List<int> pageLst = GetPage(lv.SubItems[3].Text, excel_doc.Worksheets.Count);
                if (pageLst != null && pageLst.Count > 0)
                {
                    initial = pageLst[0];
                    count = pageLst[pageLst.Count - 1];
                    if (count > excel_doc.Worksheets.Count) count = excel_doc.Worksheets.Count;
                    total = count - initial + 1;

                }
                outPath = outPath + ".pdf";
                EndRate = 100;
                ((ItemInfomation)lv.Tag).FileFullConvertPath = outPath;
            }
            Aspose.Pdf.Document new_pdf_doc = null;
            try
            {
                Aspose.Cells.Workbook new_xls_doc = new Aspose.Cells.Workbook();
                MainInfoOld mainInfo = dlg as MainInfoOld;
                MemoryStream ms;

                new_xls_doc.Worksheets.Clear();

                new_pdf_doc = new Aspose.Pdf.Document();

                if (!MainInfoOld.isReg)
                {
                    if (excel_doc.Worksheets.Count >= Program.pageFreeConvert)
                    {
                        count = Program.pageFreeConvert;
                        initial = 1;
                        total = Program.pageFreeConvert;
                    }
                }
                for (int i = initial - 1, c = 1; i < count; i++, c++)
                {
                    if (mainInfo.isClose) break;
                    try
                    {
                        while (((ItemInfomation)lv.Tag).Status == StatusType.Pause)
                        {
                            Application.DoEvents();
                        }
                        ms = new MemoryStream();
                        new_xls_doc.Worksheets.Add(i.ToString());
                        new_xls_doc.Worksheets[0].Copy(excel_doc.Worksheets[i]);
                        new_xls_doc.Save(ms, Aspose.Cells.SaveFormat.Pdf);
                        new_pdf_doc.Pages.Add(new Aspose.Pdf.Document(ms).Pages);
                        new_xls_doc.Worksheets.RemoveAt(0);
                        int cur = c * EndRate / total;
                        mainInfo.UpdateProcess(new TempClass(lv.Index, cur));
                    }
                    catch { mainInfo.UpdateProcess(new TempClass(lv.Index, c * EndRate / total)); continue; }

                }
                if (targetFormat == FORMAT.Excel2PDF)
                {
                    new_pdf_doc.Save(outPath);
                    UpdateLstState(lv, mainInfo);
                }

            }
            catch (Exception)
            {
                return null;
            }
            return new_pdf_doc;

        }

        private Aspose.Pdf.Document PptToPDF(Form dlg = null, int index = 0, ListViewItem lv = null)
        {
            //if (ppt_doc == null) return null;
            //int EndRate = 50;
            //int count = ppt_doc.Slides.Count;
            //MainInfo mainInfo = dlg as MainInfo;
            //Aspose.Slides.Presentation new_ppt_doc = new Aspose.Slides.Presentation();
            //Aspose.Pdf.Document new_pdf_doc = new Aspose.Pdf.Document();
            //if (count == 0)
            //{
            //    mainInfo.UpdateProcess(new TempClass(lv.Index, 100));
            //    UpdateLstState(lv, mainInfo);
            //    string file = ((ItemInfomation)mainInfo.lstFile.Items[index].Tag).FileFullPath;
            //    string name = getNameAndType(file);
            //    string message = name + "转换错误！";
            //    ErrorMessageArgs args = new ErrorMessageArgs(message);
            //    if (ErrorMessageEvent != null)
            //    {
            //        ErrorMessageEvent(this, args);
            //    }
            //    return new_pdf_doc;
            //}
            //List<int> pageLst = GetPage(lv.SubItems[3].Text, ppt_doc.Slides.Count);
            //if (pageLst == null || pageLst.Count < 1) //如果没有"选择页码",则把全部页码全部赋值给它,KongMengyuan注释,2015-12-16
            //{
            //    pageLst = new List<int>();
            //    for (int i = 0; i < count; i++)
            //    {
            //        pageLst.Add(i);
            //    }
            //}
            //int total = pageLst.Count;
            //if (!MainInfoOld.isReg)
            //{
            //    if (ppt_doc.Slides.Count >= 5)
            //    {
            //        total = 5;
            //    }
            //}

            //if (targetFormat == FORMAT.PPT2PDF)
            //{
            //    outPath = outPath + ".pdf";
            //    EndRate = 100;
            //    ((ItemInfomation)lv.Tag).FileFullConvertPath = outPath;
            //}


            //try
            //{
            //    MemoryStream ms;
            //    new_ppt_doc.Slides.RemoveAt(0);
            //    for (int i = 0, c = 1; i < total; i++, c++)
            //    {
            //        if (mainInfo.isClose) break;
            //        try
            //        {
            //            while (((ItemInfomation)lv.Tag).Status == StatusType.Pause)
            //            {
            //                Application.DoEvents();
            //            }
            //            ms = new MemoryStream();
            //            int page = pageLst[i];
            //            new_ppt_doc.Slides.AddClone(ppt_doc.Slides[page]);
            //            new_ppt_doc.Save(ms, Aspose.Slides.Export.SaveFormat.Pdf);
            //            new_ppt_doc.Slides.RemoveAt(0);
            //            new_pdf_doc.Pages.Add(new Aspose.Pdf.Document(ms).Pages);

            //            mainInfo.UpdateProcess(new TempClass(lv.Index, c * EndRate / total));
            //        }
            //        catch { mainInfo.UpdateProcess(new TempClass(lv.Index, c * EndRate / total)); continue; }
            //    }

            //    if (targetFormat == FORMAT.PPT2PDF)
            //    {
            //        new_pdf_doc.Save(outPath);
            //        UpdateLstState(lv, mainInfo);
            //    }
            //}
            //catch (Exception)
            //{

            //    return null;
            //}
            //return new_pdf_doc;

            //KongMengyuan修改,2015-11-10,参考互盾PDFCon
            if (ppt_doc == null) return null;
            int EndRate = 50;
            int count = ppt_doc.Slides.Count;
            MainInfoOld mainInfo = dlg as MainInfoOld;
            Aspose.Slides.Presentation new_ppt_doc = new Aspose.Slides.Presentation();
            Aspose.Pdf.Document new_pdf_doc = new Aspose.Pdf.Document();
            if (count == 0)
            {
                mainInfo.UpdateProcess(new TempClass(lv.Index, 100));
                UpdateLstState(lv, mainInfo);
                string file = ((ItemInfomation)mainInfo.lstFile.Items[index].Tag).FileFullPath;
                string name = getNameAndType(file);
                string message = name + "转换错误！";
                ErrorMessageArgs args = new ErrorMessageArgs(message);
                if (ErrorMessageEvent != null)
                {
                    ErrorMessageEvent(this, args);
                }
                return new_pdf_doc;
            }
            List<int> pageLst = GetPage(lv.SubItems[3].Text, ppt_doc.Slides.Count);
            if (pageLst == null || pageLst.Count < 1) //如果没有"选择页码",则把全部页码全部赋值给它,KongMengyuan注释,2015-12-16
            {
                pageLst = new List<int>();
                for (int i = 0; i < count; i++)
                {
                    pageLst.Add(i);
                }
            }
            //int total = pageLst.Count;
            int total = count;// KongMengyuan注释,2015-12-15
            if (SetCount == false)
            {
                SetCount = true;
                this.Count = total;
            }
            if (!MainInfoOld.isReg)
            {
                if (ppt_doc.Slides.Count >= Program.pageFreeConvert)
                {
                    total = Program.pageFreeConvert;
                }
            }

            if (targetFormat == FORMAT.PPT2PDF)
            {
                outPath = outPath + ".pdf";
                EndRate = 100;
                ((ItemInfomation)lv.Tag).FileFullConvertPath = outPath;
            }


            try
            {
                MemoryStream ms;
                new_ppt_doc.Slides.RemoveAt(0);
                for (int i = 0, c = 1; i < total; i++, c++)
                {
                    if (mainInfo.isClose) break;
                    try
                    {
                        while (((ItemInfomation)lv.Tag).Status == StatusType.Pause)
                        {
                            Application.DoEvents();
                        }
                        ms = new MemoryStream();
                        int page = pageLst[i];
                        new_ppt_doc.Slides.AddClone(ppt_doc.Slides[page]);
                        new_ppt_doc.Save(ms, Aspose.Slides.Export.SaveFormat.Pdf);
                        new_ppt_doc.Slides.RemoveAt(0);
                        new_pdf_doc.Pages.Add(new Aspose.Pdf.Document(ms).Pages);

                        mainInfo.UpdateProcess(new TempClass(lv.Index, c * EndRate / total));
                    }
                    catch { mainInfo.UpdateProcess(new TempClass(lv.Index, c * EndRate / total)); continue; }
                }

                if (targetFormat == FORMAT.PPT2PDF)
                {
                    new_pdf_doc.Save(outPath);
                    UpdateLstState(lv, mainInfo);
                }
            }
            catch (Exception)
            {

                return null;
            }
            return new_pdf_doc;

        }

        public bool Can_work()
        {
            return file_can_work;
        }

        public void Close()
        {
            try
            {
                if (fileStream != null)
                    fileStream.Close();
            }
            catch
            {

            }
        }

        public string Get_err_msg()
        {
            return err_msg;
        }
    }
}
