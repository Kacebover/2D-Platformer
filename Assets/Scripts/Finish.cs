using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finish : MonoBehaviour
{
    public GameObject layout;
    public static bool Levelpassed;

    private void Awake()
    {
        Levelpassed = false;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == Hero.Instance.gameObject && Hero.isDead == false)
        {
            layout.gameObject.SetActive(true);
            Levelpassed = true;
            Hero.isDead = true;
        }
    }
}
