using UnityEngine;

public class SortingOrderController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public int sortingOrderOffset = 0;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        spriteRenderer.sortingOrder = -(int)(transform.position.y * 100) + sortingOrderOffset;
    }
}
