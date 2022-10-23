using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class FlyingMonster : Entity
{
    private SpriteRenderer sprite;
    [SerializeField] private AIPath AIPath;
    private Collider2D col;
    private Animator anim;
    [SerializeField] private AudioSource flyingmonsterattacksound;
    [SerializeField] private GameObject hero;
    [SerializeField] private Transform herotarget;
    [SerializeField] private Transform spawntarget;
    [SerializeField] private float firstx;
    [SerializeField] private float secondx;
    [SerializeField] private float firsty;
    [SerializeField] private float secondy;
    private bool canDamage;

    private States State
    {
        get { return (States)anim.GetInteger("state"); }
        set { anim.SetInteger("state", (int)value); }
    }
    private void Awake()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
        lives = 2;
        AIPath = GetComponent<AIPath>();
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        canDamage = true;
    }
    void Update()
    {
        if (Hero.Instance.col.bounds.center.x <= secondx && Hero.Instance.col.bounds.center.x >= firstx && hero.transform.position.y <= firsty && hero.transform.position.y >= secondy)
            GetComponent<AIDestinationSetter>().target = herotarget;
        else
            GetComponent<AIDestinationSetter>().target = spawntarget;
        if (lives > 0)
            sprite.flipX = AIPath.desiredVelocity.x <= 0.01f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == Hero.Instance.gameObject && Hero.isDead == false && lives > 0 && canDamage == true)
        {
            State = States.flyingmonsterattack;
            StartCoroutine(Attacking());
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
        AIPath.enabled = false;
        State = States.flyingmonsterdeath;
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

    private IEnumerator Attacking()
    {
        yield return new WaitForSeconds(0.82f);
        State = States.flyingmonsterfly;
    }

    private void OnAttack()
    {
        flyingmonsterattacksound.Play();
        Hero.Instance.GetDamage();
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
