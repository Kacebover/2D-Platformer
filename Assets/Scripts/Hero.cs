using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Hero : Entity
{
    public GameObject[] layout = new GameObject[10];
    [SerializeField] private GameObject[] numbers = new GameObject[11];
    private int number = 0;
    public int deadzone;
    [SerializeField] private int speed = 3;
    [SerializeField] private int health;
    [SerializeField] private int jumpForce = 10;
    [SerializeField] private AudioSource jumpSound;
    [SerializeField] private AudioSource damageSound;
    [SerializeField] private AudioSource attackSound;
    [SerializeField] private AudioSource landingSound;
    private Vector2 tempy = new Vector2 (0.72f, 0);
    private bool gettingdamage;
    private bool isGrounded;
    private bool isGroundedfs;
    public static bool isDead;
    private bool jumpTimer;
    private bool onGround;
    private float normal;

    [SerializeField] private Image[] hearts;

    [SerializeField] private Sprite aliveHeart;
    [SerializeField] private Sprite deadHeart;
    [SerializeField] private bool[] knifes = new bool[10];

    private bool isAttacking;
    public static bool[] isKnifed = new bool [10];
    private bool isRecharged;
    private bool isRechargedKnife;

    [SerializeField] private Transform attackPos;
    [SerializeField] private float attackRange;
    [SerializeField] private LayerMask enemy;
    [SerializeField] private Joystick joystick;

    private Rigidbody2D rb;
    private Animator anim;
    public static SpriteRenderer sprite;
    public Collider2D col;

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
        isGroundedfs = true;
        gettingdamage = false;
        jumpTimer = true;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        for (int i = 0; i < 10; i++)
        {
            isKnifed[i] = false;
            knifes[i] = true;
        }
        isAttacking = false;
        isRecharged = true;
        isRechargedKnife= true;
    }

    private void FixedUpdate()
    {
        if (Pause.pause == false && lives > 0 && isDead == false)
        {
            if (!onGround)
                Checkground();
            if (isGrounded && isGroundedfs == false)
            {
                landingSound.Play();
                isGroundedfs = true;
            }
            else if (!isGrounded && isDead == false && gettingdamage == false) 
            {
                if (isGroundedfs == true)
                    isGroundedfs = false;
                if (State != States.jump)
                    State = States.jump;
            }
            if (!isAttacking && joystick.Horizontal != 0)
                Run();
            else if (!isAttacking && Input.GetButton("Horizontal"))
                Run();
        }
    }

    private void Update()
    {
        for (int i = 0; i < 10; i++)
            layout[i].gameObject.SetActive(!knifes[i]);
        if (isDead == false)
        {
        if (lives <= 0 && gettingdamage == false)
        {
            isDead = true;
            State = States.death;
        }
        else if (transform.position.y < deadzone)
        {
            damageSound.Play();
            isDead = true;
            State = States.death;
        }
        else if (Pause.pause == false && lives > 0)
        {
            if (isGrounded && !isAttacking && gettingdamage == false) State = States.idle;

            if (!isAttacking && joystick.Horizontal != 0)
            {
                if (isGrounded && gettingdamage == false) State = States.run;
            }
            else if (!isAttacking && Input.GetButton("Horizontal"))
            {
                if (isGrounded && gettingdamage == false) State = States.run;
            }
            if (!isAttacking && isGrounded && jumpTimer && joystick.Vertical > 0.5f)
            {
                Jump();
            }
            else if (!isAttacking && isGrounded && jumpTimer && Input.GetButtonDown("Jump"))
            {
                Jump();
            }
            else if (!isAttacking && isGrounded && jumpTimer && Input.GetKeyDown(KeyCode.W))
            {
                Jump();
            }
            else if (!isAttacking && isGrounded && jumpTimer && Input.GetKeyDown(KeyCode.UpArrow))
            {
                Jump();
            }
            if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.RightControl))
                Attack();
            if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
                Knifeatt();
        }
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.End))
            Pause.Instance.Pauser();
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
        StartCoroutine(HeroOnJump());
        jumpSound.Play();
    }

    private void Checkground()
    {
        Vector2 temp = new Vector2 (Instance.col.bounds.center.x, Instance.transform.position.y);
        Collider2D[] collider = Physics2D.OverlapBoxAll(temp, tempy, 0);
        isGrounded = collider.Length > 1;
    }

    public override void GetDamage()
    {
        damageSound.Play();
        State = States.gettingdamage;
        StartCoroutine(HeroOnAttack());
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

    public void Knifeatt()
    {
        if (isRecharged && gettingdamage == false && isRechargedKnife)
        {
            for(int i = 0; i < 10; i++)
            {
                if (knifes[i] == true)
                {
                    numbers[number].gameObject.SetActive(false);
                    number++;
                    numbers[number].gameObject.SetActive(true);
                    isKnifed[i] = true;
                    knifes[i] = false;
                    isRechargedKnife = false;
                    i = 10;
                    StartCoroutine(KnifeCoolDown());
                }
            }
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
        yield return new WaitForSeconds(0.46f);
        isAttacking = false;
    }

    private IEnumerator AttackCoolDown()
    {
        yield return new WaitForSeconds(0.7f);
        isRecharged = true;
    }

    private IEnumerator KnifeCoolDown()
    {
        yield return new WaitForSeconds(0.5f);
        isRechargedKnife = true;
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

    private IEnumerator HeroOnJump()
    {
        rb.velocity = Vector2.up * jumpForce;
        jumpTimer = false;
        yield return new WaitForSeconds(0.5f);
        jumpTimer = true;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        foreach(var i in collision.contacts)
        {
            normal = i.normal.y;
            if (i.normal.y > 0.5)
            {
                if (!onGround)
                    onGround = true;
                if (!isGrounded)
                    isGrounded = true;
                break;
            }
        }
    }

    private void OnCollisionExit2D()
    {
        onGround = false;
    }

    private void OnTriggerStay2D()
    {
        if (isGrounded)
            isGrounded = false;
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
