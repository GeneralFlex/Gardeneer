using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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
    public GameObject highlightSquare;

    private CursorEntry currentCursor;

    [Header("Lists and dicts")]
    public List<CursorEntry> cursorSprites = new List<CursorEntry>();
    public Dictionary<string, CursorEntry> cursors = new Dictionary<string, CursorEntry>();
    public Dictionary<CursorEntry, int> cursorsByPriority = new Dictionary<CursorEntry, int>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        Instance = this;
        highlightSquare = Instantiate(highlightSquarePrefab);
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

        SetCursor(cursors["arrowSmall"]);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2Int mousePosInt = Vector2Int.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        //highlightSquare.transform.position = Vector2.Lerp(highlightSquare.transform.position, mousePosInt, Time.deltaTime * (1+Vector2.Distance(highlightSquare.transform.position, mousePosInt)));
        highlightSquare.transform.DOMove(new Vector3(mousePosInt.x, mousePosInt.y, highlightSquare.transform.position.z), 0.1f).SetEase(Ease.OutQuad);

        Canvas canvas = cursor.transform.parent.GetComponent<Canvas>();
        RectTransform cursorRect = cursor.GetComponent<RectTransform>();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out Vector2 localPos);

        if (!currentCursor.center)
        {
            localPos += new Vector2(currentCursor.size.x / 2, -currentCursor.size.y / 2);
        }
        cursorRect.localPosition = localPos;

        cursorsByPriority[cursors["arrowSmall"]] += 1;
        if (Input.GetMouseButton(0))
            cursorsByPriority[cursors["arrowSmallClicked"]]+=2;

        if (Input.GetMouseButton(2))
            cursorsByPriority[cursors["move"]] += 20;
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

        cursor.GetComponent<RectTransform>().sizeDelta = entry.size;
        Image img =cursor.GetComponent<Image>();
        img.sprite = entry.sprite;
        img.color = entry.color;
    }

    public void AddCursorPriority(string name, int priority)
    {
        cursorsByPriority[cursors[name]] += priority;
    }
}
