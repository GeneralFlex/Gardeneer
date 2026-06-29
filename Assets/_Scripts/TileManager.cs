using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

public class TileManager : MonoBehaviour
{
    public BoundsInt bounds;
    public Dictionary<Vector2Int, TileInfo> tiles = new();

    public static TileManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        GenerateTiles();
    }

    public void GenerateTiles()
    {
        tiles.Clear();

        for(int x = bounds.min.x; x <= bounds.max.x; x++)
        {
            for (int y = bounds.min.y; y <= bounds.max.y; y++)
            {
                Vector2Int pos = new Vector2Int(x+(int)bounds.center.x, y + (int)bounds.center.y);
                TileInfo tileInfo = new TileInfo(pos);
                tiles.Add(pos, tileInfo);
            }
        }
    }

    // -------- HELPERS -------- 

    public TileInfo GetTile(Vector2 mousePos)
    {
        Vector2Int pos = Vector2Int.RoundToInt(mousePos);
        tiles.TryGetValue(pos, out TileInfo tile);
        return tile;
    }

    public TileInfo GetTile(Vector2Int pos)
    {
        tiles.TryGetValue(pos, out TileInfo tile);
        return tile;
    }

    public List<TileObject> GetTileObjectByPriority(Vector2 pos)
    {
        TileInfo tile = GetTile(pos);

        if (tile!=null)
        {
            return tile.GetTileObjectByPriority();
        }
        return null;
    }

    public bool CanPlaceTileObject(Vector2Int pos, TileObjectSO tileObjectSO)
    {
        Vector2[] tiles = new Vector2[tileObjectSO.size.x * tileObjectSO.size.y];

        for (int x = 0; x < tileObjectSO.size.x; x++)
        {
            for (int y = 0; y < tileObjectSO.size.y; y++)
            {
                Vector2Int partialObjectTilePos = new Vector2Int(x - (int)Mathf.Floor(tileObjectSO.size.x / 2), y - (int)Mathf.Floor(tileObjectSO.size.y / 2)) + pos;
                if (GetTile(partialObjectTilePos) == null || !(GetTile(partialObjectTilePos).CanPlace(tileObjectSO)))
                {
                    return false;
                }
            }
        }

        return true;
    }

    // -------- PLACING -------- 

    public bool PlaceTileObject(Vector2 mousePos, TileObjectSO tileObjectSO)
    {
        if (GetTile(Vector2Int.RoundToInt(mousePos)) == null)
            return false;
        return PlaceTileObject(GetTile((Vector2Int.RoundToInt(mousePos))), tileObjectSO);
    }

    public bool PlaceTileObject(TileInfo tile, TileObjectSO tileObjectSO)
    {
        if (!CanPlaceTileObject(tile.position, tileObjectSO))
            return false;

        TileObject tileObject = Instantiate(tileObjectSO.prefab).GetComponent<TileObject>();
        tileObject.SO = tileObjectSO;
        tileObject.transform.position = new Vector3(tile.position.x - (1 - (tileObjectSO.size.x % 2)) / 2f, tile.position.y - (1 - (tileObjectSO.size.y % 2)) / 2f, tile.position.y+Mathf.Floor(tileObjectSO.size.y/2)-Mathf.Clamp(tileObjectSO.interactionPriority/100f,0,0.5f));

        for (int x = 0; x < tileObjectSO.size.x; x++)
        {
            for (int y = 0; y < tileObjectSO.size.y; y++)
            {
                Vector2Int partialObjectTilePos = new Vector2Int(x - (int)Mathf.Floor(tileObjectSO.size.x / 2), y - (int)Mathf.Floor(tileObjectSO.size.y / 2)) + tile.position;

                tileObject.tiles.Add(GetTile(partialObjectTilePos));
                GetTile(partialObjectTilePos).tileObjects.Add(tileObject);
            }
        }
        return true;
    }

    // -------- REMOVING -------- 
    public bool RemoveTileObject(Vector2 mousePos)
    {
        if (GetTile(Vector2Int.RoundToInt(mousePos)) == null)
            return false;

        TileObject tileObject = GetTile(Vector2Int.RoundToInt(mousePos)).GetTileObjectByPriority().First();
        return RemoveTileObject(tileObject);
    }

    public bool RemoveTileObject(TileObject tileObject)
    {
        if (tileObject == null) return false;

        List<TileInfo> tileInfos = tileObject.tiles;
        foreach(TileInfo tileInfo in tileInfos)
        {
            tileInfo.tileObjects.Remove(tileObject);
        }
        Destroy(tileObject.gameObject);
        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }
}
