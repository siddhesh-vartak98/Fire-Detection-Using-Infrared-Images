namespace AForge.Imaging
{
    using AForge;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public static class Drawing
    {
        private static void CheckEndPoint(int width, int height, IntPoint start, ref IntPoint end)
        {
            if (end.X >= width)
            {
                int num = width - 1;
                double num2 = ((double) (num - start.X)) / ((double) (end.X - start.X));
                end.Y = start.Y + ((int) (num2 * (end.Y - start.Y)));
                end.X = num;
            }
            if (end.Y >= height)
            {
                int num3 = height - 1;
                double num4 = ((double) (num3 - start.Y)) / ((double) (end.Y - start.Y));
                end.X = start.X + ((int) (num4 * (end.X - start.X)));
                end.Y = num3;
            }
            if (end.X < 0)
            {
                double num5 = ((double) -start.X) / ((double) (end.X - start.X));
                end.Y = start.Y + ((int) (num5 * (end.Y - start.Y)));
                end.X = 0;
            }
            if (end.Y < 0)
            {
                double num6 = ((double) -start.Y) / ((double) (end.Y - start.Y));
                end.X = start.X + ((int) (num6 * (end.X - start.X)));
                end.Y = 0;
            }
        }

        private static void CheckPixelFormat(PixelFormat format)
        {
            if (((format != PixelFormat.Format24bppRgb) && (format != PixelFormat.Format8bppIndexed)) && ((format != PixelFormat.Format32bppArgb) && (format != PixelFormat.Format32bppRgb)))
            {
                throw new UnsupportedImageFormatException("Unsupported pixel format of the source image.");
            }
        }

        public static unsafe void FillRectangle(UnmanagedImage image, System.Drawing.Rectangle rectangle, Color color)
        {
            CheckPixelFormat(image.PixelFormat);
            int num = Image.GetPixelFormatSize(image.PixelFormat) / 8;
            int width = image.Width;
            int height = image.Height;
            int stride = image.Stride;
            int x = rectangle.X;
            int y = rectangle.Y;
            int num7 = (rectangle.X + rectangle.Width) - 1;
            int num8 = (rectangle.Y + rectangle.Height) - 1;
            if (((x < width) && (y < height)) && ((num7 >= 0) && (num8 >= 0)))
            {
                int num9 = Math.Max(0, x);
                int num10 = Math.Min(width - 1, num7);
                int num11 = Math.Max(0, y);
                int num12 = Math.Min(height - 1, num8);
                byte* dst = (byte*) ((image.ImageData.ToPointer() + (num11 * stride)) + (num9 * num));
                if (image.PixelFormat == PixelFormat.Format8bppIndexed)
                {
                    byte filler = (byte) (((0.2125 * color.R) + (0.7154 * color.G)) + (0.0721 * color.B));
                    int count = (num10 - num9) + 1;
                    for (int i = num11; i <= num12; i++)
                    {
                        SystemTools.SetUnmanagedMemory(dst, filler, count);
                        dst += stride;
                    }
                }
                else
                {
                    byte r = color.R;
                    byte g = color.G;
                    byte b = color.B;
                    int num19 = stride - (((num10 - num9) + 1) * num);
                    for (int j = num11; j <= num12; j++)
                    {
                        int num21 = num9;
                        while (num21 <= num10)
                        {
                            dst[2] = r;
                            dst[1] = g;
                            dst[0] = b;
                            num21++;
                            dst += num;
                        }
                        dst += num19;
                    }
                }
            }
        }

        public static void FillRectangle(BitmapData imageData, System.Drawing.Rectangle rectangle, Color color)
        {
            FillRectangle(new UnmanagedImage(imageData), rectangle, color);
        }

        public static unsafe void Line(UnmanagedImage image, IntPoint point1, IntPoint point2, Color color)
        {
            CheckPixelFormat(image.PixelFormat);
            int num = Image.GetPixelFormatSize(image.PixelFormat) / 8;
            int width = image.Width;
            int height = image.Height;
            int stride = image.Stride;
            if ((((point1.X >= 0) || (point2.X >= 0)) && ((point1.Y >= 0) || (point2.Y >= 0))) && (((point1.X < width) || (point2.X < width)) && ((point1.Y < height) || (point2.Y < height))))
            {
                CheckEndPoint(width, height, point1, ref point2);
                CheckEndPoint(width, height, point2, ref point1);
                if ((((point1.X >= 0) || (point2.X >= 0)) && ((point1.Y >= 0) || (point2.Y >= 0))) && (((point1.X < width) || (point2.X < width)) && ((point1.Y < height) || (point2.Y < height))))
                {
                    int x = point1.X;
                    int y = point1.Y;
                    int num7 = point2.X;
                    int num8 = point2.Y;
                    byte num9 = 0;
                    if (image.PixelFormat == PixelFormat.Format8bppIndexed)
                    {
                        num9 = (byte) (((0.2125 * color.R) + (0.7154 * color.G)) + (0.0721 * color.B));
                    }
                    int num10 = num7 - x;
                    int num11 = num8 - y;
                    if (Math.Abs(num10) >= Math.Abs(num11))
                    {
                        float num12 = (num10 != 0) ? (((float) num11) / ((float) num10)) : 0f;
                        int num13 = (num10 > 0) ? 1 : -1;
                        num10 += num13;
                        if (image.PixelFormat == PixelFormat.Format8bppIndexed)
                        {
                            for (int i = 0; i != num10; i += num13)
                            {
                                int num15 = x + i;
                                int num16 = y + ((int) (num12 * i));
                                byte* numPtr = (byte*) ((image.ImageData.ToPointer() + (num16 * stride)) + num15);
                                numPtr[0] = num9;
                            }
                        }
                        else
                        {
                            for (int j = 0; j != num10; j += num13)
                            {
                                int num18 = x + j;
                                int num19 = y + ((int) (num12 * j));
                                byte* numPtr2 = (byte*) ((image.ImageData.ToPointer() + (num19 * stride)) + (num18 * num));
                                numPtr2[2] = color.R;
                                numPtr2[1] = color.G;
                                numPtr2[0] = color.B;
                            }
                        }
                    }
                    else
                    {
                        float num20 = (num11 != 0) ? (((float) num10) / ((float) num11)) : 0f;
                        int num21 = (num11 > 0) ? 1 : -1;
                        num11 += num21;
                        if (image.PixelFormat == PixelFormat.Format8bppIndexed)
                        {
                            for (int k = 0; k != num11; k += num21)
                            {
                                int num23 = x + ((int) (num20 * k));
                                int num24 = y + k;
                                byte* numPtr3 = (byte*) ((image.ImageData.ToPointer() + (num24 * stride)) + num23);
                                numPtr3[0] = num9;
                            }
                        }
                        else
                        {
                            for (int m = 0; m != num11; m += num21)
                            {
                                int num26 = x + ((int) (num20 * m));
                                int num27 = y + m;
                                byte* numPtr4 = (byte*) ((image.ImageData.ToPointer() + (num27 * stride)) + (num26 * num));
                                numPtr4[2] = color.R;
                                numPtr4[1] = color.G;
                                numPtr4[0] = color.B;
                            }
                        }
                    }
                }
            }
        }

        public static void Line(BitmapData imageData, IntPoint point1, IntPoint point2, Color color)
        {
            Line(new UnmanagedImage(imageData), point1, point2, color);
        }

        public static void Polygon(UnmanagedImage image, List<IntPoint> points, Color color)
        {
            int num = 1;
            int count = points.Count;
            while (num < count)
            {
                Line(image, points[num - 1], points[num], color);
                num++;
            }
            Line(image, points[points.Count - 1], points[0], color);
        }

        public static void Polygon(BitmapData imageData, List<IntPoint> points, Color color)
        {
            Polygon(new UnmanagedImage(imageData), points, color);
        }

        public static void Polyline(UnmanagedImage image, List<IntPoint> points, Color color)
        {
            int num = 1;
            int count = points.Count;
            while (num < count)
            {
                Line(image, points[num - 1], points[num], color);
                num++;
            }
        }

        public static void Polyline(BitmapData imageData, List<IntPoint> points, Color color)
        {
            Polyline(new UnmanagedImage(imageData), points, color);
        }

        public static unsafe void Rectangle(UnmanagedImage image, System.Drawing.Rectangle rectangle, Color color)
        {
            CheckPixelFormat(image.PixelFormat);
            int num = Image.GetPixelFormatSize(image.PixelFormat) / 8;
            int width = image.Width;
            int height = image.Height;
            int stride = image.Stride;
            int x = rectangle.X;
            int y = rectangle.Y;
            int num7 = (rectangle.X + rectangle.Width) - 1;
            int num8 = (rectangle.Y + rectangle.Height) - 1;
            if (((x < width) && (y < height)) && ((num7 >= 0) && (num8 >= 0)))
            {
                int num9 = Math.Max(0, x);
                int num10 = Math.Min(width - 1, num7);
                int num11 = Math.Max(0, y);
                int num12 = Math.Min(height - 1, num8);
                if (image.PixelFormat == PixelFormat.Format8bppIndexed)
                {
                    byte filler = (byte) (((0.2125 * color.R) + (0.7154 * color.G)) + (0.0721 * color.B));
                    if (y >= 0)
                    {
                        byte* dst = (byte*) ((image.ImageData.ToPointer() + (y * stride)) + num9);
                        SystemTools.SetUnmanagedMemory(dst, filler, num10 - num9);
                    }
                    if (num8 < height)
                    {
                        byte* numPtr2 = (byte*) ((image.ImageData.ToPointer() + (num8 * stride)) + num9);
                        SystemTools.SetUnmanagedMemory(numPtr2, filler, num10 - num9);
                    }
                    if (x >= 0)
                    {
                        byte* numPtr3 = (byte*) ((image.ImageData.ToPointer() + (num11 * stride)) + x);
                        int num14 = num11;
                        while (num14 <= num12)
                        {
                            numPtr3[0] = filler;
                            num14++;
                            numPtr3 += stride;
                        }
                    }
                    if (num7 < width)
                    {
                        byte* numPtr4 = (byte*) ((image.ImageData.ToPointer() + (num11 * stride)) + num7);
                        int num15 = num11;
                        while (num15 <= num12)
                        {
                            numPtr4[0] = filler;
                            num15++;
                            numPtr4 += stride;
                        }
                    }
                }
                else
                {
                    byte r = color.R;
                    byte g = color.G;
                    byte b = color.B;
                    if (y >= 0)
                    {
                        byte* numPtr5 = (byte*) ((image.ImageData.ToPointer() + (y * stride)) + (num9 * num));
                        int num19 = num9;
                        while (num19 <= num10)
                        {
                            numPtr5[2] = r;
                            numPtr5[1] = g;
                            numPtr5[0] = b;
                            num19++;
                            numPtr5 += num;
                        }
                    }
                    if (num8 < height)
                    {
                        byte* numPtr6 = (byte*) ((image.ImageData.ToPointer() + (num8 * stride)) + (num9 * num));
                        int num20 = num9;
                        while (num20 <= num10)
                        {
                            numPtr6[2] = r;
                            numPtr6[1] = g;
                            numPtr6[0] = b;
                            num20++;
                            numPtr6 += num;
                        }
                    }
                    if (x >= 0)
                    {
                        byte* numPtr7 = (byte*) ((image.ImageData.ToPointer() + (num11 * stride)) + (x * num));
                        int num21 = num11;
                        while (num21 <= num12)
                        {
                            numPtr7[2] = r;
                            numPtr7[1] = g;
                            numPtr7[0] = b;
                            num21++;
                            numPtr7 += stride;
                        }
                    }
                    if (num7 < width)
                    {
                        byte* numPtr8 = (byte*) ((image.ImageData.ToPointer() + (num11 * stride)) + (num7 * num));
                        int num22 = num11;
                        while (num22 <= num12)
                        {
                            numPtr8[2] = r;
                            numPtr8[1] = g;
                            numPtr8[0] = b;
                            num22++;
                            numPtr8 += stride;
                        }
                    }
                }
            }
        }

        public static void Rectangle(BitmapData imageData, System.Drawing.Rectangle rectangle, Color color)
        {
            Rectangle(new UnmanagedImage(imageData), rectangle, color);
        }
    }
}

