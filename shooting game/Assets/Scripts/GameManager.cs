using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public Transform[] spawnPoints;

    public float maxSpawnDelay;
    public float curSpawnDelay;

    public GameObject player;
    public GameObject gameOverSet;
    public GameObject[] enemyObs;

    public Text scoreText;

    public Image[] lifeImage;

    public uint managerScore;

    [HideInInspector] public Player playerIsHit;

    void Start()
    {
        playerIsHit = player.GetComponent<Player>();
    }

    void Update()
    {
        SpawnDelay();

        //UI Score
        scoreText.text = string.Format("{0:n0}", managerScore);
    }

    void SpawnDelay()
    {
        curSpawnDelay += Time.deltaTime;

        if (curSpawnDelay > maxSpawnDelay)
        {
            SpawnEnemy();
            maxSpawnDelay = Random.Range(0.5f, 1.75f);
            curSpawnDelay = 0;
        }
    }

    void SpawnEnemy()
    {
        int ranEnemy = Random.Range(0, 4);
        int ranPoint = Random.Range(0, 6);
        Instantiate(enemyObs[ranEnemy], spawnPoints[ranPoint].position, spawnPoints[ranPoint].rotation);
    }

    public void RespawnPlayer()
    {
        Invoke("RespawnPlayerSet", 1.5f);
    }

    public void RespawnPlayerSet()
    {
        player.transform.position = Vector3.down * 4f;
        player.SetActive(true);

        playerIsHit.isHit = false;
    }

    public void UpdateLife(int life)
    {
        //UI life init disable 3개 다 끄고
        for (int index = 0; index < 3; index++)
            lifeImage[index].color = new Color(1, 1, 1, 0);

        //현재 남아있는 갯수대로만 킨다
        for (int index = 0; index < life; index++)
            lifeImage[index].color = new Color(1, 1, 1, 1);
    }

    public void GameOver()
    {
        gameOverSet.SetActive(true);
    }

    public void GameRetry()
    {
        SceneManager.LoadScene(0);
    }
}