using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //PlayerMove
    private bool isTouchTop;
    private bool isTouchBottom;
    private bool isTouchLeft;
    private bool isTouchRight;

    public bool isHit; //목숨 2개 깎이기 방지

    //Fire
    public GameObject bulletObA;
    public GameObject bulletObB;
    private float maxShotDelay = 0.25f;
    public float curShotDelay;

    //Player가 아이템 먹을 때
    public float powerDownDelay = 5f;
    public float power;

    private float speed = 5f;
    public int life;

    //public new Rigidbody2D rigidbody2D;

    GameManager manager;

    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Update()
    {
        LimitMove();
        PlayerMove();
        Fire();
        PlayerDelayManager();
        PowerDownTime();
    }

    void PlayerMove()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        if ((isTouchRight && horizontal == 1) || (isTouchLeft && horizontal == -1))
            horizontal = 0;

        float vertical = Input.GetAxisRaw("Vertical");
        if ((isTouchTop && vertical == 1) || (isTouchBottom && vertical == -1))
            vertical = 0;
        Vector2 curPos = transform.position;
        Vector2 nextPos = new Vector2(horizontal, vertical) * speed * Time.deltaTime;

        transform.position = curPos + nextPos;

        if (Input.GetButtonDown("Horizontal") || Input.GetButtonUp("Horizontal")) // 누른 순간 1회
            anim.SetInteger("Input", (int)horizontal); //파라미터의 Input, Integer - 정수
    }

    void Fire()
    {
        if (Input.GetKey(KeyCode.Space) && curShotDelay > maxShotDelay) // 유니티 플레이 타임
        {
            switch (power)
            {
                case 0:
                    GameObject bullet = Instantiate(bulletObA, transform.position, transform.rotation);
                    Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
                    rigid.velocity = Vector2.up * 5;
                    break;

                case 1:
                    GameObject bulletL1 = Instantiate(bulletObA, transform.position + Vector3.left * 0.1f, transform.rotation);
                    GameObject bulletR1 = Instantiate(bulletObA, transform.position + Vector3.right * 0.1f, transform.rotation);
                    Rigidbody2D rigidL1 = bulletL1.GetComponent<Rigidbody2D>();
                    Rigidbody2D rigidR1 = bulletR1.GetComponent<Rigidbody2D>();
                    rigidL1.velocity = Vector2.up * 5;
                    rigidR1.velocity = Vector2.up * 5;
                    break;

                case 2:
                    GameObject bulletL2 = Instantiate(bulletObB, transform.position + Vector3.left * 0.15f, transform.rotation);
                    GameObject bulletR2 = Instantiate(bulletObB, transform.position + Vector3.right * 0.15f, transform.rotation);
                    Rigidbody2D rigidL2 = bulletL2.GetComponent<Rigidbody2D>();
                    Rigidbody2D rigidR2 = bulletR2.GetComponent<Rigidbody2D>();
                    rigidL2.velocity = Vector2.up * 5;
                    rigidR2.velocity = Vector2.up * 5;
                    break;
            }
            curShotDelay = 0f;
        }
    }

    void PlayerDelayManager()
    {
        //파워 감소 딜레이
        if (power >= 1f)
            powerDownDelay -= Time.deltaTime;

        //총알 발사 딜레이
        curShotDelay += Time.deltaTime;
    }

    void LimitMove()
    {
        Vector2 pos = Camera.main.WorldToViewportPoint(transform.position);

        if (pos.x < 0f) pos.x = 0f;
        if (pos.x > 1f) pos.x = 1f;
        if (pos.y < 0f) pos.y = 0f;
        if (pos.y > 1f) pos.y = 1f;

        transform.position = Camera.main.ViewportToWorldPoint(pos);

    }

    void PowerDownTime()
    {
        if (powerDownDelay <= 0)
        {
            powerDownDelay = 5f;
            power--;
        }
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "EnemyBullet")
        {
            //목숨 2개 깎이기 방지
            if (isHit)
                return;

            isHit = true;
            life--;
            manager.UpdateLife(life);

            if (life == 0)
                manager.GameOver();

            else
                manager.RespawnPlayer();

            gameObject.SetActive(false);
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.tag == "Item")
        {
            if (power == 2)
            {
                manager.managerScore += 1000;
                powerDownDelay += 1f;
            }
            else
                power++;

        }
    }
}