using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worm : Entity
{
    private Animator anim;
    private Collider2D col;
    private SpriteRenderer sprite;
    private Rigidbody2D rb;
    private bool isGrounded = false;
    private bool canDamage;

    private void Awake()
    {

        sprite = GetComponentInChildren<SpriteRenderer>();

    }
    private void Start()
    {
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        lives = 1;
        canDamage = true;
    }
    private void FixedUpdate()
    {
        if (!isGrounded)
            rb.WakeUp();
        else
            rb.Sleep();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == Hero.Instance.gameObject && Hero.isDead == false && canDamage == true)
        {
            Hero.Instance.GetDamage();
        }
        for (int i = 0; i < 10; i++)
        {
            if (collision.gameObject == Hero.Instance.layout[i])
            {
                GetDamage();
                StartCoroutine(EmemyOnAttack());
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
        if (collision.gameObject == Hero.Instance.gameObject && Hero.isDead == false)
        {
            StartCoroutine(HeroOnCol());
        }
    }
    public override void Die()
    {
        rb.isKinematic = true;
        col.isTrigger = true; 
        anim.SetTrigger("death");
        StartCoroutine(Clarity());
    }
    private void OnTriggerStay2D()
    {
        isGrounded = true;
    }
    private void OnTriggerExit2D()
    {
        isGrounded = false;
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        foreach(var i in collision.contacts)
        {
            if (i.normal.y > 0.5)
            {
                isGrounded = true;
                break;
            }
        }
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
    private IEnumerator EmemyOnAttack()
    {
        SpriteRenderer enemyColor = GetComponentInChildren<SpriteRenderer>();
        enemyColor.color = new Color(1, 0.27f, 0.27f, enemyColor.color.a);
        yield return new WaitForSeconds(0.2f);
        enemyColor.color = new Color(1, 1, 1, enemyColor.color.a);
    }
    private IEnumerator HeroOnCol()
    {
        canDamage = false;
        yield return new WaitForSeconds(0.2f);
        canDamage = true;
    }
}

