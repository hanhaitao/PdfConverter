namespace Controls
{
    partial class Header
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
            this.lblFileName = new System.Windows.Forms.Label();
            this.lblSelectPage = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblOperate = new System.Windows.Forms.Label();
            this.lblNo = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblFileName
            // 
            this.lblFileName.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblFileName.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblFileName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.lblFileName.Location = new System.Drawing.Point(100, 0);
            this.lblFileName.Name = "lblFileName";
            this.lblFileName.Size = new System.Drawing.Size(100, 45);
            this.lblFileName.TabIndex = 1;
            this.lblFileName.Text = "文件名称";
            this.lblFileName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblFileName.Paint += new System.Windows.Forms.PaintEventHandler(this.lbl_Paint);
            // 
            // lblSelectPage
            // 
            this.lblSelectPage.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblSelectPage.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblSelectPage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.lblSelectPage.Location = new System.Drawing.Point(200, 0);
            this.lblSelectPage.Name = "lblSelectPage";
            this.lblSelectPage.Size = new System.Drawing.Size(96, 45);
            this.lblSelectPage.TabIndex = 2;
            this.lblSelectPage.Text = "页码选择";
            this.lblSelectPage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblSelectPage.Paint += new System.Windows.Forms.PaintEventHandler(this.lbl_Paint);
            // 
            // lblStatus
            // 
            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblStatus.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.lblStatus.Location = new System.Drawing.Point(296, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(100, 45);
            this.lblStatus.TabIndex = 3;
            this.lblStatus.Text = "状态";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblStatus.Paint += new System.Windows.Forms.PaintEventHandler(this.lbl_Paint);
            // 
            // lblOperate
            // 
            this.lblOperate.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblOperate.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblOperate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.lblOperate.Location = new System.Drawing.Point(396, 0);
            this.lblOperate.Name = "lblOperate";
            this.lblOperate.Size = new System.Drawing.Size(100, 45);
            this.lblOperate.TabIndex = 4;
            this.lblOperate.Text = "操作";
            this.lblOperate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblOperate.Paint += new System.Windows.Forms.PaintEventHandler(this.lbl_Paint);
            // 
            // lblNo
            // 
            this.lblNo.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblNo.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblNo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.lblNo.Location = new System.Drawing.Point(0, 0);
            this.lblNo.Name = "lblNo";
            this.lblNo.Size = new System.Drawing.Size(100, 45);
            this.lblNo.TabIndex = 0;
            this.lblNo.Text = "编号";
            this.lblNo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNo.Paint += new System.Windows.Forms.PaintEventHandler(this.lbl_Paint);
            // 
            // Header
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(247)))), ((int)(((byte)(247)))));
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.lblOperate);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lblSelectPage);
            this.Controls.Add(this.lblFileName);
            this.Controls.Add(this.lblNo);
            this.Name = "Header";
            this.Size = new System.Drawing.Size(818, 45);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblFileName;
        private System.Windows.Forms.Label lblSelectPage;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblOperate;
        private System.Windows.Forms.Label lblNo;
    }
}
