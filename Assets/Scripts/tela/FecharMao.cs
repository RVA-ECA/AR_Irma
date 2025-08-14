using System.Collections.Generic;
using UnityEngine;

public class FecharMao : MonoBehaviour
{
    public GameObject MenuIncial;
    public Transform cameraRef;  // Refer�ncia ao CenterEyeAnchor do OVRCameraRig
    public float distancia = 2f;

    private bool MenuAtivado = true;
    private bool menuTravado = false;

    private HashSet<string> dedosEsperados = new HashSet<string> { "Indicador", "Medio", "Anelar", "Mindinho" };
    private HashSet<string> dedosDetectados = new HashSet<string>();

    private void OnTriggerEnter(Collider other)
    {
        if (dedosEsperados.Contains(other.tag))
        {
            dedosDetectados.Add(other.tag);

            if (dedosDetectados.Count == dedosEsperados.Count && !menuTravado)
            {
                MenuAtivado = !MenuAtivado;

                if (MenuAtivado)
                {
                    AtualizarPosicaoMenu();
                    MenuIncial.SetActive(true);
                }
                else
                {
                    MenuIncial.SetActive(false);
                }

                menuTravado = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (dedosEsperados.Contains(other.tag))
        {
            dedosDetectados.Remove(other.tag);
            menuTravado = false;
        }
    }

    private void AtualizarPosicaoMenu()
    {
        if (cameraRef != null)
        {
            // Calcula a nova posi��o do menu � frente da c�mera, mantendo a dist�ncia
            Vector3 novaPosicao = cameraRef.position + cameraRef.forward * distancia;
            MenuIncial.transform.position = novaPosicao;

            // Ajusta a rota��o da tela para que ela sempre olhe para a c�mera
            // Aqui, a rota��o � ajustada para garantir que a tela sempre fique virada para o usu�rio
            Vector3 dir = cameraRef.position - MenuIncial.transform.position;
            dir.y = 0;  // Para que a tela fique alinhada horizontalmente, sem inclina��o para cima ou para baixo
            MenuIncial.transform.rotation = Quaternion.LookRotation(dir);
        }
    }
}
