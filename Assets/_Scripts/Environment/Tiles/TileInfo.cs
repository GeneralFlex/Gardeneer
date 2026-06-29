using Gaskellgames;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TileInfo {

    public Vector2Int position;

    public List<TileObject> tileObjects = new List<TileObject>();
    public List<Item> Items = new();

    public TileInfo(Vector2Int position)
    {
        this.position = position;
    }

    public bool CanPlace(TileObjectSO objectToPlace)
    {
        if (tileObjects.Count == 0 && objectToPlace.requiresTags.Contains(TileObjectTag.Empty)) return true;

        bool hasRequiredTag = false;
        List<TileObjectTag> requiredTagsLeft = new List<TileObjectTag>(objectToPlace.requiresTags);
        foreach (TileObject tileObject in tileObjects)
        {
            foreach(TileObjectTag tileTag in tileObject.SO.providesTags)
            {
                if (objectToPlace.forbiddenTags.Contains(tileTag)) return false;
                if (objectToPlace.requiresTags.Contains(tileTag))
                {
                    hasRequiredTag = true;
                    requiredTagsLeft.Remove(tileTag);
                }
            }
        }

        if (requiredTagsLeft.Count > 0 && objectToPlace.requreAll)
            return false;
        return hasRequiredTag;
    }

    public List<TileObject> GetTileObjectByPriority()
    {
        List<TileObject> sorted = tileObjects.OrderByDescending(r => r.SO.interactionPriority).ToList();
        return sorted;
    }

    public TileObject GetTileObject(TileObjectSO tileObjectSO)
    {
        return tileObjects.Where(r => r.SO == tileObjectSO).FirstOrDefault();
    }
}
