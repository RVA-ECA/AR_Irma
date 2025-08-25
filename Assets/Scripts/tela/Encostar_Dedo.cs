    using UnityEngine;
using UnityEngine.UI;

public class Encostar_Dedo : MonoBehaviour
{
    private GameObject object1;        // O objeto onde o script está (Objeto 1)
    private GameObject object2;       // Objeto com tag "Indicador" (Dedo ou indicador)
    private Button onClickButton;     // Botão que está no mesmo objeto (Objeto 1)

    private float distanceThreshold = 0.05f; // Distância para acionar o clique
    private float resetThreshold = 0.06f;    // Distância para resetar a detecção (se o dedo se afastar)

    private bool acionado = false;    // Flag para garantir que o clique não aconteça várias vezes

    void Awake()
    {
        // Puxa o próprio objeto (Objeto 1) onde o script está
        object1 = gameObject;

        // Puxa o componente Button do mesmo objeto (se existir)
        onClickButton = GetComponent<Button>();

        if (onClickButton == null)
        {
            Debug.LogWarning("Nenhum Button encontrado neste objeto. Adicione um componente Button se quiser usar o clique.");
        }

        // Procura o objeto com a tag "Indicador" (o dedo)
        object2 = GameObject.FindGameObjectWithTag("Indicador");

        if (object2 == null)
        {
            Debug.LogError("Nenhum objeto com a tag 'Indicador' foi encontrado na cena!");
        }
    }

    void Update()
    {
        if (object2 == null || onClickButton == null) return;

        // Calcula a distância entre o dedo (object2) e o objeto (object1)
        float distance = Vector3.Distance(object1.transform.position, object2.transform.position);
        Debug.Log($"Distância: {distance}");

        // Quando a distância for menor que o limiar, aciona o clique
        if (!acionado && distance < distanceThreshold)
        {
            Debug.Log("Acionou o clique no botão!");
            // Dispara o evento de clique no próprio botão
            onClickButton.onClick.Invoke();
            acionado = true; // Impede múltiplos acionamentos
        }
        else if (acionado && distance > resetThreshold)
        {
            // Se a distância aumentar além do limiar, reseta a detecção
            Debug.Log("Resetado, pronto para acionar novamente.");
            acionado = false; // Permite novo acionamento
        }
    }
}
