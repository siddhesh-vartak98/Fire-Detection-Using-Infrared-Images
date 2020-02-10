namespace AForge.Imaging
{
    using System;

    internal static class Interpolation
    {
        public static double BiCubicKernel(double x)
        {
            if (x < 0.0)
            {
                x = -x;
            }
            double num = 0.0;
            if (x <= 1.0)
            {
                return (((((1.5 * x) - 2.5) * x) * x) + 1.0);
            }
            if (x < 2.0)
            {
                num = (((((-0.5 * x) + 2.5) * x) - 4.0) * x) + 2.0;
            }
            return num;
        }
    }
}

