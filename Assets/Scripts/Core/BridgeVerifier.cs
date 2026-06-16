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
        if (!TutorialManager.IsActive) ScoreManager.Instance.AddNormalPoint();
        if (AudioManager.Instance)  AudioManager.Instance.PlaySuccess();
        PlayerController.Instance.WalkToNextPlatform();
    }

    private void OnPerfect()
    {
        if (!TutorialManager.IsActive)
        {
            GameManager.Instance.NextPlatform.TriggerPerfectFeedback();
            ScoreManager.Instance.AddPerfectPoint();
            UIManager.Instance.ShowPerfect();
        }
        if (AudioManager.Instance)  AudioManager.Instance.PlayPerfect();
        if (ScreenEffects.Instance) ScreenEffects.Instance.FlashWhite();
        PlayerController.Instance.WalkToNextPlatform();
    }

    private void OnFail()
    {
        ScoreManager.Instance.OnFail();
        if (AudioManager.Instance)  AudioManager.Instance.PlayFail();
        if (ScreenEffects.Instance) ScreenEffects.Instance.ShakeCamera();
        if (ScreenEffects.Instance) ScreenEffects.Instance.FlashRed();
        PlayerController.Instance.FallDown();
    }

    public void ResetVerifier()
    {
        verified = false;
    }
}
