using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HistoricoUIController : MonoBehaviour
{
    [Header("JSON")]
    public TextAsset jsonFile;

    [Header("UI")]
    public Transform contentContainer;     // Content do ScrollView
    public GameObject historicoItemPrefab; // Prefab Historico_Cell

    [Header("Ícones")]
    public Sprite concluidoIcon;
    public Sprite naoRecebidoIcon;
    public Sprite observacaoIcon;

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

    void Start()
    {
        if (jsonFile == null)
        {
            Debug.LogError("Arquivo JSON não atribuído!");
            return;
        }

        if (historicoItemPrefab == null)
        {
            Debug.LogError("Prefab Historico_Cell não atribuído!");
            return;
        }

        // JsonUtility precisa de um objeto raiz
        string wrappedJson = "{ \"historico\": " + jsonFile.text + "}";
        HistoricoList historicoList = JsonUtility.FromJson<HistoricoList>(wrappedJson);

        DisplayHistorico(historicoList);
    }

    public void DisplayHistorico(HistoricoList historicoLista)
    {
        // Limpa itens anteriores
        foreach (Transform child in contentContainer)
        {
            Destroy(child.gameObject);
        }

        // Cria novos itens
        foreach (HistoricoEntry entry in historicoLista.historico)
        {
            GameObject newItem = Instantiate(historicoItemPrefab, contentContainer);

            // Busca os elementos no prefab
            TMP_Text rmaText = newItem.transform.Find("RMA_text").GetComponent<TMP_Text>();
            TMP_Text clienteText = newItem.transform.Find("Cliente_text").GetComponent<TMP_Text>();
            TMP_Text dataRecebimentoText = newItem.transform.Find("DataRecebimento_text")?.GetComponent<TMP_Text>();
            TMP_Text dataSaidaText = newItem.transform.Find("DataSaida_text")?.GetComponent<TMP_Text>();
            Image statusIcon = newItem.transform.Find("icon_status").GetComponent<Image>();

            // Preenche textos
            if (rmaText) rmaText.text = entry.RMA;
            if (clienteText) clienteText.text = entry.CLIENTE;
            if (dataRecebimentoText) dataRecebimentoText.text = "Recebimento: " + (string.IsNullOrEmpty(entry.DATA_RECEBIMENTO) ? "-" : entry.DATA_RECEBIMENTO);
            if (dataSaidaText) dataSaidaText.text = "Saída: " + (string.IsNullOrEmpty(entry.DATA_SAIDA) ? "-" : entry.DATA_SAIDA);

            // Ícone de status
            switch (entry.STATUS)
            {
                case "Concluido":
                    statusIcon.sprite = concluidoIcon;
                    break;

                case "Nao recebido":
                    statusIcon.sprite = naoRecebidoIcon;
                    break;

                case "Observação":
                    statusIcon.sprite = observacaoIcon;
                    break;

                default:
                    statusIcon.sprite = null;
                    break;
            }
        }
    }
}
