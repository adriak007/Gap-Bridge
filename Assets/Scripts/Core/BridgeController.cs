using UnityEngine;
using UnityEngine.InputSystem;

public enum BridgeState { Idle, Growing, Falling, Done }

public class BridgeController : MonoBehaviour
{
    public static BridgeController Instance { get; private set; }

    [Header("Configuracoes da Ponte")]
    [SerializeField] private GameObject bridgePrefab;
    [SerializeField] private float growSpeed  = 3f;
    [SerializeField] private float fallSpeed  = 200f; // graus por segundo
    [SerializeField] private float bridgeScaleX = 0.5f; // escala uniforme no eixo X da ponte inteira (espessura)

    public BridgeState State       { get; private set; } = BridgeState.Idle;
    public float       BridgeLength { get; private set; }
    public bool        LockRelease  { get; set; } = false;

    private Transform pivotTransform;
    private Transform bridgeTransform;
    private SpriteRenderer[] bridgeSegmentRenderers;
    private float[] bridgeSegmentWidths; // largura fixa de cada sprite (sua altura nativa, ja que estao rotacionadas 90)
    private float fallAngle;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    // Cria o pivo e a ponte uma unica vez; reaproveitados em toda travessia
    private void EnsureBridgeObjects()
    {
        if (pivotTransform != null) return;

        GameObject pivotObj = new GameObject("BridgePivot");
        pivotTransform = pivotObj.transform;

        GameObject bridge = Instantiate(bridgePrefab, pivotTransform);
        bridgeTransform = bridge.transform;
        // Escala X uniforme na ponte inteira (espessura) -- preserva a proporcao entre Planks/Rails
        bridgeTransform.localScale = new Vector3(bridgeScaleX, 1f, 1f);
        bridgeSegmentRenderers = bridge.GetComponentsInChildren<SpriteRenderer>();

        // Largura fixa = altura nativa de cada sprite (em unidades), pois cada filho esta
        // rotacionado 90 graus: a altura original passa a ser a direcao perpendicular ao crescimento
        bridgeSegmentWidths = new float[bridgeSegmentRenderers.Length];
        for (int i = 0; i < bridgeSegmentRenderers.Length; i++)
        {
            Sprite spr = bridgeSegmentRenderers[i].sprite;
            bridgeSegmentWidths[i] = spr.rect.height / spr.pixelsPerUnit;
        }

        pivotObj.SetActive(false);
    }

    // Atualiza o tamanho via Tiled Draw Mode (repete a sprite) em vez de esticar por escala.
    // Planks/Rails estao rotacionados 90 graus no Z, entao o eixo local que cresce
    // (alinhado com o Y do pai, direcao de crescimento) e o X deles, nao o Y.
    private void SetBridgeSize(float length)
    {
        for (int i = 0; i < bridgeSegmentRenderers.Length; i++)
            bridgeSegmentRenderers[i].size = new Vector2(length, bridgeSegmentWidths[i]);

        bridgeTransform.localPosition = new Vector3(0f, length / 2f, 0f);
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
        if (released && State == BridgeState.Growing && !LockRelease) StartFalling();
    }

    private void StartGrowing()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager nao encontrado na cena!");
            return;
        }
        if (GameManager.Instance.CurrentPlatform == null)
        {
            Debug.LogError("CurrentPlatform esta nulo! Platform_Start tem o script Platform?");
            return;
        }
        if (bridgePrefab == null)
        {
            Debug.LogError("Bridge Prefab nao atribuido no BridgeController!");
            return;
        }

        EnsureBridgeObjects();

        State        = BridgeState.Growing;
        BridgeLength = 0f;
        fallAngle    = 0f;
        if (AudioManager.Instance) AudioManager.Instance.PlayGrowingStart();

        Platform current = GameManager.Instance.CurrentPlatform;
        Vector3 pivotPos = new Vector3(current.RightEdge, current.TopEdge, 0f);

        // Reposiciona o pivo (ja existente) na borda da plataforma atual
        pivotTransform.position    = pivotPos;
        pivotTransform.eulerAngles = Vector3.zero;
        pivotTransform.gameObject.SetActive(true);

        // Ponte comecando com comprimento zero
        SetBridgeSize(0f);
    }

    private void GrowBridge()
    {
        BridgeLength += growSpeed * Time.deltaTime;
        SetBridgeSize(BridgeLength);
    }

    private void StartFalling()
    {
        State = BridgeState.Falling;
        if (AudioManager.Instance) AudioManager.Instance.PlayGrowingStop();
        if (AudioManager.Instance) AudioManager.Instance.PlayFall();
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

    public void ForceStopGrowing()
    {
        if (State == BridgeState.Growing) StartFalling();
    }

    public float GetBridgeTipX()   => pivotTransform.position.x + BridgeLength;
    public float GetBridgePivotX() => pivotTransform.position.x;

    public void ResetBridge()
    {
        if (pivotTransform != null)
            pivotTransform.gameObject.SetActive(false);

        State = BridgeState.Idle;
    }
}
