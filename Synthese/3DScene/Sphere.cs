using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using System.Drawing;

namespace TPSynthese
{
    public class Sphere
    {
        public float radius;
        public Vector3 center;
        public Color color;
        public float lightSource;
        public float reflection;

        public Sphere() { }
        public Sphere(float r, Vector3 c) { radius = r; center = c; }
        public Sphere(float r, Vector3 c,  Color col) { radius = r; center = c; color = col; }
        public Sphere(float r, Vector3 c, Color col, float ls) { radius = r; center = c; color = col; lightSource = ls; }
        public Sphere(float r, Vector3 c, Color col, float ls, float rflx) { radius = r; center = c; color = col; lightSource = ls; reflection = rflx; }

        public Vector3 NormalOn (Vector3 onPoint)
        {
            return Vector3.Normalize(onPoint - center);
        }
    }
}
