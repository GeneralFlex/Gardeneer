using UnityEngine;

public class GrabbableItem : MonoBehaviour
{
    Transform target;
    Transform mouse;
    Transform sprite;
    Vector2 velocity;
    bool isDragged = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CircleCollider2D cc = gameObject.AddComponent<CircleCollider2D>();
        cc.radius = 0.2f;
        sprite = transform.Find("sprite");
    }

    // Update is called once per frame
    void Update()
    {
        if (isDragged && mouse != null)
        {
            mouse.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        if (target != null)
        {
            velocity = (target.position - transform.position) * 10f;
            transform.position = new Vector2(Mathf.Lerp(transform.position.x, target.position.x, Time.deltaTime * 10f), Mathf.Lerp(transform.position.y, target.position.y, Time.deltaTime * 10f* (1+Mathf.Pow(Vector2.Distance(target.position,transform.position),2))));
        } else if(velocity.magnitude > 0.1f)
        {
            velocity *= 0.95f;
            transform.position = new Vector2(transform.position.x + velocity.x * Time.deltaTime, transform.position.y + velocity.y * Time.deltaTime);
        }
        if (sprite != null && isDragged)
        {
            sprite.rotation = Quaternion.Euler(0, 0, Mathf.LerpAngle(-15, 15, Mathf.Clamp(transform.position.x - target.position.x, -1, 1) + 0.5f));
        }
        else if (sprite != null && sprite.rotation.z != 0)
        {
            sprite.rotation = Quaternion.Euler(0, 0, Mathf.LerpAngle(sprite.rotation.z, 0, Time.deltaTime) * 5f);
        }
    }

    public void Grab(Transform origin)
    {
        target = origin;
    }
    public void Release()
    {
        target = null;
    }

    private void OnMouseDown()
    {
        isDragged = true;
        mouse = new GameObject("Mouse").transform;
        mouse.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Grab(mouse);
    }

    private void OnMouseUp()
    {
        isDragged = false;
        if (mouse != null)
        {
            Destroy(mouse.gameObject);
        }
        Release();
    }
}
