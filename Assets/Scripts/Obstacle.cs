using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private bool canDamage;
    
    private void Start()
    {
        canDamage = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == Hero.Instance.gameObject && Hero.isDead == false && canDamage == true)
        {
            Hero.Instance.GetDamage();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject == Hero.Instance.gameObject && Hero.isDead == false)
        {
            StartCoroutine(HeroOnCol());
        }
    }

    private IEnumerator HeroOnCol()
    {
        canDamage = false;
        yield return new WaitForSeconds(0.2f);
        canDamage = true;
    }
}
