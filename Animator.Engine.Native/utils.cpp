#define _USE_MATH_DEFINES
#include "utils.h"
#include <math.h>

float* generateGaussKernel(int radius)
{
	float sigma = 1;
	float* kernel = new float[radius * radius];
	float mean = radius / 2.0f;
	float sum = 0.0; // For accumulating the kernel values
	for (int x = 0; x < radius; ++x)
		for (int y = 0; y < radius; ++y) {
			kernel[y * radius + x] = (float)(exp(-0.5 * (pow((x - mean) / sigma, 2.0) + pow((y - mean) / sigma, 2.0))) / (2 * M_PI * sigma * sigma));

			// Accumulate the kernel values
			sum += kernel[y * radius + x];
		}

	// Normalize the kernel
	for (int x = 0; x < radius; ++x)
		for (int y = 0; y < radius; ++y)
			kernel[y * radius + x] /= sum;

	return kernel;
}