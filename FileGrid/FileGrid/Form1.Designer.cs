namespace FileGrid
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.listViewPlus1 = new Controls.ListViewPlus();
            this.ucPicButton5 = new Controls.ucPicButton();
            this.ucPicButton4 = new Controls.ucPicButton();
            this.ucPicButton3 = new Controls.ucPicButton();
            this.ucPicButton2 = new Controls.ucPicButton();
            this.ucPicButton1 = new Controls.ucPicButton();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(139, 581);
            this.panel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 76);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(139, 54);
            this.panel2.TabIndex = 1;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(139, 76);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.ucPicButton5);
            this.panel3.Controls.Add(this.ucPicButton4);
            this.panel3.Controls.Add(this.ucPicButton3);
            this.panel3.Controls.Add(this.ucPicButton2);
            this.panel3.Controls.Add(this.ucPicButton1);
            this.panel3.Location = new System.Drawing.Point(145, 259);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(230, 273);
            this.panel3.TabIndex = 4;
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // listViewPlus1
            // 
            this.listViewPlus1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listViewPlus1.ConversionPageDefaultText = null;
            this.listViewPlus1.FileNameText = "文件名称";
            this.listViewPlus1.FileNameWidth = 275;
            this.listViewPlus1.Font = new System.Drawing.Font("宋体", 12F);
            this.listViewPlus1.FullRowSelect = true;
            this.listViewPlus1.GridLines = true;
            this.listViewPlus1.IndexText = "编号";
            this.listViewPlus1.IndexWidth = 55;
            this.listViewPlus1.ItemFont = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.listViewPlus1.Location = new System.Drawing.Point(145, 0);
            this.listViewPlus1.Name = "listViewPlus1";
            this.listViewPlus1.OpenButtonText = "打开";
            this.listViewPlus1.OperateText = "操作";
            this.listViewPlus1.OperateWidth = 130;
            this.listViewPlus1.OwnerDraw = true;
            this.listViewPlus1.SelectPageText = "选择页码";
            this.listViewPlus1.SelectPageWidth = 130;
            this.listViewPlus1.Size = new System.Drawing.Size(740, 253);
            this.listViewPlus1.StatusText = "状态";
            this.listViewPlus1.StatusWidth = 140;
            this.listViewPlus1.TabIndex = 5;
            this.listViewPlus1.UseCompatibleStateImageBehavior = false;
            this.listViewPlus1.View = System.Windows.Forms.View.Details;
            // 
            // ucPicButton5
            // 
            this.ucPicButton5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ucPicButton5.BottomLine = false;
            this.ucPicButton5.ButtonImage = ((System.Drawing.Image)(resources.GetObject("ucPicButton5.ButtonImage")));
            this.ucPicButton5.ButtonText = "Text";
            this.ucPicButton5.ButtonTextFont = new System.Drawing.Font("黑体", 14F);
            this.ucPicButton5.Dock = System.Windows.Forms.DockStyle.Top;
            this.ucPicButton5.Location = new System.Drawing.Point(0, 216);
            this.ucPicButton5.Name = "ucPicButton5";
            this.ucPicButton5.Selected = false;
            this.ucPicButton5.Size = new System.Drawing.Size(230, 54);
            this.ucPicButton5.TabIndex = 7;
            this.ucPicButton5.Click += new System.EventHandler(this.ucPicButton1_Click);
            // 
            // ucPicButton4
            // 
            this.ucPicButton4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ucPicButton4.BottomLine = false;
            this.ucPicButton4.ButtonImage = ((System.Drawing.Image)(resources.GetObject("ucPicButton4.ButtonImage")));
            this.ucPicButton4.ButtonText = "Text";
            this.ucPicButton4.ButtonTextFont = new System.Drawing.Font("黑体", 14F);
            this.ucPicButton4.Dock = System.Windows.Forms.DockStyle.Top;
            this.ucPicButton4.Location = new System.Drawing.Point(0, 162);
            this.ucPicButton4.Name = "ucPicButton4";
            this.ucPicButton4.Selected = false;
            this.ucPicButton4.Size = new System.Drawing.Size(230, 54);
            this.ucPicButton4.TabIndex = 6;
            this.ucPicButton4.Click += new System.EventHandler(this.ucPicButton1_Click);
            // 
            // ucPicButton3
            // 
            this.ucPicButton3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ucPicButton3.BottomLine = false;
            this.ucPicButton3.ButtonImage = ((System.Drawing.Image)(resources.GetObject("ucPicButton3.ButtonImage")));
            this.ucPicButton3.ButtonText = "Text";
            this.ucPicButton3.ButtonTextFont = new System.Drawing.Font("黑体", 14F);
            this.ucPicButton3.Dock = System.Windows.Forms.DockStyle.Top;
            this.ucPicButton3.Location = new System.Drawing.Point(0, 108);
            this.ucPicButton3.Name = "ucPicButton3";
            this.ucPicButton3.Selected = false;
            this.ucPicButton3.Size = new System.Drawing.Size(230, 54);
            this.ucPicButton3.TabIndex = 5;
            this.ucPicButton3.Click += new System.EventHandler(this.ucPicButton1_Click);
            // 
            // ucPicButton2
            // 
            this.ucPicButton2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ucPicButton2.BottomLine = false;
            this.ucPicButton2.ButtonImage = ((System.Drawing.Image)(resources.GetObject("ucPicButton2.ButtonImage")));
            this.ucPicButton2.ButtonText = "Text";
            this.ucPicButton2.ButtonTextFont = new System.Drawing.Font("黑体", 14F);
            this.ucPicButton2.Dock = System.Windows.Forms.DockStyle.Top;
            this.ucPicButton2.Location = new System.Drawing.Point(0, 54);
            this.ucPicButton2.Name = "ucPicButton2";
            this.ucPicButton2.Selected = false;
            this.ucPicButton2.Size = new System.Drawing.Size(230, 54);
            this.ucPicButton2.TabIndex = 4;
            this.ucPicButton2.Click += new System.EventHandler(this.ucPicButton1_Click);
            // 
            // ucPicButton1
            // 
            this.ucPicButton1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ucPicButton1.BottomLine = false;
            this.ucPicButton1.ButtonImage = ((System.Drawing.Image)(resources.GetObject("ucPicButton1.ButtonImage")));
            this.ucPicButton1.ButtonText = "Text";
            this.ucPicButton1.ButtonTextFont = new System.Drawing.Font("黑体", 14F);
            this.ucPicButton1.Dock = System.Windows.Forms.DockStyle.Top;
            this.ucPicButton1.Location = new System.Drawing.Point(0, 0);
            this.ucPicButton1.Name = "ucPicButton1";
            this.ucPicButton1.Selected = false;
            this.ucPicButton1.Size = new System.Drawing.Size(230, 54);
            this.ucPicButton1.TabIndex = 3;
            this.ucPicButton1.Click += new System.EventHandler(this.ucPicButton1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1044, 581);
            this.Controls.Add(this.listViewPlus1);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private Controls.ucPicButton ucPicButton1;
        private System.Windows.Forms.Panel panel3;
        private Controls.ucPicButton ucPicButton5;
        private Controls.ucPicButton ucPicButton4;
        private Controls.ucPicButton ucPicButton3;
        private Controls.ucPicButton ucPicButton2;
        private System.Windows.Forms.ImageList imageList1;
        private Controls.ListViewPlus listViewPlus1;

    }
}