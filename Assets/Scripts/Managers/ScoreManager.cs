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
}
