using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Verifica se encostou no player
        if (other.GetComponent<PlayerController>() == null) return;

        // So derruba se o player estiver no chao (nao pulando)
        if (PlayerController.Instance.IsOnGround)
            PlayerController.Instance.FallDown();
    }
}
