using System.Collections;
using System.Collections.Generic;
using Rewired;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public GameObject numPlayersButton;

    public Image leftArrow;
    public Image rightArrow;

    public RandomSoundPlayer randomSwishPlayer;
    public SubmitSoundPlayer submitSoundPlayer;

    private Player player;

    private void Start()
    {
        if (!numPlayersButton)
        {
            numPlayersButton = GameObject.Find("NumPlayers");
        }

        player = ReInput.players.GetPlayer(0);
    }

    public void ChangeNumPlayers(BaseEventData moveData)
    {
        if (moveData is AxisEventData)
        {
            if (((AxisEventData)moveData).moveDir == MoveDirection.Left)
            {
                try
                {
                    GameSettings.SetNumberOfPlayers(GameSettings.NumberOfPlayers - 1);
                    // Change UI to show new number
                    TextMeshProUGUI textMesh = numPlayersButton.GetComponentInChildren<TextMeshProUGUI>();
                    textMesh.text = textMesh.text.Substring(0, textMesh.text.Length - 1) + GameSettings.NumberOfPlayers;
                    leftArrow.enabled = GameSettings.NumberOfPlayers > 2;
                    rightArrow.enabled = GameSettings.NumberOfPlayers < GameSettings.MAX_PLAYERS;
                    randomSwishPlayer.PlaySound();
                }
                catch (System.ArgumentException)
                {
                    Debug.Log("Reached Limit!");
                }
            }
            if (((AxisEventData)moveData).moveDir == MoveDirection.Right)
            {
                try
                {
                    GameSettings.SetNumberOfPlayers(GameSettings.NumberOfPlayers + 1);
                    // Change UI to show new number
                    TextMeshProUGUI textMesh = numPlayersButton.GetComponentInChildren<TextMeshProUGUI>();
                    textMesh.text = textMesh.text.Substring(0, textMesh.text.Length - 1) + GameSettings.NumberOfPlayers;
                    leftArrow.enabled = GameSettings.NumberOfPlayers > 2;
                    rightArrow.enabled = GameSettings.NumberOfPlayers < GameSettings.MAX_PLAYERS;
                    randomSwishPlayer.PlaySound();
                }
                catch (System.ArgumentException)
                {
                    Debug.Log("Reached Limit!");
                }
            }
        }
    }

    private void Update()
    {
        if (player.GetButtonDown("A"))
        {
            StartGame();
        }
    }

    public void StartGame()
    {
        submitSoundPlayer.PlaySound();
        // NEED TO MAKE THIS MORE DYNAMIC
        SceneManager.LoadScene("starterScene");
    }
}
