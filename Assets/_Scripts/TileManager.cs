using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UIElements;

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

    public TileInfo GetTile(Vector2Int pos)
    {
        tiles.TryGetValue(pos, out TileInfo tile);
        return tile;
    }

    //[ContextMenu("Generate Tiles")]
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

    public bool PlaceTileObject(Vector2Int pos, TileObjectSO tileObjectSO)
    {
        if (GetTile(pos) == null) 
            return false;
        return PlaceTileObject(GetTile(pos), tileObjectSO);
    }

    public bool CanPlaceTileObject(Vector2Int pos, TileObjectSO tileObjectSO)
    {
        if (GetTile(pos)==null||!GetTile(pos).CanPlace(tileObjectSO))
        {
            return false;
        }
        return true;
    }

    public bool PlaceTileObject(TileInfo tile, TileObjectSO tileObjectSO)
    {
        if (!tile.CanPlace(tileObjectSO))   
            return false;

        TileObject tileObject = Instantiate(tileObjectSO.prefab).GetComponent<TileObject>();
        tileObject.SO = tileObjectSO;
        tileObject.tile = tile;
        tileObject.transform.position = new Vector3(tile.position.x, tile.position.y, tile.position.y / 100f);
        tile.tileObjects.Add(tileObject);
        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }
}
