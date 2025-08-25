using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class HistoricoUIController : MonoBehaviour
{
    private const string DEFAULT_DATE_TEXT = "..........";

    [Header("JSON")]
    public TextAsset jsonFile;

    [Header("UI")]
    public Transform contentContainerHistorico;
    public GameObject historicoItemPrefab;
    public GameObject miniTela;
    public TMP_Text dataRecebimentoText;
    public TMP_Text dataSaidaText;
    public GameObject RMAItemPrefab;
    public Transform contentContainerRMA;

    [Header("Telas")]
    public GameObject TelaHistorico;
    public GameObject TelaRMA;

    [Header("Botões")]
    public Button ReturnButton;

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

        string wrappedJson = "{ \"historico\": " + jsonFile.text + "}";
        HistoricoList historicoList = JsonUtility.FromJson<HistoricoList>(wrappedJson);

        DisplayHistorico(historicoList);

        if (ReturnButton != null)
        {
            ReturnButton.onClick.AddListener(() => TrocarTelas(TelaHistorico, TelaRMA));
        }
    }

    public void DisplayHistorico(HistoricoList historicoLista)
    {
        foreach (Transform child in contentContainerHistorico)
            Destroy(child.gameObject);

        foreach (HistoricoEntry entry in historicoLista.historico)
        {
            GameObject newItem = Instantiate(historicoItemPrefab, contentContainerHistorico);

            TMP_Text rmaText = newItem.transform.Find("RMA_text")?.GetComponent<TMP_Text>();
            TMP_Text clienteText = newItem.transform.Find("Cliente_text")?.GetComponent<TMP_Text>();
            Image statusIcon = newItem.transform.Find("icon_status")?.GetComponent<Image>();

            Button plusInfoButton = newItem.transform.Find("icon_plusInfo")?.GetComponent<Button>();
            Button rmaButton = newItem.transform.Find("Button_rma")?.GetComponent<Button>();

            if (rmaText != null) rmaText.text = entry.RMA;
            if (clienteText != null) clienteText.text = entry.CLIENTE;

            switch (entry.STATUS)
            {
                case "Concluido": statusIcon.sprite = concluidoIcon; break;
                case "Nao recebido": statusIcon.sprite = naoRecebidoIcon; break;
                case "Observação": statusIcon.sprite = observacaoIcon; break;
                default: statusIcon.sprite = null; break;
            }

            if (plusInfoButton != null)
            {
                plusInfoButton.onClick.AddListener(() => HandleButtonClicked(entry));
            }

            if (rmaButton != null)
            {
                rmaButton.onClick.AddListener(() => ExibirDetalhesNaTela2(entry));
            }
        }
    }

    public void ExibirDetalhesNaTela2(HistoricoEntry entry)
    {
        foreach (Transform child in contentContainerRMA)
            Destroy(child.gameObject);

        GameObject newItem = Instantiate(RMAItemPrefab, contentContainerRMA);

        TMP_Text rmaText = newItem.transform.Find("RMA_tela2_text")?.GetComponent<TMP_Text>();
        TMP_Text clienteText = newItem.transform.Find("Cliente_tela2_text")?.GetComponent<TMP_Text>();
        TMP_Text statusText = newItem.transform.Find("Status_tela2_text")?.GetComponent<TMP_Text>();
        TMP_Text dataRecebimentoTela2Text = newItem.transform.Find("DataRecebimento_tela2_text")?.GetComponent<TMP_Text>();
        TMP_Text dataSaidaTela2Text = newItem.transform.Find("DataSaida_tela2_text")?.GetComponent<TMP_Text>();

        if (rmaText != null) rmaText.text = entry.RMA;
        if (clienteText != null) clienteText.text = entry.CLIENTE;
        if (statusText != null) statusText.text = $"Status: {entry.STATUS}";
        if (dataRecebimentoTela2Text != null) dataRecebimentoTela2Text.text = $"Recebimento: {FormatarData(entry.DATA_RECEBIMENTO)}";
        if (dataSaidaTela2Text != null) dataSaidaTela2Text.text = $"Saída: {FormatarData(entry.DATA_SAIDA)}";

        TrocarTelas(TelaRMA, TelaHistorico);
    }

    public void HandleButtonClicked(HistoricoEntry entry)
    {
        string dataRecebimentoFormatada = FormatarData(entry.DATA_RECEBIMENTO);

        if (miniTela.activeSelf && dataRecebimentoText.text.Contains(dataRecebimentoFormatada))
        {
            miniTela.SetActive(false);
        }
        else
        {
            miniTela.SetActive(true);

            string dataSaidaFormatada = FormatarData(entry.DATA_SAIDA);

            dataRecebimentoText.text = $"Recebimento: {dataRecebimentoFormatada}";
            dataSaidaText.text = $"Saída: {dataSaidaFormatada}";
        }
    }

    public void TrocarTelas(GameObject telaParaAtivar, GameObject telaParaDesativar)
    {
        if (telaParaDesativar != null)
        {
            telaParaDesativar.SetActive(false);
        }
        if (telaParaAtivar != null)
        {
            telaParaAtivar.SetActive(true);
        }
    }

    private string FormatarData(string dataOriginal)
    {
        if (string.IsNullOrEmpty(dataOriginal))
        {
            return DEFAULT_DATE_TEXT;
        }

        return dataOriginal.Replace(":", "/").Replace(";", "/").Replace(" ", "/");
    }
}
