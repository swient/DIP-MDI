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
using System.IO;

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
            UpdateMenuState(); // 初始化選單狀態
        }

        private void DIPSample_MdiChildActivate(object sender, EventArgs e)
        {
            // 延後執行，確保 MSForm 在狀態完成後更新選單狀態
            this.BeginInvoke(new Action(() => UpdateMenuState()));
        }


        private void UpdateMenuState()
        {
            // 檢查是否有子表單存在
            bool msFormExists = this.MdiChildren.OfType<MSForm>().Any();
            bool sliderFormExists = this.MdiChildren.OfType<SliderForm>().Any();
            bool filterFormExists = this.MdiChildren.OfType<FilterForm>().Any();
            bool baseEnabled = msFormExists && !sliderFormExists && !filterFormExists; // 基本啟用狀態
            // 檢查是否有直方圖表單存在
            bool histogramFormExists = this.MdiChildren.Any(form => form.Text == "直方圖");

            // 根據是否存在子表單來啟用/禁用相關選單項目
            儲存ToolStripMenuItem.Enabled = baseEnabled;
            編輯ToolStripMenuItem.Enabled = baseEnabled;
            視圖ToolStripMenuItem.Enabled = baseEnabled;
            圖像toolStripMenuItem.Enabled = baseEnabled;
            濾波器ToolStripMenuItem.Enabled = baseEnabled;
            // 如果直方圖表單存在，則禁用直方圖選單項目
            直方圖ToolStripMenuItem.Enabled = !histogramFormExists;
        }

        private void 開啟ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.CheckFileExists = true; // 檢查檔案是否存在
                openFileDialog.CheckPathExists = true; // 檢查路徑是否存在
                openFileDialog.ValidateNames = true; // 驗證檔案名稱是否有效
                openFileDialog.Title = "Open File - DIP Sample";
                openFileDialog.Filter = "Image Files|*.bmp;*.png;*.jpg;*.jpeg";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        NpBitmap = new Bitmap(openFileDialog.FileName);
                        string fileName = Path.GetFileName(openFileDialog.FileName);

                        // 查找現有表單
                        MSForm childForm = this.MdiChildren.OfType<MSForm>().FirstOrDefault();

                        if (childForm == null)
                        {
                            childForm = new MSForm();
                            childForm.MdiParent = this;
                            childForm.pf1 = toolStripStatusLabel1;
                            childForm.pBitmap = new Bitmap(NpBitmap);
                            childForm.Show();
                        }

                        childForm.AddImageTab(fileName, new Bitmap(NpBitmap));
                        childForm.UpdateHistogram(NpBitmap);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"開啟檔案失敗: {ex.Message}", "錯誤",
                                      MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void 儲存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild == null)
            {
                MessageBox.Show("沒有開啟的圖片", "提示",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (ActiveMdiChild is MSForm msForm)
            {
                try
                {
                    using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                    {
                        saveFileDialog.Filter = "Bitmap Files (*.bmp)|*.bmp|" +
                                               "PNG Files (*.png)|*.png|" +
                                               "JPEG Files (*.jpg)|*.jpg";
                        saveFileDialog.Title = "Save File - DIP Sample";
                        saveFileDialog.FileName = "image.bmp";
                        saveFileDialog.AddExtension = true; // 自動添加副檔名

                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            string filePath = saveFileDialog.FileName;
                            System.Drawing.Imaging.ImageFormat format;

                            switch (Path.GetExtension(filePath).ToLower())
                            {
                                case ".bmp":
                                    format = System.Drawing.Imaging.ImageFormat.Bmp;
                                    break;
                                case ".png":
                                    format = System.Drawing.Imaging.ImageFormat.Png;
                                    break;
                                case ".jpg":
                                case ".jpeg":
                                    format = System.Drawing.Imaging.ImageFormat.Jpeg;
                                    break;
                                default:
                                    format = System.Drawing.Imaging.ImageFormat.Bmp;
                                    break;
                            }

                            msForm.GetCurrentTabImage().Save(filePath, format);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"儲存圖片失敗: {ex.Message}", "錯誤",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("請先完成編輯", "提示",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void 灰階轉換toolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MdiChildren.Length == 0)
            {
                MessageBox.Show("沒有開啟的圖片");
                return;
            }

            if (ActiveMdiChild is MSForm msForm)
            {
                NpBitmap = msForm.GetCurrentTabImage();

                // 將 Bitmap 轉換為陣列
                int[] f = ImageProcessUtils.BitmapToArray(NpBitmap);

                // 將處理後的陣列轉換回 Bitmap
                NpBitmap = ImageProcessUtils.ArrayToBitmap(f, NpBitmap.Width, NpBitmap.Height);
                msForm.SetCurrentTabImage(NpBitmap);
                msForm.UpdateHistogram(NpBitmap);
            }
            else
            {
                MessageBox.Show("請先完成編輯");
                return;
            }
        }

        private void 負片轉換ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MdiChildren.Length == 0)
            {
                MessageBox.Show("沒有開啟的圖片");
                return;
            }

            if (ActiveMdiChild is MSForm msForm)
            {
                NpBitmap = msForm.GetCurrentTabImage();

                // 將 Bitmap 轉換為陣列
                int[] R, G, B;
                ImageProcessUtils.BitmapToRGBArrays(NpBitmap, out R, out G, out B);

                int[] processedR = new int[NpBitmap.Width * NpBitmap.Height];
                int[] processedG = new int[NpBitmap.Width * NpBitmap.Height];
                int[] processedB = new int[NpBitmap.Width * NpBitmap.Height];

                // 使用 unsafe 區域進行指針處理
                unsafe
                {
                    fixed (int* srcR = R, dstR = processedR)
                    fixed (int* srcG = G, dstG = processedG)
                    fixed (int* srcB = B, dstB = processedB)
                    {
                        // 進行負片轉換的處理
                        ImageProcessUtils.negative(srcR, NpBitmap.Width, NpBitmap.Height, dstR);
                        ImageProcessUtils.negative(srcG, NpBitmap.Width, NpBitmap.Height, dstG);
                        ImageProcessUtils.negative(srcB, NpBitmap.Width, NpBitmap.Height, dstB);
                    }
                }

                // 將處理後的陣列轉換回 Bitmap
                NpBitmap = ImageProcessUtils.RGBArraysToBitmap(processedR, processedG, processedB, NpBitmap.Width, NpBitmap.Height);
                msForm.SetCurrentTabImage(NpBitmap);
                msForm.UpdateHistogram(NpBitmap);
            }
            else
            {
                MessageBox.Show("請先完成編輯");
                return;
            }
        }

        private void 局部馬賽克ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MdiChildren.Length == 0)
            {
                MessageBox.Show("沒有開啟的圖片");
                return;
            }

            if (ActiveMdiChild is MSForm msForm)
            {
                NpBitmap = msForm.GetCurrentTabImage();

                SliderForm childForm = new SliderForm();
                childForm.Select = "LocalMosaic";
                childForm.MdiParent = this;
                childForm.pf1 = toolStripStatusLabel1;
                childForm.NpBitmap = NpBitmap;
                childForm.OriginalTabTitle = msForm.GetCurrentTabTitle();
                childForm.Show();
            }
            else
            {
                MessageBox.Show("請先完成編輯");
                return;
            }
        }

        private void 亮度ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MdiChildren.Length == 0)
            {
                MessageBox.Show("沒有開啟的圖片");
                return;
            }

            if (ActiveMdiChild is MSForm msForm)
            {
                NpBitmap = msForm.GetCurrentTabImage();

                SliderForm childForm = new SliderForm();
                childForm.Select = "Brightness";
                childForm.MdiParent = this;
                childForm.pf1 = toolStripStatusLabel1;
                childForm.NpBitmap = NpBitmap;
                childForm.OriginalTabTitle = msForm.GetCurrentTabTitle();
                childForm.Show();
            }
            else
            {
                MessageBox.Show("請先完成編輯");
                return;
            }
        }

        private void 對比度ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MdiChildren.Length == 0)
            {
                MessageBox.Show("沒有開啟的圖片");
                return;
            }

            if (ActiveMdiChild is MSForm msForm)
            {
                NpBitmap = msForm.GetCurrentTabImage();

                SliderForm childForm = new SliderForm();
                childForm.Select = "Contrast";
                childForm.MdiParent = this;
                childForm.pf1 = toolStripStatusLabel1;
                childForm.NpBitmap = NpBitmap;
                childForm.OriginalTabTitle = msForm.GetCurrentTabTitle();
                childForm.Show();
            }
            else
            {
                MessageBox.Show("請先完成編輯");
                return;
            }
        }

        private void 均衡化ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MdiChildren.Length == 0)
            {
                MessageBox.Show("沒有開啟的圖片");
                return;
            }

            if (ActiveMdiChild is MSForm msForm)
            {
                NpBitmap = msForm.GetCurrentTabImage();

                // 將 Bitmap 轉換為陣列
                int[] R, G, B;
                ImageProcessUtils.BitmapToRGBArrays(NpBitmap, out R, out G, out B);

                int[] processedR = new int[NpBitmap.Width * NpBitmap.Height];
                int[] processedG = new int[NpBitmap.Width * NpBitmap.Height];
                int[] processedB = new int[NpBitmap.Width * NpBitmap.Height];

                // 使用 unsafe 區域進行指針處理
                unsafe
                {
                    fixed (int* srcR = R, dstR = processedR)
                    fixed (int* srcG = G, dstG = processedG)
                    fixed (int* srcB = B, dstB = processedB)
                    {
                        // 進行均衡化的處理
                        ImageProcessUtils.equalization(srcR, NpBitmap.Width, NpBitmap.Height, dstR);
                        ImageProcessUtils.equalization(srcG, NpBitmap.Width, NpBitmap.Height, dstG);
                        ImageProcessUtils.equalization(srcB, NpBitmap.Width, NpBitmap.Height, dstB);
                    }
                }

                // 將處理後的陣列轉換回 Bitmap
                NpBitmap = ImageProcessUtils.RGBArraysToBitmap(processedR, processedG, processedB, NpBitmap.Width, NpBitmap.Height);
                msForm.SetCurrentTabImage(NpBitmap);
                msForm.UpdateHistogram(NpBitmap);
            }
            else
            {
                MessageBox.Show("請先完成編輯");
                return;
            }
        }

        private void 二值化toolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MdiChildren.Length == 0)
            {
                MessageBox.Show("沒有開啟的圖片");
                return;
            }

            if (ActiveMdiChild is MSForm msForm)
            {
                NpBitmap = msForm.GetCurrentTabImage();

                // 將 Bitmap 轉換為陣列
                int[] f = ImageProcessUtils.BitmapToArray(NpBitmap);
                int[] g = new int[NpBitmap.Width * NpBitmap.Height];

                // 使用 unsafe 區域進行指針處理
                unsafe
                {
                    fixed (int* f0 = f) fixed (int* g0 = g)
                    {
                        ImageProcessUtils.otsuthreshold(f0, NpBitmap.Width, NpBitmap.Height, g0); // 進行二值化的處理
                    }
                }

                // 將處理後的陣列轉換回 Bitmap
                NpBitmap = ImageProcessUtils.ArrayToBitmap(g, NpBitmap.Width, NpBitmap.Height);
                msForm.SetCurrentTabImage(NpBitmap);
                msForm.UpdateHistogram(NpBitmap);
            }
            else
            {
                MessageBox.Show("請先完成編輯");
                return;
            }
        }

        private void 直方圖ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MdiChildren.Length == 0)
            {
                MessageBox.Show("沒有開啟的圖片");
                return;
            }

            if (ActiveMdiChild is MSForm msForm)
            {
                NpBitmap = msForm.GetCurrentTabImage();

                try
                {
                    int[] histogram = ImageProcessUtils.CalculateHistogram(NpBitmap);
                    PlotModel plotModel = ImageProcessUtils.CreateHistogramPlotModel(histogram);

                    // 創建繪圖視圖
                    var plotView = new PlotView
                    {
                        Model = plotModel, // 綁定數據模型
                        Dock = DockStyle.Fill // 填滿整個表單
                    };

                    // 交互控制部分
                    var disabledController = new PlotController();
                    disabledController.UnbindAll();
                    disabledController.BindMouseEnter(PlotCommands.HoverPointsOnlyTrack); // 保留滑鼠懸停提示功能

                    plotView.Controller = disabledController;
                    plotView.ContextMenu = null; // 禁用右鍵菜單

                    // 視窗設定部分
                    Form plotForm = new Form
                    {
                        Text = "直方圖",
                        Size = new Size(650, 350),
                        StartPosition = FormStartPosition.Manual,
                        Location = new Point(
                            this.ClientRectangle.Right - 650 - (this.Width - this.ClientRectangle.Width),
                            this.ClientRectangle.Bottom - 350 - (this.Height - this.ClientRectangle.Height) - 20
                        ),
                        FormBorderStyle = FormBorderStyle.FixedSingle, // 固定視窗大小
                        MaximizeBox = false, // 禁用最大化按鈕
                        MdiParent = this // 設置為MDI子視窗
                    };

                    // 將繪圖視圖加入表單
                    plotForm.Controls.Add(plotView);
                    plotForm.Show();

                    // 將表單引用傳遞給子表單，並將焦點交還 MSForm
                    msForm.plotView = plotView;
                    msForm.plotForm = plotForm;
                    msForm.Focus();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"生成直方圖時發生錯誤: {ex.Message}", "錯誤", 
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("請先完成編輯");
            }
        }

        private void 平均濾波器ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MdiChildren.Length == 0)
            {
                MessageBox.Show("沒有開啟的圖片");
                return;
            }

            if (ActiveMdiChild is MSForm msForm)
            {
                NpBitmap = msForm.GetCurrentTabImage();

                SliderForm childForm = new SliderForm();
                childForm.Select = "AverageFilter";
                childForm.MdiParent = this;
                childForm.pf1 = toolStripStatusLabel1;
                childForm.NpBitmap = NpBitmap;
                childForm.OriginalTabTitle = msForm.GetCurrentTabTitle();
                childForm.Show();
            }
            else
            {
                MessageBox.Show("請先完成編輯");
                return;
            }
        }

        private void 高通濾波器ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MdiChildren.Length == 0)
            {
                MessageBox.Show("沒有開啟的圖片");
                return;
            }

            if (ActiveMdiChild is MSForm msForm)
            {
                NpBitmap = msForm.GetCurrentTabImage();

                // 將 Bitmap 轉換為陣列
                int[] f = ImageProcessUtils.BitmapToArray(NpBitmap);
                int[] g = new int[NpBitmap.Width * NpBitmap.Height];

                // 使用 unsafe 區域進行指針處理
                unsafe
                {
                    fixed (int* f0 = f) fixed (int* g0 = g)
                    {
                        ImageProcessUtils.highpassfilter(f0, NpBitmap.Width, NpBitmap.Height, g0); // 進行高通濾波器的處理
                    }
                }

                // 將處理後的陣列轉換回 Bitmap
                NpBitmap = ImageProcessUtils.ArrayToBitmap(g, NpBitmap.Width, NpBitmap.Height);
                msForm.SetCurrentTabImage(NpBitmap);
                msForm.UpdateHistogram(NpBitmap);
            }
            else
            {
                MessageBox.Show("請先完成編輯");
                return;
            }
        }

        private void 自訂濾波器ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MdiChildren.Length == 0)
            {
                MessageBox.Show("沒有開啟的圖片");
                return;
            }

            if (ActiveMdiChild is MSForm msForm)
            {
                NpBitmap = msForm.GetCurrentTabImage();

                FilterForm childForm = new FilterForm();
                childForm.MdiParent = this;
                childForm.pf1 = toolStripStatusLabel1;
                childForm.NpBitmap = NpBitmap;
                childForm.OriginalTabTitle = msForm.GetCurrentTabTitle();
                childForm.Show();
            }
            else
            {
                MessageBox.Show("請先完成編輯");
                return;
            }
        }

        private void 向右旋轉90度ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MdiChildren.Length == 0)
            {
                MessageBox.Show("沒有開啟的圖片");
                return;
            }

            if (ActiveMdiChild is MSForm msForm)
            {
                NpBitmap = msForm.GetCurrentTabImage();

                int new_weight = NpBitmap.Height;
                int new_height = NpBitmap.Width;

                // 將 Bitmap 轉換為陣列
                int[] R, G, B;
                ImageProcessUtils.BitmapToRGBArrays(NpBitmap, out R, out G, out B);

                int[] processedR = new int[new_weight * new_height];
                int[] processedG = new int[new_weight * new_height];
                int[] processedB = new int[new_weight * new_height];

                // 使用 unsafe 區域進行指針處理
                unsafe
                {
                    fixed (int* srcR = R, dstR = processedR)
                    fixed (int* srcG = G, dstG = processedG)
                    fixed (int* srcB = B, dstB = processedB)
                    {
                        // 進行旋轉90度的處理
                        ImageProcessUtils.customrotationangle(srcR, NpBitmap.Width, NpBitmap.Height, 90, dstR);
                        ImageProcessUtils.customrotationangle(srcG, NpBitmap.Width, NpBitmap.Height, 90, dstG);
                        ImageProcessUtils.customrotationangle(srcB, NpBitmap.Width, NpBitmap.Height, 90, dstB);
                    }
                }

                // 將處理後的陣列轉換回 Bitmap
                NpBitmap = ImageProcessUtils.RGBArraysToBitmap(processedR, processedG, processedB, new_weight, new_height);
                msForm.SetCurrentTabImage(NpBitmap);
                msForm.UpdateHistogram(NpBitmap);
            }
            else
            {
                MessageBox.Show("請先完成編輯");
                return;
            }
        }

        private void 向右旋轉180度ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MdiChildren.Length == 0)
            {
                MessageBox.Show("沒有開啟的圖片");
                return;
            }

            if (ActiveMdiChild is MSForm msForm)
            {
                NpBitmap = msForm.GetCurrentTabImage();

                int new_weight = NpBitmap.Width;
                int new_height = NpBitmap.Height;

                // 將 Bitmap 轉換為陣列
                int[] R, G, B;
                ImageProcessUtils.BitmapToRGBArrays(NpBitmap, out R, out G, out B);

                int[] processedR = new int[new_weight * new_height];
                int[] processedG = new int[new_weight * new_height];
                int[] processedB = new int[new_weight * new_height];

                // 使用 unsafe 區域進行指針處理
                unsafe
                {
                    fixed (int* srcR = R, dstR = processedR)
                    fixed (int* srcG = G, dstG = processedG)
                    fixed (int* srcB = B, dstB = processedB)
                    {
                        // 進行旋轉180度的處理
                        ImageProcessUtils.customrotationangle(srcR, NpBitmap.Width, NpBitmap.Height, 180, dstR);
                        ImageProcessUtils.customrotationangle(srcG, NpBitmap.Width, NpBitmap.Height, 180, dstG);
                        ImageProcessUtils.customrotationangle(srcB, NpBitmap.Width, NpBitmap.Height, 180, dstB);
                    }
                }

                // 將處理後的陣列轉換回 Bitmap
                NpBitmap = ImageProcessUtils.RGBArraysToBitmap(processedR, processedG, processedB, new_weight, new_height);
                msForm.SetCurrentTabImage(NpBitmap);
                msForm.UpdateHistogram(NpBitmap);
            }
            else
            {
                MessageBox.Show("請先完成編輯");
                return;
            }
        }

        private void 向右旋轉270度ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MdiChildren.Length == 0)
            {
                MessageBox.Show("沒有開啟的圖片");
                return;
            }

            if (ActiveMdiChild is MSForm msForm)
            {
                NpBitmap = msForm.GetCurrentTabImage();

                int new_weight = NpBitmap.Height;
                int new_height = NpBitmap.Width;

                // 將 Bitmap 轉換為陣列
                int[] R, G, B;
                ImageProcessUtils.BitmapToRGBArrays(NpBitmap, out R, out G, out B);

                int[] processedR = new int[new_weight * new_height];
                int[] processedG = new int[new_weight * new_height];
                int[] processedB = new int[new_weight * new_height];

                // 使用 unsafe 區域進行指針處理
                unsafe
                {
                    fixed (int* srcR = R, dstR = processedR)
                    fixed (int* srcG = G, dstG = processedG)
                    fixed (int* srcB = B, dstB = processedB)
                    {
                        // 進行旋轉270度的處理
                        ImageProcessUtils.customrotationangle(srcR, NpBitmap.Width, NpBitmap.Height, 270, dstR);
                        ImageProcessUtils.customrotationangle(srcG, NpBitmap.Width, NpBitmap.Height, 270, dstG);
                        ImageProcessUtils.customrotationangle(srcB, NpBitmap.Width, NpBitmap.Height, 270, dstB);
                    }
                }

                // 將處理後的陣列轉換回 Bitmap
                NpBitmap = ImageProcessUtils.RGBArraysToBitmap(processedR, processedG, processedB, new_weight, new_height);
                msForm.SetCurrentTabImage(NpBitmap);
                msForm.UpdateHistogram(NpBitmap);
            }
            else
            {
                MessageBox.Show("請先完成編輯");
                return;
            }
        }

        private void 自訂旋轉角度ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MdiChildren.Length == 0)
            {
                MessageBox.Show("沒有開啟的圖片");
                return;
            }

            if (ActiveMdiChild is MSForm msForm)
            {
                NpBitmap = msForm.GetCurrentTabImage();

                SliderForm childForm = new SliderForm();
                childForm.Select = "CustomRotation";
                childForm.MdiParent = this;
                childForm.pf1 = toolStripStatusLabel1;
                childForm.NpBitmap = NpBitmap;
                childForm.OriginalTabTitle = msForm.GetCurrentTabTitle();
                childForm.Show();
            }
            else
            {
                MessageBox.Show("請先完成編輯");
                return;
            }
        }

        private void 水平翻轉ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MdiChildren.Length == 0)
            {
                MessageBox.Show("沒有開啟的圖片");
                return;
            }

            if (ActiveMdiChild is MSForm msForm)
            {
                NpBitmap = msForm.GetCurrentTabImage();
                Bitmap flippedBitmap = new Bitmap(NpBitmap);

                // 遍歷原圖的每個像素
                for (int x = 0; x < flippedBitmap.Width; x++)
                {
                    for (int y = 0; y < flippedBitmap.Height; y++)
                    {
                        // 計算左右翻轉後的位置
                        int newX = flippedBitmap.Width - 1 - x;
                        int newY = y;

                        // 設定像素
                        NpBitmap.SetPixel(newX, newY, flippedBitmap.GetPixel(x, y));
                    }
                }

                msForm.SetCurrentTabImage(NpBitmap);
                msForm.UpdateHistogram(NpBitmap);
            }
            else
            {
                MessageBox.Show("請先完成編輯");
                return;
            }
        }

        private void 垂直翻轉ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MdiChildren.Length == 0)
            {
                MessageBox.Show("沒有開啟的圖片");
                return;
            }

            if (ActiveMdiChild is MSForm msForm)
            {
                NpBitmap = msForm.GetCurrentTabImage();
                Bitmap flippedBitmap = new Bitmap(NpBitmap);

                // 遍歷原圖的每個像素
                for (int x = 0; x < flippedBitmap.Width; x++)
                {
                    for (int y = 0; y < flippedBitmap.Height; y++)
                    {
                        // 計算上下翻轉後的位置
                        int newX = x;
                        int newY = flippedBitmap.Height - 1 - y;

                        // 設定像素
                        NpBitmap.SetPixel(newX, newY, flippedBitmap.GetPixel(x, y));
                    }
                }

                msForm.SetCurrentTabImage(NpBitmap);
                msForm.UpdateHistogram(NpBitmap);
            }
            else
            {
                MessageBox.Show("請先完成編輯");
                return;
            }
        }

        private void 放大圖像ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MdiChildren.Length == 0)
            {
                MessageBox.Show("沒有開啟的圖片");
                return;
            }

            if (ActiveMdiChild is MSForm msForm)
            {
                // 顯示輸入框，預設值為 2
                string result = Interaction.InputBox("請輸入放大倍率", "放大圖像", "2");
                NpBitmap = msForm.GetCurrentTabImage();

                // 使用者按下取消時，result 會是空字串
                if (string.IsNullOrWhiteSpace(result))
                {
                    return;
                }

                // 嘗試將輸入轉換為數字
                if (double.TryParse(result, out double scale) && scale > 0)
                {
                    int new_weight = (int)Math.Round(NpBitmap.Width * scale);
                    int new_height = (int)Math.Round(NpBitmap.Height * scale);

                    // 將 Bitmap 轉換為陣列
                    int[] R, G, B;
                    ImageProcessUtils.BitmapToRGBArrays(NpBitmap, out R, out G, out B);

                    int[] processedR = new int[new_weight * new_height];
                    int[] processedG = new int[new_weight * new_height];
                    int[] processedB = new int[new_weight * new_height];

                    // 使用 unsafe 區域進行指針處理
                    unsafe
                    {
                        fixed (int* srcR = R, dstR = processedR)
                        fixed (int* srcG = G, dstG = processedG)
                        fixed (int* srcB = B, dstB = processedB)
                        {
                            // 進行放大的處理
                            ImageProcessUtils.zoom(srcR, NpBitmap.Width, NpBitmap.Height, scale, dstR);
                            ImageProcessUtils.zoom(srcG, NpBitmap.Width, NpBitmap.Height, scale, dstG);
                            ImageProcessUtils.zoom(srcB, NpBitmap.Width, NpBitmap.Height, scale, dstB);
                        }
                    }

                    // 將處理後的陣列轉換回 Bitmap
                    NpBitmap = ImageProcessUtils.RGBArraysToBitmap(processedR, processedG, processedB, new_weight, new_height);
                    msForm.SetCurrentTabImage(NpBitmap);
                    msForm.UpdateHistogram(NpBitmap);
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
        }

        private void 縮小圖像ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MdiChildren.Length == 0)
            {
                MessageBox.Show("沒有開啟的圖片");
                return;
            }

            if (ActiveMdiChild is MSForm msForm)
            {
                // 顯示輸入框，預設值為 2
                string result = Interaction.InputBox("請輸入縮小倍率", "縮小圖像", "2");
                NpBitmap = msForm.GetCurrentTabImage();

                // 使用者按下取消時，result 會是空字串
                if (string.IsNullOrWhiteSpace(result))
                {
                    return;
                }

                // 嘗試將輸入轉換為數字
                if (double.TryParse(result, out double scale) && scale > 0)
                {
                    scale = 1 / scale;

                    int new_weight = (int)Math.Round(NpBitmap.Width * scale);
                    int new_height = (int)Math.Round(NpBitmap.Height * scale);

                    // 將 Bitmap 轉換為陣列
                    int[] R, G, B;
                    ImageProcessUtils.BitmapToRGBArrays(NpBitmap, out R, out G, out B);

                    int[] processedR = new int[new_weight * new_height];
                    int[] processedG = new int[new_weight * new_height];
                    int[] processedB = new int[new_weight * new_height];

                    // 使用 unsafe 區域進行指針處理
                    unsafe
                    {
                        fixed (int* srcR = R, dstR = processedR)
                        fixed (int* srcG = G, dstG = processedG)
                        fixed (int* srcB = B, dstB = processedB)
                        {
                            // 進行縮小的處理
                            ImageProcessUtils.zoom(srcR, NpBitmap.Width, NpBitmap.Height, scale, dstR);
                            ImageProcessUtils.zoom(srcG, NpBitmap.Width, NpBitmap.Height, scale, dstG);
                            ImageProcessUtils.zoom(srcB, NpBitmap.Width, NpBitmap.Height, scale, dstB);
                        }
                    }

                    // 將處理後的陣列轉換回 Bitmap
                    NpBitmap = ImageProcessUtils.RGBArraysToBitmap(processedR, processedG, processedB, new_weight, new_height);
                    msForm.SetCurrentTabImage(NpBitmap);
                    msForm.UpdateHistogram(NpBitmap);
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
        }
    }
}
