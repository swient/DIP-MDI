using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using OxyPlot.WindowsForms;

namespace DIP
{
    public partial class DIPSample : Form
    {
        Bitmap NpBitmap;

        public DIPSample()
        {
            InitializeComponent();
        }

        private void DIPSample_Load(object sender, EventArgs e)
        {
            this.IsMdiContainer = true; // 設定為 MDI 容器
            this.toolStripStatusLabel1.Text = "";
        }

        private Bitmap bmp_read(OpenFileDialog oFileDlg)
        {
            Bitmap pBitmap;
            string fileloc = oFileDlg.FileName;
            pBitmap = new Bitmap(fileloc);
            return pBitmap;
        }

        private void 開啟ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.CheckFileExists = true; // 檢查檔案是否存在
            openFileDialog1.CheckPathExists = true; // 檢查路徑是否存在
            openFileDialog1.Title = "Open File - DIP Sample";
            openFileDialog1.ValidateNames = true; // 驗證檔案名稱是否有效
            openFileDialog1.Filter = "bmp files (*.bmp)|*.bmp";
            openFileDialog1.FileName = "";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                MSForm childForm = new MSForm();
                childForm.MdiParent = this;
                childForm.pf1 = toolStripStatusLabel1;
                NpBitmap = bmp_read(openFileDialog1);
                childForm.pBitmap = NpBitmap;
                childForm.Show();
            }
        }

        private void 儲存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MdiChildren.Length == 0)
            {
                MessageBox.Show("沒有開啟的圖片");
                return;
            }

            foreach (Form cF in MdiChildren)
            {
                if (cF == ActiveMdiChild) // 使用 ActiveMdiChild 確保處理當前聚焦的視窗
                {
                    // 確認視窗類型為 MSForm
                    if (cF is MSForm msForm)
                    {
                        // 開啟儲存對話框
                        SaveFileDialog saveFileDialog = new SaveFileDialog();
                        saveFileDialog.Filter = "PNG Files (*.png)|*.png|JPEG Files (*.jpg)|*.jpg|Bitmap Files (*.bmp)|*.bmp";
                        saveFileDialog.Title = "儲存圖片";

                        // 設定預設檔名
                        saveFileDialog.FileName = "image.png";  // 預設檔名為 "image.png"

                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            // 根據選擇的檔案格式儲存圖片
                            string filePath = saveFileDialog.FileName;

                            // 儲存圖片
                            msForm.pBitmap.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
                        }
                    }
                    else
                    {
                        MessageBox.Show("請先完成編輯");
                        return;
                    }
                    
                    break; // 找到聚焦的視窗後跳出迴圈
                }
            }
        }

        private void 負片轉換ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MdiChildren.Length == 0)
            {
                MessageBox.Show("沒有開啟的圖片");
                return;
            }

            // 遍歷所有 MDI 子視窗，尋找當前聚焦的視窗
            foreach (Form cF in MdiChildren)
            {
                if (cF == ActiveMdiChild) // 使用 ActiveMdiChild 確保處理當前聚焦的視窗
                {
                    // 確認視窗類型為 MSForm
                    if (cF is MSForm msForm)
                    {
                        // 將 Bitmap 轉換為陣列
                        int[] f = ImageProcessUtils.BitmapToArray(msForm.pBitmap);
                        int[] g = new int[msForm.pBitmap.Width * msForm.pBitmap.Height]; // 假設 w 和 h 是圖片的寬高

                        // 使用 unsafe 區域進行指針處理
                        unsafe
                        {
                            fixed (int* f0 = f) fixed (int* g0 = g)
                            {
                                ImageProcessUtils.negative(f0, msForm.pBitmap.Width, msForm.pBitmap.Height, g0); // 進行負片轉換的處理
                            }
                        }
                        // 將處理後的陣列轉換回 Bitmap
                        NpBitmap = ImageProcessUtils.ArrayToBitmap(g, msForm.pBitmap.Width, msForm.pBitmap.Height);
                    }
                    else
                    {
                        MessageBox.Show("請先完成編輯");
                        return;
                    }

                    break; // 找到聚焦的視窗後跳出迴圈
                }
            }

            MSForm childForm = new MSForm();
            childForm.MdiParent = this;
            childForm.pf1 = toolStripStatusLabel1;
            childForm.pBitmap = NpBitmap;
            childForm.Show();
        }

        private void 局部馬賽克ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MdiChildren.Length == 0)
            {
                MessageBox.Show("沒有開啟的圖片");
                return;
            }

            foreach (Form cF in MdiChildren)
            {
                if (cF == ActiveMdiChild) // 使用 ActiveMdiChild 確保處理當前聚焦的視窗
                {
                    // 確認視窗類型為 MSForm
                    if (cF is MSForm msForm)
                    {
                        NpBitmap = new Bitmap(msForm.pBitmap);
                    }
                    else
                    {
                        MessageBox.Show("請先完成編輯");
                        return;
                    }

                    break; // 找到聚焦的視窗後跳出迴圈
                }
            }

            SliderForm childForm = new SliderForm();
            childForm.Select = 4;
            childForm.MdiParent = this;
            childForm.pf1 = toolStripStatusLabel1;
            childForm.NpBitmap = NpBitmap;
            childForm.Show();
        }

        private void 亮度ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MdiChildren.Length == 0)
            {
                MessageBox.Show("沒有開啟的圖片");
                return;
            }

            foreach (Form cF in MdiChildren)
            {
                if (cF == ActiveMdiChild) // 使用 ActiveMdiChild 確保處理當前聚焦的視窗
                {
                    // 確認視窗類型為 MSForm
                    if (cF is MSForm msForm)
                    {
                        NpBitmap = new Bitmap(msForm.pBitmap);
                    }
                    else
                    {
                        MessageBox.Show("請先完成編輯");
                        return;
                    }

                    break; // 找到聚焦的視窗後跳出迴圈
                }
            }

            SliderForm childForm = new SliderForm();
            childForm.Select = 0;
            childForm.MdiParent = this;
            childForm.pf1 = toolStripStatusLabel1;
            childForm.NpBitmap = NpBitmap;
            childForm.Show();
        }

        private void 對比度ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MdiChildren.Length == 0)
            {
                MessageBox.Show("沒有開啟的圖片");
                return;
            }

            foreach (Form cF in MdiChildren)
            {
                if (cF == ActiveMdiChild) // 使用 ActiveMdiChild 確保處理當前聚焦的視窗
                {
                    // 確認視窗類型為 MSForm
                    if (cF is MSForm msForm)
                    {
                        NpBitmap = new Bitmap(msForm.pBitmap);
                    }
                    else
                    {
                        MessageBox.Show("請先完成編輯");
                        return;
                    }

                    break; // 找到聚焦的視窗後跳出迴圈
                }
            }

            SliderForm childForm = new SliderForm();
            childForm.Select = 1;
            childForm.MdiParent = this;
            childForm.pf1 = toolStripStatusLabel1;
            childForm.NpBitmap = NpBitmap;
            childForm.Show();
        }

        private void 均衡化ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MdiChildren.Length == 0)
            {
                MessageBox.Show("沒有開啟的圖片");
                return;
            }

            // 遍歷所有 MDI 子視窗，尋找當前聚焦的視窗
            foreach (Form cF in MdiChildren)
            {
                if (cF == ActiveMdiChild) // 使用 ActiveMdiChild 確保處理當前聚焦的視窗
                {
                    // 確認視窗類型為 MSForm
                    if (cF is MSForm msForm)
                    {
                        // 將 Bitmap 轉換為陣列
                        int[] f = ImageProcessUtils.BitmapToArray(msForm.pBitmap);
                        int[] g = new int[msForm.pBitmap.Width * msForm.pBitmap.Height]; // 假設 w 和 h 是圖片的寬高

                        // 使用 unsafe 區域進行指針處理
                        unsafe
                        {
                            fixed (int* f0 = f) fixed (int* g0 = g)
                            {
                                ImageProcessUtils.equalization(f0, msForm.pBitmap.Width, msForm.pBitmap.Height, g0); // 進行均衡化的處理
                            }
                        }

                        // 將處理後的陣列轉換回 Bitmap
                        NpBitmap = ImageProcessUtils.ArrayToBitmap(g, msForm.pBitmap.Width, msForm.pBitmap.Height);
                    }
                    else
                    {
                        MessageBox.Show("請先完成編輯");
                        return;
                    }

                    break; // 找到聚焦的視窗後跳出迴圈
                }
            }

            MSForm childForm = new MSForm();
            childForm.MdiParent = this;
            childForm.pf1 = toolStripStatusLabel1;
            childForm.pBitmap = NpBitmap;
            childForm.Show();
        }

        private void 直方圖ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MdiChildren.Length == 0)
            {
                MessageBox.Show("沒有開啟的圖片");
                return;
            }

            // 遍歷所有 MDI 子視窗，尋找當前聚焦的視窗
            foreach (Form cF in MdiChildren)
            {
                if (cF == ActiveMdiChild) // 使用 ActiveMdiChild 確保處理當前聚焦的視窗
                {
                    // 確認視窗類型為 MSForm
                    if (cF is MSForm msForm)
                    {
                        // ===== 直方圖計算部分 =====
                        Bitmap sourceBmp = msForm.pBitmap;
                        int[] histogram = new int[256]; // 初始化256階灰度直方圖陣列

                        // 鎖定Bitmap數據以提高存取效率
                        var data = sourceBmp.LockBits(
                            new Rectangle(0, 0, sourceBmp.Width, sourceBmp.Height),
                            ImageLockMode.ReadOnly,
                            PixelFormat.Format32bppArgb);

                        // 不安全代碼區塊直接操作指針
                        unsafe
                        {
                            byte* ptr = (byte*)data.Scan0; // 取得像素數據起始指針
                            for (int y = 0; y < data.Height; y++)
                            {
                                for (int x = 0; x < data.Width; x++)
                                {
                                    // 計算灰度值 (使用標準加權公式)
                                    int gray = (int)(0.299 * ptr[2] + 0.587 * ptr[1] + 0.114 * ptr[0]);
                                    histogram[gray]++; // 累計對應灰度值的計數
                                    ptr += 4; // 移動到下一個像素 (ARGB格式每個像素4字節)
                                }
                                ptr += data.Stride - data.Width * 4; // 換行處理
                            }
                        }
                        sourceBmp.UnlockBits(data); // 解鎖Bitmap

                        // ===== OxyPlot 繪圖部分 =====
                        var plotModel = new PlotModel { }; // 創建空的繪圖模型

                        // 創建直方圖系列 (柱狀圖)
                        var series = new HistogramSeries()
                        {
                            FillColor = OxyColors.SteelBlue, // 柱子填充色
                            StrokeColor = OxyColors.Black,    // 邊框顏色
                            StrokeThickness = 1               // 邊框粗細
                        };

                        // 填充直方圖數據
                        for (int i = 0; i < histogram.Length; i++)
                        {
                            // 每個柱子代表1個灰度級 (i到i+1的範圍)
                            series.Items.Add(new HistogramItem(i, i + 1, histogram[i], 0));
                        }

                        // 設置X軸 (底部軸)
                        plotModel.Axes.Add(new LinearAxis
                        {
                            Position = AxisPosition.Bottom,
                            Minimum = -1,     // 顯示範圍起點
                            Maximum = 257,    // 顯示範圍終點 (比255多2以便完整顯示)
                            MajorStep = 51    // 主刻度間隔 (255/5=51)
                        });

                        // 設置Y軸 (左側軸)
                        plotModel.Axes.Add(new LinearAxis
                        {
                            Position = AxisPosition.Left,
                            Minimum = 0 // Y軸從0開始
                        });

                        plotModel.Series.Add(series); // 將系列加入模型

                        // ===== 視窗設定部分 =====
                        Form plotForm = new Form();
                        plotForm.Text = "直方圖";
                        plotForm.MdiParent = this; // 設置為MDI子視窗

                        // 設定視窗大小
                        plotForm.Size = new Size(650, 350); // 寬650,高350

                        // 設定視窗起始位置在父視窗右下角
                        plotForm.StartPosition = FormStartPosition.Manual;
                        plotForm.Location = new Point(
                           this.ClientRectangle.Right - plotForm.Width - (this.Width - this.ClientRectangle.Width),
                            this.ClientRectangle.Bottom - plotForm.Height - (this.Height - this.ClientRectangle.Height) - 20
                        );

                        // 固定視窗大小
                        plotForm.FormBorderStyle = FormBorderStyle.FixedSingle;
                        plotForm.MaximizeBox = false; // 禁用最大化按鈕

                        // 配置提示功能格式
                        series.TrackerFormatString = "灰階值: {5}\n數量: {7}";

                        // 創建繪圖視圖
                        PlotView plotView = new PlotView();
                        plotView.Dock = DockStyle.Fill; // 填滿整個表單
                        plotView.Model = plotModel;     // 綁定數據模型

                        // ===== 交互控制部分 =====
                        var disabledController = new PlotController();
                        disabledController.UnbindAll();  // 解除所有默認交互綁定

                        // 只保留懸停提示功能
                        disabledController.BindMouseEnter(PlotCommands.HoverPointsOnlyTrack);

                        plotView.Controller = disabledController;
                        plotView.ContextMenu = null; // 禁用右鍵菜單

                        // 將繪圖視圖加入表單
                        plotForm.Controls.Add(plotView);
                        plotForm.Show();
                    }
                    else
                    {
                        MessageBox.Show("請先完成編輯");
                    }

                    break; // 找到聚焦的視窗後跳出迴圈
                }
            }
        }

        private void 平均濾波器ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MdiChildren.Length == 0)
            {
                MessageBox.Show("沒有開啟的圖片");
                return;
            }

            foreach (Form cF in MdiChildren)
            {
                if (cF == ActiveMdiChild) // 使用 ActiveMdiChild 確保處理當前聚焦的視窗
                {
                    // 確認視窗類型為 MSForm
                    if (cF is MSForm msForm)
                    {
                        NpBitmap = new Bitmap(msForm.pBitmap);
                    }
                    else
                    {
                        MessageBox.Show("請先完成編輯");
                        return;
                    }

                    break; // 找到聚焦的視窗後跳出迴圈
                }
            }

            SliderForm childForm = new SliderForm();
            childForm.Select = 2;
            childForm.MdiParent = this;
            childForm.pf1 = toolStripStatusLabel1;
            childForm.NpBitmap = NpBitmap;
            childForm.Show();
        }

        private void 高通濾波器ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MdiChildren.Length == 0)
            {
                MessageBox.Show("沒有開啟的圖片");
                return;
            }

            // 遍歷所有 MDI 子視窗，尋找當前聚焦的視窗
            foreach (Form cF in MdiChildren)
            {
                if (cF == ActiveMdiChild) // 使用 ActiveMdiChild 確保處理當前聚焦的視窗
                {
                    // 確認視窗類型為 MSForm
                    if (cF is MSForm msForm)
                    {
                        // 將 Bitmap 轉換為陣列
                        int[] f = ImageProcessUtils.BitmapToArray(msForm.pBitmap);
                        int[] g = new int[msForm.pBitmap.Width * msForm.pBitmap.Height]; // 假設 w 和 h 是圖片的寬高

                        // 使用 unsafe 區域進行指針處理
                        unsafe
                        {
                            fixed (int* f0 = f) fixed (int* g0 = g)
                            {
                                ImageProcessUtils.highpassfilter(f0, msForm.pBitmap.Width, msForm.pBitmap.Height, g0); // 進行高通濾波器的處理
                            }
                        }

                        // 將處理後的陣列轉換回 Bitmap
                        NpBitmap = ImageProcessUtils.ArrayToBitmap(g, msForm.pBitmap.Width, msForm.pBitmap.Height);
                    }
                    else
                    {
                        MessageBox.Show("請先完成編輯");
                        return;
                    }

                    break; // 找到聚焦的視窗後跳出迴圈
                }
            }

            MSForm childForm = new MSForm();
            childForm.MdiParent = this;
            childForm.pf1 = toolStripStatusLabel1;
            childForm.pBitmap = NpBitmap;
            childForm.Show();
        }

        private void 自訂濾波器ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MdiChildren.Length == 0)
            {
                MessageBox.Show("沒有開啟的圖片");
                return;
            }

            foreach (Form cF in MdiChildren)
            {
                if (cF == ActiveMdiChild) // 使用 ActiveMdiChild 確保處理當前聚焦的視窗
                {
                    // 確認視窗類型為 MSForm
                    if (cF is MSForm msForm)
                    {
                        NpBitmap = new Bitmap(msForm.pBitmap);
                    }
                    else
                    {
                        MessageBox.Show("請先完成編輯");
                        return;
                    }

                    break; // 找到聚焦的視窗後跳出迴圈
                }
            }

            FilterForm childForm = new FilterForm();
            childForm.MdiParent = this;
            childForm.pf1 = toolStripStatusLabel1;
            childForm.NpBitmap = NpBitmap;
            childForm.Show();
        }

        private void 向右旋轉90度ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MdiChildren.Length == 0)
            {
                MessageBox.Show("沒有開啟的圖片");
                return;
            }

            foreach (Form cF in MdiChildren)
            {
                if (cF == ActiveMdiChild) // 使用 ActiveMdiChild 確保處理當前聚焦的視窗
                {
                    // 確認視窗類型為 MSForm
                    if (cF is MSForm msForm)
                    {
                        double theta = 90 * Math.PI / 180;
                        int new_weight = (int)Math.Round(msForm.pBitmap.Height * Math.Abs(Math.Sin(theta)) + msForm.pBitmap.Width * Math.Abs(Math.Cos(theta)));
                        int new_height = (int)Math.Round(msForm.pBitmap.Height * Math.Abs(Math.Cos(theta)) + msForm.pBitmap.Width * Math.Abs(Math.Sin(theta)));

                        // 將 Bitmap 轉換為陣列
                        int[] f = ImageProcessUtils.BitmapToArray(msForm.pBitmap);
                        int[] g = new int[new_weight * new_height]; // 假設 w 和 h 是圖片的寬高

                        // 使用 unsafe 區域進行指針處理
                        unsafe
                        {
                            fixed (int* f0 = f) fixed (int* g0 = g)
                            {
                                ImageProcessUtils.customrotationangle(f0, msForm.pBitmap.Width, msForm.pBitmap.Height, 90, g0);
                            }
                        }

                        // 將處理後的陣列轉換回 Bitmap
                        NpBitmap = ImageProcessUtils.ArrayToBitmap(g, new_weight, new_height);
                    }
                    else
                    {
                        MessageBox.Show("請先完成編輯");
                        return;
                    }

                    break; // 找到聚焦的視窗後跳出迴圈
                }
            }

            MSForm childForm = new MSForm();
            childForm.MdiParent = this;
            childForm.pf1 = toolStripStatusLabel1;
            childForm.pBitmap = NpBitmap;
            childForm.Show();
        }

        private void 向右旋轉180度ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MdiChildren.Length == 0)
            {
                MessageBox.Show("沒有開啟的圖片");
                return;
            }

            foreach (Form cF in MdiChildren)
            {
                if (cF == ActiveMdiChild) // 使用 ActiveMdiChild 確保處理當前聚焦的視窗
                {
                    // 確認視窗類型為 MSForm
                    if (cF is MSForm msForm)
                    {
                        double theta = 180 * Math.PI / 180;
                        int new_weight = (int)Math.Round(msForm.pBitmap.Height * Math.Abs(Math.Sin(theta)) + msForm.pBitmap.Width * Math.Abs(Math.Cos(theta)));
                        int new_height = (int)Math.Round(msForm.pBitmap.Height * Math.Abs(Math.Cos(theta)) + msForm.pBitmap.Width * Math.Abs(Math.Sin(theta)));

                        // 將 Bitmap 轉換為陣列
                        int[] f = ImageProcessUtils.BitmapToArray(msForm.pBitmap);
                        int[] g = new int[new_weight * new_height]; // 假設 w 和 h 是圖片的寬高

                        // 使用 unsafe 區域進行指針處理
                        unsafe
                        {
                            fixed (int* f0 = f) fixed (int* g0 = g)
                            {
                                ImageProcessUtils.customrotationangle(f0, msForm.pBitmap.Width, msForm.pBitmap.Height, 180, g0);
                            }
                        }

                        // 將處理後的陣列轉換回 Bitmap
                        NpBitmap = ImageProcessUtils.ArrayToBitmap(g, new_weight, new_height);
                    }
                    else
                    {
                        MessageBox.Show("請先完成編輯");
                        return;
                    }

                    break; // 找到聚焦的視窗後跳出迴圈
                }
            }

            MSForm childForm = new MSForm();
            childForm.MdiParent = this;
            childForm.pf1 = toolStripStatusLabel1;
            childForm.pBitmap = NpBitmap;
            childForm.Show();
        }

        private void 向右旋轉270度ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MdiChildren.Length == 0)
            {
                MessageBox.Show("沒有開啟的圖片");
                return;
            }

            foreach (Form cF in MdiChildren)
            {
                if (cF == ActiveMdiChild) // 使用 ActiveMdiChild 確保處理當前聚焦的視窗
                {
                    // 確認視窗類型為 MSForm
                    if (cF is MSForm msForm)
                    {
                        double theta = 270 * Math.PI / 180;
                        int new_weight = (int)Math.Round(msForm.pBitmap.Height * Math.Abs(Math.Sin(theta)) + msForm.pBitmap.Width * Math.Abs(Math.Cos(theta)));
                        int new_height = (int)Math.Round(msForm.pBitmap.Height * Math.Abs(Math.Cos(theta)) + msForm.pBitmap.Width * Math.Abs(Math.Sin(theta)));

                        // 將 Bitmap 轉換為陣列
                        int[] f = ImageProcessUtils.BitmapToArray(msForm.pBitmap);
                        int[] g = new int[new_weight * new_height]; // 假設 w 和 h 是圖片的寬高

                        // 使用 unsafe 區域進行指針處理
                        unsafe
                        {
                            fixed (int* f0 = f) fixed (int* g0 = g)
                            {
                                ImageProcessUtils.customrotationangle(f0, msForm.pBitmap.Width, msForm.pBitmap.Height, 270, g0);
                            }
                        }

                        // 將處理後的陣列轉換回 Bitmap
                        NpBitmap = ImageProcessUtils.ArrayToBitmap(g, new_weight, new_height);
                    }
                    else
                    {
                        MessageBox.Show("請先完成編輯");
                        return;
                    }

                    break; // 找到聚焦的視窗後跳出迴圈
                }
            }

            MSForm childForm = new MSForm();
            childForm.MdiParent = this;
            childForm.pf1 = toolStripStatusLabel1;
            childForm.pBitmap = NpBitmap;
            childForm.Show();
        }

        private void 自訂旋轉角度ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MdiChildren.Length == 0)
            {
                MessageBox.Show("沒有開啟的圖片");
                return;
            }

            foreach (Form cF in MdiChildren)
            {
                if (cF == ActiveMdiChild) // 使用 ActiveMdiChild 確保處理當前聚焦的視窗
                {
                    // 確認視窗類型為 MSForm
                    if (cF is MSForm msForm)
                    {
                        NpBitmap = new Bitmap(msForm.pBitmap);
                    }
                    else
                    {
                        MessageBox.Show("請先完成編輯");
                        return;
                    }

                    break; // 找到聚焦的視窗後跳出迴圈
                }
            }

            SliderForm childForm = new SliderForm();
            childForm.Select = 3;
            childForm.MdiParent = this;
            childForm.pf1 = toolStripStatusLabel1;
            childForm.NpBitmap = NpBitmap;
            childForm.Show();
        }

        private void 水平翻轉ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MdiChildren.Length == 0)
            {
                MessageBox.Show("沒有開啟的圖片");
                return;
            }

            foreach (Form cF in MdiChildren)
            {
                if (cF == ActiveMdiChild) // 使用 ActiveMdiChild 確保處理當前聚焦的視窗
                {
                    // 確認視窗類型為 MSForm
                    if (cF is MSForm msForm)
                    {
                        Bitmap flippedBitmap = new Bitmap(msForm.pBitmap.Width, msForm.pBitmap.Height);
                        // 遍歷原圖的每個像素
                        for (int x = 0; x < msForm.pBitmap.Width; x++)
                        {
                            for (int y = 0; y < msForm.pBitmap.Height; y++)
                            {
                                // 計算左右翻轉後的位置
                                int newX = msForm.pBitmap.Width - 1 - x;
                                int newY = y;

                                // 設定像素
                                flippedBitmap.SetPixel(newX, newY, msForm.pBitmap.GetPixel(x, y));
                            }
                        }

                        NpBitmap = flippedBitmap;
                    }
                    else
                    {
                        MessageBox.Show("請先完成編輯");
                        return;
                    }

                    break; // 找到聚焦的視窗後跳出迴圈
                }
            }

            MSForm childForm = new MSForm();
            childForm.MdiParent = this;
            childForm.pf1 = toolStripStatusLabel1;
            childForm.pBitmap = NpBitmap;
            childForm.Show();
        }

        private void 垂直翻轉ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MdiChildren.Length == 0)
            {
                MessageBox.Show("沒有開啟的圖片");
                return;
            }

            foreach (Form cF in MdiChildren)
            {
                if (cF == ActiveMdiChild) // 使用 ActiveMdiChild 確保處理當前聚焦的視窗
                {
                    // 確認視窗類型為 MSForm
                    if (cF is MSForm msForm)
                    {
                        Bitmap flippedBitmap = new Bitmap(msForm.pBitmap.Width, msForm.pBitmap.Height);
                        // 遍歷原圖的每個像素
                        for (int x = 0; x < msForm.pBitmap.Width; x++)
                        {
                            for (int y = 0; y < msForm.pBitmap.Height; y++)
                            {
                                // 計算上下翻轉後的位置
                                int newX = x;
                                int newY = msForm.pBitmap.Height - 1 - y;

                                // 設定像素
                                flippedBitmap.SetPixel(newX, newY, msForm.pBitmap.GetPixel(x, y));
                            }
                        }

                        NpBitmap = flippedBitmap;
                    }
                    else
                    {
                        MessageBox.Show("請先完成編輯");
                        return;
                    }

                    break; // 找到聚焦的視窗後跳出迴圈
                }
            }

            MSForm childForm = new MSForm();
            childForm.MdiParent = this;
            childForm.pf1 = toolStripStatusLabel1;
            childForm.pBitmap = NpBitmap;
            childForm.Show();
        }

        private void 放大圖像ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MdiChildren.Length == 0)
            {
                MessageBox.Show("沒有開啟的圖片");
                return;
            }

            // 遍歷所有 MDI 子視窗，尋找當前聚焦的視窗
            foreach (Form cF in MdiChildren)
            {
                if (cF == ActiveMdiChild) // 使用 ActiveMdiChild 確保處理當前聚焦的視窗
                {
                    // 確認視窗類型為 MSForm
                    if (cF is MSForm msForm)
                    {
                        // 顯示輸入框，預設值為 2
                        string result = Interaction.InputBox("請輸入放大倍率", "放大圖像", "2");

                        // 使用者按下取消時，result 會是空字串
                        if (string.IsNullOrWhiteSpace(result))
                        {
                            return;
                        }

                        // 嘗試將輸入轉換為數字
                        if (double.TryParse(result, out double scale) && scale > 0)
                        {
                            int new_weight = (int)Math.Round(msForm.pBitmap.Width * scale);
                            int new_height = (int)Math.Round(msForm.pBitmap.Height * scale);

                            // 將 Bitmap 轉換為陣列
                            int[] f = ImageProcessUtils.BitmapToArray(msForm.pBitmap);
                            int[] g = new int[new_weight * new_height];

                            // 使用 unsafe 區域進行指針處理
                            unsafe
                            {
                                fixed (int* f0 = f) fixed (int* g0 = g)
                                {
                                    ImageProcessUtils.zoom(f0, msForm.pBitmap.Width, msForm.pBitmap.Height, scale, g0); // 進行放大的處理
                                }
                            }

                            // 將處理後的陣列轉換回 Bitmap
                            NpBitmap = ImageProcessUtils.ArrayToBitmap(g, new_weight, new_height);
                        }
                        else
                        {
                            MessageBox.Show("請輸入有效的數字！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("請先完成編輯");
                        return;
                    }

                    break; // 找到聚焦的視窗後跳出迴圈
                }
            }

            MSForm childForm = new MSForm();
            childForm.MdiParent = this;
            childForm.pf1 = toolStripStatusLabel1;
            childForm.pBitmap = NpBitmap;
            childForm.Show();
        }

        private void 縮小圖像ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MdiChildren.Length == 0)
            {
                MessageBox.Show("沒有開啟的圖片");
                return;
            }

            // 遍歷所有 MDI 子視窗，尋找當前聚焦的視窗
            foreach (Form cF in MdiChildren)
            {
                if (cF == ActiveMdiChild) // 使用 ActiveMdiChild 確保處理當前聚焦的視窗
                {
                    // 確認視窗類型為 MSForm
                    if (cF is MSForm msForm)
                    {
                        // 顯示輸入框，預設值為 2
                        string result = Interaction.InputBox("請輸入縮小倍率", "縮小圖像", "2");

                        // 使用者按下取消時，result 會是空字串
                        if (string.IsNullOrWhiteSpace(result))
                        {
                            return;
                        }

                        // 嘗試將輸入轉換為數字
                        if (double.TryParse(result, out double scale) && scale > 0)
                        {
                            scale = 1 / scale;
                            int new_weight = (int)Math.Round(msForm.pBitmap.Width * scale);
                            int new_height = (int)Math.Round(msForm.pBitmap.Height * scale);
                            
                            // 將 Bitmap 轉換為陣列
                            int[] f = ImageProcessUtils.BitmapToArray(msForm.pBitmap);
                            int[] g = new int[new_weight * new_height];

                            // 使用 unsafe 區域進行指針處理
                            unsafe
                            {
                                fixed (int* f0 = f) fixed (int* g0 = g)
                                {
                                    ImageProcessUtils.zoom(f0, msForm.pBitmap.Width, msForm.pBitmap.Height, scale, g0); // 進行縮小的處理
                                }
                            }

                            // 將處理後的陣列轉換回 Bitmap
                            NpBitmap = ImageProcessUtils.ArrayToBitmap(g, new_weight, new_height);
                        }
                        else
                        {
                            MessageBox.Show("請輸入有效的數字！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("請先完成編輯");
                        return;
                    }

                    break; // 找到聚焦的視窗後跳出迴圈
                }
            }

            MSForm childForm = new MSForm();
            childForm.MdiParent = this;
            childForm.pf1 = toolStripStatusLabel1;
            childForm.pBitmap = NpBitmap;
            childForm.Show();
        }
    }
}
