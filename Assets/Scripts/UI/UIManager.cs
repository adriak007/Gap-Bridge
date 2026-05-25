using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Score")]
    [SerializeField] private TMP_Text scoreText;

    [Header("Perfect")]
    [SerializeField] private TMP_Text perfectText;

    [Header("Combo")]
    [SerializeField] private TMP_Text  comboText;
    [SerializeField] private Image     comboTimerBar;
    [SerializeField] private GameObject comboContainer;

    private Vector3 perfectOriginalPos;
    private CanvasGroup perfectCanvasGroup;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        perfectOriginalPos  = perfectText.transform.localPosition;
        perfectCanvasGroup  = perfectText.GetComponent<CanvasGroup>();
        if (perfectCanvasGroup == null)
            perfectCanvasGroup = perfectText.gameObject.AddComponent<CanvasGroup>();

        perfectText.gameObject.SetActive(false);
        comboContainer.SetActive(false);
        UpdateScore(0);
    }

    // ── Score ──────────────────────────────────────────
    public void UpdateScore(int score)
    {
        scoreText.text = score.ToString();
    }

    // ── Combo ──────────────────────────────────────────
    public void ShowMultiplier(int multiplier)
    {
        comboContainer.SetActive(true);
        comboText.text = "x" + multiplier;

        // Animacao de pop no texto do combo
        StopCoroutine(nameof(PopCombo));
        StartCoroutine(nameof(PopCombo));
    }

    public void UpdateComboTimer(float normalizedValue)
    {
        if (comboTimerBar != null)
            comboTimerBar.fillAmount = Mathf.Clamp01(normalizedValue);
    }

    public void HideCombo()
    {
        comboContainer.SetActive(false);
    }

    // ── PERFECT! ───────────────────────────────────────
    public void ShowPerfect()
    {
        StopCoroutine(nameof(PerfectAnimation));
        StartCoroutine(nameof(PerfectAnimation));
    }

    // ── Coroutines ─────────────────────────────────────
    private IEnumerator PopCombo()
    {
        comboText.transform.localScale = Vector3.one * 1.4f;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 0.2f;
            comboText.transform.localScale = Vector3.Lerp(Vector3.one * 1.4f, Vector3.one, t);
            yield return null;
        }
        comboText.transform.localScale = Vector3.one;
    }

    private IEnumerator PerfectAnimation()
    {
        perfectText.gameObject.SetActive(true);
        perfectText.transform.localPosition = perfectOriginalPos;
        perfectText.transform.localScale    = Vector3.zero;
        perfectCanvasGroup.alpha = 1f;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 0.35f;
            perfectText.transform.localScale = Vector3.one * EaseOutBack(Mathf.Clamp01(t));
            yield return null;
        }
        perfectText.transform.localScale = Vector3.one;

        yield return new WaitForSeconds(0.45f);

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

    private float EaseOutBack(float t)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
    }
}
