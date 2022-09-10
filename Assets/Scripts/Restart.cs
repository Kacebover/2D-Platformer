using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    public GameObject layout;

    public void Update() 
    {
        if (Finish.Levelpassed == false)
            layout.gameObject.SetActive(Hero.isDead);
    }

    public void PlayCurrentLevel()
    {

    }

    public void OpenLevelsList()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
