using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace DIP
{
    public partial class SliderForm : Form
    {
        internal Bitmap NpBitmap;
        internal Bitmap pBitmap;
        internal ToolStripStatusLabel pf1;
        internal ToolStripStatusLabel pf2;
        internal ToolStripStatusLabel pf3;
        internal string OriginalTabTitle;
        public new string Select;
        private bool isSelecting = false; // 是否正在框選
        private Point selectionStart; // 框選的起始點
        private Point selectionEnd; // 框選的結束點
        private Rectangle selectionRect; // 當前框選區域

        public SliderForm()
        {
            InitializeComponent();
        }

        private void SliderForm_Load(object sender, EventArgs e)
        {
            bmp_dip(NpBitmap, pictureBox1);
            // 創建 pBitmap 的副本，避免共用引用
            pBitmap = new Bitmap(NpBitmap);
            // 設置初始圖像
            pictureBox1.Image = NpBitmap;

            switch (Select)
            {
                case "Brightness":
                    label1.Text = "亮度";
                    trackBar1.Minimum = -255;
                    trackBar1.Maximum = 255;
                    trackBar1.Value = 0;
                    textBox1.Text = "0";
                    break;

                case "Contrast":
                    label1.Text = "對比度";
                    trackBar1.Minimum = 0;
                    trackBar1.Maximum = 300;
                    trackBar1.Value = 100;
                    textBox1.Text = "1";
                    break;

                case "AverageFilter":
                    label1.Text = "平均濾波";
                    trackBar1.Minimum = 1;
                    trackBar1.Maximum = 10;
                    trackBar1.Value = 1;
                    textBox1.Text = "1";
                    break;

                case "CustomRotation":
                    label1.Text = "自訂旋轉角度";
                    trackBar1.Minimum = 0;
                    trackBar1.Maximum = 360;
                    trackBar1.Value = 0;
                    textBox1.Text = "0";
                    break;

                case "LocalMosaic":
                    label1.Text = "局部馬賽克";
                    trackBar1.Minimum = 1;
                    trackBar1.Maximum = 10;
                    trackBar1.Value = 3;
                    textBox1.Text = "3";
                    break;
            }
        }

        private void bmp_dip(Bitmap NpBitmap, PictureBox pictureBox1)
        {
            if (NpBitmap.Width < 256 || NpBitmap.Height < 256)
            {
                this.Width = 256 + label1.Width + trackBar1.Width + (this.Width - this.ClientRectangle.Width) + 50;
                this.Height = 256 + (this.Height - this.ClientRectangle.Height) + 100;
            }
            else
            {
                this.Width = NpBitmap.Width + label1.Width + trackBar1.Width + (this.Width - this.ClientRectangle.Width) + 50;
                this.Height = NpBitmap.Height + (this.Height - this.ClientRectangle.Height) + 100;
            }

            pictureBox1.Width = NpBitmap.Width;
            pictureBox1.Height = NpBitmap.Height;

            if (Select == "CustomRotation") // 自訂旋轉角度
            {
                if (NpBitmap.Width < 256 | NpBitmap.Height < 256)
                {
                    this.Width = (int)(256 * 1.414) + label1.Width + trackBar1.Width + (this.Width - this.ClientRectangle.Width) + 50;
                    this.Height = (int)(256 * 1.414) + (this.Height - this.ClientRectangle.Height) + 100;
                }
                else
                {
                    this.Width = (int)(Math.Max(NpBitmap.Width, NpBitmap.Height) * 1.414) + label1.Width + trackBar1.Width + (this.Width - this.ClientRectangle.Width) + 50;
                    this.Height = (int)(Math.Max(NpBitmap.Width, NpBitmap.Height) * 1.414) + (this.Height - this.ClientRectangle.Height) + 100;
                }

                pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
                pictureBox1.Width = (int)(Math.Max(NpBitmap.Width, NpBitmap.Height) * 1.414);
                pictureBox1.Height = (int)(Math.Max(NpBitmap.Width, NpBitmap.Height) * 1.414);
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            switch (Select)
            {
                case "Brightness": // 亮度
                {
                    textBox1.Text = trackBar1.Value.ToString();

                    int[] R, G, B;
                    ImageProcessUtils.BitmapToRGBArrays(NpBitmap, out R, out G, out B);

                    int[] processedR = new int[NpBitmap.Width * NpBitmap.Height];
                    int[] processedG = new int[NpBitmap.Width * NpBitmap.Height];
                    int[] processedB = new int[NpBitmap.Width * NpBitmap.Height];
                    unsafe
                    {
                        fixed (int* srcR = R, dstR = processedR)
                        fixed (int* srcG = G, dstG = processedG)
                        fixed (int* srcB = B, dstB = processedB)
                        {
                            // 進行亮度的調整
                            ImageProcessUtils.brightness(srcR, NpBitmap.Width, NpBitmap.Height, trackBar1.Value, dstR);
                            ImageProcessUtils.brightness(srcG, NpBitmap.Width, NpBitmap.Height, trackBar1.Value, dstG);
                            ImageProcessUtils.brightness(srcB, NpBitmap.Width, NpBitmap.Height, trackBar1.Value, dstB);
                        }
                    }

                    pBitmap = ImageProcessUtils.RGBArraysToBitmap(processedR, processedG, processedB, NpBitmap.Width, NpBitmap.Height);
                    break;
                }

                case "Contrast": // 對比度
                {
                    double c = (double)trackBar1.Value / 100;
                    textBox1.Text = ((double)trackBar1.Value / 100).ToString();

                    int[] R, G, B;
                    ImageProcessUtils.BitmapToRGBArrays(NpBitmap, out R, out G, out B);

                    int[] processedR = new int[NpBitmap.Width * NpBitmap.Height];
                    int[] processedG = new int[NpBitmap.Width * NpBitmap.Height];
                    int[] processedB = new int[NpBitmap.Width * NpBitmap.Height];
                    unsafe
                    {
                        fixed (int* srcR = R, dstR = processedR)
                        fixed (int* srcG = G, dstG = processedG)
                        fixed (int* srcB = B, dstB = processedB)
                        {
                            // 進行對比度的調整
                            ImageProcessUtils.contrast(srcR, NpBitmap.Width, NpBitmap.Height, c, dstR);
                            ImageProcessUtils.contrast(srcG, NpBitmap.Width, NpBitmap.Height, c, dstG);
                            ImageProcessUtils.contrast(srcB, NpBitmap.Width, NpBitmap.Height, c, dstB);
                        }
                    }

                    pBitmap = ImageProcessUtils.RGBArraysToBitmap(processedR, processedG, processedB, NpBitmap.Width, NpBitmap.Height);
                    break;
                }

                case "AverageFilter": // 平均濾波
                {
                    textBox1.Text = trackBar1.Value.ToString();

                    int[] R, G, B;
                    ImageProcessUtils.BitmapToRGBArrays(NpBitmap, out R, out G, out B);

                    int[] processedR = new int[NpBitmap.Width * NpBitmap.Height];
                    int[] processedG = new int[NpBitmap.Width * NpBitmap.Height];
                    int[] processedB = new int[NpBitmap.Width * NpBitmap.Height];
                    unsafe
                    {
                        fixed (int* srcR = R, dstR = processedR)
                        fixed (int* srcG = G, dstG = processedG)
                        fixed (int* srcB = B, dstB = processedB)
                        {
                            // 進行平均濾波的處理
                            ImageProcessUtils.averagefilter(srcR, NpBitmap.Width, NpBitmap.Height, trackBar1.Value, dstR);
                            ImageProcessUtils.averagefilter(srcG, NpBitmap.Width, NpBitmap.Height, trackBar1.Value, dstG);
                            ImageProcessUtils.averagefilter(srcB, NpBitmap.Width, NpBitmap.Height, trackBar1.Value, dstB);
                        }
                    }

                    pBitmap = ImageProcessUtils.RGBArraysToBitmap(processedR, processedG, processedB, NpBitmap.Width, NpBitmap.Height);
                    break;
                }

                case "CustomRotation": // 自訂旋轉角度
                {
                    textBox1.Text = trackBar1.Value.ToString();

                    Task.Run(() =>
                    {
                        Bitmap bitmapCopy = null;
                        int width = 0, height = 0;
                        int trackBarValue = 0;

                        pictureBox1.Invoke((MethodInvoker)delegate
                        {
                            bitmapCopy = new Bitmap(NpBitmap);
                            width = NpBitmap.Width;
                            height = NpBitmap.Height;
                            trackBarValue = trackBar1.Value;
                        });

                        double theta = (double)trackBarValue * Math.PI / 180;
                        int new_weight = (int)Math.Round(height * Math.Abs(Math.Sin(theta)) + width * Math.Abs(Math.Cos(theta)));
                        int new_height = (int)Math.Round(height * Math.Abs(Math.Cos(theta)) + width * Math.Abs(Math.Sin(theta)));

                        int[] R, G, B;
                        ImageProcessUtils.BitmapToRGBArrays(bitmapCopy, out R, out G, out B);

                        int[] processedR = new int[new_weight * new_height];
                        int[] processedG = new int[new_weight * new_height];
                        int[] processedB = new int[new_weight * new_height];
                        unsafe
                        {
                            fixed (int* srcR = R, dstR = processedR)
                            fixed (int* srcG = G, dstG = processedG)
                            fixed (int* srcB = B, dstB = processedB)
                            {
                                // 進行自訂旋轉角度的處理
                                ImageProcessUtils.customrotationangle(srcR, width, height, trackBarValue, dstR);
                                ImageProcessUtils.customrotationangle(srcG, width, height, trackBarValue, dstG);
                                ImageProcessUtils.customrotationangle(srcB, width, height, trackBarValue, dstB);
                            }
                        }

                        Bitmap rotatedBitmap = ImageProcessUtils.RGBArraysToBitmap(processedR, processedG, processedB, new_weight, new_height);

                        pictureBox1.Invoke((MethodInvoker)delegate
                        {
                            pBitmap = rotatedBitmap;
                            pictureBox1.Image = pBitmap;
                        });

                        bitmapCopy.Dispose();
                    });
                    break;
                }

                case "LocalMosaic": // 局部馬賽克
                {
                    textBox1.Text = trackBar1.Value.ToString();

                    int[] customlocation = new int[4];
                    customlocation[0] = selectionStart.X;
                    customlocation[1] = selectionStart.Y;
                    customlocation[2] = selectionEnd.X;
                    customlocation[3] = selectionEnd.Y;

                    int[] R, G, B;
                    ImageProcessUtils.BitmapToRGBArrays(NpBitmap, out R, out G, out B);
                    
                    int[] processedR = new int[NpBitmap.Width * NpBitmap.Height];
                    int[] processedG = new int[NpBitmap.Width * NpBitmap.Height];
                    int[] processedB = new int[NpBitmap.Width * NpBitmap.Height];
                    unsafe
                    {
                        fixed (int* srcR = R, dstR = processedR)
                        fixed (int* srcG = G, dstG = processedG)
                        fixed (int* srcB = B, dstB = processedB)
                        {
                            // 進行局部馬賽克的處理
                            ImageProcessUtils.mosaic(srcR, NpBitmap.Width, NpBitmap.Height, trackBar1.Value, customlocation, dstR);
                            ImageProcessUtils.mosaic(srcG, NpBitmap.Width, NpBitmap.Height, trackBar1.Value, customlocation, dstG);
                            ImageProcessUtils.mosaic(srcB, NpBitmap.Width, NpBitmap.Height, trackBar1.Value, customlocation, dstB);
                        }
                    }

                    pBitmap = ImageProcessUtils.RGBArraysToBitmap(processedR, processedG, processedB, NpBitmap.Width, NpBitmap.Height);
                    break;
                }
            }

            pictureBox1.Image = pBitmap;
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (Select == "LocalMosaic" && e.Button == MouseButtons.Left)  // 只有在局部馬賽克時才啟用框選
            {
                isSelecting = true;
                selectionStart = e.Location;  // 紀錄滑鼠按下的起始位置
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isSelecting)
            {
                // 更新矩形框的寬度和高度
                int width = e.X - selectionStart.X;
                int height = e.Y - selectionStart.Y;

                selectionRect = new Rectangle(selectionStart.X, selectionStart.Y, width, height);
                pictureBox1.Invalidate();  // 強制重繪
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (isSelecting)
            {
                isSelecting = false;
                // 紀錄框選结束的點
                selectionEnd = e.Location;

                trackBar1_Scroll(sender, e);

                // 清除矩形
                selectionRect = Rectangle.Empty;
                pictureBox1.Invalidate(); // 強制重繪
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (Select == "LocalMosaic" && selectionRect.Width > 0 && selectionRect.Height > 0)
            {
                // 繪製紅色的矩形框
                using (Pen pen = new Pen(Color.Red, 2))
                {
                    e.Graphics.DrawRectangle(pen, selectionRect);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MSForm msForm = this.MdiParent.MdiChildren.OfType<MSForm>().FirstOrDefault();
            if (msForm == null)
            {
                msForm = new MSForm();
                msForm.MdiParent = this.MdiParent;
                msForm.pf1 = this.pf1;
                msForm.pf2 = this.pf2;
                msForm.pf3 = this.pf3;
                
                // 創建新 Bitmap 副本並傳遞給 MSForm
                msForm.pBitmap = new Bitmap(pBitmap);
                msForm.AddImageTab(OriginalTabTitle, new Bitmap(pBitmap)); 
                msForm.Show();
            }
            else
            {
                // 創建新 Bitmap 副本並傳遞給 MSForm
                msForm.SetCurrentTabImage(new Bitmap(pBitmap));
                msForm.UpdateHistogram(new Bitmap(pBitmap));
            }

            this.Close();
        }
    }
}