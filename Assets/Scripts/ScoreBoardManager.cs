using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreBoardManager : MonoBehaviour
{
    public GameObject p1Text;
    public GameObject p2Text;
    public GameObject p3Text;
    public GameObject p4Text;
    public static List<GameObject> scoreBoards;
    public LevelManager levelMan;
    // Start is called before the first frame update
    void Start()
    {
        p1Text.GetComponent<PlayerScoreUIManager>().numWins = ScoreSaver.Instance.p1;
        p2Text.GetComponent<PlayerScoreUIManager>().numWins = ScoreSaver.Instance.p2;
        p3Text.GetComponent<PlayerScoreUIManager>().numWins = ScoreSaver.Instance.p3;
        p4Text.GetComponent<PlayerScoreUIManager>().numWins = ScoreSaver.Instance.p4;
        scoreBoards = new List<GameObject>();
        if(levelMan.numPlayers >= 2)
        {
            scoreBoards.Add(p1Text);
            scoreBoards.Add(p2Text);
        }
        if(levelMan.numPlayers >= 3)
        {
            scoreBoards.Add(p3Text);
        }
        if(levelMan.numPlayers == 4)
        {
            scoreBoards.Add(p4Text);
        }
        else
        {
            Debug.Log("invalid player count");
        }
    }

    public void SaveInfo()
    {
        ScoreSaver.Instance.p1 = p1Text.GetComponent<PlayerScoreUIManager>().numWins;
        ScoreSaver.Instance.p2 = p2Text.GetComponent<PlayerScoreUIManager>().numWins;
        ScoreSaver.Instance.p3 = p3Text.GetComponent<PlayerScoreUIManager>().numWins;
        ScoreSaver.Instance.p4 = p4Text.GetComponent<PlayerScoreUIManager>().numWins;
    }
}
