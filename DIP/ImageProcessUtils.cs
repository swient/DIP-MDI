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
        public static extern unsafe void negative(int* src, int* dst, int srcW, int srcH);

        [DllImport("B11217048.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void mosaic(int* src, int* dst, int srcW, int srcH, int size, int[] region);

        [DllImport("B11217048.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void equalization(int* src, int* dst, int srcW, int srcH);

        [DllImport("B11217048.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void otsuthreshold(int* src, int* dst, int srcW, int srcH, int* threshold);

        [DllImport("B11217048.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void connectedcomponent(int* src, int srcW, int srcH, int* labelCount);

        [DllImport("B11217048.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void highpassfilter(int* src, int* dst, int srcW, int srcH);

        [DllImport("B11217048.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void customrotationangle(int* src, int* dst, int srcW, int srcH, int angle);

        [DllImport("B11217048.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void zoom(int* src, int* dst, int srcW, int srcH, double scale);

        [DllImport("B11217048.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void brightness(int* src, int* dst, int srcW, int srcH, int value);

        [DllImport("B11217048.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void contrast(int* src, int* dst, int srcW, int srcH, double value);

        [DllImport("B11217048.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void bitplane(int* src, int* dst, int srcW, int srcH, int index);

        [DllImport("B11217048.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void fliphorizontal(int* src, int* dst, int srcW, int srcH);

        [DllImport("B11217048.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void flipvertical(int* src, int* dst, int srcW, int srcH);

        [DllImport("B11217048.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void averagefilter(int* src, int* dst, int srcW, int srcH, int size);

        [DllImport("B11217048.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void customfilter(int* src, int* dst, int srcW, int srcH, int divisor, int[] customKernel);

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
                        // 在處理完一行後，調整指標到下一行的起始位置
                        imgPtr += byteArray.Stride - Width * 1;
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

        // 將 Bitmap 圖像轉換為三個 RGB 陣列
        public static void BitmapToRGBArrays(Bitmap myBitmap, out int[] R, out int[] G, out int[] B)
        {
            if (myBitmap == null)
                throw new ArgumentNullException(nameof(myBitmap), "傳入的 Bitmap 不能為 null");

            R = new int[myBitmap.Width * myBitmap.Height];
            G = new int[myBitmap.Width * myBitmap.Height];
            B = new int[myBitmap.Width * myBitmap.Height];
            BitmapData byteArray = null;
            try
            {
                // 支援 24bppRgb, 32bppArgb, 32bppRgb
                PixelFormat pf = myBitmap.PixelFormat;
                if (pf != PixelFormat.Format24bppRgb && pf != PixelFormat.Format32bppArgb && pf != PixelFormat.Format32bppRgb)
                    throw new NotSupportedException("僅支援 24bppRgb, 32bppArgb, 32bppRgb 格式");

                byteArray = myBitmap.LockBits(new Rectangle(0, 0, myBitmap.Width, myBitmap.Height),
                                              ImageLockMode.ReadOnly, pf);
                int pixelSize = Image.GetPixelFormatSize(pf) / 8;
                unsafe
                {
                    byte* imgPtr = (byte*)byteArray.Scan0;
                    for (int y = 0; y < myBitmap.Height; y++)
                    {
                        for (int x = 0; x < myBitmap.Width; x++)
                        {
                            int index = x + myBitmap.Width * y;
                            B[index] = imgPtr[0];
                            G[index] = imgPtr[1];
                            R[index] = imgPtr[2];
                            imgPtr += pixelSize;
                        }
                        imgPtr += byteArray.Stride - myBitmap.Width * pixelSize;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("將 Bitmap 轉換為 RGB 陣列時發生錯誤", ex);
            }
            finally
            {
                if (byteArray != null) myBitmap.UnlockBits(byteArray);
            }
        }

        // 將三個 RGB 陣列轉換為 Bitmap 圖像
        public static Bitmap RGBArraysToBitmap(int[] R, int[] G, int[] B, int Width, int Height)
        {
            if (R == null || G == null || B == null)
                throw new ArgumentNullException("RGB 陣列不能為 null");
            if (R.Length != G.Length || R.Length != B.Length)
                throw new ArgumentException("RGB 陣列長度必須一致");
            if (R.Length < Width * Height)
                throw new ArgumentException("RGB 陣列長度不足");
            if (Width <= 0 || Height <= 0)
                throw new ArgumentException("寬高必須大於0");

            Bitmap myBitmap = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);
            BitmapData byteArray = null;
            try
            {
                byteArray = myBitmap.LockBits(new Rectangle(0, 0, Width, Height),
                                              ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
                unsafe
                {
                    byte* imgPtr = (byte*)byteArray.Scan0;
                    for (int y = 0; y < Height; y++)
                    {
                        for (int x = 0; x < Width; x++)
                        {
                            int index = x + Width * y;
                            imgPtr[0] = (byte)Math.Max(0, Math.Min(255, B[index]));
                            imgPtr[1] = (byte)Math.Max(0, Math.Min(255, G[index]));
                            imgPtr[2] = (byte)Math.Max(0, Math.Min(255, R[index]));
                            imgPtr += 3;
                        }
                        imgPtr += byteArray.Stride - Width * 3;
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

                throw new InvalidOperationException("將 RGB 陣列轉換為 Bitmap 時發生錯誤", ex);
            }
            finally
            {
                if (byteArray != null) myBitmap.UnlockBits(byteArray);
            }
            return myBitmap;
        }

        public static Bitmap ProcessBitmapChannels(
            Bitmap src,
            int dstW,
            int dstH,
            Action<IntPtr, IntPtr, int, int, object[]> processFunc,
            params object[] extraParams)
        {
            int srcW = src.Width;
            int srcH = src.Height;
            BitmapToRGBArrays(src, out int[] R, out int[] G, out int[] B);

            int[] processedR = new int[dstW * dstH];
            int[] processedG = new int[dstW * dstH];
            int[] processedB = new int[dstW * dstH];

            unsafe
            {
                fixed (int* srcR = R, dstR = processedR)
                fixed (int* srcG = G, dstG = processedG)
                fixed (int* srcB = B, dstB = processedB)
                {
                    processFunc((IntPtr)srcR, (IntPtr)dstR, srcW, srcH, extraParams);
                    processFunc((IntPtr)srcG, (IntPtr)dstG, srcW, srcH, extraParams);
                    processFunc((IntPtr)srcB, (IntPtr)dstB, srcW, srcH, extraParams);
                }
            }

            return RGBArraysToBitmap(processedR, processedG, processedB, dstW, dstH);
        }
    }
}