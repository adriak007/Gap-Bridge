using UnityEngine;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text perfectText;

    private Vector3 perfectOriginalPos;
    private CanvasGroup perfectCanvasGroup;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        perfectOriginalPos = perfectText.transform.localPosition;

        perfectCanvasGroup = perfectText.GetComponent<CanvasGroup>();
        if (perfectCanvasGroup == null)
            perfectCanvasGroup = perfectText.gameObject.AddComponent<CanvasGroup>();

        perfectText.gameObject.SetActive(false);
        UpdateScore(0);
    }

    public void UpdateScore(int score)
    {
        scoreText.text = score.ToString();
    }

    public void ShowPerfect()
    {
        StopCoroutine(nameof(PerfectAnimation));
        StartCoroutine(nameof(PerfectAnimation));
    }

    private IEnumerator PerfectAnimation()
    {
        // Reseta estado inicial
        perfectText.gameObject.SetActive(true);
        perfectText.transform.localPosition = perfectOriginalPos;
        perfectText.transform.localScale    = Vector3.zero;
        perfectCanvasGroup.alpha = 1f;

        // Pop in com bounce elastico
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 0.35f;
            perfectText.transform.localScale = Vector3.one * EaseOutBack(Mathf.Clamp01(t));
            yield return null;
        }
        perfectText.transform.localScale = Vector3.one;

        yield return new WaitForSeconds(0.45f);

        // Sobe e desaparece
        t = 0f;
        Vector3 startPos = perfectText.transform.localPosition;
        while (t < 1f)
        {
            t += Time.deltaTime / 0.4f;
            perfectCanvasGroup.alpha = 1f - t;
            perfectText.transform.localPosition = startPos + Vector3.up * 80f * t;
            yield return null;
        }

        perfectText.gameObject.SetActive(false);
        perfectText.transform.localPosition = perfectOriginalPos;
    }

    // Curva com overshoot — da o efeito de "balançada"
    private float EaseOutBack(float t)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
    }
}
