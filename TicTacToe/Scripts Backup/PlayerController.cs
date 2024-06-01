using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private bool mousePressed = false;
    private bool fieldEmpty = true;
    private Vector3 dropOffset = new Vector3(0, 1, 0);

    public void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        if (GameManager.Instance.currentGameState == GameManager.GameState.StandbyGame)
        {
            gameObject.tag = "Field";
            meshRenderer.enabled = false;
            fieldEmpty = true;
            mousePressed = false;
        }
    }

    private void OnMouseEnter()
    {
        if (fieldEmpty && GameManager.Instance.currentGameState == GameManager.GameState.OnGoingGame)
        {
            meshRenderer.enabled = true;
        }
    }

    private void OnMouseExit()
    {
        if (GameManager.Instance.currentGameState == GameManager.GameState.OnGoingGame)
        {
            meshRenderer.enabled = false;
            mousePressed = false;
        }
    }

    private void OnMouseDown()
    {
        if (fieldEmpty && GameManager.Instance.currentGameState == GameManager.GameState.OnGoingGame)
        {
            mousePressed = true;
        }
    }

    private void OnMouseUp()
    {
        if (mousePressed && fieldEmpty && GameManager.Instance.currentGameState == GameManager.GameState.OnGoingGame)
        {
            string tag = GameManager.Instance.playerTurnIndex == 0 ? "O" : "X";
            gameObject.tag = tag;
            GameManager.Instance.SpawnPlayer(dropOffset, transform.position);

            fieldEmpty = false;
            meshRenderer.enabled = false;
        }

        mousePressed = false;
    }
}
