using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerMechanics : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject hammer, player, pumpkinModel, particleSystem, UI;
    public Rigidbody hammerRb;
    Coroutine WaitFewSecsAfterHammerThrow, AnimatePumpkin;
    Vector3 startBouncePos;

    float playerMass = 10f;
    
    float initialVelocity = 16.0f;
    float distance = 2.0f;
    float r = 10.0f;
    public bool enemyHit;

    void Start()
    {
        hammer = GameObject.Find("Hammer");
        player = GameObject.Find("Player");
        pumpkinModel = GameObject.Find("Pumpkin");
        particleSystem = GameObject.Find("Particle System");
        UI = GameObject.Find("Canvas");
        hammerRb = hammer.GetComponent<Rigidbody>();
        startBouncePos = new Vector3 (0,-0.5f,0);
        pumpkinModel.transform.localPosition = startBouncePos;
        particleSystem.SetActive(false);
        enemyHit = false;
    }

    //Waiting a moment to allow the landing sound effect to play
    IEnumerator Wait()
    {
        if (UI.GetComponent<UserInterface>().audioSource.clip != UI.GetComponent<UserInterface>().ghostDeathSound)
        {
            UI.GetComponent<UserInterface>().audioSource.clip = UI.GetComponent<UserInterface>().pumpkinLandingSound;
            UI.GetComponent<UserInterface>().audioSource.Play();
        }

        yield return new WaitForSeconds(0.2f);
        hammer.transform.position = new Vector3(transform.position.x, player.transform.position.y - 0.4f, transform.position.z);
        hammerRb.isKinematic = true;
    }

    //Bouncing animation
    IEnumerator BouncePumpkin()
    {
        Vector3 endPos = new Vector3(startBouncePos.x, 2f, startBouncePos.z);

        pumpkinModel.transform.localPosition = Vector3.MoveTowards(startBouncePos, endPos, Time.deltaTime * 10.0f);
        yield return new WaitForSeconds(0.1f);
        pumpkinModel.transform.localPosition = Vector3.MoveTowards(endPos, startBouncePos, Time.deltaTime * 10.0f);
        pumpkinModel.transform.localPosition = startBouncePos;
    }

    // Update is called once per frame
    void Update()
    {
        if (player.GetComponent<Player>().numLives > 0)
        {
            //If spinning state is active, the hammer will spin
            if (player.GetComponent<GameController>().spinHammerActive == true && player.GetComponent<GameController>().thrownHammerActive == false)
            {
                hammerRb.isKinematic = false;
                StartHammerSpin();
            }

            //When the player releases the hammer, the hammer will travel a certain distance and stop automatically
            if (((Mathf.Abs(player.transform.position.x - hammer.transform.position.x) > 3) || (Mathf.Abs(player.transform.position.z - hammer.transform.position.z) > 3)) && player.GetComponent<GameController>().spinHammerActive == false && !hammerRb.isKinematic)
            {
                //Adding bouncing animation as it travels
                AnimatePumpkin= StartCoroutine("BouncePumpkin");

                //Once it reaches a distance of 5, the hammer will stop traveling
                if (((Mathf.Abs(player.transform.position.x - hammer.transform.position.x) > 5) || (Mathf.Abs(player.transform.position.z - hammer.transform.position.z) > 5)) && player.GetComponent<GameController>().spinHammerActive == false && !hammerRb.isKinematic)
                {
                    WaitFewSecsAfterHammerThrow = StartCoroutine("Wait");
                }
            }

            //The hammer will not be visible if the player is not spinning it or if it hasn't been thrown
            if (new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z) == new Vector3(hammer.transform.position.x, transform.position.y, hammer.transform.position.z))
            {
                hammer.GetComponent<SphereCollider>().enabled = false;
                pumpkinModel.GetComponent<MeshRenderer>().enabled = false;
                particleSystem.SetActive(false);
            }
            else
            {
                hammer.GetComponent<SphereCollider>().enabled = true;
                pumpkinModel.GetComponent<MeshRenderer>().enabled = true;
                particleSystem.SetActive(true);
            }
        }
    }

    void SetVelocity()
    {
        Vector3 dir = (player.transform.position - hammer.transform.position).normalized;
        Vector3 cross = Vector3.Cross(Vector3.up, dir).normalized;

        hammerRb.velocity = cross * initialVelocity;
    }

    //Function for spinning hammer
    void StartHammerSpin()
    {
        hammer.transform.position = (hammer.transform.position - player.transform.position).normalized * distance + player.transform.position;

        SetVelocity();

        Vector3 dir = (player.transform.position - hammer.transform.position).normalized;
        Vector3 cross = Vector3.Cross(Vector3.up, dir).normalized;

        hammerRb.AddForce(dir * (Mathf.Pow(initialVelocity, 2)) / Vector3.Distance(hammer.transform.position, player.transform.position));
    }
    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Enemy")
        {
            //Will only count has a hit if the hammer is not spinning
            if (!player.GetComponent<GameController>().spinHammerActive)
            {
                enemyHit = true;
            }
        }
    }
}
