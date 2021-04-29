﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Tools
{
    public static class ImageProcessing
    {
        [DllImport("Animator.Engine.Native.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ApplyAlpha(IntPtr bitmapData,
            int stride,
            int width,
            int height,
            float alpha);

        [DllImport("Animator.Engine.Native.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ApplyMask(IntPtr bitmapData,
            int bitmapStride,
            IntPtr maskData,
            int maskStride,
            int width,
            int height,
            bool invertMask);

        [DllImport("Animator.Engine.Native.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Combine(IntPtr @base,
            int baseStride,
            IntPtr first,
            int firstStride,
            IntPtr second,
            int secondStride,
            int width,
            int height);

        [DllImport("Animator.Engine.Native.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void CombineWithMask(IntPtr @base,
            int baseStride,
            IntPtr mask,
            int maskStride,
            IntPtr target,
            int targetStride,
            int width,
            int height);

        [DllImport("Animator.Engine.Native.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void CopyData(IntPtr sourceData,
            int sourceStride,
            IntPtr destinationData,
            int destinationStride,
            int width,
            int height);

        [DllImport("Animator.Engine.Native.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Blur(IntPtr bitmapData,
            int stride,
            int width,
            int height,
            int radius);

        [DllImport("Animator.Engine.Native.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void GaussianBlur(IntPtr bitmapData,
            int stride,
            int width,
            int height,
            int radius);

        [DllImport("Animator.Engine.Native.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DropShadow(IntPtr frameData,
            int frameStride,
            IntPtr backData,
            int backStride,
            int width,
            int height,
            int colorArgb,
            int dx,
            int dy,
            int radius);
    }
}