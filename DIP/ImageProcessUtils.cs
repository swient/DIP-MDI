using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using OxyPlot.WindowsForms;

namespace DIP
{
    public static class ImageProcessUtils
    {
        // 載入圖像處理 DLL 函式庫
        [DllImport("B11217048.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void negative(int* f0, int w, int h, int* g0);
        
        [DllImport("B11217048.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void mosaic(int* f0, int w, int h, int s, int[] c, int* g0);
        
        [DllImport("B11217048.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void equalization(int* f0, int w, int h, int* g0);
        
        [DllImport("B11217048.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void highpassfilter(int* f0, int w, int h, int* g0);
        
        [DllImport("B11217048.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void customrotationangle(int* f0, int w, int h, int s, int* g0);
        
        [DllImport("B11217048.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void zoom(int* f0, int w, int h, double s, int* g0);

        [DllImport("B11217048.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void brightness(int* f0, int w, int h, int s, int* g0);

        [DllImport("B11217048.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void contrast(int* f0, int w, int h, double s, int* g0);

        [DllImport("B11217048.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void averagefilter(int* f0, int w, int h, int s, int* g0);

        [DllImport("B11217048.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void customfilter(int* f0, int w, int h, int d, int[] c, int* g0);

        // 計算圖片的直方圖
        public static int[] CalculateHistogram(Bitmap bitmap)
        {
            int[] histogram = new int[256];

            var data = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);

            unsafe
            {
                byte* ptr = (byte*)data.Scan0;
                for (int y = 0; y < data.Height; y++)
                {
                    for (int x = 0; x < data.Width; x++)
                    {
                        int gray = (int)(0.299 * ptr[2] + 0.587 * ptr[1] + 0.114 * ptr[0]);
                        histogram[gray]++;
                        ptr += 4;
                    }
                    ptr += data.Stride - data.Width * 4;
                }
            }
            bitmap.UnlockBits(data);

            return histogram;
        }

        // 建立 OxyPlot 直方圖模型
        public static PlotModel CreateHistogramPlotModel(int[] histogram)
        {
            var plotModel = new PlotModel();

            var series = new HistogramSeries()
            {
                FillColor = OxyColors.SteelBlue,
                StrokeColor = OxyColors.Black,
                StrokeThickness = 1,
                TrackerFormatString = "灰階值: {5}\n數量: {7}"
            };

            for (int i = 0; i < histogram.Length; i++)
            {
                series.Items.Add(new HistogramItem(i, i + 1, histogram[i], 0));
            }

            plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Minimum = -1,
                Maximum = 257,
                MajorStep = 51
            });

            plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Minimum = 0
            });

            plotModel.Series.Add(series);

            return plotModel;
        }

        // 將 Bitmap 圖像轉換為整數陣列
        public static unsafe int[] BitmapToArray(Bitmap myBitmap)
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

        // 將整數陣列轉換為 Bitmap 圖像
        public static unsafe Bitmap ArrayToBitmap(int[] ImgData, int Width, int Height)
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