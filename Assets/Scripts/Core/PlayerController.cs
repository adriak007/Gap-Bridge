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
        currentFallSpeed = fallSpeed;
        isFalling        = true;
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
        UnityEngine.SceneManagement.SceneManager.LoadScene("MenuScene");
    }
}
