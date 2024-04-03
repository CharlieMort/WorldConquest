using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public void ToGame()
    {
        SceneManager.LoadScene("GameScene");
    }
}
