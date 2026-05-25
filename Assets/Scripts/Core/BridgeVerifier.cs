using UnityEngine;

public class BridgeVerifier : MonoBehaviour
{
    public static BridgeVerifier Instance { get; private set; }

    private bool verified = false;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Update()
    {
        if (!verified && BridgeController.Instance.State == BridgeState.Done)
        {
            verified = true;
            Verify();
        }
    }

    private void Verify()
    {
        float tipX    = BridgeController.Instance.GetBridgeTipX();
        Platform next = GameManager.Instance.NextPlatform;

        bool success = tipX >= next.LeftEdge - 0.05f && tipX <= next.RightEdge + 0.05f;
        bool perfect = tipX >= next.PerfectLeftEdge   && tipX <= next.PerfectRightEdge;

        if (success && perfect)
            OnPerfect();
        else if (success)
            OnSuccess();
        else
            OnFail();
    }

    private void OnSuccess()
    {
        ScoreManager.Instance.AddNormalPoint();
        PlayerController.Instance.WalkToNextPlatform();
    }

    private void OnPerfect()
    {
        GameManager.Instance.NextPlatform.TriggerPerfectFeedback();
        ScoreManager.Instance.AddPerfectPoint();
        UIManager.Instance.ShowPerfect();
        PlayerController.Instance.WalkToNextPlatform();
    }

    private void OnFail()
    {
        ScoreManager.Instance.OnFail();
        PlayerController.Instance.FallDown();
    }

    public void ResetVerifier()
    {
        verified = false;
    }
}
