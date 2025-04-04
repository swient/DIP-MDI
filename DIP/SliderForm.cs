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
        public new int Select;
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
            pBitmap = NpBitmap;

            switch (Select)
            {
                case 0:
                    label1.Text = "亮度";
                    trackBar1.Minimum = -255;
                    trackBar1.Maximum = 255;
                    trackBar1.Value = 0;
                    textBox1.Text = "0";
                    break;

                case 1:
                    label1.Text = "對比度";
                    trackBar1.Minimum = 0;
                    trackBar1.Maximum = 300;
                    trackBar1.Value = 100;
                    textBox1.Text = "1";
                    break;

                case 2:
                    label1.Text = "平均濾波";
                    trackBar1.Minimum = 1;
                    trackBar1.Maximum = 10;
                    trackBar1.Value = 1;
                    textBox1.Text = "1";
                    break;

                case 3:
                    label1.Text = "自訂旋轉角度";
                    trackBar1.Minimum = 0;
                    trackBar1.Maximum = 360;
                    trackBar1.Value = 0;
                    textBox1.Text = "0";
                    break;

                case 4:
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
            this.Width = NpBitmap.Width + (this.Width - this.ClientRectangle.Width) * 30;
            this.Height = NpBitmap.Height + (this.Height - this.ClientRectangle.Height) + 100;
            pictureBox1.Image = NpBitmap;
            pictureBox1.Width = NpBitmap.Width;
            pictureBox1.Height = NpBitmap.Height;

            if (Select == 3) // 自訂旋轉角度
            {
                this.Width = (int)(Math.Max(NpBitmap.Width, NpBitmap.Height) * 1.414) + (this.Width - this.ClientRectangle.Width) * 30;
                this.Height = (int)(Math.Max(NpBitmap.Width, NpBitmap.Height) * 1.414) + (this.Height - this.ClientRectangle.Height) + 100;
                pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
                pictureBox1.Width = (int)(Math.Max(NpBitmap.Width, NpBitmap.Height) * 1.414);
                pictureBox1.Height = (int)(Math.Max(NpBitmap.Width, NpBitmap.Height) * 1.414);
            }

            // 調整 trackBar1 位置：將其放在圖片的右邊，視窗的底部
            trackBar1.Location = new Point(pictureBox1.Right + 60, this.ClientRectangle.Bottom - trackBar1.Height - 100);
            // 調整 textBox1 位置：將其放在 trackBar1 的上方，並且置中
            textBox1.Location = new Point(trackBar1.Left + (trackBar1.Width - textBox1.Width) / 2, trackBar1.Top - textBox1.Height - 20);
            // 調整 label1 位置：將其放在視窗的右上角
            label1.Location = new Point(this.ClientRectangle.Right - label1.Width - 60, 20);
            // 調整 button1 位置：將其放在視窗底部並且水平居中
            button1.Location = new Point((this.ClientRectangle.Width - button1.Width) / 2, this.ClientRectangle.Bottom - button1.Height - 30);
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            int[] f;
            int[] g;

            switch (Select)
            {
                case 0: // 亮度
                    textBox1.Text = trackBar1.Value.ToString();

                    f = ImageProcessUtils.BitmapToArray(NpBitmap);
                    g = new int[NpBitmap.Width * NpBitmap.Height];
                    unsafe
                    {
                        fixed (int* f0 = f) fixed (int* g0 = g)
                        {
                            ImageProcessUtils.brightness(f0, NpBitmap.Width, NpBitmap.Height, trackBar1.Value, g0);
                        }
                    }

                    pBitmap = ImageProcessUtils.ArrayToBitmap(g, NpBitmap.Width, NpBitmap.Height);
                    break;

                case 1: // 對比度
                    double c = (double)trackBar1.Value / 100;
                    textBox1.Text = ((double)trackBar1.Value / 100).ToString();

                    f = ImageProcessUtils.BitmapToArray(NpBitmap);
                    g = new int[NpBitmap.Width * NpBitmap.Height];
                    unsafe
                    {
                        fixed (int* f0 = f) fixed (int* g0 = g)
                        {
                            ImageProcessUtils.contrast(f0, NpBitmap.Width, NpBitmap.Height, c, g0);
                        }
                    }

                    pBitmap = ImageProcessUtils.ArrayToBitmap(g, NpBitmap.Width, NpBitmap.Height);
                    break;

                case 2: // 平均濾波
                    textBox1.Text = trackBar1.Value.ToString();

                    f = ImageProcessUtils.BitmapToArray(NpBitmap);
                    g = new int[NpBitmap.Width * NpBitmap.Height];
                    unsafe
                    {
                        fixed (int* f0 = f) fixed (int* g0 = g)
                        {
                            ImageProcessUtils.averagefilter(f0, NpBitmap.Width, NpBitmap.Height, trackBar1.Value, g0);
                        }
                    }

                    pBitmap = ImageProcessUtils.ArrayToBitmap(g, NpBitmap.Width, NpBitmap.Height);
                    break;

                case 3: // 自訂旋轉角度
                    textBox1.Text = trackBar1.Value.ToString();

                    double theta = (double)trackBar1.Value * Math.PI / 180;
                    int new_weight = (int)Math.Round(NpBitmap.Height * Math.Abs(Math.Sin(theta)) + NpBitmap.Width * Math.Abs(Math.Cos(theta)));
                    int new_height = (int)Math.Round(NpBitmap.Height * Math.Abs(Math.Cos(theta)) + NpBitmap.Width * Math.Abs(Math.Sin(theta)));

                    f = ImageProcessUtils.BitmapToArray(NpBitmap);
                    g = new int[new_weight * new_height];
                    unsafe
                    {
                        fixed (int* f0 = f) fixed (int* g0 = g)
                        {
                            ImageProcessUtils.customrotationangle(f0, NpBitmap.Width, NpBitmap.Height, trackBar1.Value, g0);
                        }
                    }

                    pBitmap = ImageProcessUtils.ArrayToBitmap(g, new_weight, new_height);
                    break;

                case 4: // 局部馬賽克
                    textBox1.Text = trackBar1.Value.ToString();

                    int[] customlocation = new int[4];
                    customlocation[0] = selectionStart.X;
                    customlocation[1] = selectionStart.Y;
                    customlocation[2] = selectionEnd.X;
                    customlocation[3] = selectionEnd.Y;

                    f = ImageProcessUtils.BitmapToArray(NpBitmap);
                    g = new int[NpBitmap.Width * NpBitmap.Height];
                    unsafe
                    {
                        fixed (int* f0 = f) fixed (int* g0 = g)
                        {
                            ImageProcessUtils.mosaic(f0, NpBitmap.Width, NpBitmap.Height, trackBar1.Value, customlocation, g0);
                        }
                    }

                    pBitmap = ImageProcessUtils.ArrayToBitmap(g, NpBitmap.Width, NpBitmap.Height);
                    break;
            }

            pictureBox1.Image = pBitmap;
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (Select == 4 && e.Button == MouseButtons.Left)  // 只有在局部馬賽克時才啟用框選
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
            if (Select == 4 && selectionRect.Width > 0 && selectionRect.Height > 0)
            {
                // 绘制红色的矩形框
                using (Pen pen = new Pen(Color.Red, 2))
                {
                    e.Graphics.DrawRectangle(pen, selectionRect);  // 绘制矩形框
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
                msForm.pBitmap = new Bitmap(pBitmap);
                msForm.AddImageTab("Slider", new Bitmap(pBitmap));
                msForm.Show();
            }

            msForm.SetCurrentTabImage(pBitmap);

            this.Close();
        }
    }
}