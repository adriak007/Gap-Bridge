using UnityEngine;
using UnityEngine.UI;

/// Aplica a cor da skin equipada a todos os Images filhos do preview.
public class PlayerPreviewUI : MonoBehaviour
{
    private static readonly string[]  SkinKeys =
        { "skin_0", "skin_1", "skin_2", "skin_3", "skin_4", "skin_5" };

    private static readonly Color32[] SkinColors =
    {
        new Color32(255, 255, 255, 255),
        new Color32(255, 100,  30, 255),
        new Color32(100, 200, 255, 255),
        new Color32(255, 215,   0, 255),
        new Color32( 80,  80,  80, 255),
        new Color32(  0, 255, 120, 255),
    };

    private void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        string key   = PlayerPrefs.GetString("EquippedSkin", "skin_0");
        Color32 col  = SkinColors[0];

        for (int i = 0; i < SkinKeys.Length; i++)
        {
            if (SkinKeys[i] == key) { col = SkinColors[i]; break; }
        }

        foreach (var img in GetComponentsInChildren<Image>())
            img.color = col;
    }
}
