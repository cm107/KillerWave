using UnityEngine.SceneManagement;
using UnityEngine;
using System;

public class ScenesManager : MonoBehaviour
{
    Scenes scenes;
    public enum Scenes
    {
        bootUp,
        title,
        shop,
        level1,
        level2,
        level3,
        gameOver
    }

    float gameTimer = 0;
    float[] endLevelTimer = {30, 30, 45};
    int currentSceneNumber = 0;
    bool gameEnding = false;

    void Start()
    {
        
    }

    void Update()
    {
        if (currentSceneNumber != SceneManager.GetActiveScene().buildIndex)
        {
            currentSceneNumber = SceneManager.GetActiveScene().buildIndex;
            GetScene();
        }
        GameTimer();
    }

    private void GameTimer()
    {
        switch (scenes)
        {
            case Scenes.level1: case Scenes.level2: case Scenes.level3:
            {
                // Note: Level1 is (Scenes)3.
                if (gameTimer < endLevelTimer[currentSceneNumber-3])
                {
                    // if level has not completed
                    gameTimer += Time.deltaTime;
                }
                else
                {
                    // if level is completed
                    if (!gameEnding)
                    {
                        gameEnding = true;

                        PlayerTransition playerTransition = (
                            GameObject.FindGameObjectWithTag("Player")
                            .GetComponent<PlayerTransition>()
                        );  
                        if (SceneManager.GetActiveScene().name != "level3")
                            playerTransition.LevelEnds = true;
                        else
                            playerTransition.GameCompleted = true;
                        Invoke("NextLevel", 4);
                    }
                }
                break;
            }
        }
    }

    private void GetScene()
    {
        scenes = (Scenes)currentSceneNumber;
    }

    public void ResetScene()
    {
        gameTimer = 0;
        SceneManager.LoadScene(GameManager.currentScene);
    }

    private void NextLevel()
    {
        gameEnding = false;
        gameTimer = 0;
        SceneManager.LoadScene(GameManager.currentScene+1);
    }

    public void GameOver()
    {
        Debug.Log($"ENDSCORE: {GameManager.Instance.GetComponent<ScoreManager>().PlayersScore}");
        SceneManager.LoadScene("gameOver");
    }

    public void BeginGame(int gameLevel)
    {
        SceneManager.LoadScene(gameLevel);
    }
}
