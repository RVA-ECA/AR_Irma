using System;
using System.IO;
using TMPro;
using TMPro;
using UnityEngine;
using UnityEngine;
using UnityEngine.UI;
using static HistoricoUIController;

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
    public GameObject RMAITittlePrefab;
    public Transform TittleRMA;

    [Header("Telas")]
    public GameObject TelaHistorico;
    public GameObject TelaRMA;

    [Header("Botões")]
    public Button ReturnButton;

    [Header("Ícones Historico")]
    public Sprite concluidoIcon;
    public Sprite naoRecebidoIcon;
    public Sprite observacaoIcon;

    [Header("Ícones RMA")]
    public Sprite AtivadoIcon;
    public Sprite desativadoIcon;

    public Sprite GarantiaAtivaIcon;
    public Sprite GarantiaInativaIcon;

    public Sprite ComentarioAtivoIcon;
    public Sprite ComentarioInativoIcon;

    private string pastaHistorico;
    private string caminhoHistorico;

    [System.Serializable]
    public class Peca
    {
        public int SERIAL;
        public bool RECEBIDA;
        public bool DIVERGENCIA;
        public bool GARANTIA;
    }

    [System.Serializable]
    public class HistoricoEntry
    {
        public string RMA;
        public string CLIENTE;
        public string STATUS;
        public string DATA_RECEBIMENTO;
        public string DATA_SAIDA;
        public string CATEGORIA;
        public string TIPO;
        public Peca[] PECAS;
    }

    [System.Serializable]
    public class HistoricoList
    {
        public HistoricoEntry[] historico;
    }

    void Start()
    {
        if (jsonFile == null || historicoItemPrefab == null || RMAItemPrefab == null)
        {
            Debug.LogError("JSON ou prefab não atribuído!");
            return;
        }

        string wrappedJson = "{ \"historico\": " + jsonFile.text + "}";
        HistoricoList historicoList = JsonUtility.FromJson<HistoricoList>(wrappedJson);

        if (historicoList != null && historicoList.historico != null)
        {
            DisplayHistorico(historicoList);
        }
        else
        {
            Debug.LogError("Falha ao carregar o JSON ou lista de histórico vazia.");
        }

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
        // Limpa itens antigos, exceto o título
        foreach (Transform child in contentContainerRMA)
        {
            if (child.name != "RMA_Titulo")
                Destroy(child.gameObject);
        }

        // Atualiza título
        ExibirTituloRMA(entry);

        // Cria itens das peças
        foreach (var peca in entry.PECAS)
        {
            GameObject newPecaItem = Instantiate(RMAItemPrefab, contentContainerRMA);

            // Busca os elementos UI de forma segura
            TMP_Text serialText = newPecaItem.transform.Find("rmaSerial_text")?.GetComponent<TMP_Text>();
            Image toggleReceive = newPecaItem.transform.Find("icon_toggleReceive")?.GetComponent<Image>();
            Image toggleDivergence = newPecaItem.transform.Find("icon_toggleDivergence")?.GetComponent<Image>();
            Image toggleGarantia = newPecaItem.transform.Find("icon_rmaWarranty")?.GetComponent<Image>();
            Image comentarioImage = newPecaItem.transform.Find("icon_rmaComents")?.GetComponent<Image>();

            // Atualiza textos
            if (serialText != null)
                serialText.text = $"Serial: {peca.SERIAL}";

            // Configura ícones com base nos valores
            ConfigurarIcone(toggleReceive, peca.RECEBIDA, AtivadoIcon, desativadoIcon);
            ConfigurarIcone(toggleDivergence, peca.DIVERGENCIA, AtivadoIcon, desativadoIcon);
            ConfigurarIcone(toggleGarantia, peca.GARANTIA, GarantiaAtivaIcon, GarantiaInativaIcon);
            ConfigurarIcone(comentarioImage, peca.DIVERGENCIA, ComentarioAtivoIcon, ComentarioInativoIcon);

            // Adiciona listeners para botões interativos
            Button receiveButton = toggleReceive?.GetComponent<Button>();
            if (receiveButton != null)
            {
                receiveButton.onClick.AddListener(() =>
                {
                    peca.RECEBIDA = !peca.RECEBIDA; // Atualiza estado
                    ConfigurarIcone(toggleReceive, peca.RECEBIDA, AtivadoIcon, desativadoIcon);
                    AtualizarPeca(entry, peca.SERIAL, recebida: peca.RECEBIDA);
                });
            }

            Button divergenceButton = toggleDivergence?.GetComponent<Button>();
            if (divergenceButton != null)
            {
                divergenceButton.onClick.AddListener(() =>
                {
                    peca.DIVERGENCIA = !peca.DIVERGENCIA; // Atualiza estado
                    ConfigurarIcone(toggleDivergence, peca.DIVERGENCIA, AtivadoIcon, desativadoIcon);
                    ConfigurarIcone(comentarioImage, peca.DIVERGENCIA, ComentarioAtivoIcon, ComentarioInativoIcon);
                    AtualizarPeca(entry, peca.SERIAL, divergencia: peca.DIVERGENCIA);
                });
            }
        }

        // Troca telas
        TrocarTelas(TelaRMA, TelaHistorico);
    }

    void InicializarJSON()
    {
        pastaHistorico = Path.Combine(Application.persistentDataPath, "HistoricoData");
        caminhoHistorico = Path.Combine(pastaHistorico, "historico.json");

        // Cria a pasta caso não exista
        if (!Directory.Exists(pastaHistorico))
            Directory.CreateDirectory(pastaHistorico);

        // Se o arquivo já existe, apenas loga
        if (File.Exists(caminhoHistorico))
        {
            Debug.Log("Arquivo historico.json já existe em: " + caminhoHistorico);
        }
        else
        {
            // Salva o conteúdo do TextAsset na pasta
            File.WriteAllText(caminhoHistorico, jsonFile.text);
            Debug.Log("Arquivo historico.json criado a partir do TextAsset em: " + caminhoHistorico);
        }

        // Atualiza o TextAsset em memória para o conteúdo real
        string jsonAtual = File.ReadAllText(caminhoHistorico);
        jsonFile = new TextAsset(jsonAtual);
    }

    private void ConfigurarIcone(Image icon, bool condition, Sprite ativo, Sprite inativo)
    {
        if (icon != null)
        {
            icon.sprite = condition ? ativo : inativo;
        }
    }

    private void ExibirTituloRMA(HistoricoEntry entry)
    {
        Transform tituloExistente = TittleRMA.Find("RMA_Titulo");
        if (tituloExistente == null)
        {
            GameObject newItem = Instantiate(RMAITittlePrefab, TittleRMA);

            newItem.name = "RMA_Titulo";

            TMP_Text requestText = newItem.transform.Find("request_txt")?.GetComponent<TMP_Text>();
            TMP_Text categoryText = newItem.transform.Find("category_txt")?.GetComponent<TMP_Text>();
            TMP_Text clientText = newItem.transform.Find("client_txt")?.GetComponent<TMP_Text>();
            TMP_Text typeText = newItem.transform.Find("type_txt")?.GetComponent<TMP_Text>();

            if (requestText != null) requestText.text = entry.RMA;
            if (categoryText != null) categoryText.text = entry.CLIENTE;
            if (clientText != null) clientText.text = entry.CATEGORIA;
            if (typeText != null) typeText.text = entry.TIPO;
        }
        else
        {
            TMP_Text requestText = tituloExistente.Find("request_txt")?.GetComponent<TMP_Text>();
            TMP_Text categoryText = tituloExistente.Find("category_txt")?.GetComponent<TMP_Text>();
            TMP_Text clientText = tituloExistente.Find("client_txt")?.GetComponent<TMP_Text>();
            TMP_Text typeText = tituloExistente.Find("type_txt")?.GetComponent<TMP_Text>();

            if (requestText != null) requestText.text = entry.RMA;
            if (categoryText != null) categoryText.text = entry.CLIENTE;
            if (clientText != null) clientText.text = entry.CATEGORIA;
            if (typeText != null) typeText.text = entry.TIPO;
        }
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

    public void OnToggleRecebidaClick(HistoricoEntry entry, int serial)
    {
        // Inverte o valor atual
        Peca peca = Array.Find(entry.PECAS, p => p.SERIAL == serial);
        if (peca != null)
        {
            AtualizarPeca(entry, serial, recebida: !peca.RECEBIDA);
        }
    }

    public void OnToggleDivergenciaClick(HistoricoEntry entry, int serial)
    {
        Peca peca = Array.Find(entry.PECAS, p => p.SERIAL == serial);
        if (peca != null)
        {
            AtualizarPeca(entry, serial, divergencia: !peca.DIVERGENCIA);
        }
    }
    public void AtualizarPeca(HistoricoEntry entry, int serial, bool? recebida = null, bool? divergencia = null, bool? garantia = null)
    {
        Peca peca = Array.Find(entry.PECAS, p => p.SERIAL == serial);
        if (peca == null) return;

        if (recebida.HasValue) peca.RECEBIDA = recebida.Value;
        if (divergencia.HasValue) peca.DIVERGENCIA = divergencia.Value;
        if (garantia.HasValue) peca.GARANTIA = garantia.Value;

        foreach (Transform pecaUI in contentContainerRMA)
        {
            TMP_Text serialText = pecaUI.Find("rmaSerial_text")?.GetComponent<TMP_Text>();
            if (serialText != null && serialText.text.Contains(peca.SERIAL.ToString()))
            {
                Image toggleReceive = pecaUI.Find("icon_toggleReceive")?.GetComponent<Image>();
                Image toggleDivergence = pecaUI.Find("icon_toggleDivergence")?.GetComponent<Image>();
                Image toggleGarantia = pecaUI.Find("icon_rmaWarranty")?.GetComponent<Image>();

                ConfigurarIcone(toggleReceive, peca.RECEBIDA, AtivadoIcon, desativadoIcon);
                ConfigurarIcone(toggleDivergence, peca.DIVERGENCIA, AtivadoIcon, desativadoIcon);
                ConfigurarIcone(toggleGarantia, peca.GARANTIA, GarantiaAtivaIcon, GarantiaInativaIcon);
                break;
            }
        }

        SalvarJSON();
    }
    public void AtualizarArquivoAPartirDoTextAsset()
    {
        if (!Directory.Exists(pastaHistorico))
            Directory.CreateDirectory(pastaHistorico);

        File.WriteAllText(caminhoHistorico, jsonFile.text);
        Debug.Log("Arquivo historico.json atualizado a partir do TextAsset em: " + caminhoHistorico);
    }
    public void SalvarJSON()
    {
        HistoricoList historicoList = new HistoricoList
        {
            historico = JsonUtility.FromJson<HistoricoList>("{\"historico\":" + jsonFile.text + "}").historico
        };
        string json = JsonUtility.ToJson(historicoList, true);

        // Garante que a pasta exista
        if (!Directory.Exists(pastaHistorico))
            Directory.CreateDirectory(pastaHistorico);

        // Salva o JSON atualizado
        File.WriteAllText(caminhoHistorico, json);
        Debug.Log("JSON salvo em: " + caminhoHistorico);

        // Atualiza o TextAsset em memória
        jsonFile = new TextAsset(json);

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }

}