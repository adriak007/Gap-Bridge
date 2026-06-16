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

    // Sorting orders para camadas visuais
    private const int ORDER_PILLAR    = 0;
    private const int ORDER_TOP_CAP   = 1;
    private const int ORDER_HIGHLIGHT = 2;
    private const int ORDER_PERFZONE  = 3;

    private static readonly Color32 COLOR_PILLAR    = new Color32(48,  62,  38, 255);
    private static readonly Color32 COLOR_TOP_CAP   = new Color32(75, 140,  45, 255);
    private static readonly Color32 COLOR_HIGHLIGHT = new Color32(118, 192,  58, 255);

    private void Awake()
    {
        SetupVisuals();
    }

    private void SetupVisuals()
    {
        var sr = GetComponent<SpriteRenderer>();
        if (!sr) return;

        sr.color        = COLOR_PILLAR;
        sr.sortingOrder = ORDER_PILLAR;

        Sprite spr = sr.sprite;

        if (!transform.Find("TopCap"))
        {
            var cap = new GameObject("TopCap");
            cap.transform.SetParent(transform, false);
            // Em local-space (unidade 0–1): cobre os 10% superiores do pilar
            cap.transform.localPosition = new Vector3(0f, 0.45f, 0f);
            cap.transform.localScale    = new Vector3(1f, 0.10f, 1f);
            var capSR = cap.AddComponent<SpriteRenderer>();
            capSR.sprite       = spr;
            capSR.color        = COLOR_TOP_CAP;
            capSR.sortingOrder = ORDER_TOP_CAP;
        }

        if (!transform.Find("TopHighlight"))
        {
            var hl = new GameObject("TopHighlight");
            hl.transform.SetParent(transform, false);
            // Faixa fina no topo exato da plataforma
            hl.transform.localPosition = new Vector3(0f, 0.494f, 0f);
            hl.transform.localScale    = new Vector3(1f, 0.012f, 1f);
            var hlSR = hl.AddComponent<SpriteRenderer>();
            hlSR.sprite       = spr;
            hlSR.color        = COLOR_HIGHLIGHT;
            hlSR.sortingOrder = ORDER_HIGHLIGHT;
        }
    }

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
        var pzSR = perfectZoneVisual.GetComponent<SpriteRenderer>();
        if (pzSR) pzSR.sortingOrder = ORDER_PERFZONE;
    }

    public void TriggerPerfectFeedback()
    {
        if (perfectZoneVisual != null)
            StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        SpriteRenderer sr = perfectZoneVisual.GetComponent<SpriteRenderer>();
        Color original = sr.color;
        sr.color = Color.white;
        yield return new WaitForSeconds(0.35f);
        sr.color = original;
    }
}
