using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worm : Entity
{
    private Animator anim;
    private Collider2D col;
    private SpriteRenderer sprite;

    private void Awake()
    {

        sprite = GetComponentInChildren<SpriteRenderer>();

    }
    private void Start()
    {
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        lives = 1;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == Hero.Instance.gameObject && Hero.isDead == false)
        {
            Hero.Instance.GetDamage();
        }
        for (int i = 0; i < 10; i++)
        {
            if (collision.gameObject == Hero.Instance.layout[i])
            {
                GetDamage();
            }
        }
    }
    public override void Die()
    {
        col.isTrigger = true; 
        anim.SetTrigger("death");
        StartCoroutine(Clarity());
    }
    private IEnumerator Clarity()
    {
        Color color = sprite.color;
        color.a = 1f;
        for(;color.a > 0.0f;)
        {
            yield return new WaitForSeconds(0.015f);
            color = sprite.color;
            color.a -= 0.01f;
            sprite.color=color;
        }
        Destroy(this.gameObject);
    }
}

