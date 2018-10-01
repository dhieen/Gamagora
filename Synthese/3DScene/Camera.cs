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
        public Vector3 position;
        public PPMImage screen;
        public float size;
        public Color backgroundColor;
        public Color ambiantLight;
        public float perspective;
        public float dark;
        public float bright;
        public int maxReflectionIterations = 3;

        private float lightSmoothCorrector = 0.01f;

        public void PrintScene (List<Sphere> scene)
        {
            screen.Clear(backgroundColor);

            for (int i =  0; i < screen.pxmap.Count; i++)
            {
                int reflexionIterations = 0;
                bool rayHit = false;
                Sphere rayHitSphere = null;
                Vector3 rayHitPoint = new Vector3();
                Vector3 rayDirection = new Vector3();

                ThrowRayFromPx(screen.PXIndex2Coordinates(i), scene, out rayHit, out rayHitPoint, out rayHitSphere, out rayDirection);
                
                while (rayHit && rayHitSphere.reflection > 0f && reflexionIterations++ < maxReflectionIterations)
                {
                    Vector3 normal = rayHitSphere.NormalOn(rayHitPoint);
                    rayDirection = 2 * Vector3.Dot(-rayDirection, normal) * normal + rayDirection;
                    ThrowRayFromWorldPos(rayHitPoint, rayDirection, scene, out rayHit, out rayHitPoint, out rayHitSphere);
                }

                if (rayHit)
                {
                    Vector3 illu = GetIllumination(rayHitSphere, rayHitPoint, scene);
                    illu -= dark * Vector3.One;
                    illu /= (bright - dark);
                    screen.pxmap[i] = ColorXv3(rayHitSphere.color, ClampLightIntensity (illu));
                }
            }
        }

        private void ThrowRayFromPx (Vector2 pxCoordinates, List<Sphere> objects, out bool hit, out Vector3 hitPoint, out Sphere hitObject, out Vector3 rayDirection)
        {
            Vector3 rayStart;

            if (perspective == 0f)
            {
                rayStart = position + Scr2WorldPos(pxCoordinates);
                rayDirection = new Vector3(0f, 0f, 1f);
            }
            else
            {
                rayStart = position + new Vector3(0f, 0f, -1f) / perspective;
                rayDirection = Vector3.Normalize(Scr2WorldPos(pxCoordinates) - rayStart);
            }

            ThrowRayFromWorldPos(rayStart, rayDirection, objects, out hit, out hitPoint, out hitObject);
        }

        private void ThrowRayFromWorldPos (Vector3 rayStart, Vector3 rayDirection, List<Sphere> objects, out bool hit, out Vector3 hitPoint, out Sphere hitObject)
        {
            hit = false;
            hitPoint = Vector3.Zero;
            hitObject = null;
            
            RayTracer.Ray ray;
                        
            ray = new RayTracer.Ray(rayStart, rayDirection);

            float distance = float.MaxValue;

            foreach (Sphere sph in objects)
            {
                Vector3 newHitPoint = new Vector3();
                bool newRayHit = ray.FindSphereIntersection(sph.center, sph.radius, out newHitPoint);

                if (newRayHit)
                {
                    if (!hit) hit = true;
                    float newDistance = Vector3.Distance(rayStart, newHitPoint);
                    if (newDistance < distance)
                    {
                        hitObject = sph;
                        hitPoint = newHitPoint;
                        distance = newDistance;
                    }
                }
            }
        }

        private Vector3 GetIllumination (Sphere sceneObject, Vector3 worldPoint, List<Sphere> sceneObjects)
        {
            Vector3 illumination = Vector3.Zero;

            if (sceneObject.lightSource > 0f)
            {
                illumination = Vector3.One;
            }
            else if (sceneObject.reflection == 0f)
            {
                foreach (Sphere lightSource in sceneObjects.FindAll(s => s.lightSource > 0f))
                {
                    float lightDistance = Vector3.Distance(lightSource.center, worldPoint);
                    bool isInLight = true;
                    Vector3 newRayPoint = new Vector3();

                    foreach (Sphere sph in sceneObjects)
                    {
                        if (sph.lightSource == 0f)
                        {
                            RayTracer.Ray ray = new RayTracer.Ray();
                            ray.origin = lightSource.center;
                            ray.Direction = worldPoint + lightSmoothCorrector * Vector3.Normalize(worldPoint - sceneObject.center) - lightSource.center;

                            if (ray.FindSphereIntersection(sph.center, sph.radius, out newRayPoint)
                                && Vector3.Distance(newRayPoint, lightSource.center) < lightDistance)
                            {
                                isInLight = false;
                                break;
                            }
                        }
                    }

                    if (isInLight)
                    {
                        float cosTheta = Vector3.Dot(sceneObject.NormalOn (worldPoint), Vector3.Normalize(lightSource.center - worldPoint)) / MathF.PI;

                        float rIntensity = (lightSource.lightSource * lightSource.color.R * MathF.Max(cosTheta, 0f) / MathF.Pow(lightDistance, 2f)) / 255f;
                        float gIntensity = (lightSource.lightSource * lightSource.color.G * MathF.Max(cosTheta, 0f) / MathF.Pow(lightDistance, 2f)) / 255f;
                        float bIntensity = (lightSource.lightSource * lightSource.color.B * MathF.Max(cosTheta, 0f) / MathF.Pow(lightDistance, 2f)) / 255f;

                        illumination += new Vector3 (rIntensity, gIntensity,bIntensity);
                    }
                }
            }

            return illumination;
        }

        public Vector3 Scr2WorldPos (Vector2 scrPos)
        {
            Vector2 centered = scrPos - new Vector2(screen.width / 2, screen.height / 2);
            Vector3 worldPos = size * new Vector3(centered.X, - centered.Y, 0f);

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
