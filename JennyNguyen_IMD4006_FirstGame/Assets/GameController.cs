using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    GameObject hammer, player, UI, playerModel;
    Animation playerAnimation;
    public bool spinHammerActive;
    public bool thrownHammerActive;
    public bool GoToHammer;
    public bool releasedHammer;

    Coroutine WaitFewSecsAfterHammerThrow;

    // Start is called before the first frame update
    void Start()
    {
        hammer = GameObject.Find("Hammer");
        player = GameObject.Find("Player");
        UI = GameObject.Find("Canvas");
        playerModel = GameObject.Find("XGameCharacter");
        playerAnimation = playerModel.GetComponent<Animation>();
        spinHammerActive = false;
        releasedHammer = false;
        thrownHammerActive = false;
        GoToHammer = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (player.GetComponent<Player>().numLives > 0 && UI.GetComponent<UserInterface>().GameRunning == true)
        {
            //Player has thrown hammer and it landed on the ground
            if (hammer.GetComponent<Rigidbody>().isKinematic == true)
            {
                spinHammerActive = false;
                releasedHammer = false;
                thrownHammerActive = true;
                //Debug.Log("Landed");
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                //Player is running to the location of the thrown hammer
                if (thrownHammerActive && !spinHammerActive)
                {
                    GoToHammer = true;
                    playerAnimation.Play("Running");
                    UI.GetComponent<UserInterface>().audioSource.clip = UI.GetComponent<UserInterface>().runningSound;
                    UI.GetComponent<UserInterface>().audioSource.Play();
                }

                //Player is spinning
                if (!thrownHammerActive)
                {
                    playerAnimation.Play("Spinning");
                    spinHammerActive = true;
                    releasedHammer = true;
                    thrownHammerActive = false;
                    
                    hammer.transform.position = new Vector3(player.transform.position.x + 2f, hammer.transform.position.y, player.transform.position.z + 2f);
                    //Debug.Log("Spinning");
                }
            }

            if (Input.GetKeyUp(KeyCode.X))
            {
                spinHammerActive = false;

                //Player has released spinning hammer
                if (spinHammerActive != releasedHammer)
                {
                    //Debug.Log("just let go");
                    playerAnimation.Play("Release");
                }
            }

            //Player faces toward the hammer while spinning
            if (spinHammerActive)
            {
                playerModel.transform.LookAt(hammer.transform.position);
            }

            //Player looks at hammer while standing
            if (!spinHammerActive && (spinHammerActive == releasedHammer) && !GoToHammer && !player.GetComponent<Player>().hasBeenHit)
            {
                playerAnimation.Play("Idle");
                Quaternion rot = Quaternion.LookRotation(hammer.transform.position - playerModel.transform.position, Vector3.up);
                playerModel.transform.rotation = rot;
                playerModel.transform.eulerAngles = new Vector3(0, playerModel.transform.eulerAngles.y, 0);
            }

            //Player looks at hammer when it is released
            if (spinHammerActive != releasedHammer)
            {
                //Debug.Log("let go");
                Quaternion rot = Quaternion.LookRotation(hammer.transform.position - playerModel.transform.position, Vector3.up);
                playerModel.transform.rotation = rot;
                playerModel.transform.eulerAngles = new Vector3(0, playerModel.transform.eulerAngles.y, 0);
            }

            if (player.GetComponent<Player>().hasBeenHit)
            {
                playerAnimation.Play("OnHit");
            }
        }
        
        if (UI.GetComponent<UserInterface>().GameRunning == false)
        {
            if (Input.GetKeyUp(KeyCode.X))
            {
                spinHammerActive = false;
                //Debug.Log("Not spinning");
            }
        }
    }
}
