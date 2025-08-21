using UnityEngine;
using System.Collections;  // Necessário para usar coroutines

public class Palma : MonoBehaviour
{
    public GameObject MenuIncial;  // Referência para o menu
    public GameObject EsferaObjeto1;  // Referência para a esfera 1
    public GameObject EsferaObjeto2;  // Referência para a esfera 2
    public Transform cameraRef;
    public float distancia = 2f;

    private bool menuAtivado = false;
    private bool palmaDetectada = false;
    private bool podeDetectarNovaPalma = true;  // Controle para garantir o delay

    private bool esferaAtivada1 = false;
    private bool esferaAtivada2 = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Palma") && !palmaDetectada && podeDetectarNovaPalma)
        {
            palmaDetectada = true;

            // Alterna o estado do menu (ativo ou inativo)
            menuAtivado = !menuAtivado;
            MenuIncial.SetActive(menuAtivado);

            // Se o menu foi ativado, atualiza a posição do menu
            if (menuAtivado) AtualizarPosicaoMenu();

            // Alterna o estado da esfera 1 (ativa ou inativa)
            esferaAtivada1 = !esferaAtivada1;
            EsferaObjeto1.SetActive(esferaAtivada1);

            // Se a esfera 1 foi ativada, posiciona ela na frente do jogador
            if (esferaAtivada1) AtualizarPosicaoEsfera1();

            // Alterna o estado da esfera 2 (ativa ou inativa)
            esferaAtivada2 = !esferaAtivada2;
            EsferaObjeto2.SetActive(esferaAtivada2);

            // Se a esfera 2 foi ativada, posiciona ela na frente do jogador
            if (esferaAtivada2) AtualizarPosicaoEsfera2();

            // Inicia o delay de 2 segundos antes de permitir outra detecção
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

    private void AtualizarPosicaoEsfera1()
    {
        if (cameraRef != null)
        {
            // Defina as coordenadas fixas para a esfera 1
            Vector3 posicaoDesejada1 = new Vector3(-0.3625f, -0.306f, -0.135f);

            // Calcula a posição final, levando em conta a posição da câmera e o deslocamento
            EsferaObjeto1.transform.position = cameraRef.position + cameraRef.forward * distancia + posicaoDesejada1;

            Vector3 dir = cameraRef.position - EsferaObjeto1.transform.position;
            dir.y = 0;
            EsferaObjeto1.transform.rotation = Quaternion.LookRotation(dir);
        }
    }

    private void AtualizarPosicaoEsfera2()
    {
        if (cameraRef != null)
        {
            // Defina as coordenadas fixas para a esfera 2
            Vector3 posicaoDesejada2 = new Vector3(-0.76f, -0.306f, 0.2f);

            // Calcula a posição final, levando em conta a posição da câmera e o deslocamento
            EsferaObjeto2.transform.position = cameraRef.position + cameraRef.forward * distancia + posicaoDesejada2;

            Vector3 dir = cameraRef.position - EsferaObjeto2.transform.position;
            dir.y = 0;
            EsferaObjeto2.transform.rotation = Quaternion.LookRotation(dir);
        }
    }

    // Coroutine que aguarda 2 segundos antes de permitir uma nova detecção de palma
    private IEnumerator DelayAntesDeNovaDeteccao()
    {
        podeDetectarNovaPalma = false;  // Bloqueia a detecção até o delay acabar
        yield return new WaitForSeconds(2f);  // Espera 2 segundos
        podeDetectarNovaPalma = true;  // Permite a detecção novamente
    }
}
