using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DIP
{
    public partial class MSForm : Form
    {
        internal Bitmap pBitmap;
        internal ToolStripStatusLabel pf1;
        public bool isHistogram = false;
        int w, h;

        public MSForm()
        {
            InitializeComponent();
        }

        private void MSForm_Load(object sender, EventArgs e)
        {
            bmp_dip(pBitmap, pictureBox1);
            pf1.Text = "(Width,Height)=(" + pBitmap.Width + "," + pBitmap.Height + ")";
            w = pBitmap.Width;
            h = pBitmap.Height;

            if (isHistogram)
            {
                int x = this.MdiParent.ClientSize.Width - this.Width - 5;
                int y = this.MdiParent.ClientSize.Height - this.Height - 50;
                // 設定直方圖為自動調整大小
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                // 設定視窗位置為右下角
                this.Location = new Point(x, y);
            }
        }

        private void bmp_dip(Bitmap pBitmap, PictureBox pictureBox1)
        {
            this.Width = pBitmap.Width + (this.Width - this.ClientRectangle.Width);
            this.Height = pBitmap.Height + (this.Height - this.ClientRectangle.Height);
            pictureBox1.Image = pBitmap;
            pictureBox1.Width = pBitmap.Width;
            pictureBox1.Height = pBitmap.Height;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.X >= 0 && e.X < pBitmap.Width && e.Y >= 0 && e.Y < pBitmap.Height)
            {
                Color pixel = pBitmap.GetPixel(e.X, e.Y);
                pf1.Text = "(" + e.X + "," + e.Y + ")" +
                            "=(" + pixel.R.ToString() + "," + pixel.G.ToString() + "," + pixel.B.ToString() + ")";
            }
        }
    }
}
