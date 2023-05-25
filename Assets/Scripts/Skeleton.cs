using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Skeleton : Entity
{
    [SerializeField] private int speed = 2;
    private Vector3 dir;
    private SpriteRenderer sprite;
    [SerializeField] private AIPath AIPath;
    private Collider2D col;
    private Animator anim;
    [SerializeField] private AudioSource skeletonattacksound;
    [SerializeField] private GameObject hero;
    [SerializeField] private Transform herotarget;
    [SerializeField] private Transform spawntarget;
    [SerializeField] private float firstx;
    [SerializeField] private float secondx;
    [SerializeField] private float firsty;
    [SerializeField] private float secondy;
    private Rigidbody2D rb;
    private bool gettingdamage;
    [SerializeField] private Transform attackPos;
    [SerializeField] private float attackRange;
    [SerializeField] private LayerMask heromask;
    [SerializeField] private bool isRecharged;
    [SerializeField] private bool isAttacking;
    [SerializeField] private bool isAttackA;
    [SerializeField] private bool isGrounded;
    [SerializeField] private int jumpForce = 10;
    private Vector2 tempy = new Vector2 (0.72f, 0);
    public static Skeleton Instance { get; set; }

    private States State
    {
        get { return (States)anim.GetInteger("state"); }
        set { anim.SetInteger("state", (int)value); }
    }
    private void Awake()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
        lives = 3;
        AIPath = GetComponent<AIPath>();
        rb = GetComponent<Rigidbody2D>();
        gettingdamage = false;
        isAttacking = false;
        isRecharged = true;
        isAttackA = true;
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        dir = transform.right;
        if (Hero.Instance.isLinar)
            State = States.Stilletidle;
    }
    private void FixedUpdate()
    {
        Checkground();
    }
    void Update()
    {
        if (!isAttacking)
        {
            if (Hero.Instance.col.bounds.center.x <= secondx && Hero.Instance.col.bounds.center.x >= firstx && hero.transform.position.y <= firsty && hero.transform.position.y >= secondy)
            {
                GetComponent<AIDestinationSetter>().target = herotarget;
                if (lives > 0)
                {
                    if (Hero.Instance.col.bounds.center.x + Hero.Instance.col.bounds.size.x / 2 >= col.bounds.center.x - col.bounds.size.x / 2 - 0.1f && Hero.Instance.col.bounds.center.x - Hero.Instance.col.bounds.size.x / 2 <= col.bounds.center.x + col.bounds.size.x / 2 + 0.1f)
                    {
                        AIPath.enabled = false;
                        if (!gettingdamage)
                        {
                            if (isRecharged && Hero.isDead == false && !isAttackA)
                                AttackB();
                            else if (!Hero.Instance.isLinar)
                                State = States.skeletonidle;
                            else 
                                State = States.Stilletidle;
                        }
                    }
                    else
                    {
                        AIPath.enabled = true;
                        Move(4);
                        if (!gettingdamage)
                        {
                            if (!Hero.Instance.isLinar)
                                State = States.skeletonrun;
                            else if (isGrounded)
                            {
                                State = States.Stilletidle;
                                rb.velocity = Vector2.up * jumpForce;
                            }
                        }
                    }
                    if (isRecharged && !gettingdamage && Hero.isDead == false && isAttackA)
                    {
                        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackPos.position, attackRange, heromask);
                        if (colliders.Length == 1)
                            Attack();
                    }
                    if (AIPath.enabled)
                    {
                        if (sprite.flipX != Hero.Instance.col.bounds.center.x < col.bounds.center.x)
                        {
                            sprite.flipX = Hero.Instance.col.bounds.center.x < col.bounds.center.x;
                            dir *= -1;
                        }
                    }
                    else
                        sprite.flipX = Hero.Instance.col.bounds.center.x < col.bounds.center.x;
                }
            }
            else
            {
                AIPath.enabled = false;
                GetComponent<AIDestinationSetter>().target = spawntarget;
                if (lives > 0)
                    Move(2);
            }
        }
    }
    private void Checkground()
    {
        Vector2 temp = new Vector2 (col.bounds.center.x, transform.position.y);
        Collider2D[] collider = Physics2D.OverlapBoxAll(temp, tempy, 0);
        isGrounded = collider.Length > 1;
    }
    private void Move(int speed1)
    {
        if (!gettingdamage)
        {
            if (!Hero.Instance.isLinar)
                State = States.skeletonwalk;
            else if (isGrounded)
            {
                State = States.Stilletidle;
                rb.velocity = Vector2.up * jumpForce;
            }
        }
        Collider2D[] colliders;
        if (!Hero.Instance.isLinar)
            colliders = Physics2D.OverlapCircleAll(transform.position + transform.up * 0.1f + transform.right * dir.x * 0.7f, -0.1f);
        else
            colliders = Physics2D.OverlapCircleAll(transform.position + transform.up * -0.1f + transform.right * dir.x * 0.7f, -0.1f);
        if (col.bounds.center.x <= firstx || col.bounds.center.x >= secondx) 
        {
            dir *= -1;
        }
        transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, speed1 * Time.deltaTime);
        sprite.flipX = dir.x < 0.0f;
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

    public override void Die()
    {
        col.isTrigger = true; 
        AIPath.enabled = false;
        if (!Hero.Instance.isLinar)
            State = States.skeletondeath;
        else 
            State = States.Stilletidle;
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

    private void OnAttack()
    {
        skeletonattacksound.Play();
        if (Hero.isDead == false)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(attackPos.position, attackRange, heromask);
            if(colliders.Length == 1)
                Hero.Instance.GetDamage();

        }
    }
    private void OnAttackB()
    {
        if (Hero.Instance.col.bounds.center.x + Hero.Instance.col.bounds.size.x / 2 < col.bounds.center.x - col.bounds.size.x / 2 - 0.1f || Hero.Instance.col.bounds.center.x - Hero.Instance.col.bounds.size.x / 2 > col.bounds.center.x + col.bounds.size.x / 2 + 0.1f)
            isAttackA = true;
    }
    private IEnumerator EmemyOnAttack()
    {
        SpriteRenderer enemyColor = GetComponentInChildren<SpriteRenderer>();
        enemyColor.color = new Color(1, 0.27f, 0.27f, enemyColor.color.a);
        yield return new WaitForSeconds(0.2f);
        enemyColor.color = new Color(1, 1, 1, enemyColor.color.a);
    }
    public override IEnumerator GetHit()
    {
        if (!Hero.Instance.isLinar)
            State = States.skeletonhit;
        gettingdamage = true;
        yield return new WaitForSeconds(0.2f);
        gettingdamage = false;
    }
    private void Attack()
    {
        AIPath.enabled = false;
        if (!Hero.Instance.isLinar)
            State = States.skeletonattacka;
        else
            State = States.Stilletatta;
        isAttacking = true;
        isRecharged = false;
        isAttackA = false;
        StartCoroutine(AttackAnimation());
        StartCoroutine(AttackCoolDown());
    }
    private IEnumerator AttackAnimation()
    {
        yield return new WaitForSeconds(0.59f);
        isAttacking = false;
    }

    private IEnumerator AttackCoolDown()
    {
        yield return new WaitForSeconds(1);
        isRecharged = true;
    }
    private void AttackB()
    {
        AIPath.enabled = false;
        if (!Hero.Instance.isLinar)
            State = States.skeletonattackb;
        else
            State = States.Stilletattb;
        isAttacking = true;
        isRecharged = false;
        StartCoroutine(AttackAnimationB());
        StartCoroutine(AttackCoolDownB());
    }
    private IEnumerator AttackAnimationB()
    {
        yield return new WaitForSeconds(1.17f);
        OnAttackB();
        isAttacking = false;
    }

    private IEnumerator AttackCoolDownB()
    {
        yield return new WaitForSeconds(1.5f);
        isRecharged = true;
    }

}
