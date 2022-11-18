using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public static bool isMusic;
    public void OpenLevelsList()
    {
        SceneManager.LoadScene(1);
        isMusic = true;
    }
    public void OpenLevelsListMenu()
    {
        SceneManager.LoadScene(1);
        isMusic = false;
    }
}
