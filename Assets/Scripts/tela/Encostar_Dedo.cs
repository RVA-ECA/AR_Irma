using UnityEngine;
using UnityEngine.UI;

public class Encostar_Dedo : MonoBehaviour
{
    private GameObject object1;        // Vai ser o pr�prio objeto onde o script est�
    private GameObject object2;        // Objeto com tag "Indicador"
    private Button onClickButton;      // Bot�o que est� no mesmo objeto

    private float distanceThreshold = 0.05f; // Dist�ncia para acionar
    private float resetThreshold = 0.06f;    // Dist�ncia para resetar

    private bool acionado = false;

    void Awake()
    {
        // Puxa o pr�prio objeto
        object1 = gameObject;

        // Puxa o componente Button do mesmo objeto (se existir)
        onClickButton = GetComponent<Button>();

        if (onClickButton == null)
        {
            Debug.LogWarning("Nenhum Button encontrado neste objeto. Adicione um componente Button se quiser usar o clique.");
        }

        // Procura o objeto com a tag "Indicador"
        object2 = GameObject.FindGameObjectWithTag("Indicador");

        if (object2 == null)
        {
            Debug.LogError("Nenhum objeto com a tag 'Indicador' foi encontrado na cena!");
        }
    }

    void Update()
    {
        if (object2 == null || onClickButton == null) return;

        float distance = Vector3.Distance(object1.transform.position, object2.transform.position);
        Debug.Log(distance);

        if (!acionado && distance < distanceThreshold)
        {
            Debug.Log("Acionou!");
            onClickButton.onClick.Invoke();
            acionado = true; // trava
        }
        else if (acionado && distance > resetThreshold)
        {
            Debug.Log("Resetou, pronto para acionar de novo.");
            acionado = false; // libera para pr�ximo acionamento
        }
    }
}
