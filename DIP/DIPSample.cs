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

namespace DIP
{
    public partial class DIPSample : Form
    {
        Bitmap NpBitmap;
        int w, h;

        public DIPSample()
        {
            InitializeComponent();
        }

        [DllImport("B11217048.dll", CallingConvention = CallingConvention.Cdecl)]
        unsafe public static extern void negative(int* f0, int w, int h, int* g0);
        [DllImport("B11217048.dll", CallingConvention = CallingConvention.Cdecl)]
        unsafe public static extern void mosaic(int* f0, int w, int h, int* g0);
        [DllImport("B11217048.dll", CallingConvention = CallingConvention.Cdecl)]
        unsafe public static extern void histogram(int* f0, int w, int h, int* g0);
        [DllImport("B11217048.dll", CallingConvention = CallingConvention.Cdecl)]
        unsafe public static extern void equalization(int* f0, int w, int h, int* g0);
        [DllImport("B11217048.dll", CallingConvention = CallingConvention.Cdecl)]
        unsafe public static extern void highpassfilter(int* f0, int w, int h, int* g0);

        private void DIPSample_Load(object sender, EventArgs e)
        {
            this.IsMdiContainer = true; // 設定為 MDI 容器
            this.toolStripStatusLabel1.Text = "";
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
                w = NpBitmap.Width;
                h = NpBitmap.Height;
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
                    // 根據視窗類型來選擇對應的 pBitmap
                    if (cF is SliderForm sliderForm)
                    {
                        MessageBox.Show("請先完成編輯");
                        return;
                    }
                    else if (cF is MSForm msForm)
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
                    // 根據視窗類型來選擇對應的 pBitmap
                    if (cF is SliderForm sliderForm)
                    {
                        MessageBox.Show("請先完成編輯");
                        return;
                    }
                    else if (cF is MSForm msForm)
                    {
                        // 將 Bitmap 轉換為陣列
                        int[] f = bmp2array(msForm.pBitmap);
                        int[] g = new int[w * h]; // 假設 w 和 h 是圖片的寬高

                        // 使用 unsafe 區域進行指針處理
                        unsafe
                        {
                            fixed (int* f0 = f) fixed (int* g0 = g)
                            {
                                negative(f0, w, h, g0); // 進行負片轉換的處理
                            }
                        }
                        // 將處理後的陣列轉換回 Bitmap
                        NpBitmap = array2bmp(g, msForm.pBitmap.Width, msForm.pBitmap.Height);
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

            // 遍歷所有 MDI 子視窗，尋找當前聚焦的視窗
            foreach (Form cF in MdiChildren)
            {
                if (cF == ActiveMdiChild) // 使用 ActiveMdiChild 確保處理當前聚焦的視窗
                {
                    // 根據視窗類型來選擇對應的 pBitmap
                    if (cF is SliderForm sliderForm)
                    {
                        MessageBox.Show("請先完成編輯");
                        return;
                    }
                    else if (cF is MSForm msForm)
                    {
                        // 將 Bitmap 轉換為陣列
                        int[] f = bmp2array(msForm.pBitmap);
                        int[] g = new int[w * h]; // 假設 w 和 h 是圖片的寬高

                        // 使用 unsafe 區域進行指針處理
                        unsafe
                        {
                            fixed (int* f0 = f) fixed (int* g0 = g)
                            {
                                mosaic(f0, w, h, g0); // 進行局部馬賽克的處理
                            }
                        }

                        // 將處理後的陣列轉換回 Bitmap
                        NpBitmap = array2bmp(g, msForm.pBitmap.Width, msForm.pBitmap.Height);
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
                    // 根據視窗類型來選擇對應的 pBitmap
                    
                    if (cF is SliderForm sliderForm)
                    {
                        MessageBox.Show("已經開啟調整視窗");
                        return;
                    }
                    else if (cF is MSForm msForm)
                    {
                        NpBitmap = new Bitmap(msForm.pBitmap);
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
                    // 根據視窗類型來選擇對應的 pBitmap
                    if (cF is SliderForm sliderForm)
                    {
                        MessageBox.Show("已經開啟調整視窗");
                        return;
                    }
                    else if (cF is MSForm msForm)
                    {
                        NpBitmap = new Bitmap(msForm.pBitmap);
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
                    // 根據視窗類型來選擇對應的 pBitmap
                    if (cF is SliderForm sliderForm)
                    {
                        MessageBox.Show("請先完成編輯");
                        return;
                    }
                    else if (cF is MSForm msForm)
                    {
                        // 將 Bitmap 轉換為陣列
                        int[] f = bmp2array(msForm.pBitmap);
                        int[] g = new int[w * h]; // 假設 w 和 h 是圖片的寬高

                        // 使用 unsafe 區域進行指針處理
                        unsafe
                        {
                            fixed (int* f0 = f) fixed (int* g0 = g)
                            {
                                equalization(f0, w, h, g0); // 進行局部馬賽克的處理
                            }
                        }

                        // 將處理後的陣列轉換回 Bitmap
                        NpBitmap = array2bmp(g, msForm.pBitmap.Width, msForm.pBitmap.Height);
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
                    // 根據視窗類型來選擇對應的 pBitmap
                    if (cF is SliderForm sliderForm)
                    {
                        MessageBox.Show("請先完成編輯");
                        return;
                    }
                    else if (cF is MSForm msForm)
                    {
                        // 將 Bitmap 轉換為陣列
                        int[] f = bmp2array(msForm.pBitmap);
                        int[] g = new int[256 * 256]; // 假設 w 和 h 是圖片的寬高

                        // 使用 unsafe 區域進行指針處理
                        unsafe
                        {
                            fixed (int* f0 = f) fixed (int* g0 = g)
                            {
                                histogram(f0, w, h, g0); // 進行直方圖的計算
                            }
                        }

                        // 將處理後的陣列轉換回 Bitmap
                        NpBitmap = array2bmp(g, 256, 256);
                    }

                    break; // 找到聚焦的視窗後跳出迴圈
                }
            }

            MSForm childForm = new MSForm();
            childForm.MdiParent = this;
            childForm.pf1 = toolStripStatusLabel1;
            childForm.pBitmap = NpBitmap;
            childForm.isHistogram = true;
            childForm.Show();
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
                    // 根據視窗類型來選擇對應的 pBitmap
                    if (cF is SliderForm sliderForm)
                    {
                        MessageBox.Show("已經開啟調整視窗！");
                        return;
                    }
                    else if (cF is MSForm msForm)
                    {
                        NpBitmap = new Bitmap(msForm.pBitmap);
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
                    // 根據視窗類型來選擇對應的 pBitmap
                    if (cF is SliderForm sliderForm)
                    {
                        MessageBox.Show("請先完成編輯");
                        return;
                    }
                    else if (cF is MSForm msForm)
                    {
                        // 將 Bitmap 轉換為陣列
                        int[] f = bmp2array(msForm.pBitmap);
                        int[] g = new int[w * h]; // 假設 w 和 h 是圖片的寬高

                        // 使用 unsafe 區域進行指針處理
                        unsafe
                        {
                            fixed (int* f0 = f) fixed (int* g0 = g)
                            {
                                highpassfilter(f0, w, h, g0); // 進行局部馬賽克的處理
                            }
                        }

                        // 將處理後的陣列轉換回 Bitmap
                        NpBitmap = array2bmp(g, msForm.pBitmap.Width, msForm.pBitmap.Height);
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
                    // 根據視窗類型來選擇對應的 pBitmap
                    if (cF is SliderForm sliderForm)
                    {
                        MessageBox.Show("已經開啟調整視窗！");
                        return;
                    }
                    else if (cF is MSForm msForm)
                    {
                        NpBitmap = new Bitmap(msForm.pBitmap);
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
                    // 根據視窗類型來選擇對應的 pBitmap
                    if (cF is SliderForm sliderForm)
                    {
                        MessageBox.Show("請先完成編輯");
                        return;
                    }
                    else if (cF is MSForm msForm)
                    {
                        Bitmap rotatedBitmap = new Bitmap(msForm.pBitmap.Height, msForm.pBitmap.Width); // 90° 旋轉後寬高互換

                        // 遍歷原圖的每個像素
                        for (int y = 0; y < msForm.pBitmap.Height; y++)
                        {
                            for (int x = 0; x < msForm.pBitmap.Width; x++)
                            {
                                // 計算 90° 旋轉後的位置
                                int newX = msForm.pBitmap.Height - 1 - y;
                                int newY = x;

                                // 設定像素
                                rotatedBitmap.SetPixel(newX, newY, msForm.pBitmap.GetPixel(x, y));
                            }
                        }

                        NpBitmap = rotatedBitmap;
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
                    // 根據視窗類型來選擇對應的 pBitmap
                    if (cF is SliderForm sliderForm)
                    {
                        MessageBox.Show("請先完成編輯");
                        return;
                    }
                    else if (cF is MSForm msForm)
                    {
                        Bitmap rotatedBitmap = new Bitmap(msForm.pBitmap.Width, msForm.pBitmap.Height); // 180° 旋轉後大小不變
                                                                                                 // 遍歷原圖的每個像素
                        for (int y = 0; y < msForm.pBitmap.Height; y++)
                        {
                            for (int x = 0; x < msForm.pBitmap.Width; x++)
                            {
                                // 計算 180° 旋轉後的位置
                                int newX = msForm.pBitmap.Width - 1 - x;
                                int newY = msForm.pBitmap.Height - 1 - y;

                                // 設定像素
                                rotatedBitmap.SetPixel(newX, newY, msForm.pBitmap.GetPixel(x, y));
                            }
                        }

                        NpBitmap = rotatedBitmap;
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
                    // 根據視窗類型來選擇對應的 pBitmap
                    if (cF is SliderForm sliderForm)
                    {
                        MessageBox.Show("請先完成編輯");
                        return;
                    }
                    else if (cF is MSForm msForm)
                    {
                        Bitmap rotatedBitmap = new Bitmap(msForm.pBitmap.Height, msForm.pBitmap.Width); // 270° 旋轉後寬高互換
                        // 遍歷原圖的每個像素
                        for (int y = 0; y < msForm.pBitmap.Height; y++)
                        {
                            for (int x = 0; x < msForm.pBitmap.Width; x++)
                            {
                                // 計算 270° 旋轉後的位置
                                int newX = y;
                                int newY = msForm.pBitmap.Width - 1 - x;

                                // 設定像素
                                rotatedBitmap.SetPixel(newX, newY, msForm.pBitmap.GetPixel(x, y));
                            }
                        }

                        NpBitmap = rotatedBitmap;
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
                    // 根據視窗類型來選擇對應的 pBitmap
                    if (cF is SliderForm sliderForm)
                    {
                        MessageBox.Show("請先完成編輯");
                        return;
                    }
                    else if (cF is MSForm msForm)
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
                    // 根據視窗類型來選擇對應的 pBitmap
                    if (cF is SliderForm sliderForm)
                    {
                        MessageBox.Show("請先完成編輯");
                        return;
                    }
                    else if (cF is MSForm msForm)
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
                    
                    break; // 找到聚焦的視窗後跳出迴圈
                }
            }

            MSForm childForm = new MSForm();
            childForm.MdiParent = this;
            childForm.pf1 = toolStripStatusLabel1;
            childForm.pBitmap = NpBitmap;
            childForm.Show();
        }

        private Bitmap bmp_read(OpenFileDialog oFileDlg)
        {
            Bitmap pBitmap;
            string fileloc = oFileDlg.FileName;
            pBitmap = new Bitmap(fileloc);
            w = pBitmap.Width;
            h = pBitmap.Height;
            return pBitmap;
        }

        private int[] bmp2array(Bitmap myBitmap)
        {
            int[] ImgData = new int[myBitmap.Width * myBitmap.Height];
            BitmapData byteArray = myBitmap.LockBits(new Rectangle(0, 0, myBitmap.Width, myBitmap.Height),
                                                    ImageLockMode.ReadWrite,
                                                    myBitmap.PixelFormat);
            int ByteOfSkip = byteArray.Stride - byteArray.Width * (int)(byteArray.Stride / myBitmap.Width);

            // 判斷圖像格式
            bool isGrayscale = (myBitmap.PixelFormat == PixelFormat.Format8bppIndexed);
            bool is32bpp = (myBitmap.PixelFormat == PixelFormat.Format32bppArgb ||
                            myBitmap.PixelFormat == PixelFormat.Format32bppRgb);

            unsafe
            {
                byte* imgPtr = (byte*)(byteArray.Scan0);

                for (int y = 0; y < byteArray.Height; y++)
                {
                    for (int x = 0; x < byteArray.Width; x++)
                    {
                        int index = x + byteArray.Width * y;

                        if (isGrayscale)
                        {
                            ImgData[index] = (int)*(imgPtr);
                            imgPtr++; // 每個灰階像素佔用 1 byte
                        }
                        else
                        {
                            byte blue = imgPtr[0];
                            byte green = imgPtr[1];
                            byte red = imgPtr[2];
                            byte alpha = is32bpp ? imgPtr[3] : (byte)0; // 32bpp圖像有alpha通道

                            // 計算灰階值
                            int grayValue = (int)(0.3 * red + 0.59 * green + 0.11 * blue);
                            ImgData[index] = grayValue;

                            imgPtr += is32bpp ? 4 : 3;
                        }
                    }
                    imgPtr += ByteOfSkip;
                }
            }

            myBitmap.UnlockBits(byteArray);
            return ImgData;
        }

        private static Bitmap array2bmp(int[] ImgData, int Width, int Height)
        {
            Bitmap myBitmap = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);
            BitmapData byteArray = myBitmap.LockBits(new Rectangle(0, 0, Width, Height),
                                           ImageLockMode.WriteOnly,
                                           PixelFormat.Format24bppRgb);
            int ByteOfSkip = byteArray.Stride - Width * 3;

            unsafe
            {
                byte* imgPtr = (byte*)byteArray.Scan0;

                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        int index = x + Width * y;
                        byte grayValue = (byte)ImgData[index];

                        // 設定 R, G, B 為相同的灰階值
                        *imgPtr = grayValue;       // B
                        *(imgPtr + 1) = grayValue; // G
                        *(imgPtr + 2) = grayValue; // R

                        imgPtr += 3;
                    }
                    imgPtr += ByteOfSkip;
                }
            }

            myBitmap.UnlockBits(byteArray);
            return myBitmap;
        }
    }
}
