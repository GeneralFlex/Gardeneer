using Unity.VisualScripting;
using UnityEngine;

public class MouseShaderController : MonoBehaviour
{
    public float radius = 0;
    public float targetRadius = 0;
    private SpriteRenderer[] spriteRenderers;
    private MaterialPropertyBlock mpb;

    void Awake()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        mpb = new MaterialPropertyBlock();
    }
    public void SetTargetRadius(float radius = 2f)
    {
        if (radius == 0)
            radius = -1;
        targetRadius = radius;
    }

    void Update()
    {
        if (Mathf.Abs(targetRadius - radius) > 0.1f)
        {
            radius = Mathf.MoveTowards(radius, targetRadius, Time.deltaTime * 5 * Mathf.Abs(targetRadius - radius));
        }

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //mousePos = Vector2Int.RoundToInt(mousePos);

        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            spriteRenderer.GetPropertyBlock(mpb);

            mpb.SetFloat("_Radius", radius);
            mpb.SetVector("_MousePos", mousePos);
            spriteRenderer.SetPropertyBlock(mpb);
        }
    }
}
