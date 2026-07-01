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
    public int groupID = 1;
    public Vector2 direction;
    public ConveyorBeltDirection conveyorBeltDirection;
    public ConveyorBelt output;
    public List<ConveyorBelt> inputs = new List<ConveyorBelt>();

    private ConveyorPort conveyorPort = new ConveyorPort();

    [Header("References")]
    public SpriteRenderer spriteRenderer;
    private ConveyorBeltGroup group;

    private IEnumerator Start()
    {
        yield return null;

        group = ConveyorBeltManager.Instance.GetGroup(groupID);

        //setup conveyor port
        HashSet<Vector2> dirs = new HashSet<Vector2>();
        dirs.Add(Vector2.up);
        dirs.Add(Vector2.right);
        dirs.Add(Vector2.down);
        dirs.Add(Vector2.left);

        conveyorPort.portTypes.Add(direction, ConveyorPortType.Output);

        foreach (var dir in dirs)
        {
            if (TileManager.Instance.GetTileObject((Vector3)dir + transform.position, transform.GetComponent<TileObject>().SO.displayName) is TileObject tileObject)
            {
                ConveyorBelt neighbourConveyor = tileObject.GetComponent<ConveyorBelt>();
                if (neighbourConveyor.direction + dir == Vector2.zero)
                {
                    conveyorPort.portTypes.Add(dir, ConveyorPortType.Input);
                    neighbourConveyor.Refresh();
                    inputs.Add(neighbourConveyor);
                }
                if (dir == direction)
                { 
                    neighbourConveyor.conveyorPort.portTypes.Add(dir*-1, ConveyorPortType.Input);
                    neighbourConveyor.inputs.Add(this);
                    output = neighbourConveyor;
                    neighbourConveyor.Refresh();
                }
            }
        }

        Refresh();

        group.updateFrame.AddListener(UpdateFrame);
    }

    private void UpdateFrame(int frame)
    {
        spriteRenderer.sprite = conveyorBeltDirection.sprites[frame];
    }

    private void OnDestroy()
    {
        if (!Application.isPlaying)
                return;

        if (output != null)
        {
            output.inputs.Remove(this);
            output.conveyorPort.portTypes.Remove(direction);
            output.Refresh();
        }
    }

    public void Refresh()
    {
        conveyorBeltDirection = ConveyorBeltManager.Instance.GetDirection(group.ID, conveyorPort);
    }
}
