using UnityEngine;

public class ToggleMenu : MonoBehaviour
{
    public GameObject menu;  // Arraste o menu para esta variável no inspector

    // Start é chamado antes do primeiro quadro
    void Start()
    {
        // Se o menu começar oculto
        if (menu.activeSelf)
        {
            menu.SetActive(false);
        }
        else
        {
            menu.SetActive(true);
        }
    }

    // Função para alternar o menu
    public void Toggle_Menu()
    {
        if (menu.activeSelf)
        {
            menu.SetActive(false);
        }
        else
        {
            menu.SetActive(true);
        }
    }
}
