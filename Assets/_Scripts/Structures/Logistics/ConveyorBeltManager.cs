using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine.Events;
using UnityEditor;
using Unity.VisualScripting;


[System.Serializable]
public enum ConveyorPortType
{
    None,
    Input,
    Output
}

[System.Serializable]
public class ConveyorPort
{
    public ConveyorPortType up;
    public ConveyorPortType right;
    public ConveyorPortType down;
    public ConveyorPortType left;

    public Dictionary<Vector2, ConveyorPortType> portTypes = new Dictionary<Vector2, ConveyorPortType>();

    public bool HasEqualPorts(ConveyorPort port)
    {
        foreach(KeyValuePair<Vector2, ConveyorPortType> kvp in portTypes)
        {
            if(!port.portTypes.ContainsKey(kvp.Key) && portTypes[kvp.Key]==ConveyorPortType.None)
                continue;
            if (!port.portTypes.ContainsKey(kvp.Key) || port.portTypes[kvp.Key] != kvp.Value)
                return false;
        }
        return true;
    }
    public void RebuildDictionary()
    {
        portTypes.Clear();

        portTypes[Vector2.up] = up;
        portTypes[Vector2.right] = right;
        portTypes[Vector2.down] = down;
        portTypes[Vector2.left] = left;
    }
}

[System.Serializable]
public class ConveyorBeltDirection
{
    public string displayName;
    public Vector2 direction;
    public List<ConveyorPort> ports;
    public Texture2D sheet;
    //[HideInInspector]
    public List<Sprite> sprites;
}

[System.Serializable]
public class ConveyorBeltGroup
{
    [Header("Config")]
    public int ID;
    public List<ConveyorBeltDirection> directions;

    [Header("Animation")]
    public float speed;
    public int animationFrame = 0;
    [HideInInspector]
    public UnityEvent<int> updateFrame;
}

public class ConveyorBeltManager : MonoBehaviour
{
    public List<ConveyorBeltGroup> conveyorGroups = new List<ConveyorBeltGroup>();
    public int index=0;

    public static ConveyorBeltManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        foreach(ConveyorBeltGroup cg in conveyorGroups)
        {
            foreach(ConveyorBeltDirection direction in cg.directions)
            {
                foreach (ConveyorPort port in direction.ports)
                {
                    port.RebuildDictionary();
                }
            }
        }
    }

    private void Start()
    {
        foreach (ConveyorBeltGroup cg in conveyorGroups)
        {
            StartCoroutine(Animate(cg));
        }
    }

    public ConveyorBeltDirection GetDirection(int groupID, ConveyorPort port)
    {
        ConveyorBeltGroup group = conveyorGroups.Where(r => r.ID == groupID).ToList().First();
        return group.directions.Where(r => r.ports.Any(l => l.HasEqualPorts(port)==true)).ToList().First();
    }

    [ContextMenu("debug")]
    public void debug()
    {
        HashSet<Vector2> dirs = new HashSet<Vector2>();
        dirs.Add(Vector2.up);
        dirs.Add(Vector2.right);
        dirs.Add(Vector2.down);
        dirs.Add(Vector2.left);
        foreach (var group in conveyorGroups)
        {
            Dictionary<ConveyorPort, ConveyorBeltDirection> ports = new Dictionary<ConveyorPort, ConveyorBeltDirection>();

            foreach (ConveyorBeltDirection direction in group.directions)
            {
                foreach (ConveyorPort port in direction.ports)
                {
                    foreach(Vector2 dir in dirs)
                    {
                        try
                        {
                            Debug.Log(port.portTypes[dir]);
                        }
                        catch
                        {
                            Debug.Log(direction.displayName);
                        }
                    }
                }
            }
        }
    }

    [ContextMenu("Check For Duplicates")]
    public void CheckForDuplicates()
    {
        foreach (var group in conveyorGroups)
        {
            Dictionary<ConveyorPort, ConveyorBeltDirection> ports = new Dictionary<ConveyorPort, ConveyorBeltDirection>();

            foreach (ConveyorBeltDirection direction in group.directions)
            {
                foreach (ConveyorPort port in direction.ports)
                {
                    foreach (ConveyorPort port2 in ports.Keys) {
                        if(port2.HasEqualPorts(port))
                            Debug.LogWarning("Found duplicate:      " + direction.displayName + " [" + direction.ports.IndexOf(port) + "]    x    " + ports[port2].displayName + " [" + ports[port2].ports.IndexOf(port2) + "]");
                    }
                    try
                    {
                        ports.Add(port, direction);
                    }
                    catch { }
                }
            }
        }
        Debug.Log("Check done");
    }

    [ContextMenu("Slice Sprite Sheet")]
    private void SliceSpriteSheet()
    {
        foreach (ConveyorBeltGroup group in conveyorGroups)
        {
            foreach (ConveyorBeltDirection direction in group.directions)
            {
                if (direction == null || direction.sheet == null)
                    continue;

                string path = AssetDatabase.GetAssetPath(direction.sheet);

                Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);

                direction.sprites ??= new List<Sprite>();
                direction.sprites.Clear();

                foreach (Object asset in assets)
                    if (asset is Sprite sprite)
                        direction.sprites.Add(sprite);

                direction.sprites = direction.sprites.OrderBy(s => s.name).ToList();
            }
        }
    }

    public ConveyorBeltGroup GetGroup(int ID)
    {
        return conveyorGroups.Where(r => r.ID == ID).ToList().First();
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
}
