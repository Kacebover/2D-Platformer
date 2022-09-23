using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    public GameObject layoutoff;
    public GameObject layouton;
    public static bool pause;
    public static Pause Instance { get; set; }
    private void Awake()
    {
        pause = false;
        Instance = this;
        Time.timeScale = 1;
    }
    public void Pauser()
    {
        if (pause == true)
        {
            layouton.gameObject.SetActive(false);
            layoutoff.gameObject.SetActive(true);
            pause = false;
            Time.timeScale = 1;
        }
        else
        {
            layoutoff.gameObject.SetActive(false);
            layouton.gameObject.SetActive(true);
            pause = true;
            Time.timeScale = 0;
        }
    }
}
