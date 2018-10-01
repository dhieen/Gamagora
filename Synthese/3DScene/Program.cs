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
            spheres.Add(new Sphere(.5f, new Vector3(5f, 15f, -10f), Color.Yellow, 1000f));
            spheres.Add(new Sphere(.5f, new Vector3(-5f, 15f, -10f), Color.OrangeRed, 1000f));
            spheres.Add(new Sphere(.5f, new Vector3(0f, 25f, -10f), Color.DeepSkyBlue, 1000f));
            spheres.Add(new Sphere(1f, new Vector3(0f, 5f, -10f), Color.ForestGreen, 0f));
            spheres.Add(new Sphere(5f, new Vector3(3f, -5f, -8f), Color.Tomato, 0f));
            spheres.Add(new Sphere(10f, new Vector3(-15f, 0f, 0f), Color.Purple, 0f, 1f));
            spheres.Add(new Sphere(20f, new Vector3(0f, -25f, 0f), Color.Beige, 0f));
            spheres.Add(new Sphere(50f, new Vector3(50f, 0f, 20f), Color.DodgerBlue, 0f, 1f));
            spheres.Add(new Sphere(120f, new Vector3(-100f, 0f, 120f), Color.BurlyWood, 0f));

            Camera camera = new Camera();
            camera.position = new Vector3(0f, 0f, -5f);
            camera.size = .1f;
            camera.perspective = .02f;
            camera.screen = new PPMImage(800, 800);
            camera.ambiantLight = Color.FromArgb(0, 0, 0) ;
            camera.backgroundColor = Color.DarkGray;
            camera.bright = .4f;
            camera.dark = 0f;

            camera.PrintScene(spheres);

            string folder = "D:/TPOutputs";
            camera.screen.SaveFile(folder + "/Image.ppm");
        }
    }
}
