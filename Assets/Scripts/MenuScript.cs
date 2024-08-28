using UnityEngine;
using UnityEngine.SceneManagement; // Asegúrate de importar esto

public class MenuScript : MonoBehaviour
{
    // Método asignado al botón "Jugar" para cargar la siguiente escena
    public void LoadNextScene()
    {
        // Carga la siguiente escena en la build (por índice o nombre)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); // O usa un índice específico
    }

    // Método asignado al botón "Salir" para cerrar la aplicación
    public void ExitGame()
    {
        // Cierra la aplicación
        Application.Quit();

        // Para la funcionalidad en el editor de Unity (solo para pruebas)
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
