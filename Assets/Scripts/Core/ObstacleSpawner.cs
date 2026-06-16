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

    // Chamado assim que a próxima plataforma é gerada — o obstacle aparece
    // no espaço entre as duas plataformas antes de a ponte existir.
    public void SpawnNoGap(Platform proximaPlat)
    {
        int score = ScoreManager.Instance != null ? ScoreManager.Instance.Score : 0;
        if (score < scoreParaPrimeiro) return;
        if (Random.value > chanceObstaculo) return;

        Platform atual = GameManager.Instance.CurrentPlatform;
        float gapStart = atual.RightEdge;
        float gapEnd   = proximaPlat.LeftEdge;
        float gapLen   = gapEnd - gapStart;

        // Margens: longe do player no início, longe da plataforma de chegada no fim
        const float MARGEM_INICIO = 1.0f;
        const float MARGEM_FIM    = 0.7f;

        float spanUtil = gapLen - MARGEM_INICIO - MARGEM_FIM;
        if (spanUtil < 0.3f) return; // gap pequeno demais

        float y          = atual.TopEdge + 0.175f;
        int   quantidade = score >= scoreParaDois && Random.value > 0.5f ? 2 : 1;

        for (int i = 0; i < quantidade; i++)
        {
            float zoneSize  = spanUtil / quantidade;
            float zoneStart = gapStart + MARGEM_INICIO + zoneSize * i;
            float zoneEnd   = zoneStart + zoneSize;
            if (zoneEnd - zoneStart < 0.2f) continue;

            float x = Random.Range(zoneStart, zoneEnd);
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
