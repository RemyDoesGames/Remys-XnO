using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public RoundStatus currentRoundStatus;
    public GameState currentGameState;

    public GameObject[] field;
    public GameObject[] playerPrefab; // Circle is 0 - Cross is 1

    // UI Game Objects
    public GameObject uiCanvas;
    public GameObject mainMenuCanvas;
    public GameObject gameOverCanvas;
    public GameObject pausedGameCanvas;

    public AudioManager AudioManager;

    private Animator MainCameraAnimator;
    public AnimationClip StartGameAnimation;

    private void Start()
    {
        MainCameraAnimator = GameObject.Find("Main Camera").GetComponent<Animator>();
        currentGameState = GameState.MainMenu;
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
                mainMenuCanvas.SetActive(true);
                uiCanvas.SetActive(false);
                VolumeChange(0.8f);
                if (MainCameraAnimator.GetInteger("Phase") == 0)
                {
                    MainCameraAnimator.SetInteger("Phase", 0);
                }
                else
                {
                    MainCameraAnimator.SetInteger("Phase", 2);
                }
                break;
            case GameState.StandbyGame:
                VolumeChange(0.3f);
                StartCoroutine(WaitStartRound(1));
                break;
            case GameState.OnGoingGame:
                VolumeChange(0.3f);
                MainCameraAnimator.SetInteger("Phase", 1);
                StartRoundHasBeenCalled = true;
                GameStartHasBeenCalled = true;
                break;
            case GameState.PausedGame:
                VolumeChange(0);
                break;
            case GameState.GameOver:
                VolumeChange(0.3f);
                StartCoroutine(WaitEndGame(1));
                MainCameraAnimator.SetInteger("Phase", 2);
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
            gameOverCanvas.SetActive(true);
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
            
            MainCameraAnimator.SetInteger("Phase", 1); 
            round = 0;
            circleWins = 0;
            crossWins = 0; 
            StartRound();
        }
    }



    public void Setttings()
    {
        currentGameState = GameState.PausedGame;
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

    public void QuitGame()
    {
        Application.Quit();
    }

    public void VolumeChange(float vol)
    {
        AudioManager.sounds[0].volume = vol;
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
