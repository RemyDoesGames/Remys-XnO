
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    private TextMeshProUGUI statusText;
    public TextMeshProUGUI circleWinsText;
    public TextMeshProUGUI crossWinsText;

    GameManager GM;
    void Start()
    {
        GM = GameManager.Instance;
        statusText = GameObject.Find("Game Status").GetComponent<TextMeshProUGUI>();
    }

    
    void Update()
    {
        
        
        // This is the in-Game Match status
        switch (GM.currentRoundStatus)
        {
            case GameManager.RoundStatus.CircleTurn:
                statusText.text = "Circle is Playing";
                circleWinsText.text = System.Convert.ToString(GM.circleWins);
                crossWinsText.text = System.Convert.ToString(GM.crossWins);
                break;
            case GameManager.RoundStatus.CrossTurn:
                statusText.text = "Cross is Playing";
                circleWinsText.text = System.Convert.ToString(GM.circleWins);
                crossWinsText.text = System.Convert.ToString(GM.crossWins);
                break;
            case GameManager.RoundStatus.CircleWon:
                statusText.text = "Circle Won";
                circleWinsText.text = System.Convert.ToString(GM.circleWins);
                break;
            case GameManager.RoundStatus.CrossWon:
                statusText.text = "Cross Won";
                crossWinsText.text = System.Convert.ToString(GM.crossWins);
                break;
            case GameManager.RoundStatus.Tie:
                statusText.text = "It' a Tie";
                break;         
            default:
                Debug.Log("Unknown Game Status");
                break;
        }
    }
}
