using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

[System.Serializable]
public class MenuConfig
{
    public GameObject menu;
    public InputActionProperty exibirBotao;
}

public class MenuController : MonoBehaviour
{
    public Transform jogador;
    public float distanciaDoJogador = 3.0f;
    public MenuConfig[] menus;

    void OnEnable()
    {
        foreach (var config in menus)
        {
            config.exibirBotao.action.Enable();
        }
    }

    void OnDisable()
    {
        foreach (var config in menus)
        {
            config.exibirBotao.action.Disable();
        }
    }
    void Update()
    {

        if (InputUtils.EstaEmCampoDeTexto())
            return;

        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Atalho R executado");
        }

        foreach (var config in menus)
        {
            if (config.exibirBotao.action.WasPressedThisFrame())
            {
                // Se o menu já estiver aberto, apenas fecha ele
                if (config.menu.activeSelf)
                {
                    config.menu.SetActive(false);
                    Debug.Log($"Fechando: {config.menu.name}");
                }
                else
                {
                    // Fecha todos os outros menus
                    foreach (var other in menus)
                    {
                        if (other.menu != null)
                            other.menu.SetActive(false);
                    }

                    // Abre o menu atual
                    config.menu.SetActive(true);
                    config.menu.transform.position = jogador.position + jogador.forward * distanciaDoJogador;
                    config.menu.transform.LookAt(jogador.position);
                    config.menu.transform.forward *= -1;

                    Debug.Log($"Abrindo: {config.menu.name}");
                }
            }
        }
    }
}
