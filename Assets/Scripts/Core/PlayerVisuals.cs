using UnityEngine;

// Constrói o personagem cartoon: corpo + cabeça + olhos
public class PlayerVisuals : MonoBehaviour
{
    private void Start()
    {
        BuildCharacter();
    }

    private void BuildCharacter()
    {
        // Corpo (capsule existente)
        Renderer body = GetComponent<Renderer>();
        if (body != null)
            body.material.color = GameTheme.PlayerBody;

        // Cabeça
        GameObject head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        head.name = "Head";
        head.transform.SetParent(transform);
        head.transform.localPosition = new Vector3(0f, 1.1f, 0f);
        head.transform.localScale    = new Vector3(0.75f, 0.75f, 0.75f);
        head.GetComponent<Renderer>().material.color = GameTheme.PlayerHead;
        Destroy(head.GetComponent<SphereCollider>());

        // Olho esquerdo
        CriarOlho(head.transform, new Vector3(-0.22f, 0.1f, -0.45f));
        // Olho direito
        CriarOlho(head.transform, new Vector3( 0.22f, 0.1f, -0.45f));
    }

    private void CriarOlho(Transform pai, Vector3 localPos)
    {
        GameObject olho = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        olho.name = "Eye";
        olho.transform.SetParent(pai);
        olho.transform.localPosition = localPos;
        olho.transform.localScale    = new Vector3(0.22f, 0.22f, 0.12f);
        olho.GetComponent<Renderer>().material.color = new Color(0.1f, 0.1f, 0.1f);
        Destroy(olho.GetComponent<SphereCollider>());
    }
}
