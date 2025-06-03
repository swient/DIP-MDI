namespace DIP
{
    partial class DIPSample
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.檔案ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.開啟ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.儲存ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.編輯ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.灰階轉換toolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.負片轉換ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.均衡化ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.二值化toolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.連通標記ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.視圖ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.直方圖ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.圖像toolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.亮度ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.對比度ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.局部馬賽克ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.旋轉圖像ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.向右旋轉90度ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.向右旋轉180度ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.向右旋轉270度ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.自訂旋轉角度ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.翻轉圖像ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.水平翻轉ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.垂直翻轉ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.放大圖像ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.縮小圖像ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.濾波器ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.平均濾波器ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.高通濾波器ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.自訂濾波器ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.檔案ToolStripMenuItem,
            this.編輯ToolStripMenuItem,
            this.視圖ToolStripMenuItem,
            this.圖像toolStripMenuItem,
            this.濾波器ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1578, 32);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 檔案ToolStripMenuItem
            // 
            this.檔案ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.開啟ToolStripMenuItem,
            this.儲存ToolStripMenuItem});
            this.檔案ToolStripMenuItem.Name = "檔案ToolStripMenuItem";
            this.檔案ToolStripMenuItem.Size = new System.Drawing.Size(62, 28);
            this.檔案ToolStripMenuItem.Text = "檔案";
            // 
            // 開啟ToolStripMenuItem
            // 
            this.開啟ToolStripMenuItem.Name = "開啟ToolStripMenuItem";
            this.開啟ToolStripMenuItem.Size = new System.Drawing.Size(146, 34);
            this.開啟ToolStripMenuItem.Text = "開啟";
            this.開啟ToolStripMenuItem.Click += new System.EventHandler(this.開啟ToolStripMenuItem_Click);
            // 
            // 儲存ToolStripMenuItem
            // 
            this.儲存ToolStripMenuItem.Name = "儲存ToolStripMenuItem";
            this.儲存ToolStripMenuItem.Size = new System.Drawing.Size(146, 34);
            this.儲存ToolStripMenuItem.Text = "儲存";
            this.儲存ToolStripMenuItem.Click += new System.EventHandler(this.儲存ToolStripMenuItem_Click);
            // 
            // 編輯ToolStripMenuItem
            // 
            this.編輯ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.灰階轉換toolStripMenuItem,
            this.負片轉換ToolStripMenuItem,
            this.均衡化ToolStripMenuItem,
            this.二值化toolStripMenuItem,
            this.連通標記ToolStripMenuItem});
            this.編輯ToolStripMenuItem.Name = "編輯ToolStripMenuItem";
            this.編輯ToolStripMenuItem.Size = new System.Drawing.Size(62, 28);
            this.編輯ToolStripMenuItem.Text = "編輯";
            // 
            // 灰階轉換toolStripMenuItem
            // 
            this.灰階轉換toolStripMenuItem.Name = "灰階轉換toolStripMenuItem";
            this.灰階轉換toolStripMenuItem.Size = new System.Drawing.Size(270, 34);
            this.灰階轉換toolStripMenuItem.Text = "灰階轉換";
            this.灰階轉換toolStripMenuItem.Click += new System.EventHandler(this.灰階轉換toolStripMenuItem_Click);
            // 
            // 負片轉換ToolStripMenuItem
            // 
            this.負片轉換ToolStripMenuItem.Name = "負片轉換ToolStripMenuItem";
            this.負片轉換ToolStripMenuItem.Size = new System.Drawing.Size(270, 34);
            this.負片轉換ToolStripMenuItem.Text = "負片轉換";
            this.負片轉換ToolStripMenuItem.Click += new System.EventHandler(this.負片轉換ToolStripMenuItem_Click);
            // 
            // 均衡化ToolStripMenuItem
            // 
            this.均衡化ToolStripMenuItem.Name = "均衡化ToolStripMenuItem";
            this.均衡化ToolStripMenuItem.Size = new System.Drawing.Size(270, 34);
            this.均衡化ToolStripMenuItem.Text = "均衡化";
            this.均衡化ToolStripMenuItem.Click += new System.EventHandler(this.均衡化ToolStripMenuItem_Click);
            // 
            // 二值化toolStripMenuItem
            // 
            this.二值化toolStripMenuItem.Name = "二值化toolStripMenuItem";
            this.二值化toolStripMenuItem.Size = new System.Drawing.Size(270, 34);
            this.二值化toolStripMenuItem.Text = "二值化";
            this.二值化toolStripMenuItem.Click += new System.EventHandler(this.二值化toolStripMenuItem_Click);
            // 
            // 連通標記ToolStripMenuItem
            // 
            this.連通標記ToolStripMenuItem.Name = "連通標記ToolStripMenuItem";
            this.連通標記ToolStripMenuItem.Size = new System.Drawing.Size(270, 34);
            this.連通標記ToolStripMenuItem.Text = "連通標記";
            this.連通標記ToolStripMenuItem.Click += new System.EventHandler(this.連通標記toolStripMenuItem_Click);
            // 
            // 視圖ToolStripMenuItem
            // 
            this.視圖ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.直方圖ToolStripMenuItem});
            this.視圖ToolStripMenuItem.Name = "視圖ToolStripMenuItem";
            this.視圖ToolStripMenuItem.Size = new System.Drawing.Size(62, 28);
            this.視圖ToolStripMenuItem.Text = "視圖";
            // 
            // 直方圖ToolStripMenuItem
            // 
            this.直方圖ToolStripMenuItem.Name = "直方圖ToolStripMenuItem";
            this.直方圖ToolStripMenuItem.Size = new System.Drawing.Size(164, 34);
            this.直方圖ToolStripMenuItem.Text = "直方圖";
            this.直方圖ToolStripMenuItem.Click += new System.EventHandler(this.直方圖ToolStripMenuItem_Click);
            // 
            // 圖像toolStripMenuItem
            // 
            this.圖像toolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.亮度ToolStripMenuItem,
            this.對比度ToolStripMenuItem,
            this.局部馬賽克ToolStripMenuItem,
            this.旋轉圖像ToolStripMenuItem,
            this.翻轉圖像ToolStripMenuItem,
            this.放大圖像ToolStripMenuItem,
            this.縮小圖像ToolStripMenuItem});
            this.圖像toolStripMenuItem.Name = "圖像toolStripMenuItem";
            this.圖像toolStripMenuItem.Size = new System.Drawing.Size(62, 28);
            this.圖像toolStripMenuItem.Text = "圖像";
            // 
            // 亮度ToolStripMenuItem
            // 
            this.亮度ToolStripMenuItem.Name = "亮度ToolStripMenuItem";
            this.亮度ToolStripMenuItem.Size = new System.Drawing.Size(200, 34);
            this.亮度ToolStripMenuItem.Text = "亮度";
            this.亮度ToolStripMenuItem.Click += new System.EventHandler(this.亮度ToolStripMenuItem_Click);
            // 
            // 對比度ToolStripMenuItem
            // 
            this.對比度ToolStripMenuItem.Name = "對比度ToolStripMenuItem";
            this.對比度ToolStripMenuItem.Size = new System.Drawing.Size(200, 34);
            this.對比度ToolStripMenuItem.Text = "對比度";
            this.對比度ToolStripMenuItem.Click += new System.EventHandler(this.對比度ToolStripMenuItem_Click);
            // 
            // 局部馬賽克ToolStripMenuItem
            // 
            this.局部馬賽克ToolStripMenuItem.Name = "局部馬賽克ToolStripMenuItem";
            this.局部馬賽克ToolStripMenuItem.Size = new System.Drawing.Size(200, 34);
            this.局部馬賽克ToolStripMenuItem.Text = "局部馬賽克";
            this.局部馬賽克ToolStripMenuItem.Click += new System.EventHandler(this.局部馬賽克ToolStripMenuItem_Click);
            // 
            // 旋轉圖像ToolStripMenuItem
            // 
            this.旋轉圖像ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.向右旋轉90度ToolStripMenuItem,
            this.向右旋轉180度ToolStripMenuItem,
            this.向右旋轉270度ToolStripMenuItem,
            this.toolStripSeparator2,
            this.自訂旋轉角度ToolStripMenuItem});
            this.旋轉圖像ToolStripMenuItem.Name = "旋轉圖像ToolStripMenuItem";
            this.旋轉圖像ToolStripMenuItem.Size = new System.Drawing.Size(200, 34);
            this.旋轉圖像ToolStripMenuItem.Text = "旋轉圖像";
            // 
            // 向右旋轉90度ToolStripMenuItem
            // 
            this.向右旋轉90度ToolStripMenuItem.Name = "向右旋轉90度ToolStripMenuItem";
            this.向右旋轉90度ToolStripMenuItem.Size = new System.Drawing.Size(230, 34);
            this.向右旋轉90度ToolStripMenuItem.Text = "向右旋轉90度";
            this.向右旋轉90度ToolStripMenuItem.Click += new System.EventHandler(this.向右旋轉90度ToolStripMenuItem_Click);
            // 
            // 向右旋轉180度ToolStripMenuItem
            // 
            this.向右旋轉180度ToolStripMenuItem.Name = "向右旋轉180度ToolStripMenuItem";
            this.向右旋轉180度ToolStripMenuItem.Size = new System.Drawing.Size(230, 34);
            this.向右旋轉180度ToolStripMenuItem.Text = "向右旋轉180度";
            this.向右旋轉180度ToolStripMenuItem.Click += new System.EventHandler(this.向右旋轉180度ToolStripMenuItem_Click);
            // 
            // 向右旋轉270度ToolStripMenuItem
            // 
            this.向右旋轉270度ToolStripMenuItem.Name = "向右旋轉270度ToolStripMenuItem";
            this.向右旋轉270度ToolStripMenuItem.Size = new System.Drawing.Size(230, 34);
            this.向右旋轉270度ToolStripMenuItem.Text = "向右旋轉270度";
            this.向右旋轉270度ToolStripMenuItem.Click += new System.EventHandler(this.向右旋轉270度ToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(227, 6);
            // 
            // 自訂旋轉角度ToolStripMenuItem
            // 
            this.自訂旋轉角度ToolStripMenuItem.Name = "自訂旋轉角度ToolStripMenuItem";
            this.自訂旋轉角度ToolStripMenuItem.Size = new System.Drawing.Size(230, 34);
            this.自訂旋轉角度ToolStripMenuItem.Text = "自訂旋轉角度";
            this.自訂旋轉角度ToolStripMenuItem.Click += new System.EventHandler(this.自訂旋轉角度ToolStripMenuItem_Click);
            // 
            // 翻轉圖像ToolStripMenuItem
            // 
            this.翻轉圖像ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.水平翻轉ToolStripMenuItem,
            this.垂直翻轉ToolStripMenuItem});
            this.翻轉圖像ToolStripMenuItem.Name = "翻轉圖像ToolStripMenuItem";
            this.翻轉圖像ToolStripMenuItem.Size = new System.Drawing.Size(200, 34);
            this.翻轉圖像ToolStripMenuItem.Text = "翻轉圖像";
            // 
            // 水平翻轉ToolStripMenuItem
            // 
            this.水平翻轉ToolStripMenuItem.Name = "水平翻轉ToolStripMenuItem";
            this.水平翻轉ToolStripMenuItem.Size = new System.Drawing.Size(182, 34);
            this.水平翻轉ToolStripMenuItem.Text = "水平翻轉";
            this.水平翻轉ToolStripMenuItem.Click += new System.EventHandler(this.水平翻轉ToolStripMenuItem_Click);
            // 
            // 垂直翻轉ToolStripMenuItem
            // 
            this.垂直翻轉ToolStripMenuItem.Name = "垂直翻轉ToolStripMenuItem";
            this.垂直翻轉ToolStripMenuItem.Size = new System.Drawing.Size(182, 34);
            this.垂直翻轉ToolStripMenuItem.Text = "垂直翻轉";
            this.垂直翻轉ToolStripMenuItem.Click += new System.EventHandler(this.垂直翻轉ToolStripMenuItem_Click);
            // 
            // 放大圖像ToolStripMenuItem
            // 
            this.放大圖像ToolStripMenuItem.Name = "放大圖像ToolStripMenuItem";
            this.放大圖像ToolStripMenuItem.Size = new System.Drawing.Size(200, 34);
            this.放大圖像ToolStripMenuItem.Text = "放大圖像";
            this.放大圖像ToolStripMenuItem.Click += new System.EventHandler(this.放大圖像ToolStripMenuItem_Click);
            // 
            // 縮小圖像ToolStripMenuItem
            // 
            this.縮小圖像ToolStripMenuItem.Name = "縮小圖像ToolStripMenuItem";
            this.縮小圖像ToolStripMenuItem.Size = new System.Drawing.Size(200, 34);
            this.縮小圖像ToolStripMenuItem.Text = "縮小圖像";
            this.縮小圖像ToolStripMenuItem.Click += new System.EventHandler(this.縮小圖像ToolStripMenuItem_Click);
            // 
            // 濾波器ToolStripMenuItem
            // 
            this.濾波器ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.平均濾波器ToolStripMenuItem,
            this.高通濾波器ToolStripMenuItem,
            this.toolStripSeparator1,
            this.自訂濾波器ToolStripMenuItem});
            this.濾波器ToolStripMenuItem.Name = "濾波器ToolStripMenuItem";
            this.濾波器ToolStripMenuItem.Size = new System.Drawing.Size(80, 28);
            this.濾波器ToolStripMenuItem.Text = "濾波器";
            // 
            // 平均濾波器ToolStripMenuItem
            // 
            this.平均濾波器ToolStripMenuItem.Name = "平均濾波器ToolStripMenuItem";
            this.平均濾波器ToolStripMenuItem.Size = new System.Drawing.Size(200, 34);
            this.平均濾波器ToolStripMenuItem.Text = "平均濾波器";
            this.平均濾波器ToolStripMenuItem.Click += new System.EventHandler(this.平均濾波器ToolStripMenuItem_Click);
            // 
            // 高通濾波器ToolStripMenuItem
            // 
            this.高通濾波器ToolStripMenuItem.Name = "高通濾波器ToolStripMenuItem";
            this.高通濾波器ToolStripMenuItem.Size = new System.Drawing.Size(200, 34);
            this.高通濾波器ToolStripMenuItem.Text = "高通濾波器";
            this.高通濾波器ToolStripMenuItem.Click += new System.EventHandler(this.高通濾波器ToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(197, 6);
            // 
            // 自訂濾波器ToolStripMenuItem
            // 
            this.自訂濾波器ToolStripMenuItem.Name = "自訂濾波器ToolStripMenuItem";
            this.自訂濾波器ToolStripMenuItem.Size = new System.Drawing.Size(200, 34);
            this.自訂濾波器ToolStripMenuItem.Text = "自訂濾波器";
            this.自訂濾波器ToolStripMenuItem.Click += new System.EventHandler(this.自訂濾波器ToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2,
            this.toolStripStatusLabel3});
            this.statusStrip1.Location = new System.Drawing.Point(0, 914);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(2, 0, 14, 0);
            this.statusStrip1.Size = new System.Drawing.Size(1578, 30);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            this.toolStripStatusLabel1.Margin = new System.Windows.Forms.Padding(3, 3, 0, 2);
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(192, 25);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(192, 23);
            this.toolStripStatusLabel2.Text = "toolStripStatusLabel2";
            // 
            // toolStripStatusLabel3
            // 
            this.toolStripStatusLabel3.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            this.toolStripStatusLabel3.Size = new System.Drawing.Size(192, 23);
            this.toolStripStatusLabel3.Text = "toolStripStatusLabel3";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // DIPSample
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1578, 944);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "DIPSample";
            this.Text = "DIPSample";
            this.Load += new System.EventHandler(this.DIPSample_Load);
            this.MdiChildActivate += new System.EventHandler(this.DIPSample_MdiChildActivate);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.ToolStripMenuItem 檔案ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 開啟ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 儲存ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 編輯ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 負片轉換ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 均衡化ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 視圖ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 直方圖ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 圖像toolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 旋轉圖像ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 向右旋轉90度ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 向右旋轉180度ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 向右旋轉270度ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 翻轉圖像ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 水平翻轉ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 垂直翻轉ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 濾波器ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 平均濾波器ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 高通濾波器ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem 自訂濾波器ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem 自訂旋轉角度ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 放大圖像ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 縮小圖像ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 灰階轉換toolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 二值化toolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
        private System.Windows.Forms.ToolStripMenuItem 亮度ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 對比度ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 局部馬賽克ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 連通標記ToolStripMenuItem;
    }
}

