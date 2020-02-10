namespace AForge.Imaging.Filters
{
    using AForge;
    using AForge.Imaging;
    using AForge.Math.Geometry;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class BackwardQuadrilateralTransformation : BaseInPlaceFilter
    {
        private List<IntPoint> destinationQuadrilateral;
        private Dictionary<PixelFormat, PixelFormat> formatTranslations;
        private Bitmap sourceImage;
        private UnmanagedImage sourceUnmanagedImage;
        private bool useInterpolation;

        public BackwardQuadrilateralTransformation()
        {
            this.useInterpolation = true;
            this.formatTranslations = new Dictionary<PixelFormat, PixelFormat>();
            this.formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            this.formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
            this.formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
            this.formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
            this.formatTranslations[PixelFormat.Format32bppPArgb] = PixelFormat.Format32bppPArgb;
        }

        public BackwardQuadrilateralTransformation(UnmanagedImage sourceUnmanagedImage) : this()
        {
            this.sourceUnmanagedImage = sourceUnmanagedImage;
        }

        public BackwardQuadrilateralTransformation(Bitmap sourceImage) : this()
        {
            this.sourceImage = sourceImage;
        }

        public BackwardQuadrilateralTransformation(UnmanagedImage sourceUnmanagedImage, List<IntPoint> destinationQuadrilateral) : this()
        {
            this.sourceUnmanagedImage = sourceUnmanagedImage;
            this.destinationQuadrilateral = destinationQuadrilateral;
        }

        public BackwardQuadrilateralTransformation(Bitmap sourceImage, List<IntPoint> destinationQuadrilateral) : this()
        {
            this.sourceImage = sourceImage;
            this.destinationQuadrilateral = destinationQuadrilateral;
        }

        protected override void ProcessFilter(UnmanagedImage image)
        {
            if (this.destinationQuadrilateral == null)
            {
                throw new NullReferenceException("Destination quadrilateral was not set.");
            }
            if (this.sourceImage != null)
            {
                if (image.PixelFormat != this.sourceImage.PixelFormat)
                {
                    throw new InvalidImagePropertiesException("Source and destination images must have same pixel format.");
                }
                BitmapData bitmapData = this.sourceImage.LockBits(new Rectangle(0, 0, this.sourceImage.Width, this.sourceImage.Height), ImageLockMode.ReadOnly, this.sourceImage.PixelFormat);
                try
                {
                    this.ProcessFilter(image, new UnmanagedImage(bitmapData));
                    return;
                }
                finally
                {
                    this.sourceImage.UnlockBits(bitmapData);
                }
            }
            if (this.sourceUnmanagedImage == null)
            {
                throw new NullReferenceException("Source image is not set.");
            }
            if (image.PixelFormat != this.sourceUnmanagedImage.PixelFormat)
            {
                throw new InvalidImagePropertiesException("Source and destination images must have same pixel format.");
            }
            this.ProcessFilter(image, this.sourceUnmanagedImage);
        }

        private unsafe void ProcessFilter(UnmanagedImage dstImage, UnmanagedImage srcImage)
        {
            IntPoint point;
            IntPoint point2;
            int width = srcImage.Width;
            int height = srcImage.Height;
            int num3 = dstImage.Width;
            int num4 = dstImage.Height;
            int num5 = Image.GetPixelFormatSize(srcImage.PixelFormat) / 8;
            int stride = srcImage.Stride;
            int num7 = dstImage.Stride;
            PointsCloud.GetBoundingRectangle(this.destinationQuadrilateral, ref point, ref point2);
            if (((point2.X >= 0) && (point2.Y >= 0)) && ((point.X < num3) && (point.Y < num4)))
            {
                if (point.X < 0)
                {
                    point.X = 0;
                }
                if (point.Y < 0)
                {
                    point.Y = 0;
                }
                if (point2.X >= num3)
                {
                    point2.X = num3 - 1;
                }
                if (point2.Y >= num4)
                {
                    point2.Y = num4 - 1;
                }
                int x = point.X;
                int y = point.Y;
                int num10 = point2.X + 1;
                int num11 = point2.Y + 1;
                int num12 = num7 - ((num10 - x) * num5);
                List<IntPoint> output = new List<IntPoint> {
                    new IntPoint(0, 0),
                    new IntPoint(width - 1, 0),
                    new IntPoint(width - 1, height - 1),
                    new IntPoint(0, height - 1)
                };
                double[,] numArray = QuadTransformationCalcs.MapQuadToQuad(this.destinationQuadrilateral, output);
                byte* numPtr = (byte*) dstImage.ImageData.ToPointer();
                byte* numPtr2 = (byte*) srcImage.ImageData.ToPointer();
                numPtr += (y * num7) + (x * num5);
                if (!this.useInterpolation)
                {
                    for (int i = y; i < num11; i++)
                    {
                        for (int j = x; j < num10; j++)
                        {
                            double num15 = ((numArray[2, 0] * j) + (numArray[2, 1] * i)) + numArray[2, 2];
                            double num16 = (((numArray[0, 0] * j) + (numArray[0, 1] * i)) + numArray[0, 2]) / num15;
                            double num17 = (((numArray[1, 0] * j) + (numArray[1, 1] * i)) + numArray[1, 2]) / num15;
                            if (((num16 >= 0.0) && (num17 >= 0.0)) && ((num16 < width) && (num17 < height)))
                            {
                                byte* numPtr3 = (numPtr2 + (((int) num17) * stride)) + (((int) num16) * num5);
                                int num18 = 0;
                                while (num18 < num5)
                                {
                                    numPtr[0] = numPtr3[0];
                                    num18++;
                                    numPtr++;
                                    numPtr3++;
                                }
                            }
                            else
                            {
                                numPtr += num5;
                            }
                        }
                        numPtr += num12;
                    }
                }
                else
                {
                    int num19 = width - 1;
                    int num20 = height - 1;
                    for (int k = y; k < num11; k++)
                    {
                        for (int m = x; m < num10; m++)
                        {
                            double num31 = ((numArray[2, 0] * m) + (numArray[2, 1] * k)) + numArray[2, 2];
                            double num32 = (((numArray[0, 0] * m) + (numArray[0, 1] * k)) + numArray[0, 2]) / num31;
                            double num33 = (((numArray[1, 0] * m) + (numArray[1, 1] * k)) + numArray[1, 2]) / num31;
                            if (((num32 >= 0.0) && (num33 >= 0.0)) && ((num32 < width) && (num33 < height)))
                            {
                                byte* numPtr5;
                                byte* numPtr7;
                                int num25 = (int) num32;
                                int num27 = (num25 == num19) ? num25 : (num25 + 1);
                                double num21 = num32 - num25;
                                double num23 = 1.0 - num21;
                                int num26 = (int) num33;
                                int num28 = (num26 == num20) ? num26 : (num26 + 1);
                                double num22 = num33 - num26;
                                double num24 = 1.0 - num22;
                                byte* numPtr4 = numPtr5 = numPtr2 + (num26 * stride);
                                numPtr4 += num25 * num5;
                                numPtr5 += num27 * num5;
                                byte* numPtr6 = numPtr7 = numPtr2 + (num28 * stride);
                                numPtr6 += num25 * num5;
                                numPtr7 += num27 * num5;
                                int num34 = 0;
                                while (num34 < num5)
                                {
                                    numPtr[0] = (byte) ((num24 * ((num23 * numPtr4[0]) + (num21 * numPtr5[0]))) + (num22 * ((num23 * numPtr6[0]) + (num21 * numPtr7[0]))));
                                    num34++;
                                    numPtr++;
                                    numPtr4++;
                                    numPtr5++;
                                    numPtr6++;
                                    numPtr7++;
                                }
                            }
                            else
                            {
                                numPtr += num5;
                            }
                        }
                        numPtr += num12;
                    }
                }
            }
        }

        public List<IntPoint> DestinationQuadrilateral
        {
            get
            {
                return this.destinationQuadrilateral;
            }
            set
            {
                this.destinationQuadrilateral = value;
            }
        }

        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get
            {
                return this.formatTranslations;
            }
        }

        public Bitmap SourceImage
        {
            get
            {
                return this.sourceImage;
            }
            set
            {
                this.sourceImage = value;
                if (value != null)
                {
                    this.sourceUnmanagedImage = null;
                }
            }
        }

        public UnmanagedImage SourceUnmanagedImage
        {
            get
            {
                return this.sourceUnmanagedImage;
            }
            set
            {
                this.sourceUnmanagedImage = value;
                if (value != null)
                {
                    this.sourceImage = null;
                }
            }
        }

        public bool UseInterpolation
        {
            get
            {
                return this.useInterpolation;
            }
            set
            {
                this.useInterpolation = value;
            }
        }
    }
}

