#define _USE_MATH_DEFINES
#include "utils.h"
#include <math.h>
#include <memory>

FloatColor::FloatColor()
{
	R = G = B = A = 0;
}

FloatColor::FloatColor(float r, float g, float b, float a)
{
	R = r;
	G = g;
	B = b;
	A = a;
}

FloatColor::FloatColor(int colorArgb)
{
	A = ((colorArgb & 0xff000000) >> 24) / 255.0f;
	R = ((colorArgb & 0x00ff0000) >> 16) * 1.0f;
	G = ((colorArgb & 0x0000ff00) >> 8) * 1.0f;
	B = (colorArgb & 0x000000ff) * 1.0f;
}

IntColor::IntColor()
{
	R = G = B = A = 0;
}

IntColor::IntColor(unsigned char r, unsigned char g, unsigned char b, unsigned char a)
{
	R = r;
	G = g;
	B = b;
	A = a;
}

IntColor::IntColor(unsigned int colorArgb)
{
	A = ((colorArgb & 0xff000000) >> 24);
	R = ((colorArgb & 0x00ff0000) >> 16);
	G = ((colorArgb & 0x0000ff00) >> 8);
	B = (colorArgb & 0x000000ff);
}

std::shared_ptr<float[]> generateGaussKernel(int diameter)
{
	float sigma = diameter / 4.0f;
	std::shared_ptr<float[]> kernel(new float[diameter * diameter]);
	int mean = diameter / 2;
	float sum = 0.0; // For accumulating the kernel values
	for (int x = 0; x < diameter; ++x)
		for (int y = 0; y < diameter; ++y) {
			kernel[y * diameter + x] = (float)(exp(-0.5 * (pow((x - mean) / sigma, 2.0) + pow((y - mean) / sigma, 2.0))) / (2 * M_PI * sigma * sigma));

			// Accumulate the kernel values
			sum += kernel[y * diameter + x];
		}

	// Normalize the kernel
	for (int x = 0; x < diameter; ++x)
		for (int y = 0; y < diameter; ++y)
			kernel[y * diameter + x] /= sum;

	return kernel;
}
