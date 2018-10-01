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

        private float lightSmoothCorrector = 0.5f;

        public void PrintScene (List<Sphere> scene)
        {
            for (int i =  0; i < screen.pxmap.Count; i++)
            {
                bool rayHit = false;
                Sphere rayHitSphere = null;
                Vector3 rayHitPoint = new Vector3();

                ThrowRayFromPx(screen.PXIndex2Coordinates(i), scene, out rayHit, out rayHitPoint, out rayHitSphere);

                if (rayHit)
                {
                    screen.pxmap[i] = GetFragment(rayHitSphere, rayHitPoint, scene);
                }
                else
                {
                    screen.pxmap[i] = backgroundColor;
                }
            }
        }

        private void ThrowRayFromPx (Vector2 pxCoordinates, List<Sphere> objects, out bool hit, out Vector3 hitPoint, out Sphere hitObject)
        {
            hit = false;
            hitPoint = Vector3.Zero;
            hitObject = null;

            Vector3 rayStart = position + Scr2WorldPos(pxCoordinates);
            RayTracer.Ray ray = new RayTracer.Ray(rayStart, new Vector3(0f, 0f, 1f));
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

        private Color GetFragment (Sphere sceneObject, Vector3 worldPoint, List<Sphere> sceneObjects)
        {
            Color fragment = new Color();

            if (sceneObject.lightSource > 0f)
            {
                fragment = sceneObject.color;
            }
            else
            {
                Color lightIntensity = ambiantLight;

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
                        float cosTheta = Vector3.Dot(Vector3.Normalize(worldPoint - sceneObject.center), Vector3.Normalize(lightSource.center - worldPoint)) / MathF.PI;

                        int rIntensity = lightIntensity.R + (int)(lightSource.lightSource * lightSource.color.R * MathF.Max(cosTheta, 0f) / MathF.Pow(lightDistance, 2f));
                        int gIntensity = lightIntensity.G + (int)(lightSource.lightSource * lightSource.color.G * MathF.Max(cosTheta, 0f) / MathF.Pow(lightDistance, 2f));
                        int bIntensity = lightIntensity.B + (int)(lightSource.lightSource * lightSource.color.B * MathF.Max(cosTheta, 0f) / MathF.Pow(lightDistance, 2f));

                        lightIntensity = Color.FromArgb((int)MathF.Min(255, rIntensity), (int)MathF.Min(255, gIntensity), (int)MathF.Min(255, bIntensity));
                    }
                }

                float red = sceneObject.color.R * ((float)lightIntensity.R / 255f);
                float green = sceneObject.color.G * ((float)lightIntensity.G / 255f);
                float blue = sceneObject.color.B * ((float)lightIntensity.B / 255f);

                fragment = Color.FromArgb((int)red, (int)green, (int)blue);
            }

            return fragment;
        }

        public Vector3 Scr2WorldPos (Vector2 scrPos)
        {
            Vector2 centered = scrPos - new Vector2(screen.width / 2, screen.height / 2);
            Vector3 worldPos = size * new Vector3(centered.X, - centered.Y, 0f);

            return worldPos;
        }
    }
}
