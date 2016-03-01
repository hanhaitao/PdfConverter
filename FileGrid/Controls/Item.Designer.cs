namespace Controls
{
    partial class Item
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Item));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblFileName = new System.Windows.Forms.Label();
            this.picType = new System.Windows.Forms.PictureBox();
            this.selectPanel = new System.Windows.Forms.Panel();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.panelStatus = new System.Windows.Forms.Panel();
            this.picStatus = new System.Windows.Forms.PictureBox();
            this.panelOperate = new System.Windows.Forms.Panel();
            this.picDelete = new System.Windows.Forms.PictureBox();
            this.picOpenPath = new System.Windows.Forms.PictureBox();
            this.lblOpen = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picType)).BeginInit();
            this.selectPanel.SuspendLayout();
            this.panelStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picStatus)).BeginInit();
            this.panelOperate.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picDelete)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picOpenPath)).BeginInit();
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "delete.png");
            this.imageList1.Images.SetKeyName(1, "excel.png");
            this.imageList1.Images.SetKeyName(2, "image.png");
            this.imageList1.Images.SetKeyName(3, "path.png");
            this.imageList1.Images.SetKeyName(4, "pause.png");
            this.imageList1.Images.SetKeyName(5, "PDF.ico");
            this.imageList1.Images.SetKeyName(6, "PPT.png");
            this.imageList1.Images.SetKeyName(7, "start.png");
            this.imageList1.Images.SetKeyName(8, "stop.png");
            this.imageList1.Images.SetKeyName(9, "TXT.png");
            this.imageList1.Images.SetKeyName(10, "word.png");
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblFileName);
            this.panel1.Controls.Add(this.picType);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(300, 35);
            this.panel1.TabIndex = 2;
            // 
            // lblFileName
            // 
            this.lblFileName.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblFileName.Font = new System.Drawing.Font("宋体", 12F);
            this.lblFileName.Location = new System.Drawing.Point(44, 0);
            this.lblFileName.Name = "lblFileName";
            this.lblFileName.Size = new System.Drawing.Size(256, 35);
            this.lblFileName.TabIndex = 2;
            this.lblFileName.Text = "全世界都在一路向北.pdf";
            this.lblFileName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblFileName.Paint += new System.Windows.Forms.PaintEventHandler(this.lblNo_Paint);
            // 
            // picType
            // 
            this.picType.Image = global::Controls.Properties.Resources.picpdf;
            this.picType.Location = new System.Drawing.Point(10, 5);
            this.picType.Name = "picType";
            this.picType.Size = new System.Drawing.Size(24, 24);
            this.picType.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picType.TabIndex = 1;
            this.picType.TabStop = false;
            // 
            // selectPanel
            // 
            this.selectPanel.Controls.Add(this.comboBox1);
            this.selectPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.selectPanel.Location = new System.Drawing.Point(300, 0);
            this.selectPanel.Name = "selectPanel";
            this.selectPanel.Size = new System.Drawing.Size(115, 35);
            this.selectPanel.TabIndex = 4;
            this.selectPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.lblNo_Paint);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "全部",
            "例如：2-7"});
            this.comboBox1.Location = new System.Drawing.Point(0, 7);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(114, 20);
            this.comboBox1.TabIndex = 4;
            // 
            // panelStatus
            // 
            this.panelStatus.Controls.Add(this.picStatus);
            this.panelStatus.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelStatus.Location = new System.Drawing.Point(415, 0);
            this.panelStatus.Name = "panelStatus";
            this.panelStatus.Size = new System.Drawing.Size(175, 35);
            this.panelStatus.TabIndex = 5;
            this.panelStatus.Paint += new System.Windows.Forms.PaintEventHandler(this.lblNo_Paint);
            // 
            // picStatus
            // 
            this.picStatus.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picStatus.Image = global::Controls.Properties.Resources.satdone;
            this.picStatus.Location = new System.Drawing.Point(141, 5);
            this.picStatus.Name = "picStatus";
            this.picStatus.Size = new System.Drawing.Size(24, 24);
            this.picStatus.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picStatus.TabIndex = 1;
            this.picStatus.TabStop = false;
            this.picStatus.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseClick);
            // 
            // panelOperate
            // 
            this.panelOperate.Controls.Add(this.picDelete);
            this.panelOperate.Controls.Add(this.picOpenPath);
            this.panelOperate.Controls.Add(this.lblOpen);
            this.panelOperate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelOperate.Location = new System.Drawing.Point(590, 0);
            this.panelOperate.Name = "panelOperate";
            this.panelOperate.Size = new System.Drawing.Size(175, 35);
            this.panelOperate.TabIndex = 6;
            this.panelOperate.Paint += new System.Windows.Forms.PaintEventHandler(this.lblNo_Paint);
            // 
            // picDelete
            // 
            this.picDelete.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picDelete.Image = ((System.Drawing.Image)(resources.GetObject("picDelete.Image")));
            this.picDelete.Location = new System.Drawing.Point(120, 8);
            this.picDelete.Name = "picDelete";
            this.picDelete.Size = new System.Drawing.Size(20, 20);
            this.picDelete.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picDelete.TabIndex = 3;
            this.picDelete.TabStop = false;
            this.picDelete.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picDelete_MouseClick);
            // 
            // picOpenPath
            // 
            this.picOpenPath.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picOpenPath.Image = ((System.Drawing.Image)(resources.GetObject("picOpenPath.Image")));
            this.picOpenPath.Location = new System.Drawing.Point(85, 8);
            this.picOpenPath.Name = "picOpenPath";
            this.picOpenPath.Size = new System.Drawing.Size(20, 20);
            this.picOpenPath.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picOpenPath.TabIndex = 2;
            this.picOpenPath.TabStop = false;
            this.picOpenPath.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picOpenPath_MouseClick);
            // 
            // lblOpen
            // 
            this.lblOpen.AutoSize = true;
            this.lblOpen.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblOpen.Font = new System.Drawing.Font("宋体", 12F);
            this.lblOpen.ForeColor = System.Drawing.Color.Black;
            this.lblOpen.Location = new System.Drawing.Point(35, 10);
            this.lblOpen.Name = "lblOpen";
            this.lblOpen.Size = new System.Drawing.Size(40, 16);
            this.lblOpen.TabIndex = 1;
            this.lblOpen.Text = "打开";
            this.lblOpen.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblOpen.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lblOpen_MouseClick);
            // 
            // Item
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.panelOperate);
            this.Controls.Add(this.panelStatus);
            this.Controls.Add(this.selectPanel);
            this.Controls.Add(this.panel1);
            this.DoubleBuffered = true;
            this.Name = "Item";
            this.Size = new System.Drawing.Size(765, 35);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picType)).EndInit();
            this.selectPanel.ResumeLayout(false);
            this.panelStatus.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picStatus)).EndInit();
            this.panelOperate.ResumeLayout(false);
            this.panelOperate.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picDelete)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picOpenPath)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.PictureBox picType;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblFileName;
        private System.Windows.Forms.Panel selectPanel;
        private System.Windows.Forms.Panel panelStatus;
        private System.Windows.Forms.PictureBox picStatus;
        private System.Windows.Forms.Panel panelOperate;
        private System.Windows.Forms.PictureBox picDelete;
        private System.Windows.Forms.PictureBox picOpenPath;
        private System.Windows.Forms.Label lblOpen;
        private System.Windows.Forms.ComboBox comboBox1;
    }
}
