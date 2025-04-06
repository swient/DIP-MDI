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
            if (bitmap == null)
                throw new ArgumentNullException(nameof(bitmap), "傳入的 Bitmap 不能為 null");
                
            int[] histogram = new int[256];
            BitmapData data = null;

            try
            {
                data = bitmap.LockBits(
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
                            int gray = (int)Math.Round(0.299 * ptr[2] + 0.587 * ptr[1] + 0.114 * ptr[0]);
                            histogram[gray]++;
                            ptr += 4;
                        }
                        ptr += data.Stride - data.Width * 4;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("計算直方圖時發生錯誤", ex);
            }
            finally
            {
                if (data != null)
                {
                    bitmap.UnlockBits(data);
                }
            }

            return histogram;
        }

        // 建立 OxyPlot 直方圖模型
        public static PlotModel CreateHistogramPlotModel(int[] histogram)
        {
            if (histogram == null)
                throw new ArgumentNullException(nameof(histogram), "直方圖數據不能為 null");
                
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
            if (myBitmap == null)
                throw new ArgumentNullException(nameof(myBitmap), "傳入的 Bitmap 不能為 null");
                
            int[] ImgData = new int[myBitmap.Width * myBitmap.Height];
            BitmapData byteArray = null;

            try
            {
                byteArray = myBitmap.LockBits(new Rectangle(0, 0, myBitmap.Width, myBitmap.Height),
                                                    ImageLockMode.ReadOnly,
                                                    myBitmap.PixelFormat);
                // 判斷圖像格式
                bool is8bpp = (myBitmap.PixelFormat == PixelFormat.Format8bppIndexed);
                int pixelSize = Image.GetPixelFormatSize(myBitmap.PixelFormat) / 8;
                unsafe
                {
                    byte* imgPtr = (byte*)(byteArray.Scan0);

                    for (int y = 0; y < byteArray.Height; y++)
                    {
                        for (int x = 0; x < byteArray.Width; x++)
                        {
                            int index = x + byteArray.Width * y;

                            if (is8bpp)
                            {
                                ImgData[index] = (int)*(imgPtr);
                            }
                            else
                            {
                                byte blue = imgPtr[0];
                                byte green = imgPtr[1];
                                byte red = imgPtr[2];

                                // 計算灰階值
                                int grayValue = (int)Math.Round(0.299 * red + 0.587 * green + 0.114 * blue);
                                ImgData[index] = grayValue;
                            }
                            imgPtr += pixelSize;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("將 Bitmap 轉換為陣列時發生錯誤", ex);
            }
            finally
            {
                if (byteArray != null)
                {
                    myBitmap.UnlockBits(byteArray);
                }
            }

            return ImgData;
        }

        // 將整數陣列轉換為 Bitmap 圖像
        public static unsafe Bitmap ArrayToBitmap(int[] ImgData, int Width, int Height)
        {
            if (ImgData == null)
                throw new ArgumentNullException(nameof(ImgData), "圖像數據不能為 null");
            if (Width <= 0 || Height <= 0)
                throw new ArgumentException("圖像寬度和高度必須大於0");
            if (ImgData.Length < Width * Height)
                throw new ArgumentException("圖像數據長度不足");
                
            Bitmap myBitmap = new Bitmap(Width, Height, PixelFormat.Format8bppIndexed);

            // 設定灰階調色板
            ColorPalette palette = myBitmap.Palette;
            for (int i = 0; i < 256; i++)
            {
                palette.Entries[i] = Color.FromArgb(i, i, i);
            }
            myBitmap.Palette = palette;

            BitmapData byteArray = null;

            try
            {
                byteArray = myBitmap.LockBits(new Rectangle(0, 0, Width, Height),
                                           ImageLockMode.WriteOnly,
                                           PixelFormat.Format8bppIndexed);

                unsafe
                {
                    byte* imgPtr = (byte*)byteArray.Scan0;

                    for (int y = 0; y < Height; y++)
                    {
                        for (int x = 0; x < Width; x++)
                        {
                            int index = x + Width * y;
                            // 確保灰階值在有效範圍內
                            int value = ImgData[index];
                            byte grayValue = (byte)Math.Max(0, Math.Min(255, value));

                            // 設定ARGB值
                            imgPtr[0] = grayValue;

                            imgPtr += 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // 處理異常，釋放位圖資源
                if (myBitmap != null)
                {
                    myBitmap.Dispose();
                }
                
                throw new InvalidOperationException("將陣列轉換為 Bitmap 時發生錯誤", ex);
            }
            finally
            {
                if (byteArray != null)
                {
                    myBitmap.UnlockBits(byteArray);
                }
            }

            return myBitmap;
        }
    }
} 