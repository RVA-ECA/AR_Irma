using UnityEngine;
using System.Diagnostics;
using System.IO;

public class PythonBridge : MonoBehaviour
{
    void Start()
    {
        string pythonPath = @"C:\Users\joahi\AppData\Local\Programs\Python\Python311\python.exe";
        string scriptPath = @"C:\Users\joahi\Downloads\meu_script.py";

        // Dados para enviar
        string inputData = "{\"valor\": 42}";

        // Cria o processo
        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = pythonPath,
            Arguments = $"\"{scriptPath}\" \"{inputData}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        Process process = new Process { StartInfo = psi };
        process.Start();

        // Lê retorno do Python
        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();
        process.WaitForExit();

        if (!string.IsNullOrEmpty(error))
            UnityEngine.Debug.LogError("Erro Python: " + error);
        else
            UnityEngine.Debug.Log("Retorno Python: " + output);
    }
}
