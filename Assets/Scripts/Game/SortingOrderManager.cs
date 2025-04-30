using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SortingOrderManager : MonoBehaviour
{
    [Header("Variable Section")]
    [SerializeField] int sortingOrderOffset = 0;
    [SerializeField] private bool hasMark;
    [SerializeField] private bool isNecesaryShowUp;

    private SpriteRenderer spriteRenderer;


    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (isNecesaryShowUp) sortingOrderOffset += 100;
        
        UpdateSortingOrder();
    }

    // Método para actualizar el orden de renderizado del objeto en función de su posición
    public void UpdateSortingOrder()
    {
        int newOrder = -(int)(transform.position.y * 100) + sortingOrderOffset;
        spriteRenderer.sortingOrder = newOrder;

        if (hasMark && transform.childCount > 0)
        {
            var markRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
            if (markRenderer != null) markRenderer.sortingOrder = newOrder + 500;
        }
    }
}
