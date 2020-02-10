namespace AForge.Imaging
{
    using AForge;
    using System;
    using System.ComponentModel;
    using System.Drawing;

    public class Blob
    {
        private int area;
        private IntPoint cog;
        private Color colorMean;
        private Color colorStdDev;
        private double fullness;
        private int id;
        private UnmanagedImage image;
        private bool originalSize;
        private System.Drawing.Rectangle rect;

        internal Blob(Blob source)
        {
            this.colorMean = Color.Black;
            this.colorStdDev = Color.Black;
            this.id = source.id;
            this.rect = source.rect;
            this.cog = source.cog;
            this.area = source.area;
            this.fullness = source.fullness;
            this.colorMean = source.colorMean;
            this.colorStdDev = source.colorStdDev;
        }

        internal Blob(int id, System.Drawing.Rectangle rect)
        {
            this.colorMean = Color.Black;
            this.colorStdDev = Color.Black;
            this.id = id;
            this.rect = rect;
        }

        public int Area
        {
            get
            {
                return this.area;
            }
            internal set
            {
                this.area = value;
            }
        }

        public IntPoint CenterOfGravity
        {
            get
            {
                return this.cog;
            }
            internal set
            {
                this.cog = value;
            }
        }

        public Color ColorMean
        {
            get
            {
                return this.colorMean;
            }
            internal set
            {
                this.colorMean = value;
            }
        }

        public Color ColorStdDev
        {
            get
            {
                return this.colorStdDev;
            }
            internal set
            {
                this.colorStdDev = value;
            }
        }

        public double Fullness
        {
            get
            {
                return this.fullness;
            }
            internal set
            {
                this.fullness = value;
            }
        }

        [Browsable(false)]
        public int ID
        {
            get
            {
                return this.id;
            }
            internal set
            {
                this.id = value;
            }
        }

        [Browsable(false)]
        public UnmanagedImage Image
        {
            get
            {
                return this.image;
            }
            internal set
            {
                this.image = value;
            }
        }

        [Browsable(false)]
        public bool OriginalSize
        {
            get
            {
                return this.originalSize;
            }
            internal set
            {
                this.originalSize = value;
            }
        }

        public System.Drawing.Rectangle Rectangle
        {
            get
            {
                return this.rect;
            }
        }
    }
}

