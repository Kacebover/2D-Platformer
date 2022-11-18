using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    [SerializeField] private Button[] levels;
    [SerializeField] private AudioSource menu;
    [SerializeField] private Slider slider;
    [SerializeField] private GameObject slide;

    private int numLevel;
    private void Start()
    {
        int levelReached = PlayerPrefs.GetInt("levelReached", 1);

        for (int i = 0; i < levels.Length; i++)
            if (i + 1 > levelReached)
                levels[i].interactable = false;
        if (MainMenu.isMusic)
            menu.Play();
    }

    public void ToMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void Select(int numberInBuild)
    {
        slide.SetActive(true);
        numLevel = numberInBuild;
        StartCoroutine(LoadAsync());
        Destroy(GameObject.Find("Audio Source"));
        menu.Stop();
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
