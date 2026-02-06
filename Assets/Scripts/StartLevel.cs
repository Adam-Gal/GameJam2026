using UnityEngine;
using UnityEngine.SceneManagement;

public class StartLevel : MonoBehaviour
{
    public void LevelChanger(string level)
    {
        SceneManager.LoadScene(level);
    }
}
