using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text bestScoreText;
    [SerializeField] private TMP_Text tapToPlayText;

    private bool canPlay = false;

    private void Start()
    {
        int best = PlayerPrefs.GetInt("BestScore", 0);
        bestScoreText.text = best > 0 ? "MELHOR: " + best : "";

        StartCoroutine(PulseTapText());

        // Pequeno delay para nao iniciar imediatamente ao entrar na cena
        Invoke(nameof(EnablePlay), 0.5f);
    }

    private void EnablePlay() => canPlay = true;

    private void Update()
    {
        if (!canPlay) return;

        bool tapped = (Mouse.current      != null && Mouse.current.leftButton.wasPressedThisFrame)
                   || (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame);

        if (tapped)
            StartCoroutine(LoadGame());
    }

    private IEnumerator LoadGame()
    {
        canPlay = false;
        // Pequena pausa antes de carregar para nao cortar a animacao
        yield return new WaitForSeconds(0.15f);
        SceneManager.LoadScene("GameScene");
    }

    // Animacao de pulso no texto "TOQUE PARA JOGAR"
    private IEnumerator PulseTapText()
    {
        while (true)
        {
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / 0.8f;
                tapToPlayText.alpha = Mathf.Lerp(0.2f, 1f, t);
                yield return null;
            }
            t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / 0.8f;
                tapToPlayText.alpha = Mathf.Lerp(1f, 0.2f, t);
                yield return null;
            }
        }
    }
}
