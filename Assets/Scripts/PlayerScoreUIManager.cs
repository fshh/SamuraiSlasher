using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScoreUIManager : MonoBehaviour
{   
    public int numWins;
    public GameObject win1;
    public GameObject win2;
    public GameObject win3;
    // Start is called before the first frame update
    void Start()
    {
        //DontDestroyOnLoad(gameObject);   
    }

    // Update is called once per frame
    void Update()
    {
        if(numWins == 1)
        {
            win1.SetActive(true);
        }
        if(numWins > 1)
        {
            win1.SetActive(true);
            win2.SetActive(true);
        }
        if(numWins > 2)
        {
            win1.SetActive(true);
            win2.SetActive(true);
            win3.SetActive(true);
        }
    }
}
