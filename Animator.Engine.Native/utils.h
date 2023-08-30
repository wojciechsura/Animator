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

struct IntColor
{
public:
	unsigned int R;
	unsigned int G;
	unsigned int B;
	unsigned int A;

	IntColor();
	IntColor(unsigned char r, unsigned char g, unsigned char b, unsigned char a);
	IntColor(unsigned int colorArgb);
	IntColor(Color color);
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

inline IntColor getIntColor(unsigned char* bitmap, int stride, int x, int y)
{
	unsigned int colorArgb = *(unsigned int*)(bitmap + y * stride + x * BYTES_PER_PIXEL);

	return IntColor(((colorArgb & 0x00ff0000) >> 16), 
		((colorArgb & 0x0000ff00) >> 8),
		(colorArgb & 0x000000ff),
		((colorArgb & 0xff000000) >> 24));
}

inline void setAlpha(unsigned char* bitmap, int stride, int x, int y, float value)
{
	bitmap[y * stride + x * BYTES_PER_PIXEL + ALPHA_OFFSET] = (unsigned char)(value * 255.0f);
}

inline void setIntColor(unsigned char* bitmap, int stride, int x, int y, IntColor value) 
{
	unsigned int colorBgra = (value.A << 24) | (value.R << 16) | (value.G << 8) | (value.B);
	*(unsigned int*)(bitmap + y * stride + x * BYTES_PER_PIXEL) = colorBgra;
}

inline void setColor(unsigned char* bitmap, int stride, int x, int y, Color value)
{
	bitmap[y * stride + x * BYTES_PER_PIXEL + ALPHA_OFFSET] = (unsigned char)(value.A * 255.0f);
	bitmap[y * stride + x * BYTES_PER_PIXEL + R_OFFSET] = (unsigned char)(value.R);
	bitmap[y * stride + x * BYTES_PER_PIXEL + G_OFFSET] = (unsigned char)(value.G);
	bitmap[y * stride + x * BYTES_PER_PIXEL + B_OFFSET] = (unsigned char)(value.B);
}

inline void alphaBlend(IntColor& baseColor, IntColor targetColor)
{
	// Old algorithm (R, G, B are floats ranging from 0 to 255, A is a float ranging from 0 to 1)
	// 
	// float newAlpha = (1 - targetColor.A) * baseColor.A + targetColor.A;
	// baseColor.R = ((1 - targetColor.A) * baseColor.A * baseColor.R + targetColor.A * targetColor.R) / newAlpha;
	// baseColor.G = ((1 - targetColor.A) * baseColor.A * baseColor.G + targetColor.A * targetColor.G) / newAlpha;
	// baseColor.B = ((1 - targetColor.A) * baseColor.A * baseColor.B + targetColor.A * targetColor.B) / newAlpha;
	//
	// The code below represents converting the above to unsigned int RGBA ranging from 0 to 255.
	// Instances of A were replaced by (A / 255) and then formulas were transformed so that number
	// of divisions was minimized.
	//
	// Binary shift was introduced as a replacement for division/multiplying by 255 (that is *not* 
	// equivalent, but difference is acceptable)

	unsigned int a = (((baseColor.A + targetColor.A) << 8) - targetColor.A * baseColor.A) >> 8;

	if (a > 0)
	{
		unsigned int divisor = a << 8;

		unsigned int baseAR = baseColor.A * baseColor.R;
		baseColor.R = (((targetColor.A * targetColor.R + baseAR) << 8) - (baseAR * targetColor.A)) / divisor;

		unsigned int baseAG = baseColor.A * baseColor.G;
		baseColor.G = (((targetColor.A * targetColor.G + baseAG) << 8) - (baseAG * targetColor.A)) / divisor;

		unsigned int baseAB = baseColor.A * baseColor.B;
		baseColor.B = (((targetColor.A * targetColor.B + baseAB) << 8) - (baseAB * targetColor.A)) / divisor;

		baseColor.A = a;
	}
	else
	{
		baseColor.R = 0;
		baseColor.G = 0;
		baseColor.B = 0;
		baseColor.A = 0;
	}
}
