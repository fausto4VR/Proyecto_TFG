using UnityEngine;

public class SortingOrderManager : MonoBehaviour
{
    [SerializeField] private bool hasMark;   

    public int sortingOrderOffset = 0;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        spriteRenderer.sortingOrder = -(int)(transform.position.y * 100) + sortingOrderOffset;

        if(hasMark)
        {
            transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sortingOrder = spriteRenderer.sortingOrder + 500;
        }
    }
}
