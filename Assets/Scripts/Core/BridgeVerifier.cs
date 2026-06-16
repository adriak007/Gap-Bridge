using UnityEngine;

public class BridgeVerifier : MonoBehaviour
{
    public static BridgeVerifier Instance { get; private set; }

    private bool verified        = false;
    private bool obstacleSpawned = false;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Update()
    {
        // Spawna o obstáculo assim que o player solta a ponte (começa a cair).
        // O obstáculo fica visível durante toda a animação de queda (~0.45s),
        // dando tempo do player planejar onde vai pular.
        if (!obstacleSpawned && BridgeController.Instance.State == BridgeState.Falling)
        {
            obstacleSpawned = true;
            if (ObstacleSpawner.Instance) ObstacleSpawner.Instance.SpawnNaPonte();
        }

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
        if (AudioManager.Instance)  AudioManager.Instance.PlaySuccess();
        PlayerController.Instance.WalkToNextPlatform();
    }

    private void OnPerfect()
    {
        GameManager.Instance.NextPlatform.TriggerPerfectFeedback();
        ScoreManager.Instance.AddPerfectPoint();
        UIManager.Instance.ShowPerfect();
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
        verified        = false;
        obstacleSpawned = false;
    }
}
