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
    unsigned int colorBgra = *(unsigned int*)(bitmap + y * stride + x * BYTES_PER_PIXEL);
    return IntColor(colorBgra);
}

inline void setAlpha(unsigned char* bitmap, int stride, int x, int y, float value)
{
    bitmap[y * stride + x * BYTES_PER_PIXEL + ALPHA_OFFSET] = (unsigned char)(value * 255.0f);
}

inline void setIntColor(unsigned char* bitmap, int stride, int x, int y, IntColor value) 
{    
    bitmap[y * stride + x * BYTES_PER_PIXEL + ALPHA_OFFSET] = value.A;
    bitmap[y * stride + x * BYTES_PER_PIXEL + R_OFFSET] = value.R;
    bitmap[y * stride + x * BYTES_PER_PIXEL + G_OFFSET] = value.G;
    bitmap[y * stride + x * BYTES_PER_PIXEL + B_OFFSET] = value.B;
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

    unsigned int a = (((bA + tA) << 8) - tA * bA) >> 8;
    
    if (a > 0)
    {
        unsigned int divisor = a << 8;
        
        unsigned int baseAR = bA * bR;
        baseColor.R = (((tA * tR + baseAR) << 8) - (baseAR * tA)) / divisor;
        
        unsigned int baseAG = bA * bG;
        baseColor.G = (((tA * tG + baseAG) << 8) - (baseAG * tA)) / divisor;
        
        unsigned int baseAB = bA * bB;
        baseColor.B = (((tA * tB + baseAB) << 8) - (baseAB * tA)) / divisor;
        
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
