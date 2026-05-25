using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    [Header("Textos")]
    [SerializeField] private TMP_Text bestScoreText;

    [Header("Paineis (para implementar depois)")]
    [SerializeField] private GameObject painelLoja;
    [SerializeField] private GameObject painelRanking;
    [SerializeField] private GameObject painelDesafios;
    [SerializeField] private GameObject painelOpcoes;
    [SerializeField] private GameObject painelPerfil;

    private void Start()
    {
        int best = PlayerPrefs.GetInt("BestScore", 0);
        bestScoreText.text = best > 0 ? "MELHOR: " + best : "";

        // Garante paineis fechados
        FecharTodosPaineis();
    }

    // ── Botao principal ────────────────────────────────
    public void OnJogarClick()
    {
        StartCoroutine(CarregarJogo());
    }

    private IEnumerator CarregarJogo()
    {
        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene("GameScene");
    }

    // ── Botoes secundarios ─────────────────────────────
    public void OnLojaClick()      => AbrirPainel(painelLoja);
    public void OnRankingClick()   => AbrirPainel(painelRanking);
    public void OnDesafiosClick()  => AbrirPainel(painelDesafios);
    public void OnOpcoesClick()    => AbrirPainel(painelOpcoes);
    public void OnPerfilClick()    => AbrirPainel(painelPerfil);

    public void FecharTodosPaineis()
    {
        painelLoja?.SetActive(false);
        painelRanking?.SetActive(false);
        painelDesafios?.SetActive(false);
        painelOpcoes?.SetActive(false);
        painelPerfil?.SetActive(false);
    }

    private void AbrirPainel(GameObject painel)
    {
        FecharTodosPaineis();
        painel?.SetActive(true);
    }
}
