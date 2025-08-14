using UnityEngine;
using System.Collections;  // Necess�rio para usar coroutines

public class Palma : MonoBehaviour
{
    public GameObject MenuIncial;
    public Transform cameraRef;
    public float distancia = 2f;

    private bool menuAtivado = false;
    private bool palmaDetectada = false;
    private bool podeDetectarNovaPalma = true;  // Controle para garantir o delay

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Palma") && !palmaDetectada && podeDetectarNovaPalma)
        {
            palmaDetectada = true;
            menuAtivado = !menuAtivado;
            MenuIncial.SetActive(menuAtivado);
            if (menuAtivado) AtualizarPosicaoMenu();

            // Inicia o delay de 2 segundos antes de permitir outra detec��o
            StartCoroutine(DelayAntesDeNovaDeteccao());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Palma"))
            palmaDetectada = false;
    }

    private void AtualizarPosicaoMenu()
    {
        if (cameraRef != null)
        {
            MenuIncial.transform.position = cameraRef.position + cameraRef.forward * distancia;
            Vector3 dir = cameraRef.position - MenuIncial.transform.position;
            dir.y = 0;
            MenuIncial.transform.rotation = Quaternion.LookRotation(dir);
        }
    }

    // Coroutine que aguarda 2 segundos antes de permitir uma nova detec��o
    private IEnumerator DelayAntesDeNovaDeteccao()
    {
        podeDetectarNovaPalma = false;  // Bloqueia a detec��o at� o delay acabar
        yield return new WaitForSeconds(2f);  // Espera 2 segundos
        podeDetectarNovaPalma = true;  // Permite a detec��o novamente
    }
}
