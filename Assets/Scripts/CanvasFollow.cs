using UnityEngine;

public class CanvasFollow : MonoBehaviour
{
    public Transform target;           // Objeto que o Canvas seguirá
    public Vector3 offset;             // Posição relativa ao objeto
    public bool faceCamera = true;     // Canvas voltado para a câmera (padrão true)

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Segue a posição do alvo com offset
        transform.position = target.position + offset;

        if (faceCamera && mainCamera != null)
        {
            // Canvas olha para a câmera
            transform.rotation = Quaternion.LookRotation(transform.position - mainCamera.transform.position);
        }
        else
        {
            // Canvas adota a mesma rotação do alvo
            transform.rotation = target.rotation;
        }
    }
}
