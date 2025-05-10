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
        internal ToolStripStatusLabel pf2;
        internal ToolStripStatusLabel pf3;
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
            if (NpBitmap.Width < 256 || NpBitmap.Height < 256)
            {
                this.Width = 256 * 2 + label1.Width + panel1.Width + (this.Width - this.ClientRectangle.Width) + 150;
                this.Height = 256 + (this.Height - this.ClientRectangle.Height) + 200;
            }
            else
            {
                this.Width = NpBitmap.Width * 2 + label1.Width + panel1.Width + (this.Width - this.ClientRectangle.Width) + 150;
                this.Height = NpBitmap.Height + (this.Height - this.ClientRectangle.Height) + 200;
            }

            pictureBox1.Width = NpBitmap.Width;
            pictureBox1.Height = NpBitmap.Height;
            pictureBox2.Width = NpBitmap.Width;
            pictureBox2.Height = NpBitmap.Height;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int divisor;
            int[] customKernel = new int[9];
            if (!int.TryParse(textBox1.Text, out customKernel[0]) ||
                !int.TryParse(textBox2.Text, out customKernel[1]) ||
                !int.TryParse(textBox3.Text, out customKernel[2]) ||
                !int.TryParse(textBox4.Text, out customKernel[3]) ||
                !int.TryParse(textBox5.Text, out customKernel[4]) ||
                !int.TryParse(textBox6.Text, out customKernel[5]) ||
                !int.TryParse(textBox7.Text, out customKernel[6]) ||
                !int.TryParse(textBox8.Text, out customKernel[7]) ||
                !int.TryParse(textBox9.Text, out customKernel[8]) ||
                !int.TryParse(textBox10.Text, out divisor))
            {
                MessageBox.Show("請輸入有效的數字！");
                return;
            }

            if (divisor == 0)
            {
                MessageBox.Show("除數不可為0");
                return;
            }

            // 使用泛型方法處理自定義濾波器
            Action<IntPtr, IntPtr, int, int, object[]> customFilterWrapper = (srcPtr, dstPtr, srcW, srcH, extra) =>
            {
                unsafe
                {
                    ImageProcessUtils.customfilter((int*)srcPtr, (int*)dstPtr, srcW, srcH, (int)extra[0], (int[])extra[1]);
                }
            };
            pBitmap = ImageProcessUtils.ProcessBitmapChannels(NpBitmap, NpBitmap.Width, NpBitmap.Height, customFilterWrapper, divisor, customKernel);

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
                msForm.pf2 = this.pf2;
                msForm.pf3 = this.pf3;
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