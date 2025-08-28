using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cooldown : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ButtonManager : MonoBehaviour
{
    [Header("Configuração de Cooldown (segundos)")]
    public float cooldown = 0.5f;

    private Dictionary<Button, float> lastClickTimes = new Dictionary<Button, float>();

    void Start()
    {
        // Pega todos os botões dentro do Canvas (ou objeto pai onde este script está)
        Button[] buttons = GetComponentsInChildren<Button>(true);

        foreach (Button btn in buttons)
        {
            // Inicializa tempo
            lastClickTimes[btn] = -cooldown;

            // Adiciona listener com cooldown
            btn.onClick.AddListener(() => HandleButtonClick(btn));
        }
    }

    void HandleButtonClick(Button btn)
    {
        if (Time.time - lastClickTimes[btn] < cooldown)
        {
            Debug.Log($"[IGNORADO] Clique muito rápido em {btn.name}");
            return; // ignora clique repetido
        }

        lastClickTimes[btn] = Time.time;

        // Chama a função real que o botão deveria executar
        Debug.Log($"[OK] Clique válido em {btn.name}");

        // Aqui você pode chamar uma função diferente dependendo do botão
        // Exemplo:
        if (btn.name == "icon_toggleReceive")
        {
            // Sua lógica desse botão
        }
    }
}
