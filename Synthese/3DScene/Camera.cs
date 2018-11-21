using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using System.Drawing;
using TPSynthese;

namespace TPSynthese
{
    class Camera
    {
        public Vector3Double position;
        public Vector3Double rotation;
        public PPMImage screen;
        public double size;
        public Color backgroundColor;
        public Color ambiantLight;
        public double perspective;
        public float dark;
        public float bright;
        public int maxReflectionIterations = 10;

        private double lightSmoothCorrector = .2;

        public void PrintScene (List<iSceneObject> scene, List<Box> boxes = null)
        {
            screen.Clear(backgroundColor);

            for (int i =  0; i < screen.pxmap.Count; i++)
            {
                int reflexionIterations = 0;
                bool rayHit = false;
                iSceneObject rayHitObject = null;
                Vector3Double rayHitPoint = new Vector3Double();
                Vector3Double rayDirection = new Vector3Double();

                ThrowRayFromPx(screen.PXIndex2Coordinates(i), scene, out rayHit, out rayHitPoint, out rayHitObject, out rayDirection);
                
                while (rayHit && rayHitObject.Reflection > 0f && reflexionIterations++ < maxReflectionIterations)
                {
                    Vector3Double normal = rayHitObject.NormalOn(rayHitPoint);
                    rayDirection = 2 * Vector3Double.Dot(-rayDirection, normal) * normal + rayDirection;
                    ThrowRayFromWorldPos(rayHitPoint + normal * lightSmoothCorrector, rayDirection, scene, out rayHit, out rayHitPoint, out rayHitObject);
                }

                if (rayHit)
                {
                    Vector3 illu = GetDirectIllumination(rayHitObject, rayHitPoint, scene);
                    illu -= dark * Vector3.One;
                    illu /= (bright - dark);
                    screen.pxmap[i] = ColorXv3(rayHitObject.Color, ClampLightIntensity (illu));
                }
            }
        }

        private void ThrowRayFromPx (Vector2 pxCoordinates, List<iSceneObject> objects, out bool hit, out Vector3Double hitPoint, out iSceneObject hitObject, out Vector3Double rayDirection)
        {
            Vector3Double rayStart;

            if (perspective == 0)
            {
                rayStart = position + Scr2WorldPos(pxCoordinates);
                rayDirection = new Vector3Double(0, 0, 1);
            }
            else
            {
                rayStart = position ;
                rayDirection = Vector3Double.Normalize(new Vector3Double(0, 0, 1 / perspective) + Scr2WorldPos(pxCoordinates)).Rotated(new Vector3Double(0,1,0), Math.PI/8);
            }

            ThrowRayFromWorldPos(rayStart, rayDirection, objects, out hit, out hitPoint, out hitObject);
        }

        private void ThrowRayFromWorldPos (Vector3Double rayStart, Vector3Double rayDirection, List<iSceneObject> objects, out bool hit, out Vector3Double hitPoint, out iSceneObject hitObject)
        {
            hit = false;
            hitPoint = Vector3Double.Zero;
            hitObject = null;
            
            RayTracer.Ray ray;
                        
            ray = new RayTracer.Ray(rayStart, rayDirection);

            double distance = double.MaxValue;

            foreach (iSceneObject o in objects)
            {
                Vector3Double newHitPoint = new Vector3Double();
                bool newRayHit = o.FindRayIntersection (ray, out newHitPoint);

                if (newRayHit)
                {
                    if (!hit)
                    {
                        hit = true;
                        hitObject = o;
                        hitPoint = newHitPoint;
                        distance = Vector3Double.Distance(rayStart, newHitPoint);
                    }
                    else
                    {
                        double newDistance = Vector3Double.Distance(rayStart, newHitPoint);
                        if (newDistance < distance)
                        {
                            hitObject = o;
                            hitPoint = newHitPoint;
                            distance = newDistance;
                        }
                    }
                }
            }            
        }

        private Vector3 GetDirectIllumination (iSceneObject sceneObject, Vector3Double worldPoint, List<iSceneObject> sceneObjects)
        {
            Vector3 illumination = Vector3.Zero;
            double lightDistance = 0;

            if (sceneObject.LightSource > 0f)
            {
                illumination = Vector3.One;
            }
            else if (sceneObject.Reflection == 0f)
            {
                foreach (iSceneObject lightSource in sceneObjects.FindAll(s => s.LightSource > 0f))
                {
                    if (IsPointInLight (worldPoint, lightSource, sceneObjects, out lightDistance) && sceneObject.NormalOn(worldPoint) != null)
                    {
                        double cosTheta = Vector3Double.Dot(sceneObject.NormalOn (worldPoint), Vector3Double.Normalize(lightSource.Center - worldPoint)) / MathF.PI;

                        double rIntensity = (lightSource.LightSource * lightSource.Color.R * Math.Max(cosTheta, 0f) / Math.Pow(lightDistance, 2f)) / 255f;
                        double gIntensity = (lightSource.LightSource * lightSource.Color.G * Math.Max(cosTheta, 0f) / Math.Pow(lightDistance, 2f)) / 255f;
                        double bIntensity = (lightSource.LightSource * lightSource.Color.B * Math.Max(cosTheta, 0f) / Math.Pow(lightDistance, 2f)) / 255f;

                        illumination += new Vector3 ((float)rIntensity, (float)gIntensity, (float)bIntensity);
                    }
                }
            }

            return illumination;
        }

        /*
        private Vector3 GetIndirectIllumination (Sphere sceneObject, Vector3Double worldPoint, List<Sphere> sceneObjects, int rayQuantity)
        {
            Vector3 illumination = Vector3.Zero;

            for (int i = 0; i < rayQuantity; i++)
            {
                RayTracer.Ray ray = new RayTracer.Ray(worldPoint, sceneObject.NormalOn(worldPoint));
                ray.RandomizeDirection(ray.Direction, Math.PI);
                
                foreach (Sphere sph in sceneObjects)
                {
                    ray.FindSphereIntersection (sph.center, sph.radius, )
                }
            }

            return illumination;
        }
        */

        private bool IsPointInLight (Vector3Double point, iSceneObject lightSource, List<iSceneObject> sceneObjects, out double lightDistance)
        {
            lightDistance = Vector3Double.Distance(lightSource.Center, point);
            bool isInLight = true;
            Vector3Double newRayPoint = new Vector3Double();

            RayTracer.Ray ray = new RayTracer.Ray();
            ray.origin = lightSource.Center;
            ray.Direction = point - lightSource.Center;
            foreach (iSceneObject so in sceneObjects)
            {
                if (so.LightSource == 0f)
                {
                    if (so.FindRayIntersection(ray, out newRayPoint)
                        && Vector3Double.Distance(lightSource.Center, newRayPoint) < lightDistance
                        && Vector3Double.Distance(newRayPoint, point) > lightSmoothCorrector)
                    {
                        isInLight = false;
                        break;
                    }
                }
            }

            return isInLight;
        }

        

        public Vector3Double Scr2WorldPos (Vector2 scrPos)
        {
            Vector2 centered = scrPos - new Vector2(screen.width / 2, screen.height / 2);
            Vector3Double worldPos = size * new Vector3Double(centered.X, - centered.Y, 0f);

            return worldPos;
        }

        private Color GammaCorrection ( Color c)
        {
            return Color.FromArgb((int)MathF.Pow(c.R, 2.2f), (int)MathF.Pow(c.G, 2.2f), (int)MathF.Pow(c.B, 2.2f));
        }

        private Color ColorXv3 (Color c, Vector3 v)
        {
            return Color.FromArgb((int)((float)c.R * v.X), (int)((float)c.G * v.Y), (int)((float)c.B * v.Z));
        }

        private Vector3 ClampLightIntensity (Vector3 intensity)
        {
            return new Vector3(ClampLightIntensity(intensity.X), ClampLightIntensity(intensity.Y), ClampLightIntensity(intensity.Z));
        }

        private float ClampLightIntensity(float f)
        {
            return MathF.Max(MathF.Min(f, 1f), 0f);
        }
    }
}
