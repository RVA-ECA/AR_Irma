using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginEfetuado : MonoBehaviour
{
    [SerializeField] private string nomeCenaDestino;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Indicador"))
        {
            SceneManager.LoadScene(nomeCenaDestino);
        }
    }
}
