using Meta.WitAi;
using Meta.WitAi.Dictation;
using Oculus.Voice.Dictation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VoiceToTextManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button stopButton;
    [SerializeField] private TMP_Text resultText;

    [Header("Voice")]
    [SerializeField] private AppDictationExperience dictationExperience;

    private bool isListening = false;

    private void Start()
    {
        if (startButton != null)
            startButton.onClick.AddListener(StartDictation);

        if (stopButton != null)
            stopButton.onClick.AddListener(StopDictation);

        UpdateButtonStates();
    }

    private void StartDictation()
    {
        if (dictationExperience == null)
        {
            Debug.LogError("DictationExperience não foi atribuído!");
            return;
        }

        if (isListening) return;

        isListening = true;
        resultText.text = " Ouvindo...";
        dictationExperience.Activate();
        UpdateButtonStates();
    }

    private void StopDictation()
    {
        if (dictationExperience == null)
        {
            Debug.LogError("DictationExperience não foi atribuído!");
            return;
        }

        if (!isListening) return;

        isListening = false;
        dictationExperience.Deactivate();
        resultText.text += "\n Gravação finalizada.";
        UpdateButtonStates();
    }

    private void UpdateButtonStates()
    {
        if (startButton != null)
            startButton.interactable = !isListening;

        if (stopButton != null)
            stopButton.interactable = isListening;
    }

    private void OnEnable()
    {
        if (dictationExperience != null && dictationExperience.DictationEvents != null)
        {
            dictationExperience.DictationEvents.OnPartialTranscription.AddListener(OnPartialTranscription);
            dictationExperience.DictationEvents.OnFullTranscription.AddListener(OnFullTranscription);
            dictationExperience.DictationEvents.OnError.AddListener(OnDictationError);
            dictationExperience.DictationEvents.OnResponse.AddListener(OnDictationResponse);
        }
    }

    private void OnDisable()
    {
        if (dictationExperience != null && dictationExperience.DictationEvents != null)
        {
            dictationExperience.DictationEvents.OnPartialTranscription.RemoveListener(OnPartialTranscription);
            dictationExperience.DictationEvents.OnFullTranscription.RemoveListener(OnFullTranscription);
            dictationExperience.DictationEvents.OnError.RemoveListener(OnDictationError);
            dictationExperience.DictationEvents.OnResponse.RemoveListener(OnDictationResponse);
        }
    }

    private void OnPartialTranscription(string text)
    {
        resultText.text = " " + text;
    }

    private void OnFullTranscription(string text)
    {
        resultText.text = text;
    }

    private void OnDictationResponse(Meta.WitAi.Json.WitResponseNode response)
    {
        Debug.Log(" Resposta completa recebida do Wit.ai");
    }

    private void OnDictationError(string error, string message)
    {
        resultText.text = $" Erro: {error}\n{message}";
        isListening = false;
        UpdateButtonStates();
    }
}
