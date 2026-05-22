using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [Header("Parallax")]
    [SerializeField] private float parallaxStrength = 0.15f; // quanto desloca ao andar
    [SerializeField] private float returnSpeed      = 5f;    // velocidade de retorno ao centro

    private Camera mainCamera;
    private float lastCamX;
    private float offset;       // deslocamento atual do parallax
    private float baseOffsetX;  // distancia inicial entre fundo e camera

    private void Start()
    {
        mainCamera  = Camera.main;
        lastCamX    = mainCamera.transform.position.x;
        baseOffsetX = transform.position.x - mainCamera.transform.position.x;
    }

    private void LateUpdate()
    {
        float delta = mainCamera.transform.position.x - lastCamX;

        // Quando a camera move, empurra o fundo na direcao oposta (efeito de profundidade)
        offset -= delta * parallaxStrength;

        // Retorna suavemente ao centro quando a camera para
        offset = Mathf.Lerp(offset, 0f, returnSpeed * Time.deltaTime);

        // Fundo sempre segue a camera + pequeno offset de parallax
        float newX = mainCamera.transform.position.x + baseOffsetX + offset;
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);

        lastCamX = mainCamera.transform.position.x;
    }
}
