using UnityEngine;

public class FollowPlayerUI : MonoBehaviour
{
    [Header("UI Objects Section")]
    public RectTransform inspectLayoutTransform;    
    public RectTransform mapLayoutTransform;

    [Header("Variable Section")]
    [SerializeField] private Vector3 inspectPositionOffset = new Vector3(0f, 0f, 0f);
    [SerializeField] private Vector3 mapPositionOffset = new Vector3(0f, 0f, 0f);
    
    void Update()
    {
        inspectLayoutTransform.position = transform.position + inspectPositionOffset;
        mapLayoutTransform.position = transform.position + mapPositionOffset;
    }
}
