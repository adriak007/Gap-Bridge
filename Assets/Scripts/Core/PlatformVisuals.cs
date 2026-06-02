using UnityEngine;

// Aplica o visual cartoon nos pilares e cria o capô do topo
[RequireComponent(typeof(Platform))]
public class PlatformVisuals : MonoBehaviour
{
    private void Start()
    {
        ApplyPillarColor();
        CreateTopCap();
    }

    private void ApplyPillarColor()
    {
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
            rend.material.color = GameTheme.Pillar;
    }

    private void CreateTopCap()
    {
        Vector3 s = transform.localScale;

        GameObject cap = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cap.name = "PillarCap";
        cap.transform.SetParent(transform);

        // Capô levemente mais largo e fino — compensa escala do pai
        cap.transform.localPosition = new Vector3(0f, 0.5f + 0.025f, 0f);
        cap.transform.localScale    = new Vector3(
            (s.x + 0.25f) / s.x,   // um pouco mais largo
            0.05f / s.y,            // fino
            (s.z + 0.1f) / s.z
        );

        cap.GetComponent<Renderer>().material.color = GameTheme.PillarCap;
        Destroy(cap.GetComponent<BoxCollider>());
    }
}
