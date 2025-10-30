using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[DisallowMultipleComponent]
public class UIItemVisibilityController : MonoBehaviour
{
    private RectTransform viewport;
    private RectTransform item;
    private Collider colliderComp;

    void Start()
    {
        // Pega o viewport do ScrollRect mais próximo acima
        var scrollRect = GetComponentInParent<ScrollRect>();
        if (scrollRect == null)
        {
            Debug.LogWarning($"{name}: Nenhum ScrollRect encontrado no pai.");
            return;
        }

        viewport = scrollRect.viewport;
        item = GetComponent<RectTransform>();
        colliderComp = GetComponent<Collider>();

        if (colliderComp == null)
            Debug.LogWarning($"{name}: Nenhum Collider encontrado — o script será inativo.");
    }

    void Update()
    {
        if (viewport == null || item == null || colliderComp == null)
            return;

        // Verifica se o botão está dentro da área visível do viewport
        bool isVisible = RectTransformUtility.RectangleContainsScreenPoint(
            viewport, item.position, null
        );

        colliderComp.enabled = isVisible;
    }
}
