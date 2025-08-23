using UnityEngine;

public class TelaManager : MonoBehaviour
{
    public GameObject tela;   // A tela (painel ou canvas) que ser� alternada

    // M�todo chamado quando o bot�o � clicado
    public void AlternarTela()
    {
        // Verifica o estado atual da tela
        if (tela != null)
        {
            // Alterna o estado ativo da tela
            bool isActive = tela.activeSelf;  // Verifica se a tela est� ativa
            tela.SetActive(!isActive);        // Se estiver ativa, desativa; se n�o, ativa

            // Imprime no console o estado da tela
            if (isActive)
                Debug.Log("Tela desativada.");
            else
                Debug.Log("Tela ativada.");
        }
        else
        {
            Debug.LogError("A tela n�o foi atribu�da corretamente no Inspector.");
        }
    }
}
