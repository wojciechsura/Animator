#define NOMINMAX
#define _USE_MATH_DEFINES
#include <Windows.h>
#include <cstdio>
#include <algorithm>
#include <math.h>

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

					int alpha = copy[y1 * stride + x1 * BYTES_PER_PIXEL + ALPHA_OFFSET];

					rSum += (float)copy[y1 * stride + x1 * BYTES_PER_PIXEL + R_OFFSET] * alpha / 255.0f;
					gSum += (float)copy[y1 * stride + x1 * BYTES_PER_PIXEL + G_OFFSET] * alpha / 255.0f;
					bSum += (float)copy[y1 * stride + x1 * BYTES_PER_PIXEL + B_OFFSET] * alpha / 255.0f;
					aSum += alpha;

					count++;
				}

			float a = aSum / count;
			float r = 0;
			float g = 0;
			float b = 0;

			if (a > 0)
			{
				r = ((rSum / count) * 255 / a);
				g = ((gSum / count) * 255 / a);
				b = ((bSum / count) * 255 / a);
			}

			bitmapData[y * stride + x * BYTES_PER_PIXEL + R_OFFSET] = (unsigned char)r;
			bitmapData[y * stride + x * BYTES_PER_PIXEL + G_OFFSET] = (unsigned char)g;
			bitmapData[y * stride + x * BYTES_PER_PIXEL + B_OFFSET] = (unsigned char)b;
			bitmapData[y * stride + x * BYTES_PER_PIXEL + ALPHA_OFFSET] = (unsigned char)a;
		}

	delete[] copy;
}

extern "C" void __cdecl GaussianBlur(unsigned char* bitmapData, int stride, int width, int height, int radius)
{
	// Gaussian kernel

	float sigma = 1;
	float* kernel = new float[radius * radius];
	float mean = radius / 2.0f;
	float sum = 0.0; // For accumulating the kernel values
	for (int x = 0; x < radius; ++x)
		for (int y = 0; y < radius; ++y) {
			kernel[y * radius + x] = (float)(exp(-0.5 * (pow((x - mean) / sigma, 2.0) + pow((y - mean) / sigma, 2.0))) / (2 *  M_PI * sigma * sigma));

			// Accumulate the kernel values
			sum += kernel[y * radius + x];
		}

	// Normalize the kernel
	for (int x = 0; x < radius; ++x)
		for (int y = 0; y < radius; ++y)
			kernel[y * radius + x] /= sum;

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

			for (int x1 = std::max(0, x - radius / 2); x1 <= std::min(width - 1, x + radius / 2); x1++)
				for (int y1 = std::max(0, y - radius / 2); y1 <= std::min(height - 1, y + radius / 2); y1++)
				{
					// Find weight

					int kernelX = x1 - (x - radius / 2);
					int kernelY = y1 - (y - radius / 2);
					float kernelValue = kernel[kernelY * radius + kernelX];

					// Premultiply alpha

					int alpha = copy[y1 * stride + x1 * BYTES_PER_PIXEL + ALPHA_OFFSET];

					rSum += (float)(copy[y1 * stride + x1 * BYTES_PER_PIXEL + R_OFFSET] * alpha / 255.0f) * kernelValue;
					gSum += (float)(copy[y1 * stride + x1 * BYTES_PER_PIXEL + G_OFFSET] * alpha / 255.0f) * kernelValue;
					bSum += (float)(copy[y1 * stride + x1 * BYTES_PER_PIXEL + B_OFFSET] * alpha / 255.0f) * kernelValue;
					aSum += alpha * kernelValue;

					weight += kernelValue;
				}

			float a = aSum / weight;
			float r = 0;
			float g = 0;
			float b = 0;

			if (a > 0)
			{
				r = ((rSum / weight) * 255 / a);
				g = ((gSum / weight) * 255 / a);
				b = ((bSum / weight) * 255 / a);
			}

			bitmapData[y * stride + x * BYTES_PER_PIXEL + R_OFFSET] = (unsigned char)r;
			bitmapData[y * stride + x * BYTES_PER_PIXEL + G_OFFSET] = (unsigned char)g;
			bitmapData[y * stride + x * BYTES_PER_PIXEL + B_OFFSET] = (unsigned char)b;
			bitmapData[y * stride + x * BYTES_PER_PIXEL + ALPHA_OFFSET] = (unsigned char)a;
		}

	delete[] copy;
	delete[] kernel;
}