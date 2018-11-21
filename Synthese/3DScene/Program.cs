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
            List<iSceneObject> scene = new List<iSceneObject>();

            // Lumières
            scene.Add(new Sphere(.2, new Vector3Double(0, 0, 0), Color.White, 40f));
            scene.Add(new Sphere(.2, new Vector3Double(4, 2, -4), Color.Yellow, 20f));
            scene.Add(new Sphere(.2, new Vector3Double(-4, -2, 4), Color.Blue, 20f));

            // Murs
            scene.Add(new Sphere(1e3, new Vector3Double(1e3 + 8, 0, 0), Color.Tomato));
            scene.Add(new Sphere(1e3, new Vector3Double(-1e3 - 8, 0, 0), Color.Thistle));
            scene.Add(new Sphere(1e3, new Vector3Double(0, 1e3 + 5, 0), Color.Yellow));
            scene.Add(new Sphere(1e3, new Vector3Double(0, -1e3 - 5, 0), Color.SkyBlue));
            scene.Add(new Sphere(1e3, new Vector3Double(0, 0, 1e3 + 5), Color.Firebrick));
            //scene.Add(new Sphere(1e3, new Vector3Double(0, 0, -1e3 - 5), Color.White));
            
            // Spheres
            /*
            scene.Add(new Sphere(2, new Vector3Double(4, -2, 3), Color.CadetBlue));
            scene.Add(new Sphere(2, new Vector3Double(-4, 2, -3), Color.CadetBlue));
            scene.Add(new Sphere(1, new Vector3Double(-2, -4, 1), Color.BurlyWood));
            scene.Add(new Sphere(2, new Vector3Double(4, -2, 3), Color.CadetBlue));
            scene.Add(new Sphere(2, new Vector3Double(-4, 2, -3), Color.CadetBlue));
            scene.Add(new Sphere(1, new Vector3Double(-2, -4, 1), Color.BurlyWood));
            */

            // Plein de sphères
            double r = 0.2;
            Vector3Double pos = new Vector3Double(-7, 0, 2);

            for (int x = 0; x < 35; x++)
            for (int y = 0; y < 4; y++)
            for (int z = 0; z < 4; z++)
            {
                scene.Add(new Sphere(r, pos + new Vector3Double(x * 2*r, y * 2*r + 2 * Math.Cos ((double)x/2), z * 2*r + Math.Cos((double)x/2)), RandomColor()));
            }

            // Cubes
            scene.Add(new Box(new Vector3Double(2, -4, -4), new Vector3Double(5, -1, -1), Color.Aqua));
            scene.Add(new Box(new Vector3Double(0, -6, -2), new Vector3Double(1, -4, -1), Color.Crimson));
            scene.Add(new Box(new Vector3Double(-4, -6, -4), new Vector3Double(0, -4, -2), Color.Purple));

            Camera camera = new Camera();
            camera.position = new Vector3Double(-3, 0, -10);
            camera.size = 1;
            camera.perspective = 0.002;
            camera.screen = new PPMImage(800, 600);
            camera.ambiantLight = Color.FromArgb(50, 50, 50) ;
            camera.backgroundColor = Color.Green;
            camera.bright = .8f;
            camera.dark = .0f;

            
            camera.PrintScene(scene);
            string folder = "D:/TPOutputs";
            camera.screen.SaveFile(folder + "/Image.ppm");
            
            /*
            BoxTree bt = new BoxTree(10, scene);
            BoxTree.BoundingBox bb = bt.topBox;
            int cpt = 0;
            DisplayByBox(camera, bb, ref cpt);
            */
        }

        static void DisplayByBox (Camera cam, BoxTree.BoundingBox bb, ref int cpt)
        {
            if (bb is BoxTree.TipBoundingBox)
            {
                cam.PrintScene((bb as BoxTree.TipBoundingBox).content);
                string folder = "D:/TPOutputs";
                cam.screen.SaveFile(folder + "/Boxed" + cpt.ToString() + ".ppm");
            }
            if (bb is BoxTree.NodeBoundingBox)
            {
                foreach (BoxTree.BoundingBox ub in (bb as BoxTree.NodeBoundingBox).underBoxes)
                {
                    cpt++;
                    DisplayByBox(cam, ub, ref cpt);
                }
            }
        }

        static Color RandomColor ()
        {
            Random random = new Random();
            return Color.FromArgb(random.Next() % 255, random.Next() % 255, random.Next() % 255);
        }
    }
}
