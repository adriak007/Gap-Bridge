# Gap Bridge

Um jogo mobile casual de arcade estilo **Stick Hero**, desenvolvido em Unity 6 com URP.

O jogador segura a tela para crescer uma ponte e a solta para ela cair sobre a próxima plataforma. Se cair na zona perfeita (centro da plataforma), ganha pontos extras com multiplicador exponencial.

---

## Gameplay

1. **Segure** o botão/tela para a ponte crescer
2. **Solte** para a ponte cair (rotaciona 90°)
3. Se a ponta da ponte cair **sobre a plataforma** → sucesso, o jogador atravessa
4. Se cair na **zona perfeita** (centro ±0.2m) → bônus! Multiplicador sobe exponencialmente
5. Se a ponte não alcançar ou ultrapassar → jogador cai, fim de jogo

### Sistema de Pontuação

| Ação | Pontos |
|------|--------|
| Travessia normal | +1 ponto |
| Perfect Zone | +2 × multiplicador atual |

**Multiplicador de combo:** duplica a cada Perfect consecutivo (1×, 2×, 4×, 8×, 16× máx). O timer de combo é de 8 segundos — errar ou demorar reseta para 1×.

---

## Tecnologias

- **Unity 6.0** (6000.4.7f1)
- **C#** — toda a lógica do jogo
- **Universal Render Pipeline (URP 17.4)**
- **Input System 1.19** — suporte a mouse e touchscreen
- **TextMesh Pro** — textos da UI

---

## Estrutura de Pastas

```
Assets/
├── Scripts/
│   ├── Core/
│   │   ├── BridgeController.cs    # Cresce/cai/rotaciona a ponte (4 estados)
│   │   ├── BridgeVerifier.cs      # Detecta sucesso, falha ou perfect
│   │   ├── CameraFollow.cs        # Câmera smooth com offset à frente do player
│   │   ├── ParallaxLayer.cs       # Efeito parallax no background
│   │   ├── Platform.cs            # Bordas da plataforma + Perfect Zone
│   │   └── PlayerController.cs    # Animação de caminhada e queda
│   └── Managers/
│       ├── GameManager.cs         # Geração procedural de plataformas
│       ├── ScoreManager.cs        # Pontuação e combo
│       ├── UIManager.cs           # Display de score e feedback na tela
│       └── MenuManager.cs         # Menu principal
├── Scenes/
│   ├── MenuScene.unity
│   └── GameScene.unity
├── Prefabs/
│   ├── Bridge.prefab
│   └── Platform_Prefab.prefab
└── Materials/
    ├── BridgeMaterial.mat
    ├── PlayerMaterial.mat
    └── PerfectZoneMaterial.mat
```

---

## Configuração da Câmera

- **Modo:** Orthographic, Size 5
- **Cor de fundo:** Azul `#5B9BD5`
- **Orientação:** Portrait fixo (1080×1920)
- **Offset:** X = +1.5 (câmera levemente à frente do jogador para preview da próxima plataforma)

---

## Geração de Plataformas

Plataformas geradas proceduralmente a cada travessia:

| Parâmetro | Valor |
|-----------|-------|
| Largura da plataforma | 1.2 – 2.8 m (aleatório) |
| Distância entre plataformas | 1.8 – 3.5 m (aleatório) |
| Zona Perfeita | Centro ± 0.2 m |

---

## Detalhes Técnicos

- **Input:** Novo Input System (`UnityEngine.InputSystem`) — mouse e touch unificados
- **BridgePivot:** objeto vazio na borda da plataforma atual; a ponte é filho dele e rotaciona a partir desse pivô
- **Game Over:** recarrega cena 0 (MenuScene) via `SceneManager.LoadScene(0)`; best score salvo em `PlayerPrefs`
- **Estados da Ponte:** `Idle → Growing → Falling → Done`

---

## Como Rodar

1. Abra o projeto no **Unity 6.0** (recomendado 6000.4.7f1+)
2. Abra a cena `Assets/Scenes/MenuScene.unity`
3. Pressione **Play** no Editor ou faça build para Android/PC

---

## Status de Desenvolvimento

| Passo | Descrição | Status |
|-------|-----------|--------|
| 1 | Câmera, resolução, estrutura do projeto | ✅ Concluído |
| 2 | Plataformas, Player, GameManager | ✅ Concluído |
| 3 | BridgeController + Input System | ✅ Concluído |
| 4 | BridgeVerifier (sucesso/falha) | ✅ Concluído |
| 5 | PlayerController + CameraFollow | ✅ Concluído |
| 6 | Perfect Zone + sistema de combo | ✅ Concluído |
| 7 | ScoreManager + UIManager | ✅ Concluído |
| 8 | MenuScene (best score, botões) | ✅ Concluído |
| — | Áudio (SFX / BGM) | Pendente |
| — | Loja / Ranking / Desafios | Pendente |

---

## Autor

**adriak007** — Projeto pessoal de jogo mobile casual.
