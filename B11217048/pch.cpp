// pch.cpp: 對應到先行編譯標頭的來源檔案

#include "pch.h"
#include <stdio.h>
#include <stdlib.h>
#include <cstring>
#include <math.h>
#include <cmath>

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

    __declspec(dllexport) void brightness(int* f, int w, int h, int s, int* g)
    {
        for (int i = 0; i < w * h; i++) {
            g[i] = f[i] + s;
            if (g[i] > 255) g[i] = 255;
            if (g[i] < 0) g[i] = 0;
        }
    }

    __declspec(dllexport) void contrast(int* f, int w, int h, double s, int* g)
    {
        for (int i = 0; i < w * h; i++) {
            g[i] = (int)((f[i] - 128) * s + 128);
            if (g[i] > 255) g[i] = 255;
            if (g[i] < 0) g[i] = 0;
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

    __declspec(dllexport) void averagefilter(int* f, int w, int h, int s, int* g)
    {
        int** matrix = Convert1DTo2D(f, w, h);
        int** tempMatrix = Convert1DTo2D(g, w, h);
        int start = (int)ceil(-s / 2) + 1;
        int end = (int)floor(s / 2);

        for (int y = 0; y < h; y++) {
            for (int x = 0; x < w; x++) {
                int sum = 0;
                int count = 0;

                for (int j = start; j <= end; j++) {
                    for (int i = start; i <= end; i++) {
                        if (x + i >= 0 && x + i < w && y + j >= 0 && y + j < h) {
                            sum += matrix[y + j][x + i];
                            count++;
                        }
                    }
                }
                tempMatrix[y][x] = (count > 0) ? sum / count : matrix[y][x];
            }
        }

        Convert2DTo1D(tempMatrix, g, w, h);

        for (int i = 0; i < h; i++) {
            free(matrix[i]);
			free(tempMatrix[i]);
        }
        free(matrix);
		free(tempMatrix);
    }

    __declspec(dllexport) void highpassfilter(int* f, int w, int h, int* g)
    {
        int** matrix = Convert1DTo2D(f, w, h);
        int** tempMatrix = Convert1DTo2D(g, w, h);
        const int kernel[3][3] = {
            { -1, 0, -1 },
            { 0, 4, 0 },
            { -1, 0, -1 }
        };

        int minVal = INT_MAX;
        int maxVal = INT_MIN;

        for (int y = 0; y < h; y++) {
            for (int x = 0; x < w; x++) {
                int sum = 0;

                for (int j = -1; j <= 1; j++) {
                    for (int i = -1; i <= 1; i++) {
                        int newX = x + i;
                        int newY = y + j;

                        if (newX >= 0 && newX < w && newY >= 0 && newY < h) {
                            sum += matrix[newY][newX] * kernel[j + 1][i + 1];
                        }
                    }
                }
                minVal = min(minVal, sum);
                maxVal = max(maxVal, sum);

                tempMatrix[y][x] = sum;
            }
        }

        for (int y = 0; y < h; y++) {
            for (int x = 0; x < w; x++) {
                int normalized = (tempMatrix[y][x] - minVal) * 255 / (maxVal - minVal);
                tempMatrix[y][x] = min(max(normalized, 0), 255);
            }
        }

        Convert2DTo1D(tempMatrix, g, w, h);

        for (int i = 0; i < h; i++) {
            free(matrix[i]);
			free(tempMatrix[i]);
        }
        free(matrix);
		free(tempMatrix);
    }

	__declspec(dllexport) void customfilter(int* f, int w, int h, int d, int* c, int* g)
	{
		int** matrix = Convert1DTo2D(f, w, h);
		int** tempMatrix = Convert1DTo2D(g, w, h);
		int kernel[3][3] = {
			{ c[0], c[1], c[2] },
			{ c[3], c[4], c[5] },
			{ c[6], c[7], c[8] }
		};

        for (int y = 0; y < h; y++) {
            for (int x = 0; x < w; x++) {
                int sum = 0;

                for (int j = -1; j <= 1; j++) {
                    for (int i = -1; i <= 1; i++) {
                        int newX = x + i;
                        int newY = y + j;

                        if (newX >= 0 && newX < w && newY >= 0 && newY < h) {
                            sum += matrix[newY][newX] * kernel[j + 1][i + 1];
                        }
                    }
                }
                tempMatrix[y][x] = min(max(sum / d, 0), 255);
            }
        }

        Convert2DTo1D(tempMatrix, g, w, h);

        for (int i = 0; i < h; i++) {
            free(matrix[i]);
			free(tempMatrix[i]);
        }
        free(matrix);
		free(tempMatrix);
	}

    __declspec(dllexport) void customrotationangle(int* f, int w, int h, int a, int* g)
    {
        const double M_PI = 3.14159265358979323846;
        double theta = a * M_PI / 180.0;

        int nw = (int)(h * std::abs(sin(theta)) + w * std::abs(cos(theta)));
        int nh = (int)(w * std::abs(sin(theta)) + h * std::abs(cos(theta)));

        for (int i = 0; i < nw * nh; i++)
        {
            g[i] = 255;
        }

        int** matrix = Convert1DTo2D(f, w, h);
        int** tempMatrix = Convert1DTo2D(g, nw, nh);

        int cx = w / 2, cy = h / 2;
        int cx_new = nw / 2, cy_new = nh / 2;

        for (int y = 0; y < nh; y++) {
            for (int x = 0; x < nw; x++) {
                int x_orig = (int)round(cos(-theta) * (x - cx_new) - sin(-theta) * (y - cy_new) + cx);
                int y_orig = (int)round(sin(-theta) * (x - cx_new) + cos(-theta) * (y - cy_new) + cy);

                if (x_orig >= 0 && x_orig < w && y_orig >= 0 && y_orig < h) {
                    tempMatrix[y][x] = matrix[y_orig][x_orig];
                }
            }
        }

        Convert2DTo1D(tempMatrix, g, nw, nh);

        for (int i = 0; i < h; i++) {
            free(matrix[i]);
        }
        free(matrix);

		for (int i = 0; i < nh; i++) {
			free(tempMatrix[i]);
		}
		free(tempMatrix);
    }
}