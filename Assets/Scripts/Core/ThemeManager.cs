using UnityEngine;

// Paleta de cores cartoon colorido — unica fonte de verdade visual do jogo
public static class GameTheme
{
    public static readonly Color Sky         = new Color(0.53f, 0.81f, 0.98f); // #87CEEB azul claro
    public static readonly Color Pillar      = new Color(0.18f, 0.42f, 0.31f); // #2d6a4f verde floresta
    public static readonly Color PillarCap   = new Color(0.25f, 0.58f, 0.42f); // #3d9469 verde claro
    public static readonly Color Bridge      = new Color(0.55f, 0.37f, 0.24f); // #8B5E3C madeira marrom
    public static readonly Color PlayerBody  = new Color(1.00f, 0.39f, 0.28f); // #FF6348 coral
    public static readonly Color PlayerHead  = new Color(1.00f, 0.76f, 0.60f); // #FFC299 pele clara
    public static readonly Color PerfectZone = new Color(1.00f, 0.84f, 0.00f); // #FFD700 dourado
}

public class ThemeManager : MonoBehaviour
{
    private void Awake()
    {
        ApplyCamera();
        ApplyLight();
    }

    private void ApplyCamera()
    {
        Camera.main.backgroundColor = GameTheme.Sky;
    }

    private void ApplyLight()
    {
        Light sun = FindAnyObjectByType<Light>();
        if (sun == null) return;
        sun.color     = new Color(1f, 0.95f, 0.84f); // luz solar quente
        sun.intensity = 1.2f;
        sun.transform.eulerAngles = new Vector3(45f, -30f, 0f);
    }
}
