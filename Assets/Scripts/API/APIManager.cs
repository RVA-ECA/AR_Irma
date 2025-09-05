using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

// Classes para desserialização do JSON
[System.Serializable]
public class Peca
{
    public int SERIAL;
    public bool RECEBIDA;
    public bool DIVERGENCIA;
    public bool GARANTIA;
    public string COMMENT;
}

[System.Serializable]
public class RMA
{
    public string RMA_Codigo;
    public string CLIENTE;
    public string STATUS;
    public string DATA_RECEBIMENTO;
    public string DATA_SAIDA;
    public Peca[] PECAS;
}

// Helper para desserializar arrays puros
public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        string newJson = "{ \"array\": " + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.array;
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] array;
    }
}

// Script principal
public class APIManager : MonoBehaviour
{
    public string apiUrl = "https://0002e27b1229.ngrok-free.app/api/dados";

    public RMA[] rmas; // Armazena os dados para usar no app

    void Start()
    {
        StartCoroutine(FetchAndProcessRMA());
    }

    IEnumerator FetchAndProcessRMA()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl))
        {
            // Cabeçalho para pular aviso do Ngrok
            request.SetRequestHeader("ngrok-skip-browser-warning", "true");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Erro ao acessar API: " + request.error);
            }
            else
            {
                string json = request.downloadHandler.text;

                // Desserializa o JSON
                rmas = JsonHelper.FromJson<RMA>(json);

                // Aplica regras de atualização de STATUS
                AtualizarStatusRMAs(rmas);

                // Exemplo de uso: printar cada RMA
                foreach (var r in rmas)
                {
                    Debug.Log($"RMA: {r.RMA_Codigo} | STATUS: {r.STATUS}");
                }
            }
        }
    }

    void AtualizarStatusRMAs(RMA[] dados)
    {
        foreach (RMA rma in dados)
        {
            Peca[] pecas = rma.PECAS;

            // Nenhuma peça recebida
            bool nenhumaRecebida = true;
            foreach (var p in pecas)
            {
                if (p.RECEBIDA)
                {
                    nenhumaRecebida = false;
                    break;
                }
            }
            if (nenhumaRecebida)
            {
                rma.STATUS = "Nao recebido";
                continue;
            }

            // Alguma peça precisa de observação
            bool precisaObservacao = false;
            foreach (var p in pecas)
            {
                if (!p.RECEBIDA || p.DIVERGENCIA || !p.GARANTIA)
                {
                    precisaObservacao = true;
                    break;
                }
            }

            if (precisaObservacao)
                rma.STATUS = "Observação";
            else
                rma.STATUS = "Concluido";
        }
    }
}