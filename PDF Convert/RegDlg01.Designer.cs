namespace PDF_Convert
{
    partial class RegDlg01
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RegDlg01));
            this.txtRegCode = new System.Windows.Forms.TextBox();
            this.txtMachineCode = new System.Windows.Forms.TextBox();
            this.pbQQ = new System.Windows.Forms.PictureBox();
            this.pbHelp = new System.Windows.Forms.PictureBox();
            this.pbRegCodeGet = new System.Windows.Forms.PictureBox();
            this.pbClose = new System.Windows.Forms.PictureBox();
            this.pbActive = new System.Windows.Forms.PictureBox();
            this.lblCursorPosition = new System.Windows.Forms.Label();
            this.pbBuy = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbQQ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbHelp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbRegCodeGet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbClose)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbActive)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbBuy)).BeginInit();
            this.SuspendLayout();
            // 
            // txtRegCode
            // 
            this.txtRegCode.BackColor = System.Drawing.Color.White;
            this.txtRegCode.Font = new System.Drawing.Font("宋体", 16F);
            this.txtRegCode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(122)))), ((int)(((byte)(245)))));
            this.txtRegCode.Location = new System.Drawing.Point(155, 150);
            this.txtRegCode.Name = "txtRegCode";
            this.txtRegCode.PasswordChar = '*';
            this.txtRegCode.Size = new System.Drawing.Size(223, 32);
            this.txtRegCode.TabIndex = 110;
            this.txtRegCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtRegCode_KeyDown);
            // 
            // txtMachineCode
            // 
            this.txtMachineCode.Font = new System.Drawing.Font("宋体", 16F);
            this.txtMachineCode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(122)))), ((int)(((byte)(245)))));
            this.txtMachineCode.Location = new System.Drawing.Point(155, 112);
            this.txtMachineCode.Name = "txtMachineCode";
            this.txtMachineCode.Size = new System.Drawing.Size(223, 32);
            this.txtMachineCode.TabIndex = 111;
            this.txtMachineCode.MouseClick += new System.Windows.Forms.MouseEventHandler(this.txtMachineCode_MouseClick);
            // 
            // pbQQ
            // 
            this.pbQQ.BackColor = System.Drawing.Color.Transparent;
            this.pbQQ.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbQQ.Location = new System.Drawing.Point(21, 323);
            this.pbQQ.Name = "pbQQ";
            this.pbQQ.Size = new System.Drawing.Size(144, 27);
            this.pbQQ.TabIndex = 112;
            this.pbQQ.TabStop = false;
            this.pbQQ.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pbQQ_MouseClick);
            // 
            // pbHelp
            // 
            this.pbHelp.BackColor = System.Drawing.Color.Transparent;
            this.pbHelp.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbHelp.Location = new System.Drawing.Point(175, 323);
            this.pbHelp.Name = "pbHelp";
            this.pbHelp.Size = new System.Drawing.Size(73, 27);
            this.pbHelp.TabIndex = 113;
            this.pbHelp.TabStop = false;
            this.pbHelp.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pbHelp_MouseClick);
            // 
            // pbRegCodeGet
            // 
            this.pbRegCodeGet.BackColor = System.Drawing.Color.Transparent;
            this.pbRegCodeGet.BackgroundImage = global::PDF_Convert.Properties.Resources.close_01;
            this.pbRegCodeGet.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pbRegCodeGet.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbRegCodeGet.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pbRegCodeGet.Location = new System.Drawing.Point(185, 196);
            this.pbRegCodeGet.Name = "pbRegCodeGet";
            this.pbRegCodeGet.Size = new System.Drawing.Size(124, 36);
            this.pbRegCodeGet.TabIndex = 117;
            this.pbRegCodeGet.TabStop = false;
            this.pbRegCodeGet.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pbRegCodeGet_MouseClick);
            this.pbRegCodeGet.MouseEnter += new System.EventHandler(this.pbRegCodeGet_MouseEnter);
            this.pbRegCodeGet.MouseLeave += new System.EventHandler(this.pbRegCodeGet_MouseLeave);
            // 
            // pbClose
            // 
            this.pbClose.BackColor = System.Drawing.Color.Transparent;
            this.pbClose.BackgroundImage = global::PDF_Convert.Properties.Resources.close_01;
            this.pbClose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pbClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbClose.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pbClose.Location = new System.Drawing.Point(452, 12);
            this.pbClose.Name = "pbClose";
            this.pbClose.Size = new System.Drawing.Size(16, 16);
            this.pbClose.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbClose.TabIndex = 116;
            this.pbClose.TabStop = false;
            this.pbClose.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pbClose_MouseClick);
            this.pbClose.MouseEnter += new System.EventHandler(this.pbClose_MouseEnter);
            this.pbClose.MouseLeave += new System.EventHandler(this.pbClose_MouseLeave);
            // 
            // pbActive
            // 
            this.pbActive.BackColor = System.Drawing.Color.Transparent;
            this.pbActive.BackgroundImage = global::PDF_Convert.Properties.Resources.close_01;
            this.pbActive.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pbActive.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbActive.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pbActive.Location = new System.Drawing.Point(320, 196);
            this.pbActive.Name = "pbActive";
            this.pbActive.Size = new System.Drawing.Size(124, 36);
            this.pbActive.TabIndex = 115;
            this.pbActive.TabStop = false;
            this.pbActive.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pbActive_MouseClick);
            this.pbActive.MouseEnter += new System.EventHandler(this.pbActive_MouseEnter);
            this.pbActive.MouseLeave += new System.EventHandler(this.pbActive_MouseLeave);
            // 
            // lblCursorPosition
            // 
            this.lblCursorPosition.AutoSize = true;
            this.lblCursorPosition.BackColor = System.Drawing.Color.Transparent;
            this.lblCursorPosition.Location = new System.Drawing.Point(365, 66);
            this.lblCursorPosition.Name = "lblCursorPosition";
            this.lblCursorPosition.Size = new System.Drawing.Size(89, 12);
            this.lblCursorPosition.TabIndex = 118;
            this.lblCursorPosition.Text = "CursorPosition";
            this.lblCursorPosition.UseWaitCursor = true;
            this.lblCursorPosition.Visible = false;
            // 
            // pbBuy
            // 
            this.pbBuy.BackColor = System.Drawing.Color.Transparent;
            this.pbBuy.BackgroundImage = global::PDF_Convert.Properties.Resources.close_01;
            this.pbBuy.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pbBuy.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbBuy.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pbBuy.Location = new System.Drawing.Point(50, 196);
            this.pbBuy.Name = "pbBuy";
            this.pbBuy.Size = new System.Drawing.Size(124, 36);
            this.pbBuy.TabIndex = 119;
            this.pbBuy.TabStop = false;
            this.pbBuy.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pbBuy_MouseClick);
            this.pbBuy.MouseEnter += new System.EventHandler(this.pbBuy_MouseEnter);
            this.pbBuy.MouseLeave += new System.EventHandler(this.pbBuy_MouseLeave);
            // 
            // RegDlg01
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(494, 362);
            this.Controls.Add(this.pbBuy);
            this.Controls.Add(this.lblCursorPosition);
            this.Controls.Add(this.pbRegCodeGet);
            this.Controls.Add(this.pbClose);
            this.Controls.Add(this.pbActive);
            this.Controls.Add(this.pbQQ);
            this.Controls.Add(this.pbHelp);
            this.Controls.Add(this.txtMachineCode);
            this.Controls.Add(this.txtRegCode);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "RegDlg01";
            this.Text = "RegDlg01";
            this.Load += new System.EventHandler(this.RegDlg01_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RegDlg01_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.RegDlg01_MouseMove);
            ((System.ComponentModel.ISupportInitialize)(this.pbQQ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbHelp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbRegCodeGet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbClose)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbActive)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbBuy)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtRegCode;
        private System.Windows.Forms.TextBox txtMachineCode;
        private System.Windows.Forms.PictureBox pbQQ;
        private System.Windows.Forms.PictureBox pbHelp;
        private System.Windows.Forms.PictureBox pbRegCodeGet;
        private System.Windows.Forms.PictureBox pbClose;
        private System.Windows.Forms.PictureBox pbActive;
        private System.Windows.Forms.Label lblCursorPosition;
        private System.Windows.Forms.PictureBox pbBuy;
    }
}