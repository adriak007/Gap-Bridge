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

    // Chamado pelo BridgeVerifier quando a ponte alcanca a plataforma
    public void WalkToNextPlatform()
    {
        Platform next = GameManager.Instance.NextPlatform;

        // Posiciona o jogador em cima da proxima plataforma
        float standX = next.LeftEdge + 0.5f;
        float standY = next.TopEdge + 0.3f; // 0.3 = meia altura da capsula escalada

        targetPosition = new Vector3(standX, standY, 0f);
        isWalking = true;
    }

    // Chamado pelo BridgeVerifier quando a ponte falha
    public void FallDown()
    {
        isFalling = true;
    }

    private void OnArrived()
    {
        // Avanca o jogo: proxima plataforma vira atual, nova e gerada
        GameManager.Instance.AdvanceToNextPlatform();
        BridgeController.Instance.ResetBridge();
        BridgeVerifier.Instance.ResetVerifier();
    }

    private void OnGameOver()
    {
        Debug.Log("GAME OVER");
        // Passo 10: tela de game over aqui
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
