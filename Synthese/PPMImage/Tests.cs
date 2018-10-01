using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace TPSynthese
{
    class Tests
    {
        static public PPMImage DrawExample1(int w, int h)
        {
            PPMImage pic = new PPMImage(w, h);

            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    int r = 255 * (int)MathF.Abs(x - w / 2) / (w / 2);
                    int g = 255 - 255 * (int)MathF.Abs(x - w / 2) / (w / 2);
                    int b = 255 * (int)MathF.Abs(y - h / 2) / (h / 2);

                    Color c = Color.FromArgb(r % 255, g % 255, b % 255);

                    pic.SetPX(x, y, c);
                }                
            }

            return pic;
        }

        static public PPMImage DrawExample2(int w, int h, int a)
        {
            PPMImage pic = new PPMImage(w, h);

            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    int d = (int)((MathF.Pow(x - w / 2, 2) + MathF.Pow(y - h / 2, 2)));

                    Color c = ((d / a) % 2 == 0) ? Color.DarkKhaki : Color.LightGoldenrodYellow;

                    pic.SetPX(x, y, c);
                }
            }

            return pic;
        }
    }
}
