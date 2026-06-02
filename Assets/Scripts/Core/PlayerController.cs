using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    [Header("Movimento")]
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float fallSpeed = 6f;

    private bool isWalking = false;
    private bool isFalling = false;
    private Vector3 targetPosition;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    // ── Movimento ──────────────────────────────────────

    private void Update()
    {
        if (isWalking) HandleWalking();
        if (isFalling) HandleFalling();
    }

    private void HandleWalking()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, walkSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            transform.position = targetPosition;
            isWalking = false;
            OnArrived();
        }
    }

    private void HandleFalling()
    {
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;
        if (transform.position.y < -12f)
        {
            isFalling = false;
            OnGameOver();
        }
    }

    public void WalkToNextPlatform()
    {
        Platform next  = GameManager.Instance.NextPlatform;
        float standX   = next.LeftEdge + 0.5f;
        float standY   = next.TopEdge + 0.3f;
        targetPosition = new Vector3(standX, standY, 0f);
        isWalking      = true;
    }

    public void FallDown() => isFalling = true;

    private void OnArrived()
    {
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
