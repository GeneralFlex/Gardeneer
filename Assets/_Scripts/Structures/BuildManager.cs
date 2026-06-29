using UnityEngine;
using System.Collections.Generic;
using Component = UnityEngine.Component;
using DG.Tweening;
using static UnityEditor.PlayerSettings;
using System.Collections;
using System.Linq;

public class BuildManager : MonoBehaviour
{
    [Header("Color")]
    public Color validColor;
    public Color invalidColor;
    private Color currentColor;
    public MouseShaderController mouseShaderController;

    [Header("Position")]
    private Vector2 mousePos;
    private Vector2Int mousePosInt;
    private TileInfo targetTileInfo;
    private TileInfo lastTileInfo;

    [Header("Object")]
    [HideInInspector]
    public TileObjectSO currentTileObjectSO;

    [Header("Ghost")]
    [HideInInspector]
    public GameObject ghost;
    private SpriteRenderer[] ghostRenderers;
    private Vector3 ghostVelocityRef;

    [Header("HSL")]
    public GameObject higlightSquarePrefab;
    private GameObject HLSquare;
    private Coroutine HLSquareCoroutine;

    [Header("Quick Select")]
    private TileInfo lastQuickSelectTile;
    private int lastQuickSelectIndex=0;

    [Header("Delete")]
    public GameObject deleteCross;
    public float timeToDelete = 1f;

    private void Start()
    {
        HLSquare = Instantiate(higlightSquarePrefab);
    }

    private void Update()
    {
        //mouse pos
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (currentTileObjectSO != null)
            mousePosInt = Vector2Int.RoundToInt(mousePos + new Vector2((1 - (currentTileObjectSO.size.x % 2)) / 2f, (1 - (currentTileObjectSO.size.y % 2)) / 2f));

        targetTileInfo = TileManager.Instance.GetTile(mousePos);

        //not building - return
        if (currentTileObjectSO != null)
        {
            //cancel build mode
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButton(1))
            {
                StopBuilding();
                return;
            }

            //rotate
            if (Input.GetKeyDown(KeyCode.R))
                Rotate();

            //ghost color and pos, HLS pos
            if (ghost != null && currentTileObjectSO != null)
            {
                UpdateGhost();
            }

            //place
            if (Input.GetMouseButton(0))
            {
                lastQuickSelectTile = null;
                TileManager.Instance.PlaceTileObject(mousePosInt, currentTileObjectSO);
            }
        }
        else
        {
            //quick select
            if (Input.GetKeyDown(KeyCode.Q))
                QuickSelect();

            //TODO: rotate in place - transfer items and settings
            /*
            //rotate in place
            if (Input.GetKeyDown(KeyCode.R))
                Rotate();
            */

            //TODO: delete

            lastTileInfo = targetTileInfo;
        }
    }


    public void UpdateGhost()
    {
        //ghost pos
        Vector3 targetGhostPos = new Vector3(mousePosInt.x - (1 - (currentTileObjectSO.size.x % 2)) / 2f, mousePosInt.y - (1 - (currentTileObjectSO.size.y % 2)) / 2f, mousePosInt.y - Mathf.Floor(currentTileObjectSO.size.y / 2) - 0.5f);
        ghost.transform.position = Vector3.SmoothDamp(ghost.transform.position, targetGhostPos, ref ghostVelocityRef, 0.05f);

        //HLS
        HLSquare.transform.position = ghost.transform.position;

        if (!TileManager.Instance.CanPlaceTileObject(mousePosInt, currentTileObjectSO))
        {
            SetGhostColor(invalidColor);
            return;
        }
        else
        {
            SetGhostColor(validColor);
        }
    }

    public void Rotate()
    {
        int rotationIndex = currentTileObjectSO.rotations.FindIndex(x => x == currentTileObjectSO);
            if (rotationIndex >= currentTileObjectSO.rotations.Count - 1)
                rotationIndex = -1;
            StartBuilding(currentTileObjectSO.rotations[rotationIndex+1]);
    }

    public void QuickSelect()
    {
        TileObject selectedObject = null;
        TileInfo selectedTile = TileManager.Instance.GetTile(Camera.main.ScreenToWorldPoint(Input.mousePosition));

        List<TileObject> tiles = selectedTile.GetTileObjectByPriority();
        if (tiles.Count > 0)
        {
            if (lastQuickSelectIndex >= tiles.Count - 1 || lastQuickSelectTile != selectedTile)
                lastQuickSelectIndex = -1;
            selectedObject = TileManager.Instance.GetTileObjectByPriority(selectedTile.position)[lastQuickSelectIndex + 1];
            lastQuickSelectIndex++;
            lastQuickSelectTile = selectedTile;
        }
        if (selectedObject != null)
            StartBuilding(selectedObject.SO);
    }

    public void StopBuilding()
    {
        currentTileObjectSO = null;
        SetHSLsize(Vector2.zero);
        Destroy(ghost);
        mouseShaderController.SetTargetRadius(0);
    }

    public void StartBuilding(TileObjectSO tileObjectSO)
    {
        StopBuilding();
        currentTileObjectSO = tileObjectSO;
        
        //ghost
        ghost = GenerateGhost(tileObjectSO);
        currentColor = invalidColor;
        SetGhostColor(validColor);

        //HLS
        SetHSLsize(tileObjectSO.size);

        //grid
        mouseShaderController.SetTargetRadius(Mathf.Max(tileObjectSO.size.x, tileObjectSO.size.y) + 1);
    }

    public GameObject GenerateGhost(TileObjectSO tileObjectSO)
    {
        GameObject ghost = Instantiate(tileObjectSO.prefab);

        ghost.transform.position = (Vector2)Vector2Int.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        ghostRenderers = ghost.GetComponentsInChildren<SpriteRenderer>(true);

        foreach (MonoBehaviour script in ghost.GetComponentsInChildren<MonoBehaviour>(true))
        {
            Destroy(script);
        }
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

    public void SetHSLsize(Vector2 size)
    {
        if (HLSquare == null)
            return;
        if (HLSquareCoroutine != null)
            StopCoroutine(HLSquareCoroutine);
        HLSquareCoroutine = StartCoroutine(SetHLSize(size));
    }

    private IEnumerator SetHLSize(Vector2 size)
    {
        size *= 2;
        SpriteRenderer[] renderers = HLSquare.GetComponentsInChildren<SpriteRenderer>();
        SpriteRenderer sr = renderers[0];
        while (Vector2.Distance(sr.size, size) > 0.01f)
        {
            sr.size = Vector2.Lerp(sr.size, size, 1f - Mathf.Exp(-20 * Time.deltaTime));
            foreach (SpriteRenderer renderer in renderers)
            {
                renderer.size = sr.size;
            }
            yield return null;
        }
    }
}
