using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenEffects : MonoBehaviour
{
    public static ScreenEffects Instance { get; private set; }

    [SerializeField] private Image flashImage; // Image transparente que cobre a tela

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    // Tela treme ao falhar
    public void ShakeCamera(float duration = 0.35f, float magnitude = 0.18f)
    {
        StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    // Flash vermelho ao falhar
    public void FlashRed()
    {
        StartCoroutine(FlashRoutine(new Color(1f, 0f, 0f, 0.35f)));
    }

    // Flash branco ao acertar perfect
    public void FlashWhite()
    {
        StartCoroutine(FlashRoutine(new Color(1f, 1f, 1f, 0.25f)));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        Camera cam      = Camera.main;
        Vector3 origin  = cam.transform.localPosition;
        float elapsed   = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            cam.transform.localPosition = new Vector3(origin.x + x, origin.y + y, origin.z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        cam.transform.localPosition = origin;
    }

    private IEnumerator FlashRoutine(Color color)
    {
        if (flashImage == null) yield break;

        flashImage.color = color;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 0.3f;
            flashImage.color = Color.Lerp(color, Color.clear, t);
            yield return null;
        }

        flashImage.color = Color.clear;
    }
}
