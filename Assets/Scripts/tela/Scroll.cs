using UnityEngine;
using UnityEngine.UI;

public class PinchScrollAuto : MonoBehaviour
{
    [Header("Configurações")]
    public string indicadorTag = "IndicadorR"; // Tag do dedo indicador
    public float scrollSensibilidade = 300f;    // Velocidade do scroll

    private GameObject indicador;
    private bool emPinça = false;
    private float posAnteriorY;
    private ScrollRect scrollRect;

    void Awake()
    {
        indicador = GameObject.FindGameObjectWithTag(indicadorTag);
        if (indicador == null)
        {
            Debug.LogError("[PinchScrollAuto] Indicador não encontrado! Verifique a tag.");
        }
    }

    void Update()
    {
        if (!emPinça || indicador == null) return;

        // Se ainda não pegou o ScrollRect, tenta pegar o ativo
        if (scrollRect == null)
        {
            scrollRect = FindObjectOfType<ScrollRect>();
            if (scrollRect != null)
            {
                Debug.Log("[PinchScrollAuto] ScrollRect detectado: " + scrollRect.gameObject.name);
            }
            else
            {
                Debug.LogWarning("[PinchScrollAuto] Nenhum ScrollRect encontrado nesta cena.");
                return;
            }
        }

        // Calcula movimento vertical do indicador
        float deltaY = indicador.transform.position.y - posAnteriorY;

        // Atualiza a posição do scroll
        float newPos = scrollRect.verticalNormalizedPosition - deltaY * scrollSensibilidade * Time.deltaTime;

        // Limita entre 0 e 1
        newPos = Mathf.Clamp01(newPos);

        // Debug detalhado
        Debug.Log($"[PinchScrollAuto] Scroll movido | deltaY={deltaY:F4} | pos={scrollRect.verticalNormalizedPosition:F4}→{newPos:F4}");

        scrollRect.verticalNormalizedPosition = newPos;

        posAnteriorY = indicador.transform.position.y;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(indicadorTag))
        {
            emPinça = true;
            posAnteriorY = indicador.transform.position.y;
            Debug.Log("[PinchScrollAuto] Pinça iniciada!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(indicadorTag))
        {
            emPinça = false;
            Debug.Log("[PinchScrollAuto] Pinça finalizada!");
        }
    }
}
