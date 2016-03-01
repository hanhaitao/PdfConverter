namespace PDF_Convert
{
    partial class ucPicClearBar
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
            this.lblText = new System.Windows.Forms.Label();
            this.picImg = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picImg)).BeginInit();
            this.SuspendLayout();
            // 
            // lblText
            // 
            this.lblText.AutoSize = true;
            this.lblText.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblText.ForeColor = System.Drawing.Color.DimGray;
            this.lblText.Location = new System.Drawing.Point(33, 3);
            this.lblText.Name = "lblText";
            this.lblText.Size = new System.Drawing.Size(69, 20);
            this.lblText.TabIndex = 69;
            this.lblText.Text = " 清空列表";
            this.lblText.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lblText_MouseClick);
            this.lblText.MouseEnter += new System.EventHandler(this.ucPicClearBar_MouseEnter);
            this.lblText.MouseLeave += new System.EventHandler(this.ucPicClearBar_MouseLeave);
            // 
            // picImg
            // 
            this.picImg.Image = global::PDF_Convert.Properties.Resources.qingkong1;
            this.picImg.Location = new System.Drawing.Point(5, 2);
            this.picImg.Name = "picImg";
            this.picImg.Size = new System.Drawing.Size(25, 28);
            this.picImg.TabIndex = 68;
            this.picImg.TabStop = false;
            this.picImg.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picImg_MouseClick);
            this.picImg.MouseEnter += new System.EventHandler(this.ucPicClearBar_MouseEnter);
            this.picImg.MouseLeave += new System.EventHandler(this.ucPicClearBar_MouseLeave);
            // 
            // ucPicClearBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblText);
            this.Controls.Add(this.picImg);
            this.DoubleBuffered = true;
            this.Name = "ucPicClearBar";
            this.Size = new System.Drawing.Size(109, 33);
            this.MouseEnter += new System.EventHandler(this.ucPicClearBar_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.ucPicClearBar_MouseLeave);
            ((System.ComponentModel.ISupportInitialize)(this.picImg)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblText;
        private System.Windows.Forms.PictureBox picImg;

    }
}
