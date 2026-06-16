using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Events;
using TMPro;

/// <summary>
/// Reconstrói toda a UI da MenuScene.
/// Menu: Gap Bridge > Build Menu UI
/// </summary>
public static class MenuUIBuilder
{
    // ── Paleta ─────────────────────────────────────────────────────────────────
    static readonly Color32 BG      = new Color32( 13,  13,  26, 255);
    static readonly Color32 PANELBG = new Color32( 20,  25,  55, 245);
    static readonly Color32 CARDBG  = new Color32( 28,  32,  58, 255);
    static readonly Color32 ACCENT  = new Color32(233,  69,  96, 255);
    static readonly Color32 GOLD    = new Color32(255, 215,   0, 255);
    static readonly Color32 WHITE   = new Color32(234, 234, 234, 255);
    static readonly Color32 GRAY    = new Color32(140, 140, 165, 255);
    static readonly Color32 OVERLAY = new Color32(  0,   0,   0, 170);
    static readonly Color32 BTNBG2  = new Color32( 30,  40,  85, 255);
    static readonly Color32 DARKBG  = new Color32( 15,  20,  45, 255);

    [MenuItem("Gap Bridge/Build Menu UI")]
    public static void Build()
    {
        // ── Abrir MenuScene ─────────────────────────────────────────────────────
        var guids = AssetDatabase.FindAssets("MenuScene t:Scene");
        if (guids.Length > 0)
        {
            string scenePath = AssetDatabase.GUIDToAssetPath(guids[0]);
            var active = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if (!active.path.EndsWith("MenuScene.unity"))
                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        }

        // ── Limpar objetos antigos ─────────────────────────────────────────────
        string[] toDestroy = { "Canvas", "[Manager] MenuManager", "MenuManager" };
        foreach (var n in toDestroy)
        {
            var go = GameObject.Find(n);
            if (go) Object.DestroyImmediate(go);
        }

        // ── EventSystem ────────────────────────────────────────────────────────
        if (Object.FindAnyObjectByType<EventSystem>() == null)
        {
            var es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
        }

        // ── Canvas ─────────────────────────────────────────────────────────────
        var canvasGO = new GameObject("Canvas");
        var canvas   = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight  = 0.5f;
        canvasGO.AddComponent<GraphicRaycaster>();

        // ── Background com a imagem do jogo ────────────────────────────────────
        var bgTex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/img/Background.png");
        var bgGO  = new GameObject("Background", typeof(RectTransform), typeof(RawImage));
        bgGO.transform.SetParent(canvasGO.transform, false);
        var rawImg = bgGO.GetComponent<RawImage>();
        if (bgTex) rawImg.texture = bgTex;
        rawImg.color = Color.white;
        Stretch(bgGO);

        // Overlay escuro para legibilidade do texto
        var overlayGO = Img(canvasGO.transform, "DarkOverlay", new Color32(0, 0, 0, 145));
        Stretch(overlayGO);

        // ── Construir painéis ──────────────────────────────────────────────────
        TMP_Text bestScoreText, coinsText, lojaCoinsText;
        ShopItemCard[]  shopCards;
        RankingEntry[]  rankingEntries;
        Toggle          musicToggle, sfxToggle;

        var mainPanel    = BuildMainPanel   (canvasGO.transform, out bestScoreText, out coinsText);
        var painelLoja   = BuildLojaPanel   (canvasGO.transform, out shopCards,     out lojaCoinsText);
        var painelRanking= BuildRankingPanel(canvasGO.transform, out rankingEntries);
        var painelOpcoes = BuildOpcoesPanel (canvasGO.transform, out musicToggle,   out sfxToggle);

        painelLoja.SetActive(false);
        painelRanking.SetActive(false);
        painelOpcoes.SetActive(false);

        // ── MenuManager ────────────────────────────────────────────────────────
        var mmGO = new GameObject("[Manager] MenuManager");
        var mm   = mmGO.AddComponent<MenuManager>();

        var so = new SerializedObject(mm);
        so.FindProperty("bestScoreText") .objectReferenceValue = bestScoreText;
        so.FindProperty("coinsText")     .objectReferenceValue = coinsText;
        so.FindProperty("painelLoja")    .objectReferenceValue = painelLoja;
        so.FindProperty("painelRanking") .objectReferenceValue = painelRanking;
        so.FindProperty("painelOpcoes")  .objectReferenceValue = painelOpcoes;
        so.FindProperty("lojaCoinsText") .objectReferenceValue = lojaCoinsText;
        so.FindProperty("musicToggle")   .objectReferenceValue = musicToggle;
        so.FindProperty("sfxToggle")     .objectReferenceValue = sfxToggle;

        // Preview do personagem
        var charPreviewGO = mainPanel.transform.Find("CharacterPreview");
        so.FindProperty("playerPreview").objectReferenceValue =
            charPreviewGO ? charPreviewGO.GetComponent<PlayerPreviewUI>() : null;

        var shopProp = so.FindProperty("shopCards");
        shopProp.arraySize = shopCards.Length;
        for (int i = 0; i < shopCards.Length; i++)
            shopProp.GetArrayElementAtIndex(i).objectReferenceValue = shopCards[i];

        var rankProp = so.FindProperty("rankingEntries");
        rankProp.arraySize = rankingEntries.Length;
        for (int i = 0; i < rankingEntries.Length; i++)
            rankProp.GetArrayElementAtIndex(i).objectReferenceValue = rankingEntries[i];

        so.ApplyModifiedProperties();

        // ── Ligar botões ───────────────────────────────────────────────────────
        Wire(mainPanel.transform.Find("BtnJogar"),              mm.OnJogarClick);
        Wire(mainPanel.transform.Find("BottomBar/BtnLoja"),     mm.OnLojaClick);
        Wire(mainPanel.transform.Find("BottomBar/BtnRanking"),  mm.OnRankingClick);
        Wire(mainPanel.transform.Find("Header/BtnOpcoes"),      mm.OnOpcoesClick);
        Wire(painelLoja.transform.Find   ("PainelContent/Header/BtnFechar"), mm.FecharTodosPaineis);
        Wire(painelRanking.transform.Find("PainelContent/Header/BtnFechar"), mm.FecharTodosPaineis);
        Wire(painelOpcoes.transform.Find ("PainelContent/Header/BtnFechar"), mm.FecharTodosPaineis);

        // ── Salvar ─────────────────────────────────────────────────────────────
        EditorSceneManager.MarkSceneDirty(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        Debug.Log("[GapBridge] Menu UI construído com sucesso!");
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // MAIN PANEL
    // ═══════════════════════════════════════════════════════════════════════════

    static GameObject BuildMainPanel(Transform parent, out TMP_Text bestScore, out TMP_Text coins)
    {
        var panel = Empty(parent, "MainPanel");
        Stretch(panel);

        // Header ───────────────────────────────────────────────────────────────
        var header = Img(panel.transform, "Header", new Color32(0, 0, 0, 80));
        AnchorTop(header, 160);

        // Moedas (esquerda do header)
        var coinGroup = Empty(header.transform, "CoinsGroup");
        AnchorLeft(RT(coinGroup), new Vector2(25, 0), new Vector2(220, 80));
        var hlgMain = coinGroup.AddComponent<HorizontalLayoutGroup>();
        hlgMain.spacing = 10;
        hlgMain.childAlignment = TextAnchor.MiddleLeft;
        hlgMain.childForceExpandWidth = false;
        hlgMain.childForceExpandHeight = false;
        hlgMain.childControlWidth = false;
        hlgMain.childControlHeight = false;

        var coinIcon = Img(coinGroup.transform, "CoinIcon", GOLD);
        RT(coinIcon).sizeDelta = new Vector2(48, 48);

        var coinsTMP = TMP(coinGroup.transform, "CoinsText", "0", 38, GOLD, TextAlignmentOptions.Left);
        coinsTMP.fontStyle = FontStyles.Bold;
        RT(coinsTMP.gameObject).sizeDelta = new Vector2(150, 55);
        coins = coinsTMP;

        // Botão opções (direita do header)
        var btnOpc = Btn(header.transform, "BtnOpcoes", "OPCOES", BTNBG2, WHITE, 24);
        AnchorRight(btnOpc, new Vector2(-20, 0), new Vector2(160, 65));

        // Título ───────────────────────────────────────────────────────────────
        var gapTMP = TMP(panel.transform, "TitleGAP", "GAP", 130, WHITE, TextAlignmentOptions.Center);
        gapTMP.fontStyle = FontStyles.Bold;
        AnchorCenter(gapTMP.gameObject, new Vector2(0, 480), new Vector2(700, 175));

        var bridgeTMP = TMP(panel.transform, "TitleBRIDGE", "BRIDGE", 95, ACCENT, TextAlignmentOptions.Center);
        bridgeTMP.fontStyle = FontStyles.Bold;
        AnchorCenter(bridgeTMP.gameObject, new Vector2(0, 305), new Vector2(700, 135));

        // Record ───────────────────────────────────────────────────────────────
        var bsTMP = TMP(panel.transform, "BestScoreText", "", 36, GRAY, TextAlignmentOptions.Center);
        AnchorCenter(bsTMP.gameObject, new Vector2(0, 155), new Vector2(600, 60));
        bestScore = bsTMP;

        // Personagem animado (bobbing) ─────────────────────────────────────────
        var charContainer = Empty(panel.transform, "CharacterPreview");
        AnchorCenter(charContainer, new Vector2(0, -20), new Vector2(90, 165));
        charContainer.AddComponent<IdleBobbingUI>();
        charContainer.AddComponent<PlayerPreviewUI>();

        var charHead = Img(charContainer.transform, "Head", WHITE);
        {
            var rt = RT(charHead);
            rt.anchorMin = new Vector2(0.5f, 1); rt.anchorMax = new Vector2(0.5f, 1);
            rt.pivot     = new Vector2(0.5f, 1);
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta = new Vector2(70, 70);
            charHead.GetComponent<Image>().sprite = RoundedSprite;
        }

        var charBody = Img(charContainer.transform, "Body", WHITE);
        {
            var rt = RT(charBody);
            rt.anchorMin = new Vector2(0.5f, 1); rt.anchorMax = new Vector2(0.5f, 1);
            rt.pivot     = new Vector2(0.5f, 1);
            rt.anchoredPosition = new Vector2(0, -75);
            rt.sizeDelta = new Vector2(52, 90);
        }

        // Botão JOGAR ──────────────────────────────────────────────────────────
        var btnJogar = Btn(panel.transform, "BtnJogar", "JOGAR", ACCENT, WHITE, 54);
        AnchorCenter(btnJogar, new Vector2(0, -215), new Vector2(520, 120));
        var btnImg = btnJogar.GetComponent<Image>();
        if (btnImg) btnImg.color = ACCENT;

        // Barra inferior ───────────────────────────────────────────────────────
        var bottomBar = Img(panel.transform, "BottomBar", new Color32(0, 0, 0, 80));
        AnchorBottom(bottomBar, 160);

        var btnLoja = Btn(bottomBar.transform, "BtnLoja", "LOJA", BTNBG2, WHITE, 34);
        {
            var rt = RT(btnLoja);
            rt.anchorMin = new Vector2(0, 0.5f); rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.offsetMin = new Vector2(20, -52); rt.offsetMax = new Vector2(-10, 52);
        }

        var btnRank = Btn(bottomBar.transform, "BtnRanking", "RANKING", BTNBG2, WHITE, 34);
        {
            var rt = RT(btnRank);
            rt.anchorMin = new Vector2(0.5f, 0.5f); rt.anchorMax = new Vector2(1, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.offsetMin = new Vector2(10, -52); rt.offsetMax = new Vector2(-20, 52);
        }

        return panel;
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // LOJA PANEL
    // ═══════════════════════════════════════════════════════════════════════════

    static GameObject BuildLojaPanel(Transform parent, out ShopItemCard[] cards, out TMP_Text lojaCoins)
    {
        var panel = Empty(parent, "PainelLoja");
        Stretch(panel);

        var overlay = Img(panel.transform, "Overlay", OVERLAY);
        Stretch(overlay);

        var content = Img(panel.transform, "PainelContent", PANELBG);
        AnchorCenter(content, new Vector2(0, 20), new Vector2(1000, 1640));

        // Header da loja ───────────────────────────────────────────────────────
        var header = Img(content.transform, "Header", DARKBG);
        AnchorTop(header, 150, content.transform);

        var title = TMP(header.transform, "Title", "LOJA", 62, WHITE, TextAlignmentOptions.Center);
        title.fontStyle = FontStyles.Bold;
        Stretch(title.gameObject);

        var lojaCoinsTMP = TMP(header.transform, "CoinsText", "0", 34, GOLD, TextAlignmentOptions.Left);
        lojaCoinsTMP.fontStyle = FontStyles.Bold;
        AnchorLeft(lojaCoinsTMP.gameObject.GetComponent<RectTransform>(), new Vector2(25, 0), new Vector2(180, 55));
        lojaCoins = lojaCoinsTMP;

        var closeBtn = Btn(header.transform, "BtnFechar", "X", ACCENT, WHITE, 42);
        AnchorRight(closeBtn, new Vector2(-18, 0), new Vector2(80, 80));

        // Grid de skins ────────────────────────────────────────────────────────
        var gridGO = Empty(content.transform, "ShopGrid");
        {
            var rt = RT(gridGO);
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(0, 0); rt.offsetMax = new Vector2(0, -150);
        }

        var grid = gridGO.AddComponent<GridLayoutGroup>();
        grid.cellSize        = new Vector2(455, 300);
        grid.spacing         = new Vector2(22, 22);
        grid.padding         = new RectOffset(30, 30, 25, 25);
        grid.constraint      = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = 2;
        grid.childAlignment  = TextAnchor.UpperCenter;

        cards = new ShopItemCard[6];
        for (int i = 0; i < 6; i++)
            cards[i] = BuildShopCard(gridGO.transform, i);

        return panel;
    }

    static ShopItemCard BuildShopCard(Transform parent, int index)
    {
        var cardGO = Img(parent, "ShopCard_" + index, CARDBG);

        // Mini-personagem: cabeça + corpo (coloridos pela skin)
        var miniHead = Img(cardGO.transform, "Head", WHITE);
        {
            var rt = RT(miniHead);
            rt.anchorMin = new Vector2(0.5f, 1); rt.anchorMax = new Vector2(0.5f, 1);
            rt.pivot     = new Vector2(0.5f, 1);
            rt.anchoredPosition = new Vector2(0, -14);
            rt.sizeDelta = new Vector2(48, 48);
            miniHead.GetComponent<Image>().sprite = RoundedSprite;
        }

        var miniBody = Img(cardGO.transform, "Body", WHITE);
        {
            var rt = RT(miniBody);
            rt.anchorMin = new Vector2(0.5f, 1); rt.anchorMax = new Vector2(0.5f, 1);
            rt.pivot     = new Vector2(0.5f, 1);
            rt.anchoredPosition = new Vector2(0, -67);
            rt.sizeDelta = new Vector2(36, 55);
        }

        // Nome da skin
        var nameT = TMP(cardGO.transform, "NameText", "Skin", 30, WHITE, TextAlignmentOptions.Center);
        nameT.fontStyle = FontStyles.Bold;
        {
            var rt = RT(nameT.gameObject);
            rt.anchorMin = new Vector2(0, 1); rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(0.5f, 1);
            rt.anchoredPosition = new Vector2(0, -135);
            rt.sizeDelta = new Vector2(0, 42);
        }

        // Status (preço ou "EQUIPADO")
        var statusT = TMP(cardGO.transform, "StatusText", "0 moedas", 24, GOLD, TextAlignmentOptions.Center);
        {
            var rt = RT(statusT.gameObject);
            rt.anchorMin = new Vector2(0, 1); rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(0.5f, 1);
            rt.anchoredPosition = new Vector2(0, -183);
            rt.sizeDelta = new Vector2(0, 38);
        }

        // Botão de ação
        var btn = Btn(cardGO.transform, "ActionButton", "COMPRAR", ACCENT, WHITE, 26);
        {
            var rt = RT(btn);
            rt.anchorMin = new Vector2(0.08f, 0); rt.anchorMax = new Vector2(0.92f, 0);
            rt.pivot = new Vector2(0.5f, 0);
            rt.offsetMin = new Vector2(0, 14);
            rt.offsetMax = new Vector2(0, 70);
        }

        // ShopItemCard component
        var card = cardGO.AddComponent<ShopItemCard>();
        var so   = new SerializedObject(card);

        var cpProp = so.FindProperty("colorParts");
        cpProp.arraySize = 2;
        cpProp.GetArrayElementAtIndex(0).objectReferenceValue = miniHead.GetComponent<Image>();
        cpProp.GetArrayElementAtIndex(1).objectReferenceValue = miniBody.GetComponent<Image>();

        so.FindProperty("nameText")      .objectReferenceValue = nameT;
        so.FindProperty("statusText")    .objectReferenceValue = statusT;
        so.FindProperty("actionButton")  .objectReferenceValue = btn.GetComponent<Button>();
        var lblChild = btn.transform.Find("Label");
        so.FindProperty("buttonLabel")   .objectReferenceValue = lblChild ? lblChild.GetComponent<TextMeshProUGUI>() : null;
        so.FindProperty("cardBackground").objectReferenceValue = cardGO.GetComponent<Image>();
        so.ApplyModifiedProperties();

        UnityEventTools.AddPersistentListener(btn.GetComponent<Button>().onClick, card.OnActionClick);
        return card;
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // RANKING PANEL
    // ═══════════════════════════════════════════════════════════════════════════

    static GameObject BuildRankingPanel(Transform parent, out RankingEntry[] entries)
    {
        var panel = Empty(parent, "PainelRanking");
        Stretch(panel);

        var overlay = Img(panel.transform, "Overlay", OVERLAY);
        Stretch(overlay);

        var content = Img(panel.transform, "PainelContent", PANELBG);
        AnchorCenter(content, new Vector2(0, 60), new Vector2(920, 1140));

        // Header ───────────────────────────────────────────────────────────────
        var header = Img(content.transform, "Header", DARKBG);
        AnchorTop(header, 150, content.transform);

        var title = TMP(header.transform, "Title", "RANKING", 62, WHITE, TextAlignmentOptions.Center);
        title.fontStyle = FontStyles.Bold;
        Stretch(title.gameObject);

        var closeBtn = Btn(header.transform, "BtnFechar", "X", ACCENT, WHITE, 42);
        AnchorRight(closeBtn, new Vector2(-18, 0), new Vector2(80, 80));

        // Lista de entradas ────────────────────────────────────────────────────
        var listGO = Empty(content.transform, "RankingList");
        {
            var rt = RT(listGO);
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(0, 0); rt.offsetMax = new Vector2(0, -150);
        }

        var vlg = listGO.AddComponent<VerticalLayoutGroup>();
        vlg.spacing              = 18;
        vlg.padding              = new RectOffset(30, 30, 30, 30);
        vlg.childForceExpandWidth  = true;
        vlg.childForceExpandHeight = false;
        vlg.childControlWidth      = true;
        vlg.childControlHeight     = false;

        entries = new RankingEntry[5];
        for (int i = 0; i < 5; i++)
            entries[i] = BuildRankingEntry(listGO.transform, i);

        return panel;
    }

    static RankingEntry BuildRankingEntry(Transform parent, int index)
    {
        var entryGO = Img(parent, "RankEntry_" + index, new Color32(255, 255, 255, 15));
        var le = entryGO.AddComponent<LayoutElement>();
        le.preferredHeight = 148;
        le.flexibleWidth   = 1;

        // Posição (ex: #1, #2)
        var rankT = TMP(entryGO.transform, "RankText", "#" + (index + 1), 52, GOLD, TextAlignmentOptions.Center);
        rankT.fontStyle = FontStyles.Bold;
        {
            var rt = RT(rankT.gameObject);
            rt.anchorMin = new Vector2(0, 0); rt.anchorMax = new Vector2(0, 1);
            rt.offsetMin = new Vector2(20, 0); rt.offsetMax = new Vector2(150, 0);
        }

        // Pontuação
        var scoreT = TMP(entryGO.transform, "ScoreText", "---", 44, WHITE, TextAlignmentOptions.Right);
        {
            var rt = RT(scoreT.gameObject);
            rt.anchorMin = new Vector2(0, 0); rt.anchorMax = new Vector2(1, 1);
            rt.offsetMin = new Vector2(150, 0); rt.offsetMax = new Vector2(-25, 0);
        }

        var entry = entryGO.AddComponent<RankingEntry>();
        var so    = new SerializedObject(entry);
        so.FindProperty("rankText")  .objectReferenceValue = rankT;
        so.FindProperty("scoreText") .objectReferenceValue = scoreT;
        so.FindProperty("background").objectReferenceValue = entryGO.GetComponent<Image>();
        so.ApplyModifiedProperties();

        return entry;
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // OPCOES PANEL
    // ═══════════════════════════════════════════════════════════════════════════

    static GameObject BuildOpcoesPanel(Transform parent, out Toggle musicTgl, out Toggle sfxTgl)
    {
        var panel = Empty(parent, "PainelOpcoes");
        Stretch(panel);

        var overlay = Img(panel.transform, "Overlay", OVERLAY);
        Stretch(overlay);

        var content = Img(panel.transform, "PainelContent", PANELBG);
        AnchorCenter(content, Vector2.zero, new Vector2(840, 700));

        // Header ───────────────────────────────────────────────────────────────
        var header = Img(content.transform, "Header", DARKBG);
        AnchorTop(header, 150, content.transform);

        var title = TMP(header.transform, "Title", "OPCOES", 62, WHITE, TextAlignmentOptions.Center);
        title.fontStyle = FontStyles.Bold;
        Stretch(title.gameObject);

        var closeBtn = Btn(header.transform, "BtnFechar", "X", ACCENT, WHITE, 42);
        AnchorRight(closeBtn, new Vector2(-18, 0), new Vector2(80, 80));

        // Rows container ───────────────────────────────────────────────────────
        var rowsGO = Empty(content.transform, "RowsContainer");
        {
            var rt = RT(rowsGO);
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(30, 20); rt.offsetMax = new Vector2(-30, -150);
        }

        var vlg = rowsGO.AddComponent<VerticalLayoutGroup>();
        vlg.spacing              = 24;
        vlg.padding              = new RectOffset(0, 0, 24, 24);
        vlg.childForceExpandWidth  = true;
        vlg.childForceExpandHeight = false;
        vlg.childControlWidth      = true;
        vlg.childControlHeight     = false;

        musicTgl = BuildToggleRow(rowsGO.transform, "Musica");
        sfxTgl   = BuildToggleRow(rowsGO.transform, "SFX");

        return panel;
    }

    static Toggle BuildToggleRow(Transform parent, string label)
    {
        var row = Img(parent, "Row_" + label, new Color32(0, 0, 0, 50));
        var le  = row.AddComponent<LayoutElement>();
        le.preferredHeight = 130;
        le.flexibleWidth   = 1;

        var labelT = TMP(row.transform, "Label", label, 44, WHITE, TextAlignmentOptions.Left);
        {
            var rt = RT(labelT.gameObject);
            rt.anchorMin = new Vector2(0, 0); rt.anchorMax = new Vector2(0.65f, 1);
            rt.offsetMin = new Vector2(28, 0); rt.offsetMax = Vector2.zero;
        }

        var tglGO = new GameObject("Toggle_" + label, typeof(RectTransform));
        tglGO.transform.SetParent(row.transform, false);
        var tRT = RT(tglGO);
        tRT.anchorMin = new Vector2(1, 0.5f); tRT.anchorMax = new Vector2(1, 0.5f);
        tRT.pivot = new Vector2(1, 0.5f);
        tRT.anchoredPosition = new Vector2(-28, 0);
        tRT.sizeDelta = new Vector2(130, 65);

        var bgImg = Img(tglGO.transform, "Background", BTNBG2);
        Stretch(bgImg);

        var check = Img(bgImg.transform, "Checkmark", ACCENT);
        Stretch(check, 8, 8, 8, 8);

        var toggle = tglGO.AddComponent<Toggle>();
        toggle.targetGraphic = bgImg.GetComponent<Image>();
        toggle.graphic       = check.GetComponent<Image>();
        toggle.isOn          = true;

        return toggle;
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // HELPERS
    // ═══════════════════════════════════════════════════════════════════════════

    static Sprite s_RoundedSprite;
    static Sprite RoundedSprite
    {
        get
        {
            if (!s_RoundedSprite)
                s_RoundedSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            return s_RoundedSprite;
        }
    }

    static RectTransform RT(GameObject go) => go.GetComponent<RectTransform>();

    static void Stretch(GameObject go, float l = 0, float r = 0, float b = 0, float t = 0)
    {
        var rt = RT(go);
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
        rt.offsetMin = new Vector2(l, b); rt.offsetMax = new Vector2(-r, -t);
    }

    static void AnchorCenter(GameObject go, Vector2 pos, Vector2 size)
    {
        var rt = RT(go);
        rt.anchorMin = new Vector2(0.5f, 0.5f); rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot     = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos; rt.sizeDelta = size;
    }

    static void AnchorTop(GameObject go, float height, Transform parent = null)
    {
        var rt = RT(go);
        rt.anchorMin = new Vector2(0, 1); rt.anchorMax = new Vector2(1, 1);
        rt.pivot     = new Vector2(0.5f, 1f);
        rt.offsetMin = new Vector2(0, -height); rt.offsetMax = Vector2.zero;
    }

    static void AnchorBottom(GameObject go, float height)
    {
        var rt = RT(go);
        rt.anchorMin = new Vector2(0, 0); rt.anchorMax = new Vector2(1, 0);
        rt.pivot     = new Vector2(0.5f, 0f);
        rt.offsetMin = Vector2.zero; rt.offsetMax = new Vector2(0, height);
    }

    static void AnchorLeft(RectTransform rt, Vector2 pos, Vector2 size)
    {
        rt.anchorMin = new Vector2(0, 0.5f); rt.anchorMax = new Vector2(0, 0.5f);
        rt.pivot     = new Vector2(0, 0.5f);
        rt.anchoredPosition = pos; rt.sizeDelta = size;
    }

    static void AnchorRight(GameObject go, Vector2 pos, Vector2 size)
    {
        var rt = RT(go);
        rt.anchorMin = new Vector2(1, 0.5f); rt.anchorMax = new Vector2(1, 0.5f);
        rt.pivot     = new Vector2(1, 0.5f);
        rt.anchoredPosition = pos; rt.sizeDelta = size;
    }

    static GameObject Empty(Transform parent, string name)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return go;
    }

    static GameObject Img(Transform parent, string name, Color32 color)
    {
        var go  = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        go.GetComponent<Image>().color = color;
        return go;
    }

    static TMP_Text TMP(Transform parent, string name, string text, int size, Color32 color, TextAlignmentOptions align)
    {
        var go  = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);
        var tmp = go.GetComponent<TextMeshProUGUI>();
        tmp.text                = text;
        tmp.fontSize            = size;
        tmp.color               = color;
        tmp.alignment           = align;
        tmp.enableWordWrapping  = false;
        tmp.overflowMode        = TextOverflowModes.Overflow;
        return tmp;
    }

    static GameObject Btn(Transform parent, string name, string label, Color32 bg, Color32 fg, int fontSize = 32)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        go.GetComponent<Image>().color = bg;

        var lbl = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
        lbl.transform.SetParent(go.transform, false);
        Stretch(lbl);

        var tmp = lbl.GetComponent<TextMeshProUGUI>();
        tmp.text       = label;
        tmp.fontSize   = fontSize;
        tmp.color      = fg;
        tmp.alignment  = TextAlignmentOptions.Center;
        tmp.fontStyle  = FontStyles.Bold;

        return go;
    }

    static void Wire(Transform t, UnityEngine.Events.UnityAction action)
    {
        if (t == null) { Debug.LogWarning("[MenuUIBuilder] Transform not found for wiring: " + action.Method.Name); return; }
        var btn = t.GetComponent<Button>();
        if (btn == null) { Debug.LogWarning("[MenuUIBuilder] No Button on: " + t.name); return; }
        UnityEventTools.AddPersistentListener(btn.onClick, action);
    }
}
