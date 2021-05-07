#pragma once

#include <memory>

const int BYTES_PER_PIXEL = 4;
const int B_OFFSET = 0;
const int G_OFFSET = 1;
const int R_OFFSET = 2;
const int ALPHA_OFFSET = 3;

struct Color 
{
public:
	float R;
	float G;
	float B;
	float A;

	Color();
	Color(float r, float g, float b, float a);
	Color(int colorArgb);
};

std::shared_ptr<float[]> generateGaussKernel(int radius);

inline float getAlpha(unsigned char* bitmap, int stride, int x, int y)
{
	return bitmap[y * stride + x * BYTES_PER_PIXEL + ALPHA_OFFSET] / 255.0f;
}

inline Color getColor(unsigned char* bitmap, int stride, int x, int y)
{
	Color result;
	result.A = bitmap[y * stride + x * BYTES_PER_PIXEL + ALPHA_OFFSET] / 255.0f;
	result.R = bitmap[y * stride + x * BYTES_PER_PIXEL + R_OFFSET];
	result.G = bitmap[y * stride + x * BYTES_PER_PIXEL + G_OFFSET];
	result.B = bitmap[y * stride + x * BYTES_PER_PIXEL + B_OFFSET];

	return result;
}

inline void setAlpha(unsigned char* bitmap, int stride, int x, int y, float value)
{
	bitmap[y * stride + x * BYTES_PER_PIXEL + ALPHA_OFFSET] = (unsigned char)(value * 255.0f);
}

inline void setColor(unsigned char* bitmap, int stride, int x, int y, Color value)
{
	bitmap[y * stride + x * BYTES_PER_PIXEL + ALPHA_OFFSET] = (unsigned char)(value.A * 255.0f);
	bitmap[y * stride + x * BYTES_PER_PIXEL + R_OFFSET] = (unsigned char)(value.R);
	bitmap[y * stride + x * BYTES_PER_PIXEL + G_OFFSET] = (unsigned char)(value.G);
	bitmap[y * stride + x * BYTES_PER_PIXEL + B_OFFSET] = (unsigned char)(value.B);
}

inline void alphaBlend(Color& baseColor, Color targetColor)
{
	float newAlpha = (1 - targetColor.A) * baseColor.A + targetColor.A;

	baseColor.R = ((1 - targetColor.A) * baseColor.A * baseColor.R + targetColor.A * targetColor.R) / newAlpha;
	baseColor.G = ((1 - targetColor.A) * baseColor.A * baseColor.G + targetColor.A * targetColor.G) / newAlpha;
	baseColor.B = ((1 - targetColor.A) * baseColor.A * baseColor.B + targetColor.A * targetColor.B) / newAlpha;
	baseColor.A = newAlpha;
}
