#define NOMINMAX
#define _USE_MATH_DEFINES
#include <Windows.h>
#include <cstdio>
#include <algorithm>
#include <math.h>
#include "utils.h"

const int BYTES_PER_PIXEL = 4;
const int B_OFFSET = 0;
const int G_OFFSET = 1;
const int R_OFFSET = 2;
const int ALPHA_OFFSET = 3;

extern "C" void __cdecl ApplyAlpha(unsigned char* bitmapData, int stride, int width, int height, float alpha)
{
	if (alpha < 0.0f)
		alpha = 0.0f;

	if (alpha > 1.0f)
		alpha = 1.0f;

	for (int y = 0; y < height; y++)
	{
		int index = y * stride + ALPHA_OFFSET;

		for (int x = 0; x < width; x++)
		{
			int oldAlpha = (int)bitmapData[index];
			int newAlpha = (int)((((float)oldAlpha / 255.5f) * alpha) * 255.0f);
			bitmapData[index] = (unsigned char)newAlpha;

			index += BYTES_PER_PIXEL;
		}
	}
}

extern "C" void __cdecl CopyData(unsigned char* sourceData, int sourceStride, unsigned char* destinationData, int destinationStride, int width, int height)
{
	for (int y = 0; y < height; y++)
	{
		unsigned char* sourceLine = sourceData + y * sourceStride;
		unsigned char* destinationLine = destinationData + y * destinationStride;

		memcpy(destinationLine, sourceLine, width * BYTES_PER_PIXEL);
	}
}

extern "C" void __cdecl Blur(unsigned char* bitmapData, int stride, int width, int height, int radius)
{
	// TODO optimize

	unsigned char* copy = new unsigned char[height * stride];

	for (int y = 0; y < height; y++)
		memcpy(copy + y * stride, bitmapData + y * stride, width * BYTES_PER_PIXEL);

	for (int y = 0; y < height; y++)
		for (int x = 0; x < width; x++)
		{
			float rSum = 0;
			float gSum = 0;
			float bSum = 0;
			float aSum = 0;

			int count = 0;

			for (int x1 = std::max(0, x - radius / 2); x1 <= std::min(width - 1, x + radius / 2); x1++)
				for (int y1 = std::max(0, y - radius / 2); y1 <= std::min(height - 1, y + radius / 2); y1++)
				{
					// Premultiply alpha

					float alpha = copy[y1 * stride + x1 * BYTES_PER_PIXEL + ALPHA_OFFSET] / 255.0f;

					rSum += (float)copy[y1 * stride + x1 * BYTES_PER_PIXEL + R_OFFSET] * alpha;
					gSum += (float)copy[y1 * stride + x1 * BYTES_PER_PIXEL + G_OFFSET] * alpha;
					bSum += (float)copy[y1 * stride + x1 * BYTES_PER_PIXEL + B_OFFSET] * alpha;
					aSum += alpha;

					count++;
				}

			if (count > 0)
			{
				float a = aSum / count;
				float r = 0;
				float g = 0;
				float b = 0;

				if (a > 0)
				{
					r = ((rSum / count) / a);
					g = ((gSum / count) / a);
					b = ((bSum / count) / a);
				}

				bitmapData[y * stride + x * BYTES_PER_PIXEL + R_OFFSET] = (unsigned char)r;
				bitmapData[y * stride + x * BYTES_PER_PIXEL + G_OFFSET] = (unsigned char)g;
				bitmapData[y * stride + x * BYTES_PER_PIXEL + B_OFFSET] = (unsigned char)b;
				bitmapData[y * stride + x * BYTES_PER_PIXEL + ALPHA_OFFSET] = (unsigned char)(a * 255.0f);
			}
		}

	delete[] copy;
}

extern "C" void __cdecl GaussianBlur(unsigned char* bitmapData, int stride, int width, int height, int radius)
{
	// Gaussian kernel

	float* kernel = generateGaussKernel(radius);

	// Blur

	unsigned char* copy = new unsigned char[height * stride];

	for (int y = 0; y < height; y++)
		memcpy(copy + y * stride, bitmapData + y * stride, width * BYTES_PER_PIXEL);

	for (int y = 0; y < height; y++)
		for (int x = 0; x < width; x++)
		{
			float rSum = 0;
			float gSum = 0;
			float bSum = 0;
			float aSum = 0;

			float weight = 0.0f;
			int count = 0;

			for (int x1 = std::max(0, x - radius / 2); x1 <= std::min(width - 1, x + radius / 2); x1++)
				for (int y1 = std::max(0, y - radius / 2); y1 <= std::min(height - 1, y + radius / 2); y1++)
				{
					// Find weight

					int kernelX = x1 - (x - radius / 2);
					int kernelY = y1 - (y - radius / 2);
					float kernelValue = kernel[kernelY * radius + kernelX];

					// Premultiply alpha

					float alpha = copy[y1 * stride + x1 * BYTES_PER_PIXEL + ALPHA_OFFSET] / 255.0f;

					rSum += (float)(copy[y1 * stride + x1 * BYTES_PER_PIXEL + R_OFFSET] * alpha) * kernelValue;
					gSum += (float)(copy[y1 * stride + x1 * BYTES_PER_PIXEL + G_OFFSET] * alpha) * kernelValue;
					bSum += (float)(copy[y1 * stride + x1 * BYTES_PER_PIXEL + B_OFFSET] * alpha) * kernelValue;
					aSum += alpha * kernelValue;

					weight += kernelValue;
					count++;
				}

			if (count > 0)
			{
				float a = aSum / weight;
				float r = 0;
				float g = 0;
				float b = 0;

				if (a > 0)
				{
					r = ((rSum / weight) / a);
					g = ((gSum / weight) / a);
					b = ((bSum / weight) / a);
				}

				bitmapData[y * stride + x * BYTES_PER_PIXEL + R_OFFSET] = (unsigned char)r;
				bitmapData[y * stride + x * BYTES_PER_PIXEL + G_OFFSET] = (unsigned char)g;
				bitmapData[y * stride + x * BYTES_PER_PIXEL + B_OFFSET] = (unsigned char)b;
				bitmapData[y * stride + x * BYTES_PER_PIXEL + ALPHA_OFFSET] = (unsigned char)(a * 255.0f);
			}
		}

	delete[] copy;
	delete[] kernel;
}

extern "C" void __cdecl DropShadow(unsigned char* frameData, 
	int frameStride, 
	unsigned char * backData, 
	int backStride, 
	int width,
	int height, 
	int colorArgb,
	int dx,
	int dy, 
	int radius)
{
	// Gaussian kernel

	float* kernel = generateGaussKernel(radius);

	// Drop shadow

	float shadowA = ((colorArgb & 0xff000000) >> 24) / 255.0f;
	float shadowR = ((colorArgb & 0x00ff0000) >> 16) / 255.0f;
	float shadowG = ((colorArgb & 0x0000ff00) >> 8) / 255.0f;
	float shadowB = (colorArgb & 0x000000ff) / 255.0f;

	for (int y = 0; y < height; y++)
		for (int x = 0; x < width; x++)
		{
			float aSum = 0;

			float weight = 0.0f;
			int count = 0;

			int xStart = x - radius / 2 - dx;
			int yStart = y - radius / 2 - dy;
			int xEnd = xStart + radius - 1;
			int yEnd = yStart + radius - 1;

			for (int x1 = std::max(0, xStart); x1 <= std::min(width - 1, xEnd); x1++)
				for (int y1 = std::max(0, yStart); y1 <= std::min(height - 1, yEnd); y1++)
				{
					// Find weight

					int kernelX = x1 - xStart;
					int kernelY = y1 - yStart;
					float kernelValue = kernel[kernelY * radius + kernelX];

					float alpha = (frameData[y1 * frameStride + x1 * BYTES_PER_PIXEL + ALPHA_OFFSET] / 255.0f) * shadowA;

					aSum += alpha * kernelValue;

					weight += kernelValue;
					count++;
				}

			if (count > 0)
			{
				float a = aSum / weight;
				float r = 0;
				float g = 0;
				float b = 0;

				if (a > 0)
				{
					r = shadowR;
					g = shadowG;
					b = shadowB;
				}

				// Combine with specific pixel in backdata

				float orgA = backData[y * backStride + x * BYTES_PER_PIXEL + ALPHA_OFFSET] / 255.0f;
				float orgR = backData[y * backStride + x * BYTES_PER_PIXEL + R_OFFSET] / 255.0f;
				float orgG = backData[y * backStride + x * BYTES_PER_PIXEL + G_OFFSET] / 255.0f;
				float orgB = backData[y * backStride + x * BYTES_PER_PIXEL + B_OFFSET] / 255.0f;

				float targetAlpha = (1 - a) * orgA + a;
				float targetR = ((1 - a) * orgA * orgR + a * r) / targetAlpha;
				float targetG = ((1 - a) * orgA * orgR + a * g) / targetAlpha;
				float targetB = ((1 - a) * orgA * orgR + a * b) / targetAlpha;

				backData[y * backStride + x * BYTES_PER_PIXEL + ALPHA_OFFSET] = (unsigned char)(targetAlpha * 255.0f);
				backData[y * backStride + x * BYTES_PER_PIXEL + R_OFFSET] = (unsigned char)(targetR * 255.0f);
				backData[y * backStride + x * BYTES_PER_PIXEL + G_OFFSET] = (unsigned char)(targetG * 255.0f);
				backData[y * backStride + x * BYTES_PER_PIXEL + B_OFFSET] = (unsigned char)(targetB * 255.0f);
			}
		}

	delete[] kernel;
}