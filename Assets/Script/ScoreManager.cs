using System;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    static int playerScore;
    public int PlayersScore
    {
        get {return playerScore;}
    }

    public void SetScore(int incomingScore)
    {
        playerScore += incomingScore;
        UpdateScore();
    }

    public void ResetScore()
    {
        playerScore = 00000000;
        UpdateScore();
    }

    private void UpdateScore()
    {
        GameObject score = GameObject.Find("score");
        if (score != null)
            score.GetComponent<Text>().text = playerScore.ToString();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
