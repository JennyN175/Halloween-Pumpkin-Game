using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemy, player, UI, ghostModel;
    public GameObject[] listEnemies;
    float xPos;
    float zPos;
    public int enemyCount, numEnemies, currentNumEnemies;
    int spawnSide;
    public bool spawnEnemies;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        UI = GameObject.Find("Canvas");
        spawnSide = Random.Range(0, 4);
        enemyCount = 0;
        numEnemies = 10;
        currentNumEnemies = 10;
        spawnEnemies = false;
    }

    // Update is called once per frame
    void Update()
    {
        //If a new game starts, destroy all enemies from the previous game and start spawning new ones
        if (UI.GetComponent<UserInterface>().GameRunning == true && spawnEnemies == false)
        {
            listEnemies = GameObject.FindGameObjectsWithTag("Enemy");
            for (int i = 0; i < listEnemies.Length; i++)
            {
                Destroy(listEnemies[i]);
            }

            enemyCount = 0;
            StartCoroutine(SpawnEnemy());
            spawnEnemies = true;
        }
    }

    IEnumerator SpawnEnemy()
    {
        if (player.GetComponent<Player>().numLives > 0)
        {
            while (enemyCount < numEnemies)
            {
                if (spawnSide == 0)
                {
                    //Instantiate enemy object and ghost model, and parent the ghost model to the enemy object
                    GameObject enemyObject = Instantiate(enemy, new Vector3(-24.0f, 0.2f, Random.Range(player.transform.position.z - 10.0f, player.transform.position.z + 10.0f)), Quaternion.identity);
                    GameObject ghostObject = Instantiate(ghostModel, new Vector3(enemyObject.transform.position.x, enemyObject.transform.position.y-0.5f, enemyObject.transform.position.z), Quaternion.identity);
                    ghostObject.transform.parent = enemyObject.transform;

                    //Choose random side of screen to spawn enemy from
                    spawnSide = Random.Range(0, 4);
                }
                else if (spawnSide == 1)
                {
                    GameObject enemyObject = Instantiate(enemy, new Vector3(24.0f, 0.2f, Random.Range(player.transform.position.z - 10.0f, player.transform.position.z + 10.0f)), Quaternion.identity);
                    GameObject ghostObject = Instantiate(ghostModel, new Vector3(enemyObject.transform.position.x, enemyObject.transform.position.y - 0.5f, enemyObject.transform.position.z), Quaternion.identity);
                    ghostObject.transform.parent = enemyObject.transform;
                    spawnSide = Random.Range(0, 4);
                }
                else if (spawnSide == 2)
                {
                    GameObject enemyObject = Instantiate(enemy, new Vector3(Random.Range(player.transform.position.x - 20.0f, player.transform.position.x + 20.0f), 0.2f, -14f), Quaternion.identity);
                    GameObject ghostObject = Instantiate(ghostModel, new Vector3(enemyObject.transform.position.x, enemyObject.transform.position.y - 0.5f, enemyObject.transform.position.z), Quaternion.identity);
                    ghostObject.transform.parent = enemyObject.transform;
                    spawnSide = Random.Range(0, 4);
                }
                else if (spawnSide == 3)
                {
                    GameObject enemyObject = Instantiate(enemy, new Vector3(Random.Range(player.transform.position.x - 20.0f, player.transform.position.x + 20.0f), 0.2f, 14.0f), Quaternion.identity);
                    GameObject ghostObject = Instantiate(ghostModel, new Vector3(enemyObject.transform.position.x, enemyObject.transform.position.y - 0.5f, enemyObject.transform.position.z), Quaternion.identity);
                    ghostObject.transform.parent = enemyObject.transform;
                    spawnSide = Random.Range(0, 4);
                }

                //Spawns an enemy every 4 seconds
                yield return new WaitForSeconds(4.0f);
                enemyCount += 1;
            }
        }
    }
}
