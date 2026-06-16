using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    [Header("Menu Principal")]
    [SerializeField] private TMP_Text bestScoreText;
    [SerializeField] private TMP_Text coinsText;

    [Header("Paineis")]
    [SerializeField] private GameObject painelLoja;
    [SerializeField] private GameObject painelRanking;
    [SerializeField] private GameObject painelOpcoes;

    [Header("Loja — cards (6 itens)")]
    [SerializeField] private ShopItemCard[] shopCards;
    [SerializeField] private TMP_Text       lojaCoinsText;

    [Header("Ranking — entradas (5 linhas)")]
    [SerializeField] private RankingEntry[] rankingEntries;

    [Header("Preview do Personagem")]
    [SerializeField] private PlayerPreviewUI playerPreview;

    [Header("Opcoes")]
    [SerializeField] private Toggle musicToggle;
    [SerializeField] private Toggle sfxToggle;

    private static readonly ShopSkin[] Skins =
    {
        new ShopSkin("Classico",  0,   new Color32(255, 255, 255, 255), "skin_0"),
        new ShopSkin("Fogo",      50,  new Color32(255, 100,  30, 255), "skin_1"),
        new ShopSkin("Gelo",      80,  new Color32(100, 200, 255, 255), "skin_2"),
        new ShopSkin("Ouro",      120, new Color32(255, 215,   0, 255), "skin_3"),
        new ShopSkin("Sombra",    150, new Color32( 80,  80,  80, 255), "skin_4"),
        new ShopSkin("Neon",      200, new Color32(  0, 255, 120, 255), "skin_5"),
    };

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        FecharTodosPaineis();
        RefreshCoinsDisplay();

        int best = PlayerPrefs.GetInt("BestScore", 0);
        if (bestScoreText) bestScoreText.text = best > 0 ? "MELHOR: " + best : "";

        if (musicToggle)
        {
            musicToggle.SetIsOnWithoutNotify(PlayerPrefs.GetInt("MusicOn", 1) == 1);
            musicToggle.onValueChanged.AddListener(OnMusicToggle);
        }
        if (sfxToggle)
        {
            sfxToggle.SetIsOnWithoutNotify(PlayerPrefs.GetInt("SFXOn", 1) == 1);
            sfxToggle.onValueChanged.AddListener(OnSFXToggle);
        }
    }

    // ── Navegação ─────────────────────────────────────────

    public void OnJogarClick()
    {
        StartCoroutine(CarregarJogo());
    }

    private IEnumerator CarregarJogo()
    {
        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene("GameScene");
    }

    public void OnLojaClick()
    {
        AbrirPainel(painelLoja);
        PopulateLoja();
    }

    public void OnRankingClick()
    {
        AbrirPainel(painelRanking);
        PopulateRanking();
    }

    public void OnTutorialClick()
    {
        PlayerPrefs.SetInt("TutorialMode", 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene("GameScene");
    }

    public void OnOpcoesClick()
    {
        AbrirPainel(painelOpcoes);
    }

    public void FecharTodosPaineis()
    {
        if (painelLoja)    painelLoja.SetActive(false);
        if (painelRanking) painelRanking.SetActive(false);
        if (painelOpcoes)  painelOpcoes.SetActive(false);
    }

    private void AbrirPainel(GameObject painel)
    {
        FecharTodosPaineis();
        if (painel) painel.SetActive(true);
    }

    // ── Coins ─────────────────────────────────────────────

    void RefreshCoinsDisplay()
    {
        int coins = PlayerPrefs.GetInt("Coins", 0);
        if (coinsText)     coinsText.text     = coins.ToString();
        if (lojaCoinsText) lojaCoinsText.text = coins.ToString();
    }

    // ── Loja ──────────────────────────────────────────────

    void PopulateLoja()
    {
        RefreshCoinsDisplay();
        if (shopCards == null) return;

        string equipped = PlayerPrefs.GetString("EquippedSkin", "skin_0");

        for (int i = 0; i < shopCards.Length && i < Skins.Length; i++)
        {
            if (!shopCards[i]) continue;
            var skin       = Skins[i];
            bool unlocked  = skin.price == 0 || PlayerPrefs.GetInt(skin.key + "_unlocked", 0) == 1;
            bool isEquipped = equipped == skin.key;
            shopCards[i].Setup(skin, unlocked, isEquipped, OnBuySkin, OnEquipSkin);
        }
    }

    void OnBuySkin(ShopSkin skin)
    {
        int coins = PlayerPrefs.GetInt("Coins", 0);
        if (coins < skin.price) return;

        PlayerPrefs.SetInt("Coins", coins - skin.price);
        PlayerPrefs.SetInt(skin.key + "_unlocked", 1);
        PlayerPrefs.Save();
        OnEquipSkin(skin);
    }

    void OnEquipSkin(ShopSkin skin)
    {
        PlayerPrefs.SetString("EquippedSkin", skin.key);
        PlayerPrefs.Save();
        PopulateLoja();
        if (playerPreview) playerPreview.Refresh();
    }

    // ── Ranking ───────────────────────────────────────────

    void PopulateRanking()
    {
        if (rankingEntries == null) return;

        var scores = LoadTopScores();
        for (int i = 0; i < rankingEntries.Length; i++)
        {
            if (!rankingEntries[i]) continue;
            if (i < scores.Count)
                rankingEntries[i].Setup(i + 1, scores[i]);
            else
                rankingEntries[i].SetEmpty();
        }
    }

    public static List<int> LoadTopScores()
    {
        var list = new List<int>();
        for (int i = 0; i < 5; i++)
        {
            int s = PlayerPrefs.GetInt("TopScore_" + i, 0);
            if (s > 0) list.Add(s);
        }
        return list;
    }

    // ── Opções ────────────────────────────────────────────

    public void OnMusicToggle(bool on)
    {
        PlayerPrefs.SetInt("MusicOn", on ? 1 : 0);
        PlayerPrefs.Save();
        if (AudioManager.Instance) AudioManager.Instance.SetMusicEnabled(on);
    }

    public void OnSFXToggle(bool on)
    {
        PlayerPrefs.SetInt("SFXOn", on ? 1 : 0);
        PlayerPrefs.Save();
        if (AudioManager.Instance) AudioManager.Instance.SetSFXEnabled(on);
    }
}

[System.Serializable]
public class ShopSkin
{
    public string  name;
    public int     price;
    public Color32 color;
    public string  key;

    public ShopSkin(string name, int price, Color32 color, string key)
    {
        this.name = name; this.price = price;
        this.color = color; this.key = key;
    }
}
