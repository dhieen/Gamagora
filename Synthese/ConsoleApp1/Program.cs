using System;
using System.Numerics;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Intersection
{
    class MyApplication
    {        
        [STAThread]
        public static void Main()
        {
            CircleMesh circle = new CircleMesh(new OpenTK.Vector3(0f, 0f, 0f), .5f, 3f);

            using (var game = new GameWindow(800, 800))
            {                
                game.Load += (sender, e) =>
                {
                    // setup settings, load textures, sounds
                    game.VSync = VSyncMode.On;
                };

                game.Resize += (sender, e) =>
                {
                    GL.Viewport(0, 0, game.Width, game.Height);
                };

                game.UpdateFrame += (sender, e) =>
                {
                    // add game logic, input handling
                    if (game.Keyboard[Key.Escape])
                    {
                        game.Exit();
                    }
                };

                game.RenderFrame += (sender, e) =>
                {
                    // render graphics
                    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                    GL.MatrixMode(MatrixMode.Projection);
                    GL.LoadIdentity();
                    GL.Ortho(-1.0, 1.0, -1.0, 1.0, 0.0, 4.0);

                    GL.Begin(PrimitiveType.LineLoop);

                    GL.Color3(OpenTK.Color.White);
                    foreach (OpenTK.Vector3 v in circle.vectrices)
                        GL.Vertex2(v.X, v.Y);

                    GL.End();

                    game.SwapBuffers();
                };

                // Run the game at 60 updates per second
                game.Run(60.0);
            }
        }
    }

    /*
    class Program
    {
        static void Main(string[] args)
        {
            RayTracer.Ray ray = new RayTracer.Ray(Vector3.Zero, Vector3.UnitX);
            Vector3 intersection = new Vector3();
            int isIntersection = ray.FindSphereIntersection(10f * Vector3.UnitX, 5f, out intersection);

            switch (isIntersection)
            {
                case -1:
                    Console.WriteLine("Ray Error");
                    break;
                case 0:
                    Console.WriteLine("No intersection");
                    break;
                case 1:
                    Console.WriteLine("Intersection: " + intersection.ToString());
                    break;
            }

            Console.ReadKey();
        }
    }
    */
}
