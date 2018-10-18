using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using System.Drawing;

namespace TPSynthese
{
    public class Sphere
    {
        public double radius;
        public Vector3Double center;
        public Color color;
        public float lightSource;
        public float reflection;

        public Sphere() { }
        public Sphere(double r, Vector3Double c) { radius = r; center = c; }
        public Sphere(double r, Vector3Double c,  Color col) { radius = r; center = c; color = col; }
        public Sphere(double r, Vector3Double c, Color col, float ls) { radius = r; center = c; color = col; lightSource = ls; }
        public Sphere(double r, Vector3Double c, Color col, float ls, float rflx) { radius = r; center = c; color = col; lightSource = ls; reflection = rflx; }

        public Vector3Double NormalOn (Vector3Double onPoint)
        {
            return Vector3Double.Normalize(onPoint - center);
        }
    }
}