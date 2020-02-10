namespace AForge.Imaging
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class ExhaustiveTemplateMatching : ITemplateMatching
    {
        private float similarityThreshold;

        public ExhaustiveTemplateMatching()
        {
            this.similarityThreshold = 0.9f;
        }

        public ExhaustiveTemplateMatching(float similarityThreshold)
        {
            this.similarityThreshold = 0.9f;
            this.similarityThreshold = similarityThreshold;
        }

        public TemplateMatch[] ProcessImage(UnmanagedImage image, UnmanagedImage template)
        {
            return this.ProcessImage(image, template, new Rectangle(0, 0, image.Width, image.Height));
        }

        public TemplateMatch[] ProcessImage(Bitmap image, Bitmap template)
        {
            return this.ProcessImage(image, template, new Rectangle(0, 0, image.Width, image.Height));
        }

        public TemplateMatch[] ProcessImage(BitmapData imageData, BitmapData templateData)
        {
            return this.ProcessImage(new UnmanagedImage(imageData), new UnmanagedImage(templateData), new Rectangle(0, 0, imageData.Width, imageData.Height));
        }

        public unsafe TemplateMatch[] ProcessImage(UnmanagedImage image, UnmanagedImage template, Rectangle searchZone)
        {
            if (((image.PixelFormat != PixelFormat.Format8bppIndexed) && (image.PixelFormat != PixelFormat.Format24bppRgb)) || (image.PixelFormat != template.PixelFormat))
            {
                throw new UnsupportedImageFormatException("Unsupported pixel format of the source or template image.");
            }
            Rectangle rectangle = searchZone;
            rectangle.Intersect(new Rectangle(0, 0, image.Width, image.Height));
            int x = rectangle.X;
            int y = rectangle.Y;
            int width = rectangle.Width;
            int height = rectangle.Height;
            int num5 = template.Width;
            int num6 = template.Height;
            if ((num5 > width) || (num6 > height))
            {
                throw new InvalidImagePropertiesException("Template's size should be smaller or equal to search zone.");
            }
            int num7 = (image.PixelFormat == PixelFormat.Format8bppIndexed) ? 1 : 3;
            int stride = image.Stride;
            int num9 = (width - num5) + 1;
            int num10 = (height - num6) + 1;
            int[,] numArray = new int[num10 + 4, num9 + 4];
            int num11 = ((num5 * num6) * num7) * 0xff;
            int num12 = (int) (this.similarityThreshold * num11);
            int num13 = num5 * num7;
            byte* numPtr = (byte*) image.ImageData.ToPointer();
            byte* numPtr2 = (byte*) template.ImageData.ToPointer();
            int num14 = image.Stride - (num5 * num7);
            int num15 = template.Stride - (num5 * num7);
            for (int i = 0; i < num10; i++)
            {
                for (int j = 0; j < num9; j++)
                {
                    byte* numPtr3 = (numPtr + (stride * (i + y))) + (num7 * (j + x));
                    byte* numPtr4 = numPtr2;
                    int num18 = 0;
                    for (int k = 0; k < num6; k++)
                    {
                        int num20 = 0;
                        while (num20 < num13)
                        {
                            int num21 = numPtr3[0] - numPtr4[0];
                            if (num21 > 0)
                            {
                                num18 += num21;
                            }
                            else
                            {
                                num18 -= num21;
                            }
                            num20++;
                            numPtr3++;
                            numPtr4++;
                        }
                        numPtr3 += num14;
                        numPtr4 += num15;
                    }
                    int num22 = num11 - num18;
                    if (num22 >= num12)
                    {
                        numArray[i + 2, j + 2] = num22;
                    }
                }
            }
            List<TemplateMatch> list = new List<TemplateMatch>();
            int num23 = 2;
            int num24 = num10 + 2;
            while (num23 < num24)
            {
                int num25 = 2;
                int num26 = num9 + 2;
                while (num25 < num26)
                {
                    int num27 = numArray[num23, num25];
                    for (int m = -2; (num27 != 0) && (m <= 2); m++)
                    {
                        for (int n = -2; n <= 2; n++)
                        {
                            if (numArray[num23 + m, num25 + n] > num27)
                            {
                                num27 = 0;
                                break;
                            }
                        }
                    }
                    if (num27 != 0)
                    {
                        list.Add(new TemplateMatch(new Rectangle((num25 - 2) + x, (num23 - 2) + y, num5, num6), ((float) num27) / ((float) num11)));
                    }
                    num25++;
                }
                num23++;
            }
            TemplateMatch[] array = new TemplateMatch[list.Count];
            list.CopyTo(array);
            Array.Sort(array, new MatchingsSorter());
            return array;
        }

        public TemplateMatch[] ProcessImage(Bitmap image, Bitmap template, Rectangle searchZone)
        {
            TemplateMatch[] matchArray;
            if (((image.PixelFormat != PixelFormat.Format8bppIndexed) && (image.PixelFormat != PixelFormat.Format24bppRgb)) || (image.PixelFormat != template.PixelFormat))
            {
                throw new UnsupportedImageFormatException("Unsupported pixel format of the source or template image.");
            }
            if ((template.Width > image.Width) || (template.Height > image.Height))
            {
                throw new InvalidImagePropertiesException("Template's size should be smaller or equal to source image's size.");
            }
            BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
            BitmapData data2 = template.LockBits(new Rectangle(0, 0, template.Width, template.Height), ImageLockMode.ReadOnly, template.PixelFormat);
            try
            {
                matchArray = this.ProcessImage(new UnmanagedImage(bitmapData), new UnmanagedImage(data2), searchZone);
            }
            finally
            {
                image.UnlockBits(bitmapData);
                template.UnlockBits(data2);
            }
            return matchArray;
        }

        public TemplateMatch[] ProcessImage(BitmapData imageData, BitmapData templateData, Rectangle searchZone)
        {
            return this.ProcessImage(new UnmanagedImage(imageData), new UnmanagedImage(templateData), searchZone);
        }

        public float SimilarityThreshold
        {
            get
            {
                return this.similarityThreshold;
            }
            set
            {
                this.similarityThreshold = Math.Min(1f, Math.Max(0f, value));
            }
        }

        private class MatchingsSorter : IComparer
        {
            public int Compare(object x, object y)
            {
                float num = ((TemplateMatch) y).Similarity - ((TemplateMatch) x).Similarity;
                if (num > 0f)
                {
                    return 1;
                }
                if (num >= 0f)
                {
                    return 0;
                }
                return -1;
            }
        }
    }
}

