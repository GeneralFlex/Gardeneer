using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.U2D;

[RequireComponent(typeof(BoxCollider2D))]
public class Harvestable : MonoBehaviour
{
    public float wiggleToHarvest = 2;
    private float wiggleAmount;
    private bool wiggling;

    private float lastRotation = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.rotation.z != 0&&!wiggling) 
        {
            transform.rotation = Quaternion.Euler(0, 0, Mathf.LerpAngle(transform.rotation.z, 0, Time.deltaTime) * 5f);
        }
    }
    private void OnMouseDown()
    {
        wiggling = true;
    }
    private void OnMouseUp()
    {
        wiggling = false;
    }

    private void OnMouseOver()
    {
        CursorManager.Instance.AddCursorPriority("sickle", 14);
    }

    private void OnMouseDrag()
    {
        CursorManager.Instance.AddCursorPriority("handGrab", 15);
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.rotation = Quaternion.Euler(0, 0, Mathf.LerpAngle(-15, 15, Mathf.Clamp((transform.position.x - mousePosition.x)/2, -1, 1) + 0.5f));

        wiggleAmount += Mathf.Abs(transform.rotation.z - lastRotation);

        lastRotation = transform.rotation.z;

        if(wiggleAmount > wiggleToHarvest)
        {
            Harvest();
        }
    }

    public void Harvest()
    {
        Destroy(gameObject);
    }
}
