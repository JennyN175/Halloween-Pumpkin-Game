using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    GameObject player, UI, enemySpawner;
    Animation enemyAnimation;
    bool hasBeenHit;
    Coroutine HitSequence;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        UI = GameObject.Find("Canvas");
        enemySpawner = GameObject.Find("EnemySpawner");
        hasBeenHit = false;
    }
    IEnumerator PlayDeathAnimation()
    {
        GetComponent<Collider>().enabled = false;
        enemyAnimation.Play("Armature|Action_Die");
        yield return new WaitForSeconds(1.0f);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (player.GetComponent<Player>().numLives > 0)
        {
            //Move towards player while active
            if (gameObject.activeSelf == true && !hasBeenHit)
            {
                enemyAnimation = GetComponentInChildren<Animation>();
                enemyAnimation.Play("Armature|Action_Atack");
                Quaternion rot = Quaternion.LookRotation(player.transform.position - transform.position, Vector3.up);
                transform.rotation = rot;
                transform.position = Vector3.MoveTowards(transform.position, player.transform.position, Time.deltaTime * 1.5f);
            }
        }
    }

    void OnTriggerEnter(Collider col)
    {
        //Destroy enemy object if it collides with the hammer or the player
        if (col.name == "Hammer")
        {
            if (!player.GetComponent<GameController>().spinHammerActive)
            {
                hasBeenHit = true;
                player.GetComponent<Player>().numEnemiesKilled++;
                enemySpawner.GetComponent<EnemySpawner>().currentNumEnemies--;
                HitSequence = StartCoroutine("PlayDeathAnimation");
            }
        }

        if (col.name == "Player")
        {
            Destroy(gameObject);
            enemySpawner.GetComponent<EnemySpawner>().currentNumEnemies--;
        }
    }
}
