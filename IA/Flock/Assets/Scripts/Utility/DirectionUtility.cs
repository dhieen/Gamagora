using UnityEngine;

public enum DIRECTION { ZERO = 0, UP, DOWN, RIGHT, LEFT, FRONT, BACK }

public class DirectionUtility
{
    public class Direction3 {

        public DIRECTION x;
        public DIRECTION y;
        public DIRECTION z;

        public Direction3()
        {
            x = DIRECTION.ZERO;
            y = DIRECTION.ZERO;
            z = DIRECTION.ZERO;
        }

        public bool IsZero()
        {
            return (x == DIRECTION.ZERO && y == DIRECTION.ZERO && z == DIRECTION.ZERO);
        }

        public Vector3 ToVector3 ()
        {
            return DirectionUtility.ToVector3(x) + DirectionUtility.ToVector3(y) + DirectionUtility.ToVector3(z);
        }
    }

    public static Vector3 ToVector3 (DIRECTION dir)
    {
        switch (dir)
        {
            case DIRECTION.DOWN: return Vector3.down;
            case DIRECTION.LEFT: return Vector3.left;
            case DIRECTION.RIGHT: return Vector3.right;
            case DIRECTION.UP: return Vector3.up;
            case DIRECTION.FRONT: return Vector3.forward;
            case DIRECTION.BACK: return Vector3.back;
        }

        return Vector3.zero;
    }

    public static Direction3 ToDirection3 (Vector3 vec)
    {
        Direction3 dir = new Direction3();

        if (vec.x > 0f) dir.x = DIRECTION.RIGHT;
        else if (vec.x < 0f) dir.x = DIRECTION.LEFT;
        else dir.x = DIRECTION.ZERO;

        if (vec.y > 0f) dir.y = DIRECTION.UP;
        else if (vec.y < 0f) dir.y = DIRECTION.DOWN;
        else dir.y = DIRECTION.ZERO;

        if (vec.z > 0f) dir.z = DIRECTION.FRONT;
        else if (vec.z < 0f) dir.z = DIRECTION.BACK;
        else dir.z = DIRECTION.ZERO;

        return dir;
    }
}