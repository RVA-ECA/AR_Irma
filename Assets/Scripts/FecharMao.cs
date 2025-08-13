using System.Collections.Generic;
using UnityEngine;

public class FecharMao : MonoBehaviour
{
    public GameObject MenuIncial;
    public Transform cameraRef; // Referência ao CenterEyeAnchor do OVRCameraRig
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
            Vector3 novaPosicao = cameraRef.position + cameraRef.forward * distancia;
            MenuIncial.transform.position = novaPosicao;

            // Rotaciona para olhar para a câmera
            MenuIncial.transform.rotation = Quaternion.LookRotation(MenuIncial.transform.position - cameraRef.position);
        }
    }
}
