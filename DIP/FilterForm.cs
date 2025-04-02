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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;

namespace DIP
{
    public partial class FilterForm : Form
    {
        internal Bitmap NpBitmap;
        internal Bitmap pBitmap;
        internal ToolStripStatusLabel pf1;
        public new int Select;

        public FilterForm()
        {
            InitializeComponent();
        }

        [DllImport("B11217048.dll", CallingConvention = CallingConvention.Cdecl)]
        unsafe public static extern void customfilter(int* f0, int w, int h, int d, int[] c, int* g0);

        private void FilterForm_Load(object sender, EventArgs e)
        {
            bmp_dip(NpBitmap, pictureBox1);
            pBitmap = NpBitmap;
        }

        private void bmp_dip(Bitmap NpBitmap, PictureBox pictureBox1)
        {
            this.Width = NpBitmap.Width * 2 + (this.Width - this.ClientRectangle.Width) + 250 + 180;
            this.Height = NpBitmap.Height + (this.Height - this.ClientRectangle.Height) + 100;
            pictureBox1.Image = NpBitmap;
            pictureBox2.Image = NpBitmap;
            pictureBox1.Width = NpBitmap.Width;
            pictureBox1.Height = NpBitmap.Height;
            pictureBox2.Width = NpBitmap.Width;
            pictureBox2.Height = NpBitmap.Height;
            // 調整 pictureBox2 的位置：將其放在視窗的右上角
            pictureBox2.Location = new Point(pictureBox1.Right + 250, 0);
            // 調整 textBox 位置：將其放在圖片的右邊，視窗的上方
            textBox1.Location = new Point(pictureBox1.Right + 75, 60);
            textBox2.Location = new Point(pictureBox1.Right + 110, 60);
            textBox3.Location = new Point(pictureBox1.Right + 145, 60);
            textBox4.Location = new Point(pictureBox1.Right + 75, 85);
            textBox5.Location = new Point(pictureBox1.Right + 110, 85);
            textBox6.Location = new Point(pictureBox1.Right + 145, 85);
            textBox7.Location = new Point(pictureBox1.Right + 75, 110);
            textBox8.Location = new Point(pictureBox1.Right + 110, 110);
            textBox9.Location = new Point(pictureBox1.Right + 145, 110);
            // 調整 label1 位置：將其放在pictureBox2 和視窗右邊框的中間，視窗的右上角
            label1.Location = new Point((pictureBox2.Right + this.ClientRectangle.Width - label1.Width) / 2, 20);
            // 調整 label2 位置：將其放在 pictureBox1 的右邊，並且置中
            label2.Location = new Point(pictureBox1.Right + 85, 150);
            // 調整 textBox10 位置：將其放在 label2 的右邊，並且置中
            textBox10.Location = new Point(pictureBox1.Right + label2.Width + 85 + 5, 150);
            // 調整 button1 位置：將其放在視窗底部並且水平居中
            button1.Location = new Point((pictureBox1.Left + pictureBox1.Right - button1.Width) / 2, this.ClientRectangle.Bottom - button1.Height - 30);
            // 調整 button2 位置：將其放在視窗底部並且水平居中
            button2.Location = new Point((pictureBox2.Left + pictureBox2.Right - button2.Width) / 2, this.ClientRectangle.Bottom - button2.Height - 30);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int[] f;
            int[] g;
            int divide = textBox10.Text == "" ? 9 : int.Parse(textBox10.Text);
            int[] customize = new int[9];
            customize[0] = textBox1.Text == "" ? 1 : int.Parse(textBox1.Text);
            customize[1] = textBox2.Text == "" ? 1 : int.Parse(textBox2.Text);
            customize[2] = textBox3.Text == "" ? 1 : int.Parse(textBox3.Text);
            customize[3] = textBox4.Text == "" ? 1 : int.Parse(textBox4.Text);
            customize[4] = textBox5.Text == "" ? 1 : int.Parse(textBox5.Text);
            customize[5] = textBox6.Text == "" ? 1 : int.Parse(textBox6.Text);
            customize[6] = textBox7.Text == "" ? 1 : int.Parse(textBox7.Text);
            customize[7] = textBox8.Text == "" ? 1 : int.Parse(textBox8.Text);
            customize[8] = textBox9.Text == "" ? 1 : int.Parse(textBox9.Text);

            if (divide == 0)
            {
                MessageBox.Show("除數不可為0");
                return;
            }

            f = bmp2array(NpBitmap);
            g = new int[NpBitmap.Width * NpBitmap.Height];
            unsafe
            {
                fixed (int* f0 = f) fixed (int* g0 = g)
                {
                    customfilter(f0, NpBitmap.Width, NpBitmap.Height, divide, customize, g0);
                }
            }

            pBitmap = array2bmp(g, NpBitmap.Width, NpBitmap.Height);

            pictureBox2.Image = pBitmap;
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

        private void button2_Click(object sender, EventArgs e)
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
