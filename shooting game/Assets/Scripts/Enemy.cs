using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public string enemyName;
    [SerializeField] private float speed;
    public float maxShotDelay;
    public float curShotDelay;

    public int health;
    public int randomPercentage;
    private int requestPercentage1 = 70;
    private int requestPercentage2 = 50;
    private int requestPercentage3 = 30;
    public uint enemyScore;

    public Sprite[] sprites;

    public GameObject bulletObA;
    public GameObject bulletObB;
    public GameObject player;

    public bool isDead;

    public GameObject powerUpItem;

    Rigidbody2D rigid;

    GameManager manager;

    SpriteRenderer spriterenderer;

    void Awake()
    {
        //player가 false일 때(=총알 맞은 후 투명화 상태) 소환될 경우 멈추는 현상 완화
        rigid = GetComponent<Rigidbody2D>();
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        spriterenderer = GetComponent<SpriteRenderer>();
        rigid.velocity = Vector2.down * speed;
    }

    void Start()
    {
        randomPercentage = (int)Random.Range(0, 101f);
    }

    void Update()
    {
        Fire();
        EnemyBulletDelay();
    }

    void EnemyBulletDelay()
    {
        curShotDelay += Time.deltaTime;
    }

    void OnHit(int dmg)
    {
        health -= dmg;

        //피격 효과를 나타내기 위한 스프라이트 변경
        spriterenderer.sprite = sprites[1];
        Invoke("ReturnSprite", 0.1f);

        if (health <= 0 && !isDead)
        {
            //목숨 2개 소멸 방지
            isDead = true;

            manager.managerScore += enemyScore;

            if (randomPercentage >= requestPercentage1)
                Instantiate(powerUpItem, transform.position, transform.rotation);
            else if (randomPercentage >= requestPercentage2)
                Instantiate(powerUpItem, transform.position, transform.rotation);
            else if (randomPercentage >= requestPercentage3)
                Instantiate(powerUpItem, transform.position, transform.rotation);

            Destroy(gameObject);
        }

        return;
    }

    void ReturnSprite()
    {
        spriterenderer.sprite = sprites[0];
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "PlayerBullet")
        {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            OnHit(bullet.dmg);
            Destroy(collision.gameObject);
        }

        else if (collision.gameObject.tag == "BorderBullet")
            Destroy(gameObject);
    }

    void Fire()
    {
        if (curShotDelay > maxShotDelay)
        {
            if (enemyName == "Enemy2")
            {
                GameObject bullet = Instantiate(bulletObA, transform.position, transform.rotation);
                Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
                Vector3 dirVec = player.transform.position - transform.position;
                rigid.AddForce(dirVec.normalized * 5, ForceMode2D.Impulse);
            }
            else if (enemyName == "Enemy3")
            {
                GameObject bulletL = Instantiate(bulletObB, transform.position + Vector3.left * 0.2f, transform.rotation);
                GameObject bulletR = Instantiate(bulletObB, transform.position + Vector3.right * 0.2f, transform.rotation);
                Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
                Vector3 dirVecL = player.transform.position - (transform.position + Vector3.left * 0.3f);
                Vector3 dirVecR = player.transform.position - (transform.position + Vector3.right * 0.3f);
                rigidL.AddForce(dirVecL.normalized * 5, ForceMode2D.Impulse);
                rigidR.AddForce(dirVecR.normalized * 5, ForceMode2D.Impulse);
            }
            curShotDelay = 0;
        }
    }
}