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
        float tipX       = BridgeController.Instance.GetBridgeTipX();
        Platform next    = GameManager.Instance.NextPlatform;

        bool success = tipX >= next.LeftEdge && tipX <= next.RightEdge;

        if (success)
            OnSuccess();
        else
            OnFail();
    }

    private void OnSuccess()
    {
        PlayerController.Instance.WalkToNextPlatform();
    }

    private void OnFail()
    {
        PlayerController.Instance.FallDown();
    }

    public void ResetVerifier()
    {
        verified = false;
    }
}
