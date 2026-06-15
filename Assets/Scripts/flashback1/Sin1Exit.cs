using UnityEngine;
using UnityEngine.SceneManagement;

public class Sin1Exit : MonoBehaviour
{
    public string mainSceneName = "MainRoom";

    public void ReturnToMain()
    {
        GameState.returnedFromSin1 = true;
        SceneManager.LoadScene(mainSceneName);
    }
}
