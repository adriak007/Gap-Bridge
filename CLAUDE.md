# Gap Bridge — CLAUDE.md

Jogo mobile casual estilo Stick Hero, construído em Unity 6.4 com URP.
Portrait mode, câmera ortográfica (Size 5), resolução 1080x1920.

---

## Regras de código (NUNCA violar)

- **Input System**: SEMPRE usar `UnityEngine.InputSystem` (Mouse.current / Touchscreen.current). NUNCA usar `Input.GetMouseButton` (Input System legado).
- **Null check Unity**: NUNCA usar `?.` em objetos Unity. Usar `if (X) X.Method()`.
- **Singleton**: Todo manager usa `public static T Instance { get; private set; }` com guard no Awake.
- **Propriedades computadas**: Nunca cachear Width/Edges da Platform no Awake — GameManager seta o scale depois do Instantiate. Sempre `=> transform...` (computed property).
- **FindObjectByType**: Usar `FindAnyObjectByType<T>()`. NÃO usar `FindFirstObjectByType` (deprecado no Unity 6).
- **MenuManager**: Usar blocos `{ }` explícitos nos métodos — arrow `=>` não aparece no dropdown de OnClick do Button.
- **Cenas**: "MenuScene" e "GameScene". Transição via `SceneManager.LoadScene("...")`.
- **UI Text**: Sempre usar `TMP_Text` (TextMeshPro). Nunca `Text` legado.

---

## Arquitetura

```
Assets/Scripts/
├── Core/
│   ├── Platform.cs          — Bordas e Perfect Zone da plataforma
│   ├── BridgeController.cs  — Crescimento e queda da ponte (state machine)
│   ├── BridgeVerifier.cs    — Detecta success / perfect / fail
│   ├── PlayerController.cs  — Movimento, pulo (física), queda, obstáculo
│   ├── CameraFollow.cs      — Segue player no eixo X (LateUpdate, Lerp)
│   ├── ParallaxLayer.cs     — Parallax reativo: move e volta ao centro
│   ├── Obstacle.cs          — Detecção por distância (sem Rigidbody/Trigger)
│   └── ObstacleSpawner.cs   — Spawna obstáculos em cima da ponte
└── Managers/
    ├── GameManager.cs       — Spawn e avanço de plataformas, dificuldade
    ├── ScoreManager.cs      — Pontos, combo, multiplicador, PlayerPrefs
    ├── AudioManager.cs      — DontDestroyOnLoad, 3 AudioSources
    └── MenuManager.cs       — Menu principal, cenas, highscore
UI/
    └── UIManager.cs         — Score, Perfect!, combo timer bar
```

---

## Valores-chave

| Constante | Valor | Onde |
|---|---|---|
| platformTopY | -3.25f | GameManager |
| pillarHeight | 8f | GameManager |
| centerY (pilar) | -7.25f | platformTopY - pillarHeight/2 |
| Camera ortho size | 5 | Camera |
| cameraOffsetX | 1.5f | CameraFollow |
| jumpForce | 11f | PlayerController |
| jumpGravity | 22f | PlayerController |
| scoreParaDificuldadeMax | 30 | GameManager |
| perfectZoneWidth | 0.4f | Platform |

---

## Platform — propriedades computadas

```csharp
public float Width     => transform.localScale.x;
public float RightEdge => transform.position.x + Width / 2f;
public float LeftEdge  => transform.position.x - Width / 2f;
public float TopEdge   => transform.position.y + transform.localScale.y / 2f;
public float PerfectLeftEdge  => transform.position.x - perfectZoneWidth / 2f;
public float PerfectRightEdge => transform.position.x + perfectZoneWidth / 2f;
```

---

## PlayerController — lógica de movimento

- **WalkToNextPlatform()**: seta `groundY`, `targetX`, `isWalking = true`
- **FallDown()**: `isWalking = false`, `isFalling = true` (para horizontal NA HORA)
- **HitObstacle()**: chama ScoreManager.OnFail + efeitos + FallDown()
- **OnArrived()**: LimparObstaculos → AdvanceToNextPlatform → ResetBridge → ResetVerifier
- **OnGameOver()**: SaveHighScore → LoadScene("MenuScene")
- Eixos X e Y são **completamente independentes**
- `IsOnGround => jumpYOffset <= 0.05f` (usado pelo Obstacle para não matar em pulo)

---

## BridgeController — estados

```
Idle → (press) → Growing → (release) → Falling → Done
```
- Pivot criado na `CurrentPlatform.RightEdge / TopEdge`
- `GetBridgeTipX() = pivotTransform.position.x + BridgeLength`
- ResetBridge() destrói o pivot object

---

## Obstacle — detecção por distância

```csharp
// Sem Rigidbody, sem Trigger — checado no Update
float distX = Mathf.Abs(playerPos.x - transform.position.x);
float distY = Mathf.Abs(playerPos.y - transform.position.y);
if (distX < hitRangeX && distY < hitRangeY) { ... }
```
- Só detecta quando `IsOnGround` (não matar durante pulo)
- `hitRangeX = 0.22f`, `hitRangeY = 0.28f`

---

## ParallaxLayer — parallax reativo

```csharp
// NÃO filho da câmera
offset -= delta * parallaxStrength;
offset = Mathf.Lerp(offset, 0f, returnSpeed * Time.deltaTime);
// parallaxStrength = 0.15f, returnSpeed = 5f
```

---

## Dificuldade progressiva (GameManager)

Usa `Mathf.Lerp(easy, hard, t)` onde `t = score / 30f`.
- Plataforma fácil: 1.2–2.8 largura, gap 1.8–3.5
- Plataforma difícil: 0.7–1.6 largura, gap 2.8–5.0

---

## Score / Combo (ScoreManager)

- **AddNormalPoint()**: Score += 1
- **AddPerfectPoint()**: consecutivos dobram o Multiplier (max 16), Score += 2 × Multiplier
- **OnFail()**: reseta combo e multiplier
- **comboTimeLimit**: 8f segundos

---

## AudioManager

- 3 AudioSources: `musicSource`, `sfxSource`, `growingSource`
- DontDestroyOnLoad
- Todos os clips são opcionais (null check antes de Play)
- Métodos: PlayGrowingStart/Stop, PlayFall, PlaySuccess, PlayPerfect, PlayFail

---

## ScreenEffects

- **ShakeCamera(0.35s, 0.18)**: move Camera.main
- **FlashRed()**: Image UI full-screen, vermelho #FF0000 alpha 0.35, fade 0.3s
- **FlashWhite()**: branco alpha 0.25, fade
- Requer `flashImage` (UI Image, full screen, transparente, Raycast Target OFF)

---

## Hierarquia da GameScene (referência)

```
GameScene
├── [Manager] GameManager
├── [Manager] ScoreManager
├── [Manager] AudioManager
├── [Manager] BridgeController
├── [Manager] BridgeVerifier
├── [Manager] ObstacleSpawner
├── Platform_Start         ← tem script Platform
├── Main Camera            ← CameraFollow, ScreenEffects
├── Background             ← ParallaxLayer (NÃO filho da câmera)
└── Canvas
    ├── ScoreText (TMP)
    ├── PerfectText (TMP)
    ├── ComboContainer
    │   ├── ComboText (TMP)
    │   └── ComboTimerBar (Image, Fill Horizontal)
    └── FlashImage         ← full screen, alpha 0, Raycast Target OFF
```

---

## O que ainda falta (pendente)

- [ ] Atribuir audio clips no AudioManager (pasta `Assets/Casual Game Sounds U6/`)
- [ ] Coins, loja, outros extras do PASSO 10
- [ ] Ícone e splash screen para build mobile

---

## Erros comuns já resolvidos (não repetir)

| Erro | Causa | Solução |
|---|---|---|
| NullReference no BridgeController | GameManager sumiu em Play mode | Criar objetos em Edit mode, Ctrl+S antes de Play |
| Platform.Width errado | Valor cacheado no Awake antes do Instantiate+SetScale | Computed property `=> transform.localScale.x` |
| Método não aparece no OnClick | Arrow `=>` no MenuManager | Usar blocos `{ }` explícitos |
| Obstacle não mata player | Sem Rigidbody, OnTrigger não dispara | Detecção por distância no Update |
| Player continua andando após obstáculo | FallDown não parava horizontal | `isWalking = false` no início de FallDown |
| FindFirstObjectByType deprecated | Unity 6 | `FindAnyObjectByType<T>()` |
| ScoreManager duplicado | NullReference em cascata | Remover duplicata da hierarquia |
