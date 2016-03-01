namespace Controls
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
            this.picImg = new System.Windows.Forms.PictureBox();
            this.lblText = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picImg)).BeginInit();
            this.SuspendLayout();
            // 
            // picImg
            // 
            this.picImg.BackColor = System.Drawing.Color.Transparent;
            this.picImg.Image = global::Controls.Properties.Resources.picword;
            this.picImg.Location = new System.Drawing.Point(41, 3);
            this.picImg.Name = "picImg";
            this.picImg.Size = new System.Drawing.Size(35, 30);
            this.picImg.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picImg.TabIndex = 1;
            this.picImg.TabStop = false;
            this.picImg.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ControlMouseClick);
            this.picImg.MouseEnter += new System.EventHandler(this.ControlMouseEnter);
            this.picImg.MouseLeave += new System.EventHandler(this.ControlMouseLeave);
            // 
            // lblText
            // 
            this.lblText.AutoSize = true;
            this.lblText.BackColor = System.Drawing.Color.Transparent;
            this.lblText.Font = new System.Drawing.Font("黑体", 14F);
            this.lblText.Location = new System.Drawing.Point(82, 8);
            this.lblText.Name = "lblText";
            this.lblText.Size = new System.Drawing.Size(49, 19);
            this.lblText.TabIndex = 2;
            this.lblText.Text = "Text";
            this.lblText.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ControlMouseClick);
            this.lblText.MouseEnter += new System.EventHandler(this.ControlMouseEnter);
            this.lblText.MouseLeave += new System.EventHandler(this.ControlMouseLeave);
            // 
            // ucPicButton
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Controls.Add(this.lblText);
            this.Controls.Add(this.picImg);
            this.DoubleBuffered = true;
            this.Name = "ucPicButton";
            this.Size = new System.Drawing.Size(230, 36);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.ucPicButton_Paint);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ControlMouseClick);
            this.MouseEnter += new System.EventHandler(this.ControlMouseEnter);
            this.MouseLeave += new System.EventHandler(this.ControlMouseLeave);
            ((System.ComponentModel.ISupportInitialize)(this.picImg)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picImg;
        private System.Windows.Forms.Label lblText;
    }
}
