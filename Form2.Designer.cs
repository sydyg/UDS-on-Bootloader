namespace UDS上位机
{
    partial class Form2
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form2));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.设备选择ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uSBCANToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uSBCAN1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uSBCAN2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uSBCAN2EUToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pCANToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.vN1600ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.vspyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.kvaserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.通道选择ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cAN1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cAN2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.波特率ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.kToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.kToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.kToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.帧类型ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.标准帧ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.扩展帧ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.设备选择ToolStripMenuItem,
            this.通道选择ToolStripMenuItem,
            this.波特率ToolStripMenuItem,
            this.帧类型ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(796, 28);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 设备选择ToolStripMenuItem
            // 
            this.设备选择ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.uSBCANToolStripMenuItem,
            this.pCANToolStripMenuItem,
            this.vN1600ToolStripMenuItem,
            this.vspyToolStripMenuItem,
            this.kvaserToolStripMenuItem});
            this.设备选择ToolStripMenuItem.Name = "设备选择ToolStripMenuItem";
            this.设备选择ToolStripMenuItem.Size = new System.Drawing.Size(83, 24);
            this.设备选择ToolStripMenuItem.Text = "设备选择";
            // 
            // uSBCANToolStripMenuItem
            // 
            this.uSBCANToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.uSBCAN1ToolStripMenuItem,
            this.uSBCAN2ToolStripMenuItem,
            this.uSBCAN2EUToolStripMenuItem});
            this.uSBCANToolStripMenuItem.Name = "uSBCANToolStripMenuItem";
            this.uSBCANToolStripMenuItem.Size = new System.Drawing.Size(154, 26);
            this.uSBCANToolStripMenuItem.Text = "USBCAN";
            // 
            // uSBCAN1ToolStripMenuItem
            // 
            this.uSBCAN1ToolStripMenuItem.Name = "uSBCAN1ToolStripMenuItem";
            this.uSBCAN1ToolStripMenuItem.Size = new System.Drawing.Size(182, 26);
            this.uSBCAN1ToolStripMenuItem.Text = "USBCAN1";
            this.uSBCAN1ToolStripMenuItem.Click += new System.EventHandler(this.uSBCAN1ToolStripMenuItem_Click);
            // 
            // uSBCAN2ToolStripMenuItem
            // 
            this.uSBCAN2ToolStripMenuItem.Name = "uSBCAN2ToolStripMenuItem";
            this.uSBCAN2ToolStripMenuItem.Size = new System.Drawing.Size(182, 26);
            this.uSBCAN2ToolStripMenuItem.Text = "USBCAN2";
            this.uSBCAN2ToolStripMenuItem.Click += new System.EventHandler(this.uSBCAN2ToolStripMenuItem_Click);
            // 
            // uSBCAN2EUToolStripMenuItem
            // 
            this.uSBCAN2EUToolStripMenuItem.Name = "uSBCAN2EUToolStripMenuItem";
            this.uSBCAN2EUToolStripMenuItem.Size = new System.Drawing.Size(182, 26);
            this.uSBCAN2EUToolStripMenuItem.Text = "USBCAN2EU";
            this.uSBCAN2EUToolStripMenuItem.Click += new System.EventHandler(this.uSBCAN2EUToolStripMenuItem_Click);
            // 
            // pCANToolStripMenuItem
            // 
            this.pCANToolStripMenuItem.Name = "pCANToolStripMenuItem";
            this.pCANToolStripMenuItem.Size = new System.Drawing.Size(154, 26);
            this.pCANToolStripMenuItem.Text = "PCAN";
            this.pCANToolStripMenuItem.Click += new System.EventHandler(this.pCANToolStripMenuItem_Click);
            // 
            // vN1600ToolStripMenuItem
            // 
            this.vN1600ToolStripMenuItem.Name = "vN1600ToolStripMenuItem";
            this.vN1600ToolStripMenuItem.Size = new System.Drawing.Size(154, 26);
            this.vN1600ToolStripMenuItem.Text = "VN1600";
            this.vN1600ToolStripMenuItem.Click += new System.EventHandler(this.vN1600ToolStripMenuItem_Click);
            // 
            // vspyToolStripMenuItem
            // 
            this.vspyToolStripMenuItem.Name = "vspyToolStripMenuItem";
            this.vspyToolStripMenuItem.Size = new System.Drawing.Size(154, 26);
            this.vspyToolStripMenuItem.Text = "Vspy";
            this.vspyToolStripMenuItem.Click += new System.EventHandler(this.vspyToolStripMenuItem_Click);
            // 
            // kvaserToolStripMenuItem
            // 
            this.kvaserToolStripMenuItem.Name = "kvaserToolStripMenuItem";
            this.kvaserToolStripMenuItem.Size = new System.Drawing.Size(154, 26);
            this.kvaserToolStripMenuItem.Text = "Kvaser";
            this.kvaserToolStripMenuItem.Click += new System.EventHandler(this.kvaserToolStripMenuItem_Click);
            // 
            // 通道选择ToolStripMenuItem
            // 
            this.通道选择ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cAN1ToolStripMenuItem,
            this.cAN2ToolStripMenuItem});
            this.通道选择ToolStripMenuItem.Name = "通道选择ToolStripMenuItem";
            this.通道选择ToolStripMenuItem.Size = new System.Drawing.Size(83, 24);
            this.通道选择ToolStripMenuItem.Text = "通道选择";
            // 
            // cAN1ToolStripMenuItem
            // 
            this.cAN1ToolStripMenuItem.Name = "cAN1ToolStripMenuItem";
            this.cAN1ToolStripMenuItem.Size = new System.Drawing.Size(134, 26);
            this.cAN1ToolStripMenuItem.Text = "CAN1";
            this.cAN1ToolStripMenuItem.Click += new System.EventHandler(this.cAN1ToolStripMenuItem_Click);
            // 
            // cAN2ToolStripMenuItem
            // 
            this.cAN2ToolStripMenuItem.Name = "cAN2ToolStripMenuItem";
            this.cAN2ToolStripMenuItem.Size = new System.Drawing.Size(134, 26);
            this.cAN2ToolStripMenuItem.Text = "CAN2";
            this.cAN2ToolStripMenuItem.Click += new System.EventHandler(this.cAN2ToolStripMenuItem_Click);
            // 
            // 波特率ToolStripMenuItem
            // 
            this.波特率ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.kToolStripMenuItem,
            this.kToolStripMenuItem1,
            this.kToolStripMenuItem2});
            this.波特率ToolStripMenuItem.Name = "波特率ToolStripMenuItem";
            this.波特率ToolStripMenuItem.Size = new System.Drawing.Size(68, 24);
            this.波特率ToolStripMenuItem.Text = "波特率";
            // 
            // kToolStripMenuItem
            // 
            this.kToolStripMenuItem.Name = "kToolStripMenuItem";
            this.kToolStripMenuItem.Size = new System.Drawing.Size(138, 26);
            this.kToolStripMenuItem.Text = "250K";
            this.kToolStripMenuItem.Click += new System.EventHandler(this.kToolStripMenuItem_Click);
            // 
            // kToolStripMenuItem1
            // 
            this.kToolStripMenuItem1.Name = "kToolStripMenuItem1";
            this.kToolStripMenuItem1.Size = new System.Drawing.Size(138, 26);
            this.kToolStripMenuItem1.Text = "500K";
            this.kToolStripMenuItem1.Click += new System.EventHandler(this.kToolStripMenuItem1_Click);
            // 
            // kToolStripMenuItem2
            // 
            this.kToolStripMenuItem2.Name = "kToolStripMenuItem2";
            this.kToolStripMenuItem2.Size = new System.Drawing.Size(138, 26);
            this.kToolStripMenuItem2.Text = "1000K";
            this.kToolStripMenuItem2.Click += new System.EventHandler(this.kToolStripMenuItem2_Click);
            // 
            // 帧类型ToolStripMenuItem
            // 
            this.帧类型ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.标准帧ToolStripMenuItem,
            this.扩展帧ToolStripMenuItem});
            this.帧类型ToolStripMenuItem.Name = "帧类型ToolStripMenuItem";
            this.帧类型ToolStripMenuItem.Size = new System.Drawing.Size(68, 24);
            this.帧类型ToolStripMenuItem.Text = "帧类型";
            // 
            // 标准帧ToolStripMenuItem
            // 
            this.标准帧ToolStripMenuItem.Name = "标准帧ToolStripMenuItem";
            this.标准帧ToolStripMenuItem.Size = new System.Drawing.Size(137, 26);
            this.标准帧ToolStripMenuItem.Text = "标准帧";
            this.标准帧ToolStripMenuItem.Click += new System.EventHandler(this.标准帧ToolStripMenuItem_Click);
            // 
            // 扩展帧ToolStripMenuItem
            // 
            this.扩展帧ToolStripMenuItem.Name = "扩展帧ToolStripMenuItem";
            this.扩展帧ToolStripMenuItem.Size = new System.Drawing.Size(137, 26);
            this.扩展帧ToolStripMenuItem.Text = "扩展帧";
            this.扩展帧ToolStripMenuItem.Click += new System.EventHandler(this.扩展帧ToolStripMenuItem_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripButton2});
            this.toolStrip1.Location = new System.Drawing.Point(0, 28);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(796, 27);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(73, 24);
            this.toolStripButton1.Text = "启动设备";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(73, 24);
            this.toolStripButton2.Text = "关闭设备";
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(16, 268);
            this.textBox3.Multiline = true;
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(768, 170);
            this.textBox3.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tabPage1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tabPage1.Controls.Add(this.button3);
            this.tabPage1.Controls.Add(this.button2);
            this.tabPage1.Controls.Add(this.button1);
            this.tabPage1.Controls.Add(this.progressBar1);
            this.tabPage1.Controls.Add(this.textBox2);
            this.tabPage1.Controls.Add(this.textBox1);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Font = new System.Drawing.Font("微软雅黑", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(768, 166);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "bootloader";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.label1.Location = new System.Drawing.Point(24, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "App";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.label2.Location = new System.Drawing.Point(24, 71);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 20);
            this.label2.TabIndex = 0;
            this.label2.Text = "Driver";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.label3.Location = new System.Drawing.Point(24, 118);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 20);
            this.label3.TabIndex = 0;
            this.label3.Text = "progress";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(113, 23);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(405, 31);
            this.textBox1.TabIndex = 1;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(113, 65);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(405, 31);
            this.textBox2.TabIndex = 1;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(113, 118);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(405, 23);
            this.progressBar1.TabIndex = 2;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.Transparent;
            this.button1.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.button1.Location = new System.Drawing.Point(548, 23);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(86, 31);
            this.button1.TabIndex = 3;
            this.button1.Text = "App";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.button2.Location = new System.Drawing.Point(548, 65);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(86, 31);
            this.button2.TabIndex = 3;
            this.button2.Text = "Driver";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.button3.Location = new System.Drawing.Point(548, 113);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(86, 31);
            this.button3.TabIndex = 3;
            this.button3.Text = "开始";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(768, 166);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(16, 67);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(776, 195);
            this.tabControl1.TabIndex = 2;
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(796, 450);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form2";
            this.Text = "Form2";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 设备选择ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uSBCANToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uSBCAN1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uSBCAN2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uSBCAN2EUToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pCANToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem vN1600ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem vspyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem kvaserToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 通道选择ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 波特率ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 帧类型ToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        public System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.ToolStripMenuItem cAN1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cAN2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem kToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem kToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem kToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem 标准帧ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 扩展帧ToolStripMenuItem;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabControl tabControl1;
    }
}