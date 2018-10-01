using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace TPSynthese
{
    public class PPMImage
    {
        public int width;
        public int height;
        public List<Color> pxmap;

        public PPMImage(int w, int h)
        {
            width = w;
            height = h;
            pxmap = new List<Color>(w * h);

            for (int i = 0; i < w * h; i++) pxmap.Add(Color.White);
        }

        public Vector2 PXIndex2Coordinates (int index)
        {
            if (index < 0 || index > pxmap.Count) return -Vector2.One;

            return new Vector2(index % width, index / width);
        }

        public void SetPX(int x, int y, Color c)
        {
            int i = y * width + x;
            if (i < 0 || i > pxmap.Count) return;

            pxmap[i] = c;
        }

        public string[] ToStrings()
        {
            List<string> content = new List<string>();

            content.Add("P3");
            content.Add(" " + width.ToString() + " " + height.ToString());
            content.Add("255");

            for (int y = 0; y < height; y++)
            {
                string line = "";

                for (int x = 0; x < width; x++)
                {
                    int i = y * width + x;
                    line = line
                        + pxmap[i].R.ToString() + " "
                        + pxmap[i].G.ToString() + " "
                        + pxmap[i].B.ToString() + "   ";
                }

                content.Add(line);
            }

            return content.ToArray();
        }

        public void SaveFile(string path)
        {
            File.WriteAllLines(path, this.ToStrings());
        }
    }
}
