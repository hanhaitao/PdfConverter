namespace PDF_Convert
{
    partial class ucBuy
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
            this.buyImg = new System.Windows.Forms.PictureBox();
            this.labTile = new System.Windows.Forms.Label();
            this.labIn = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.buyImg)).BeginInit();
            this.SuspendLayout();
            // 
            // buyImg
            // 
            this.buyImg.Location = new System.Drawing.Point(2, 5);
            this.buyImg.Name = "buyImg";
            this.buyImg.Size = new System.Drawing.Size(30, 30);
            this.buyImg.TabIndex = 0;
            this.buyImg.TabStop = false;
            this.buyImg.MouseClick += new System.Windows.Forms.MouseEventHandler(this.buyImg_MouseClick);
            // 
            // labTile
            // 
            this.labTile.AutoSize = true;
            this.labTile.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labTile.ForeColor = System.Drawing.SystemColors.ButtonShadow;
            this.labTile.Location = new System.Drawing.Point(37, 2);
            this.labTile.Name = "labTile";
            this.labTile.Size = new System.Drawing.Size(52, 17);
            this.labTile.TabIndex = 1;
            this.labTile.Text = "在线QQ";
            this.labTile.MouseClick += new System.Windows.Forms.MouseEventHandler(this.labTile_MouseClick);
            // 
            // labIn
            // 
            this.labIn.AutoSize = true;
            this.labIn.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labIn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(171)))), ((int)(((byte)(253)))));
            this.labIn.Location = new System.Drawing.Point(36, 19);
            this.labIn.Name = "labIn";
            this.labIn.Size = new System.Drawing.Size(89, 19);
            this.labIn.TabIndex = 2;
            this.labIn.Text = "4006685572";
            this.labIn.MouseClick += new System.Windows.Forms.MouseEventHandler(this.labIn_MouseClick);
            // 
            // ucBuy
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.labIn);
            this.Controls.Add(this.labTile);
            this.Controls.Add(this.buyImg);
            this.Name = "ucBuy";
            this.Size = new System.Drawing.Size(130, 40);
            ((System.ComponentModel.ISupportInitialize)(this.buyImg)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox buyImg;
        private System.Windows.Forms.Label labTile;
        private System.Windows.Forms.Label labIn;
    }
}
