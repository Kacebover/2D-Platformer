using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Monster : Entity
{
    private Animator anim;
    [SerializeField] private int speed = 4;
    private Vector3 dir;
    private SpriteRenderer sprite;
    private Collider2D col;
    private bool canDamage;

    private States State
    {
        get { return (States)anim.GetInteger("state"); }
        set { anim.SetInteger("state", (int)value); }
    }
    private void Awake()
    {

        sprite = GetComponentInChildren<SpriteRenderer>();

    }

    private void Start()
    {
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        dir = transform.right;
        lives = 1;
        canDamage = true;
    }

    private void Move()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position + transform.up * 0.1f + transform.right * dir.x * 0.7f, -0.1f);
        if (colliders.Length > 0) 
        {
            if (colliders.All(x=>x.GetComponent<Hero>()) && Hero.isDead == false && canDamage == true)
                Hero.Instance.GetDamage();
            else if (colliders.All(x=>x.GetComponent<Monster>()))
            {
                for (int i = 0; i < colliders.Length; i++)
                {
                    colliders[i].GetComponent<Monster>().dir *= -1;
                }
            }
            dir *= -1;
        }
        transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, speed * Time.deltaTime);
        sprite.flipX = dir.x < 0.0f;
    }

    private void Update()
    {
        if (lives > 0)
        {
            Move();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == Hero.Instance.gameObject && Hero.isDead == false && canDamage == true)
        {
            Hero.Instance.GetDamage();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
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
        if (collision.gameObject == Hero.Instance.gameObject && Hero.isDead == false)
        {
            StartCoroutine(HeroOnCol());
        }
    }

    public override void Die()
    {
        col.isTrigger = true; 
        State = States.deathmonster;
        StartCoroutine(Clarity());
    }

    private IEnumerator Clarity()
    {
        Color color = sprite.color;
        for(;color.a > 0.0f;)
        {
            yield return new WaitForSeconds(0.1f);
            color = sprite.color;
            color.a -= 0.1f;
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

    public override IEnumerator GetHit()
    {
        State = States.monsterhit;
        yield return new WaitForSeconds(0.25f);
        if (lives > 0)
            State = States.monster;
    }
}
