using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using System.Drawing;

namespace TPSynthese
{
    public class Box : iSceneObject
    {
        public Vector3Double minPoint;
        public Vector3Double maxPoint;

        public Color Color { get { return color; } }
        public float LightSource { get { return lightSource; } }
        public float Reflection { get { return reflection; } }

        private Color color;
        private float lightSource;
        private float reflection;
        public Box() { }
        public Box(Vector3Double min, Vector3Double max) { minPoint = min; maxPoint = max; color = Color.Green; lightSource = -1f; }
        public Box(Vector3Double min, Vector3Double max, Color col) { minPoint = min; maxPoint = max; color = col; lightSource = -1f; }
        public Box(Vector3Double min, Vector3Double max, Color col, float ls) { minPoint = min; maxPoint = max; color = col; lightSource = ls; }
        public Box(Vector3Double min, Vector3Double max, Color col, float ls, float rflx) { minPoint = min; maxPoint = max; color = col; lightSource = ls; reflection = rflx; }

        public Vector3Double Center
        {
            get
            {
                return maxPoint + minPoint / 2;
            }
        }

        public Box BoundingBox
        {
            get
            {
                return new Box (minPoint, maxPoint);
            }
        }
        public float Size
        {
            get
            {
                Vector3Double dpt = maxPoint - minPoint;
                return (float)Math.Max(dpt.X, Math.Max(dpt.Y, dpt.Z));
            }
        }
        public Vector3Double NormalOn(Vector3Double onPoint)
        {
            if ((onPoint.X > minPoint.X && onPoint.X < maxPoint.X && onPoint.Z > minPoint.Z && onPoint.Z < maxPoint.Z)
                || (onPoint.Y > minPoint.Y && onPoint.Y < maxPoint.Y && onPoint.Z > minPoint.Z && onPoint.Z < maxPoint.Z)
                || (onPoint.X > minPoint.X && onPoint.X < maxPoint.X && onPoint.Y > minPoint.Y && onPoint.Y < maxPoint.Y))
            {
                if (onPoint.X >= maxPoint.X) return new Vector3Double(1, 0, 0);
                if (onPoint.X <= minPoint.X) return new Vector3Double(-1, 0, 0);

                if (onPoint.Y >= maxPoint.Y) return new Vector3Double(0, 1, 0);
                if (onPoint.Y <= minPoint.Y) return new Vector3Double(0, -1, 0);

                if (onPoint.Z >= maxPoint.Z) return new Vector3Double(0, 0, 1);
                if (onPoint.Z <= minPoint.Z) return new Vector3Double(0, 0, -1);
            }

            return null;
        }
        public bool FindRayIntersection(RayTracer.Ray ray, out Vector3Double intersection)
        {
            intersection = new Vector3Double();
            if (ray.Direction == Vector3Double.Zero) return false;

            double txmin = (minPoint.X - ray.origin.X) / ray.Direction.X;
            double txmax = (maxPoint.X - ray.origin.X) / ray.Direction.X;

            if (txmin > txmax)
            {
                double s = txmin;
                txmin = txmax;
                txmax = s;
            }

            double tymin = (minPoint.Y - ray.origin.Y) / ray.Direction.Y;
            double tymax = (maxPoint.Y - ray.origin.Y) / ray.Direction.Y;

            if (tymin > tymax)
            {
                double s = tymin;
                tymin = tymax;
                tymax = s;
            }

            if ((txmin > tymax) || (tymin > txmax))
                return false;

            if (tymin > txmin)
                txmin = tymin;

            if (tymax < txmax)
                txmax = tymax;

            double tzmin = (minPoint.Z - ray.origin.Z) / ray.Direction.Z;
            double tzmax = (maxPoint.Z - ray.origin.Z) / ray.Direction.Z;

            if (tzmin > tzmax)
            {
                double s = tzmin;
                tzmin = tzmax;
                tzmax = s;
            }

            if ((txmin > tzmax) || (tzmin > txmax))
                return false;

            if (tzmin > txmin)
                txmin = tzmin;

            if (tzmax < txmax)
                txmax = tzmax;

            intersection = ray.origin + txmin * ray.Direction;
            return true;
        }
    }
}
