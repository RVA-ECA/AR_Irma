using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

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
        if (jsonFile == null || historicoItemPrefab == null)
        {
            Debug.LogError("JSON ou prefab não atribuído!");
            return;
        }

        // Wrap do JSON para JsonUtility
        string wrappedJson = "{ \"historico\": " + jsonFile.text + "}";
        HistoricoList historicoList = JsonUtility.FromJson<HistoricoList>(wrappedJson);

        DisplayHistorico(historicoList);
    }

    public void DisplayHistorico(HistoricoList historicoLista)
    {
        foreach (Transform child in contentContainer)
            Destroy(child.gameObject);

        foreach (HistoricoEntry entry in historicoLista.historico)
        {
            GameObject newItem = Instantiate(historicoItemPrefab, contentContainer);

            TMP_Text rmaText = newItem.transform.Find("RMA_text")?.GetComponent<TMP_Text>();
            TMP_Text clienteText = newItem.transform.Find("Cliente_text")?.GetComponent<TMP_Text>();
            Image statusIcon = newItem.transform.Find("icon_status")?.GetComponent<Image>();
            Button button = newItem.transform.Find("icon_plusInfo")?.GetComponent<Button>();

            if (rmaText != null) rmaText.text = entry.RMA;
            if (clienteText != null) clienteText.text = entry.CLIENTE;

            switch (entry.STATUS)
            {
                case "Concluido": statusIcon.sprite = concluidoIcon; break;
                case "Nao recebido": statusIcon.sprite = naoRecebidoIcon; break;
                case "Observação": statusIcon.sprite = observacaoIcon; break;
                default: statusIcon.sprite = null; break;
            }

            // Configura o botão para enviar o evento de clique
            if (button != null)
            {
                HistoricoButton historicoButton = button.gameObject.AddComponent<HistoricoButton>();
                historicoButton.entry = entry;
                historicoButton.onClicked += HandleButtonClicked;
            }
        }
    }

    // Esse método é chamado quando qualquer botão é clicado
    private void HandleButtonClicked(HistoricoEntry entry)
    {
        if (miniTela.activeSelf && dataRecebimentoText.text.Contains(entry.DATA_RECEBIMENTO))
        {
            // Se o mesmo botão for clicado, esconde a tela
            miniTela.SetActive(false);
        }
        else
        {
            // Mostra mini tela com os dados da célula
            miniTela.SetActive(true);
            dataRecebimentoText.text = "Recebimento: \n" + (string.IsNullOrEmpty(entry.DATA_RECEBIMENTO) ? "-" : entry.DATA_RECEBIMENTO);
            dataSaidaText.text = "Saída: \n" + (string.IsNullOrEmpty(entry.DATA_SAIDA) ? "-" : entry.DATA_SAIDA);
        }
    }
}
