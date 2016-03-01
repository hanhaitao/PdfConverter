namespace PDF_Convert
{
    partial class RegTipsOld
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RegTipsOld));
            this.pbClose = new System.Windows.Forms.PictureBox();
            this.btnOver = new PDF_Convert.ucPicBrowseBar();
            this.btnConfirm = new PDF_Convert.ucPicBrowseBar();
            this.lblTitle = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pbClose)).BeginInit();
            this.SuspendLayout();
            // 
            // pbClose
            // 
            this.pbClose.BackColor = System.Drawing.Color.Transparent;
            this.pbClose.BackgroundImage = global::PDF_Convert.Properties.Resources.close_01;
            this.pbClose.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.pbClose, "pbClose");
            this.pbClose.Name = "pbClose";
            this.pbClose.TabStop = false;
            this.pbClose.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pbClose_MouseClick);
            this.pbClose.MouseEnter += new System.EventHandler(this.pbClose_MouseEnter);
            this.pbClose.MouseLeave += new System.EventHandler(this.pbClose_MouseLeave);
            // 
            // btnOver
            // 
            resources.ApplyResources(this.btnOver, "btnOver");
            this.btnOver.BackColor = System.Drawing.Color.Transparent;
            this.btnOver.BackgroundImage = global::PDF_Convert.Properties.Resources.RegTips_btn_dialog_reg_01;
            this.btnOver.ButtonBackIMG = global::PDF_Convert.Properties.Resources.RegTips_btn_dialog_reg_01;
            this.btnOver.ButtonForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnOver.ButtonText = "注册并激活";
            this.btnOver.ButtonTextFont = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Bold);
            this.btnOver.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnOver.ForeColor = System.Drawing.Color.Coral;
            this.btnOver.IsEnable = true;
            this.btnOver.Name = "btnOver";
            this.btnOver.MouseClick += new System.Windows.Forms.MouseEventHandler(this.btnOver_MouseClick);
            this.btnOver.MouseEnter += new System.EventHandler(this.btnOver_MouseEnter);
            this.btnOver.MouseLeave += new System.EventHandler(this.btnOver_MouseLeave);
            // 
            // btnConfirm
            // 
            this.btnConfirm.BackColor = System.Drawing.Color.Transparent;
            this.btnConfirm.BackgroundImage = global::PDF_Convert.Properties.Resources.RegTips_btn_dialog_buy_01;
            resources.ApplyResources(this.btnConfirm, "btnConfirm");
            this.btnConfirm.ButtonBackIMG = global::PDF_Convert.Properties.Resources.RegTips_btn_dialog_buy_01;
            this.btnConfirm.ButtonForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnConfirm.ButtonText = "购买正式版";
            this.btnConfirm.ButtonTextFont = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Bold);
            this.btnConfirm.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConfirm.IsEnable = true;
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.MouseClick += new System.Windows.Forms.MouseEventHandler(this.btnConfirm_MouseClick);
            this.btnConfirm.MouseEnter += new System.EventHandler(this.btnConfirm_MouseEnter);
            this.btnConfirm.MouseLeave += new System.EventHandler(this.btnConfirm_MouseLeave);
            // 
            // lblTitle
            // 
            resources.ApplyResources(this.lblTitle, "lblTitle");
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RegTips_MouseDown);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this.label1.Name = "label1";
            this.label1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RegTips_MouseDown);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(177)))), ((int)(((byte)(159)))), ((int)(((byte)(26)))));
            this.label2.Name = "label2";
            this.label2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RegTips_MouseDown);
            // 
            // RegTips
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = global::PDF_Convert.Properties.Resources.RegTips_bg;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.pbClose);
            this.Controls.Add(this.btnOver);
            this.Controls.Add(this.btnConfirm);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "RegTips";
            this.Load += new System.EventHandler(this.ClearListTips_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RegTips_MouseDown);
            ((System.ComponentModel.ISupportInitialize)(this.pbClose)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pbClose;
        private ucPicBrowseBar btnConfirm;
        private ucPicBrowseBar btnOver;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}