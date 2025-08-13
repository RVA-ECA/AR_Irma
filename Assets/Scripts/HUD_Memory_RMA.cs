using UnityEngine;
using TMPro;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CSVReader : MonoBehaviour
{
    public TMP_Text Serial;          // Texto para exibir
    public TMP_Text Warranty;          // Texto para exibir
    public TMP_Text Comment;          // Texto para exibir
    public TextAsset csvTextAsset;    // CSV de entrada (arrastável)
    public string serialToFilter;     // Serial que deseja filtrar

    [Header("Exportação CSV")]
    public TextAsset ArquivoDestino;  // CSV de saída (arrastável)
    public InputActionProperty SalvarMemoria; // Botão para salvar memória
    public InputActionProperty ExclurMemoria; // Botão para excluir memória

    [System.Serializable]
    public class Item
    {
        public string serial;
        public int warranty;
        public string comentario;
    }

    private List<Item> items = new List<Item>();

    void Start()
    {
        // Carrega CSV de entrada
        if (csvTextAsset != null)
        {
            LerCSV(csvTextAsset.text);

            Item encontrado = items.FirstOrDefault(i => i.serial == serialToFilter);
            if (encontrado != null)
            {
                Serial.text = $"{encontrado.serial}";
                Warranty.text = encontrado.warranty == 0 ? "Fora da garantia" :
                    encontrado.warranty == 1 ? "Garantia ativa" :
                    encontrado.warranty.ToString();
                Comment.text = $"{encontrado.comentario}";
            }
            else
            {
                Comment.text = "Serial não encontrado.";
            }
        }
        else
        {
            Debug.LogError("Arquivo CSV de entrada não foi atribuído.");
        }

        // Atrelar ações aos comandos
        if (SalvarMemoria != null)
            SalvarMemoria.action.performed += ctx => ExportarCSV();

        if (ExclurMemoria != null)
            ExclurMemoria.action.performed += ctx => LimparMemoria();

        // Ativar as ações
        SalvarMemoria.action.Enable();
        ExclurMemoria.action.Enable();
    }

    void LerCSV(string conteudo)
    {
        string[] linhas = conteudo.Split(new[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        for (int i = 1; i < linhas.Length; i++) // pula o cabeçalho
        {
            string linha = linhas[i].Trim();
            if (string.IsNullOrWhiteSpace(linha)) continue;

            string[] campos = SepararCamposCSV(linha);
            if (campos.Length >= 3)
            {
                items.Add(new Item
                {
                    serial = campos[0],
                    warranty = int.TryParse(campos[1], out int num) ? num : 0,
                    comentario = campos[2]
                });
            }
        }
    }

    string[] SepararCamposCSV(string linha)
    {
        List<string> campos = new List<string>();
        bool dentroAspas = false;
        string campoAtual = "";

        foreach (char c in linha)
        {
            if (c == '\"')
            {
                dentroAspas = !dentroAspas;
            }
            else if (c == ',' && !dentroAspas)
            {
                campos.Add(campoAtual);
                campoAtual = "";
            }
            else
            {
                campoAtual += c;
            }
        }
        campos.Add(campoAtual);
        return campos.ToArray();
    }

    public void BuscarAleatorio()
    {
        if (items.Count > 0)
        {
            int randomIndex = Random.Range(0, items.Count);
            Item randomItem = items[randomIndex];
            Comment.text = $"Serial: {randomItem.serial} | Número: {randomItem.warranty} | Comentário: {randomItem.comentario}";
        }
        else
        {
            Comment.text = "Nenhum item encontrado.";
        }
    }
    public void ExportarCSV()
    {
#if UNITY_EDITOR
        if (ArquivoDestino == null)
        {
            Debug.LogError("Arquivo de destino não atribuído.");
            return;
        }

        string assetPath = AssetDatabase.GetAssetPath(ArquivoDestino);
        string caminhoFinal = Path.Combine(Application.dataPath, assetPath.Replace("Assets/", ""));

        var itemFiltrado = items.FirstOrDefault(i => i.serial == serialToFilter);
        if (itemFiltrado == null)
        {
            Debug.LogWarning("Serial para exportação não encontrado na memória.");
            return;
        }

        List<string> linhas = new List<string>();
        bool achouSerial = false;

        if (File.Exists(caminhoFinal))
        {
            linhas = File.ReadAllLines(caminhoFinal).ToList();
            // Verifica e atualiza o serial na lista, pulando cabeçalho na linha 0
            for (int i = 1; i < linhas.Count; i++)
            {
                string[] campos = SepararCamposCSV(linhas[i]);
                if (campos.Length > 0 && campos[0] == serialToFilter)
                {
                    string comentarioFormatado = itemFiltrado.comentario.Contains(",") || itemFiltrado.comentario.Contains("\n")
                        ? $"\"{itemFiltrado.comentario}\""
                        : itemFiltrado.comentario;

                    linhas[i] = $"{itemFiltrado.serial},{itemFiltrado.warranty},{comentarioFormatado}";
                    achouSerial = true;
                    break;
                }
            }
        }
        else
        {
            // Cria cabeçalho se arquivo não existir
            linhas.Add("serial,numero,comentário");
        }

        if (!achouSerial)
        {
            string comentarioFormatado = itemFiltrado.comentario.Contains(",") || itemFiltrado.comentario.Contains("\n")
                ? $"\"{itemFiltrado.comentario}\""
                : itemFiltrado.comentario;

            linhas.Add($"{itemFiltrado.serial},{itemFiltrado.warranty},{comentarioFormatado}");
        }

        File.WriteAllLines(caminhoFinal, linhas, System.Text.Encoding.UTF8);
        Debug.Log($"Dados do serial '{serialToFilter}' exportados para: {caminhoFinal}");
#else
    Debug.LogError("Exportar para dentro de Assets só funciona no Editor. Em builds, use Application.persistentDataPath.");
#endif
    }

    public void LimparMemoria()
    {
        items.Clear();
        Comment.text = "Memória limpa.";
        Debug.Log("Todos os dados da memória RAM foram apagados.");
    }
}
