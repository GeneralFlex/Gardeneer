using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.Windows;
using System.Collections;
using System.Linq;


public class ConveyorBelt : MonoBehaviour
{
    [Header("input/output settings")]
    public int uniqeGroupID = 1;
    public Vector2 direction;
    public HashSet<ConveyorBelt> inputs = new HashSet<ConveyorBelt>();
    public ConveyorBelt output;
    public int code = 0;

    [Header("References")]
    public SpriteRenderer spriteRenderer;
    private ConveyorBeltGroup group;

    [Header("Lists and dicts")]
    private List<Sprite> sprites = new List<Sprite>();

    // 0 = empty
    // 1 = input
    // 2 = output

    // *clockwise* --> 0201

    //  0 
    // 1=2
    //  0

    private IEnumerator Start()
    {
        yield return null;
        HashSet<Vector2> dirs = new HashSet<Vector2>();
        dirs.Add(Vector2.up);
        dirs.Add(Vector2.right);
        dirs.Add(Vector2.down);
        dirs.Add(Vector2.left);

        foreach (Vector2 dir in dirs)
        {
            if(TileManager.Instance.GetTileObject((Vector3)dir+transform.position, transform.GetComponent<TileObject>().SO.displayName) is TileObject tileObject)
            {
                ConveyorBelt belt = tileObject.GetComponent<ConveyorBelt>();
                if (belt.direction+dir == Vector2.zero)
                {
                    belt.output = this;
                    inputs.Add(belt);
                    belt.Refresh();
                }
                else if( direction == dir)
                {
                    output = belt;
                    belt.inputs.Add(this);
                    belt.Refresh();
                }
            }
        }
        group = ConveyorBeltManager.Instance.GetGroup(uniqeGroupID);
        group.updateFrame.AddListener(UpdateFrame);

        Refresh();
    }

    private void UpdateFrame(int frame)
    {
        spriteRenderer.sprite = sprites[frame];
    }

    private void OnDestroy()
    {
        if (output != null)
            output.inputs.Remove(this);
    }

    public void Refresh()
    {
        code = 0;

        int up = 0;
        int right = 0;
        int down = 0;
        int left = 0;

        foreach(Vector2 input in inputs.Select(r=>r.direction))
        {
            if (input == Vector2.up)
                down = 1;
            else if (input == Vector2.right)
                left = 1;
            else if (input == Vector2.down)
                up = 1;
            else if (input == Vector2.left)
                right = 1;
        }

        Vector2 output = direction;
        if (output == Vector2.up)
            up = 2;
        else if (output == Vector2.right)
            right = 2;
        else if (output == Vector2.down)
            down = 2;
        else if (output == Vector2.left)
            left = 2;

        code = 1000 * up + 100 * right + 10 * down + left;

        //TODO: edges

        sprites = group.sprites[code];
    }
}
