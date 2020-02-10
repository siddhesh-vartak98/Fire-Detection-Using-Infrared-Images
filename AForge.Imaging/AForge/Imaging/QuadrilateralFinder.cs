namespace AForge.Imaging
{
    using AForge;
    using AForge.Math.Geometry;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class QuadrilateralFinder
    {
        private void CheckPixelFormat(PixelFormat format)
        {
            if (((format != PixelFormat.Format8bppIndexed) && (format != PixelFormat.Format24bppRgb)) && ((format != PixelFormat.Format32bppArgb) && (format != PixelFormat.Format32bppPArgb)))
            {
                throw new UnsupportedImageFormatException("Unsupported pixel format of the source image.");
            }
        }

        public unsafe List<IntPoint> ProcessImage(UnmanagedImage image)
        {
            bool flag;
            this.CheckPixelFormat(image.PixelFormat);
            int width = image.Width;
            int height = image.Height;
            List<IntPoint> list = new List<IntPoint>();
            byte* numPtr = (byte*) image.ImageData.ToPointer();
            int stride = image.Stride;
            if (image.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                for (int i = 0; i < height; i++)
                {
                    flag = true;
                    for (int j = 0; j < width; j++)
                    {
                        if (numPtr[j] != 0)
                        {
                            list.Add(new IntPoint(j, i));
                            flag = false;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        for (int k = width - 1; k >= 0; k--)
                        {
                            if (numPtr[k] != 0)
                            {
                                list.Add(new IntPoint(k, i));
                                break;
                            }
                        }
                    }
                    numPtr += stride;
                }
            }
            else
            {
                int num7 = Image.GetPixelFormatSize(image.PixelFormat) / 8;
                byte* numPtr2 = null;
                for (int m = 0; m < height; m++)
                {
                    flag = true;
                    numPtr2 = numPtr;
                    int x = 0;
                    while (x < width)
                    {
                        if (((numPtr2[2] != 0) || (numPtr2[1] != 0)) || (numPtr2[0] != 0))
                        {
                            list.Add(new IntPoint(x, m));
                            flag = false;
                            break;
                        }
                        x++;
                        numPtr2 += num7;
                    }
                    if (!flag)
                    {
                        numPtr2 = (numPtr + (width * num7)) - num7;
                        int num10 = width - 1;
                        while (num10 >= 0)
                        {
                            if (((numPtr2[2] != 0) || (numPtr2[1] != 0)) || (numPtr2[0] != 0))
                            {
                                list.Add(new IntPoint(num10, m));
                                break;
                            }
                            num10--;
                            numPtr2 -= num7;
                        }
                    }
                    numPtr += stride;
                }
            }
            return PointsCloud.FindQuadrilateralCorners(list);
        }

        public List<IntPoint> ProcessImage(Bitmap image)
        {
            this.CheckPixelFormat(image.PixelFormat);
            BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
            List<IntPoint> list = null;
            try
            {
                list = this.ProcessImage(new UnmanagedImage(bitmapData));
            }
            finally
            {
                image.UnlockBits(bitmapData);
            }
            return list;
        }

        public List<IntPoint> ProcessImage(BitmapData imageData)
        {
            return this.ProcessImage(new UnmanagedImage(imageData));
        }
    }
}

