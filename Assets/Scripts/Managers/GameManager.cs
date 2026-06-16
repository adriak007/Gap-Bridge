using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Plataformas")]
    [SerializeField] private GameObject platformPrefab;
    [SerializeField] private float platformTopY = -3.25f;
    [SerializeField] private float pillarHeight  = 8f;

    [Header("Dificuldade Inicial")]
    [SerializeField] private float minPlatformWidth = 1.2f;
    [SerializeField] private float maxPlatformWidth = 2.8f;
    [SerializeField] private float minGap = 1.8f;
    [SerializeField] private float maxGap = 3.5f;

    [Header("Dificuldade Maxima (atingida aos 30 pontos)")]
    [SerializeField] private float minPlatformWidthHard = 0.7f;
    [SerializeField] private float maxPlatformWidthHard = 1.6f;
    [SerializeField] private float minGapHard = 2.8f;
    [SerializeField] private float maxGapHard = 5.0f;
    [SerializeField] private int   scoreParaDificuldadeMax = 30;

    public Platform CurrentPlatform { get; private set; }
    public Platform NextPlatform    { get; private set; }


    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        CurrentPlatform = FindAnyObjectByType<Platform>();
        SpawnNextPlatform();
    }

    public void SpawnNextPlatform()
    {
        float t = GetDifficultyT();

        float w   = Random.Range(
            Mathf.Lerp(minPlatformWidth, minPlatformWidthHard, t),
            Mathf.Lerp(maxPlatformWidth, maxPlatformWidthHard, t)
        );
        float gap = Random.Range(
            Mathf.Lerp(minGap, minGapHard, t),
            Mathf.Lerp(maxGap, maxGapHard, t)
        );

        float x       = CurrentPlatform.RightEdge + gap + w / 2f;
        float centerY = platformTopY - pillarHeight / 2f;

        GameObject obj = Instantiate(platformPrefab, new Vector3(x, centerY, 0f), Quaternion.identity);
        obj.transform.localScale = new Vector3(w, pillarHeight, 1f);
        obj.name = "Platform_Next";

        NextPlatform = obj.GetComponent<Platform>();

        // Mostra o obstacle no gap assim que a plataforma aparece,
        // para o player já saber onde vai precisar pular.
        if (ObstacleSpawner.Instance)
            ObstacleSpawner.Instance.SpawnNoGap(NextPlatform);
    }

    public void AdvanceToNextPlatform()
    {
        if (CurrentPlatform.name != "Platform_Start")
            Destroy(CurrentPlatform.gameObject);

        CurrentPlatform      = NextPlatform;
        CurrentPlatform.name = "Platform_Current";
        SpawnNextPlatform();
    }

    // Retorna 0.0 (facil) ate 1.0 (dificuldade maxima) baseado no score
    private float GetDifficultyT()
    {
        int score = ScoreManager.Instance != null ? ScoreManager.Instance.Score : 0;
        return Mathf.Clamp01((float)score / scoreParaDificuldadeMax);
    }
}
