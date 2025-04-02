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

        public SliderForm()
        {
            InitializeComponent();
        }

        [DllImport("B11217048.dll", CallingConvention = CallingConvention.Cdecl)]
        unsafe public static extern void brightness(int* f0, int w, int h, int s, int* g0);

        [DllImport("B11217048.dll", CallingConvention = CallingConvention.Cdecl)]
        unsafe public static extern void contrast(int* f0, int w, int h, double s, int* g0);

        [DllImport("B11217048.dll", CallingConvention = CallingConvention.Cdecl)]
        unsafe public static extern void averagefilter(int* f0, int w, int h, int s, int* g0);

        [DllImport("B11217048.dll", CallingConvention = CallingConvention.Cdecl)]
        unsafe public static extern void customrotationangle(int* f0, int w, int h, int a, int* g0);

        private void SliderForm_Load(object sender, EventArgs e)
        {
            bmp_dip(NpBitmap, pictureBox1);
            pBitmap = NpBitmap;

            if (Select == 0)
            {
                label1.Text = "亮度";
                // 設置最小值
                trackBar1.Minimum = -255;
                // 設置最大值
                trackBar1.Maximum = 255;
                // 設置初始值
                trackBar1.Value = 0;
                textBox1.Text = trackBar1.Value.ToString();
                
            }
            else if (Select == 1)
            {
                label1.Text = "對比度";
                // 設置最小值
                trackBar1.Minimum = 0;
                // 設置最大值
                trackBar1.Maximum = 300;
                // 設置初始值
                trackBar1.Value = 100;
                textBox1.Text = (trackBar1.Value/100).ToString();
            }
            else if (Select == 2)
            {
                label1.Text = "平均濾波";
                // 設置最小值
                trackBar1.Minimum = 1;
                // 設置最大值
                trackBar1.Maximum = 10;
                // 設置初始值
                trackBar1.Value = 1;
                textBox1.Text = trackBar1.Value.ToString();
            }
            else if(Select == 3)
            {
                label1.Text = "自訂旋轉角度";
                // 設置最小值
                trackBar1.Minimum = 0;
                // 設置最大值
                trackBar1.Maximum = 360;
                // 設置初始值
                trackBar1.Value = 0;
                textBox1.Text = trackBar1.Value.ToString();
            }
        }

        private void bmp_dip(Bitmap NpBitmap, PictureBox pictureBox1)
        {
            this.Width = NpBitmap.Width + (this.Width - this.ClientRectangle.Width) * 30;
            this.Height = NpBitmap.Height + (this.Height - this.ClientRectangle.Height) + 100;
            pictureBox1.Image = NpBitmap;
            pictureBox1.Width = NpBitmap.Width;
            pictureBox1.Height = NpBitmap.Height;

            if (Select == 3)
            {
                this.Width = (int)(NpBitmap.Width * 1.414) + (this.Width - this.ClientRectangle.Width) * 30;
                this.Height = (int)(NpBitmap.Height * 1.414) + (this.Height - this.ClientRectangle.Height) + 100;
                pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
                pictureBox1.Width = (int)(NpBitmap.Width * 1.414);
                pictureBox1.Height = (int)(NpBitmap.Height * 1.414);
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

            if (Select == 0) // 亮度
            {
                textBox1.Text = trackBar1.Value.ToString();

                f = bmp2array(NpBitmap);
                g = new int[NpBitmap.Width * NpBitmap.Height];
                unsafe
                {
                    fixed (int* f0 = f) fixed (int* g0 = g)
                    {
                        brightness(f0, NpBitmap.Width, NpBitmap.Height, trackBar1.Value, g0);
                    }
                }

                pBitmap = array2bmp(g, NpBitmap.Width, NpBitmap.Height);
            }
            else if (Select == 1) // 對比度
            {
                double c = (double)trackBar1.Value / 100;
                textBox1.Text = c.ToString();

                f = bmp2array(NpBitmap);
                g = new int[NpBitmap.Width * NpBitmap.Height];
                unsafe
                {
                    fixed (int* f0 = f) fixed (int* g0 = g)
                    {
                        contrast(f0, NpBitmap.Width, NpBitmap.Height, c, g0);
                    }
                }

                pBitmap = array2bmp(g, NpBitmap.Width, NpBitmap.Height);
            }
            else if (Select == 2) // 平均濾波
            {
                textBox1.Text = trackBar1.Value.ToString();

                f = bmp2array(NpBitmap);
                g = new int[NpBitmap.Width * NpBitmap.Height];
                unsafe
                {
                    fixed (int* f0 = f) fixed (int* g0 = g)
                    {
                        averagefilter(f0, NpBitmap.Width, NpBitmap.Height, trackBar1.Value, g0);
                    }
                }

                pBitmap = array2bmp(g, NpBitmap.Width, NpBitmap.Height);
            }
            else if (Select == 3) // 自訂旋轉角度
            {
                textBox1.Text = trackBar1.Value.ToString();

                f = bmp2array(NpBitmap);
                double theta;
                theta = (double)trackBar1.Value * Math.PI / 180;
                int new_weight = (int)(NpBitmap.Height * Math.Abs(Math.Sin(theta)) + NpBitmap.Width * Math.Abs(Math.Cos(theta)));
                int new_height = (int)(NpBitmap.Height * Math.Abs(Math.Cos(theta)) + NpBitmap.Width * Math.Abs(Math.Sin(theta)));
                g = new int[new_weight * new_height];
                unsafe
                {
                    fixed (int* f0 = f) fixed (int* g0 = g)
                    {
                        
                        customrotationangle(f0, NpBitmap.Width, NpBitmap.Height, trackBar1.Value, g0);
                    }
                }

                pBitmap = array2bmp(g, new_weight, new_height);
            }

            pictureBox1.Image = pBitmap;
        }

        private int[] bmp2array(Bitmap myBitmap)
        {
            int[] ImgData = new int[myBitmap.Width * myBitmap.Height];
            BitmapData byteArray = myBitmap.LockBits(new Rectangle(0, 0, myBitmap.Width, myBitmap.Height),
                                                    ImageLockMode.ReadWrite,
                                                    myBitmap.PixelFormat);
            int ByteOfSkip = byteArray.Stride - byteArray.Width * (int)(byteArray.Stride / myBitmap.Width);
            unsafe
            {
                byte* imgPtr = (byte*)(byteArray.Scan0);
                for (int y = 0; y < byteArray.Height; y++)
                {
                    for (int x = 0; x < byteArray.Width; x++)
                    {
                        ImgData[x + byteArray.Width * y] = (int)*(imgPtr);
                        imgPtr += (int)(byteArray.Stride / myBitmap.Width);
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
            int ByteOfSkip = byteArray.Stride - myBitmap.Width * 3;
            unsafe
            {                                   // 指標取出影像資料
                byte* imgPtr = (byte*)byteArray.Scan0;
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        int index = x + Width * y;
                        *imgPtr = (byte)ImgData[index];       //B
                        *(imgPtr + 1) = (byte)ImgData[index]; //G
                        *(imgPtr + 2) = (byte)ImgData[index]; //R
                        imgPtr += 3;
                    }
                    imgPtr += ByteOfSkip; // 跳過Padding bytes
                }
            }
            myBitmap.UnlockBits(byteArray);
            return myBitmap;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MSForm msForm = new MSForm();
            msForm.MdiParent = this.MdiParent;
            msForm.pf1 = this.pf1;
            msForm.pBitmap = pBitmap;
            msForm.Show();
            this.Close();
        }
    }
}