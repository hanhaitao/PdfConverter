namespace PDF_Convert
{
    partial class frmMain01Setting03
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain01Setting03));
            this.plAll = new System.Windows.Forms.Panel();
            this.lblCursorPosition = new System.Windows.Forms.Label();
            this.tbPageSelect1 = new PDF_Convert.ucTextBoxListView();
            this.pbConfirm = new System.Windows.Forms.PictureBox();
            this.tbPageSelect2 = new PDF_Convert.ucTextBoxListView();
            this.pbClose = new System.Windows.Forms.PictureBox();
            this.pbAll = new System.Windows.Forms.PictureBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.rtbTemp = new System.Windows.Forms.RichTextBox();
            this.plAll.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbConfirm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbClose)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAll)).BeginInit();
            this.SuspendLayout();
            // 
            // plAll
            // 
            this.plAll.AllowDrop = true;
            this.plAll.AutoSize = true;
            this.plAll.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.plAll.Controls.Add(this.rtbTemp);
            this.plAll.Controls.Add(this.lblCursorPosition);
            this.plAll.Controls.Add(this.tbPageSelect1);
            this.plAll.Controls.Add(this.pbConfirm);
            this.plAll.Controls.Add(this.tbPageSelect2);
            this.plAll.Controls.Add(this.pbClose);
            this.plAll.Controls.Add(this.pbAll);
            this.plAll.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plAll.Location = new System.Drawing.Point(0, 0);
            this.plAll.Name = "plAll";
            this.plAll.Size = new System.Drawing.Size(369, 292);
            this.plAll.TabIndex = 1;
            // 
            // lblCursorPosition
            // 
            this.lblCursorPosition.AutoSize = true;
            this.lblCursorPosition.BackColor = System.Drawing.Color.Transparent;
            this.lblCursorPosition.Location = new System.Drawing.Point(245, 47);
            this.lblCursorPosition.Name = "lblCursorPosition";
            this.lblCursorPosition.Size = new System.Drawing.Size(89, 12);
            this.lblCursorPosition.TabIndex = 117;
            this.lblCursorPosition.Text = "CursorPosition";
            this.lblCursorPosition.UseWaitCursor = true;
            this.lblCursorPosition.Visible = false;
            // 
            // tbPageSelect1
            // 
            this.tbPageSelect1.BackGroundText = "此处要有文字否则cs代码处不可引用";
            this.tbPageSelect1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbPageSelect1.Img = null;
            this.tbPageSelect1.Location = new System.Drawing.Point(26, 81);
            this.tbPageSelect1.Name = "tbPageSelect1";
            this.tbPageSelect1.Size = new System.Drawing.Size(320, 23);
            this.tbPageSelect1.TabIndex = 0;
            this.tbPageSelect1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tbPageSelect1_MouseClick);
            this.tbPageSelect1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbPageSelect1_KeyDown);
            // 
            // pbConfirm
            // 
            this.pbConfirm.BackColor = System.Drawing.Color.Transparent;
            this.pbConfirm.BackgroundImage = global::PDF_Convert.Properties.Resources.close_01;
            this.pbConfirm.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pbConfirm.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbConfirm.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pbConfirm.Location = new System.Drawing.Point(112, 242);
            this.pbConfirm.Name = "pbConfirm";
            this.pbConfirm.Size = new System.Drawing.Size(91, 36);
            this.pbConfirm.TabIndex = 115;
            this.pbConfirm.TabStop = false;
            this.pbConfirm.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pbConfirm_MouseClick);
            this.pbConfirm.MouseEnter += new System.EventHandler(this.pbConfirm_MouseEnter);
            this.pbConfirm.MouseLeave += new System.EventHandler(this.pbConfirm_MouseLeave);
            // 
            // tbPageSelect2
            // 
            this.tbPageSelect2.BackGroundText = "此处要有文字否则cs代码处不可引用";
            this.tbPageSelect2.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbPageSelect2.Img = null;
            this.tbPageSelect2.Location = new System.Drawing.Point(26, 164);
            this.tbPageSelect2.Name = "tbPageSelect2";
            this.tbPageSelect2.Size = new System.Drawing.Size(320, 23);
            this.tbPageSelect2.TabIndex = 1;
            this.tbPageSelect2.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tbPageSelect2_MouseClick);
            this.tbPageSelect2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbPageSelect2_KeyDown);
            // 
            // pbClose
            // 
            this.pbClose.BackColor = System.Drawing.Color.Transparent;
            this.pbClose.BackgroundImage = global::PDF_Convert.Properties.Resources.close_01;
            this.pbClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbClose.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pbClose.Location = new System.Drawing.Point(327, 14);
            this.pbClose.Name = "pbClose";
            this.pbClose.Size = new System.Drawing.Size(16, 16);
            this.pbClose.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbClose.TabIndex = 113;
            this.pbClose.TabStop = false;
            this.pbClose.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pbClose_MouseClick);
            this.pbClose.MouseEnter += new System.EventHandler(this.pbClose_MouseEnter);
            this.pbClose.MouseLeave += new System.EventHandler(this.pbClose_MouseLeave);
            // 
            // pbAll
            // 
            this.pbAll.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pbAll.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbAll.Location = new System.Drawing.Point(0, 0);
            this.pbAll.Name = "pbAll";
            this.pbAll.Size = new System.Drawing.Size(369, 292);
            this.pbAll.TabIndex = 0;
            this.pbAll.TabStop = false;
            this.pbAll.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbAll_MouseDown);
            this.pbAll.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbAll_MouseMove);
            // 
            // rtbTemp
            // 
            this.rtbTemp.Location = new System.Drawing.Point(26, 193);
            this.rtbTemp.Name = "rtbTemp";
            this.rtbTemp.Size = new System.Drawing.Size(317, 43);
            this.rtbTemp.TabIndex = 0;
            this.rtbTemp.Text = "放一个文本框目的是在初次显示窗体时把鼠标放在这里，否则tbPageSelect1会不显示提示信息";
            // 
            // frmMain01Setting03
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(369, 292);
            this.Controls.Add(this.plAll);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmMain01Setting03";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Load += new System.EventHandler(this.frmMain01Setting03_Load);
            this.plAll.ResumeLayout(false);
            this.plAll.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbConfirm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbClose)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAll)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel plAll;
        private System.Windows.Forms.Label lblCursorPosition;
        private System.Windows.Forms.PictureBox pbConfirm;
        private System.Windows.Forms.PictureBox pbClose;
        private System.Windows.Forms.PictureBox pbAll;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolTip toolTip1;
        private ucTextBoxListView tbPageSelect1;
        private ucTextBoxListView tbPageSelect2;
        private System.Windows.Forms.RichTextBox rtbTemp;


    }
}