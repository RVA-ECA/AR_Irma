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
    public GameObject miniTela;            // Mini tela ao lado, oculta inicialmente
    public TMP_Text dataRecebimentoText;   // TextField para exibir DATA_RECEBIMENTO
    public TMP_Text dataSaidaText;         // TextField para exibir DATA_SAIDA

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
            Image statusIcon = newItem.transform.Find("icon_status").GetComponent<Image>();
            Button button = newItem.transform.Find("icon_plusInfo").GetComponent<Button>(); // Botão "icon_plusInfo"

            // Preenche textos
            if (rmaText) rmaText.text = entry.RMA;
            if (clienteText) clienteText.text = entry.CLIENTE;

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

            // Configura o evento do botão para exibir a mini tela ao clicar
            if (button != null)
            {
                button.onClick.RemoveAllListeners(); // Remove listeners anteriores
                button.onClick.AddListener(() => OnButtonClick(entry)); // Adiciona o listener para o botão
            }
        }
    }

    // Método chamado quando o botão da célula é clicado
    public void OnButtonClick(HistoricoEntry entry)
    {
        // Alterna a mini tela entre aparecer e desaparecer
        if (miniTela.activeSelf)
        {
            miniTela.SetActive(false); // Se já está visível, oculta
        }
        else
        {
            miniTela.SetActive(true);  // Se está oculta, exibe

            // Atualiza os campos de texto com as datas da célula
            dataRecebimentoText.text = "Recebimento: \n" + (string.IsNullOrEmpty(entry.DATA_RECEBIMENTO) ? "-" : entry.DATA_RECEBIMENTO);
            dataSaidaText.text = "Saída: \n" + (string.IsNullOrEmpty(entry.DATA_SAIDA) ? "-" : entry.DATA_SAIDA);
        }
    }
}
