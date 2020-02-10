namespace AForge
{
    using System;
    using System.Runtime.InteropServices;

    public static class SystemTools
    {
        public static unsafe IntPtr CopyUnmanagedMemory(IntPtr dst, IntPtr src, int count)
        {
            CopyUnmanagedMemory((byte*) dst.ToPointer(), (byte*) src.ToPointer(), count);
            return dst;
        }

        public static unsafe byte* CopyUnmanagedMemory(byte* dst, byte* src, int count)
        {
            return memcpy(dst, src, count);
        }

        [DllImport("ntdll.dll")]
        private static extern unsafe byte* memcpy(byte* dst, byte* src, int count);
        [DllImport("ntdll.dll")]
        private static extern unsafe byte* memset(byte* dst, int filler, int count);
        public static unsafe byte* SetUnmanagedMemory(byte* dst, int filler, int count)
        {
            return memset(dst, filler, count);
        }

        public static unsafe IntPtr SetUnmanagedMemory(IntPtr dst, int filler, int count)
        {
            SetUnmanagedMemory((byte*) dst.ToPointer(), filler, count);
            return dst;
        }
    }
}

