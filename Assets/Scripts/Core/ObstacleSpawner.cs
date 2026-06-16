using UnityEngine;
using System.Collections.Generic;

public class ObstacleSpawner : MonoBehaviour
{
    public static ObstacleSpawner Instance { get; private set; }

    [Header("Obstaculo")]
    [SerializeField] private GameObject obstaclePrefab;

    [Header("Dificuldade")]
    [SerializeField] private int   scoreParaPrimeiro  = 5;  // score minimo para comecar a spawnar
    [SerializeField] private int   scoreParaDois      = 20; // score para chance de 2 obstaculos
    [SerializeField] private float chanceObstaculo    = 0.6f; // 60% de chance de spawnar

    private List<GameObject> ativos = new List<GameObject>();

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void SpawnNaPonte()
    {
        int score = ScoreManager.Instance != null ? ScoreManager.Instance.Score : 0;
        if (score < scoreParaPrimeiro) return;
        if (Random.value > chanceObstaculo) return;

        float pivotX    = BridgeController.Instance.GetBridgePivotX();
        float tipX      = BridgeController.Instance.GetBridgeTipX();
        float bridgeY   = GameManager.Instance.CurrentPlatform.TopEdge;
        float bridgeLen = tipX - pivotX;

        // Margens seguras: longe do player (início) e longe da borda da plataforma (fim)
        const float MARGEM_INICIO = 1.1f; // espaço para o player começar a andar
        const float MARGEM_FIM    = 0.9f; // espaço para pousar sem colidir

        float spanUtil = bridgeLen - MARGEM_INICIO - MARGEM_FIM;
        if (spanUtil < 0.4f) return; // ponte curta demais — não spawna

        int quantidade = score >= scoreParaDois && Random.value > 0.5f ? 2 : 1;

        for (int i = 0; i < quantidade; i++)
        {
            float zoneSize  = spanUtil / quantidade;
            float zoneStart = pivotX + MARGEM_INICIO + zoneSize * i;
            float zoneEnd   = zoneStart + zoneSize;

            if (zoneEnd - zoneStart < 0.2f) continue;

            float x   = Random.Range(zoneStart, zoneEnd);
            float y   = bridgeY + 0.175f;
            ativos.Add(Instantiate(obstaclePrefab, new Vector3(x, y, 0f), Quaternion.identity));
        }
    }

    public void LimparObstaculos()
    {
        foreach (var obj in ativos)
            if (obj != null) Destroy(obj);
        ativos.Clear();
    }
}
