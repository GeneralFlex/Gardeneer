using Gaskellgames;
using UnityEngine;
using System.Collections.Generic;

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
        if (tileObjects.Count == 0 && objectToPlace.requiresTags.Contains(TileObjectTag.None)) return true;

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
}
