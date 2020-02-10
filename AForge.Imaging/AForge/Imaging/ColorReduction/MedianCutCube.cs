namespace AForge.Imaging.ColorReduction
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Runtime.InteropServices;

    internal class MedianCutCube
    {
        private List<System.Drawing.Color> colors;
        private System.Drawing.Color? cubeColor = null;
        private readonly byte maxB;
        private readonly byte maxG;
        private readonly byte maxR;
        private readonly byte minB;
        private readonly byte minG;
        private readonly byte minR;

        public MedianCutCube(List<System.Drawing.Color> colors)
        {
            this.colors = colors;
            this.minR = this.minG = (byte) (this.minB = 0xff);
            this.maxR = this.maxG = (byte) (this.maxB = 0);
            foreach (System.Drawing.Color color in colors)
            {
                if (color.R < this.minR)
                {
                    this.minR = color.R;
                }
                if (color.R > this.maxR)
                {
                    this.maxR = color.R;
                }
                if (color.G < this.minG)
                {
                    this.minG = color.G;
                }
                if (color.G > this.maxG)
                {
                    this.maxG = color.G;
                }
                if (color.B < this.minB)
                {
                    this.minB = color.B;
                }
                if (color.B > this.maxB)
                {
                    this.maxB = color.B;
                }
            }
        }

        public void SplitAtMedian(int rgbComponent, out MedianCutCube cube1, out MedianCutCube cube2)
        {
            switch (rgbComponent)
            {
                case 0:
                    this.colors.Sort(new BlueComparer());
                    break;

                case 1:
                    this.colors.Sort(new GreenComparer());
                    break;

                case 2:
                    this.colors.Sort(new RedComparer());
                    break;
            }
            int count = this.colors.Count / 2;
            cube1 = new MedianCutCube(this.colors.GetRange(0, count));
            cube2 = new MedianCutCube(this.colors.GetRange(count, this.colors.Count - count));
        }

        public int BlueSize
        {
            get
            {
                return (this.maxB - this.minB);
            }
        }

        public System.Drawing.Color Color
        {
            get
            {
                if (!this.cubeColor.HasValue)
                {
                    int red = 0;
                    int green = 0;
                    int blue = 0;
                    foreach (System.Drawing.Color color in this.colors)
                    {
                        red += color.R;
                        green += color.G;
                        blue += color.B;
                    }
                    int count = this.colors.Count;
                    if (count != 0)
                    {
                        red /= count;
                        green /= count;
                        blue /= count;
                    }
                    this.cubeColor = new System.Drawing.Color?(System.Drawing.Color.FromArgb(red, green, blue));
                }
                return this.cubeColor.Value;
            }
        }

        public int GreenSize
        {
            get
            {
                return (this.maxG - this.minG);
            }
        }

        public int RedSize
        {
            get
            {
                return (this.maxR - this.minR);
            }
        }

        private class BlueComparer : IComparer<Color>
        {
            public int Compare(Color c1, Color c2)
            {
                return c1.B.CompareTo(c2.B);
            }
        }

        private class GreenComparer : IComparer<Color>
        {
            public int Compare(Color c1, Color c2)
            {
                return c1.G.CompareTo(c2.G);
            }
        }

        private class RedComparer : IComparer<Color>
        {
            public int Compare(Color c1, Color c2)
            {
                return c1.R.CompareTo(c2.R);
            }
        }
    }
}

