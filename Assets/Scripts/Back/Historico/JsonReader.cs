using UnityEngine;

[System.Serializable]
public class HistoricoEntry
{
    public string RMA;
    public string CLIENTE;
    public string STATUS;
    public string DATA_RECEBIMENTO; 
    public string DATA_SAIDA;
}

[System.Serializable]
public class HistoricoList
{
    public HistoricoEntry[] historico;
}

public class JsonReader : MonoBehaviour
{
    // Apenas mude o nome da variável. Isso forçará o Unity a reimportar o script.
    public TextAsset historicoJsonFile;
    public HistoricoUIController uiController;

    void Start()
    {
        if (historicoJsonFile == null)
        {
            Debug.LogError("O arquivo JSON de histórico não foi atribuído no Inspector!");
            return;
        }
        
        if (uiController == null)
        {
            Debug.LogError("O UI Controller não foi devidamente atribuído ao Inspector!");
            return;
        } 

        string jsonContent = historicoJsonFile.text;
        string jsonWrapper = "{\"historico\":" + jsonContent + "}";

        HistoricoList historicoLista = JsonUtility.FromJson<HistoricoList>(jsonWrapper);
        Debug.Log("Total de registros lidos: " + historicoLista.historico.Length);

        uiController.DisplayHistorico(historicoLista);
    }
}
