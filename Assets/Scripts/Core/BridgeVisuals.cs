using UnityEngine;

// Aplica cor de madeira na ponte assim que ela é criada
public class BridgeVisuals : MonoBehaviour
{
    private void Start()
    {
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
            rend.material.color = GameTheme.Bridge;
    }
}
