using UnityEngine;

public class IdleBobbingUI : MonoBehaviour
{
    [SerializeField] private float amplitude = 13f;
    [SerializeField] private float frequency = 1.3f;

    private RectTransform rt;
    private float baseY;

    private void Start()
    {
        rt    = GetComponent<RectTransform>();
        baseY = rt.anchoredPosition.y;
    }

    private void Update()
    {
        if (!rt) return;
        rt.anchoredPosition = new Vector2(
            rt.anchoredPosition.x,
            baseY + Mathf.Sin(Time.time * frequency * Mathf.PI * 2f) * amplitude
        );
    }
}
