namespace PDF_Convert
{
    partial class ucPicStatusBar
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
            this.lblText.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.lblText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblText.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblText.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            this.lblText.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblText.Location = new System.Drawing.Point(0, 0);
            this.lblText.Name = "lblText";
            this.lblText.Size = new System.Drawing.Size(120, 46);
            this.lblText.TabIndex = 3;
            this.lblText.Text = "在线教程";
            this.lblText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblText.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lblText_MouseClick);
            this.lblText.MouseEnter += new System.EventHandler(this.ucPicStatusBar_MouseEnter);
            this.lblText.MouseLeave += new System.EventHandler(this.ucPicStatusBar_MouseLeave);
            // 
            // picImg
            // 
            this.picImg.Image = global::PDF_Convert.Properties.Resources.online;
            this.picImg.Location = new System.Drawing.Point(0, 0);
            this.picImg.Name = "picImg";
            this.picImg.Size = new System.Drawing.Size(26, 46);
            this.picImg.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picImg.TabIndex = 0;
            this.picImg.TabStop = false;
            this.picImg.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picImg_MouseClick);
            this.picImg.MouseEnter += new System.EventHandler(this.ucPicStatusBar_MouseEnter);
            this.picImg.MouseLeave += new System.EventHandler(this.ucPicStatusBar_MouseLeave);
            // 
            // ucPicStatusBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblText);
            this.Controls.Add(this.picImg);
            this.DoubleBuffered = true;
            this.Name = "ucPicStatusBar";
            this.Size = new System.Drawing.Size(120, 46);
            this.MouseEnter += new System.EventHandler(this.ucPicStatusBar_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.ucPicStatusBar_MouseLeave);
            ((System.ComponentModel.ISupportInitialize)(this.picImg)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox picImg;
        private System.Windows.Forms.Label lblText;
    }
}
