using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

public class RayTracer
{
    public class Ray
    {
        public Vector3 origin;
        public Vector3 Direction
        {
            get { return _direction; }
            set { _direction = Vector3.Normalize(value); }
        }
        private Vector3 _direction;

        private void Normalize()
        {
            if (Vector3.DistanceSquared(Vector3.Zero, _direction) != 1f) _direction = Vector3.Normalize(_direction);
        }

        public Ray () { origin = Vector3.Zero; _direction = Vector3.Zero; }
        public Ray (Vector3 o, Vector3 d)
        {
            origin = o;
            _direction = Vector3.Normalize(d);
        }

        public bool FindSphereIntersection (Vector3 center, float radius, out Vector3 intersection)
        {
            intersection = new Vector3();
            if (_direction == Vector3.Zero) return false;
            Normalize();

            float B = 2 * (Vector3.Dot(origin, _direction) - Vector3.Dot(center, _direction));
            float C = Vector3.DistanceSquared(center, origin) - MathF.Pow(radius, 2f);
            float delta = MathF.Pow(B, 2f) - 4 * C;

            if (delta < 0f) return false;

            float t = MathF.Min ((-B + MathF.Sqrt(delta)) / 2f, (-B - MathF.Sqrt(delta)) / 2f);
            intersection = origin + _direction * t;

            return true;
        }
    }
}
