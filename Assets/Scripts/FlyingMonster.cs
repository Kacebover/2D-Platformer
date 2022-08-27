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
    public GameObject hero;
    [SerializeField] private Transform herotarget;
    [SerializeField] private Transform spawntarget;

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
    }
    void Update()
    {
        if (hero.transform.position.x <= 119.9 && hero.transform.position.x >= 98 && hero.transform.position.y <= -1)
            GetComponent<AIDestinationSetter>().target = herotarget;
        else
            GetComponent<AIDestinationSetter>().target = spawntarget;
        if (lives > 0)
            sprite.flipX = AIPath.desiredVelocity.x < 0.01f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == Hero.Instance.gameObject && Hero.isDead == false)
        {
            State = States.flyingmonsterattack;
            StartCoroutine(Attacking());
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
}
