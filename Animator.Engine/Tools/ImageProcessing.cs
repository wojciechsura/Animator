using System;
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
        public static extern void ApplyAlpha(IntPtr bitmapData, int width, int height, int stride, float alpha);
    }
}
