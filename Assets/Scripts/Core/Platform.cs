using UnityEngine;
using System.Collections;

public class Platform : MonoBehaviour
{
    [Header("Perfect Zone")]
    [SerializeField] private float perfectZoneWidth = 0.4f;
    [SerializeField] private GameObject perfectZoneVisual;

    public float Width     => transform.localScale.x;
    public float RightEdge => transform.position.x + Width / 2f;
    public float LeftEdge  => transform.position.x - Width / 2f;
    public float TopEdge   => transform.position.y + transform.localScale.y / 2f;

    public float PerfectLeftEdge  => transform.position.x - perfectZoneWidth / 2f;
    public float PerfectRightEdge => transform.position.x + perfectZoneWidth / 2f;

    private void Start()
    {
        if (perfectZoneVisual != null)
            PositionPerfectZone();
    }

    private void PositionPerfectZone()
    {
        Vector3 s = transform.localScale;
        perfectZoneVisual.transform.localPosition = new Vector3(0f, 0.5f, 0f);
        perfectZoneVisual.transform.localScale = new Vector3(
            perfectZoneWidth / s.x,
            0.06f / s.y,
            1.1f  / s.z
        );
    }

    public void TriggerPerfectFeedback()
    {
        if (perfectZoneVisual != null)
            StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        Renderer rend = perfectZoneVisual.GetComponent<Renderer>();
        Color original = rend.material.color;
        rend.material.color = Color.white;
        yield return new WaitForSeconds(0.35f);
        rend.material.color = original;
    }
}
