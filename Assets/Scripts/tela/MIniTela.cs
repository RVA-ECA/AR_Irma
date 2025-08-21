using System.Collections.Generic;
using UnityEngine;

public class Minitela : MonoBehaviour
{
    public GameObject Tela;  // A tela que ser� ativada/desativada

    private HashSet<string> dedosEsperados = new HashSet<string> { "Indicador", "Medio", "Anelar", "Mindinho" };
    private HashSet<string> dedosDetectados = new HashSet<string>();  // Dedos que est�o em contato com o objeto

    private bool menuAtivado = false;  // Controle do estado do menu

    void Update()
    {
        // Verifica se todos os dedos esperados est�o tocando o objeto
        if (dedosDetectados.Count == dedosEsperados.Count)
        {
            // Se todos os dedos est�o em contato, ativa a tela
            if (!menuAtivado)
            {
                menuAtivado = true;
                Tela.SetActive(true);
            }
        }
        else
        {
            // Se algum dedo n�o est� em contato, desativa a tela
            if (menuAtivado)
            {
                menuAtivado = false;
                Tela.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Verifica se o dedo que entrou � um dos esperados
        if (dedosEsperados.Contains(other.tag))
        {
            dedosDetectados.Add(other.tag);  // Marca o dedo como detectado
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Verifica se o dedo que saiu � um dos esperados
        if (dedosEsperados.Contains(other.tag))
        {
            dedosDetectados.Remove(other.tag);  // Remove o dedo da lista de detectados
        }
    }
}
