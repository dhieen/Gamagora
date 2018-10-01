using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Intersection
{
    class GeometryMesh
    {
        public List<Vector3> vectrices;
        public GeometryMesh() { vectrices = new List<Vector3>(); }
    }

    class CircleMesh : GeometryMesh
    {
        public Vector3 center;
        public float radius;

        public CircleMesh () { vectrices = new List<Vector3>(); }
        public CircleMesh (Vector3 c, float r, float smoothness)
        {
            center = c;
            radius = r;

            float step = MathF.Pow (radius, 2f) / smoothness;
            int nSteps = (int) (2*MathF.PI / step);
            step = 2*MathF.PI / (float) nSteps;

            for (int a = 0; a < nSteps; a++)
            {
                vectrices.Add(center + radius * new Vector3(MathF.Cos(a * step), MathF.Sin(a * step), 0f));
            }
        }
    }
}
