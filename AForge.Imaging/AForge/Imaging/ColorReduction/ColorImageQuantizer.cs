namespace AForge.Imaging.ColorReduction
{
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class ColorImageQuantizer
    {
        [NonSerialized]
        private Dictionary<Color, int> cache = new Dictionary<Color, int>();
        [NonSerialized]
        private Color[] paletteToUse;
        private IColorQuantizer quantizer;
        private bool useCaching;

        public ColorImageQuantizer(IColorQuantizer quantizer)
        {
            this.quantizer = quantizer;
        }

        public unsafe Color[] CalculatePalette(UnmanagedImage image, int paletteSize)
        {
            if (((image.PixelFormat != PixelFormat.Format24bppRgb) && (image.PixelFormat != PixelFormat.Format32bppRgb)) && ((image.PixelFormat != PixelFormat.Format32bppArgb) && (image.PixelFormat != PixelFormat.Format32bppPArgb)))
            {
                throw new UnsupportedImageFormatException("Unsupported format of the source image.");
            }
            this.quantizer.Clear();
            int width = image.Width;
            int height = image.Height;
            int num3 = Image.GetPixelFormatSize(image.PixelFormat) / 8;
            byte* numPtr = (byte*) image.ImageData.ToPointer();
            int num4 = image.Stride - (width * num3);
            for (int i = 0; i < height; i++)
            {
                int num6 = 0;
                while (num6 < width)
                {
                    this.quantizer.AddColor(Color.FromArgb(numPtr[2], numPtr[1], numPtr[0]));
                    num6++;
                    numPtr += num3;
                }
                numPtr += num4;
            }
            return this.quantizer.GetPalette(paletteSize);
        }

        public Color[] CalculatePalette(Bitmap image, int paletteSize)
        {
            Color[] colorArray;
            BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
            try
            {
                colorArray = this.CalculatePalette(new UnmanagedImage(bitmapData), paletteSize);
            }
            finally
            {
                image.UnlockBits(bitmapData);
            }
            return colorArray;
        }

        private int GetClosestColor(int red, int green, int blue)
        {
            Color key = Color.FromArgb(red, green, blue);
            if (this.useCaching && this.cache.ContainsKey(key))
            {
                return this.cache[key];
            }
            int num = 0;
            int num2 = 0x7fffffff;
            int index = 0;
            int length = this.paletteToUse.Length;
            while (index < length)
            {
                int num5 = red - this.paletteToUse[index].R;
                int num6 = green - this.paletteToUse[index].G;
                int num7 = blue - this.paletteToUse[index].B;
                int num8 = ((num5 * num5) + (num6 * num6)) + (num7 * num7);
                if (num8 < num2)
                {
                    num2 = num8;
                    num = (byte) index;
                }
                index++;
            }
            if (this.useCaching)
            {
                this.cache.Add(key, num);
            }
            return num;
        }

        public Bitmap ReduceColors(UnmanagedImage image, int paletteSize)
        {
            if ((paletteSize < 2) || (paletteSize > 0x100))
            {
                throw new ArgumentException("Invalid size of the target color palette.");
            }
            return this.ReduceColors(image, this.CalculatePalette(image, paletteSize));
        }

        public unsafe Bitmap ReduceColors(UnmanagedImage image, Color[] palette)
        {
            if (((image.PixelFormat != PixelFormat.Format24bppRgb) && (image.PixelFormat != PixelFormat.Format32bppRgb)) && ((image.PixelFormat != PixelFormat.Format32bppArgb) && (image.PixelFormat != PixelFormat.Format32bppPArgb)))
            {
                throw new UnsupportedImageFormatException("Unsupported format of the source image.");
            }
            if ((palette.Length < 2) || (palette.Length > 0x100))
            {
                throw new ArgumentException("Invalid size of the target color palette.");
            }
            this.paletteToUse = palette;
            this.cache.Clear();
            int width = image.Width;
            int height = image.Height;
            int stride = image.Stride;
            int num4 = Image.GetPixelFormatSize(image.PixelFormat) / 8;
            int num5 = stride - (width * num4);
            Bitmap bitmap = new Bitmap(width, height, (palette.Length > 0x10) ? PixelFormat.Format8bppIndexed : PixelFormat.Format4bppIndexed);
            ColorPalette palette2 = bitmap.Palette;
            int index = 0;
            int length = palette.Length;
            while (index < length)
            {
                palette2.Entries[index] = palette[index];
                index++;
            }
            bitmap.Palette = palette2;
            BitmapData bitmapdata = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            byte* numPtr = (byte*) image.ImageData.ToPointer();
            byte* numPtr2 = (byte*) bitmapdata.Scan0.ToPointer();
            bool flag = palette.Length > 0x10;
            for (int i = 0; i < height; i++)
            {
                byte* numPtr3 = numPtr2 + (i * bitmapdata.Stride);
                int num9 = 0;
                while (num9 < width)
                {
                    byte num10 = (byte) this.GetClosestColor(numPtr[2], numPtr[1], numPtr[0]);
                    if (flag)
                    {
                        numPtr3[0] = num10;
                        numPtr3++;
                    }
                    else if ((num9 % 2) == 0)
                    {
                        numPtr3[0] = (byte) (numPtr3[0] | ((byte) (num10 << 4)));
                    }
                    else
                    {
                        numPtr3[0] = (byte) (numPtr3[0] | num10);
                        numPtr3++;
                    }
                    num9++;
                    numPtr += num4;
                }
                numPtr += num5;
            }
            bitmap.UnlockBits(bitmapdata);
            return bitmap;
        }

        public Bitmap ReduceColors(Bitmap image, Color[] palette)
        {
            Bitmap bitmap;
            BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
            try
            {
                bitmap = this.ReduceColors(new UnmanagedImage(bitmapData), palette);
            }
            finally
            {
                image.UnlockBits(bitmapData);
            }
            return bitmap;
        }

        public Bitmap ReduceColors(Bitmap image, int paletteSize)
        {
            Bitmap bitmap;
            BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
            try
            {
                bitmap = this.ReduceColors(new UnmanagedImage(bitmapData), paletteSize);
            }
            finally
            {
                image.UnlockBits(bitmapData);
            }
            return bitmap;
        }

        public IColorQuantizer Quantizer
        {
            get
            {
                return this.quantizer;
            }
            set
            {
                this.quantizer = value;
            }
        }

        public bool UseCaching
        {
            get
            {
                return this.useCaching;
            }
            set
            {
                this.useCaching = value;
            }
        }
    }
}

