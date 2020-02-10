namespace AForge.Imaging
{
    using AForge;
    using System;

    public class BlockMatch
    {
        private IntPoint matchPoint;
        private float similarity;
        private IntPoint sourcePoint;

        public BlockMatch(IntPoint sourcePoint, IntPoint matchPoint, float similarity)
        {
            this.sourcePoint = sourcePoint;
            this.matchPoint = matchPoint;
            this.similarity = similarity;
        }

        public IntPoint MatchPoint
        {
            get
            {
                return this.matchPoint;
            }
        }

        public float Similarity
        {
            get
            {
                return this.similarity;
            }
        }

        public IntPoint SourcePoint
        {
            get
            {
                return this.sourcePoint;
            }
        }
    }
}

