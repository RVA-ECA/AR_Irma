using UnityEngine;

public class Esfera : MonoBehaviour
{
    public GameObject MenuTela;  // O Canvas a ser ativado/desativado

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Indicador"))  // Quando o "Indicador" tocar na esfera
        {
            // Alterna a visibilidade do Menu
            MenuTela.SetActive(!MenuTela.activeSelf);
        }
    }
}
