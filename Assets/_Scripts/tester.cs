using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class tester : MonoBehaviour
{
    public GameObject highlightSquare;
    private Transform hsT;
    public TileObjectSO soil;
    public TileObjectSO richSoil;
    public TileObjectSO tomato;
    public TileObjectSO luxuryTomato;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hsT = Instantiate(highlightSquare).transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int roundedPos = Vector2Int.RoundToInt(mousePos);
        hsT.position = (Vector2)roundedPos;
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TileManager.Instance.PlaceTileObject(roundedPos, soil);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            TileManager.Instance.PlaceTileObject(roundedPos, richSoil);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            TileManager.Instance.PlaceTileObject(roundedPos, tomato);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            TileManager.Instance.PlaceTileObject(roundedPos, luxuryTomato);
        }
    }
}
