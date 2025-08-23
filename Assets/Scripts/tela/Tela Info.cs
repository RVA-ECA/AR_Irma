using UnityEngine;

public class TelaManager : MonoBehaviour
{
    public GameObject tela;   // A tela (painel ou canvas) que será alternada

    // Método chamado quando o botão é clicado
    public void AlternarTela()
    {
        // Verifica o estado atual da tela
        if (tela != null)
        {
            // Alterna o estado ativo da tela
            bool isActive = tela.activeSelf;  // Verifica se a tela está ativa
            tela.SetActive(!isActive);        // Se estiver ativa, desativa; se não, ativa

            // Imprime no console o estado da tela
            if (isActive)
                Debug.Log("Tela desativada.");
            else
                Debug.Log("Tela ativada.");
        }
        else
        {
            Debug.LogError("A tela não foi atribuída corretamente no Inspector.");
        }
    }
}
