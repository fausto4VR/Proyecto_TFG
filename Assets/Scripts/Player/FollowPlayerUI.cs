using UnityEngine;

public class FollowPlayerUI : MonoBehaviour
{
    public RectTransform inspectLayoutTransform;
    public Vector3 positionOffset;

    void Start()
    {
        positionOffset = new Vector3(0f, 0f, 0f);
    }
    
    void Update()
    {
        inspectLayoutTransform.position = transform.position + positionOffset;
    }
}
