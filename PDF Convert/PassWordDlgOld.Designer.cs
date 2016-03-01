namespace PDF_Convert
{
    partial class PassWordDlgOld
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PassWordDlgOld));
            this.pbClose = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lblFileName = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.btnCancel = new PDF_Convert.ucPicBrowseBar();
            this.btnConfirm = new PDF_Convert.ucPicBrowseBar();
            ((System.ComponentModel.ISupportInitialize)(this.pbClose)).BeginInit();
            this.SuspendLayout();
            // 
            // pbClose
            // 
            this.pbClose.BackColor = System.Drawing.Color.Transparent;
            this.pbClose.BackgroundImage = global::PDF_Convert.Properties.Resources.regClose;
            this.pbClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbClose.Image = global::PDF_Convert.Properties.Resources.close_03;
            resources.ApplyResources(this.pbClose, "pbClose");
            this.pbClose.Name = "pbClose";
            this.pbClose.TabStop = false;
            this.pbClose.Click += new System.EventHandler(this.pbClose_Click);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(147)))), ((int)(((byte)(150)))), ((int)(((byte)(150)))));
            this.label2.Name = "label2";
            this.label2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PassWordDlg_MouseDown);
            // 
            // lblFileName
            // 
            resources.ApplyResources(this.lblFileName, "lblFileName");
            this.lblFileName.BackColor = System.Drawing.Color.Transparent;
            this.lblFileName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(122)))), ((int)(((byte)(245)))));
            this.lblFileName.Name = "lblFileName";
            this.lblFileName.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PassWordDlg_MouseDown);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Name = "label1";
            this.label1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PassWordDlg_MouseDown);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label4.Name = "label4";
            this.label4.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PassWordDlg_MouseDown);
            // 
            // lblTitle
            // 
            resources.ApplyResources(this.lblTitle, "lblTitle");
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PassWordDlg_MouseDown);
            // 
            // txtPassword
            // 
            resources.ApplyResources(this.txtPassword, "txtPassword");
            this.txtPassword.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(122)))), ((int)(((byte)(245)))));
            this.txtPassword.Name = "txtPassword";
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.BackColor = System.Drawing.Color.Transparent;
            this.btnCancel.BackgroundImage = global::PDF_Convert.Properties.Resources.cancel_01;
            this.btnCancel.ButtonBackIMG = global::PDF_Convert.Properties.Resources.cancel_01;
            this.btnCancel.ButtonForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnCancel.ButtonText = "取消";
            this.btnCancel.ButtonTextFont = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold);
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.IsEnable = true;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ucPicBrowseBar1_MouseClick);
            this.btnCancel.MouseEnter += new System.EventHandler(this.btnCancel_MouseEnter);
            this.btnCancel.MouseLeave += new System.EventHandler(this.btnCancel_MouseLeave);
            // 
            // btnConfirm
            // 
            this.btnConfirm.BackColor = System.Drawing.Color.Transparent;
            this.btnConfirm.BackgroundImage = global::PDF_Convert.Properties.Resources.sure_01;
            resources.ApplyResources(this.btnConfirm, "btnConfirm");
            this.btnConfirm.ButtonBackIMG = global::PDF_Convert.Properties.Resources.sure_01;
            this.btnConfirm.ButtonForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnConfirm.ButtonText = "确定";
            this.btnConfirm.ButtonTextFont = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold);
            this.btnConfirm.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConfirm.IsEnable = true;
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.MouseClick += new System.Windows.Forms.MouseEventHandler(this.btnConfirm_MouseClick);
            this.btnConfirm.MouseEnter += new System.EventHandler(this.btnConfirm_MouseEnter);
            this.btnConfirm.MouseLeave += new System.EventHandler(this.btnConfirm_MouseLeave);
            // 
            // PassWordDlg
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = global::PDF_Convert.Properties.Resources.dialog_ad_code;
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.pbClose);
            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblFileName);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PassWordDlg";
            this.Load += new System.EventHandler(this.PassWordDlg_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PassWordDlg_MouseDown);
            ((System.ComponentModel.ISupportInitialize)(this.pbClose)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblFileName;
        private System.Windows.Forms.PictureBox pbClose;
        private ucPicBrowseBar btnConfirm;
        private ucPicBrowseBar btnCancel;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.TextBox txtPassword;
    }
}