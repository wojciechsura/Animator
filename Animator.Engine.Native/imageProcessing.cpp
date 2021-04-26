#include <Windows.h>
#include <cstdio>

const int BYTES_PER_PIXEL = 4;
const int ALPHA_OFFSET = 3;

extern "C" __declspec(dllexport) void __cdecl ApplyAlpha(unsigned char* bitmapData, int width, int height, int stride, float alpha)
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