using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TempSceneReturn : MonoBehaviour
{
    public string returnSceneName = "MainRoom";
    public float  delay = 5f;
    public bool   isSin1 = true;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(delay);
        if (isSin1) GameState.returnedFromSin1 = true;
        else        GameState.returnedFromSin2 = true;
        SceneManager.LoadScene(returnSceneName);
    }
}
