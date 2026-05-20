using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private float offsetX = 1.5f; // camera a frente do jogador para ver a proxima plataforma

    private void LateUpdate()
    {
        if (target == null) return;

        float desiredX = target.position.x + offsetX;
        Vector3 desiredPos = new Vector3(desiredX, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, desiredPos, smoothSpeed * Time.deltaTime);
    }
}
