using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Combo")]
    [SerializeField] private float comboTimeLimit = 8f; // segundos para manter o combo
    [SerializeField] private int   maxMultiplier  = 16; // teto do multiplicador

    public int   Score           { get; private set; }
    public int   Multiplier      { get; private set; } = 1;
    public float ComboTimeLeft   { get; private set; }

    private int consecutivePerfects = 0;
    private bool comboActive        = false;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Update()
    {
        if (!comboActive) return;

        ComboTimeLeft -= Time.deltaTime;
        UIManager.Instance.UpdateComboTimer(ComboTimeLeft / comboTimeLimit);

        if (ComboTimeLeft <= 0f)
            ResetCombo();
    }

    // Chamado ao atravessar normalmente (sem perfect)
    public void AddNormalPoint()
    {
        Score += 1;
        UIManager.Instance.UpdateScore(Score);

        // Cruzamento normal nao reseta o combo, mas para o timer
        // (jogador nao perde o multiplicador por cruzar normal, so por errar ou timer)
    }

    // Chamado ao acertar a perfect zone
    public void AddPerfectPoint()
    {
        consecutivePerfects++;
        Multiplier    = Mathf.Min((int)Mathf.Pow(2, consecutivePerfects - 1), maxMultiplier);
        ComboTimeLeft = comboTimeLimit;
        comboActive   = true;

        int points = 2 * Multiplier;
        Score += points;

        UIManager.Instance.UpdateScore(Score);
        UIManager.Instance.ShowMultiplier(Multiplier);
    }

    // Chamado ao cair (falhar)
    public void OnFail()
    {
        ResetCombo();
    }

    private void ResetCombo()
    {
        consecutivePerfects = 0;
        Multiplier          = 1;
        ComboTimeLeft       = 0f;
        comboActive         = false;
        UIManager.Instance.HideCombo();
    }

    public void SaveHighScore()
    {
        int current = PlayerPrefs.GetInt("BestScore", 0);
        if (Score > current)
            PlayerPrefs.SetInt("BestScore", Score);

        // Award coins: 1 moeda por ponto
        int coins = PlayerPrefs.GetInt("Coins", 0) + Mathf.Max(Score, 0);
        PlayerPrefs.SetInt("Coins", coins);

        AddToTopScores(Score);
        PlayerPrefs.Save();
    }

    private void AddToTopScores(int score)
    {
        if (score <= 0) return;
        var scores = new System.Collections.Generic.List<int>();
        for (int i = 0; i < 5; i++)
        {
            int s = PlayerPrefs.GetInt("TopScore_" + i, 0);
            if (s > 0) scores.Add(s);
        }
        scores.Add(score);
        scores.Sort((a, b) => b.CompareTo(a));
        if (scores.Count > 5) scores.RemoveRange(5, scores.Count - 5);
        for (int i = 0; i < scores.Count; i++)
            PlayerPrefs.SetInt("TopScore_" + i, scores[i]);
    }
}
