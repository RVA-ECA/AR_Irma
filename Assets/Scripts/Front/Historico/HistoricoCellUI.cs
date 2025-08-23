using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HistoricoItemUI : MonoBehaviour
{
    public TMP_Text rmaText;
    public TMP_Text clienteText;
    public Image statusIcon;

    public void Setup(HistoricoEntry entry, HistoricoUIController controller)
    {
        if (rmaText != null)
        {
            rmaText.text = string.IsNullOrEmpty(entry.RMA) ? "MA: Pendente" : "MA: " + entry.RMA;
        }

        if (clienteText != null)
        {
            clienteText.text = "Cliente: " + entry.CLIENTE;
        }

        if (statusIcon != null)
        {
            switch (entry.STATUS)
            {
                case "Concluido":
                    statusIcon.sprite = controller.concluidoIcon;
                    break;
                case "Observação":
                    statusIcon.sprite = controller.observacaoIcon;
                    break;
                case "Nao recebido":
                    statusIcon.sprite = controller.naoRecebidoIcon;
                    break;
                default:
                    statusIcon.sprite = null;
                    break;
            }
        }
    }
}