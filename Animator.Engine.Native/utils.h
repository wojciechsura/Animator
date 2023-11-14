#pragma once

#include <memory>
#include <vector>
#include <algorithm>

const int BYTES_PER_PIXEL = 4;
const int B_OFFSET = 0;
const int G_OFFSET = 1;
const int R_OFFSET = 2;
const int ALPHA_OFFSET = 3;

struct FloatColor 
{
public:
    float R;
    float G;
    float B;
    float A;

    FloatColor();
    FloatColor(float r, float g, float b, float a);
    FloatColor(int colorArgb);
};

union IntColor
{
public:
    struct
    {
        unsigned char B;
        unsigned char G;
        unsigned char R;
        unsigned char A;
    };
    unsigned int ColorBgra;

    IntColor();
    IntColor(unsigned char r, unsigned char g, unsigned char b, unsigned char a);
    IntColor(unsigned int colorBgra);
};


std::shared_ptr<float[]> generateGaussKernel(int radius);

inline float getFloatAlpha(unsigned char* bitmap, int stride, int x, int y)
{
    return bitmap[y * stride + x * BYTES_PER_PIXEL + ALPHA_OFFSET] / 255.0f;
}

inline unsigned char getIntAlpha(unsigned char* bitmap, int stride, int x, int y)
{
    return bitmap[y * stride + x * BYTES_PER_PIXEL + ALPHA_OFFSET];
}

inline void findRoiByAlpha(unsigned char* bitmap, int stride, int width, int height, int& left, int& top, int& right, int& bottom)
{
    // Top

    int y = 0;
    while (y < height)
    {
        int x = 0;
        while (x < width && getIntAlpha(bitmap, stride, x, y) == 0)
            x++;

        if (x < width)
        {
            top = y;
            break;
        }

        y++;
    }

    if (y == height)
    {
        // Effectively the image is fully transparent
        // No ROI found.

        left = top = 0;
        right = bottom = -1;
        return;
    }

    // Here we know that there is at least one
    // non-transparent pixel, so we always fill find
    // other boundaries.

    // Bottom

    y = height - 1;
    while (y >= top)
    {
        int x = 0;
        while (x < width && getIntAlpha(bitmap, stride, x, y) == 0)
            x++;

        if (x < width)
        {
            bottom = y;
            break;
        }

        y--;
    }

#ifdef DEBUG
    if (y < top)
        throw std::exception("Error in findRoiByAlpha (bottom)!");
#endif

    // Left

    int x = 0;
    while (x < width)
    {
        int y = 0;
        while (y < height && getIntAlpha(bitmap, stride, x, y) == 0)
            y++;

        if (y < height)
        {
            left = x;
            break;
        }

        x++;
    }

#ifdef DEBUG
    if (x >= width)
        throw std::exception("Error in findRoiByAlpha (left)!");
#endif

    x = width - 1;
    while (x >= left)
    {
        int y = 0;
        while (y < height && getIntAlpha(bitmap, stride, x, y) == 0)
            y++;

        if (y < height)
        {
            right = x;
            break;
        }

        x--;
    }

#ifdef DEBUG
    if (x < left)
        throw std::exception("Error in findRoiByAlpha (right)!");
#endif
}

inline FloatColor getFloatColor(unsigned char* bitmap, int stride, int x, int y)
{
    FloatColor result;
    result.A = bitmap[y * stride + x * BYTES_PER_PIXEL + ALPHA_OFFSET] / 255.0f;
    result.R = bitmap[y * stride + x * BYTES_PER_PIXEL + R_OFFSET];
    result.G = bitmap[y * stride + x * BYTES_PER_PIXEL + G_OFFSET];
    result.B = bitmap[y * stride + x * BYTES_PER_PIXEL + B_OFFSET];

    return result;
}

inline IntColor getIntColor(unsigned char* bitmap, int stride, int x, int y)
{
    unsigned int colorBgra = *(unsigned int*)(bitmap + y * stride + x * BYTES_PER_PIXEL);
    return IntColor(colorBgra);
}

inline void setAlpha(unsigned char* bitmap, int stride, int x, int y, unsigned char value)
{
    bitmap[y * stride + x * BYTES_PER_PIXEL + ALPHA_OFFSET] = value;
}

inline void setIntColor(unsigned char* bitmap, int stride, int x, int y, IntColor value) 
{    
    bitmap[y * stride + x * BYTES_PER_PIXEL + ALPHA_OFFSET] = value.A;
    bitmap[y * stride + x * BYTES_PER_PIXEL + R_OFFSET] = value.R;
    bitmap[y * stride + x * BYTES_PER_PIXEL + G_OFFSET] = value.G;
    bitmap[y * stride + x * BYTES_PER_PIXEL + B_OFFSET] = value.B;
}

inline void setFloatColor(unsigned char* bitmap, int stride, int x, int y, FloatColor value)
{
    bitmap[y * stride + x * BYTES_PER_PIXEL + ALPHA_OFFSET] = (unsigned char)(value.A * 255.0f);
    bitmap[y * stride + x * BYTES_PER_PIXEL + R_OFFSET] = (unsigned char)(value.R);
    bitmap[y * stride + x * BYTES_PER_PIXEL + G_OFFSET] = (unsigned char)(value.G);
    bitmap[y * stride + x * BYTES_PER_PIXEL + B_OFFSET] = (unsigned char)(value.B);
}

inline void alphaBlend(FloatColor& baseColor, FloatColor targetColor)
{
    float newAlpha = (1 - targetColor.A) * baseColor.A + targetColor.A;
    if (newAlpha > 0.0f)
    {
        baseColor.R = ((1 - targetColor.A) * baseColor.A * baseColor.R + targetColor.A * targetColor.R) / newAlpha;
        baseColor.G = ((1 - targetColor.A) * baseColor.A * baseColor.G + targetColor.A * targetColor.G) / newAlpha;
        baseColor.B = ((1 - targetColor.A) * baseColor.A * baseColor.B + targetColor.A * targetColor.B) / newAlpha;
        baseColor.A = newAlpha;
    }
    else
    {
        baseColor.R = 0.0f;
        baseColor.G = 0.0f;
        baseColor.B = 0.0f;
        baseColor.A = 0.0f;
    }
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
    // of divisions was minimized and operations fit inside unsigned int.
    //
    // Binary shift was introduced as a replacement for division/multiplying by 255 (that is *not* 
    // equivalent, but difference is acceptable)

    unsigned int bA = baseColor.A;
    unsigned int bR = baseColor.R;
    unsigned int bG = baseColor.G;
    unsigned int bB = baseColor.B;

    unsigned int tA = targetColor.A;
    unsigned int tR = targetColor.R;
    unsigned int tG = targetColor.G;
    unsigned int tB = targetColor.B;

    unsigned int a = ((bA + tA) << 8) - tA * bA;

    if (a > 0)
    {
        unsigned int baseAR = bA * bR;
        baseColor.R = (((tA * tR + baseAR) << 8) - (baseAR * tA)) / a;

        unsigned int baseAG = bA * bG;
        baseColor.G = (((tA * tG + baseAG) << 8) - (baseAG * tA)) / a;

        unsigned int baseAB = bA * bB;
        baseColor.B = (((tA * tB + baseAB) << 8) - (baseAB * tA)) / a;

        a >>= 8;

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
