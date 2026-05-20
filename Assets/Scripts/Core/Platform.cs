using UnityEngine;

public class Platform : MonoBehaviour
{
    [HideInInspector] public float width;

    private void Awake()
    {
        width = transform.localScale.x;
    }

    public float RightEdge => transform.position.x + width / 2f;
    public float LeftEdge  => transform.position.x - width / 2f;
    public float TopEdge   => transform.position.y + transform.localScale.y / 2f;
}
