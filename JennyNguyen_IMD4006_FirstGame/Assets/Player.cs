using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    GameObject player, hammer, UI, hammerMech, playerModel;
    Animation playerAnimation;
    public int numLives, numEnemiesKilled;
    public bool hasBeenHit;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        hammer = GameObject.Find("Hammer");
        UI = GameObject.Find("Canvas");
        hammerMech = GameObject.Find("Hammer");
        playerModel = GameObject.Find("XGameCharacter");
        playerAnimation = playerModel.GetComponent<Animation>();
        numEnemiesKilled = 0;
        hasBeenHit = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (player.GetComponent<Player>().numLives > 0 && UI.GetComponent<UserInterface>().GameRunning == true)
        {
            if (player.GetComponent<GameController>().thrownHammerActive == true && player.GetComponent<GameController>().spinHammerActive == false)
            {
                //If post-released hammer state is active, the player will move towards the hammer
                if (player.GetComponent<GameController>().GoToHammer)
                {
                    transform.position = Vector3.MoveTowards(transform.position, new Vector3(hammer.transform.position.x, transform.position.y, hammer.transform.position.z), Time.deltaTime * 15f);

                    //Once the player reaches the thrown location of the hammer, reset the throw states
                    if (new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z) == new Vector3(hammer.transform.position.x, transform.position.y, hammer.transform.position.z))
                    {
                        player.GetComponent<GameController>().thrownHammerActive = false;
                        player.GetComponent<GameController>().GoToHammer = false;
                        hammerMech.GetComponent<HammerMechanics>().hammerRb.isKinematic = false;
                    }
                }
            }
        }
    }

    void OnTriggerEnter(Collider col)
    {
        //Player loses a life when they hit an enemy 
        if (col.tag == "Enemy")
        {
            if (player.GetComponent<GameController>().spinHammerActive == true || player.GetComponent<GameController>().thrownHammerActive == true || Input.GetKeyDown(KeyCode.X) == false)
            {
                hasBeenHit = true;
                numLives--;
            }
            
        }

        //Player will die completely if they collide with the outer wall
        if (col.tag == "Wall")
        {
            numLives = 0;
            numEnemiesKilled = 1;
        }
    }
}
