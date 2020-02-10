namespace AForge.Imaging.ColorReduction
{
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Runtime.InteropServices;

    public abstract class ErrorDiffusionColorDithering
    {
        [NonSerialized]
        private Dictionary<Color, byte> cache = new Dictionary<Color, byte>();
        private Color[] colorTable = new Color[] { Color.Black, Color.DarkBlue, Color.DarkGreen, Color.DarkCyan, Color.DarkRed, Color.DarkMagenta, Color.DarkKhaki, Color.LightGray, Color.Gray, Color.Blue, Color.Green, Color.Cyan, Color.Red, Color.Magenta, Color.Yellow, Color.White };
        protected int height;
        protected int pixelSize;
        protected int stride;
        private bool useCaching;
        protected int width;
        protected int x;
        protected int y;

        protected ErrorDiffusionColorDithering()
        {
        }

        public unsafe Bitmap Apply(UnmanagedImage sourceImage)
        {
            if (((sourceImage.PixelFormat != PixelFormat.Format24bppRgb) && (sourceImage.PixelFormat != PixelFormat.Format32bppRgb)) && ((sourceImage.PixelFormat != PixelFormat.Format32bppArgb) && (sourceImage.PixelFormat != PixelFormat.Format32bppPArgb)))
            {
                throw new UnsupportedImageFormatException("Unsupported pixel format of the source image.");
            }
            this.cache.Clear();
            UnmanagedImage image = sourceImage.Clone();
            this.width = sourceImage.Width;
            this.height = sourceImage.Height;
            this.stride = sourceImage.Stride;
            this.pixelSize = Image.GetPixelFormatSize(sourceImage.PixelFormat) / 8;
            int num = this.stride - (this.width * this.pixelSize);
            Bitmap bitmap = new Bitmap(this.width, this.height, (this.colorTable.Length > 0x10) ? PixelFormat.Format8bppIndexed : PixelFormat.Format4bppIndexed);
            ColorPalette palette = bitmap.Palette;
            int index = 0;
            int length = this.colorTable.Length;
            while (index < length)
            {
                palette.Entries[index] = this.colorTable[index];
                index++;
            }
            bitmap.Palette = palette;
            BitmapData bitmapdata = bitmap.LockBits(new Rectangle(0, 0, this.width, this.height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            byte* ptr = (byte*) image.ImageData.ToPointer();
            byte* numPtr2 = (byte*) bitmapdata.Scan0.ToPointer();
            bool flag = this.colorTable.Length > 0x10;
            this.y = 0;
            while (this.y < this.height)
            {
                byte* numPtr3 = numPtr2 + (this.y * bitmapdata.Stride);
                this.x = 0;
                while (this.x < this.width)
                {
                    byte num7;
                    int red = ptr[2];
                    int green = ptr[1];
                    int blue = ptr[0];
                    Color color = this.GetClosestColor(red, green, blue, out num7);
                    this.Diffuse(red - color.R, green - color.G, blue - color.B, ptr);
                    if (flag)
                    {
                        numPtr3[0] = num7;
                        numPtr3++;
                    }
                    else if ((this.x % 2) == 0)
                    {
                        numPtr3[0] = (byte) (numPtr3[0] | ((byte) (num7 << 4)));
                    }
                    else
                    {
                        numPtr3[0] = (byte) (numPtr3[0] | num7);
                        numPtr3++;
                    }
                    this.x++;
                    ptr += this.pixelSize;
                }
                ptr += num;
                this.y++;
            }
            bitmap.UnlockBits(bitmapdata);
            image.Dispose();
            return bitmap;
        }

        public Bitmap Apply(Bitmap sourceImage)
        {
            BitmapData bitmapData = sourceImage.LockBits(new Rectangle(0, 0, sourceImage.Width, sourceImage.Height), ImageLockMode.ReadOnly, sourceImage.PixelFormat);
            Bitmap bitmap = null;
            try
            {
                bitmap = this.Apply(new UnmanagedImage(bitmapData));
            }
            finally
            {
                sourceImage.UnlockBits(bitmapData);
            }
            return bitmap;
        }

        protected abstract unsafe void Diffuse(int rError, int gError, int bError, byte* ptr);
        private Color GetClosestColor(int red, int green, int blue, out byte colorIndex)
        {
            Color key = Color.FromArgb(red, green, blue);
            if (this.useCaching && this.cache.ContainsKey(key))
            {
                colorIndex = this.cache[key];
            }
            else
            {
                colorIndex = 0;
                int num = 0x7fffffff;
                int index = 0;
                int length = this.colorTable.Length;
                while (index < length)
                {
                    int num4 = red - this.colorTable[index].R;
                    int num5 = green - this.colorTable[index].G;
                    int num6 = blue - this.colorTable[index].B;
                    int num7 = ((num4 * num4) + (num5 * num5)) + (num6 * num6);
                    if (num7 < num)
                    {
                        num = num7;
                        colorIndex = (byte) index;
                    }
                    index++;
                }
                if (this.useCaching)
                {
                    this.cache.Add(key, colorIndex);
                }
            }
            return this.colorTable[colorIndex];
        }

        public Color[] ColorTable
        {
            get
            {
                return this.colorTable;
            }
            set
            {
                if ((this.colorTable.Length < 2) || (this.colorTable.Length > 0x100))
                {
                    throw new ArgumentException("Color table length must be in the [2, 256] range.");
                }
                this.colorTable = value;
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

