using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RoundManager : MonoBehaviour
{
    public static int playersAlive;
    private List<GameObject> players;
    public ScoreBoardManager scoreBoard;
    private Text winText;
    private bool madeWin = false;
    private bool gameOver = false;
    // Start is called before the first frame update
    void Start()
    {
        winText = GetComponent<Text>();
        playersAlive = GameObject.Find("Managers").GetComponent<LevelManager>().numPlayers;
    }

    // Update is called once per frame
    void Update()
    {
        if (playersAlive == 1 && !madeWin)
        {
            GameObject winningPlayer = GameObject.FindGameObjectWithTag("Player");
            string name = winningPlayer.name;
            AddWin(name);
            madeWin = true;
            scoreBoard.SaveInfo();

            foreach(GameObject g in ScoreBoardManager.scoreBoards)
            {
                if(g.GetComponent<PlayerScoreUIManager>().numWins >= 3)
                {
                    gameOver = true;
                }
            }
            if(!gameOver)
            {
                StartCoroutine(RoundRestart());
            }
            else
            {
                StartCoroutine(EndGame());
            }
        }
    }
    
    IEnumerator RoundRestart()
    {
        GameObject winningPlayer = GameObject.FindGameObjectWithTag("Player");
        string name = winningPlayer.name;
        winText.text = name + " Victory";
        winText.color = FindObjectOfType<PlayerColorManager>().GetPlayerColor(false, int.Parse(name.Substring(name.Length - 1)));
        winText.enabled = true;
        yield return new WaitForSecondsRealtime(2);
        winText.enabled = false;
        foreach (GameObject g in ScoreBoardManager.scoreBoards)
        {
            g.SetActive(true);
        }
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("starterScene");
    }
    IEnumerator EndGame()
    {
        GameObject winningPlayer = GameObject.FindGameObjectWithTag("Player");
        string name = winningPlayer.name;
        winText.text = name + " Victory";
        winText.color = FindObjectOfType<PlayerColorManager>().GetPlayerColor(false, int.Parse(name.Substring(name.Length - 1)));
        winText.enabled = true;
        yield return new WaitForSecondsRealtime(2);
        winText.enabled = false;
        foreach (GameObject g in ScoreBoardManager.scoreBoards)
        {
            g.SetActive(true);
        }
        yield return new WaitForSeconds(3);
        foreach (GameObject g in ScoreBoardManager.scoreBoards)
        {
            g.SetActive(false);
        }
        winText.text = name + "\nis\nTHE SHOGUN";
        winText.color = FindObjectOfType<PlayerColorManager>().GetPlayerColor(false, int.Parse(name.Substring(name.Length - 1)));
        winText.enabled = true;
        Application.Quit();
    }

    public void AddWin(string name)
    {
        if (name.Contains("1"))
        {
            scoreBoard.p1Text.GetComponent<PlayerScoreUIManager>().numWins += 1;
        }
        if (name.Contains("2"))
        {
            scoreBoard.p2Text.GetComponent<PlayerScoreUIManager>().numWins += 1;
        }
        if (name.Contains("3"))
        {
            scoreBoard.p3Text.GetComponent<PlayerScoreUIManager>().numWins += 1;
        }
        if (name.Contains("4"))
        {
            scoreBoard.p4Text.GetComponent<PlayerScoreUIManager>().numWins += 1;
        }
    }
}
