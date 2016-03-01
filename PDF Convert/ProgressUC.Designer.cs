namespace PDF_Convert
{
    partial class ProgressUC
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
            this.labProgress = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labProgress
            // 
            this.labProgress.AutoSize = true;
            this.labProgress.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labProgress.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(173)))), ((int)(((byte)(253)))));
            this.labProgress.Location = new System.Drawing.Point(18, 34);
            this.labProgress.Name = "labProgress";
            this.labProgress.Size = new System.Drawing.Size(49, 31);
            this.labProgress.TabIndex = 0;
            this.labProgress.Text = "0%";
            // 
            // ProgressUC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.BackgroundImage = global::PDF_Convert.Properties.Resources.yuan_03;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.Controls.Add(this.labProgress);
            this.DoubleBuffered = true;
            this.Name = "ProgressUC";
            this.Size = new System.Drawing.Size(100, 100);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labProgress;
    }
}
