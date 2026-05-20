using UnityEngine;
using UnityEngine.InputSystem;

public enum BridgeState { Idle, Growing, Falling, Done }

public class BridgeController : MonoBehaviour
{
    public static BridgeController Instance { get; private set; }

    [Header("Configuracoes da Ponte")]
    [SerializeField] private GameObject bridgePrefab;
    [SerializeField] private float growSpeed = 3f;
    [SerializeField] private float fallSpeed = 200f; // graus por segundo

    public BridgeState State { get; private set; } = BridgeState.Idle;
    public float BridgeLength { get; private set; }

    private Transform pivotTransform;
    private float fallAngle;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Update()
    {
        HandleInput();

        if (State == BridgeState.Growing) GrowBridge();
        if (State == BridgeState.Falling) RotateBridge();
    }

    private void HandleInput()
    {
        bool pressed  = false;
        bool released = false;

        // Mouse (teste no Editor)
        if (Mouse.current != null)
        {
            pressed  = Mouse.current.leftButton.wasPressedThisFrame;
            released = Mouse.current.leftButton.wasReleasedThisFrame;
        }

        // Touch (mobile)
        if (Touchscreen.current != null)
        {
            var touch = Touchscreen.current.primaryTouch;
            pressed  = pressed  || touch.press.wasPressedThisFrame;
            released = released || touch.press.wasReleasedThisFrame;
        }

        if (pressed  && State == BridgeState.Idle)    StartGrowing();
        if (released && State == BridgeState.Growing) StartFalling();
    }

    private void StartGrowing()
    {
        State        = BridgeState.Growing;
        BridgeLength = 0f;
        fallAngle    = 0f;

        Platform current = GameManager.Instance.CurrentPlatform;
        Vector3 pivotPos = new Vector3(current.RightEdge, current.TopEdge, 0f);

        // Objeto vazio na borda da plataforma — serve como pivo de rotacao
        GameObject pivotObj = new GameObject("BridgePivot");
        pivotObj.transform.position = pivotPos;
        pivotTransform = pivotObj.transform;

        // Ponte como filho do pivo, comecando com altura zero
        GameObject bridge = Instantiate(bridgePrefab, pivotTransform);
        bridge.transform.localPosition = Vector3.zero;
        bridge.transform.localScale    = new Vector3(0.15f, 0f, 0.15f);
    }

    private void GrowBridge()
    {
        BridgeLength += growSpeed * Time.deltaTime;

        Transform bridge = pivotTransform.GetChild(0);
        bridge.localScale    = new Vector3(0.15f, BridgeLength, 0.15f);
        // Move para cima junto com o crescimento (pivot na base)
        bridge.localPosition = new Vector3(0f, BridgeLength / 2f, 0f);
    }

    private void StartFalling()
    {
        State = BridgeState.Falling;
    }

    private void RotateBridge()
    {
        fallAngle -= fallSpeed * Time.deltaTime;

        if (fallAngle <= -90f)
        {
            fallAngle = -90f;
            pivotTransform.eulerAngles = new Vector3(0f, 0f, fallAngle);
            State = BridgeState.Done;
            return;
        }

        pivotTransform.eulerAngles = new Vector3(0f, 0f, fallAngle);
    }

    // Retorna a posicao da ponta da ponte (usada no Passo 4 para verificacao)
    public float GetBridgeTipX()
    {
        return pivotTransform.position.x + BridgeLength;
    }

    public void ResetBridge()
    {
        if (pivotTransform != null)
            Destroy(pivotTransform.gameObject);

        State = BridgeState.Idle;
    }
}
