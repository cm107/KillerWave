using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static GameManager instance;
    public static GameManager Instance
    {
        get {return instance;}
    }

    public static int playerLives = 3;
    public static int currentScene = 0;
    public static int gameLevelScene = 3;

    bool died = false;
    public bool Died
    {
        get {return died;}
        set {died = value;}
    }

    void Awake()
    {
        CheckGameManagerIsInTheScene();
        currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        LightAndCameraSetup(currentScene);

        #if UNITY_ANDROID
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        #endif
    }

    void Start()
    {
        SetLivesDisplay(playerLives);
    }

    private void LightAndCameraSetup(int sceneNumber)
    {
        switch (sceneNumber)
        {
            // testLevel, Level1, Level2, Level3
            case 3: case 4:
            {
                LightSetup();
                CameraSetup(0);
                break;
            }
            case 5:
            {
                CameraSetup(150);
                break;
            }
        }
    }

    void CheckGameManagerIsInTheScene()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void CameraSetup(float camSpeed)
    {
        GameObject gameCamera = GameObject.FindGameObjectWithTag("MainCamera");

        // Camera Transform
        gameCamera.transform.position = new Vector3(0, 0, -300);
        gameCamera.transform.eulerAngles = Vector3.zero;

        // Camera Properties
        Camera camera = gameCamera.GetComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color32(0,0,0,255);

        // Camera Movement Speed
        gameCamera.GetComponent<CameraMovement>().CamSpeed = camSpeed;
    }

    void LightSetup()
    {
        GameObject dirLight = GameObject.Find("Directional Light");
        dirLight.transform.eulerAngles = new Vector3(50, -30, 0);
        Light light = dirLight.GetComponent<Light>();
        light.color = new Color32(152, 204, 255, 255);
    }

    void Update()
    {
        
    }

    public void LifeLost()
    {
        StartCoroutine(DelayedLifeLost());
    }

    IEnumerator DelayedLifeLost()
    {
        yield return new WaitForSeconds(2);
        
        // lose life
        if (playerLives >= 1)
        {
            playerLives--;
            Debug.Log($"Lives left: {playerLives}");
            GetComponent<ScenesManager>().ResetScene();
        }
        else
        {
            playerLives = 3;
            GetComponent<ScenesManager>().GameOver();
        }
    }

    public void SetLivesDisplay(int players)
    {
        GameObject lives = GameObject.Find("lives");
        if (lives != null)
        {
            if (lives.transform.childCount < 1)
            {
                for (int i = 0; i < 5; i++)
                {
                    GameObject life = GameObject.Instantiate(
                        Resources.Load("life")
                    ) as GameObject;
                    life.transform.SetParent(lives.transform);
                }
            }

            // Note: We are adjusting the scale because we don't want to affect the Horizontal Layout Group
            // set visual lives
            for (int i = 0; i < lives.transform.childCount; i++)
                lives.transform.GetChild(i).localScale = Vector3.one;
            
            // remove visual lives
            for (int i = 0; i < (lives.transform.childCount - players); i++)
                lives.transform.GetChild(
                    lives.transform.childCount - i - 1
                ).localScale = Vector3.zero;
            }
    }
}
