using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Knife : MonoBehaviour
{
    [SerializeField] private int speed = 8;
    private Vector3[] dir = new Vector3[10];
    public static SpriteRenderer[] sprite = new SpriteRenderer[10];
    public static Knife Instance { get; set; }

    private void Awake()
    {
        for(int i = 0; i < 10; i++)
            sprite[i] = Hero.Instance.layout[i].GetComponentInChildren<SpriteRenderer>();
        Instance = this;
    }

    private void Update()
    {
        for (int i = 0; i < 10; i++)
        {
                if (Hero.isKnifed[i] == true)
                {
                    Hero.isKnifed[i] = false;
                    if (Hero.sprite.flipX == false)
                        dir[i] = Hero.Instance.layout[i].transform.right * -1;
                    else
                        dir[i] = Hero.Instance.layout[i].transform.right * 1;
                    if (dir[i].x > 0.0f)
                        Hero.Instance.layout[i].transform.position = new Vector3(Hero.Instance.col.bounds.center.x, Hero.sprite.transform.position.y + 1, Hero.sprite.transform.position.z);
                    else
                        Hero.Instance.layout[i].transform.position = new Vector3(Hero.Instance.col.bounds.center.x - 0.6f, Hero.sprite.transform.position.y + 1, Hero.sprite.transform.position.z);
                    sprite[i].flipX = dir[i].x < 0.0f;
                }
                Hero.Instance.layout[i].transform.position = Vector3.MoveTowards(Hero.Instance.layout[i].transform.position, Hero.Instance.layout[i].transform.position + dir[i], speed * Time.deltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
            if (collision.gameObject != Hero.Instance.gameObject)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y - (transform.position.y + -(Hero.Instance.deadzone)), transform.position.z);
            }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
            if (collision.gameObject != Hero.Instance.gameObject)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y - (transform.position.y + -(Hero.Instance.deadzone)), transform.position.z);
            }
    }
    private void OnBecameInvisible()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y - (transform.position.y + -(Hero.Instance.deadzone)), transform.position.z);
    }
}
