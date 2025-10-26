using UnityEngine;
using UnityEngine.UI;

public class Encostar_Dedo_Collider : MonoBehaviour
{
    private Button onClickButton; // Botão que está no mesmo objeto

    void Awake()
    {
        // Puxa o componente Button do mesmo objeto
        onClickButton = GetComponent<Button>();

        if (onClickButton == null)
        {
            Debug.LogWarning("Nenhum componente Button encontrado neste objeto.");
        }
    }

    // Chamado quando outro colisor entra no trigger deste objeto
    private void OnTriggerEnter(Collider other)
    {
        // Verifica se o objeto que colidiu tem a tag "Indicador"
        if (other.CompareTag("IndicadorR"))
        {
            // Dispara o evento de clique do botão
            if (onClickButton != null)
            {
                onClickButton.onClick.Invoke();
                Debug.Log("Colisão detectada! Clicando no botão.");
            }
        }
    }

    // Opcional: Para feedback visual, você pode usar o OnTriggerExit
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("IndicadorR"))
        {
            Debug.Log("Indicador saiu da colisão. Pronto para novo clique.");
        }
    }
}