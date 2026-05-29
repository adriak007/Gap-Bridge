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
    [SerializeField] private float platformTopY  = -3.25f; // altura do topo de todos os pilares
    [SerializeField] private float pillarHeight  = 8f;     // altura do pilar (some abaixo da tela)

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
        CurrentPlatform = FindAnyObjectByType<Platform>();
        SpawnNextPlatform();
    }

    public void SpawnNextPlatform()
    {
        float w   = Random.Range(minPlatformWidth, maxPlatformWidth);
        float gap = Random.Range(minGap, maxGap);
        float x   = CurrentPlatform.RightEdge + gap + w / 2f;

        float centerY = platformTopY - pillarHeight / 2f;
        GameObject obj = Instantiate(platformPrefab, new Vector3(x, centerY, 0f), Quaternion.identity);
        obj.transform.localScale = new Vector3(w, pillarHeight, 1f);
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
