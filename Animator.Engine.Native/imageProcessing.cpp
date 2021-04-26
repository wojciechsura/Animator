#include <Windows.h>

const int BYTES_PER_PIXEL = 4;

extern "C" __declspec(dllexport) void __cdecl ApplyAlpha(char* bitmapData, int width, int height, int stride, float alpha)
{
	if (alpha < 0.0f)
		alpha = 0.0f;

	if (alpha > 1.0f)
		alpha = 1.0f;

	for (int y = 0; y < height; y++)
	{
		char* row = bitmapData + (y * stride) + 3;

		for (int x = 0; x < width; x++)
		{
			char newAlpha = (char)((((float)*row) / 255.0f) * alpha);

			*row = newAlpha;
			*row += BYTES_PER_PIXEL;
		}
	}
}