#define NOMINMAX
#define _USE_MATH_DEFINES
#include <Windows.h>
#include <cstdio>
#include <algorithm>
#include <math.h>
#include <memory>
#include "utils.h"

extern "C" void __cdecl ApplyAlpha(unsigned char* bitmapData, 
	int stride, 
	int width, 
	int height, 
	float alpha)
{
	if (alpha < 0.0f)
		alpha = 0.0f;

	if (alpha > 1.0f)
		alpha = 1.0f;

	for (int y = 0; y < height; y++)
		for (int x = 0; x < width; x++)
		{
			float oldAlpha = getAlpha(bitmapData, stride, x, y);
			float newAlpha = oldAlpha * alpha;
			setAlpha(bitmapData, stride, x, y, newAlpha);
		}
}

extern "C" void __cdecl ApplyMask(unsigned char* bitmapData,
	int bitmapStride,
	unsigned char* maskData,
	int maskStride,
	int width,
	int height,
	bool invertMask)
{
	for (int y = 0; y < height; y++)
		for (int x = 0; x < width; x++)
		{
			Color color = getColor(bitmapData, bitmapStride, x, y);
			Color maskColor = getColor(maskData, maskStride, x, y);

			if (invertMask)
				color.A *= (1.0f - maskColor.A);
			else
				color.A *= maskColor.A;

			setColor(bitmapData, bitmapStride, x, y, color);
		}
}

extern "C" void __cdecl CopyData(unsigned char* sourceData, 
	int sourceStride, 
	unsigned char* destinationData, 
	int destinationStride, 
	int width, 
	int height)
{
	for (int y = 0; y < height; y++)
	{
		unsigned char* sourceLine = sourceData + y * sourceStride;
		unsigned char* destinationLine = destinationData + y * destinationStride;

		memcpy(destinationLine, sourceLine, width * BYTES_PER_PIXEL);
	}
}

extern "C" void __cdecl Combine(unsigned char* base,
	int baseStride,
	unsigned char* first,
	int firstStride,
	unsigned char* second,
	int secondStride,
	int width,
	int height)
{
	for (int y = 0; y < height; y++)
		for (int x = 0; x < width; x++)
		{
			Color baseColor = getColor(base, baseStride, x, y);
			Color firstColor = getColor(first, firstStride, x, y);
			Color secondColor = getColor(second, secondStride, x, y);

			alphaBlend(baseColor, firstColor);
			alphaBlend(baseColor, secondColor);

			setColor(base, baseStride, x, y, baseColor);
		}
}

extern "C" void __cdecl CombineWithMask(unsigned char* base,
	int baseStride,
	unsigned char* mask,
	int maskStride,
	unsigned char* target,
	int targetStride,
	int width,
	int height)
{
	for (int y = 0; y < height; y++)
		for (int x = 0; x < height; x++)
		{
			Color sourceColor = getColor(base, baseStride, x, y);
			Color maskColor = getColor(mask, maskStride, x, y);

			sourceColor.A *= maskColor.A;

			Color targetColor = getColor(target, targetStride, x, y);
			alphaBlend(targetColor, sourceColor);
			setColor(target, targetStride, x, y, targetColor);
		}
}

extern "C" void __cdecl Blur(unsigned char* bitmapData, 
	int stride, 
	int width, 
	int height, 
	int radius)
{
	// TODO optimize

	auto copy = std::shared_ptr<unsigned char[]>(new unsigned char[height * stride]);

	for (int y = 0; y < height; y++)
		memcpy(copy.get() + y * stride, bitmapData + y * stride, width * BYTES_PER_PIXEL);

	for (int y = 0; y < height; y++)
		for (int x = 0; x < width; x++)
		{
			int count = 0;
			Color sum;

			for (int x1 = std::max(0, x - radius / 2); x1 <= std::min(width - 1, x + radius / 2); x1++)
				for (int y1 = std::max(0, y - radius / 2); y1 <= std::min(height - 1, y + radius / 2); y1++)
				{
					// Premultiply alpha

					Color color = getColor(copy.get(), stride, x1, y1);

					sum.R += color.R * color.A;
					sum.G += color.G * color.A;
					sum.B += color.B * color.A;
					sum.A += color.A;

					count++;
				}

			if (count > 0)
			{
				Color result;

				result.A = sum.A / count;
				if (result.A > 0)
				{
					result.R = ((sum.R / count) / result.A);
					result.G = ((sum.G / count) / result.A);
					result.B = ((sum.B / count) / result.A);
				}			

				setColor(bitmapData, stride, x, y, result);
			}
		}
}

extern "C" void __cdecl GaussianBlur(unsigned char* bitmapData, 
	int stride, 
	int width, 
	int height, 
	int radius)
{
	// Gaussian kernel

	std::shared_ptr<float[]> kernel = generateGaussKernel(radius);

	// Blur

	auto copy = std::shared_ptr<unsigned char[]>(new unsigned char[height * stride]);

	for (int y = 0; y < height; y++)
		memcpy(copy.get() + y * stride, bitmapData + y * stride, width * BYTES_PER_PIXEL);

	for (int y = 0; y < height; y++)
		for (int x = 0; x < width; x++)
		{
			Color sum;
			float weight = 0.0f;
			int count = 0;

			int xStart = x - radius / 2;
			int xEnd = x + radius / 2;
			int yStart = y - radius / 2;
			int yEnd = y + radius / 2;

			for (int x1 = std::max(0, xStart); x1 <= std::min(width - 1, xEnd); x1++)
				for (int y1 = std::max(0, yStart); y1 <= std::min(height - 1, yEnd); y1++)
				{
					// Find weight

					int kernelX = x1 - xStart;
					int kernelY = y1 - yStart;
					float kernelValue = kernel[kernelY * radius + kernelX];

					// Premultiply alpha

					Color color = getColor(copy.get(), stride, x1, y1);

					sum.R += (float)(color.R * color.A) * kernelValue;
					sum.G += (float)(color.G * color.A) * kernelValue;
					sum.B += (float)(color.B * color.A) * kernelValue;
					sum.A += color.A * kernelValue;

					weight += kernelValue;
					count++;
				}

			if (count > 0)
			{
				Color result;
				result.A = sum.A / weight;

				if (result.A > 0)
				{
					result.R = ((sum.R / weight) / result.A);
					result.G = ((sum.G / weight) / result.A);
					result.B = ((sum.B / weight) / result.A);
				}

				setColor(bitmapData, stride, x, y, result);
			}
		}
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

	std::shared_ptr<float []> kernel = generateGaussKernel(radius);

	// Drop shadow

	Color shadow(colorArgb);

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

					float alpha = getAlpha(frameData, frameStride, x1, y1) * shadow.A;

					aSum += alpha * kernelValue;
					weight += kernelValue;
					count++;
				}

			if (count > 0)
			{
				Color result;

				result.A = aSum / weight;
				if (result.A > 0)
				{
					result.R = shadow.R;
					result.G = shadow.G;
					result.B = shadow.B;
				}

				Color org = getColor(backData, backStride, x, y);
				alphaBlend(org, result);
				setColor(backData, backStride, x, y, org);
			}
		}
}