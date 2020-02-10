namespace AForge.Imaging
{
    using AForge.Math;
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class ComplexImage : ICloneable
    {
        private Complex[,] data;
        private bool fourierTransformed;
        private int height;
        private int width;

        protected ComplexImage(int width, int height)
        {
            this.width = width;
            this.height = height;
            this.data = new Complex[height, width];
            this.fourierTransformed = false;
        }

        public void BackwardFourierTransform()
        {
            if (this.fourierTransformed)
            {
                FourierTransform.FFT2(this.data, -1);
                this.fourierTransformed = false;
                for (int i = 0; i < this.height; i++)
                {
                    for (int j = 0; j < this.width; j++)
                    {
                        if (((j + i) & 1) != 0)
                        {
                            Complex complex1 = this.data[i, j];
                            complex1.Re *= -1.0;
                            Complex complex2 = this.data[i, j];
                            complex2.Im *= -1.0;
                        }
                    }
                }
            }
        }

        public object Clone()
        {
            ComplexImage image = new ComplexImage(this.width, this.height);
            Complex[,] data = image.data;
            for (int i = 0; i < this.height; i++)
            {
                for (int j = 0; j < this.width; j++)
                {
                    *(data[i, j]) = *(this.data[i, j]);
                }
            }
            image.fourierTransformed = this.fourierTransformed;
            return image;
        }

        public void ForwardFourierTransform()
        {
            if (!this.fourierTransformed)
            {
                for (int i = 0; i < this.height; i++)
                {
                    for (int j = 0; j < this.width; j++)
                    {
                        if (((j + i) & 1) != 0)
                        {
                            Complex complex1 = this.data[i, j];
                            complex1.Re *= -1.0;
                            Complex complex2 = this.data[i, j];
                            complex2.Im *= -1.0;
                        }
                    }
                }
                FourierTransform.FFT2(this.data, 1);
                this.fourierTransformed = true;
            }
        }

        public static ComplexImage FromBitmap(Bitmap image)
        {
            ComplexImage image2;
            if (image.PixelFormat != PixelFormat.Format8bppIndexed)
            {
                throw new UnsupportedImageFormatException("Source image can be graysclae (8bpp indexed) image only.");
            }
            BitmapData imageData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
            try
            {
                image2 = FromBitmap(imageData);
            }
            finally
            {
                image.UnlockBits(imageData);
            }
            return image2;
        }

        public static unsafe ComplexImage FromBitmap(BitmapData imageData)
        {
            if (imageData.PixelFormat != PixelFormat.Format8bppIndexed)
            {
                throw new UnsupportedImageFormatException("Source image can be graysclae (8bpp indexed) image only.");
            }
            int width = imageData.Width;
            int height = imageData.Height;
            int num3 = imageData.Stride - width;
            if (!Tools.IsPowerOf2(width) || !Tools.IsPowerOf2(height))
            {
                throw new InvalidImagePropertiesException("Image width and height should be power of 2.");
            }
            ComplexImage image = new ComplexImage(width, height);
            Complex[,] data = image.data;
            byte* numPtr = (byte*) imageData.Scan0.ToPointer();
            for (int i = 0; i < height; i++)
            {
                int num5 = 0;
                while (num5 < width)
                {
                    data[i, num5].Re = ((float) numPtr[0]) / 255f;
                    num5++;
                    numPtr++;
                }
                numPtr += num3;
            }
            return image;
        }

        public unsafe Bitmap ToBitmap()
        {
            Bitmap bitmap = Image.CreateGrayscaleImage(this.width, this.height);
            BitmapData bitmapdata = bitmap.LockBits(new Rectangle(0, 0, this.width, this.height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
            int num = bitmapdata.Stride - this.width;
            double num2 = this.fourierTransformed ? Math.Sqrt((double) (this.width * this.height)) : 1.0;
            byte* numPtr = (byte*) bitmapdata.Scan0.ToPointer();
            for (int i = 0; i < this.height; i++)
            {
                int num4 = 0;
                while (num4 < this.width)
                {
                    numPtr[0] = (byte) Math.Max(0.0, Math.Min((double) 255.0, (double) ((this.data[i, num4].get_Magnitude() * num2) * 255.0)));
                    num4++;
                    numPtr++;
                }
                numPtr += num;
            }
            bitmap.UnlockBits(bitmapdata);
            return bitmap;
        }

        public Complex[,] Data
        {
            get
            {
                return this.data;
            }
        }

        public bool FourierTransformed
        {
            get
            {
                return this.fourierTransformed;
            }
        }

        public int Height
        {
            get
            {
                return this.height;
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

