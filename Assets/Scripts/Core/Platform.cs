using UnityEngine;
using System.Collections;

public class Platform : MonoBehaviour
{
    [Header("Perfect Zone")]
    [SerializeField] private float perfectZoneWidth = 0.4f;
    [SerializeField] private GameObject perfectZoneVisual;

    [Header("Visual do Topo (capa)")]
    [SerializeField] private GameObject topCapRoot;

    [Header("Visual do Corpo (preenchimento)")]
    [SerializeField] private GameObject bodyFill;

    public float Width     => transform.localScale.x;
    public float RightEdge => transform.position.x + Width / 2f;
    public float LeftEdge  => transform.position.x - Width / 2f;
    public float TopEdge   => transform.position.y + transform.localScale.y / 2f;

    public float PerfectLeftEdge  => transform.position.x - perfectZoneWidth / 2f;
    public float PerfectRightEdge => transform.position.x + perfectZoneWidth / 2f;

    private const int ORDER_PERFZONE = 3;

    private void Start()
    {
        if (perfectZoneVisual != null)
            PositionPerfectZone();
        if (topCapRoot != null)
            PositionTopCap();
        if (bodyFill != null)
            PositionBodyFill();
    }

    // Preenche tudo que fica abaixo da capa (o pilar que desce 8 unidades) com a
    // textura de pedra/terra repetida (tiled), em vez de cor solida
    private void PositionBodyFill()
    {
        Vector3 s = transform.localScale;
        float w = s.x;
        float h = s.y;

        float capScale     = Mathf.Min(1f, w / 2f);
        float bodyHeight   = Mathf.Max(0.01f, h - capScale);
        float bodyCenterY  = -capScale / 2f;

        bodyFill.transform.localScale    = new Vector3(1f / w, 1f / h, 1f);
        bodyFill.transform.localPosition = new Vector3(0f, bodyCenterY / h, 0f);

        var sr = bodyFill.GetComponent<SpriteRenderer>();
        if (sr) sr.size = new Vector2(w, bodyHeight);
    }

    // Posiciona as 3 pecas do topo (esquerda/meio tiled/direita) compensando a
    // escala nao-uniforme da plataforma, para a textura nao distorcer ao mudar a largura
    private void PositionTopCap()
    {
        Vector3 s = transform.localScale;
        float w = s.x;
        float h = s.y;

        topCapRoot.transform.localScale = new Vector3(1f / w, 1f / h, 1f);

        float capScale = Mathf.Min(1f, w / 2f);
        float topY = h / 2f - capScale / 2f;

        Transform left  = topCapRoot.transform.Find("CapLeft");
        Transform mid   = topCapRoot.transform.Find("CapMiddle");
        Transform right = topCapRoot.transform.Find("CapRight");

        if (left)
        {
            left.localScale    = Vector3.one * capScale;
            left.localPosition = new Vector3(-w / 2f + capScale / 2f, topY, 0f);
        }

        if (right)
        {
            right.localScale    = Vector3.one * capScale;
            right.localPosition = new Vector3(w / 2f - capScale / 2f, topY, 0f);
        }

        float midWidth = Mathf.Max(0f, w - capScale * 2f);
        if (mid)
        {
            mid.gameObject.SetActive(midWidth > 0.01f);
            mid.localPosition = new Vector3(0f, topY, 0f);
            var midSR = mid.GetComponent<SpriteRenderer>();
            if (midSR) midSR.size = new Vector2(midWidth, capScale);
        }
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
