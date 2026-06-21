using Gaskellgames;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Item", order = 1)]
public class ItemSO : ScriptableObject
{
    public GameObject prefab;
    public string displayName;
    public string description;
    public Sprite icon;
}

public class Item : MonoBehaviour
{
    public ItemSO item;
}

public class ItemDrop
{
    public ItemSO item;
    public int minCount;
    public int maxCount;
    [MinMaxSlider(0, 100)]
    public float spawnChance;
}