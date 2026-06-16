using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }
    public static bool IsActive => PlayerPrefs.GetInt("TutorialMode", 0) == 1;

    private enum TutState { Intro, WatchBridge, SpikeIntro, PlayerCrosses, Complete }
    private TutState _state = TutState.Intro;

    private TMP_Text _mainText;
    private TMP_Text _subText;

    private void Awake()
    {
        if (!IsActive) { Destroy(gameObject); return; }
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        BuildPanel();
        PlayerController.OnArrivedEvent += OnPlayerArrived;
        if (BridgeController.Instance) BridgeController.Instance.LockRelease = true;
        SetText("Aperte e segure a tela", "para crescer a ponte");
    }

    private void OnDestroy()
    {
        PlayerController.OnArrivedEvent -= OnPlayerArrived;
        if (BridgeController.Instance) BridgeController.Instance.LockRelease = false;
    }

    private void Update()
    {
        if (!BridgeController.Instance || !GameManager.Instance) return;

        if (_state == TutState.Intro && BridgeController.Instance.State == BridgeState.Growing)
        {
            _state = TutState.WatchBridge;
            SetText("Continue segurando!", "");
        }
        else if (_state == TutState.WatchBridge && BridgeController.Instance.State == BridgeState.Growing)
        {
            Platform next = GameManager.Instance.NextPlatform;
            if (next && BridgeController.Instance.GetBridgeTipX() >= next.PerfectLeftEdge)
            {
                BridgeController.Instance.LockRelease = false;
                BridgeController.Instance.ForceStopGrowing();
                _state = TutState.SpikeIntro;
                StartCoroutine(PerfectPause());
            }
        }
    }

    private IEnumerator PerfectPause()
    {
        SetText("PERFEITO!", "Voce acertou o ponto exato!");
        yield return new WaitForSeconds(0.6f);
        SetText("A ponte vai cair agora...", "Caminhe ate a proxima plataforma!");
    }

    private void OnPlayerArrived()
    {
        StopAllCoroutines();
        if (_state == TutState.SpikeIntro)
            StartCoroutine(SpikeStepSetup());
        else if (_state == TutState.PlayerCrosses)
            StartCoroutine(TutorialComplete());
    }

    private IEnumerator SpikeStepSetup()
    {
        yield return new WaitForSeconds(0.5f);
        if (ObstacleSpawner.Instance)
            ObstacleSpawner.Instance.ForceSpawnCenter(GameManager.Instance.NextPlatform);

        SetText("Cuidado! Ha um obstaculo!", "Voce vai precisar PULAR sobre ele!");
        yield return new WaitForSeconds(2.2f);
        SetText("Aperte e segure a tela...", "...e pule na hora certa!");
        _state = TutState.PlayerCrosses;
    }

    private IEnumerator TutorialComplete()
    {
        _state = TutState.Complete;
        PlayerPrefs.DeleteKey("TutorialMode");
        PlayerPrefs.Save();
        SetText("Parabens!", "Voce esta pronto para jogar!");
        yield return new WaitForSeconds(1f);
        ShowCompletionButtons();
    }

    public void OnPlayerFailed()
    {
        if (_state == TutState.PlayerCrosses)
        {
            StopAllCoroutines();
            StartCoroutine(RetrySpike());
        }
        else
        {
            PlayerPrefs.SetInt("TutorialMode", 1);
            PlayerPrefs.Save();
            SceneManager.LoadScene("GameScene");
        }
    }

    private IEnumerator RetrySpike()
    {
        SetText("Nao foi dessa vez!", "Vamos tentar de novo...");
        yield return new WaitForSeconds(1.5f);

        if (ObstacleSpawner.Instance) ObstacleSpawner.Instance.LimparObstaculos();
        if (BridgeController.Instance) BridgeController.Instance.ResetBridge();
        if (BridgeVerifier.Instance)  BridgeVerifier.Instance.ResetVerifier();
        if (ObstacleSpawner.Instance)
            ObstacleSpawner.Instance.ForceSpawnCenter(GameManager.Instance.NextPlatform);

        PlayerController.Instance.TutorialReset(GameManager.Instance.CurrentPlatform);

        SetText("Aperte e segure a tela...", "...e pule sobre o obstaculo!");
    }

    // ── UI ────────────────────────────────────────────────

    private void SetText(string main, string sub)
    {
        if (_mainText) _mainText.text = main;
        if (_subText)  _subText.text  = sub;
    }

    private void BuildPanel()
    {
        var canvas = FindAnyObjectByType<Canvas>();
        if (!canvas) return;

        // Instruction panel (bottom strip)
        var panelGO = new GameObject("TutorialPanel");
        panelGO.transform.SetParent(canvas.transform, false);
        var panelRT = panelGO.AddComponent<RectTransform>();
        panelRT.anchorMin = new Vector2(0f, 0f);
        panelRT.anchorMax = new Vector2(1f, 0.28f);
        panelRT.sizeDelta = Vector2.zero;
        panelRT.anchoredPosition = Vector2.zero;
        var panelBG = panelGO.AddComponent<Image>();
        panelBG.color = new Color(0f, 0f, 0f, 0.75f);
        panelBG.raycastTarget = false;

        // Main instruction text
        var mainGO = new GameObject("MainText");
        mainGO.transform.SetParent(panelGO.transform, false);
        var mainRT = mainGO.AddComponent<RectTransform>();
        mainRT.anchorMin = new Vector2(0.04f, 0.52f);
        mainRT.anchorMax = new Vector2(0.96f, 1.0f);
        mainRT.sizeDelta = Vector2.zero;
        _mainText = mainGO.AddComponent<TextMeshProUGUI>();
        _mainText.alignment = TextAlignmentOptions.Center;
        _mainText.fontSize = 54f;
        _mainText.fontStyle = FontStyles.Bold;
        _mainText.color = Color.white;
        _mainText.raycastTarget = false;

        // Sub text
        var subGO = new GameObject("SubText");
        subGO.transform.SetParent(panelGO.transform, false);
        var subRT = subGO.AddComponent<RectTransform>();
        subRT.anchorMin = new Vector2(0.04f, 0.04f);
        subRT.anchorMax = new Vector2(0.96f, 0.50f);
        subRT.sizeDelta = Vector2.zero;
        _subText = subGO.AddComponent<TextMeshProUGUI>();
        _subText.alignment = TextAlignmentOptions.Center;
        _subText.fontSize = 40f;
        _subText.color = new Color(0.88f, 0.88f, 0.88f, 1f);
        _subText.raycastTarget = false;

        // Skip button (top-right corner)
        var skipGO = new GameObject("BtnSkipTutorial");
        skipGO.transform.SetParent(canvas.transform, false);
        var skipRT = skipGO.AddComponent<RectTransform>();
        skipRT.anchorMin = new Vector2(0.6f, 0.93f);
        skipRT.anchorMax = new Vector2(1.0f, 1.0f);
        skipRT.sizeDelta = Vector2.zero;
        var skipImg = skipGO.AddComponent<Image>();
        skipImg.color = new Color(0f, 0f, 0f, 0.55f);
        var skipBtn = skipGO.AddComponent<Button>();
        skipBtn.targetGraphic = skipImg;
        skipBtn.onClick.AddListener(SkipTutorial);

        var skipLblGO = new GameObject("Label");
        skipLblGO.transform.SetParent(skipGO.transform, false);
        var skipLblRT = skipLblGO.AddComponent<RectTransform>();
        skipLblRT.anchorMin = Vector2.zero;
        skipLblRT.anchorMax = Vector2.one;
        skipLblRT.sizeDelta = Vector2.zero;
        var skipTmp = skipLblGO.AddComponent<TextMeshProUGUI>();
        skipTmp.text = "PULAR TUTORIAL";
        skipTmp.alignment = TextAlignmentOptions.Center;
        skipTmp.fontSize = 28f;
        skipTmp.color = Color.white;
        skipTmp.raycastTarget = false;
    }

    private void ShowCompletionButtons()
    {
        var canvas = FindAnyObjectByType<Canvas>();
        if (!canvas) return;

        var cardGO = new GameObject("TutorialCompleteCard");
        cardGO.transform.SetParent(canvas.transform, false);
        var cardRT = cardGO.AddComponent<RectTransform>();
        cardRT.anchorMin = new Vector2(0.08f, 0.28f);
        cardRT.anchorMax = new Vector2(0.92f, 0.72f);
        cardRT.sizeDelta = Vector2.zero;
        var cardImg = cardGO.AddComponent<Image>();
        cardImg.color = new Color(0.08f, 0.08f, 0.14f, 0.96f);

        MakeCompletionBtn(cardGO.transform, "JOGAR AGORA",
            new Color32(40, 175, 80, 255),
            new Vector2(0.06f, 0.08f), new Vector2(0.94f, 0.44f),
            () => { Time.timeScale = 1f; SceneManager.LoadScene("GameScene"); });

        MakeCompletionBtn(cardGO.transform, "MENU PRINCIPAL",
            new Color32(50, 60, 120, 255),
            new Vector2(0.06f, 0.54f), new Vector2(0.94f, 0.9f),
            () => { Time.timeScale = 1f; SceneManager.LoadScene("MenuScene"); });
    }

    private void MakeCompletionBtn(Transform parent, string label, Color32 bg,
        Vector2 ancMin, Vector2 ancMax, UnityEngine.Events.UnityAction onClick)
    {
        var go = new GameObject(label.Replace(" ", "") + "Btn");
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = ancMin;
        rt.anchorMax = ancMax;
        rt.sizeDelta = Vector2.zero;
        var img = go.AddComponent<Image>();
        img.color = bg;
        var btn = go.AddComponent<Button>();
        btn.targetGraphic = img;
        btn.onClick.AddListener(onClick);

        var lblGO = new GameObject("Label");
        lblGO.transform.SetParent(go.transform, false);
        var lblRT = lblGO.AddComponent<RectTransform>();
        lblRT.anchorMin = Vector2.zero;
        lblRT.anchorMax = Vector2.one;
        lblRT.sizeDelta = Vector2.zero;
        var tmp = lblGO.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontSize = 46f;
        tmp.fontStyle = FontStyles.Bold;
        tmp.color = Color.white;
        tmp.raycastTarget = false;
    }

    private void SkipTutorial()
    {
        PlayerPrefs.DeleteKey("TutorialMode");
        PlayerPrefs.Save();
        SceneManager.LoadScene("MenuScene");
    }
}
