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
    public string displayName;
    public string description;
    public bool requreAll = false;
    public List<TileObjectTag> requiresTags;
    public List<TileObjectTag> providesTags;
    public List<TileObjectTag> forbiddenTags;
}
