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
        // Exibindo o RMA de forma mais clara
        if (rmaText != null)
        {
            // Caso o RMA esteja vazio ou nulo, mostra "MA: Não informado"
            rmaText.text = string.IsNullOrEmpty(entry.RMA) ? "MA: Não informado" : "MA: " + entry.RMA;
        }

        // Exibindo o nome do cliente
        if (clienteText != null)
        {
            clienteText.text = "Cliente: " + entry.CLIENTE;
        }

        // Exibindo o ícone de status com base no status da entrada
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
