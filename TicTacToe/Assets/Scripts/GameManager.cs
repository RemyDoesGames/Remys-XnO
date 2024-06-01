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

    private float timer;

    private PostProcessVolume blur;

    public RoundStatus currentRoundStatus;
    public GameState currentGameState;

    public GameObject[] field;
    public GameObject[] playerPrefab; // Circle is 0 - Cross is 1

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
        timer = 0;
    }

    private void Update()
    {
        switch (currentGameState)
        {
            case GameState.MainMenu:
                audioManager.FadeVolume(0.8f);
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
                audioManager.FadeVolume(0.3f);
                blur.enabled = false;
                if (WaitTimer(1))
                {
                    StartRound();
                }
                break;

            case GameState.OnGoingGame:
                CheckWinConditions();
                CheckRounds();
                audioManager.FadeVolume(0.3f);
                MainCameraAnimator.SetInteger("Phase", 1);
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
                audioManager.FadeVolume(0.3f);
                GameStart();
                MainCameraAnimator.SetInteger("Phase", 2);
                blur.enabled = false;
                break;
        }

        piecesPlaced = GameObject.FindGameObjectsWithTag("Player").Length;
    }

    private void CheckRounds()
    {
        if (round == 5)
        {
            currentGameState = GameState.GameOver;
        }
    }

    private void ResetPlayers()
    {
        foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
        {
            Destroy(player);
        }
    }

    private void ResetFields()
    {
        for (int i = 0; i < field.Length; i++)
        {
            field[i].tag = "Field";
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
        else if (piecesPlaced == 9)
        {
            currentRoundStatus = RoundStatus.Tie;
            round++;
            currentGameState = GameState.StandbyGame;
        }

    }

    private bool CheckWinCondition(string symbol)
    {
        return (field[0].CompareTag(symbol) && field[1].CompareTag(symbol) && field[2].CompareTag(symbol)) ||
               (field[3].CompareTag(symbol) && field[4].CompareTag(symbol) && field[5].CompareTag(symbol)) ||
               (field[6].CompareTag(symbol) && field[7].CompareTag(symbol) && field[8].CompareTag(symbol)) ||
               (field[0].CompareTag(symbol) && field[3].CompareTag(symbol) && field[6].CompareTag(symbol)) ||
               (field[1].CompareTag(symbol) && field[4].CompareTag(symbol) && field[7].CompareTag(symbol)) ||
               (field[2].CompareTag(symbol) && field[5].CompareTag(symbol) && field[8].CompareTag(symbol)) ||
               (field[0].CompareTag(symbol) && field[4].CompareTag(symbol) && field[8].CompareTag(symbol)) ||
               (field[2].CompareTag(symbol) && field[4].CompareTag(symbol) && field[6].CompareTag(symbol));
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
        ResetPlayers(); 
        SetTurn();
        ResetFields();
        currentGameState = GameState.OnGoingGame; 
    }

    public void GameStart()
    {
        MainCameraAnimator.SetInteger("Phase", 1);
        round = 0;
        circleWins = 0;
        crossWins = 0;
        currentGameState = GameState.StandbyGame;
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
    }

    public void ResumeGame()
    {
        currentGameState = GameState.OnGoingGame;
    }

    private bool WaitTimer(float sec)
    {
        Debug.Log(timer);
        timer += Time.deltaTime;
        if (timer > sec)
        {
            timer = 0;
            return true;
            
        }
        else
        {
            return false;
        }
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
