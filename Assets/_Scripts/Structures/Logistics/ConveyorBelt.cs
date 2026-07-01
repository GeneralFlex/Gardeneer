using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using Gaskellgames;
using UnityEditor.VersionControl;
using UnityEditor;
using NUnit.Framework;

[System.Serializable]
public class ConveyorBeltShape
{
    public string name;
    public Texture2D spriteSheet;
    public List<Sprite> sprites;
    public List<Vector2> inputs;
    public void SliceSpriteSheet()
    {
        string path = AssetDatabase.GetAssetPath(spriteSheet); 
        Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path); 
        sprites = assets.OfType<Sprite>().OrderBy(s => s.name).ToList();
    }
}

public class ConveyorBelt : MonoBehaviour
{
    [Header("Conveyor settings")]
    public int groupIndex;
    private ConveyorBeltGroup group => ConveyorBeltManager.Instance.groups[groupIndex];
    public Vector2Int direction;
    public ConveyorBelt output;
    public List<ConveyorBelt> inputs;
    public List<ConveyorBeltShape> shapes;
    private ConveyorBeltShape currentShape;

    [Header("References")]
    public SpriteRenderer conveyorSpriteRenderer;
    
    private void Start()
    {
        HashSet<Vector2> dirs = new HashSet<Vector2>();
        dirs.Add(Vector2.up);
        dirs.Add(Vector2.right);
        dirs.Add(Vector2.down);
        dirs.Add(Vector2.left);

        foreach (Vector2 dir in dirs)
        {
            if (TileManager.Instance.GetTileObject((Vector3)dir + transform.position, transform.GetComponent<TileObject>().SO.displayName) is TileObject tileObject)
            {
                ConveyorBelt belt = tileObject.GetComponent<ConveyorBelt>();
                if (belt.direction + dir == Vector2.zero)
                {
                    belt.output = this;
                    inputs.Add(belt);
                    belt.Refresh();
                }
                else if (direction == dir)
                {
                    output = belt;
                    belt.inputs.Add(this);
                    belt.Refresh();
                }
            }
        }
        Refresh();
        group.newFrame.AddListener(Animate);
    }
    [ContextMenu("Slice sprite sheets")]
    private void SliceSpriteSheets()
    {
        foreach (ConveyorBeltShape shape in shapes)
            shape.SliceSpriteSheet();
    }

    public void Refresh()
    {
        List<Vector2> dirs = new List<Vector2>();

        foreach (Vector2 dir in inputs.Select(r => r.direction))
            if(dir!=-direction)
                dirs.Add(dir);

        if (shapes.FirstOrDefault(r => new HashSet<Vector2>(r.inputs).SetEquals(dirs)) is ConveyorBeltShape shape)
            currentShape = shape;
        else currentShape = shapes.FirstOrDefault();
    }

    public void Animate(int frame) 
    {
        if(conveyorSpriteRenderer == null || currentShape == null)
            return;
        conveyorSpriteRenderer.sprite = currentShape.sprites[frame];
    }
}
