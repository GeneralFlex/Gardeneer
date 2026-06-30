using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

[System.Serializable]
public class CursorEntry
{
    public string name;
    public Sprite sprite;
    public Vector2Int size = new Vector2Int(32, 32);
    public Color color = Color.white;
    public bool center = true;
}

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance { get; private set; }

    [Header("Object references")]
    public GameObject cursor;
    public GameObject highlightSquarePrefab;
    public GameObject HLSquare;

    private Canvas canvas;
    private RectTransform cursorRect;
    private CursorEntry currentCursor;
    private CursorEntry mainCursor;

    [Header("Lists and dicts")]
    public String mainCursorName;
    public List<CursorEntry> cursorSprites = new List<CursorEntry>();
    public Dictionary<string, CursorEntry> cursors = new Dictionary<string, CursorEntry>();
    public Dictionary<CursorEntry, int> cursorsByPriority = new Dictionary<CursorEntry, int>();

    [Header("Misc")]
    private Coroutine HLSquareCoroutine;
    private Vector3 HLSquareVelocityRef;
    private Vector2 offset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        Instance = this;
        HLSquare = Instantiate(highlightSquarePrefab);
        //highlightSquare.SetActive(false);
    }

    void Start()
    {
        Cursor.visible = false;
        foreach (CursorEntry entry in cursorSprites)
        {
            cursors.Add(entry.name, entry);
            cursorsByPriority.Add(entry, -1);
        }

        canvas = cursor.transform.parent.GetComponent<Canvas>();
        cursorRect = cursor.GetComponent<RectTransform>();

        mainCursor = cursors[mainCursorName];
        SetCursor(mainCursor);
    }

    // Update is called once per frame
    void Update()
    {
        cursorsByPriority[mainCursor] += 1;
        if (Input.GetMouseButton(0))
            cursorsByPriority[cursors["arrowSmallClicked"]]+=2;

        if (Input.GetMouseButton(2))
            cursorsByPriority[cursors["move"]] += 20;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out Vector2 localPos);
        cursorRect.localPosition = localPos+offset;

    }
    private void LateUpdate()
    {
        SetCursorByPriority();
        foreach (var key in cursorsByPriority.Keys.ToList())
        {
            cursorsByPriority[key] = -1;
        }
    }

    void SetCursorByPriority()
    {
        CursorEntry highest = cursorsByPriority.OrderByDescending(x => x.Value).First().Key;

        if (highest != currentCursor)
        {
            SetCursor(highest);
        }
    }

    void SetCursor(CursorEntry entry)
    {
        currentCursor = entry;

        Image img = cursor.GetComponent<Image>();
        img.sprite = null;

        cursorRect.sizeDelta = entry.size;
        if (!entry.center)
            offset = new Vector2(entry.size.x / 2, -entry.size.y / 2);
        else
            offset = new Vector2(mainCursor.size.x/2, -mainCursor.size.y/2);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out Vector2 localPos);
        cursorRect.localPosition = localPos + offset;

        img.sprite = entry.sprite;
        img.color = entry.color;
    }

    public void AddCursorPriority(string name, int priority)
    {
        cursorsByPriority[cursors[name]] += priority;
    }

    public void SetHSLsize(Vector2 size)
    {
        if(HLSquareCoroutine != null)
            StopCoroutine(HLSquareCoroutine);
        HLSquareCoroutine = StartCoroutine(SetHLSize(size));
    }

    private IEnumerator SetHLSize(Vector2 size)
    {
        size *= 2;
        SpriteRenderer[] renderers = HLSquare.GetComponentsInChildren<SpriteRenderer>();
        SpriteRenderer sr = renderers[0];
        while(Vector2.Distance(sr.size, size) > 0.01f)
        {
            sr.size = Vector2.Lerp(sr.size, size, 1f - Mathf.Exp(-20 * Time.deltaTime));
            foreach(SpriteRenderer renderer in renderers)
            {
                renderer.size = sr.size;
            }
            yield return null;
        }
    }

    public void MoveHLSquare(Vector2 pos)
    {
        HLSquare.transform.position = Vector3.SmoothDamp(HLSquare.transform.position, new Vector3(pos.x, pos.y, HLSquare.transform.position.z), ref HLSquareVelocityRef, 0.05f);
    }
}
