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
            this.toolStripStatusLabel2.Text = "";
            this.toolStripStatusLabel3.Text = "";
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
            // 檢查是否有表單存在
            bool histogramFormExists = this.MdiChildren.Any(form => form.Text == "直方圖");
            bool aboutFormExists = this.MdiChildren.OfType<AboutForm>().Any();

            // 根據是否存在子表單來啟用/禁用相關選單項目
            儲存ToolStripMenuItem.Enabled = baseEnabled;
            編輯ToolStripMenuItem.Enabled = baseEnabled;
            視圖ToolStripMenuItem.Enabled = baseEnabled;
            圖像toolStripMenuItem.Enabled = baseEnabled;
            濾波器ToolStripMenuItem.Enabled = baseEnabled;
            // 如果表單存在，則禁用直方圖選單項目
            直方圖ToolStripMenuItem.Enabled = !histogramFormExists;
            關於ToolStripMenuItem.Enabled = !aboutFormExists;
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
                            childForm.pf2 = toolStripStatusLabel2;
                            childForm.pf3 = toolStripStatusLabel3;
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

                // 使用泛用方法處理負片
                Action<IntPtr, IntPtr, int, int, object[]> negativeWrapper = (srcPtr, dstPtr, srcW, srcH, extra) =>
                {
                    unsafe { ImageProcessUtils.negative((int*)srcPtr, (int*)dstPtr, srcW, srcH); }
                };

                // 將處理後的陣列轉換回 Bitmap
                NpBitmap = ImageProcessUtils.ProcessBitmapChannels(NpBitmap, NpBitmap.Width, NpBitmap.Height, negativeWrapper);
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
                childForm.pf2 = toolStripStatusLabel2;
                childForm.pf3 = toolStripStatusLabel3;
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
                childForm.pf2 = toolStripStatusLabel2;
                childForm.pf3 = toolStripStatusLabel3;
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
                childForm.pf2 = toolStripStatusLabel2;
                childForm.pf3 = toolStripStatusLabel3;
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

                // 使用泛用方法處理均衡化
                Action<IntPtr, IntPtr, int, int, object[]> equalizationWrapper = (srcPtr, dstPtr, srcW, srcH, extra) =>
                {
                    unsafe { ImageProcessUtils.equalization((int*)srcPtr, (int*)dstPtr, srcW, srcH); }
                };

                // 將處理後的陣列轉換回 Bitmap
                NpBitmap = ImageProcessUtils.ProcessBitmapChannels(NpBitmap, NpBitmap.Width, NpBitmap.Height, equalizationWrapper);
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
                int threshold = 0;
                
                // 使用 unsafe 區域進行指針處理
                unsafe
                {
                    fixed (int* src = f) fixed (int* dst = g)
                    {
                        ImageProcessUtils.otsuthreshold(src, dst, NpBitmap.Width, NpBitmap.Height, &threshold); // 進行二值化的處理
                    }
                }

                // 將處理後的陣列轉換回 Bitmap
                NpBitmap = ImageProcessUtils.ArrayToBitmap(g, NpBitmap.Width, NpBitmap.Height);
                msForm.SetCurrentTabImage(NpBitmap);
                msForm.UpdateHistogram(NpBitmap);
                msForm.SetCurrentTabInfo(threshold: threshold);
                msForm.UpdateStatusBar();
            }
            else
            {
                MessageBox.Show("請先完成編輯");
                return;
            }
        }

        private void 連通標記toolStripMenuItem_Click(object sender, EventArgs e)
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
                int labelCount = 0;

                // 使用 unsafe 區域進行指針處理
                unsafe
                {
                    fixed (int* src = f)
                    {
                        ImageProcessUtils.connectedcomponent(src, NpBitmap.Width, NpBitmap.Height, &labelCount); // 進行連通標記的處理
                    }
                }

                msForm.SetCurrentTabInfo(labelCount: labelCount);
                msForm.UpdateStatusBar();
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
                childForm.pf2 = toolStripStatusLabel2;
                childForm.pf3 = toolStripStatusLabel3;
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
                    fixed (int* src = f) fixed (int* dst = g)
                    {
                        ImageProcessUtils.highpassfilter(src, dst, NpBitmap.Width, NpBitmap.Height); // 進行高通濾波器的處理
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
                childForm.pf2 = toolStripStatusLabel2;
                childForm.pf3 = toolStripStatusLabel3;
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

                // 使用泛用方法處理旋轉90度
                Action<IntPtr, IntPtr, int, int, object[]> rotate90Wrapper = (srcPtr, dstPtr, srcW, srcH, extra) =>
                {
                    unsafe { ImageProcessUtils.customrotationangle((int*)srcPtr, (int*)dstPtr, srcW, srcH, (int)extra[0]); }
                };

                // 將處理後的陣列轉換回 Bitmap
                NpBitmap = ImageProcessUtils.ProcessBitmapChannels(NpBitmap, new_weight, new_height, rotate90Wrapper, 90);
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

                // 使用泛用方法處理旋轉180度
                Action<IntPtr, IntPtr, int, int, object[]> rotate180Wrapper = (srcPtr, dstPtr, srcW, srcH, extra) =>
                {
                    unsafe { ImageProcessUtils.customrotationangle((int*)srcPtr, (int*)dstPtr, srcW, srcH, (int)extra[0]); }
                };

                // 將處理後的陣列轉換回 Bitmap
                NpBitmap = ImageProcessUtils.ProcessBitmapChannels(NpBitmap, new_weight, new_height, rotate180Wrapper, 180);
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

                // 使用泛用方法處理旋轉270度
                Action<IntPtr, IntPtr, int, int, object[]> rotate270Wrapper = (srcPtr, dstPtr, srcW, srcH, extra) =>
                {
                    unsafe { ImageProcessUtils.customrotationangle((int*)srcPtr, (int*)dstPtr, srcW, srcH, (int)extra[0]); }
                };

                // 將處理後的陣列轉換回 Bitmap
                NpBitmap = ImageProcessUtils.ProcessBitmapChannels(NpBitmap, new_weight, new_height, rotate270Wrapper, 270);
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
                childForm.pf2 = toolStripStatusLabel2;
                childForm.pf3 = toolStripStatusLabel3;
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

        private void 位元切片ToolStripMenuItem_Click(object sender, EventArgs e)
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
                childForm.Select = "BitPlane";
                childForm.MdiParent = this;
                childForm.pf1 = toolStripStatusLabel1;
                childForm.pf2 = toolStripStatusLabel2;
                childForm.pf3 = toolStripStatusLabel3;
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

                // 使用泛用方法處理水平翻轉
                Action<IntPtr, IntPtr, int, int, object[]> flipHWrapper = (srcPtr, dstPtr, srcW, srcH, extra) =>
                {
                    unsafe { ImageProcessUtils.fliphorizontal((int*)srcPtr, (int*)dstPtr, srcW, srcH); }
                };

                // 將處理後的陣列轉換回 Bitmap
                NpBitmap = ImageProcessUtils.ProcessBitmapChannels(NpBitmap, NpBitmap.Width, NpBitmap.Height, flipHWrapper);
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

                // 使用泛用方法處理垂直翻轉
                Action<IntPtr, IntPtr, int, int, object[]> flipVWrapper = (srcPtr, dstPtr, srcW, srcH, extra) =>
                {
                    unsafe { ImageProcessUtils.flipvertical((int*)srcPtr, (int*)dstPtr, srcW, srcH); }
                };

                // 將處理後的陣列轉換回 Bitmap
                NpBitmap = ImageProcessUtils.ProcessBitmapChannels(NpBitmap, NpBitmap.Width, NpBitmap.Height, flipVWrapper);
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

                    // 使用泛用方法處理放大
                    Action<IntPtr, IntPtr, int, int, object[]> zoomWrapper = (srcPtr, dstPtr, srcW, srcH, extra) =>
                    {
                        unsafe { ImageProcessUtils.zoom((int*)srcPtr, (int*)dstPtr, srcW, srcH, (double)extra[0]); }
                    };

                    // 將處理後的陣列轉換回 Bitmap
                    NpBitmap = ImageProcessUtils.ProcessBitmapChannels(NpBitmap, new_weight, new_height, zoomWrapper, scale);
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

                    // 使用泛用方法處理縮小
                    Action<IntPtr, IntPtr, int, int, object[]> zoomWrapper = (srcPtr, dstPtr, srcW, srcH, extra) =>
                    {
                        unsafe { ImageProcessUtils.zoom((int*)srcPtr, (int*)dstPtr, srcW, srcH, (double)extra[0]); }
                    };

                    // 將處理後的陣列轉換回 Bitmap
                    NpBitmap = ImageProcessUtils.ProcessBitmapChannels(NpBitmap, new_weight, new_height, zoomWrapper, scale);
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

        private void 關於ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm childForm = this.MdiChildren.OfType<AboutForm>().FirstOrDefault();

            if (childForm == null)
            {
                childForm = new AboutForm();
                childForm.MdiParent = this;
                childForm.Show();
            }
        }
    }
}
