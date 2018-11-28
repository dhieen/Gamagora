﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Geometry2D {

	public struct Line
    {
        public Vector2 pointOn;
        public Vector2 direction;

        public Line (Vector2 pt, Vector2 dir) { pointOn = pt; direction = dir; }

        public static Vector2 Intersection (Line A, Line B)
        {
            float t = ((B.pointOn.x * B.direction.y - B.pointOn.y * B.direction.x) - (A.pointOn.x * B.direction.y - A.pointOn.y * B.direction.x)) / (A.direction.x * B.direction.y - A.direction.y * B.direction.x);

            return A.pointOn + t * A.direction;
        }

        public static Vector2 Intersection (Vector2 pt1, Vector2 d1, Vector2 pt2, Vector2 d2)
        {
            return Intersection(new Line(pt1, d1), new Line(pt2, d2));
        }
    }
}
