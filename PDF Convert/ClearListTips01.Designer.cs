namespace PDF_Convert
{
    partial class ClearListTips01
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClearListTips01));
            this.pbConfirm = new System.Windows.Forms.PictureBox();
            this.pbClose = new System.Windows.Forms.PictureBox();
            this.pbOver = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbConfirm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbClose)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbOver)).BeginInit();
            this.SuspendLayout();
            // 
            // pbConfirm
            // 
            this.pbConfirm.BackColor = System.Drawing.Color.Transparent;
            this.pbConfirm.BackgroundImage = global::PDF_Convert.Properties.Resources.close_01;
            resources.ApplyResources(this.pbConfirm, "pbConfirm");
            this.pbConfirm.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbConfirm.Name = "pbConfirm";
            this.pbConfirm.TabStop = false;
            this.pbConfirm.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pbConfirm_MouseClick);
            this.pbConfirm.MouseEnter += new System.EventHandler(this.pbConfirm_MouseEnter);
            this.pbConfirm.MouseLeave += new System.EventHandler(this.pbConfirm_MouseLeave);
            // 
            // pbClose
            // 
            this.pbClose.BackColor = System.Drawing.Color.Transparent;
            this.pbClose.BackgroundImage = global::PDF_Convert.Properties.Resources.close_01;
            resources.ApplyResources(this.pbClose, "pbClose");
            this.pbClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbClose.Name = "pbClose";
            this.pbClose.TabStop = false;
            this.pbClose.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pbOver_MouseClick);
            this.pbClose.MouseEnter += new System.EventHandler(this.pbClose_MouseEnter);
            this.pbClose.MouseLeave += new System.EventHandler(this.pbClose_MouseLeave);
            // 
            // pbOver
            // 
            this.pbOver.BackColor = System.Drawing.Color.Transparent;
            this.pbOver.BackgroundImage = global::PDF_Convert.Properties.Resources.close_01;
            resources.ApplyResources(this.pbOver, "pbOver");
            this.pbOver.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbOver.Name = "pbOver";
            this.pbOver.TabStop = false;
            this.pbOver.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pbOver_MouseClick);
            this.pbOver.MouseEnter += new System.EventHandler(this.pbOver_MouseEnter);
            this.pbOver.MouseLeave += new System.EventHandler(this.pbOver_MouseLeave);
            // 
            // ClearListTips01
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.pbConfirm);
            this.Controls.Add(this.pbClose);
            this.Controls.Add(this.pbOver);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ClearListTips01";
            this.Load += new System.EventHandler(this.ClearListTips01_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ClearListTips01_KeyDown);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ClearListTips01_MouseDown);
            ((System.ComponentModel.ISupportInitialize)(this.pbConfirm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbClose)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbOver)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pbOver;
        private System.Windows.Forms.PictureBox pbClose;
        private System.Windows.Forms.PictureBox pbConfirm;
    }
}