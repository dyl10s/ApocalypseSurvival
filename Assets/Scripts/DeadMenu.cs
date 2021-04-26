using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeadMenu : MonoBehaviour
{
    public void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    public void PlayGame()
    {
        //if we ever change the name of the "main" loading point for the game this is
        // where we change it, there's other ways to load scenes as well by creating
        //a que of scenes, then incrementing by one if we want to have other scenes
        SceneManager.LoadScene("MainScene");
        Debug.Log("loading....");
    }

    public void ExitGame()
    {
        Debug.Log("QUIT...");
        Application.Quit();
    }
}
