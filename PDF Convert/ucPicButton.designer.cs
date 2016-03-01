namespace PDF_Convert
{
    partial class ucPicButton
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
            this.hotPB = new System.Windows.Forms.PictureBox();
            this.picImg = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.hotPB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picImg)).BeginInit();
            this.SuspendLayout();
            // 
            // lblText
            // 
            this.lblText.AutoSize = true;
            this.lblText.BackColor = System.Drawing.Color.Transparent;
            this.lblText.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.lblText.Location = new System.Drawing.Point(60, 5);
            this.lblText.Name = "lblText";
            this.lblText.Size = new System.Drawing.Size(32, 17);
            this.lblText.TabIndex = 2;
            this.lblText.Text = "Text";
            this.lblText.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ControlMouseClick);
            this.lblText.MouseEnter += new System.EventHandler(this.ControlMouseEnter);
            this.lblText.MouseLeave += new System.EventHandler(this.ControlMouseLeave);
            // 
            // hotPB
            // 
            this.hotPB.Image = global::PDF_Convert.Properties.Resources.imgs_46;
            this.hotPB.Location = new System.Drawing.Point(158, 6);
            this.hotPB.Name = "hotPB";
            this.hotPB.Size = new System.Drawing.Size(22, 12);
            this.hotPB.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.hotPB.TabIndex = 3;
            this.hotPB.TabStop = false;
            this.hotPB.Visible = false;
            // 
            // picImg
            // 
            this.picImg.BackColor = System.Drawing.Color.Transparent;
            this.picImg.Image = global::PDF_Convert.Properties.Resources.picword;
            this.picImg.Location = new System.Drawing.Point(25, 0);
            this.picImg.Name = "picImg";
            this.picImg.Size = new System.Drawing.Size(35, 30);
            this.picImg.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picImg.TabIndex = 1;
            this.picImg.TabStop = false;
            this.picImg.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ControlMouseClick);
            this.picImg.MouseEnter += new System.EventHandler(this.ControlMouseEnter);
            this.picImg.MouseLeave += new System.EventHandler(this.ControlMouseLeave);
            // 
            // ucPicButton
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.Controls.Add(this.hotPB);
            this.Controls.Add(this.lblText);
            this.Controls.Add(this.picImg);
            this.DoubleBuffered = true;
            this.Name = "ucPicButton";
            this.Size = new System.Drawing.Size(193, 30);
            this.Load += new System.EventHandler(this.ucPicButton_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.ucPicButton_Paint);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ControlMouseClick);
            this.MouseEnter += new System.EventHandler(this.ControlMouseEnter);
            this.MouseLeave += new System.EventHandler(this.ControlMouseLeave);
            ((System.ComponentModel.ISupportInitialize)(this.hotPB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picImg)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picImg;
        private System.Windows.Forms.Label lblText;
        private System.Windows.Forms.PictureBox hotPB;
    }
}
