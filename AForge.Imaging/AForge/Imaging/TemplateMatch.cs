namespace AForge.Imaging
{
    using System;
    using System.Drawing;

    public class TemplateMatch
    {
        private System.Drawing.Rectangle rect;
        private float similarity;

        public TemplateMatch(System.Drawing.Rectangle rect, float similarity)
        {
            this.rect = rect;
            this.similarity = similarity;
        }

        public System.Drawing.Rectangle Rectangle
        {
            get
            {
                return this.rect;
            }
        }

        public float Similarity
        {
            get
            {
                return this.similarity;
            }
        }
    }
}

