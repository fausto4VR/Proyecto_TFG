using UnityEngine;

public class FollowPlayerUI : MonoBehaviour
{
    public RectTransform inspectLayoutTransform;
    public Vector3 inspectPositionOffset;
    public RectTransform mapLayoutTransform;
    public Vector3 mapPositionOffset;

    void Start()
    {
        inspectPositionOffset = new Vector3(0f, 0f, 0f);        
        mapPositionOffset = new Vector3(0f, 0f, 0f);
    }
    
    void Update()
    {
        inspectLayoutTransform.position = transform.position + inspectPositionOffset;
        mapLayoutTransform.position = transform.position + mapPositionOffset;
    }
}
