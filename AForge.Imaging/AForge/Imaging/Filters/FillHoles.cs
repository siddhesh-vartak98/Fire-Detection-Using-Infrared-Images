namespace AForge.Imaging.Filters
{
    using AForge.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing.Imaging;

    public class FillHoles : BaseInPlaceFilter
    {
        private bool coupledSizeFiltering = true;
        private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();
        private int maxHoleHeight = 0x7fffffff;
        private int maxHoleWidth = 0x7fffffff;

        public FillHoles()
        {
            this.formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
        }

        protected override unsafe void ProcessFilter(UnmanagedImage image)
        {
            int width = image.Width;
            int height = image.Height;
            UnmanagedImage image2 = new Invert().Apply(image);
            BlobCounter counter = new BlobCounter();
            counter.ProcessImage(image2);
            Blob[] objectsInformation = counter.GetObjectsInformation();
            byte[] buffer = new byte[objectsInformation.Length + 1];
            buffer[0] = 0xff;
            int index = 0;
            int length = objectsInformation.Length;
            while (index < length)
            {
                Blob blob = objectsInformation[index];
                if (((blob.Rectangle.Left == 0) || (blob.Rectangle.Top == 0)) || ((blob.Rectangle.Right == width) || (blob.Rectangle.Bottom == height)))
                {
                    buffer[blob.ID] = 0;
                }
                else if (((this.coupledSizeFiltering && (blob.Rectangle.Width <= this.maxHoleWidth)) && (blob.Rectangle.Height <= this.maxHoleHeight)) | (!this.coupledSizeFiltering && ((blob.Rectangle.Width <= this.maxHoleWidth) || (blob.Rectangle.Height <= this.maxHoleHeight))))
                {
                    buffer[blob.ID] = 0xff;
                }
                else
                {
                    buffer[blob.ID] = 0;
                }
                index++;
            }
            byte* numPtr = (byte*) image.ImageData.ToPointer();
            int num5 = image.Stride - width;
            int[] objectLabels = counter.ObjectLabels;
            int num6 = 0;
            int num7 = 0;
            while (num6 < height)
            {
                int num8 = 0;
                while (num8 < width)
                {
                    numPtr[0] = buffer[objectLabels[num7]];
                    num8++;
                    num7++;
                    numPtr++;
                }
                numPtr += num5;
                num6++;
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

        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get
            {
                return this.formatTranslations;
            }
        }

        public int MaxHoleHeight
        {
            get
            {
                return this.maxHoleHeight;
            }
            set
            {
                this.maxHoleHeight = Math.Max(value, 0);
            }
        }

        public int MaxHoleWidth
        {
            get
            {
                return this.maxHoleWidth;
            }
            set
            {
                this.maxHoleWidth = Math.Max(value, 0);
            }
        }
    }
}

