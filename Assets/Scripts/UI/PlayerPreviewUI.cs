using UnityEngine;
using UnityEngine.UI;

public class PlayerPreviewUI : MonoBehaviour
{
    private void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        string key   = PlayerPrefs.GetString("EquippedSkin", "skin_0");
        Color32 col  = SkinData.Colors[0];

        for (int i = 0; i < SkinData.Keys.Length; i++)
        {
            if (SkinData.Keys[i] == key) { col = SkinData.Colors[i]; break; }
        }

        foreach (var img in GetComponentsInChildren<Image>())
            img.color = col;
    }
}
