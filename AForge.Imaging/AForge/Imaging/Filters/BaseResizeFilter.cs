namespace AForge.Imaging.Filters
{
    using AForge.Imaging;
    using System;
    using System.Drawing;

    public abstract class BaseResizeFilter : BaseTransformationFilter
    {
        protected int newHeight;
        protected int newWidth;

        protected BaseResizeFilter(int newWidth, int newHeight)
        {
            this.newWidth = newWidth;
            this.newHeight = newHeight;
        }

        protected override Size CalculateNewImageSize(UnmanagedImage sourceData)
        {
            return new Size(this.newWidth, this.newHeight);
        }

        public int NewHeight
        {
            get
            {
                return this.newHeight;
            }
            set
            {
                this.newHeight = Math.Max(1, value);
            }
        }

        public int NewWidth
        {
            get
            {
                return this.newWidth;
            }
            set
            {
                this.newWidth = Math.Max(1, value);
            }
        }
    }
}

