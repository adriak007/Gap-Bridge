using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    [Header("Movimento")]
    [SerializeField] private float walkSpeed        = 4f;
    [SerializeField] private float fallSpeed        = 10f;
    [SerializeField] private float fallAcceleration = 18f;

    [Header("Pulo")]
    [SerializeField] private float jumpForce   = 11f;
    [SerializeField] private float jumpGravity = 22f;

    private bool    isWalking        = false;
    private bool    isFalling        = false;
    private float   currentFallSpeed = 0f;
    private float   jumpVelocity     = 0f;
    private float   jumpYOffset      = 0f;

    public bool IsOnGround => jumpYOffset <= 0.05f;
    private float   groundY          = 0f; // Y base do personagem na plataforma
    private float   targetX          = 0f; // X destino

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        CreatePlayerVisuals();
        ApplySkinColor(PlayerPrefs.GetString("EquippedSkin", "skin_0"));
    }

    private static readonly string[]  SkinKeys   = { "skin_0","skin_1","skin_2","skin_3","skin_4","skin_5" };
    private static readonly Color32[] SkinColors =
    {
        new Color32(255, 255, 255, 255),
        new Color32(255, 100,  30, 255),
        new Color32(100, 200, 255, 255),
        new Color32(255, 215,   0, 255),
        new Color32( 80,  80,  80, 255),
        new Color32(  0, 255, 120, 255),
    };

    private void CreatePlayerVisuals()
    {
        var bodySR = GetComponent<SpriteRenderer>();
        if (!bodySR) return;

        bodySR.sortingOrder = 5; // acima de todas as camadas da plataforma

        if (transform.Find("PlayerHead")) return;

        var head = new GameObject("PlayerHead");
        head.transform.SetParent(transform, false);
        head.transform.localPosition = new Vector3(0f, 0.7f, 0f);
        head.transform.localScale    = new Vector3(0.72f, 0.58f, 1f);

        var headSR = head.AddComponent<SpriteRenderer>();
        headSR.sprite       = bodySR.sprite;
        headSR.color        = bodySR.color;
        headSR.sortingOrder = 6;
    }

    private void ApplySkinColor(string key)
    {
        var sr = GetComponent<SpriteRenderer>();
        if (!sr) return;
        for (int i = 0; i < SkinKeys.Length; i++)
        {
            if (SkinKeys[i] != key) continue;
            sr.color = SkinColors[i];
            var headT = transform.Find("PlayerHead");
            if (headT)
            {
                var headSR = headT.GetComponent<SpriteRenderer>();
                if (headSR) headSR.color = SkinColors[i];
            }
            return;
        }
    }

    private void Update()
    {
        if (isWalking)
        {
            HandleJumpInput();
            UpdateJumpPhysics();
            MoveHorizontal();
        }

        if (isFalling) HandleFalling();
    }

    // ── Pulo — so afeta Y ──────────────────────────────

    private void HandleJumpInput()
    {
        bool tapped = (Mouse.current      != null && Mouse.current.leftButton.wasPressedThisFrame)
                   || (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame);

        if (tapped && jumpYOffset <= 0f)
            jumpVelocity = jumpForce;
    }

    private void UpdateJumpPhysics()
    {
        jumpVelocity -= jumpGravity * Time.deltaTime;
        jumpYOffset  += jumpVelocity * Time.deltaTime;

        if (jumpYOffset < 0f)
        {
            jumpYOffset  = 0f;
            jumpVelocity = 0f;
        }

        // Y = chao + offset do pulo (totalmente independente do X)
        transform.position = new Vector3(transform.position.x, groundY + jumpYOffset, 0f);
    }

    // ── Horizontal — so afeta X ────────────────────────

    private void MoveHorizontal()
    {
        float newX = Mathf.MoveTowards(transform.position.x, targetX, walkSpeed * Time.deltaTime);
        transform.position = new Vector3(newX, transform.position.y, 0f);

        if (Mathf.Abs(newX - targetX) < 0.01f && jumpYOffset <= 0f)
        {
            transform.position = new Vector3(targetX, groundY, 0f);
            isWalking          = false;
            OnArrived();
        }
    }

    // ── Queda ──────────────────────────────────────────

    private void HandleFalling()
    {
        currentFallSpeed   += fallAcceleration * Time.deltaTime;
        transform.position += Vector3.down * currentFallSpeed * Time.deltaTime;

        if (transform.position.y < -12f)
        {
            isFalling = false;
            OnGameOver();
        }
    }

    // ── API ────────────────────────────────────────────

    public void WalkToNextPlatform()
    {
        Platform next = GameManager.Instance.NextPlatform;
        groundY       = next.TopEdge + 0.3f;
        targetX       = next.LeftEdge + 0.5f;
        jumpVelocity  = 0f;
        jumpYOffset   = 0f;
        isWalking     = true;
    }

    public void FallDown()
    {
        isWalking        = false;
        jumpVelocity     = 0f;
        jumpYOffset      = 0f;
        currentFallSpeed = fallSpeed;
        isFalling        = true;
    }

    // Chamado pelo obstáculo — igual ao fail normal com todos os efeitos
    public void HitObstacle()
    {
        ScoreManager.Instance.OnFail();
        if (AudioManager.Instance)  AudioManager.Instance.PlayFail();
        if (ScreenEffects.Instance) ScreenEffects.Instance.ShakeCamera();
        if (ScreenEffects.Instance) ScreenEffects.Instance.FlashRed();
        FallDown();
    }

    private void OnArrived()
    {
        if (ObstacleSpawner.Instance) ObstacleSpawner.Instance.LimparObstaculos();
        GameManager.Instance.AdvanceToNextPlatform();
        BridgeController.Instance.ResetBridge();
        BridgeVerifier.Instance.ResetVerifier();
    }

    private void OnGameOver()
    {
        ScoreManager.Instance.SaveHighScore();
        int best = PlayerPrefs.GetInt("BestScore", 0);
        UIManager.Instance.ShowGameOver(ScoreManager.Instance.Score, best);
    }
}
