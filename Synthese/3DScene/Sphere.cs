using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using System.Drawing;

namespace TPSynthese
{
    public class Sphere : iSceneObject
    {
        private double radius;
        private Vector3Double center;

        public Color Color { get { return color; } }
        public float LightSource { get { return lightSource; } }
        public float Reflection { get { return reflection; } }

        private Color color;
        private float lightSource;
        private float reflection;

        public Sphere() { }
        public Sphere(double r, Vector3Double c) { radius = r; center = c; color = Color.Green; lightSource = -1f; }
        public Sphere(double r, Vector3Double c, Color col) { radius = r; center = c; color = col; lightSource = -1f; }
        public Sphere(double r, Vector3Double c, Color col, float ls) { radius = r; center = c; color = col; lightSource = ls; }
        public Sphere(double r, Vector3Double c, Color col, float ls, float rflx) { radius = r; center = c; color = col; lightSource = ls; reflection = rflx; }

        public Vector3Double Center {get{ return center;} }
        public Box BoundingBox
        {
            get
            {
                return new Box(center - new Vector3Double(radius, radius, radius), center + new Vector3Double(radius, radius, radius));
            }
        }
        public float Size
        {
            get
            {
                return 2f * (float)radius;
            }
        }
        public Vector3Double NormalOn (Vector3Double onPoint)
        {
            return Vector3Double.Normalize(onPoint - center);
        }

        public bool FindRayIntersection(RayTracer.Ray ray, out Vector3Double intersection)
        {
            intersection = new Vector3Double();
            if (ray.Direction == Vector3Double.Zero) return false;

            double B = 2 * (Vector3Double.Dot(ray.origin, ray.Direction) - Vector3Double.Dot(center, ray.Direction));
            double C = Vector3Double.DistanceSquared(center, ray.origin) - Math.Pow(radius, 2);
            double delta = Math.Pow(B, 2) - 4 * C;

            if (delta < 0f) return false;

            double t = (-B - Math.Sqrt(delta)) / 2;
            if (t < 0)
                t = (-B + Math.Sqrt(delta)) / 2;
            if (t < 0) return false;

            intersection = ray.origin + ray.Direction * t;

            return true;
        }
    }
}