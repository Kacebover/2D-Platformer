using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Restart : MonoBehaviour
{
    [SerializeField] private GameObject layout;
    [SerializeField] private Slider slider;
    [SerializeField] private GameObject slide;

    private int numLevel;

    private void Update() 
    {
        if (Finish.Levelpassed == false)
            layout.gameObject.SetActive(Hero.isDead);
    }

    public void OpenLevelsList()
    {
        slide.SetActive(true);
        numLevel = SceneManager.GetActiveScene().buildIndex;
        StartCoroutine(LoadAsync());
    }

    public void NextLevel()
    {
        slide.SetActive(true);
        numLevel = SceneManager.GetActiveScene().buildIndex + 1;
        StartCoroutine(LoadAsync());
    }
    private IEnumerator LoadAsync()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(numLevel);
        //asyncLoad.allowSceneActivation = false;
        while(!asyncLoad.isDone)
        {
            slider.value = asyncLoad.progress;
            //if (asyncLoad.progress >= .9f && !asyncLoad.allowSceneActivation)
            //{
                //if (Input.anyKeyDown)
               // {
                //    asyncLoad.allowSceneActivation = true;
               // }
            //}
            yield return null;
        }
    }
}
