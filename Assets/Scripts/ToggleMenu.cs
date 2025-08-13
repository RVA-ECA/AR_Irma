using UnityEngine;

public class ToggleMenu : MonoBehaviour
{
    public GameObject menu;  // Arraste o menu para esta vari�vel no inspector

    // Start � chamado antes do primeiro quadro
    void Start()
    {
        // Se o menu come�ar oculto
        if (menu.activeSelf)
        {
            menu.SetActive(false);
        }
        else
        {
            menu.SetActive(true);
        }
    }

    // Fun��o para alternar o menu
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
