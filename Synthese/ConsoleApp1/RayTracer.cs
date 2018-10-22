using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

public class RayTracer
{
    public class Ray
    {
        public Vector3Double origin;
        public Vector3Double Direction
        {
            get { return _direction; }
            set { _direction = Vector3Double.Normalize(value); }
        }
        private Vector3Double _direction;

        private void Normalize()
        {
            if (Vector3Double.DistanceSquared(Vector3Double.Zero, _direction) != 1f) _direction = Vector3Double.Normalize(_direction);
        }

        public Ray () { origin = Vector3Double.Zero; _direction = Vector3Double.Zero; }
        public Ray (Vector3Double o, Vector3Double d)
        {
            origin = o;
            _direction = Vector3Double.Normalize(d);
        }

        public bool FindSphereIntersection (Vector3Double center, double radius, out Vector3Double intersection)
        {
            intersection = new Vector3Double();
            if (_direction == Vector3Double.Zero) return false;
            Normalize();

            double B = 2 * (Vector3Double.Dot(origin, _direction) - Vector3Double.Dot(center, _direction));
            double C = Vector3Double.DistanceSquared(center, origin) - Math.Pow(radius, 2);
            double delta = Math.Pow(B, 2) - 4 * C;

            if (delta < 0f) return false;

            double t = (-B - Math.Sqrt(delta)) / 2;
            if (t < 0)
                t = (-B + Math.Sqrt(delta)) / 2;
            if (t < 0) return false;

            intersection = origin + _direction * t;

            return true;
        }

        public void RandomizeDirection (Vector3Double around, double angleRange)
        {
            Vector3Double u = (around.Y != 0 && around.Z != 0) ? new Vector3Double(1, 0, 0) : new Vector3Double(0, 1, 0);
            Vector3Double axis1 = Vector3Double.Cross(around, u);
            Vector3Double axis2 = around;

            double angle1 = new Random().NextDouble() * angleRange;
            double angle2 = new Random().NextDouble() * Math.PI * 2;

            _direction = _direction.Rotated(axis1, angle1);
            _direction = _direction.Rotated(axis2, angle2);
        }
    }
}
