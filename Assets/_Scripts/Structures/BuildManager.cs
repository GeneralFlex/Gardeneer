using UnityEngine;
using System.Collections.Generic;
using Component = UnityEngine.Component;

public class BuildManager : MonoBehaviour
{
    [Header("color")]
    public Color validColor;
    public Color invalidColor;
    private Color currentColor;

    [Header("position")]
    private Vector2 mousePos;
    private Vector2Int mousePosInt;
    private TileInfo targetTileInfo;

    [Header("object")]
    [HideInInspector]
    public TileObjectSO currentTileObjectSO;

    [Header("ghost")]
    [HideInInspector]
    public GameObject ghost;
    private SpriteRenderer[] ghostRenderers;


    private void Update()
    {
        if (currentTileObjectSO == null)
            return;

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButton(1))
        {
            currentTileObjectSO = null;
            Destroy(ghost);
            return;
        }

        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosInt = Vector2Int.RoundToInt(mousePos);
        if (ghost != null)
        {
            ghost.transform.position = (Vector2)mousePosInt;
            SetGhostColor(TileManager.Instance.CanPlaceTileObject(mousePosInt, currentTileObjectSO) ? validColor : invalidColor);
        }

        if (Input.GetMouseButton(0))
        {
            TileManager.Instance.PlaceTileObject(mousePosInt, currentTileObjectSO);
        }

    }

    public void StartBuilding(TileObjectSO tileObjectSO)
    {
        if (ghost != null) {
            Destroy(ghost);
        }
        currentTileObjectSO = tileObjectSO;
        ghost = GenerateGhost(tileObjectSO);
    }

    public GameObject GenerateGhost(TileObjectSO tileObjectSO)
    {
        GameObject ghost = Instantiate(tileObjectSO.prefab);

        ghostRenderers = ghost.GetComponentsInChildren<SpriteRenderer>(true);

        foreach (MonoBehaviour script in ghost.GetComponentsInChildren<MonoBehaviour>(true))
        {
            Destroy(script);
        }

        SetGhostColor(validColor);

        return ghost;
    }

    public void SetGhostColor(Color color) {

        if(color == currentColor)
            return;

        foreach (SpriteRenderer renderer in ghostRenderers)
        {
            renderer.color = color;
        }
        currentColor = color;
    }
}
