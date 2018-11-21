using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using System.Drawing;

namespace TPSynthese
{
    public interface iSceneObject
    {
        Color Color { get; }
        float LightSource { get; }
        float Reflection { get; }
        Vector3Double Center { get; }
        Box BoundingBox { get; }

        Vector3Double NormalOn(Vector3Double onPoint);
        bool FindRayIntersection(RayTracer.Ray ray, out Vector3Double intersection);       
        float Size { get; }
    }
}
