using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using OxyPlot;
using OxyPlot.WindowsForms;

namespace DIP
{
    public partial class MSForm : Form
    {
        private Dictionary<string, Bitmap> originalImages = new Dictionary<string, Bitmap>();
        private readonly object _updateLock = new object();
        internal PlotView plotView = null;
        internal Form plotForm = null;
        internal Bitmap pBitmap;
        internal ToolStripStatusLabel pf1;
        internal ToolStripStatusLabel pf2;
        internal ToolStripStatusLabel pf3;

        public MSForm()
        {
            InitializeComponent();
            SetupTabContextMenu();

            tabControl1.DrawItem += TabControl_DrawItem;
        }

        private void MSForm_Load(object sender, EventArgs e)
        {
            bmp_dip(pBitmap);
        }

        private void bmp_dip(Bitmap pBitmap)
        {
            this.Width = pBitmap.Width + 100 + (this.Width - this.ClientRectangle.Width);
            this.Height = pBitmap.Height + 100 + tabControl1.ItemSize.Height + (this.Height - this.ClientRectangle.Height);

            if (pBitmap.Width < 256) this.Width = 356 + (this.Width - this.ClientRectangle.Width);
            if (pBitmap.Height < 256) this.Height = 356 + tabControl1.ItemSize.Height + (this.Height - this.ClientRectangle.Height);
        }

        public void AddImageTab(string title, Bitmap image)
        {
            // 檢查是否已存在相同標題的 Tab
            foreach (TabPage tab in tabControl1.TabPages)
            {
                if (tab.Text == title)
                {
                    // 切換到現有的 Tab 並返回
                    tabControl1.SelectedTab = tab;
                    return;
                }
            }

            // 建立一個新的 TabPage
            TabPage tabPage = new TabPage(title);

            // 建立 PictureBox 來顯示圖片
            PictureBox pictureBox = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.CenterImage,
                Image = image
            };

            // 綁定 MouseMove 事件
            pictureBox.MouseMove += pictureBox1_MouseMove;
            // 把 PictureBox 加入 TabPage
            tabPage.Controls.Add(pictureBox);
            // 把 TabPage 加入 TabControl
            tabControl1.TabPages.Add(tabPage);
            // 切換到最新加入的 Tab
            tabControl1.SelectedTab = tabPage;
            // 儲存原始圖片副本
            originalImages[title] = new Bitmap(image);

            // 檢查新圖片是否大於目前視窗客戶區，若是則調整視窗大小
            if (image.Width > this.ClientRectangle.Width || image.Height > this.ClientRectangle.Height)
            {
                bmp_dip(image);
            }

            UpdateStatusBar();
        }

        public string GetCurrentTabTitle()
        {
            if (tabControl1.TabCount == 0 || tabControl1.SelectedTab == null)
                return "Untitled";

            return tabControl1.SelectedTab.Text;
        }

        public Bitmap GetCurrentTabImage()
        {
            if (tabControl1.TabCount == 0 || tabControl1.SelectedTab == null)
                return null;

            var pictureBox = tabControl1.SelectedTab.Controls.OfType<PictureBox>().FirstOrDefault();
            return (pictureBox?.Image != null) ? new Bitmap(pictureBox.Image) : null;
        }

        public void SetCurrentTabImage(Bitmap newImage)
        {
            if (tabControl1.TabCount == 0 || tabControl1.SelectedTab == null)
                return;

            var pictureBox = tabControl1.SelectedTab.Controls.OfType<PictureBox>().FirstOrDefault();
            if (pictureBox != null)
            {
                // 檢查新圖片是否有改變大小，若是則調整視窗大小
                if (newImage.Width != pictureBox.Image.Width || newImage.Height != pictureBox.Image.Height)
                {
                    bmp_dip(newImage);
                }

                if (pictureBox.Image != null)
                    pictureBox.Image.Dispose();

                pictureBox.Image = newImage;
            }
        }

        public void SetCurrentTabThreshold(int t)
        {
            if (tabControl1.SelectedTab != null)
                tabControl1.SelectedTab.Tag = t;
        }

        public int? GetCurrentTabThreshold()
        {
            if (tabControl1.SelectedTab != null && tabControl1.SelectedTab.Tag is int t)
                return t;
            return null;
        }

        public void UpdateStatusBar(int? x = null, int? y = null, Color? pixel = null)
        {
            if (pf1 == null || pf2 == null || pf3 == null) return;

            // pf1: (Width,Height)
            Bitmap image = GetCurrentTabImage();
            if (image != null)
            { 
                pf1.Text = $"(Width,Height): ({image.Width},{image.Height})";
                pf1.BorderSides = ToolStripStatusLabelBorderSides.All;
            }
            else
            {
                pf1.Text = "";
                pf1.BorderSides = ToolStripStatusLabelBorderSides.None;
            }

            // pf2: (X, Y) 與 RGB
            if (x.HasValue && y.HasValue && pixel.HasValue)
            {
                pf2.Text = $"(X, Y): ({x}, {y})  RGB: ({pixel.Value.R}, {pixel.Value.G}, {pixel.Value.B})";
                pf2.BorderSides = ToolStripStatusLabelBorderSides.All;
            }
            else
            {
                pf2.Text = "";
                pf2.BorderSides = ToolStripStatusLabelBorderSides.None;
            }

            // pf3: Threshold
            int? threshold = GetCurrentTabThreshold();
            if (threshold.HasValue)
            {
                pf3.Text = $"Threshold: {threshold.Value}";
                pf3.BorderSides = ToolStripStatusLabelBorderSides.All;
            }
            else
            {
                pf3.Text = "";
                pf3.BorderSides = ToolStripStatusLabelBorderSides.None;
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            var pictureBox = sender as PictureBox;
            if (pictureBox?.Image == null)
            {
                return;
            }

            // CenterImage 專用座標轉換
            int centerX = (pictureBox.Width - pictureBox.Image.Width) / 2;
            int centerY = (pictureBox.Height - pictureBox.Image.Height) / 2;
            int imgX = e.X - centerX;
            int imgY = e.Y - centerY;

            // 檢查座標是否在圖片範圍內
            if (imgX >= 0 && imgX < pictureBox.Image.Width &&
                imgY >= 0 && imgY < pictureBox.Image.Height)
            {
                Color imgPixel = ((Bitmap)pictureBox.Image).GetPixel(imgX, imgY);
                UpdateStatusBar(x: imgX, y: imgY, pixel: imgPixel);
            }
            else
            {
                UpdateStatusBar(x: null, y: null, pixel: null);
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            pBitmap = GetCurrentTabImage();
            UpdateHistogram(pBitmap);
            UpdateStatusBar();
        }

        public void UpdateHistogram(Bitmap pBitmap)
        {
            if (plotView == null || plotForm == null || plotForm.IsDisposed || pBitmap == null)
                return;

            lock (_updateLock)
            {
                var originalController = plotView.Controller;

                try
                {
                    // 禁用所有交互
                    plotView.Controller = new PlotController();

                    // 同步計算數據
                    int[] histogram;
                    using (var bitmapCopy = new Bitmap(pBitmap)) // 使用副本避免GDI+鎖
                    {
                        histogram = ImageProcessUtils.CalculateHistogram(bitmapCopy);
                    }

                    // 創建模型並更新
                    var plotModel = ImageProcessUtils.CreateHistogramPlotModel(histogram);
                    plotView.Model = plotModel;
                    plotView.InvalidatePlot(true);
                }
                finally
                {
                    // 延遲綁定交互
                    Task.Delay(150).ContinueWith(_ =>
                    {
                        if (plotForm != null && !plotForm.IsDisposed && plotView != null)
                        {
                            plotView.Controller = originalController;
                        }
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
        }

        private void SetupTabContextMenu()
        {
            tabControl1.ContextMenuStrip = new ContextMenuStrip();

            tabControl1.ContextMenuStrip.Items.Add("還原圖片", null, (sender, e) =>
            {
                RestoreOriginalImage(tabControl1.SelectedTab.Text);
            });

            tabControl1.ContextMenuStrip.Items.Add("複製分頁", null, (sender, e) =>
            {
                pBitmap = GetCurrentTabImage();

                string originalTitle = tabControl1.SelectedTab.Text;
                string newTitle = GenerateCopyTabTitle(originalTitle);

                AddImageTab(newTitle, pBitmap);
            });

            tabControl1.ContextMenuStrip.Items.Add("關閉分頁", null, (sender, e) =>
            {
                // 釋放資源
                var pictureBox = tabControl1.SelectedTab.Controls
                                     .OfType<PictureBox>()
                                     .FirstOrDefault();
                pictureBox?.Image?.Dispose();

                // 移除分頁
                tabControl1.TabPages.Remove(tabControl1.SelectedTab);

                if (tabControl1.TabCount == 0)
                {
                    this.Close();
                }
            });
        }

        private string GenerateCopyTabTitle(string originalTitle)
        {
            // 取得基礎標題（移除可能的「 - 複製」後綴）
            string baseTitle = originalTitle.Contains(" - 複製")
                ? originalTitle.Split(new[] { " - 複製" }, StringSplitOptions.None)[0]
                : originalTitle;

            // 計算下一個可用編號
            int nextNumber = 1; // 預設從1開始
            bool hasUnnumberedCopy = false;

            foreach (TabPage tab in tabControl1.TabPages)
            {
                if (tab.Text.StartsWith(baseTitle + " - 複製"))
                {
                    if (tab.Text == baseTitle + " - 複製")
                    {
                        hasUnnumberedCopy = true;
                    }
                    else
                    {
                        var match = Regex.Match(tab.Text, @" - 複製\((\d+)\)$");
                        if (match.Success)
                        {
                            int currentNum = int.Parse(match.Groups[1].Value);
                            nextNumber = Math.Max(nextNumber, currentNum + 1);
                        }
                    }
                }
            }

            // 決定標題格式
            return hasUnnumberedCopy
                ? $"{baseTitle} - 複製({nextNumber})"
                : (nextNumber > 1
                    ? $"{baseTitle} - 複製({nextNumber})"
                    : $"{baseTitle} - 複製");
        }

        public void RestoreOriginalImage(string tabTitle)
        {
            if (originalImages.TryGetValue(tabTitle, out Bitmap original))
            {
                var tabPage = tabControl1.TabPages.Cast<TabPage>()
                                .FirstOrDefault(t => t.Text == tabTitle);

                if (tabPage != null)
                {
                    tabPage.Tag = null;

                    var pictureBox = tabPage.Controls.OfType<PictureBox>().FirstOrDefault();
                    if (pictureBox != null)
                    {
                        // 檢查新圖片是否有改變大小，若是則調整視窗大小
                        if (original.Width != pictureBox.Image.Width || original.Height != pictureBox.Image.Height)
                        {
                            bmp_dip(original);
                        }

                        // 釋放當前圖片
                        pictureBox.Image?.Dispose();
                        // 還原為原始圖片
                        pictureBox.Image = new Bitmap(original); // 再創建一個新副本
                        UpdateHistogram(original); // 更新直方圖
                    }
                }
            }
        }

        private void TabControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            var tc = (TabControl)sender;
            bool isSelected = (tc.SelectedIndex == e.Index);

            // 繪製背景（選中=藍色，未選中=系統默認）
            using (var brush = new SolidBrush(isSelected ? Color.DodgerBlue : SystemColors.Control))
            {
                e.Graphics.FillRectangle(brush, e.Bounds);
            }

            // 繪製文字
            TextRenderer.DrawText(
                e.Graphics,
                tc.TabPages[e.Index].Text,
                e.Font,
                e.Bounds,
                isSelected ? Color.White : SystemColors.ControlText,
                TextFormatFlags.HorizontalCenter |
                TextFormatFlags.VerticalCenter |
                TextFormatFlags.NoPrefix);
        }

        private void MSForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (plotForm != null && !plotForm.IsDisposed)
            {
                plotView.Dispose();
                plotForm.Dispose();
                plotForm.Close();
            }
            // 釋放所有 tab 內的圖片資源並釋放 tab
            while (tabControl1.TabPages.Count > 0)
            {
                TabPage tab = tabControl1.TabPages[0];
                var pictureBox = tab.Controls.OfType<PictureBox>().FirstOrDefault();
                if (pictureBox?.Image != null)
                {
                    pictureBox.Image.Dispose();
                    pictureBox.Image = null;
                }
                tabControl1.TabPages.Remove(tab);
                tab.Dispose();
            }
            // 釋放 originalImages 字典的 Bitmap
            foreach (var bmp in originalImages.Values)
            {
                bmp.Dispose();
            }
            originalImages.Clear();
            UpdateStatusBar();
        }
    }
}
