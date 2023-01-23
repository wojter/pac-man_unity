using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject pacman;

    public GameObject leftWarpNode;
    public GameObject rightWarpNode;

    public AudioSource siren;
    public AudioSource munch1;
    public AudioSource munch2;
    public AudioSource powerPelletAudio;
    public AudioSource respawningAudio;
    public AudioSource ghostEatenAudio;

    public int currentMunch = 0;

    public int score;
    public Text scoreText;

    public GameObject ghostNodeLeft;
    public GameObject ghostNodeRight;
    public GameObject ghostNodeCenter;
    public GameObject ghostNodeStart;

    public GameObject redGhost;
    public GameObject blueGhost;
    public GameObject pinkGhost;
    public GameObject orangeGhost;

    public EnemyController redGhostContoller;
    public EnemyController blueGhostContoller;
    public EnemyController orangeGhostContoller;
    public EnemyController pinkGhostContoller;


    public int totalPellets;
    public int pelletsLeft;
    public int pelletsCollectedOnThisLive;

    public bool hadDeathOnThisLevel = false;

    public bool gameIsRunning;

    public List<NodeController> nodeControllers = new List<NodeController>();

    public bool newGame;
    public bool clearedLevel;

    public AudioSource startGameAudio;
    public AudioSource death;

    public int lives;
    public int currentLevel;

    public Image blackBackground;

    public Text gameOverText;

    public int[] ghostModeTimers = new int[] { 7, 20, 7, 20, 5, 20, 5 };
    public int ghostModeTimerIndex;
    public float ghostModeTimer = 0f;
    public bool runningTimer;
    public bool completedTimer;


    public bool isPowerPelletRunning = false;
    public float currentPowerPelletTime = 0;
    public float powrPelletTimer = 8f;
    public int powerPelletMultiplier = 1;

    public enum GhostMode
    {
        chase, scatter
    }
    public GhostMode currentGhostMode;
    // Start is called before the first frame update
    void Awake()
    {
        newGame = true;
        clearedLevel = false;

        redGhostContoller = redGhost.GetComponent<EnemyController>();
        blueGhostContoller = blueGhost.GetComponent<EnemyController>();
        orangeGhostContoller = orangeGhost.GetComponent<EnemyController>();
        pinkGhostContoller = pinkGhost.GetComponent<EnemyController>();


        ghostNodeStart.GetComponent<NodeController>().isGhostStartingNode = true;
        pacman = GameObject.Find("Player");

        blackBackground.enabled = false;

        StartCoroutine(Setup());
    }

    public IEnumerator Setup()
    {
        ghostModeTimerIndex = 0;
        ghostModeTimer = 0;
        gameOverText.enabled = false;
        completedTimer = false;
        runningTimer = true;
        //If pacman clears a level, a background will appear covering the level, and the game will pause for 0.1 seconds
        if (clearedLevel)
        {
            //Activate background
            blackBackground.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }
        blackBackground.enabled = false;

        pelletsCollectedOnThisLive = 0;
        currentGhostMode = GhostMode.scatter;
        gameIsRunning = false;
        currentMunch = 0;

        float waitTimer = 1f;

        if (clearedLevel || newGame)
        {
            pelletsLeft = totalPellets;
            waitTimer = 4f;
            //Pellets will respawn when pacman clears the level or starts a new game
            for (int i=0; i< nodeControllers.Count; i++)
            {
                nodeControllers[i].RespawnPellet();
            }
        }

        if (newGame)
        {
            startGameAudio.Play();
            score = 0;
            scoreText.text = "Score: " + score.ToString();
            lives = 3;
            currentLevel = 1;
        }

        pacman.GetComponent<PlayerController>().Setup();

        redGhostContoller.Setup();
        pinkGhostContoller.Setup();
        blueGhostContoller.Setup();
        orangeGhostContoller.Setup();

        newGame = false;
        clearedLevel = false;
        yield return new WaitForSeconds(waitTimer);

        StartGame();
    }

    void StartGame()
    {
        gameIsRunning = true;
        siren.Play();

    }

    void StopGame()
    {
        gameIsRunning = false;
        siren.Stop();
        powerPelletAudio.Stop();
        pacman.GetComponent<PlayerController>().Stop();
    }

    // Update is called once per frame
    void Update()
    {

        if (!gameIsRunning)
        {
            return;
        }

        if ((redGhostContoller.ghostNodeState == EnemyController.GhostNodesStatesEnum.respawning)
            || (blueGhostContoller.ghostNodeState == EnemyController.GhostNodesStatesEnum.respawning)
            || (pinkGhostContoller.ghostNodeState == EnemyController.GhostNodesStatesEnum.respawning)
            || (orangeGhostContoller.ghostNodeState == EnemyController.GhostNodesStatesEnum.respawning)
            )
        {
            if (!respawningAudio.isPlaying)
            {
                respawningAudio.Play();
            }
        }
        else
        {
            if (respawningAudio.isPlaying)
            {
                respawningAudio.Stop();
            }
        }

         if (!completedTimer && runningTimer)
        {
            ghostModeTimer += Time.deltaTime;
            if (ghostModeTimer >= ghostModeTimers[ghostModeTimerIndex])
            {
                ghostModeTimer = 0;
                ghostModeTimerIndex++;
                if (currentGhostMode == GhostMode.chase)
                {
                    currentGhostMode = GhostMode.scatter;
                }
                else
                {
                    currentGhostMode = GhostMode.chase;
                }

                if (ghostModeTimerIndex == ghostModeTimers.Length)
                {
                    completedTimer = true;
                    runningTimer = false;
                    currentGhostMode = GhostMode.chase;
                }
            }
        }
         
         if (isPowerPelletRunning)
        {
            currentPowerPelletTime += Time.deltaTime;
            if (currentPowerPelletTime >= powrPelletTimer)
            {
                isPowerPelletRunning = false;
                currentPowerPelletTime = 0;
                powerPelletAudio.Stop();
                siren.Play();
                powerPelletMultiplier = 1;
            }
        }
    }

    public void GetPelletFromNodeController(NodeController nodeController)
    {
        nodeControllers.Add(nodeController);
        totalPellets++;
        pelletsLeft++;
    }

    public void AddToScore(int amount)
    {
        score += amount;
        scoreText.text = "Score:" + score.ToString();
    }

    public IEnumerator CollectedPellet(NodeController nodeController)
    {
        if (currentMunch == 0)
        {
            munch1.Play();
            currentMunch = 1;
        }
        else if (currentMunch == 1)
        {
            munch2.Play();
            currentMunch = 0;
        }

        pelletsLeft--;
        pelletsCollectedOnThisLive++;

        int requiredBluePellets = 0;
        int requiredOrangePellets = 0;

        if (hadDeathOnThisLevel)
        {
            requiredBluePellets = 12;
            requiredOrangePellets = 32;
        }
        else
        {
            requiredBluePellets = 30;
            requiredOrangePellets = 60;
        }

        if (pelletsCollectedOnThisLive >= requiredBluePellets && !blueGhost.GetComponent<EnemyController>().leftHomeBefore)
        {
            blueGhost.GetComponent<EnemyController>().readyToLeaveHome = true;
        }

        if (pelletsCollectedOnThisLive >= requiredOrangePellets && !orangeGhost.GetComponent<EnemyController>().leftHomeBefore)
        {
            orangeGhost.GetComponent<EnemyController>().readyToLeaveHome = true;
        }
        //Add to our score 
        AddToScore(10);
        
        //Check if there are any pellets left
        if (pelletsLeft == 0)
        {
            currentLevel +=1;
            clearedLevel = true;
            StopGame();
            yield return new WaitForSeconds(1);
            StartCoroutine(Setup());
        }

        //It this a power pellet
        if (nodeController.isPowerPellet)
        {
            siren.Stop();
            powerPelletAudio.Play();
            isPowerPelletRunning = true;
            currentPowerPelletTime = 0;

            redGhostContoller.SetFrightened(true);
            blueGhostContoller.SetFrightened(true);
            pinkGhostContoller.SetFrightened(true);
            orangeGhostContoller.SetFrightened(true);
        }
    }

    public IEnumerator PauseGame(float timeToPause)
    {
        gameIsRunning = false;
        yield return new WaitForSeconds(timeToPause);
        gameIsRunning = true;
    }

    public void GhostEaten()
    {
        ghostEatenAudio.Play();
        AddToScore(400 * powerPelletMultiplier);
        powerPelletMultiplier++;
        StartCoroutine(PauseGame(1));

    }

    public IEnumerator PlayerEaten()
    {
        hadDeathOnThisLevel = true;
        StopGame();
        yield return new WaitForSeconds(1);

        redGhostContoller.SetVisible(false);
        blueGhostContoller.SetVisible(false);
        orangeGhostContoller.SetVisible(false);
        pinkGhostContoller.SetVisible(false);

        pacman.GetComponent<PlayerController>().Death();
        death.Play();
        yield return new WaitForSeconds(3);

        lives--;
        if (lives <= 0)
        {
            newGame = true;
            gameOverText.enabled = true;
            yield return new WaitForSeconds(3);
        }

        StartCoroutine(Setup());
    }
}
