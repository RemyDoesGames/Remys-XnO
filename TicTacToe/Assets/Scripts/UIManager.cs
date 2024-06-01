
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


public class UIManager : MonoBehaviour
{
    public VisualTreeAsset uiAsset;
    public PanelSettings panelSettings;
    public StyleSheet styleSheet;

    private UIDocument _uiDocument;

    private VisualElement uiRoot;

    private bool settingsClicked;

    Button _playbutton;
    Button _resumeButton;
    Button _backButton;
    Button _pauseButton;
    Button _restartButton1;
    Button _restartButton2;
    Button _settingsButton1;
    Button _settingsButton2;
    Button _settingsButton3;
    Button _mainMenuButton1;
    Button _mainMenuButton2;

    Toggle _musicToggle;

    Label statusText;
    Label circleWinsText;
    Label crossWinsText;
    Label winnerText;

    VisualElement _mainMenuUI;
    VisualElement _gameOverUI;
    VisualElement _gui;
    VisualElement _pausedUI;
    VisualElement _settingsUI;

    GameManager GM;

    private void Awake()
    {
        GM = GameManager.Instance;
        _uiDocument = GetComponent<UIDocument>();
        uiRoot = _uiDocument.rootVisualElement;

        _mainMenuUI = uiRoot.Q<VisualElement>("MainMenu");
        _gameOverUI = uiRoot.Q<VisualElement>("GameOver");
        _gui = uiRoot.Q<VisualElement>("GUI");
        _pausedUI = uiRoot.Q<VisualElement>("Paused");
        _settingsUI = uiRoot.Q<VisualElement>("Settings");

        statusText = uiRoot.Q<Label>("GameStatusText");
        circleWinsText = uiRoot.Q<Label>("CounterCircle");
        crossWinsText = uiRoot.Q<Label>("CounterCross");
        winnerText = uiRoot.Q<Label>("Winner");

        _playbutton = uiRoot.Q<Button>("PlayButton");
        _pauseButton = uiRoot.Q<Button>("PauseButton");
        _resumeButton = uiRoot.Q<Button>("ResumeButton");
        _backButton = uiRoot.Q<Button>("BackButton");
        _restartButton1 = uiRoot.Q<Button>("RestartButton1");
        _restartButton2 = uiRoot.Q<Button>("RestartButton2");
        _settingsButton1 = uiRoot.Q<Button>("SettingsButton1");
        _settingsButton2 = uiRoot.Q<Button>("SettingsButton2");
        _settingsButton3 = uiRoot.Q<Button>("SettingsButton3");
        _mainMenuButton1 = uiRoot.Q<Button>("MainMenuButton1");
        _mainMenuButton2 = uiRoot.Q<Button>("MainMenuButton2");

        _musicToggle = uiRoot.Q<Toggle>("MusicToggle");

        _playbutton.RegisterCallback<ClickEvent>(OnPlayButtonClicked);
        _pauseButton.RegisterCallback<ClickEvent>(OnPauseButtonClicked);
        _resumeButton.RegisterCallback<ClickEvent>(OnResumeButtonClicked);
        _backButton.RegisterCallback<ClickEvent>(OnBackButtonClicked);
        _restartButton1.RegisterCallback<ClickEvent>(OnRestartButtonClicked);
        _restartButton2.RegisterCallback<ClickEvent>(OnRestartButtonClicked);
        _settingsButton1.RegisterCallback<ClickEvent>(OnSettingsButtonClicked);
        _settingsButton2.RegisterCallback<ClickEvent>(OnSettingsButtonClicked);
        _settingsButton3.RegisterCallback<ClickEvent>(OnSettingsButtonClicked);
        _mainMenuButton1.RegisterCallback<ClickEvent>(OnMainMenuButtonClicked);
        _mainMenuButton2.RegisterCallback<ClickEvent>(OnMainMenuButtonClicked);

        _musicToggle.RegisterCallback<ChangeEvent<bool>>(OnMusicToggle);
    }

    void Update()
    {
        switch (GM.currentRoundStatus)
        {
            case GameManager.RoundStatus.CircleTurn:
                statusText.text = "Player 2 Playing";
                circleWinsText.text = Convert.ToString(GM.circleWins);
                crossWinsText.text = Convert.ToString(GM.crossWins);
                break;

            case GameManager.RoundStatus.CrossTurn:
                statusText.text = "Player 1 Playing";
                circleWinsText.text = Convert.ToString(GM.circleWins);
                crossWinsText.text = Convert.ToString(GM.crossWins);
                break;

            case GameManager.RoundStatus.CircleWon:
                statusText.text = "Player 2 Won";
                circleWinsText.text = Convert.ToString(GM.circleWins);
                break;

            case GameManager.RoundStatus.CrossWon:
                statusText.text = "Player 1 Won";
                crossWinsText.text = Convert.ToString(GM.crossWins);
                break;

            case GameManager.RoundStatus.Tie:
                statusText.text = "Tie";
                break;     
                
            default:
                Debug.Log("Unknown Game Status");
                break;
        }

        switch (GM.currentGameState)
        {
            case GameManager.GameState.GameOver:
                winnerText.text = GM.gameWinner;
                if (settingsClicked == true)
                {
                    ChangeDisplay(_settingsUI);
                }
                else if (settingsClicked == false)
                {
                    ChangeDisplay(_gameOverUI);
                }
                break;

            default:
                break;
        }
    }

    private void ChangeDisplay(VisualElement _visualElement)
    {
        foreach (var ui in uiRoot.Children())
        {
            ui.style.display = DisplayStyle.None;
        }
        _visualElement.style.display = DisplayStyle.Flex;
    }

    private void OnPlayButtonClicked(ClickEvent evt)
    {
        ChangeDisplay(_gui);
        GM.GameStart();
    }

    private void OnPauseButtonClicked(ClickEvent evt)
    {
        ChangeDisplay(_pausedUI);
        GM.PausedGame();
    }

    private void OnResumeButtonClicked(ClickEvent evt)
    {
        ChangeDisplay(_gui);
        GM.ResumeGame();
    }
    private void OnRestartButtonClicked(ClickEvent evt)
    {
        ChangeDisplay(_gui);
        GM.GameStart();
    }
    private void OnSettingsButtonClicked(ClickEvent evt)
    {
        if (GM.currentGameState == GameManager.GameState.GameOver)
        {
            settingsClicked = true;
        }
        ChangeDisplay(_settingsUI);
    }
    private void OnBackButtonClicked(ClickEvent evt)
    {
        if (GM.currentGameState == GameManager.GameState.MainMenu)
        {
            ChangeDisplay(_mainMenuUI);
        }
        if (GM.currentGameState == GameManager.GameState.GameOver)
        {
            settingsClicked = false;
            ChangeDisplay(_gameOverUI);
        }
        if (GM.currentGameState == GameManager.GameState.PausedGame)
        {
            ChangeDisplay(_pausedUI);
        }
    }
    private void OnMainMenuButtonClicked(ClickEvent evt)
    {
        ChangeDisplay(_mainMenuUI);
        GM.MainMenu();
    }
    private void OnMusicToggle(ChangeEvent<bool> evt)
    {
        if (evt.newValue)
        {
            GM.musicMuted = true;
        }
        else
        {
            GM.musicMuted = false;
        }
    }
}
