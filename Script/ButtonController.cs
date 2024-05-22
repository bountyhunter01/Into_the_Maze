using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour
{
    public string sceneName;
     public static bool GameIsPaused = false;
    public GameObject menuSet;
    public void RetryButton()
    {
        SceneManager.LoadScene(sceneName);
    }
   
}
