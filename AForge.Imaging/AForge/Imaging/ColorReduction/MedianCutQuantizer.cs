namespace AForge.Imaging.ColorReduction
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    public class MedianCutQuantizer : IColorQuantizer
    {
        private List<Color> colors = new List<Color>();

        public void AddColor(Color color)
        {
            this.colors.Add(color);
        }

        public void Clear()
        {
            this.colors.Clear();
        }

        public Color[] GetPalette(int colorCount)
        {
            List<MedianCutCube> cubes = new List<MedianCutCube> {
                new MedianCutCube(this.colors)
            };
            this.SplitCubes(cubes, colorCount);
            Color[] colorArray = new Color[colorCount];
            for (int i = 0; i < colorCount; i++)
            {
                colorArray[i] = cubes[i].Color;
            }
            return colorArray;
        }

        private void SplitCubes(List<MedianCutCube> cubes, int count)
        {
            int index = cubes.Count - 1;
            while (cubes.Count < count)
            {
                MedianCutCube cube2;
                MedianCutCube cube3;
                MedianCutCube cube = cubes[index];
                if ((cube.RedSize >= cube.GreenSize) && (cube.RedSize >= cube.BlueSize))
                {
                    cube.SplitAtMedian(2, out cube2, out cube3);
                }
                else if (cube.GreenSize >= cube.BlueSize)
                {
                    cube.SplitAtMedian(1, out cube2, out cube3);
                }
                else
                {
                    cube.SplitAtMedian(0, out cube2, out cube3);
                }
                cubes.RemoveAt(index);
                cubes.Insert(index, cube2);
                cubes.Insert(index, cube3);
                if (--index < 0)
                {
                    index = cubes.Count - 1;
                }
            }
        }
    }
}

