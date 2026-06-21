using Unity.VisualScripting;
using UnityEngine;

public abstract class TileObject : MonoBehaviour
{
    public TileInfo tile;
    [HideInInspector]
    public TileObjectSO SO;
}