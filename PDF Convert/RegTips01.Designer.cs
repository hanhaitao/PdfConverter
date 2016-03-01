namespace PDF_Convert
{
    partial class RegTips01
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RegTips01));
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
            this.pbConfirm.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pbConfirm.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbConfirm.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pbConfirm.Location = new System.Drawing.Point(66, 188);
            this.pbConfirm.Name = "pbConfirm";
            this.pbConfirm.Size = new System.Drawing.Size(125, 36);
            this.pbConfirm.TabIndex = 110;
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
            this.pbClose.Location = new System.Drawing.Point(393, 12);
            this.pbClose.Name = "pbClose";
            this.pbClose.Size = new System.Drawing.Size(16, 16);
            this.pbClose.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbClose.TabIndex = 109;
            this.pbClose.TabStop = false;
            this.pbClose.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pbClose_MouseClick);
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
            this.pbOver.Location = new System.Drawing.Point(234, 188);
            this.pbOver.Name = "pbOver";
            this.pbOver.Size = new System.Drawing.Size(125, 36);
            this.pbOver.TabIndex = 108;
            this.pbOver.TabStop = false;
            this.pbOver.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pbOver_MouseClick);
            this.pbOver.MouseEnter += new System.EventHandler(this.pbOver_MouseEnter);
            this.pbOver.MouseLeave += new System.EventHandler(this.pbOver_MouseLeave);
            // 
            // RegTips01
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(419, 240);
            this.Controls.Add(this.pbConfirm);
            this.Controls.Add(this.pbClose);
            this.Controls.Add(this.pbOver);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "RegTips01";
            this.Text = "RegTips01";
            this.Load += new System.EventHandler(this.RegTips01_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RegTips01_MouseDown);
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
    }
}