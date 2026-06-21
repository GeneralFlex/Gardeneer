using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Crop", menuName = "ScriptableObjects/Crop", order = 1)]
public class CropSO : TileObjectSO
{
    public float growthTime;
    public List<ItemDrop> drops;
}