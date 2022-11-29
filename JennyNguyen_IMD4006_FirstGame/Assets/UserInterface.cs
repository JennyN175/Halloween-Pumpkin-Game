using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour
{
    Slider healthBar;
    GameObject player, menu, enemySpawner, hammer, playerModel, mainCamera, hammerMechanics;
    Animation playerAnimation;
    //Color healthBarColour;
    public Image healthBarFill, redScreen;
    Image MenuBG;
    public bool GameRunning, GameLost, GameStart;
    TMPro.TextMeshProUGUI menuText, enemyCountText;
    Vector3 startPos;

    Coroutine OnDamage;
    float timeOfTravel = 0.5f; 
    float currentTime = 0;
    float normalizedValue;

    public AudioClip ghostDeathSound, spinningSound, takeDamageSound, runningSound, pumpkinLandingSound, bgMusic;
    public AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        healthBar = GameObject.Find("HealthBar").GetComponent<Slider>();
        player = GameObject.Find("Player");
        menu = GameObject.Find("MenuBG");
        enemySpawner = GameObject.Find("EnemySpawner");
        hammer = GameObject.Find("Hammer");
        MenuBG = menu.GetComponent<Image>();
        menuText = GameObject.Find("MenuText").GetComponent<TMPro.TextMeshProUGUI>();
        enemyCountText = GameObject.Find("EnemyCount").GetComponent<TMPro.TextMeshProUGUI>();
        redScreen = GameObject.Find("RedScreen").GetComponent<Image>();
        playerModel = GameObject.Find("XGameCharacter");
        playerAnimation = playerModel.GetComponent<Animation>();
        mainCamera = GameObject.Find("Main Camera");
        hammerMechanics = GameObject.Find("Hammer");
        audioSource = GetComponent<AudioSource>();

        GameRunning = false;
        healthBar.maxValue = 6;
        redScreen.color = new Color(1, 0, 0, 0);
        startPos = mainCamera.transform.localPosition;

        GameStart = true;
        audioSource.PlayOneShot(bgMusic, 0.4F);
    }

    //Function for flashing red screen
    IEnumerator FlashDamageScreen()
    {
        currentTime = 0;
        while (currentTime <= timeOfTravel)
        {
            currentTime += Time.deltaTime;
            normalizedValue = currentTime / timeOfTravel; // we normalize our time 

            redScreen.color = Color.Lerp(new Color(1, 0, 0, 0), new Color(1, 0, 0, 0.1f), normalizedValue);
        }
        yield return new WaitForSeconds(0.4f);
        currentTime = 0;
        while (currentTime <= timeOfTravel)
        {
            currentTime += Time.deltaTime;
            normalizedValue = currentTime / timeOfTravel; // we normalize our time 

            redScreen.color = Color.Lerp(new Color(1, 0, 0, 0.1f), new Color(1, 0, 0, 0), normalizedValue);
        }

        player.GetComponent<Player>().hasBeenHit = false;
    }

    //Function for shaking camera
    IEnumerator ShakeCamera()
    {
        for (int i = 0; i < 2; i++)
        {
            mainCamera.transform.localPosition = new Vector3(Random.Range((startPos.x + 0.1f), (startPos.x - 0.1f)), Random.Range((startPos.y + 0.1f), (startPos.y - 0.1f)), Random.Range((startPos.z + 0.1f), (startPos.z - 0.1f)));
            yield return new WaitForSeconds(0.2f);
            mainCamera.transform.localPosition = startPos;
            yield return new WaitForSeconds(0.2f);
        }
    }

    //When the player takes damage, the screen will flash red and the camera will shake
    IEnumerator ShowDamageEffect()
    {
        OnDamage = StartCoroutine("FlashDamageScreen");
        OnDamage = StartCoroutine("ShakeCamera");
        yield return new WaitForSeconds(0.2f);
        player.GetComponent<Player>().hasBeenHit = false;
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.value = player.GetComponent<Player>().numLives;
        enemyCountText.text = "Enemies Left: " + enemySpawner.GetComponent<EnemySpawner>().currentNumEnemies;

        //Change colour of the health bar depending on the player's number of lives
        if (healthBar.value > 0)
        {
            GameRunning = true;

            if (healthBar.value > 3)
            {
                healthBarFill.color = Color.Lerp(Color.yellow, Color.green, healthBar.value / healthBar.maxValue);
            }
            else if (healthBar.value < 3)
            {
                healthBarFill.color = Color.Lerp(Color.red, Color.yellow, healthBar.value / healthBar.maxValue);
            }
            else if (healthBar.value == 3)
            {
                healthBarFill.color = Color.yellow;
            }

            if (enemySpawner.GetComponent<EnemySpawner>().currentNumEnemies == 0)
            {
                GameRunning = false;
            }
        }
        else
        {
            GameRunning = false;
        }

        //When player restarts the game, all variables and enemies are reset
        if (Input.GetKey(KeyCode.X) && GameRunning == false && !player.GetComponent<GameController>().spinHammerActive)
        {
            GameStart = false;
            GameRunning = true;
            enemySpawner.GetComponent<EnemySpawner>().spawnEnemies = false;
            player.GetComponent<Player>().numLives = 6;
            player.GetComponent<Player>().numEnemiesKilled = 0;
            enemySpawner.GetComponent<EnemySpawner>().currentNumEnemies = 10;
            player.transform.position = new Vector3(0, 1.11f, 0);
            hammer.transform.position = new Vector3(0, 0, 0);
        }
        
        //If the game stops running, a different UI will be shown depending on if the player won or lost
        if (GameRunning == false)
        {
            menu.SetActive(true);

            if (GameStart)
            {
                menuText.text = "Press X to start the game!";
            }
            else
            {
                if (enemySpawner.GetComponent<EnemySpawner>().currentNumEnemies > 0)
                {
                    menuText.text = "You lost! Press X to play again.";
                    player.GetComponent<Player>().hasBeenHit = false;
                }
                else if (enemySpawner.GetComponent<EnemySpawner>().currentNumEnemies == 0)
                {
                    menuText.text = "Congratulations! You've deafeated all the enemies. Press X to play again.";
                    player.GetComponent<Player>().hasBeenHit = false;
                }
            }
        }
        else
        {
            menu.SetActive(false);
            menuText.text = "";

            //Playing sound effects
            if (player.GetComponent<Player>().hasBeenHit)
            {
                audioSource.clip = takeDamageSound;
                audioSource.Play();
                OnDamage = StartCoroutine("ShakeCamera");
                OnDamage = StartCoroutine("FlashDamageScreen");
            }

            if (hammerMechanics.GetComponent<HammerMechanics>().enemyHit)
            {
                audioSource.clip = ghostDeathSound;
                audioSource.Play();
                hammerMechanics.GetComponent<HammerMechanics>().enemyHit = false;
            }

            if (player.GetComponent<GameController>().spinHammerActive)
            {
                audioSource.clip = spinningSound;
                audioSource.Play();
            }
        }
    }
}
