using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DualTilemap : MonoBehaviour
{
    public Tilemap bottom;
    public Tilemap top;
    //public Tile tile0000;
    public Tile tile0001;
    public Tile tile0010;
    public Tile tile0011;
    public Tile tile0100;
    public Tile tile0101;
    public Tile tile0110;
    public Tile tile0111;
    public Tile tile1000;
    public Tile tile1001;
    public Tile tile1010;
    public Tile tile1011;
    public Tile tile1100;
    public Tile tile1101;
    public Tile tile1110;
    public Tile tile1111;

    private Dictionary<int, Tile> tiles = new Dictionary<int, Tile>();

    [ContextMenu("Generate")]
    public void Generate()
    {
        //tiles.Add(0b0000, tile0000);
        tiles.Clear();
        tiles.Add(0b0001, tile0001);
        tiles.Add(0b0010, tile0010);
        tiles.Add(0b0011, tile0011);
        tiles.Add(0b0100, tile0100);
        tiles.Add(0b0101, tile0101);
        tiles.Add(0b0110, tile0110);
        tiles.Add(0b0111, tile0111);
        tiles.Add(0b1000, tile1000);
        tiles.Add(0b1001, tile1001);
        tiles.Add(0b1010, tile1010);
        tiles.Add(0b1011, tile1011);
        tiles.Add(0b1100, tile1100);
        tiles.Add(0b1101, tile1101);
        tiles.Add(0b1110, tile1110);
        tiles.Add(0b1111, tile1111);

        GenerateTileMap();
    }

    public void GenerateTileMap()
    {
        top.ClearAllTiles();
        bottom.gameObject.SetActive(false);

        HashSet<Vector3Int> bottomTiles = new HashSet<Vector3Int>();
        foreach (Vector3Int position in bottom.cellBounds.allPositionsWithin)
        {
            if (bottom.GetTile(position) == null) continue;
            bottomTiles.Add(position);
        }
        foreach (Vector3Int position in bottomTiles)
        {
            List<Vector3Int> offsets = new List<Vector3Int> {Vector3Int.zero, Vector3Int.left, Vector3Int.down, Vector3Int.left+Vector3Int.down};
            foreach (Vector3Int offset in offsets)
            {
                if(bottomTiles.Contains(position + offset)&&offset!=Vector3Int.zero) continue;
                int code = GetTileCode(position + offset);
                if (code == 0) continue;
                top.SetTile(position+offset, tiles[code]);
            }
        }
    }

    public int GetTileCode(Vector3Int position)
    {
        int code = 0b0000;
        if (bottom.GetTile(position) != null) code |= 0b0010;
        if (bottom.GetTile(position + Vector3Int.up) != null) code |= 0b1000;
        if (bottom.GetTile(position + Vector3Int.right) != null) code |= 0b001;
        if (bottom.GetTile(position + Vector3Int.up+Vector3Int.right) != null) code |= 0b0100;
        return code;
    }

}
