using UnityEngine.SceneManagement;
using UnityEngine;

public class TitleComponent : MonoBehaviour
{
    void Start()
    {
        GameManager.playerLives = 3;
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
            SceneManager.LoadScene("shop");
    }
}
