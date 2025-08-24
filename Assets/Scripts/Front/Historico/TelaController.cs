using UnityEngine;
using UnityEngine.UI;
using System;

public class PrintMessageButton : MonoBehaviour
{
    public event Action onClicked;

    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        if (button == null)
        {
            Debug.LogError("Botão não encontrado no objeto PrintMessageButton!");
        }
        else
        {
            button.onClick.AddListener(NotifyClick);
        }
    }

    private void NotifyClick()
    {
        onClicked?.Invoke();
    }
}