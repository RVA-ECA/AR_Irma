using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public static class InputUtils
{
    public static bool EstaEmCampoDeTexto()
    {
        var go = EventSystem.current.currentSelectedGameObject;
        if (go == null) return false;

        return go.GetComponent<UnityEngine.UI.InputField>() != null ||
               go.GetComponent<TMP_InputField>() != null;
    }
}
