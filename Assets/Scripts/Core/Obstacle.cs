using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private float hitRangeX = 0.22f;
    [SerializeField] private float hitRangeY = 0.28f;

    private bool atingiu = false;

    private void Update()
    {
        if (atingiu) return;
        if (PlayerController.Instance == null) return;
        if (!PlayerController.Instance.IsOnGround) return;

        Vector3 playerPos   = PlayerController.Instance.transform.position;
        float   distX       = Mathf.Abs(playerPos.x - transform.position.x);
        float   distY       = Mathf.Abs(playerPos.y - transform.position.y);

        if (distX < hitRangeX && distY < hitRangeY)
        {
            atingiu = true;
            PlayerController.Instance.HitObstacle();
        }
    }
}
