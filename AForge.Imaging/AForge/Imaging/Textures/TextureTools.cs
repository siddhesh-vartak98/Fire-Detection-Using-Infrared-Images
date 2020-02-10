namespace AForge.Imaging.Textures
{
    using AForge.Imaging;
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class TextureTools
    {
        private TextureTools()
        {
        }

        public static unsafe float[,] FromBitmap(UnmanagedImage image)
        {
            if (image.PixelFormat != PixelFormat.Format8bppIndexed)
            {
                throw new UnsupportedImageFormatException("Only grayscale (8 bpp indexed images) are supported.");
            }
            int width = image.Width;
            int height = image.Height;
            float[,] numArray = new float[height, width];
            byte* numPtr = (byte*) image.ImageData.ToPointer();
            int num3 = image.Stride - width;
            for (int i = 0; i < height; i++)
            {
                int num5 = 0;
                while (num5 < width)
                {
                    numArray[i, num5] = ((float) numPtr[0]) / 255f;
                    num5++;
                    numPtr++;
                }
                numPtr += num3;
            }
            return numArray;
        }

        public static float[,] FromBitmap(Bitmap image)
        {
            BitmapData imageData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
            float[,] numArray = FromBitmap(imageData);
            image.UnlockBits(imageData);
            return numArray;
        }

        public static float[,] FromBitmap(BitmapData imageData)
        {
            return FromBitmap(new UnmanagedImage(imageData));
        }

        public static unsafe Bitmap ToBitmap(float[,] texture)
        {
            int length = texture.GetLength(1);
            int height = texture.GetLength(0);
            Bitmap bitmap = Image.CreateGrayscaleImage(length, height);
            BitmapData bitmapdata = bitmap.LockBits(new Rectangle(0, 0, length, height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
            byte* numPtr = (byte*) bitmapdata.Scan0.ToPointer();
            int num3 = bitmapdata.Stride - length;
            for (int i = 0; i < height; i++)
            {
                int num5 = 0;
                while (num5 < length)
                {
                    numPtr[0] = (byte) (texture[i, num5] * 255f);
                    num5++;
                    numPtr++;
                }
                numPtr += num3;
            }
            bitmap.UnlockBits(bitmapdata);
            return bitmap;
        }
    }
}

