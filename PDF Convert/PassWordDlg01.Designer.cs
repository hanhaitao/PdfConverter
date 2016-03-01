namespace PDF_Convert
{
    partial class PassWordDlg01
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PassWordDlg01));
            this.pbConfirm = new System.Windows.Forms.PictureBox();
            this.pbClose = new System.Windows.Forms.PictureBox();
            this.pbOver = new System.Windows.Forms.PictureBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.lblFileName = new System.Windows.Forms.Label();
            this.lblCursorPosition = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pbConfirm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbClose)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbOver)).BeginInit();
            this.SuspendLayout();
            // 
            // pbConfirm
            // 
            this.pbConfirm.BackColor = System.Drawing.Color.Transparent;
            this.pbConfirm.BackgroundImage = global::PDF_Convert.Properties.Resources.close_01;
            this.pbConfirm.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pbConfirm.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbConfirm.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pbConfirm.Location = new System.Drawing.Point(77, 238);
            this.pbConfirm.Name = "pbConfirm";
            this.pbConfirm.Size = new System.Drawing.Size(108, 36);
            this.pbConfirm.TabIndex = 107;
            this.pbConfirm.TabStop = false;
            this.pbConfirm.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pbConfirm_MouseClick);
            this.pbConfirm.MouseEnter += new System.EventHandler(this.pbConfirm_MouseEnter);
            this.pbConfirm.MouseLeave += new System.EventHandler(this.pbConfirm_MouseLeave);
            // 
            // pbClose
            // 
            this.pbClose.BackColor = System.Drawing.Color.Transparent;
            this.pbClose.BackgroundImage = global::PDF_Convert.Properties.Resources.close_01;
            this.pbClose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pbClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbClose.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pbClose.Location = new System.Drawing.Point(394, 12);
            this.pbClose.Name = "pbClose";
            this.pbClose.Size = new System.Drawing.Size(16, 16);
            this.pbClose.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbClose.TabIndex = 106;
            this.pbClose.TabStop = false;
            this.pbClose.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pbOver_MouseClick);
            this.pbClose.MouseEnter += new System.EventHandler(this.pbClose_MouseEnter);
            this.pbClose.MouseLeave += new System.EventHandler(this.pbClose_MouseLeave);
            // 
            // pbOver
            // 
            this.pbOver.BackColor = System.Drawing.Color.Transparent;
            this.pbOver.BackgroundImage = global::PDF_Convert.Properties.Resources.close_01;
            this.pbOver.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pbOver.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbOver.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pbOver.Location = new System.Drawing.Point(236, 238);
            this.pbOver.Name = "pbOver";
            this.pbOver.Size = new System.Drawing.Size(108, 36);
            this.pbOver.TabIndex = 105;
            this.pbOver.TabStop = false;
            this.pbOver.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pbOver_MouseClick);
            this.pbOver.MouseEnter += new System.EventHandler(this.pbOver_MouseEnter);
            this.pbOver.MouseLeave += new System.EventHandler(this.pbOver_MouseLeave);
            // 
            // txtPassword
            // 
            this.txtPassword.Font = new System.Drawing.Font("宋体", 16F);
            this.txtPassword.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(122)))), ((int)(((byte)(245)))));
            this.txtPassword.Location = new System.Drawing.Point(139, 180);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(223, 32);
            this.txtPassword.TabIndex = 109;
            // 
            // lblFileName
            // 
            this.lblFileName.AutoSize = true;
            this.lblFileName.BackColor = System.Drawing.Color.Transparent;
            this.lblFileName.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.lblFileName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(122)))), ((int)(((byte)(245)))));
            this.lblFileName.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblFileName.Location = new System.Drawing.Point(139, 135);
            this.lblFileName.Name = "lblFileName";
            this.lblFileName.Size = new System.Drawing.Size(43, 20);
            this.lblFileName.TabIndex = 108;
            this.lblFileName.Text = "1.pdf";
            // 
            // lblCursorPosition
            // 
            this.lblCursorPosition.AutoSize = true;
            this.lblCursorPosition.BackColor = System.Drawing.Color.Transparent;
            this.lblCursorPosition.Location = new System.Drawing.Point(277, 90);
            this.lblCursorPosition.Name = "lblCursorPosition";
            this.lblCursorPosition.Size = new System.Drawing.Size(89, 12);
            this.lblCursorPosition.TabIndex = 119;
            this.lblCursorPosition.Text = "CursorPosition";
            this.lblCursorPosition.UseWaitCursor = true;
            this.lblCursorPosition.Visible = false;
            // 
            // PassWordDlg01
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(420, 300);
            this.Controls.Add(this.lblCursorPosition);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.lblFileName);
            this.Controls.Add(this.pbConfirm);
            this.Controls.Add(this.pbClose);
            this.Controls.Add(this.pbOver);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PassWordDlg01";
            this.Text = "PassWordDlg01";
            this.Load += new System.EventHandler(this.PassWordDlg01_Load);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PassWordDlg01_MouseMove);
            ((System.ComponentModel.ISupportInitialize)(this.pbConfirm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbClose)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbOver)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pbConfirm;
        private System.Windows.Forms.PictureBox pbClose;
        private System.Windows.Forms.PictureBox pbOver;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label lblFileName;
        private System.Windows.Forms.Label lblCursorPosition;
    }
}