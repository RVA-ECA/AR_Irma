using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class HistoricoUIController : MonoBehaviour
{
    private const string DEFAULT_DATE_TEXT = "00/00/0000";

    [Header("JSON")]
    public TextAsset jsonFile;

    [Header("UI")]
    public Transform contentContainer;
    public GameObject historicoItemPrefab;
    public GameObject miniTela;
    public TMP_Text dataRecebimentoText;
    public TMP_Text dataSaidaText;

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

            if (button != null)
            {
                HistoricoButton historicoButton = button.gameObject.AddComponent<HistoricoButton>();
                historicoButton.entry = entry;
                historicoButton.onClicked += HandleButtonClicked;
            }
        }
    }

    private void HandleButtonClicked(HistoricoEntry entry)
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

            dataRecebimentoText.text = $"recebimento\n({dataRecebimentoFormatada})";
            dataSaidaText.text = $"saída\n({dataSaidaFormatada})";
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