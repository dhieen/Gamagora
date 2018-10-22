using System;
using System.Collections.Generic;
using System.Numerics;
using System.Drawing;

namespace TPSynthese
{
    
    class Program
    {        
        static void Main(string[] args)
        {
            List<Sphere> spheres = new List<Sphere>();
            spheres.Add(new Sphere(.2, new Vector3Double(0, 0, 0), Color.White, 40f));
            spheres.Add(new Sphere(.2, new Vector3Double(4, 2, 0), Color.Yellow, 20f));
            spheres.Add(new Sphere(.2, new Vector3Double(-4, -2, 0), Color.Blue, 20f));
                        
            spheres.Add(new Sphere(1e3, new Vector3Double(1e3 + 8, 0, 0), Color.Tomato));
            spheres.Add(new Sphere(1e3, new Vector3Double(-1e3 - 8, 0, 0), Color.Thistle));
            spheres.Add(new Sphere(1e3, new Vector3Double(0, 1e3 + 5, 0), Color.Yellow));
            spheres.Add(new Sphere(1e3, new Vector3Double(0, -1e3 - 5, 0), Color.SkyBlue));
            spheres.Add(new Sphere(1e3, new Vector3Double(0, 0, 1e3 + 5), Color.Firebrick));
            spheres.Add(new Sphere(1e3, new Vector3Double(0, 0, -1e3 - 5), Color.White, 0f, 1f));

            spheres.Add(new Sphere(2, new Vector3Double(4, -2, 3), Color.CadetBlue, 0f, 1f));
            spheres.Add(new Sphere(2, new Vector3Double(-4, 2, -3), Color.CadetBlue));
            spheres.Add(new Sphere(1, new Vector3Double(-2, -4, 1), Color.BurlyWood, 0f, 1f));

            Camera camera = new Camera();
            camera.position = new Vector3Double(-3, 0, -10);
            camera.size = 1;
            camera.perspective = 0.002;
            camera.screen = new PPMImage(800, 600);
            camera.ambiantLight = Color.FromArgb(50, 50, 50) ;
            camera.backgroundColor = Color.Green;
            camera.bright = .8f;
            camera.dark = .0f;

            camera.PrintScene(spheres);

            string folder = "D:/TPOutputs";
            camera.screen.SaveFile(folder + "/Image.ppm");
        }
    }
}
