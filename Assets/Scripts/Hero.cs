using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Hero : Entity
{
    [SerializeField] private int speed = 3;
    [SerializeField] private int health;
    [SerializeField] private int jumpForce = 10;
    [SerializeField] private AudioSource jumpSound;
    [SerializeField] private AudioSource damageSound;
    [SerializeField] private AudioSource attackSound;
    [SerializeField] private AudioSource landingSound;
    private bool gettingdamage = false;
    public bool isGrounded = false;
    public bool isGroundedfs = true;
    public static bool isDead;

    [SerializeField] private Image[] hearts;

    [SerializeField] private Sprite aliveHeart;
    [SerializeField] private Sprite deadHeart;

    public bool isAttacking = false;
    public bool isRecharged;

    public Transform attackPos;
    public float attackRange;
    public LayerMask enemy;
    public Joystick joystick;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;

    public static Hero Instance { get; set; }

    private States State
    {
        get { return (States)anim.GetInteger("state"); }
        set { anim.SetInteger("state", (int)value); }
    }
    private void Awake()
    {
        lives = 5;
        health = lives;
        Instance = this;
        isDead = false;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        isRecharged = true;
    }

    private void FixedUpdate()
    {
        Checkground();
    }

    private void Update()
    {
        if (isDead == false)
        {
        if (lives <= 0 && gettingdamage == false)
        {
            isDead = true;
            State = States.death;
        }
        else if (transform.position.y < -10)
        {
            damageSound.Play();
            isDead = true;
            State = States.death;
        }
        else
        {
            if (isGrounded && !isAttacking && gettingdamage == false) State = States.idle;

            if (!isAttacking && joystick.Horizontal != 0)
                Run();
            else if (!isAttacking && Input.GetButton("Horizontal"))
                Run();
            if (!isAttacking && isGrounded && joystick.Vertical > 0.5f)
            {
                Jump();
            }
            else if (!isAttacking && isGrounded && Input.GetButtonDown("Jump"))
            {
                Jump();
            }
            if (Input.GetButtonDown("Fire2"))
                Attack();
        }
        if (health > lives)
            health = lives;

        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < health)
                hearts[i].sprite = aliveHeart;
            else
                hearts[i].sprite = deadHeart;
        }
        }
        else if (Finish.Levelpassed && State != States.idle)
            State = States.idle;
    }

    private void Run()
    {
        if (isGrounded && gettingdamage == false) State = States.run;
        Vector3 dir;
        if (joystick.Horizontal != 0)
            dir = transform.right * joystick.Horizontal;
        else
            dir = transform.right * Input.GetAxis("Horizontal");
        transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, speed * Time.deltaTime);
        sprite.flipX = dir.x > 0.0f;
    }

    private void Jump()
    {
        rb.velocity = Vector2.up * jumpForce;
        jumpSound.Play();
    }

    private void Checkground()
    {
        Collider2D[] collider = Physics2D.OverlapCircleAll(transform.position, 0.1f);
        isGrounded = collider.Length > 1;
        if (isGrounded && isGroundedfs == false)
        {
            landingSound.Play();
            isGroundedfs = true;
        }
        else if (!isGrounded && isDead == false && gettingdamage == false) 
        {
            if (isGroundedfs == true)
                isGroundedfs = false;
            State = States.jump;
        }
    }

    public override void GetDamage()
    {
        damageSound.Play();
        State = States.gettingdamage;
        StartCoroutine(HeroOnAttack());
        if (isDead == false)
            lives -= 1;
    }

    public void Attack()
    {
        if (isGrounded && isRecharged && gettingdamage == false)
        {
            State = States.attack;
            isAttacking = true;
            isRecharged = false;


            StartCoroutine(AttackAnimation());
            StartCoroutine(AttackCoolDown());
        }
    }

    private void OnAttack()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackPos.position, attackRange, enemy);
        attackSound.Play();
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].GetComponent<Entity>().GetDamage();
            StartCoroutine(EmemyOnAttack(colliders[i]));
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRange);
    }

    private IEnumerator AttackAnimation()
    {
        yield return new WaitForSeconds(0.7f);
        isAttacking = false;
    }

    private IEnumerator AttackCoolDown()
    {
        yield return new WaitForSeconds(0.7f);
        isRecharged = true;
    }

    private IEnumerator EmemyOnAttack(Collider2D enemy)
    {
        SpriteRenderer enemyColor = enemy.GetComponentInChildren<SpriteRenderer>();
        enemyColor.color = new Color(1, 0.27f, 0.27f, enemyColor.color.a);
        yield return new WaitForSeconds(0.2f);
        enemyColor.color = new Color(1, 1, 1, enemyColor.color.a);
    }

    private IEnumerator HeroOnAttack()
    {
        gettingdamage = true;
        sprite.color = new Color(1, 0.27f, 0.27f, sprite.color.a);
        yield return new WaitForSeconds(0.2f);
        sprite.color = new Color(1, 1, 1, sprite.color.a);
        gettingdamage = false;
    }
}

public enum States
{
    idle,
    run,
    jump,
    attack,
    death,
    flyingmonsterattack,
    flyingmonsterfly,
    flyingmonsterdeath,
    gettingdamage
}
