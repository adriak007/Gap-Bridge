using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public int Score { get; private set; }
    public int Combo { get; private set; }

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void AddPoint(bool perfect)
    {
        Combo++;
        Score += perfect ? 2 : 1;
        UIManager.Instance.UpdateScore(Score);
    }

    public void ResetCombo()
    {
        Combo = 0;
    }
}
