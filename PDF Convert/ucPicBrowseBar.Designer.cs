namespace PDF_Convert
{
    partial class ucPicBrowseBar
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
            this.SuspendLayout();
            // 
            // lblText
            // 
            this.lblText.BackColor = System.Drawing.Color.Transparent;
            this.lblText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblText.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.lblText.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblText.Location = new System.Drawing.Point(0, 0);
            this.lblText.Name = "lblText";
            this.lblText.Size = new System.Drawing.Size(74, 30);
            this.lblText.TabIndex = 0;
            this.lblText.Text = "浏览";
            this.lblText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblText.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lblText_MouseClick);
            this.lblText.MouseEnter += new System.EventHandler(this.lblText_MouseEnter);
            this.lblText.MouseLeave += new System.EventHandler(this.lblText_MouseLeave);
            // 
            // ucPicBrowseBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::PDF_Convert.Properties.Resources.look;
            this.Controls.Add(this.lblText);
            this.DoubleBuffered = true;
            this.Name = "ucPicBrowseBar";
            this.Size = new System.Drawing.Size(74, 30);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblText;


    }
}
