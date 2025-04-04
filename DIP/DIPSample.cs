﻿using System;
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
                int[] f = ImageProcessUtils.BitmapToArray(NpBitmap);
                int[] g = new int[NpBitmap.Width * NpBitmap.Height]; // 假設 w 和 h 是圖片的寬高

                // 使用 unsafe 區域進行指針處理
                unsafe
                {
                    fixed (int* f0 = f) fixed (int* g0 = g)
                    {
                        ImageProcessUtils.negative(f0, NpBitmap.Width, NpBitmap.Height, g0); // 進行負片轉換的處理
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
                childForm.Select = 4;
                childForm.MdiParent = this;
                childForm.pf1 = toolStripStatusLabel1;
                childForm.NpBitmap = NpBitmap;
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
                childForm.Select = 0;
                childForm.MdiParent = this;
                childForm.pf1 = toolStripStatusLabel1;
                childForm.NpBitmap = NpBitmap;
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
                childForm.Select = 1;
                childForm.MdiParent = this;
                childForm.pf1 = toolStripStatusLabel1;
                childForm.NpBitmap = NpBitmap;
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
                int[] f = ImageProcessUtils.BitmapToArray(NpBitmap);
                int[] g = new int[NpBitmap.Width * NpBitmap.Height]; // 假設 w 和 h 是圖片的寬高

                // 使用 unsafe 區域進行指針處理
                unsafe
                {
                    fixed (int* f0 = f) fixed (int* g0 = g)
                    {
                        ImageProcessUtils.equalization(f0, NpBitmap.Width, NpBitmap.Height, g0); // 進行均衡化的處理
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

                // 將表單引用傳遞給子表單
                msForm.plotView = plotView;
                msForm.plotForm = plotForm;
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
                childForm.Select = 2;
                childForm.MdiParent = this;
                childForm.pf1 = toolStripStatusLabel1;
                childForm.NpBitmap = NpBitmap;
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
                int[] g = new int[NpBitmap.Width * NpBitmap.Height]; // 假設 w 和 h 是圖片的寬高

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

                double theta = 90 * Math.PI / 180;
                int new_weight = (int)Math.Round(NpBitmap.Height * Math.Abs(Math.Sin(theta)) + NpBitmap.Width * Math.Abs(Math.Cos(theta)));
                int new_height = (int)Math.Round(NpBitmap.Height * Math.Abs(Math.Cos(theta)) + NpBitmap.Width * Math.Abs(Math.Sin(theta)));

                // 將 Bitmap 轉換為陣列
                int[] f = ImageProcessUtils.BitmapToArray(NpBitmap);
                int[] g = new int[new_weight * new_height]; // 假設 w 和 h 是圖片的寬高

                // 使用 unsafe 區域進行指針處理
                unsafe
                {
                    fixed (int* f0 = f) fixed (int* g0 = g)
                    {
                        ImageProcessUtils.customrotationangle(f0, NpBitmap.Width, NpBitmap.Height, 90, g0);
                    }
                }

                // 將處理後的陣列轉換回 Bitmap
                NpBitmap = ImageProcessUtils.ArrayToBitmap(g, new_weight, new_height);
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

                double theta = 90 * Math.PI / 180;
                int new_weight = (int)Math.Round(NpBitmap.Height * Math.Abs(Math.Sin(theta)) + NpBitmap.Width * Math.Abs(Math.Cos(theta)));
                int new_height = (int)Math.Round(NpBitmap.Height * Math.Abs(Math.Cos(theta)) + NpBitmap.Width * Math.Abs(Math.Sin(theta)));

                // 將 Bitmap 轉換為陣列
                int[] f = ImageProcessUtils.BitmapToArray(NpBitmap);
                int[] g = new int[new_weight * new_height]; // 假設 w 和 h 是圖片的寬高

                // 使用 unsafe 區域進行指針處理
                unsafe
                {
                    fixed (int* f0 = f) fixed (int* g0 = g)
                    {
                        ImageProcessUtils.customrotationangle(f0, NpBitmap.Width, NpBitmap.Height, 180, g0);
                    }
                }

                // 將處理後的陣列轉換回 Bitmap
                NpBitmap = ImageProcessUtils.ArrayToBitmap(g, new_weight, new_height);
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

                double theta = 90 * Math.PI / 180;
                int new_weight = (int)Math.Round(NpBitmap.Height * Math.Abs(Math.Sin(theta)) + NpBitmap.Width * Math.Abs(Math.Cos(theta)));
                int new_height = (int)Math.Round(NpBitmap.Height * Math.Abs(Math.Cos(theta)) + NpBitmap.Width * Math.Abs(Math.Sin(theta)));

                // 將 Bitmap 轉換為陣列
                int[] f = ImageProcessUtils.BitmapToArray(NpBitmap);
                int[] g = new int[new_weight * new_height]; // 假設 w 和 h 是圖片的寬高

                // 使用 unsafe 區域進行指針處理
                unsafe
                {
                    fixed (int* f0 = f) fixed (int* g0 = g)
                    {
                        ImageProcessUtils.customrotationangle(f0, NpBitmap.Width, NpBitmap.Height, 270, g0);
                    }
                }

                // 將處理後的陣列轉換回 Bitmap
                NpBitmap = ImageProcessUtils.ArrayToBitmap(g, new_weight, new_height);
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
                childForm.Select = 3;
                childForm.MdiParent = this;
                childForm.pf1 = toolStripStatusLabel1;
                childForm.NpBitmap = NpBitmap;
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
                    int[] f = ImageProcessUtils.BitmapToArray(NpBitmap);
                    int[] g = new int[new_weight * new_height];

                    // 使用 unsafe 區域進行指針處理
                    unsafe
                    {
                        fixed (int* f0 = f) fixed (int* g0 = g)
                        {
                            ImageProcessUtils.zoom(f0, NpBitmap.Width, NpBitmap.Height, scale, g0); // 進行放大的處理
                        }
                    }

                    // 將處理後的陣列轉換回 Bitmap
                    NpBitmap = ImageProcessUtils.ArrayToBitmap(g, new_weight, new_height);
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
                    scale = 1 / scale;

                    int new_weight = (int)Math.Round(NpBitmap.Width * scale);
                    int new_height = (int)Math.Round(NpBitmap.Height * scale);

                    // 將 Bitmap 轉換為陣列
                    int[] f = ImageProcessUtils.BitmapToArray(NpBitmap);
                    int[] g = new int[new_weight * new_height];

                    // 使用 unsafe 區域進行指針處理
                    unsafe
                    {
                        fixed (int* f0 = f) fixed (int* g0 = g)
                        {
                            ImageProcessUtils.zoom(f0, NpBitmap.Width, NpBitmap.Height, scale, g0); // 進行放大的處理
                        }
                    }

                    // 將處理後的陣列轉換回 Bitmap
                    NpBitmap = ImageProcessUtils.ArrayToBitmap(g, new_weight, new_height);
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
