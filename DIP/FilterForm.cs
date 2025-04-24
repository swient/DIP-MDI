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
        internal string OriginalTabTitle;
        public new int Select;

        public FilterForm()
        {
            InitializeComponent();
        }

        private void FilterForm_Load(object sender, EventArgs e)
        {
            bmp_dip(NpBitmap, pictureBox1);
            // 創建 pBitmap 的副本，避免共用引用
            pBitmap = new Bitmap(NpBitmap);
            // 設置初始圖像
            pictureBox1.Image = NpBitmap;
            pictureBox2.Image = pBitmap;
        }

        private void bmp_dip(Bitmap NpBitmap, PictureBox pictureBox1)
        {
            this.Width = NpBitmap.Width * 2 + (this.Width - this.ClientRectangle.Width) + 250 + 180;
            this.Height = NpBitmap.Height + (this.Height - this.ClientRectangle.Height) + 100;
            
            // 調整 pictureBox 的大小而不設置圖像
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
            // 調整 flowLayoutPanel1 位置：將其放在視窗底部並且水平放在 button1 與 button2 之間
            flowLayoutPanel1.Location = new Point((button1.Left + button2.Left + button2.Width - flowLayoutPanel1.Width) / 2, this.ClientRectangle.Bottom - button2.Height - 50);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int[] f;
            int[] g;
            int divide;
            int[] customize = new int[9];
            if (!int.TryParse(textBox1.Text, out customize[0]) ||
                !int.TryParse(textBox2.Text, out customize[1]) ||
                !int.TryParse(textBox3.Text, out customize[2]) ||
                !int.TryParse(textBox4.Text, out customize[3]) ||
                !int.TryParse(textBox5.Text, out customize[4]) ||
                !int.TryParse(textBox6.Text, out customize[5]) ||
                !int.TryParse(textBox7.Text, out customize[6]) ||
                !int.TryParse(textBox8.Text, out customize[7]) ||
                !int.TryParse(textBox9.Text, out customize[8]) ||
                !int.TryParse(textBox10.Text, out divide))
            {
                MessageBox.Show("請輸入有效的數字！");
                return;
            }

            if (divide == 0)
            {
                MessageBox.Show("除數不可為0");
                return;
            }

            f = ImageProcessUtils.BitmapToArray(NpBitmap);
            g = new int[NpBitmap.Width * NpBitmap.Height];
            unsafe
            {
                fixed (int* f0 = f) fixed (int* g0 = g)
                {
                    ImageProcessUtils.customfilter(f0, NpBitmap.Width, NpBitmap.Height, divide, customize, g0);
                }
            }

            pBitmap = ImageProcessUtils.ArrayToBitmap(g, NpBitmap.Width, NpBitmap.Height);

            pictureBox2.Image = pBitmap;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MSForm msForm = this.MdiParent.MdiChildren.OfType<MSForm>().FirstOrDefault();
            if (msForm == null)
            {
                msForm = new MSForm();
                msForm.MdiParent = this.MdiParent;
                msForm.pf1 = this.pf1;
                msForm.pBitmap = new Bitmap(pBitmap);
                msForm.AddImageTab(OriginalTabTitle, new Bitmap(pBitmap));
                msForm.Show();
            }
            else
            {
                msForm.SetCurrentTabImage(pBitmap);
                msForm.UpdateHistogram(pBitmap);
            }

            this.Close();
        }

        private void SetKernelValues(int[] kernel, int divisor)
        {
            if (kernel == null || kernel.Length != 9)
            {
                return;
            }

            textBox1.Text = kernel[0].ToString();
            textBox2.Text = kernel[1].ToString();
            textBox3.Text = kernel[2].ToString();
            textBox4.Text = kernel[3].ToString();
            textBox5.Text = kernel[4].ToString();
            textBox6.Text = kernel[5].ToString();
            textBox7.Text = kernel[6].ToString();
            textBox8.Text = kernel[7].ToString();
            textBox9.Text = kernel[8].ToString();
            textBox10.Text = divisor.ToString();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                // 高斯模糊
                int[] kernel = { 1, 2, 1, 2, 4, 2, 1, 2, 1 };
                int divisor = 16;
                SetKernelValues(kernel, divisor);
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                // 浮雕效果
                int[] kernel = { -2, -1, 0, -1, 1, 1, 0, 1, 2 };
                int divisor = 1;
                SetKernelValues(kernel, divisor);
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
            {
                // 銳化
                int[] kernel = { 0, -1, 0, -1, 5, -1, 0, -1, 0 };
                int divisor = 1;
                SetKernelValues(kernel, divisor);
            }
        }
    }
}