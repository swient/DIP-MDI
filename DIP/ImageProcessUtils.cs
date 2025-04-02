using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

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
        public static extern unsafe void histogram(int* f0, int w, int h, int* g0);
        
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