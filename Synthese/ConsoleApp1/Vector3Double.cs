using System;
using System.Collections.Generic;
using System.Text;

public class Vector3Double
{
    public double X;
    public double Y;
    public double Z;

    public Vector3Double() { }
    public Vector3Double(double x, double y, double z) { X = x; Y = y; Z = z; }

    public static Vector3Double Zero { get { return new Vector3Double(0, 0, 0); } }

    public static Vector3Double operator- (Vector3Double v)
    {
        return new Vector3Double(-v.X, -v.Y, -v.Z);
    }
    public static Vector3Double operator +(Vector3Double a, Vector3Double b)
    {
        return new Vector3Double(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    }

    public static Vector3Double operator -(Vector3Double a, Vector3Double b)
    {
        return new Vector3Double(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    }
    public static Vector3Double operator *(Vector3Double a, double b)
    {
        return new Vector3Double(a.X * b, a.Y * b, a.Z * b);
    }
    public static Vector3Double operator *(double b, Vector3Double a)
    {
        return new Vector3Double(a.X * b, a.Y * b, a.Z * b);
    }
    public static Vector3Double operator /(Vector3Double a, double b)
    {
        return new Vector3Double(a.X / b, a.Y / b, a.Z / b);
    }

    public double magnitude { get { return Distance (this, Zero); } }
    public static Vector3Double Normalize (Vector3Double v)
    {
        if (v == Zero) return Zero;
        return v / v.magnitude;
    }

    public static double DistanceSquared (Vector3Double a, Vector3Double b)
    {
        return Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2) + Math.Pow(a.Z - b.Z, 2);
    }
    public static double Distance(Vector3Double a, Vector3Double b)
    {
        return Math.Sqrt (DistanceSquared(a,b));
    }

    public static double Dot (Vector3Double a, Vector3Double b)
    {
        return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
    }

    public static Vector3Double Cross (Vector3Double a, Vector3Double b)
    {
        return new Vector3Double(a.Y * b.Z - a.Z * b.Y, a.Z * b.X - a.X * b.Z, a.X * b.Y - a.Y * b.X);
    }

    public Vector3Double Rotated (Vector3Double axis, double angle)
    {
        Vector3Double r = new Vector3Double();
        Vector3Double ax = Normalize(axis);
        double c = Math.Cos(angle);
        double s = Math.Sin(angle);

        r.X = (ax.X * ax.X * (1 - c) + c) * X 
            + (ax.X * ax.Y * (1 - c) - ax.Z * s) * Y 
            + (ax.X * ax.Z * (1 - c) + ax.Y * s) * Z;
        r.Y = (ax.X * ax.Y * (1 - c) + ax.Z * s) * X 
            + (ax.Y * ax.Y * (1 - c) + c) * Y 
            + (ax.Y * ax.Z * (1 - c) - ax.X * s) * Z;
        r.Z = (ax.X * ax.Z * (1 - c) - ax.Y * s) * X 
            + (ax.Y * ax.Z * (1 - c) + ax.X * s) * Y 
            + (ax.Z * ax.Z * (1 - c) + c) * Z;

        return r;
    }

    public override string ToString()
    {
        return "(" + X.ToString() +"; "+ Y.ToString() + ";" + Z.ToString() + ")";
    }
}