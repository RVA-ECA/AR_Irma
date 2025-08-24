using UnityEngine;
using UnityEngine.UI;
using System;

public class HistoricoButton : MonoBehaviour
{
    [HideInInspector]
    public HistoricoUIController.HistoricoEntry entry;

    public event Action<HistoricoUIController.HistoricoEntry> onClicked;

    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        if (button == null)
        {
            Debug.LogError("Botão não encontrado no objeto HistoricoButton!");
        }
        else
        {
            button.onClick.AddListener(NotifyClick);
        }
    }

    private void NotifyClick()
    {
        onClicked?.Invoke(entry);
    }
}
