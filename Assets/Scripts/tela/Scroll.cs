using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class EncostarListaScrollPinça : MonoBehaviour
{
    [Header("Configurações")]
    public string indicadorTag = "Indicador";
    public string polegarTag = "Polegar";      // Novo: objeto do polegar
    public float pinchThreshold = 0.03f;       // Distância máxima para considerar pinça
    public float clickThreshold = 0.01f;       // Distância mínima para clique
    public float resetThreshold = 0.02f;       // Distância para resetar clique
    public float scrollSensibilidade = 3f;     // Velocidade de scroll

    private GameObject indicador;
    private GameObject polegar;
    private ScrollRect scrollRect;
    private List<Button> botoes = new List<Button>();

    private bool clicando = false;
    private Button botaoAtual = null;

    private Vector3 posicaoAnteriorDedo;

    void Awake()
    {
        indicador = GameObject.FindGameObjectWithTag(indicadorTag);
        polegar = GameObject.FindGameObjectWithTag(polegarTag);

        if (indicador == null || polegar == null)
        {
            Debug.LogError("Indicador ou Polegar não encontrados! Configure as tags corretamente.");
            return;
        }

        scrollRect = GetComponent<ScrollRect>();
        if (scrollRect == null)
        {
            Debug.LogError("Nenhum ScrollRect encontrado! Adicione ao objeto.");
        }

        botoes.AddRange(GetComponentsInChildren<Button>());
        posicaoAnteriorDedo = indicador.transform.position;
    }

    void Update()
    {
        if (indicador == null || polegar == null) return;

        Vector3 posicaoAtual = indicador.transform.position;

        // 🔹 Detecta se está em pinça (indicador e polegar próximos)
        float pinchDist = Vector3.Distance(indicador.transform.position, polegar.transform.position);
        bool emPinça = pinchDist < pinchThreshold;

        if (emPinça)
        {
            // 👉 Faz scroll
            float deltaY = posicaoAtual.y - posicaoAnteriorDedo.y;
            if (Mathf.Abs(deltaY) > 0.001f && scrollRect != null)
            {
                scrollRect.verticalNormalizedPosition += deltaY * scrollSensibilidade * Time.deltaTime;
            }
        }
        else
        {
            // 👉 Faz clique normal
            Button maisProximo = null;
            float menorDistancia = Mathf.Infinity;

            foreach (Button b in botoes)
            {
                float d = Vector3.Distance(b.transform.position, posicaoAtual);
                if (d < menorDistancia)
                {
                    menorDistancia = d;
                    maisProximo = b;
                }
            }

            if (maisProximo != null)
            {
                if (!clicando && menorDistancia < clickThreshold)
                {
                    Debug.Log("Clicou no botão: " + maisProximo.gameObject.name);
                    maisProximo.onClick.Invoke();
                    botaoAtual = maisProximo;
                    clicando = true;
                }
                else if (clicando && menorDistancia > resetThreshold)
                {
                    clicando = false;
                    botaoAtual = null;
                }
            }
        }

        posicaoAnteriorDedo = posicaoAtual;
    }
}
