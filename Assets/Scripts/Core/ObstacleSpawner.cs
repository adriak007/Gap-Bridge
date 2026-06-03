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

        // Abaixo do score minimo nao spawna
        if (score < scoreParaPrimeiro) return;

        // Chance de nao spawnar nenhum
        if (Random.value > chanceObstaculo) return;

        float pivotX      = BridgeController.Instance.GetBridgePivotX();
        float tipX        = BridgeController.Instance.GetBridgeTipX();
        float bridgeY     = GameManager.Instance.CurrentPlatform.TopEdge;
        float bridgeLen   = tipX - pivotX;

        // Quantidade de obstaculos baseada no score
        int quantidade = score >= scoreParaDois && Random.value > 0.5f ? 2 : 1;

        for (int i = 0; i < quantidade; i++)
        {
            // Divide a ponte em zonas para evitar sobreposicao
            float zoneSize = bridgeLen / quantidade;
            float zoneStart = pivotX + 0.5f + zoneSize * i;
            float zoneEnd   = zoneStart + zoneSize - 0.5f;

            if (zoneEnd <= zoneStart) continue;

            float x   = Random.Range(zoneStart, zoneEnd);
            float y   = bridgeY + 0.175f; // meia altura do obstaculo
            var   obj = Instantiate(obstaclePrefab, new Vector3(x, y, 0f), Quaternion.identity);
            ativos.Add(obj);
        }
    }

    public void LimparObstaculos()
    {
        foreach (var obj in ativos)
            if (obj != null) Destroy(obj);
        ativos.Clear();
    }
}
