using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HistoricoUIController : MonoBehaviour
{
    public Transform contentContainer;
    public GameObject historicoItemPrefab;

    // Os ícones continuam aqui para serem acessados pelo outro script
    public Sprite concluidoIcon; 
    public Sprite naoRecebidoIcon;
    public Sprite observacaoIcon; 

    public void DisplayHistorico(HistoricoList historicoLista)
    {
        foreach (Transform child in contentContainer)
        {
            Destroy(child.gameObject);
        }

        if (historicoItemPrefab == null)
        {
            Debug.LogError("HistoricoItemPrefab não está atribuído no Inspector!");
            return;
        }

        foreach (HistoricoEntry entry in historicoLista.historico)
        {
            GameObject newItem = Instantiate(historicoItemPrefab, contentContainer);
            
            HistoricoItemUI itemUI = newItem.GetComponent<HistoricoItemUI>();
            if (itemUI != null)
            {
                itemUI.Setup(entry, this);
            }
            else
            {
                Debug.LogError("O prefab 'Historico_Cell' está sem o script 'HistoricoItemUI'!", newItem);
            }
        }
    }
}