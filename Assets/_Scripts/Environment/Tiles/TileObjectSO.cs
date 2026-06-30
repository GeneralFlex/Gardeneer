using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public enum TileObjectTag
{
    Empty,
    Soil,
    RichSoil,
    Crop,
    Conveyor,
    Structure
}

public class TileObjectSO : ScriptableObject
{
    public GameObject prefab;
    public TileObjectSO previousRotation;
    public TileObjectSO nextRotation;
    public Vector2Int size = Vector2Int.one;
    public string displayName;
    public string description;
    public bool requreAll = false;
    public int interactionPriority = 0;
    public List<TileObjectTag> requiresTags;
    public List<TileObjectTag> providesTags;
    public List<TileObjectTag> forbiddenTags;
}

