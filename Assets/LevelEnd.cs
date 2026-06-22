using UnityEngine;
using UnityEngine.SceneManagement; // Обязательно для работы со сценами

public class LevelEnd : MonoBehaviour
{
    public string mainMenuSceneName = "MainMenu"; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
    }
}