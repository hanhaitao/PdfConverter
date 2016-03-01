namespace PDF_Convert
{
    partial class ucPicButtonBarLong
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
            this.picBtn = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picBtn)).BeginInit();
            this.SuspendLayout();
            // 
            // lblText
            // 
            this.lblText.AutoSize = true;
            this.lblText.BackColor = System.Drawing.Color.Transparent;
            this.lblText.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblText.ForeColor = System.Drawing.Color.White;
            this.lblText.Location = new System.Drawing.Point(29, 5);
            this.lblText.Name = "lblText";
            this.lblText.Size = new System.Drawing.Size(65, 20);
            this.lblText.TabIndex = 0;
            this.lblText.Text = "添加文档";
            this.lblText.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lblText_MouseClick);
            this.lblText.MouseEnter += new System.EventHandler(this.lblText_MouseEnter);
            this.lblText.MouseLeave += new System.EventHandler(this.lblText_MouseLeave);
            // 
            // picBtn
            // 
            this.picBtn.BackColor = System.Drawing.Color.Transparent;
            this.picBtn.Image = global::PDF_Convert.Properties.Resources.mouse_29;
            this.picBtn.Location = new System.Drawing.Point(8, 5);
            this.picBtn.Name = "picBtn";
            this.picBtn.Size = new System.Drawing.Size(20, 20);
            this.picBtn.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picBtn.TabIndex = 1;
            this.picBtn.TabStop = false;
            // 
            // ucPicButtonBarLong
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::PDF_Convert.Properties.Resources.you_23;
            this.Controls.Add(this.picBtn);
            this.Controls.Add(this.lblText);
            this.DoubleBuffered = true;
            this.Name = "ucPicButtonBarLong";
            this.Size = new System.Drawing.Size(125, 30);
            ((System.ComponentModel.ISupportInitialize)(this.picBtn)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblText;
        private System.Windows.Forms.PictureBox picBtn;


    }
}
