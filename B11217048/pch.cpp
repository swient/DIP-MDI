// pch.cpp: 對應到先行編譯標頭的來源檔案

#include "pch.h"
#include <stdio.h>
#include <stdlib.h>
#include <cstring>
#include <math.h>
#include <cmath>
#include <numeric>

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

    __declspec(dllexport) void negative(int* src, int* dst, int srcW, int srcH)
    {
        for (int y = 0; y < srcH; y++) {
            for (int x = 0; x < srcW; x++) {
				dst[y * srcW + x] = 255 - src[y * srcW + x];
            }
        }
    }

    __declspec(dllexport) void fliphorizontal(int* src, int* dst, int srcW, int srcH)
    {
        for (int y = 0; y < srcH; y++) {
            for (int x = 0; x < srcW; x++) {
                dst[y * srcW + x] = src[y * srcW + (srcW - 1 - x)];
            }
        }
    }

    __declspec(dllexport) void flipvertical(int* src, int* dst, int srcW, int srcH)
    {
        for (int y = 0; y < srcH; y++) {
            for (int x = 0; x < srcW; x++) {
                dst[y * srcW + x] = src[(srcH - 1 - y) * srcW + x];
            }
        }
    }
    
    __declspec(dllexport) void mosaic(int* src, int* dst, int srcW, int srcH, int size, int* region)
    {
        int x_start = region[0], x_end = region[2];
        int y_start = region[1], y_end = region[3];

        int block_size = size;

        // 複製原圖 f 到結果圖 g
        for (int i = 0; i < srcW * srcH; i++) {
            dst[i] = src[i];
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

                        if (nx < srcW && ny < srcH)
                        {
                            int pixel = src[ny * srcW + nx];
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

                        if (nx < srcW && ny < srcH)
                        {
                            dst[ny * srcW + nx] = avg;
                        }
                    }
                }
            }
        }
    }

    __declspec(dllexport) void brightness(int* src, int* dst, int srcW, int srcH, int value)
    {
        for (int i = 0; i < srcW * srcH; i++) {
            dst[i] = src[i] + value;
            if (dst[i] > 255) dst[i] = 255;
            if (dst[i] < 0) dst[i] = 0;
        }
    }

    __declspec(dllexport) void contrast(int* src, int* dst, int srcW, int srcH, double value)
    {
        for (int i = 0; i < srcW * srcH; i++) {
            dst[i] = (int)((src[i] - 128) * value + 128);
            if (dst[i] > 255) dst[i] = 255;
            if (dst[i] < 0) dst[i] = 0;
        }
    }

    __declspec(dllexport) void equalization(int* src, int* dst, int srcW, int srcH)
    {
        int* hist = (int*)calloc(256, sizeof(int));
        int* cdf = (int*)calloc(256, sizeof(int));

        // 檢查記憶體分配是否成功
        if (hist == NULL || cdf == NULL) {
            return;
        }

        // 計算原始圖像的直方圖
        for (int i = 0; i < srcW * srcH; i++) {
            if (src[i] >= 0 && src[i] < 256) {
                hist[src[i]]++;
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
        for (int i = 0; i < srcW * srcH; i++) {
			if (src[i] >= 0 && src[i] < 256) {
				dst[i] = cdf[src[i]];
			}
        }

        // 釋放記憶體
        free(hist);
        free(cdf);
    }

    __declspec(dllexport) void otsuthreshold(int* src, int* dst, int srcW, int srcH, int* threshold)
    {
       if (threshold == NULL) {
           return;  
       }

       int* hist = (int*)calloc(256, sizeof(int));

       if (hist == NULL) {
           return;
       }

       // 計算原始圖像的直方圖
       for (int i = 0; i < srcW * srcH; i++) {
           if (src[i] >= 0 && src[i] < 256) {
               hist[src[i]]++;
           }
       }

       // 計算灰階總和
       double total_intensity_sum = 0;
       for (int i = 0; i < 256; i++) {
           total_intensity_sum += (double)i * hist[i];
       }

       double max_variance = 0.0;
       *threshold = 0;

       long long w0 = 0; // 背景像素數
       double sum0 = 0.0; // 背景灰階和

       // 遍歷所有可能的閾值
       for (int t = 0; t < 256; t++) {
           w0 += hist[t]; // 累加背景像素數
           if (w0 == 0) continue; // 如果背景像素數為0，跳過

           long long w1 = srcW * srcH - w0; // 前景像素數
           if (w1 == 0) break; // 如果前景像素數為0，後續閾值無需計算

           sum0 += (double)t * hist[t]; // 累加背景灰階和
           double sum1 = total_intensity_sum - sum0; // 前景灰階和

           double mu0 = sum0 / w0; // 背景平均灰階
           double mu1 = sum1 / w1; // 前景平均灰階

           // 計算組間變異數
           double between_class_variance = (double)w0 * w1 * (mu1 - mu0) * (mu1 - mu0);

           // 找到最大變異數對應的閾值
           if (between_class_variance > max_variance) {
               max_variance = between_class_variance;
               *threshold = t;
           }
       }

       for (int i = 0; i < srcW * srcH; i++) {
           dst[i] = (src[i] > *threshold) ? 255 : 0;
       }

       free(hist);
    }

    __declspec(dllexport) void connectedcomponent(int* src, int srcW, int srcH, int* labelCount)
    {
        // Otsu 二值化
        int* binaryImage = (int*)malloc(srcW * srcH * sizeof(int));
        int threshold = 0;
        otsuthreshold(src, binaryImage, srcW, srcH, &threshold);

        // 兩遍法連通元件標記
        int* labelImage = (int*)calloc(srcW * srcH, sizeof(int));
        int* labelParent = (int*)calloc(srcW * srcH, sizeof(int));
        int nextLabelId = 1;

        for (int i = 0; i < srcW * srcH; i++) labelParent[i] = i;

        for (int y = 0; y < srcH; y++) {
            for (int x = 0; x < srcW; x++) {
                int idx = y * srcW + x;
                if (binaryImage[idx] == 0) continue;

                int leftLabel = (x > 0) ? labelImage[y * srcW + (x - 1)] : 0;
                int upLabel = (y > 0) ? labelImage[(y - 1) * srcW + x] : 0;

                if (leftLabel == 0 && upLabel == 0) {
                    labelImage[idx] = nextLabelId++;
                } else if (leftLabel != 0 && upLabel == 0) {
                    labelImage[idx] = leftLabel;
                } else if (leftLabel == 0 && upLabel != 0) {
                    labelImage[idx] = upLabel;
                } else {
                    labelImage[idx] = (leftLabel < upLabel) ? leftLabel : upLabel;
                    int a = leftLabel, b = upLabel;
                    while (labelParent[a] != a) a = labelParent[a];
                    while (labelParent[b] != b) b = labelParent[b];
                    if (a != b) labelParent[b] = a;
                }
            }
        }

        for (int i = 1; i < nextLabelId; i++) {
            int root = i;
            while (labelParent[root] != root) root = labelParent[root];
            labelParent[i] = root;
        }

        int maxLabelId = 0;
        for (int i = 0; i < srcW * srcH; i++) {
            if (labelImage[i] != 0) {
                int root = labelImage[i];
                while (labelParent[root] != root) root = labelParent[root];
                labelImage[i] = root;
                if (root > maxLabelId) maxLabelId = root;
            }
        }

        int* labelLUT = (int*)calloc(nextLabelId, sizeof(int));
        int newLabelId = 1;
        for (int i = 1; i <= maxLabelId; i++) {
            if (labelParent[i] == i) {
                labelLUT[i] = newLabelId++;
            }
        }
        for (int i = 0; i < srcW * srcH; i++) {
            if (labelImage[i] != 0) {
                labelImage[i] = labelLUT[labelImage[i]];
            }
        }

        if (labelCount) *labelCount = newLabelId - 1;

        free(binaryImage);
        free(labelImage);
        free(labelParent);
        free(labelLUT);
    }

    __declspec(dllexport) void averagefilter(int* src, int* dst, int srcW, int srcH, int size)
    {
        int** matrix = Convert1DTo2D(src, srcW, srcH);
        int** tempMatrix = Convert1DTo2D(dst, srcW, srcH);
        int start = (int)ceil(-size / 2) + 1;
        int end = (int)floor(size / 2);

        for (int y = 0; y < srcH; y++) {
            for (int x = 0; x < srcW; x++) {
                int sum = 0;
                int count = 0;

                for (int j = start; j <= end; j++) {
                    for (int i = start; i <= end; i++) {
                        if (x + i >= 0 && x + i < srcW && y + j >= 0 && y + j < srcH) {
                            sum += matrix[y + j][x + i];
                            count++;
                        }
                    }
                }
                tempMatrix[y][x] = (count > 0) ? sum / count : matrix[y][x];
            }
        }

        Convert2DTo1D(tempMatrix, dst, srcW, srcH);

        for (int i = 0; i < srcH; i++) {
            free(matrix[i]);
			free(tempMatrix[i]);
        }
        free(matrix);
		free(tempMatrix);
    }

    __declspec(dllexport) void highpassfilter(int* src, int* dst, int srcW, int srcH)
    {
        int** matrix = Convert1DTo2D(src, srcW, srcH);
        int** tempMatrix = Convert1DTo2D(dst, srcW, srcH);
        const int kernel[3][3] = {
            { -1, 0, -1 },
            { 0, 4, 0 },
            { -1, 0, -1 }
        };

        int minVal = INT_MAX;
        int maxVal = INT_MIN;

        for (int y = 0; y < srcH; y++) {
            for (int x = 0; x < srcW; x++) {
                int sum = 0;

                for (int j = -1; j <= 1; j++) {
                    for (int i = -1; i <= 1; i++) {
                        int newX = x + i;
                        int newY = y + j;

                        if (newX >= 0 && newX < srcW && newY >= 0 && newY < srcH) {
                            sum += matrix[newY][newX] * kernel[j + 1][i + 1];
                        }
                    }
                }
                minVal = min(minVal, sum);
                maxVal = max(maxVal, sum);

                tempMatrix[y][x] = sum;
            }
        }

        for (int y = 0; y < srcH; y++) {
            for (int x = 0; x < srcW; x++) {
                int normalized = (tempMatrix[y][x] - minVal) * 255 / (maxVal - minVal);
                tempMatrix[y][x] = min(max(normalized, 0), 255);
            }
        }

        Convert2DTo1D(tempMatrix, dst, srcW, srcH);

        for (int i = 0; i < srcH; i++) {
            free(matrix[i]);
			free(tempMatrix[i]);
        }
        free(matrix);
		free(tempMatrix);
    }

	__declspec(dllexport) void customfilter(int* src, int* dst, int srcW, int srcH, int divisor, int* customKernel)
	{
		int** matrix = Convert1DTo2D(src, srcW, srcH);
		int** tempMatrix = Convert1DTo2D(dst, srcW, srcH);
		int kernel[3][3] = {
			{ customKernel[0], customKernel[1], customKernel[2] },
			{ customKernel[3], customKernel[4], customKernel[5] },
			{ customKernel[6], customKernel[7], customKernel[8] }
		};

        for (int y = 0; y < srcH; y++) {
            for (int x = 0; x < srcW; x++) {
                int sum = 0;

                for (int j = -1; j <= 1; j++) {
                    for (int i = -1; i <= 1; i++) {
                        int newX = x + i;
                        int newY = y + j;

                        // 邊界處理
                        int clampedX = max(0, min(newX, srcW - 1));
                        int clampedY = max(0, min(newY, srcH - 1));

                        sum += matrix[clampedY][clampedX] * kernel[j + 1][i + 1];
                    }
                }
                tempMatrix[y][x] = min(max(sum / divisor, 0), 255);
            }
        }

        Convert2DTo1D(tempMatrix, dst, srcW, srcH);

        for (int i = 0; i < srcH; i++) {
            free(matrix[i]);
			free(tempMatrix[i]);
        }
        free(matrix);
		free(tempMatrix);
	}

    __declspec(dllexport) void customrotationangle(int* src, int* dst, int srcW, int srcH, int angle)
    {
        const double M_PI = 3.14159265358979323846;
        double theta = angle * M_PI / 180.0;

        int nw = (int)round(srcH * std::abs(sin(theta)) + srcW * std::abs(cos(theta)));
        int nh = (int)round(srcW * std::abs(sin(theta)) + srcH * std::abs(cos(theta)));

        for (int i = 0; i < nw * nh; i++)
        {
            dst[i] = 255;
        }

        int** matrix = Convert1DTo2D(src, srcW, srcH);
        int** tempMatrix = Convert1DTo2D(dst, nw, nh);

        double cx = (srcW - 1.0) / 2.0;
        double cy = (srcH - 1.0) / 2.0;
        double cx_new = (nw - 1.0) / 2.0;
        double cy_new = (nh - 1.0) / 2.0;

        for (int y = 0; y < nh; y++) {
            for (int x = 0; x < nw; x++) {
                int x_orig = (int)round(cos(-theta) * (x - cx_new) - sin(-theta) * (y - cy_new) + cx);
                int y_orig = (int)round(sin(-theta) * (x - cx_new) + cos(-theta) * (y - cy_new) + cy);

                if (x_orig >= 0 && x_orig < srcW && y_orig >= 0 && y_orig < srcH) {
                    tempMatrix[y][x] = matrix[y_orig][x_orig];
                }
            }
        }

        Convert2DTo1D(tempMatrix, dst, nw, nh);

        for (int i = 0; i < srcH; i++) {
            free(matrix[i]);
        }
        free(matrix);

		for (int i = 0; i < nh; i++) {
			free(tempMatrix[i]);
		}
		free(tempMatrix);
    }

	__declspec(dllexport) void bitplane(int* src, int* dst, int srcW, int srcH, int index)
	{
		for (int i = 0; i < srcW * srcH; i++) {
			if (((src[i] & 0xff) >> index) & 1) {
				dst[i] = 255;
			}
			else {
				dst[i] = 0;
			}
		}
	}

    __declspec(dllexport) void zoom(int* src, int* dst, int srcW, int srcH, double scale)
    {
        int nw = (int)round(srcW * scale);
        int nh = (int)round(srcH * scale);

        double scale_x = (double)(srcW - 1) / (nw - 1);
        double scale_y = (double)(srcH - 1) / (nh - 1);

        for (int j = 0; j < nh; j++)
        {
            for (int i = 0; i < nw; i++)
            {
                // 計算目標像素在原始影像中的浮點座標
                double ox = (i + 0.5) * scale_x - 0.5;
                double oy = (j + 0.5) * scale_y - 0.5;

                // 取得整數座標作為左上角參考點，並進行邊界約束
                int x1 = max(0, min((int)floor(ox), srcW - 1));
                int y1 = max(0, min((int)floor(oy), srcH - 1));

                // 計算右下角座標，並進行邊界約束
                int x2 = max(0, min(x1 + 1, srcW - 1));
                int y2 = max(0, min(y1 + 1, srcH - 1));

                // 計算小數部分 (距離)，並確保在 [0.0, 1.0) 範圍內
                double dx = max(0.0, min(ox - x1, 1.0 - 1e-9)); // 減去一個極小值確保 < 1.0
                double dy = max(0.0, min(oy - y1, 1.0 - 1e-9)); // 減去一個極小值確保 < 1.0

                // 取得四個鄰近像素的值
                int p1 = src[y1 * srcW + x1]; // 左上
                int p2 = src[y1 * srcW + x2]; // 右上
                int p3 = src[y2 * srcW + x1]; // 左下
                int p4 = src[y2 * srcW + x2]; // 右下

                // 進行雙線性內插
                double interpolated_value =
                    p1 * (1.0 - dx) * (1.0 - dy) +
                    p2 * dx * (1.0 - dy) +
                    p3 * (1.0 - dx) * dy +
                    p4 * dx * dy;

                dst[j * nw + i] = min(max((int)round(interpolated_value), 0), 255);
            }
        }
    }
}