using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Plataformas")]
    [SerializeField] private GameObject platformPrefab;
    [SerializeField] private float minPlatformWidth = 1.2f;
    [SerializeField] private float maxPlatformWidth = 2.8f;
    [SerializeField] private float minGap = 1.8f;
    [SerializeField] private float maxGap = 3.5f;
    [SerializeField] private float platformY = -3.5f;

    public Platform CurrentPlatform { get; private set; }
    public Platform NextPlatform    { get; private set; }

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        // A plataforma inicial ja esta na cena — encontra ela
        CurrentPlatform = FindFirstObjectByType<Platform>();
        SpawnNextPlatform();
    }

    public void SpawnNextPlatform()
    {
        float w   = Random.Range(minPlatformWidth, maxPlatformWidth);
        float gap = Random.Range(minGap, maxGap);
        float x   = CurrentPlatform.RightEdge + gap + w / 2f;

        GameObject obj = Instantiate(platformPrefab, new Vector3(x, platformY, 0f), Quaternion.identity);
        obj.transform.localScale = new Vector3(w, 0.5f, 1f);
        obj.name = "Platform_Next";

        NextPlatform = obj.GetComponent<Platform>();
    }

    // Chamado apos o jogador atravessar com sucesso
    public void AdvanceToNextPlatform()
    {
        if (CurrentPlatform.name != "Platform_Start")
            Destroy(CurrentPlatform.gameObject);

        CurrentPlatform = NextPlatform;
        CurrentPlatform.name = "Platform_Current";
        SpawnNextPlatform();
    }
}
