namespace AForge.Imaging
{
    using AForge;
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;

    public static class Image
    {
        public static Bitmap Clone(Bitmap source)
        {
            BitmapData sourceData = source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadOnly, source.PixelFormat);
            Bitmap bitmap = Clone(sourceData);
            source.UnlockBits(sourceData);
            if (((source.PixelFormat == PixelFormat.Format1bppIndexed) || (source.PixelFormat == PixelFormat.Format4bppIndexed)) || ((source.PixelFormat == PixelFormat.Format8bppIndexed) || (source.PixelFormat == PixelFormat.Indexed)))
            {
                ColorPalette palette = source.Palette;
                ColorPalette palette2 = bitmap.Palette;
                int length = palette.Entries.Length;
                for (int i = 0; i < length; i++)
                {
                    palette2.Entries[i] = palette.Entries[i];
                }
                bitmap.Palette = palette2;
            }
            return bitmap;
        }

        public static Bitmap Clone(BitmapData sourceData)
        {
            int width = sourceData.Width;
            int height = sourceData.Height;
            Bitmap bitmap = new Bitmap(width, height, sourceData.PixelFormat);
            BitmapData bitmapdata = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            SystemTools.CopyUnmanagedMemory(bitmapdata.Scan0, sourceData.Scan0, height * sourceData.Stride);
            bitmap.UnlockBits(bitmapdata);
            return bitmap;
        }

        public static Bitmap Clone(Bitmap source, PixelFormat format)
        {
            if (source.PixelFormat == format)
            {
                return Clone(source);
            }
            int width = source.Width;
            int height = source.Height;
            Bitmap image = new Bitmap(width, height, format);
            Graphics graphics = Graphics.FromImage(image);
            graphics.DrawImage(source, 0, 0, width, height);
            graphics.Dispose();
            return image;
        }

        public static unsafe Bitmap Convert16bppTo8bpp(Bitmap bimap)
        {
            Bitmap bitmap = null;
            int num = 0;
            int width = bimap.Width;
            int height = bimap.Height;
            switch (bimap.PixelFormat)
            {
                case PixelFormat.Format64bppPArgb:
                    bitmap = new Bitmap(width, height, PixelFormat.Format32bppPArgb);
                    num = 4;
                    break;

                case PixelFormat.Format64bppArgb:
                    bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                    num = 4;
                    break;

                case PixelFormat.Format16bppGrayScale:
                    bitmap = CreateGrayscaleImage(width, height);
                    num = 1;
                    break;

                case PixelFormat.Format48bppRgb:
                    bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
                    num = 3;
                    break;

                default:
                    throw new UnsupportedImageFormatException("Invalid pixel format of the source image.");
            }
            BitmapData bitmapdata = bimap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, bimap.PixelFormat);
            BitmapData data2 = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            int num4 = (int) bitmapdata.Scan0.ToPointer();
            int num5 = (int) data2.Scan0.ToPointer();
            int stride = bitmapdata.Stride;
            int num7 = data2.Stride;
            for (int i = 0; i < height; i++)
            {
                ushort* numPtr = (ushort*) (num4 + (i * stride));
                byte* numPtr2 = (byte*) (num5 + (i * num7));
                int num9 = 0;
                int num10 = width * num;
                while (num9 < num10)
                {
                    numPtr2[0] = (byte) (numPtr[0] >> 8);
                    num9++;
                    numPtr++;
                    numPtr2++;
                }
            }
            bimap.UnlockBits(bitmapdata);
            bitmap.UnlockBits(data2);
            return bitmap;
        }

        public static unsafe Bitmap Convert8bppTo16bpp(Bitmap bimap)
        {
            Bitmap bitmap = null;
            int num = 0;
            int width = bimap.Width;
            int height = bimap.Height;
            switch (bimap.PixelFormat)
            {
                case PixelFormat.Format32bppPArgb:
                    bitmap = new Bitmap(width, height, PixelFormat.Format64bppPArgb);
                    num = 4;
                    break;

                case PixelFormat.Format32bppArgb:
                    bitmap = new Bitmap(width, height, PixelFormat.Format64bppArgb);
                    num = 4;
                    break;

                case PixelFormat.Format24bppRgb:
                    bitmap = new Bitmap(width, height, PixelFormat.Format48bppRgb);
                    num = 3;
                    break;

                case PixelFormat.Format8bppIndexed:
                    bitmap = new Bitmap(width, height, PixelFormat.Format16bppGrayScale);
                    num = 1;
                    break;

                default:
                    throw new UnsupportedImageFormatException("Invalid pixel format of the source image.");
            }
            BitmapData bitmapdata = bimap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, bimap.PixelFormat);
            BitmapData data2 = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            int num4 = (int) bitmapdata.Scan0.ToPointer();
            int num5 = (int) data2.Scan0.ToPointer();
            int stride = bitmapdata.Stride;
            int num7 = data2.Stride;
            for (int i = 0; i < height; i++)
            {
                byte* numPtr = (byte*) (num4 + (i * stride));
                ushort* numPtr2 = (ushort*) (num5 + (i * num7));
                int num9 = 0;
                int num10 = width * num;
                while (num9 < num10)
                {
                    numPtr2[0] = (ushort) (numPtr[0] << 8);
                    num9++;
                    numPtr++;
                    numPtr2++;
                }
            }
            bimap.UnlockBits(bitmapdata);
            bitmap.UnlockBits(data2);
            return bitmap;
        }

        public static Bitmap CreateGrayscaleImage(int width, int height)
        {
            Bitmap image = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
            SetGrayscalePalette(image);
            return image;
        }

        [Obsolete("Use Clone(Bitmap, PixelFormat) method instead and specify desired pixel format")]
        public static void FormatImage(ref Bitmap image)
        {
            if ((((image.PixelFormat != PixelFormat.Format24bppRgb) && (image.PixelFormat != PixelFormat.Format32bppRgb)) && ((image.PixelFormat != PixelFormat.Format32bppArgb) && (image.PixelFormat != PixelFormat.Format48bppRgb))) && (((image.PixelFormat != PixelFormat.Format64bppArgb) && (image.PixelFormat != PixelFormat.Format16bppGrayScale)) && !IsGrayscale(image)))
            {
                Bitmap source = image;
                image = Clone(source, PixelFormat.Format24bppRgb);
                source.Dispose();
            }
        }

        public static Bitmap FromFile(string fileName)
        {
            Bitmap bitmap = null;
            FileStream stream = null;
            try
            {
                stream = File.OpenRead(fileName);
                MemoryStream stream2 = new MemoryStream();
                byte[] buffer = new byte[0x2710];
                while (true)
                {
                    int count = stream.Read(buffer, 0, 0x2710);
                    if (count == 0)
                    {
                        break;
                    }
                    stream2.Write(buffer, 0, count);
                }
                bitmap = (Bitmap) Image.FromStream(stream2);
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                    stream.Dispose();
                }
            }
            return bitmap;
        }

        public static bool IsGrayscale(Bitmap image)
        {
            bool flag = false;
            if (image.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                flag = true;
                ColorPalette palette = image.Palette;
                for (int i = 0; i < 0x100; i++)
                {
                    Color color = palette.Entries[i];
                    if (((color.R != i) || (color.G != i)) || (color.B != i))
                    {
                        return false;
                    }
                }
            }
            return flag;
        }

        public static void SetGrayscalePalette(Bitmap image)
        {
            if (image.PixelFormat != PixelFormat.Format8bppIndexed)
            {
                throw new UnsupportedImageFormatException("Source image is not 8 bpp image.");
            }
            ColorPalette palette = image.Palette;
            for (int i = 0; i < 0x100; i++)
            {
                palette.Entries[i] = Color.FromArgb(i, i, i);
            }
            image.Palette = palette;
        }
    }
}

