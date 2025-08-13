using UnityEngine;
using TMPro;

public class Tecla : MonoBehaviour
{
    public string caractere; // Letra dessa tecla
    public TMP_InputField campoAtivo; // Campo para enviar a letra

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Indicador") && campoAtivo != null)
        {
            campoAtivo.text += caractere;
        }
    }
}
