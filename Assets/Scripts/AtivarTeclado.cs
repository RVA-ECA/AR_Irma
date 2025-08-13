using UnityEngine;
using TMPro;

public class AtivarTeclado : MonoBehaviour
{
    public GameObject teclado;
    public TMP_InputField campo;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Indicador"))
        {
            teclado.SetActive(true);

            // Atualiza todos os botões do teclado com o campo correto
            foreach (Tecla tecla in teclado.GetComponentsInChildren<Tecla>())
            {
                tecla.campoAtivo = campo;
            }
        }
    }
}
