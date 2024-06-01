using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("GameManager");
                    instance = obj.AddComponent<GameManager>();
                }
            }
            return instance;
        }
    }

    public int playerTurnIndex;
    private int playerTurnFirst;
    private int piecesPlaced;
    private int round;
    public int circleWins;
    public int crossWins;

    private bool StartRoundHasBeenCalled = false;
    private bool GameStartHasBeenCalled = false;
    private PostProcessVolume blur;

    public RoundStatus currentRoundStatus;
    public GameState currentGameState;

    public GameObject[] field;
    public GameObject[] playerPrefab; // Circle is 0 - Cross is 1

    // UI Game Objects


    private AudioManager audioManager;
    private AudioSource audioSource;

    public bool musicMuted;


    private Animator MainCameraAnimator;

    private void Start()
    {
        MainCameraAnimator = GameObject.Find("Main Camera").GetComponent<Animator>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        audioSource = GameObject.Find("AudioManager").GetComponent<AudioSource>();
        blur = Camera.main.GetComponent<PostProcessVolume>();
        currentGameState = GameState.MainMenu;
        musicMuted = false;
    }

    private void Update()
    {
        
        if (round == 5)
        {
            currentGameState = GameState.GameOver;
        }

        if (currentGameState == GameState.OnGoingGame && round != 5)
        {
            CheckWinConditions();
        }

        if (piecesPlaced == 9 && currentGameState == GameState.OnGoingGame)
        {
            currentRoundStatus = RoundStatus.Tie;
            round++;
            currentGameState = GameState.StandbyGame;
        }

        switch (currentGameState)
        {
            case GameState.MainMenu:
                
                audioSource.volume = audioManager.FadeVolume(0.8f);
                if (MainCameraAnimator.GetInteger("Phase") == 0)
                {
                    MainCameraAnimator.SetInteger("Phase", 0);
                }
                else
                {
                    MainCameraAnimator.SetInteger("Phase", 2);
                }
                blur.enabled = false;
                break;

            case GameState.StandbyGame:
                audioSource.volume = audioManager.FadeVolume(0.3f);
                StartCoroutine(WaitStartRound(1));
                blur.enabled = false;
                break;

            case GameState.OnGoingGame:
                audioSource.volume = audioManager.FadeVolume(0.3f);
                MainCameraAnimator.SetInteger("Phase", 1);
                StartRoundHasBeenCalled = true;
                GameStartHasBeenCalled = true;
                blur.enabled = false;
                break;

            case GameState.PausedGame:
                if (MainCameraAnimator.GetInteger("Phase") == 1)
                {
                    audioSource.volume = 0;
                }
                blur.enabled = true;
                break;
            
            case GameState.GameOver:
                audioSource.volume = audioManager.FadeVolume(0.3f);
                StartCoroutine(WaitEndGame(1));
                MainCameraAnimator.SetInteger("Phase", 2);
                blur.enabled = false;
                break;
        }

        piecesPlaced = GameObject.FindGameObjectsWithTag("Player").Length;
    }

    private void ResetPlayers()
    {
        foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
        {
            Destroy(player);
        }
    }

    private void SetTurn()
    {
        if (circleWins == 0 && crossWins == 0)
        {
            playerTurnIndex = Random.Range(0, 2);
            playerTurnFirst = playerTurnIndex;
        }
        else
        {
            playerTurnFirst++;
            playerTurnIndex = playerTurnFirst;
            playerTurnIndex %= 2;
            
        }
        currentRoundStatus = playerTurnIndex == 0 ? RoundStatus.CircleTurn : RoundStatus.CrossTurn;
    }

    private void CheckWinConditions()
    {
        // Check if Circle wins
        if (CheckWinCondition("O"))
        {
            currentRoundStatus = RoundStatus.CircleWon;
            circleWins++;
            round++;
            currentGameState = GameState.StandbyGame;
        }
        // Check if Cross wins
        else if (CheckWinCondition("X"))
        {
            currentRoundStatus = RoundStatus.CrossWon;
            crossWins++;
            round++;
            currentGameState = GameState.StandbyGame;
        }
        
    }

    private bool CheckWinCondition(string symbol)
    {
        return (field[0].tag == symbol && field[1].tag == symbol && field[2].tag == symbol) ||
               (field[3].tag == symbol && field[4].tag == symbol && field[5].tag == symbol) ||
               (field[6].tag == symbol && field[7].tag == symbol && field[8].tag == symbol) ||
               (field[0].tag == symbol && field[3].tag == symbol && field[6].tag == symbol) ||
               (field[1].tag == symbol && field[4].tag == symbol && field[7].tag == symbol) ||
               (field[2].tag == symbol && field[5].tag == symbol && field[8].tag == symbol) ||
               (field[0].tag == symbol && field[4].tag == symbol && field[8].tag == symbol) ||
               (field[2].tag == symbol && field[4].tag == symbol && field[6].tag == symbol);
    }

    private IEnumerator WaitStartRound(float time)
    {
        StartRoundHasBeenCalled = false;
        yield return new WaitForSeconds(time);
        StartRound();
    }
    private IEnumerator WaitEndGame(float time)
    {
        GameStartHasBeenCalled = false;
        yield return new WaitForSeconds(time);
        if (!GameStartHasBeenCalled)
        {
            GameStartHasBeenCalled = true;
        }
    }

    public void SpawnPlayer(Vector3 dropOffset, Vector3 position)
    {
        if (playerPrefab.Length == 0)
        {
            Debug.LogError("Player prefab array is empty!");
            return;
        }

        playerTurnIndex %= playerPrefab.Length;

        Instantiate(playerPrefab[playerTurnIndex], position + dropOffset, Quaternion.identity);

        playerTurnIndex = (playerTurnIndex + 1) % playerPrefab.Length;

        currentRoundStatus = playerTurnIndex == 0 ? RoundStatus.CircleTurn : RoundStatus.CrossTurn;
    }

    public void StartRound()
    {
        
        if (!StartRoundHasBeenCalled)
        {
            ResetPlayers(); 
            SetTurn(); 
            currentGameState = GameState.OnGoingGame; 
        }
    }

    public void GameStart()
    {
        if (!GameStartHasBeenCalled)
        {
            currentGameState = GameState.StandbyGame;
            MainCameraAnimator.SetInteger("Phase", 1); 
            round = 0;
            circleWins = 0;
            crossWins = 0; 
            StartRound();
        }
    }

    public void Setttings()
    {

    }

    public void MainMenu()
    {
        MainCameraAnimator.SetInteger("Phase", 1);
        currentGameState = GameState.MainMenu;
    }

    public void PausedGame()
    {
        currentGameState = GameState.PausedGame;
        GameStartHasBeenCalled = false;
        StartRoundHasBeenCalled = false;
    }

    public void ResumeGame()
    {
        currentGameState = GameState.OnGoingGame;
    }

    public enum RoundStatus
    {
        CircleTurn,
        CrossTurn,
        CircleWon,
        CrossWon,
        Tie
    }

    public enum GameState
    {
        MainMenu,
        OnGoingGame,
        StandbyGame,
        PausedGame,
        GameOver
    }
}
