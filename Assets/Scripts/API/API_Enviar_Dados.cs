using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using TMPro;

public class EnviarAPI : MonoBehaviour
{
    [Header("Referências de Entrada")]
    public RawImage imagemOrigem;       // Arraste o objeto RawImage com a imagem
    public TMP_InputField campoTexto;   // Campo de texto (TextMeshPro)

    [Header("Referência de Saída")]
    public TMP_Text campoResposta;      // Onde exibir a resposta da API

    [Header("Configuração da API")]
    public string urlAPI = "http://localhost:8000/processar";

    [ContextMenu("Enviar para API")]
    public void Enviar()
    {
        StartCoroutine(EnviarDados());
    }

    IEnumerator EnviarDados()
    {
        string imagemBase64 = "";

        // Obtém a textura e converte para base64 se existir
        if (imagemOrigem != null && imagemOrigem.texture != null)
        {
            Texture2D textura = new Texture2D(imagemOrigem.texture.width, imagemOrigem.texture.height, TextureFormat.RGB24, false);
            RenderTexture renderTex = RenderTexture.GetTemporary(imagemOrigem.texture.width, imagemOrigem.texture.height, 0);
            Graphics.Blit(imagemOrigem.texture, renderTex);
            RenderTexture.active = renderTex;
            textura.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
            textura.Apply();
            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(renderTex);

            byte[] bytes = textura.EncodeToPNG();
            imagemBase64 = System.Convert.ToBase64String(bytes);
        }

        // Captura o texto digitado
        string texto = campoTexto != null ? campoTexto.text : "";

        // Monta o JSON
        string json = JsonUtility.ToJson(new DadosEnvio(texto, imagemBase64));

        // Configura o request
        UnityWebRequest req = new UnityWebRequest(urlAPI, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        req.uploadHandler = new UploadHandlerRaw(bodyRaw);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        // Envia
        yield return req.SendWebRequest();

        // Exibe resultado
        if (req.result == UnityWebRequest.Result.Success)
        {
            string resposta = req.downloadHandler.text;
            Debug.Log("Resposta da API: " + resposta);
            if (campoResposta != null) campoResposta.text = resposta;
        }
        else
        {
            Debug.LogError("Erro: " + req.error);
            if (campoResposta != null) campoResposta.text = "Erro: " + req.error;
        }
    }

    [System.Serializable]
    public class DadosEnvio
    {
        public string texto;
        public string imagem_base64;
        public DadosEnvio(string texto, string imagem)
        {
            this.texto = texto;
            this.imagem_base64 = imagem;
        }
    }
}
