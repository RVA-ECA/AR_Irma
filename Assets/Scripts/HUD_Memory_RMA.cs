using UnityEngine;
using TMPro;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System.Linq;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class Info_Memorys : MonoBehaviour
{
    public TMP_Text Memory_Type;
    public TMP_Text Serial;
    public TMP_Text Warranty;
    public TMP_Text Comment;
    public TMP_Text History;
    public TextAsset csvTextAsset;
    public string serialToFilter;

    [Header("Exportação CSV")]
    public TextAsset ArquivoDestino;
    public InputActionProperty SalvarMemoria;
    public InputActionProperty ExclurMemoria;
    public TMP_Text HistEntradas;

    [Serializable]
    public class Item
    {
        public int memoryType;    // Coluna 0
        public string serial;     // Coluna 1
        public int warranty;      // Coluna 2
        public string comentario; // Coluna 3
        public string historico;  // Coluna 4
    }

    private readonly List<Item> items = new List<Item>();

    void Start()
    {
        if (csvTextAsset == null)
        {
            Debug.LogError("Arquivo CSV de entrada não foi atribuído.");
            return;
        }

        var rows = ParseCsvRows(csvTextAsset.text);
        LoadItems(rows);

        var encontrado = FindBySerial(serialToFilter);
        if (encontrado != null)
        {
            Memory_Type.text = encontrado.memoryType == 0 ? "Memória RAM" :
                               encontrado.memoryType == 1 ? "SSD NVME" :
                               $"Tipo {encontrado.memoryType}";

            Serial.text = encontrado.serial;
            Warranty.text = encontrado.warranty == 0 ? "Fora da garantia" :
                            encontrado.warranty == 1 ? "Garantia ativa" :
                            encontrado.warranty == 2 ? "Garantia Vencida" :
                            encontrado.warranty.ToString();

            Comment.text = encontrado.comentario;
            History.text = encontrado.historico;
        }
        else
        {
            Comment.text = "Serial não encontrado.";
        }

        if (SalvarMemoria.action != null)
        {
            SalvarMemoria.action.performed += OnSalvarPerformed;
            SalvarMemoria.action.Enable();
        }

        if (ExclurMemoria.action != null)
        {
            ExclurMemoria.action.performed += OnExcluirPerformed;
            ExclurMemoria.action.Enable();
        }
    }

    private void OnSalvarPerformed(InputAction.CallbackContext ctx) => ExportarCSV();
    private void OnExcluirPerformed(InputAction.CallbackContext ctx) => LimparMemoria();

    private Item FindBySerial(string s)
    {
        if (string.IsNullOrEmpty(s)) return null;
        string key = s.Trim();
        for (int i = 0; i < items.Count; i++)
        {
            var it = items[i];
            if (!string.IsNullOrEmpty(it.serial) &&
                string.Equals(it.serial.Trim(), key, StringComparison.OrdinalIgnoreCase))
                return it;
        }
        return null;
    }

    private void LoadItems(List<string[]> rows)
    {
        items.Clear();
        for (int i = 1; i < rows.Count; i++) // pula cabeçalho
        {
            var campos = rows[i];
            if (campos.Length < 5) continue;

            int tipo = -1; int.TryParse(N(campos, 0), out tipo);
            int gar = 0; int.TryParse(N(campos, 2), out gar);

            items.Add(new Item
            {
                memoryType = tipo,
                serial = N(campos, 1).Trim(),
                warranty = gar,
                comentario = N(campos, 3).Trim(),
                historico = N(campos, 4).Trim()
            });
        }

        string N(string[] a, int idx) => idx < a.Length ? (a[idx] ?? "") : "";
    }

    // Parser de CSV que respeita aspas, quebras de linha e aspas escapadas ("")
    private static List<string[]> ParseCsvRows(string text)
    {
        var rows = new List<string[]>();
        var row = new List<string>();
        var field = new StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];

            if (c == '\"')
            {
                if (inQuotes && i + 1 < text.Length && text[i + 1] == '\"')
                {
                    field.Append('\"'); // aspas escapada
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes; // abre/fecha aspas
                }
            }
            else if (c == ',' && !inQuotes)
            {
                row.Add(field.ToString());
                field.Length = 0;
            }
            else if ((c == '\n' || c == '\r') && !inQuotes)
            {
                // fim de linha (ignora \r\n duplo)
                if (c == '\r' && i + 1 < text.Length && text[i + 1] == '\n') i++;
                row.Add(field.ToString());
                field.Length = 0;
                if (row.Count > 0) rows.Add(row.ToArray());
                row.Clear();
            }
            else
            {
                field.Append(c);
            }
        }

        // último campo/linha
        row.Add(field.ToString());
        if (row.Count > 0) rows.Add(row.ToArray());

        return rows;
    }

    private static string CsvEscape(string s)
    {
        if (s == null) return "";
        bool precisaAspas = s.Contains(",") || s.Contains("\n") || s.Contains("\r") || s.Contains("\"");
        if (s.Contains("\"")) s = s.Replace("\"", "\"\"");
        return precisaAspas ? $"\"{s}\"" : s;
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
        string caminhoFinal = Path.Combine(Application.dataPath, assetPath.Substring("Assets/".Length));

        var item = FindBySerial(serialToFilter);
        if (item == null)
        {
            Debug.LogWarning("Serial para exportação não encontrado na memória.");
            return;
        }

        List<string[]> rows;
        if (File.Exists(caminhoFinal))
        {
            string conteudo = File.ReadAllText(caminhoFinal, Encoding.UTF8);
            rows = ParseCsvRows(conteudo);
            if (rows.Count == 0)
                rows.Add(new[] { "numero", "serial", "numero", "texto", "texto" });
        }
        else
        {
            rows = new List<string[]>();
            rows.Add(new[] { "numero", "serial", "numero", "texto", "texto" });
        }

        bool achou = false;
        string serialKey = (serialToFilter ?? "").Trim();
        for (int i = 1; i < rows.Count; i++)
        {
            string colSerial = (rows[i].Length > 1 ? rows[i][1] : "").Trim();
            if (string.Equals(colSerial, serialKey, StringComparison.OrdinalIgnoreCase))
            {
                rows[i] = new[]
                {
                item.memoryType.ToString(),
                item.serial,
                item.warranty.ToString(),
                item.comentario,
                item.historico
            };
                achou = true;
                break;
            }
        }

        if (!achou)
        {
            rows.Add(new[]
            {
            item.memoryType.ToString(),
            item.serial,
            item.warranty.ToString(),
            item.comentario,
            item.historico
        });
        }

        // Remove linhas totalmente vazias
        rows.RemoveAll(r =>
            r == null || r.All(c => string.IsNullOrWhiteSpace(c))
        );

        // Salva no arquivo
        var linhas = rows.Select(r =>
            string.Join(",", r.Select(c =>
                c.Contains(",") || c.Contains("\n") ? $"\"{c.Replace("\"", "\"\"")}\"" : c
            ))
        );

        File.WriteAllText(caminhoFinal, string.Join("\n", linhas), Encoding.UTF8);

        Debug.Log("Exportação concluída.");

        // Chama seleção aleatória (opcional)
        SelecionarMemoriaAleatoria();
#endif
    }

    public void SelecionarMemoriaAleatoria()
    {
        if (items.Count == 0)
        {
            Debug.LogWarning("Nenhum item disponível para seleção aleatória.");
            return;
        }

        // Escolhe índice aleatório
        int index = UnityEngine.Random.Range(0, items.Count);
        var mem = items[index];

        // Preenche campos de texto
        Memory_Type.text = mem.memoryType == 0 ? "Memória RAM" :
                           mem.memoryType == 1 ? "SSD NVME" :
                           $"Tipo {mem.memoryType}";

        Serial.text = mem.serial;
        Warranty.text = mem.warranty == 0 ? "Fora da garantia" :
                        mem.warranty == 1 ? "Garantia ativa" :
                        mem.warranty == 2 ? "Garantia Vencida" :
                        mem.warranty.ToString();

        Comment.text = mem.comentario;
        History.text = mem.historico;

        // Atualiza serial atual para exportação/edição se necessário
        serialToFilter = mem.serial;

        #if UNITY_EDITOR
        if (ArquivoDestino != null)
        {
            try
            {
                string assetPath = AssetDatabase.GetAssetPath(ArquivoDestino);
                string caminhoFinal = Path.Combine(Application.dataPath, assetPath.Replace("Assets/", ""));

                if (File.Exists(caminhoFinal))
                {
                    string conteudo = File.ReadAllText(caminhoFinal, Encoding.UTF8);
                    var rows = ParseCsvRows(conteudo);

                    StringBuilder sb = new StringBuilder();
                    for (int i = 1; i < rows.Count; i++) // pula cabeçalho
                    {
                        if (rows[i].Length > 1 && !string.IsNullOrWhiteSpace(rows[i][1]))
                            sb.AppendLine(rows[i][1]);
                    }

                    HistEntradas.text = sb.ToString().TrimEnd();
                }
                else
                {
                    HistEntradas.text = "Arquivo de destino não encontrado.";
                }
            }
            catch (Exception ex)
            {
                HistEntradas.text = $"Erro ao ler arquivo: {ex.Message}";
            }
        }
        else
        {
            HistEntradas.text = "Arquivo de destino não atribuído.";
        }
#endif

        Debug.Log($"Memória aleatória selecionada: {mem.serial}");
    }

    public void LimparMemoria()
    {
        items.Clear();
        Comment.text = "Memória limpa.";
        Debug.Log("Todos os dados da memória RAM foram apagados.");
    }

    void OnDestroy()
    {
        if (SalvarMemoria.action != null) SalvarMemoria.action.performed -= OnSalvarPerformed;
        if (ExclurMemoria.action != null) ExclurMemoria.action.performed -= OnExcluirPerformed;
    }
}
