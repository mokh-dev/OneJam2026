using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public void ResetGame()
    {
        SceneManager.LoadScene(0);
    }

    public void StartGameFromMenu()
    {
        SceneManager.LoadScene(1);
    }
}
