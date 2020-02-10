namespace AForge.Imaging
{
    using AForge;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Runtime.InteropServices;

    public abstract class BlobCounterBase
    {
        private List<Blob> blobs;
        private bool coupledSizeFiltering;
        private IBlobsFilter filter;
        private bool filterBlobs;
        protected int imageHeight;
        protected int imageWidth;
        private int maxHeight;
        private int maxWidth;
        private int minHeight;
        private int minWidth;
        protected int[] objectLabels;
        protected int objectsCount;
        private AForge.Imaging.ObjectsOrder objectsOrder;

        public BlobCounterBase()
        {
            this.blobs = new List<Blob>();
            this.minWidth = 1;
            this.minHeight = 1;
            this.maxWidth = 0x7fffffff;
            this.maxHeight = 0x7fffffff;
        }

        public BlobCounterBase(UnmanagedImage image)
        {
            this.blobs = new List<Blob>();
            this.minWidth = 1;
            this.minHeight = 1;
            this.maxWidth = 0x7fffffff;
            this.maxHeight = 0x7fffffff;
            this.ProcessImage(image);
        }

        public BlobCounterBase(Bitmap image)
        {
            this.blobs = new List<Blob>();
            this.minWidth = 1;
            this.minHeight = 1;
            this.maxWidth = 0x7fffffff;
            this.maxHeight = 0x7fffffff;
            this.ProcessImage(image);
        }

        public BlobCounterBase(BitmapData imageData)
        {
            this.blobs = new List<Blob>();
            this.minWidth = 1;
            this.minHeight = 1;
            this.maxWidth = 0x7fffffff;
            this.maxHeight = 0x7fffffff;
            this.ProcessImage(imageData);
        }

        protected abstract void BuildObjectsMap(UnmanagedImage image);
        private unsafe void CollectObjectsInfo(UnmanagedImage image)
        {
            int num2;
            int index = 0;
            int[] numArray = new int[this.objectsCount + 1];
            int[] numArray2 = new int[this.objectsCount + 1];
            int[] numArray3 = new int[this.objectsCount + 1];
            int[] numArray4 = new int[this.objectsCount + 1];
            int[] numArray5 = new int[this.objectsCount + 1];
            long[] numArray6 = new long[this.objectsCount + 1];
            long[] numArray7 = new long[this.objectsCount + 1];
            long[] numArray8 = new long[this.objectsCount + 1];
            long[] numArray9 = new long[this.objectsCount + 1];
            long[] numArray10 = new long[this.objectsCount + 1];
            long[] numArray11 = new long[this.objectsCount + 1];
            long[] numArray12 = new long[this.objectsCount + 1];
            long[] numArray13 = new long[this.objectsCount + 1];
            for (int i = 1; i <= this.objectsCount; i++)
            {
                numArray[i] = this.imageWidth;
                numArray2[i] = this.imageHeight;
            }
            byte* numPtr = (byte*) image.ImageData.ToPointer();
            if (image.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                int num4 = image.Stride - this.imageWidth;
                for (int k = 0; k < this.imageHeight; k++)
                {
                    int num7 = 0;
                    while (num7 < this.imageWidth)
                    {
                        num2 = this.objectLabels[index];
                        if (num2 != 0)
                        {
                            if (num7 < numArray[num2])
                            {
                                numArray[num2] = num7;
                            }
                            if (num7 > numArray3[num2])
                            {
                                numArray3[num2] = num7;
                            }
                            if (k < numArray2[num2])
                            {
                                numArray2[num2] = k;
                            }
                            if (k > numArray4[num2])
                            {
                                numArray4[num2] = k;
                            }
                            numArray5[num2]++;
                            numArray6[num2] += num7;
                            numArray7[num2] += k;
                            byte num5 = numPtr[0];
                            numArray9[num2] += num5;
                            numArray12[num2] += num5 * num5;
                        }
                        num7++;
                        index++;
                        numPtr++;
                    }
                    numPtr += num4;
                }
                for (int m = 1; m <= this.objectsCount; m++)
                {
                    numArray8[m] = numArray10[m] = numArray9[m];
                    numArray11[m] = numArray13[m] = numArray9[m];
                }
            }
            else
            {
                int num9 = Image.GetPixelFormatSize(image.PixelFormat) / 8;
                int num10 = image.Stride - (this.imageWidth * num9);
                for (int n = 0; n < this.imageHeight; n++)
                {
                    int num15 = 0;
                    while (num15 < this.imageWidth)
                    {
                        num2 = this.objectLabels[index];
                        if (num2 != 0)
                        {
                            if (num15 < numArray[num2])
                            {
                                numArray[num2] = num15;
                            }
                            if (num15 > numArray3[num2])
                            {
                                numArray3[num2] = num15;
                            }
                            if (n < numArray2[num2])
                            {
                                numArray2[num2] = n;
                            }
                            if (n > numArray4[num2])
                            {
                                numArray4[num2] = n;
                            }
                            numArray5[num2]++;
                            numArray6[num2] += num15;
                            numArray7[num2] += n;
                            byte num11 = numPtr[2];
                            byte num12 = numPtr[1];
                            byte num13 = numPtr[0];
                            numArray8[num2] += num11;
                            numArray9[num2] += num12;
                            numArray10[num2] += num13;
                            numArray11[num2] += num11 * num11;
                            numArray12[num2] += num12 * num12;
                            numArray13[num2] += num13 * num13;
                        }
                        num15++;
                        index++;
                        numPtr += num9;
                    }
                    numPtr += num10;
                }
            }
            this.blobs.Clear();
            for (int j = 1; j <= this.objectsCount; j++)
            {
                Blob blob;
                int num17 = numArray5[j];
                blob = new Blob(j, new Rectangle(numArray[j], numArray2[j], (numArray3[j] - numArray[j]) + 1, (numArray4[j] - numArray2[j]) + 1)) {
                    Area = num17,
                    Fullness = ((double) num17) / ((double) (((numArray3[j] - numArray[j]) + 1) * ((numArray4[j] - numArray2[j]) + 1))),
                    CenterOfGravity = new IntPoint((int) (numArray6[j] / ((long) num17)), (int) (numArray7[j] / ((long) num17))),
                    ColorMean = Color.FromArgb((byte) (numArray8[j] / ((long) num17)), (byte) (numArray9[j] / ((long) num17)), (byte) (numArray10[j] / ((long) num17))),
                    ColorStdDev = Color.FromArgb((byte) Math.Sqrt((double) ((numArray11[j] / ((long) num17)) - (blob.ColorMean.R * blob.ColorMean.R))), (byte) Math.Sqrt((double) ((numArray12[j] / ((long) num17)) - (blob.ColorMean.G * blob.ColorMean.G))), (byte) Math.Sqrt((double) ((numArray13[j] / ((long) num17)) - (blob.ColorMean.B * blob.ColorMean.B))))
                };
                this.blobs.Add(blob);
            }
        }

        public unsafe void ExtractBlobsImage(UnmanagedImage image, Blob blob, bool extractInOriginalSize)
        {
            if (this.objectLabels == null)
            {
                throw new ApplicationException("Image should be processed before to collect objects map.");
            }
            if ((((image.PixelFormat != PixelFormat.Format24bppRgb) && (image.PixelFormat != PixelFormat.Format8bppIndexed)) && ((image.PixelFormat != PixelFormat.Format32bppArgb) && (image.PixelFormat != PixelFormat.Format32bppRgb))) && (image.PixelFormat != PixelFormat.Format32bppPArgb))
            {
                throw new UnsupportedImageFormatException("Unsupported pixel format of the provided image.");
            }
            int width = image.Width;
            int height = image.Height;
            int stride = image.Stride;
            int num4 = Image.GetPixelFormatSize(image.PixelFormat) / 8;
            int num5 = blob.Rectangle.Width;
            int num6 = blob.Rectangle.Height;
            int num7 = extractInOriginalSize ? width : num5;
            int num8 = extractInOriginalSize ? height : num6;
            int left = blob.Rectangle.Left;
            int num10 = (left + num5) - 1;
            int top = blob.Rectangle.Top;
            int num12 = (top + num6) - 1;
            int iD = blob.ID;
            blob.Image = UnmanagedImage.Create(num7, num8, image.PixelFormat);
            blob.OriginalSize = extractInOriginalSize;
            byte* numPtr = (byte*) ((image.ImageData.ToPointer() + (top * stride)) + (left * num4));
            byte* numPtr2 = (byte*) blob.Image.ImageData.ToPointer();
            int index = (top * width) + left;
            if (extractInOriginalSize)
            {
                numPtr2 += (top * blob.Image.Stride) + (left * num4);
            }
            int num15 = stride - (num5 * num4);
            int num16 = blob.Image.Stride - (num5 * num4);
            int num17 = width - num5;
            for (int i = top; i <= num12; i++)
            {
                int num19 = left;
                while (num19 <= num10)
                {
                    if (this.objectLabels[index] == iD)
                    {
                        numPtr2[0] = numPtr[0];
                        if (num4 > 1)
                        {
                            numPtr2[1] = numPtr[1];
                            numPtr2[2] = numPtr[2];
                            if (num4 > 3)
                            {
                                numPtr2[3] = numPtr[3];
                            }
                        }
                    }
                    num19++;
                    index++;
                    numPtr2 += num4;
                    numPtr += num4;
                }
                numPtr += num15;
                numPtr2 += num16;
                index += num17;
            }
        }

        public void ExtractBlobsImage(Bitmap image, Blob blob, bool extractInOriginalSize)
        {
            BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
            try
            {
                this.ExtractBlobsImage(new UnmanagedImage(bitmapData), blob, extractInOriginalSize);
            }
            finally
            {
                image.UnlockBits(bitmapData);
            }
        }

        public List<IntPoint> GetBlobsEdgePoints(Blob blob)
        {
            if (this.objectLabels == null)
            {
                throw new ApplicationException("Image should be processed before to collect objects map.");
            }
            List<IntPoint> list = new List<IntPoint>();
            int left = blob.Rectangle.Left;
            int num2 = (left + blob.Rectangle.Width) - 1;
            int top = blob.Rectangle.Top;
            int num4 = (top + blob.Rectangle.Height) - 1;
            int iD = blob.ID;
            int[] numArray = new int[blob.Rectangle.Height];
            int[] numArray2 = new int[blob.Rectangle.Height];
            for (int i = top; i <= num4; i++)
            {
                int index = (i * this.imageWidth) + left;
                int x = left;
                while (x <= num2)
                {
                    if (this.objectLabels[index] == iD)
                    {
                        list.Add(new IntPoint(x, i));
                        numArray[i - top] = x;
                        break;
                    }
                    x++;
                    index++;
                }
                index = (i * this.imageWidth) + num2;
                int num9 = num2;
                while (num9 >= left)
                {
                    if (this.objectLabels[index] == iD)
                    {
                        if (numArray[i - top] != num9)
                        {
                            list.Add(new IntPoint(num9, i));
                        }
                        numArray2[i - top] = num9;
                        break;
                    }
                    num9--;
                    index--;
                }
            }
            for (int j = left; j <= num2; j++)
            {
                int num11 = (top * this.imageWidth) + j;
                int y = top;
                int num13 = 0;
                while (y <= num4)
                {
                    if (this.objectLabels[num11] == iD)
                    {
                        if ((numArray[num13] != j) && (numArray2[num13] != j))
                        {
                            list.Add(new IntPoint(j, y));
                        }
                        break;
                    }
                    y++;
                    num13++;
                    num11 += this.imageWidth;
                }
                num11 = (num4 * this.imageWidth) + j;
                int num14 = num4;
                int num15 = num4 - top;
                while (num14 >= top)
                {
                    if (this.objectLabels[num11] == iD)
                    {
                        if ((numArray[num15] != j) && (numArray2[num15] != j))
                        {
                            list.Add(new IntPoint(j, num14));
                        }
                        break;
                    }
                    num14--;
                    num15--;
                    num11 -= this.imageWidth;
                }
            }
            return list;
        }

        public void GetBlobsLeftAndRightEdges(Blob blob, out List<IntPoint> leftEdge, out List<IntPoint> rightEdge)
        {
            if (this.objectLabels == null)
            {
                throw new ApplicationException("Image should be processed before to collect objects map.");
            }
            leftEdge = new List<IntPoint>();
            rightEdge = new List<IntPoint>();
            int left = blob.Rectangle.Left;
            int num2 = (left + blob.Rectangle.Width) - 1;
            int top = blob.Rectangle.Top;
            int num4 = (top + blob.Rectangle.Height) - 1;
            int iD = blob.ID;
            for (int i = top; i <= num4; i++)
            {
                int index = (i * this.imageWidth) + left;
                int x = left;
                while (x <= num2)
                {
                    if (this.objectLabels[index] == iD)
                    {
                        leftEdge.Add(new IntPoint(x, i));
                        break;
                    }
                    x++;
                    index++;
                }
                index = (i * this.imageWidth) + num2;
                int num9 = num2;
                while (num9 >= left)
                {
                    if (this.objectLabels[index] == iD)
                    {
                        rightEdge.Add(new IntPoint(num9, i));
                        break;
                    }
                    num9--;
                    index--;
                }
            }
        }

        public void GetBlobsTopAndBottomEdges(Blob blob, out List<IntPoint> topEdge, out List<IntPoint> bottomEdge)
        {
            if (this.objectLabels == null)
            {
                throw new ApplicationException("Image should be processed before to collect objects map.");
            }
            topEdge = new List<IntPoint>();
            bottomEdge = new List<IntPoint>();
            int left = blob.Rectangle.Left;
            int num2 = (left + blob.Rectangle.Width) - 1;
            int top = blob.Rectangle.Top;
            int num4 = (top + blob.Rectangle.Height) - 1;
            int iD = blob.ID;
            for (int i = left; i <= num2; i++)
            {
                int index = (top * this.imageWidth) + i;
                int y = top;
                while (y <= num4)
                {
                    if (this.objectLabels[index] == iD)
                    {
                        topEdge.Add(new IntPoint(i, y));
                        break;
                    }
                    y++;
                    index += this.imageWidth;
                }
                index = (num4 * this.imageWidth) + i;
                int num9 = num4;
                while (num9 >= top)
                {
                    if (this.objectLabels[index] == iD)
                    {
                        bottomEdge.Add(new IntPoint(i, num9));
                        break;
                    }
                    num9--;
                    index -= this.imageWidth;
                }
            }
        }

        public unsafe Blob[] GetObjects(UnmanagedImage image, bool extractInOriginalSize)
        {
            if (this.objectLabels == null)
            {
                throw new ApplicationException("Image should be processed before to collect objects map.");
            }
            if ((((image.PixelFormat != PixelFormat.Format24bppRgb) && (image.PixelFormat != PixelFormat.Format8bppIndexed)) && ((image.PixelFormat != PixelFormat.Format32bppArgb) && (image.PixelFormat != PixelFormat.Format32bppRgb))) && (image.PixelFormat != PixelFormat.Format32bppPArgb))
            {
                throw new UnsupportedImageFormatException("Unsupported pixel format of the provided image.");
            }
            int width = image.Width;
            int height = image.Height;
            int stride = image.Stride;
            int num4 = Image.GetPixelFormatSize(image.PixelFormat) / 8;
            Blob[] blobArray = new Blob[this.objectsCount];
            for (int i = 0; i < this.objectsCount; i++)
            {
                int num6 = this.blobs[i].Rectangle.Width;
                int num7 = this.blobs[i].Rectangle.Height;
                int num8 = extractInOriginalSize ? width : num6;
                int num9 = extractInOriginalSize ? height : num7;
                int x = this.blobs[i].Rectangle.X;
                int num11 = (x + num6) - 1;
                int y = this.blobs[i].Rectangle.Y;
                int num13 = (y + num7) - 1;
                int iD = this.blobs[i].ID;
                UnmanagedImage image2 = UnmanagedImage.Create(num8, num9, image.PixelFormat);
                byte* numPtr = (byte*) ((image.ImageData.ToPointer() + (y * stride)) + (x * num4));
                byte* numPtr2 = (byte*) image2.ImageData.ToPointer();
                int index = (y * width) + x;
                if (extractInOriginalSize)
                {
                    numPtr2 += (y * image2.Stride) + (x * num4);
                }
                int num16 = stride - (num6 * num4);
                int num17 = image2.Stride - (num6 * num4);
                int num18 = width - num6;
                for (int j = y; j <= num13; j++)
                {
                    int num20 = x;
                    while (num20 <= num11)
                    {
                        if (this.objectLabels[index] == iD)
                        {
                            numPtr2[0] = numPtr[0];
                            if (num4 > 1)
                            {
                                numPtr2[1] = numPtr[1];
                                numPtr2[2] = numPtr[2];
                                if (num4 > 3)
                                {
                                    numPtr2[3] = numPtr[3];
                                }
                            }
                        }
                        num20++;
                        index++;
                        numPtr2 += num4;
                        numPtr += num4;
                    }
                    numPtr += num16;
                    numPtr2 += num17;
                    index += num18;
                }
                blobArray[i] = new Blob(this.blobs[i]);
                blobArray[i].Image = image2;
                blobArray[i].OriginalSize = extractInOriginalSize;
            }
            return blobArray;
        }

        public Blob[] GetObjects(Bitmap image, bool extractInOriginalSize)
        {
            Blob[] objects = null;
            BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
            try
            {
                objects = this.GetObjects(new UnmanagedImage(bitmapData), extractInOriginalSize);
            }
            finally
            {
                image.UnlockBits(bitmapData);
            }
            return objects;
        }

        public Blob[] GetObjectsInformation()
        {
            if (this.objectLabels == null)
            {
                throw new ApplicationException("Image should be processed before to collect objects map.");
            }
            Blob[] blobArray = new Blob[this.objectsCount];
            for (int i = 0; i < this.objectsCount; i++)
            {
                blobArray[i] = new Blob(this.blobs[i]);
            }
            return blobArray;
        }

        public Rectangle[] GetObjectsRectangles()
        {
            if (this.objectLabels == null)
            {
                throw new ApplicationException("Image should be processed before to collect objects map.");
            }
            Rectangle[] rectangleArray = new Rectangle[this.objectsCount];
            for (int i = 0; i < this.objectsCount; i++)
            {
                rectangleArray[i] = this.blobs[i].Rectangle;
            }
            return rectangleArray;
        }

        public void ProcessImage(UnmanagedImage image)
        {
            this.imageWidth = image.Width;
            this.imageHeight = image.Height;
            this.BuildObjectsMap(image);
            this.CollectObjectsInfo(image);
            if (this.filterBlobs)
            {
                int[] numArray = new int[this.objectsCount + 1];
                for (int i = 1; i <= this.objectsCount; i++)
                {
                    numArray[i] = i;
                }
                int num2 = 0;
                if (this.filter == null)
                {
                    for (int k = this.objectsCount - 1; k >= 0; k--)
                    {
                        int width = this.blobs[k].Rectangle.Width;
                        int height = this.blobs[k].Rectangle.Height;
                        if (!this.coupledSizeFiltering)
                        {
                            if (((width < this.minWidth) || (height < this.minHeight)) || ((width > this.maxWidth) || (height > this.maxHeight)))
                            {
                                numArray[k + 1] = 0;
                                num2++;
                                this.blobs.RemoveAt(k);
                            }
                        }
                        else if (((width < this.minWidth) && (height < this.minHeight)) || ((width > this.maxWidth) && (height > this.maxHeight)))
                        {
                            numArray[k + 1] = 0;
                            num2++;
                            this.blobs.RemoveAt(k);
                        }
                    }
                }
                else
                {
                    for (int m = this.objectsCount - 1; m >= 0; m--)
                    {
                        if (!this.filter.Check(this.blobs[m]))
                        {
                            numArray[m + 1] = 0;
                            num2++;
                            this.blobs.RemoveAt(m);
                        }
                    }
                }
                int num7 = 0;
                for (int j = 1; j <= this.objectsCount; j++)
                {
                    if (numArray[j] != 0)
                    {
                        num7++;
                        numArray[j] = num7;
                    }
                }
                int index = 0;
                int length = this.objectLabels.Length;
                while (index < length)
                {
                    this.objectLabels[index] = numArray[this.objectLabels[index]];
                    index++;
                }
                this.objectsCount -= num2;
                int num11 = 0;
                int count = this.blobs.Count;
                while (num11 < count)
                {
                    this.blobs[num11].ID = num11 + 1;
                    num11++;
                }
            }
            if (this.objectsOrder != AForge.Imaging.ObjectsOrder.None)
            {
                this.blobs.Sort(new BlobsSorter(this.objectsOrder));
            }
        }

        public void ProcessImage(Bitmap image)
        {
            BitmapData imageData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
            try
            {
                this.ProcessImage(imageData);
            }
            finally
            {
                image.UnlockBits(imageData);
            }
        }

        public void ProcessImage(BitmapData imageData)
        {
            this.ProcessImage(new UnmanagedImage(imageData));
        }

        public IBlobsFilter BlobsFilter
        {
            get
            {
                return this.filter;
            }
            set
            {
                this.filter = value;
            }
        }

        public bool CoupledSizeFiltering
        {
            get
            {
                return this.coupledSizeFiltering;
            }
            set
            {
                this.coupledSizeFiltering = value;
            }
        }

        public bool FilterBlobs
        {
            get
            {
                return this.filterBlobs;
            }
            set
            {
                this.filterBlobs = value;
            }
        }

        public int MaxHeight
        {
            get
            {
                return this.maxHeight;
            }
            set
            {
                this.maxHeight = value;
            }
        }

        public int MaxWidth
        {
            get
            {
                return this.maxWidth;
            }
            set
            {
                this.maxWidth = value;
            }
        }

        public int MinHeight
        {
            get
            {
                return this.minHeight;
            }
            set
            {
                this.minHeight = value;
            }
        }

        public int MinWidth
        {
            get
            {
                return this.minWidth;
            }
            set
            {
                this.minWidth = value;
            }
        }

        public int[] ObjectLabels
        {
            get
            {
                return this.objectLabels;
            }
        }

        public int ObjectsCount
        {
            get
            {
                return this.objectsCount;
            }
        }

        public AForge.Imaging.ObjectsOrder ObjectsOrder
        {
            get
            {
                return this.objectsOrder;
            }
            set
            {
                this.objectsOrder = value;
            }
        }

        private class BlobsSorter : IComparer<Blob>
        {
            private ObjectsOrder order;

            public BlobsSorter(ObjectsOrder order)
            {
                this.order = order;
            }

            public int Compare(Blob a, Blob b)
            {
                Rectangle rectangle = a.Rectangle;
                Rectangle rectangle2 = b.Rectangle;
                switch (this.order)
                {
                    case ObjectsOrder.Size:
                        return ((rectangle2.Width * rectangle2.Height) - (rectangle.Width * rectangle.Height));

                    case ObjectsOrder.Area:
                        return (b.Area - a.Area);

                    case ObjectsOrder.YX:
                        return (((rectangle.Y * 0x186a0) + rectangle.X) - ((rectangle2.Y * 0x186a0) + rectangle2.X));

                    case ObjectsOrder.XY:
                        return (((rectangle.X * 0x186a0) + rectangle.Y) - ((rectangle2.X * 0x186a0) + rectangle2.Y));
                }
                return 0;
            }
        }
    }
}

