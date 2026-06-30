using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Linq;
using UnityEngine.Events;

[System.Serializable]
public class ConveyorBeltGroup
{
    public int uniqueID;
    public float speed;
    public int animationFrame = 0;
    public Dictionary<int, List<Sprite>> sprites = new Dictionary<int, List<Sprite>>();
    public UnityEvent<int> updateFrame;

    [Header("Base directions")]
    public List<Sprite> left;
    public List<Sprite> right;
    public List<Sprite> up;
    public List<Sprite> down;

    [Header("Ends")]
    public List<Sprite> leftL;
    public List<Sprite> leftR;
    public List<Sprite> rightL;
    public List<Sprite> rightR;
    public List<Sprite> upU;
    public List<Sprite> upD;
    public List<Sprite> downU;
    public List<Sprite> downD;

    [Header("Corners")]
    public List<Sprite> downLeft;
    public List<Sprite> downRight;
    public List<Sprite> upLeft;
    public List<Sprite> upRight;
    public List<Sprite> leftUp;
    public List<Sprite> leftDown;
    public List<Sprite> rightUp;
    public List<Sprite> rightDown;
}

public class ConveyorBeltManager : MonoBehaviour
{
    public List<ConveyorBeltGroup> conveyorGroups = new List<ConveyorBeltGroup>();
    public int index=0;

    public static ConveyorBeltManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        GenerateDictionary();
    }

    private void Start()
    {
        foreach (ConveyorBeltGroup cg in conveyorGroups)
        {
            StartCoroutine(Animate(cg));
        }
    }

    public ConveyorBeltGroup GetGroup(int ID)
    {
        return conveyorGroups.Where(r => r.uniqueID == ID).ToList().First();
    }

    private IEnumerator Animate(ConveyorBeltGroup cg)
    {
        cg.animationFrame = (cg.animationFrame+1)%4;
        if (cg.animationFrame > 4)
            cg.animationFrame = 0;

        cg.updateFrame?.Invoke(cg.animationFrame);

        yield return new WaitForSeconds(1/cg.speed);
        StartCoroutine(Animate(cg));
    }

    private void GenerateDictionary()
    {
        foreach (ConveyorBeltGroup cg in conveyorGroups) {
            Dictionary<int, List<Sprite>> d = cg.sprites;
            //base direction
            d.Clear();
            d.Add(0002, cg.left);
            d.Add(0102, cg.left);
            d.Add(1102, cg.left);
            d.Add(0112, cg.left);
            d.Add(1112, cg.left);
            d.Add(1012, cg.left);

            d.Add(0200, cg.right);
            d.Add(0201, cg.right);
            d.Add(1201, cg.right);
            d.Add(0211, cg.right);
            d.Add(1211, cg.right);
            d.Add(1210, cg.right);

            d.Add(2000, cg.up);
            d.Add(2010, cg.up);
            d.Add(2110, cg.up);
            d.Add(2011, cg.up);
            d.Add(2111, cg.up);
            d.Add(2101, cg.up);

            d.Add(0020, cg.down);
            d.Add(1020, cg.down);
            d.Add(1120, cg.down);
            d.Add(1021, cg.down);
            d.Add(1121, cg.down);
            d.Add(0121, cg.down);

            //ends
            /*
            d.Add(0100, cg.leftL);
            d.Add(0002, cg.leftR);
            d.Add(0200, cg.rightL);
            d.Add(0001, cg.rightR);
            d.Add(0010, cg.upU);
            d.Add(2000, cg.upD);
            d.Add(0020, cg.downU);
            d.Add(1000, cg.downD);
            */
            //corners
            d.Add(1002, cg.downLeft);
            d.Add(1200, cg.downRight);
            d.Add(0012, cg.upLeft);
            d.Add(0210, cg.upRight);
            d.Add(2100, cg.leftUp);
            d.Add(0120, cg.leftDown);
            d.Add(2001, cg.rightUp);
            d.Add(0021, cg.rightDown);
        }   
    }
}
