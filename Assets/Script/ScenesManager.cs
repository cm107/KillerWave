using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

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
    public MusicMode musicMode;
    public enum MusicMode
    {
        noSound, // no music is playing
        fadeDown, // the music's volume will fade to zero
        musicOn // the music will be playing and will be set to its maximum valume
    }

    float gameTimer = 0;
    float[] endLevelTimer = {30, 30, 45};
    int currentSceneNumber = 0;
    bool gameEnding = false;

    void Start()
    {
        StartCoroutine(MusicVolume(MusicMode.musicOn));
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene aScene, LoadSceneMode aMode)
    {
        StartCoroutine(MusicVolume(MusicMode.musicOn));

        GetComponent<GameManager>().SetLivesDisplay(GameManager.playerLives);
        GameObject score = GameObject.Find("score");
        if (score != null)
            score.GetComponent<Text>().text = GetComponent<ScoreManager>().PlayersScore.ToString();
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

    IEnumerator MusicVolume(MusicMode musicMode)
    {
        AudioSource audioSource = GetComponentInChildren<AudioSource>();
        switch (musicMode)
        {
            case MusicMode.noSound:
            {
                audioSource.Stop();
                break;
            }
            case MusicMode.fadeDown:
            {
                audioSource.volume -= Time.deltaTime / 3;
                break;
            }
            case MusicMode.musicOn:
            {
                if (audioSource.clip != null)
                {
                    audioSource.Play();
                    audioSource.volume = 1;
                }
                break;
            }
        }
        yield return new WaitForSeconds(0.1f);
    }

    private void GameTimer()
    {
        AudioSource audioSource = GetComponentInChildren<AudioSource>();
        switch (scenes)
        {
            case Scenes.level1: case Scenes.level2: case Scenes.level3:
            {
                if (audioSource.clip == null)
                {
                    AudioClip lvlMusic = Resources.Load<AudioClip>("Sound/lvlMusic") as AudioClip;
                    audioSource.clip = lvlMusic;
                    audioSource.Play();
                }

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
                        StartCoroutine(MusicVolume(MusicMode.fadeDown));

                        GameObject player = GameObject.Find("Player");
                        player.GetComponent<Rigidbody>().isKinematic = true;
                        Player.mobile = false;
                        CancelInvoke(); // Stops auto-fire when using mobile platform.

                        PlayerTransition playerTransition = (
                            GameObject.FindGameObjectWithTag("Player")
                            .GetComponent<PlayerTransition>()
                        );  
                        if (SceneManager.GetActiveScene().name != "level3")
                            playerTransition.LevelEnds = true;
                        else
                            playerTransition.GameCompleted = true;
                        
                        SendInJsonFormat(SceneManager.GetActiveScene().name);

                        Invoke("NextLevel", 4);
                    }
                }
                break;
            }
            default:
            {
                audioSource.clip = null;
                break;
            }
        }
    }

    private void SendInJsonFormat(string lastLevel)
    {
        if (lastLevel == "level3")
        {
            GameStats gameStats = new GameStats();
            gameStats.livesLeft = GameManager.playerLives;
            gameStats.completed = System.DateTime.Now.ToString();
            gameStats.score = GetComponent<ScoreManager>().PlayersScore;

            string json = JsonUtility.ToJson(gameStats, prettyPrint: true);
            Debug.Log(json);

            string savePath = $"{Application.persistentDataPath}/GameStatsSaved.json";
            Debug.Log(savePath);
            System.IO.File.WriteAllText(savePath, json);
        }
    }

    private void GetScene()
    {
        scenes = (Scenes)currentSceneNumber;
    }

    public void ResetScene()
    {
        StartCoroutine(MusicVolume(MusicMode.noSound));
        gameTimer = 0;
        SceneManager.LoadScene(GameManager.currentScene);
    }

    private void NextLevel()
    {
        gameEnding = false;
        gameTimer = 0;
        SceneManager.LoadScene(GameManager.currentScene+1);
        StartCoroutine(MusicVolume(MusicMode.musicOn));
    }

    public void GameOver()
    {
        Debug.Log($"ENDSCORE: {GameManager.Instance.GetComponent<ScoreManager>().PlayersScore}");
        SceneManager.LoadScene("gameOver");
    }

    public void BeginGame(int gameLevel)
    {
        gameTimer = 0;
        SceneManager.LoadScene(gameLevel);
    }
}
