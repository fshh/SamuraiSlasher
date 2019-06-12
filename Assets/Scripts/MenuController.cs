using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public GameObject numPlayersButton;
    public GameObject startButton;

    private void Start()
    {
        if(!numPlayersButton)
        {
            numPlayersButton = GameObject.Find("NumPlayers");
        }
    }

    public void ChangeNumPlayers(BaseEventData moveData)
    {
        if (moveData is AxisEventData)
        {
            if (moveData.selectedObject == numPlayersButton)
            {
                if (((AxisEventData)moveData).moveDir == MoveDirection.Left)
                {
                    try
                    {
                        GameSettings.SetNumberOfPlayers(GameSettings.NumberOfPlayers - 1);
                        // Change UI to show new number
                        string text = numPlayersButton.GetComponentInChildren<Text>().text;
                        numPlayersButton.GetComponentInChildren<Text>().text = text.Substring(0, text.Length - 1) + GameSettings.NumberOfPlayers;
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
                        string text = numPlayersButton.GetComponentInChildren<Text>().text;
                        numPlayersButton.GetComponentInChildren<Text>().text = text.Substring(0, text.Length - 1) + GameSettings.NumberOfPlayers;
                    }
                    catch (System.ArgumentException)
                    {
                        Debug.Log("Reached Limit!");
                    }
                }
                if (((AxisEventData)moveData).moveDir == MoveDirection.Up || ((AxisEventData)moveData).moveDir == MoveDirection.Down)
                {
                    EventSystem.current.SetSelectedGameObject(startButton);
                }
            }
        }
    }

    public void StartGame()
    {
        // NEED TO MAKE THIS MORE DYNAMIC
        SceneManager.LoadScene(0);
    }
}
