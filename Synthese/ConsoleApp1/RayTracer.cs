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

        public Ray () { origin = Vector3Double.Zero; _direction = Vector3Double.Zero; }
        public Ray (Vector3Double o, Vector3Double d)
        {
            origin = o;
            _direction = Vector3Double.Normalize(d);
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
