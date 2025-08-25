    using UnityEngine;
using UnityEngine.UI;

public class Encostar_Dedo : MonoBehaviour
{
    private GameObject object1;        // O objeto onde o script est� (Objeto 1)
    private GameObject object2;       // Objeto com tag "Indicador" (Dedo ou indicador)
    private Button onClickButton;     // Bot�o que est� no mesmo objeto (Objeto 1)

    private float distanceThreshold = 0.05f; // Dist�ncia para acionar o clique
    private float resetThreshold = 0.06f;    // Dist�ncia para resetar a detec��o (se o dedo se afastar)

    private bool acionado = false;    // Flag para garantir que o clique n�o aconte�a v�rias vezes

    void Awake()
    {
        // Puxa o pr�prio objeto (Objeto 1) onde o script est�
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

        // Calcula a dist�ncia entre o dedo (object2) e o objeto (object1)
        float distance = Vector3.Distance(object1.transform.position, object2.transform.position);
        Debug.Log($"Dist�ncia: {distance}");

        // Quando a dist�ncia for menor que o limiar, aciona o clique
        if (!acionado && distance < distanceThreshold)
        {
            Debug.Log("Acionou o clique no bot�o!");
            // Dispara o evento de clique no pr�prio bot�o
            onClickButton.onClick.Invoke();
            acionado = true; // Impede m�ltiplos acionamentos
        }
        else if (acionado && distance > resetThreshold)
        {
            // Se a dist�ncia aumentar al�m do limiar, reseta a detec��o
            Debug.Log("Resetado, pronto para acionar novamente.");
            acionado = false; // Permite novo acionamento
        }
    }
}
