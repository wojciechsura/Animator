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

extern "C" void __cdecl Blur(unsigned char* bitmapData,
	int stride,
	int width,
	int height,
	int radius)
{
	// TODO optimize

	int diameter = 2 * radius + 1;

	auto copy = std::shared_ptr<unsigned char[]>(new unsigned char[height * stride]);

	for (int y = 0; y < height; y++)
		memcpy(copy.get() + y * stride, bitmapData + y * stride, width * BYTES_PER_PIXEL);

	for (int y = 0; y < height; y++)
		for (int x = 0; x < width; x++)
		{
			int count = 0;
			Color sum;

			for (int x1 = x - radius; x1 <= x + radius; x1++)
				for (int y1 = y - radius; y1 <= y + radius; y1++)
				{
					// Premultiply alpha
					Color color;
					if (x1 >= 0 && x1 < width && y1 >= 0 && y1 < height)
						color = getColor(copy.get(), stride, x1, y1);
					else
						color = Color(0);

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

extern "C" void __cdecl CombineThree(unsigned char* base,
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
			IntColor baseColor = getIntColor(base, baseStride, x, y);
			IntColor firstColor = getIntColor(first, firstStride, x, y);
			IntColor secondColor = getIntColor(second, secondStride, x, y);

			alphaBlend(baseColor, firstColor);
			alphaBlend(baseColor, secondColor);

			setIntColor(base, baseStride, x, y, baseColor);
		}
}

extern "C" void __cdecl CombineTwo(unsigned char* base,
	int baseStride,
	unsigned char* image,
	int imageStride,
	int width,
	int height)
{
	for (int y = 0; y < height; y++)
		for (int x = 0; x < width; x++)
		{
			IntColor baseColor = getIntColor(base, baseStride, x, y);
			IntColor imageColor = getIntColor(image, imageStride, x, y);

			alphaBlend(baseColor, imageColor);

			setIntColor(base, baseStride, x, y, baseColor);
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
			IntColor sourceColor = getIntColor(base, baseStride, x, y);
			IntColor maskColor = getIntColor(mask, maskStride, x, y);

			sourceColor.A = (unsigned char)(((int)sourceColor.A * maskColor.A) / 255);

			IntColor targetColor = getIntColor(target, targetStride, x, y);
			alphaBlend(targetColor, sourceColor);
			setIntColor(target, targetStride, x, y, targetColor);
		}
}

extern "C" void __cdecl DropShadow(unsigned char* frameData,
	int frameStride,
	unsigned char* backData,
	int backStride,
	int width,
	int height,
	int colorArgb,
	int dx,
	int dy,
	int radius)
{
	// Gaussian kernel

	int diameter = 2 * radius + 1;
	std::shared_ptr<float[]> kernel = generateGaussKernel(diameter);

	// Drop shadow

	Color shadow(colorArgb);

	for (int y = 0; y < height; y++)
		for (int x = 0; x < width; x++)
		{
			float aSum = 0;

			float weight = 0.0f;
			int count = 0;

			int xStart = x - radius - dx;
			int yStart = y - radius - dy;
			int xEnd = xStart + 2 * radius;
			int yEnd = yStart + 2 * radius;

			for (int x1 = xStart; x1 <= xEnd; x1++)
				for (int y1 = yStart; y1 <= yEnd; y1++)
				{
					float alpha;

					// Find weight
					if (x1 >= 0 && x1 < width && y1 >= 0 && y1 < height)
					{
						alpha = getAlpha(frameData, frameStride, x1, y1) * shadow.A;
					}
					else
					{
						alpha = 0.0f;					
					}

					int kernelX = x1 - xStart;
					int kernelY = y1 - yStart;
					float kernelValue = kernel[kernelY * diameter + kernelX];

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

				IntColor org = getIntColor(backData, backStride, x, y);
				alphaBlend(org, IntColor(result));
				setIntColor(backData, backStride, x, y, org);
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

	int diameter = 2 * radius + 1;
	std::shared_ptr<float[]> kernel = generateGaussKernel(diameter);

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

			int xStart = x - radius;
			int xEnd = x + radius;
			int yStart = y - radius;
			int yEnd = y + radius;

			for (int x1 = xStart; x1 <= xEnd; x1++)
				for (int y1 = yStart; y1 <= yEnd; y1++)
				{
					// Find weight

					int kernelX = x1 - xStart;
					int kernelY = y1 - yStart;
					float kernelValue = kernel[kernelY * diameter + kernelX];

					// Premultiply alpha

					Color color;
					if (x1 >= 0 && x1 < width && y1 >= 0 && y1 < height)
						color = getColor(copy.get(), stride, x1, y1);
					else
						color = Color(0);

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

extern "C" void __cdecl Scanlines(unsigned char* bitmapData,
	int stride,
	int width,
	int height,
	int lineHeight,
	int darkenLevel) {

	for (int y = 0; y < height; y++) {

		if ((y / lineHeight) % 2 == 0) {

			for (int x = 0; x < width; x++)
			{
				unsigned char a = bitmapData[y * stride + x * BYTES_PER_PIXEL + ALPHA_OFFSET];
				unsigned char r = bitmapData[y * stride + x * BYTES_PER_PIXEL + R_OFFSET];
				unsigned char g = bitmapData[y * stride + x * BYTES_PER_PIXEL + G_OFFSET];
				unsigned char b = bitmapData[y * stride + x * BYTES_PER_PIXEL + B_OFFSET];

				r = r >= darkenLevel ? r - (unsigned char)darkenLevel : 0;
				g = g >= darkenLevel ? g - (unsigned char)darkenLevel : 0;
				b = b >= darkenLevel ? b - (unsigned char)darkenLevel : 0;

				bitmapData[y * stride + x * BYTES_PER_PIXEL + ALPHA_OFFSET] = a;
				bitmapData[y * stride + x * BYTES_PER_PIXEL + R_OFFSET] = r;
				bitmapData[y * stride + x * BYTES_PER_PIXEL + G_OFFSET] = g;
				bitmapData[y * stride + x * BYTES_PER_PIXEL + B_OFFSET] = b;
			}
		}
	}
}