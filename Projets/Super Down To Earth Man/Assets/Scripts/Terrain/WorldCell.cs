using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WorldCell : MonoBehaviour
{
    public WorldCell Instantiate(Transform parent, Vector2 localPosition)
    {
        WorldCell cell = Instantiate<WorldCell>(this as WorldCell, parent);
        cell.transform.localPosition = localPosition;

        return cell;
    }

    public abstract void GenerateContent(Vector2 bound, List<WorldCell> neighbours);
}
