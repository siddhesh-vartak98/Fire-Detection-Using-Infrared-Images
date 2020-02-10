namespace AForge.Imaging
{
    using AForge;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Runtime.InteropServices;

    public class UnmanagedImage : IDisposable
    {
        private int height;
        private IntPtr imageData;
        private bool mustBeDisposed;
        private System.Drawing.Imaging.PixelFormat pixelFormat;
        private int stride;
        private int width;

        public UnmanagedImage(BitmapData bitmapData)
        {
            this.imageData = bitmapData.Scan0;
            this.width = bitmapData.Width;
            this.height = bitmapData.Height;
            this.stride = bitmapData.Stride;
            this.pixelFormat = bitmapData.PixelFormat;
        }

        public UnmanagedImage(IntPtr imageData, int width, int height, int stride, System.Drawing.Imaging.PixelFormat pixelFormat)
        {
            this.imageData = imageData;
            this.width = width;
            this.height = height;
            this.stride = stride;
            this.pixelFormat = pixelFormat;
        }

        public UnmanagedImage Clone()
        {
            IntPtr imageData = Marshal.AllocHGlobal((int) (this.stride * this.height));
            UnmanagedImage image = new UnmanagedImage(imageData, this.width, this.height, this.stride, this.pixelFormat) {
                mustBeDisposed = true
            };
            SystemTools.CopyUnmanagedMemory(imageData, this.imageData, this.stride * this.height);
            return image;
        }

        public unsafe ushort[] Collect16bppPixelValues(List<IntPoint> points)
        {
            ushort* numPtr2;
            int num = Image.GetPixelFormatSize(this.pixelFormat) / 8;
            if (((this.pixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed) || (num == 3)) || (num == 4))
            {
                throw new UnsupportedImageFormatException("Unsupported pixel format of the source image. Use Collect8bppPixelValues() method for it.");
            }
            ushort[] numArray = new ushort[points.Count * ((this.pixelFormat == System.Drawing.Imaging.PixelFormat.Format16bppGrayScale) ? 1 : 3)];
            byte* numPtr = (byte*) this.imageData.ToPointer();
            if (this.pixelFormat == System.Drawing.Imaging.PixelFormat.Format16bppGrayScale)
            {
                int num2 = 0;
                foreach (IntPoint point in points)
                {
                    numPtr2 = (ushort*) ((numPtr + (this.stride * point.Y)) + (point.X * num));
                    numArray[num2++] = numPtr2[0];
                }
                return numArray;
            }
            int num3 = 0;
            foreach (IntPoint point2 in points)
            {
                numPtr2 = (ushort*) ((numPtr + (this.stride * point2.Y)) + (point2.X * num));
                numArray[num3++] = numPtr2[2];
                numArray[num3++] = numPtr2[1];
                numArray[num3++] = numPtr2[0];
            }
            return numArray;
        }

        public unsafe byte[] Collect8bppPixelValues(List<IntPoint> points)
        {
            byte* numPtr2;
            int num = Image.GetPixelFormatSize(this.pixelFormat) / 8;
            if ((this.pixelFormat == System.Drawing.Imaging.PixelFormat.Format16bppGrayScale) || (num > 4))
            {
                throw new UnsupportedImageFormatException("Unsupported pixel format of the source image. Use Collect16bppPixelValues() method for it.");
            }
            byte[] buffer = new byte[points.Count * ((this.pixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed) ? 1 : 3)];
            byte* numPtr = (byte*) this.imageData.ToPointer();
            if (this.pixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
            {
                int num2 = 0;
                foreach (IntPoint point in points)
                {
                    numPtr2 = (numPtr + (this.stride * point.Y)) + point.X;
                    buffer[num2++] = numPtr2[0];
                }
                return buffer;
            }
            int num3 = 0;
            foreach (IntPoint point2 in points)
            {
                numPtr2 = (numPtr + (this.stride * point2.Y)) + (point2.X * num);
                buffer[num3++] = numPtr2[2];
                buffer[num3++] = numPtr2[1];
                buffer[num3++] = numPtr2[0];
            }
            return buffer;
        }

        public unsafe void Copy(UnmanagedImage destImage)
        {
            if (((this.width != destImage.width) || (this.height != destImage.height)) || (this.pixelFormat != destImage.pixelFormat))
            {
                throw new InvalidImagePropertiesException("Destination image has different size or pixel format.");
            }
            if (this.stride == destImage.stride)
            {
                SystemTools.CopyUnmanagedMemory(destImage.imageData, this.imageData, this.stride * this.height);
            }
            else
            {
                int stride = destImage.stride;
                int count = (this.stride < stride) ? this.stride : stride;
                byte* src = (byte*) this.imageData.ToPointer();
                byte* dst = (byte*) destImage.imageData.ToPointer();
                for (int i = 0; i < this.height; i++)
                {
                    SystemTools.CopyUnmanagedMemory(dst, src, count);
                    dst += stride;
                    src += this.stride;
                }
            }
        }

        public static UnmanagedImage Create(int width, int height, System.Drawing.Imaging.PixelFormat pixelFormat)
        {
            int num = 0;
            switch (pixelFormat)
            {
                case System.Drawing.Imaging.PixelFormat.Format8bppIndexed:
                    num = 1;
                    break;

                case System.Drawing.Imaging.PixelFormat.Format32bppPArgb:
                case System.Drawing.Imaging.PixelFormat.Format32bppRgb:
                case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                    num = 4;
                    break;

                case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
                    num = 3;
                    break;

                case System.Drawing.Imaging.PixelFormat.Format16bppGrayScale:
                    num = 2;
                    break;

                case System.Drawing.Imaging.PixelFormat.Format48bppRgb:
                    num = 6;
                    break;

                case System.Drawing.Imaging.PixelFormat.Format64bppPArgb:
                case System.Drawing.Imaging.PixelFormat.Format64bppArgb:
                    num = 8;
                    break;

                default:
                    throw new UnsupportedImageFormatException("Can not create image with specified pixel format.");
            }
            if ((width <= 0) || (height <= 0))
            {
                throw new InvalidImagePropertiesException("Invalid image size specified.");
            }
            int stride = width * num;
            if ((stride % 4) != 0)
            {
                stride += 4 - (stride % 4);
            }
            IntPtr dst = Marshal.AllocHGlobal((int) (stride * height));
            SystemTools.SetUnmanagedMemory(dst, 0, stride * height);
            return new UnmanagedImage(dst, width, height, stride, pixelFormat) { mustBeDisposed = true };
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.mustBeDisposed && (this.imageData != IntPtr.Zero))
            {
                Marshal.FreeHGlobal(this.imageData);
                this.imageData = IntPtr.Zero;
            }
        }

        ~UnmanagedImage()
        {
            this.Dispose(false);
        }

        public static UnmanagedImage FromManagedImage(Bitmap image)
        {
            UnmanagedImage image2 = null;
            BitmapData imageData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
            try
            {
                image2 = FromManagedImage(imageData);
            }
            finally
            {
                image.UnlockBits(imageData);
            }
            return image2;
        }

        public static UnmanagedImage FromManagedImage(BitmapData imageData)
        {
            System.Drawing.Imaging.PixelFormat pixelFormat = imageData.PixelFormat;
            if (((((pixelFormat != System.Drawing.Imaging.PixelFormat.Format8bppIndexed) && (pixelFormat != System.Drawing.Imaging.PixelFormat.Format16bppGrayScale)) && ((pixelFormat != System.Drawing.Imaging.PixelFormat.Format24bppRgb) && (pixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppRgb))) && (((pixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb) && (pixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppPArgb)) && ((pixelFormat != System.Drawing.Imaging.PixelFormat.Format48bppRgb) && (pixelFormat != System.Drawing.Imaging.PixelFormat.Format64bppArgb)))) && (pixelFormat != System.Drawing.Imaging.PixelFormat.Format64bppPArgb))
            {
                throw new UnsupportedImageFormatException("Unsupported pixel format of the source image.");
            }
            IntPtr ptr = Marshal.AllocHGlobal((int) (imageData.Stride * imageData.Height));
            UnmanagedImage image = new UnmanagedImage(ptr, imageData.Width, imageData.Height, imageData.Stride, pixelFormat);
            SystemTools.CopyUnmanagedMemory(ptr, imageData.Scan0, imageData.Stride * imageData.Height);
            image.mustBeDisposed = true;
            return image;
        }

        public unsafe Bitmap ToManagedImage()
        {
            Bitmap bitmap = (this.pixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed) ? Image.CreateGrayscaleImage(this.width, this.height) : new Bitmap(this.width, this.height, this.pixelFormat);
            BitmapData bitmapdata = bitmap.LockBits(new Rectangle(0, 0, this.width, this.height), ImageLockMode.ReadWrite, this.pixelFormat);
            int stride = bitmapdata.Stride;
            int count = Math.Min(this.stride, stride);
            byte* dst = (byte*) bitmapdata.Scan0.ToPointer();
            byte* src = (byte*) this.imageData.ToPointer();
            for (int i = 0; i < this.height; i++)
            {
                SystemTools.CopyUnmanagedMemory(dst, src, count);
                dst += stride;
                src += this.stride;
            }
            bitmap.UnlockBits(bitmapdata);
            return bitmap;
        }

        public int Height
        {
            get
            {
                return this.height;
            }
        }

        public IntPtr ImageData
        {
            get
            {
                return this.imageData;
            }
        }

        public System.Drawing.Imaging.PixelFormat PixelFormat
        {
            get
            {
                return this.pixelFormat;
            }
        }

        public int Stride
        {
            get
            {
                return this.stride;
            }
        }

        public int Width
        {
            get
            {
                return this.width;
            }
        }
    }
}

