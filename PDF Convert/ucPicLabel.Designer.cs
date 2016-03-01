namespace PDF_Convert
{
    partial class ucPicLabel
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
            this.img = new System.Windows.Forms.PictureBox();
            this.txtIn = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.img)).BeginInit();
            this.SuspendLayout();
            // 
            // img
            // 
            this.img.Image = global::PDF_Convert.Properties.Resources.RegDlg_ico_dialog_tel;
            this.img.Location = new System.Drawing.Point(-1, 3);
            this.img.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.img.Name = "img";
            this.img.Size = new System.Drawing.Size(14, 14);
            this.img.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.img.TabIndex = 0;
            this.img.TabStop = false;
            // 
            // txtIn
            // 
            this.txtIn.AutoSize = true;
            this.txtIn.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtIn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(192)))), ((int)(((byte)(239)))));
            this.txtIn.Location = new System.Drawing.Point(15, 2);
            this.txtIn.Name = "txtIn";
            this.txtIn.Size = new System.Drawing.Size(104, 17);
            this.txtIn.TabIndex = 1;
            this.txtIn.Text = "可永久无现在使用";
            // 
            // ucPicLabel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.img);
            this.Controls.Add(this.txtIn);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(210)))), ((int)(((byte)(132)))));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "ucPicLabel";
            this.Size = new System.Drawing.Size(184, 29);
            ((System.ComponentModel.ISupportInitialize)(this.img)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox img;
        private System.Windows.Forms.Label txtIn;
    }
}
