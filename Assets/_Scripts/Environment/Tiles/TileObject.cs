using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

public abstract class TileObject : MonoBehaviour
{
    public List<TileInfo> tiles = new List<TileInfo>();
    [HideInInspector]
    public TileObjectSO SO;
}