// pch.cpp: 對應到先行編譯標頭的來源檔案

#include "pch.h"
#include <stdio.h>
#include <stdlib.h>
#include <cstring>
#include <math.h>

// 使用先行編譯的標頭時，需要來源檔案才能使編譯成功。

extern "C" {
    // 將二維陣列轉為一維陣列
    void Convert2DTo1D(int** array2D, int* array1D, int width, int height)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                array1D[y * width + x] = array2D[y][x];
            }
        }

		for (int i = 0; i < height; i++)
		{
			free(array2D[i]);
		}

		free(array2D);
    }

    // 將一維陣列轉為二維陣列
    int** Convert1DTo2D(int* array1D, int width, int height) {
        int** array2D = (int**)malloc(height * sizeof(int*));

        if (array2D == NULL) {
            printf("記憶體分配失敗\n");
            return NULL;
        }

        for (int i = 0; i < height; i++) {
            array2D[i] = (int*)malloc(width * sizeof(int));
            if (array2D[i] == NULL) {
                printf("記憶體分配失敗\n");
                for (int j = 0; j < i; j++) {
                    free(array2D[j]);
                }
                free(array2D);
                return NULL;
            }
        }

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                array2D[y][x] = array1D[y * width + x];
            }
        }

        return array2D;
    }

    __declspec(dllexport) void negative(int* f, int w, int h, int* g)
    {
        for (int y = 0; y < h; y++) {
            for (int x = 0; x < w; x++) {
				g[y * w + x] = 255 - f[y * w + x];
            }
        }
    }

    __declspec(dllexport) void mosaic(int* f, int w, int h, int* g)
    {
        int x_start = 100, x_end = 200;
        int y_start = 100, y_end = 200;

        int block_size = 4;

        // 複製原圖 f 到結果圖 g
        for (int i = 0; i < w * h; i++) {
            g[i] = f[i];
        }

        // 只處理範圍內的像素
        for (int y = y_start; y < y_end; y += block_size)
        {
            for (int x = x_start; x < x_end; x += block_size)
            {
                int sum = 0;
                int count = 0;

                for (int dy = 0; dy < block_size; dy++)
                {
                    for (int dx = 0; dx < block_size; dx++)
                    {
                        int nx = x + dx;
                        int ny = y + dy;

                        if (nx < w && ny < h)
                        {
                            int pixel = f[ny * w + nx];
                            sum += pixel;
                            count++;
                        }
                    }
                }

                int avg = (count > 0) ? sum / count : 0;

                // 用計算出的平均灰階值填充整個馬賽克塊
                for (int dy = 0; dy < block_size; dy++)
                {
                    for (int dx = 0; dx < block_size; dx++)
                    {
                        int nx = x + dx;
                        int ny = y + dy;

                        if (nx < w && ny < h)
                        {
                            g[ny * w + nx] = avg;
                        }
                    }
                }
            }
        }
    }

    __declspec(dllexport) void histogram(int* f, int w, int h, int* g)
    {
        int* hist = (int*)calloc(256, sizeof(int));

        if (hist == NULL) {
            return;
        }

        // 背景設為白色
        for (int i = 0; i < 256 * 256; i++) {
            g[i] = 255;
        }

        // 計算原始圖像的直方圖
        for (int i = 0; i < w * h; i++) {
            if (f[i] >= 0 && f[i] < 256) {
                hist[f[i]]++;
            }
        }

        // 計算最大頻率值
        int max_hist = 0;
        for (int i = 0; i < 256; i++) {
            if (hist[i] > max_hist) {
                max_hist = hist[i];
            }
        }

        // 計算每个灰階的高度，按比例缩放到 256 高度的畫布中
        for (int x = 0; x < 256; x++) {
            int bar_height = (int)((float)hist[x] / max_hist * 255);

            // 繪制直方圖的每一條柱狀圖
            for (int y = 0; y <= bar_height; y++) {
                g[(255 - y) * 256 + x] = 0;
            }
        }

        free(hist);
    }

    __declspec(dllexport) void equalization(int* f, int w, int h, int* g)
    {
        int* hist = (int*)calloc(256, sizeof(int));
        int* cdf = (int*)calloc(256, sizeof(int));

        // 檢查記憶體分配是否成功
        if (hist == NULL || cdf == NULL) {
            return;
        }

        // 計算原始圖像的直方圖
        for (int i = 0; i < w * h; i++) {
            if (f[i] >= 0 && f[i] < 256) {
                hist[f[i]]++;
            }
        }

        // 計算累積直方圖
        cdf[0] = hist[0];
        for (int i = 1; i < 256; i++) {
            cdf[i] = cdf[i - 1] + hist[i];
        }

        // 計算CDF的最大值
        int max_hist = cdf[255];
        // 規一化累積直方图
        for (int i = 0; i < 256; i++) {
            cdf[i] = (cdf[i] * 255) / max_hist;
        }

        // 使用CDF映射原始圖像像素到输出圖像
        for (int i = 0; i < w * h; i++) {
			if (f[i] >= 0 && f[i] < 256) {
				g[i] = cdf[f[i]];
			}
        }

        // 釋放記憶體
        free(hist);
        free(cdf);
    }
}