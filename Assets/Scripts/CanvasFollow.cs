using UnityEngine;

public class CanvasFollow : MonoBehaviour
{
    public Transform target;           // Objeto que o Canvas seguir�
    public Vector3 offset;             // Posi��o relativa ao objeto
    public bool faceCamera = true;     // Canvas voltado para a c�mera (padr�o true)

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Segue a posi��o do alvo com offset
        transform.position = target.position + offset;

        if (faceCamera && mainCamera != null)
        {
            // Canvas olha para a c�mera
            transform.rotation = Quaternion.LookRotation(transform.position - mainCamera.transform.position);
        }
        else
        {
            // Canvas adota a mesma rota��o do alvo
            transform.rotation = target.rotation;
        }
    }
}
