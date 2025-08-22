using UnityEngine;
using UnityEngine.UI;

public class HandScroll : MonoBehaviour
{
    public Scrollbar scrollbar;
    public Transform rightHandIndicator;
    private Vector3 touchPoint;
    private float initialScrollValue;
    private bool isTouching = false;

    public float scrollSensitivity = 0.5f;

    // Esta fun��o � chamada quando um colisor de gatilho entra
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Indicador"))
        {
            isTouching = true;
            touchPoint = rightHandIndicator.position;
            initialScrollValue = scrollbar.value;
            Debug.Log("OnTriggerEnter: Tocando a scrollbar com o Indicador.");
        }
    }

    // Esta fun��o � chamada a cada frame enquanto os colisores de gatilho est�o se tocando
    void OnTriggerStay(Collider other)
    {
        if (isTouching && other.CompareTag("Indicador"))
        {
            // A rolagem s� acontece aqui dentro!
            float deltaY = rightHandIndicator.position.y - touchPoint.y;
            float newScrollValue = initialScrollValue + deltaY * scrollSensitivity;
            scrollbar.value = Mathf.Clamp01(newScrollValue);
            Debug.Log("OnTriggerStay: Scrollbar se movendo.");
        }
    }

    // Esta fun��o � chamada quando um colisor de gatilho sai
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Indicador"))
        {
            isTouching = false;
            Debug.Log("OnTriggerExit: Parando o scroll.");
        }
    }
}