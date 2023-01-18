using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine;

public class PauseComponent : MonoBehaviour
{
    [SerializeField] GameObject pauseScreen;
    [SerializeField] AudioMixer masterMixer;
    [SerializeField] GameObject musicSlider;
    [SerializeField] GameObject effectsSlider;

    void Awake()
    {
        Debug.Log("PauseComponent Awake");
        pauseScreen.SetActive(false);
        SetPauseButtonActive(false);
        Invoke("DelayPauseAppear", 5);

        // Works when we go back to the title screen and then return to the level.
        // Doesn't work if we stop the game and load the game back up again.
        // Although the slider bar is updated correctly, the music isn't being updated correctly.
        masterMixer.SetFloat("musicVol", PlayerPrefs.GetFloat("musicVolume"));
        masterMixer.SetFloat("effectsVol", PlayerPrefs.GetFloat("effectsVolume"));
        musicSlider.GetComponent<Slider>().value = GetMusicLevelFromMixer();
        effectsSlider.GetComponent<Slider>().value = GetEffectsLevelFromMixer();
    }

    void SetPauseButtonActive(bool switchButton)
    {
        Toggle toggle = GetComponentInChildren<Toggle>();
        ColorBlock col = toggle.colors;
        if (switchButton == false)
        {
            col.normalColor = new Color32(0,0,0,0);
            col.highlightedColor = new Color32(0,0,0,0);
            col.pressedColor = new Color32(0,0,0,0);
            col.disabledColor = new Color32(0,0,0,0);
            toggle.interactable = false;
        }
        else
        {
            col.normalColor = new Color32(245,245,245,255);
            col.highlightedColor = new Color32(245,245,245,255);
            col.pressedColor = new Color32(200,200,200,255);
            col.disabledColor = new Color32(200,200,200,128);
            toggle.interactable = true;
        }
        toggle.colors = col;
        toggle.transform.GetChild(0).GetChild(0).gameObject.SetActive(switchButton);
    }

    void DelayPauseAppear()
    {
        SetPauseButtonActive(true);
    }

    public void PauseGame()
    {
        pauseScreen.SetActive(true);
        SetPauseButtonActive(false);
        Time.timeScale = 0;
    }

    public void Resume()
    {
        pauseScreen.SetActive(false);
        SetPauseButtonActive(true);
        Time.timeScale = 1;
    }

    public void Quit()
    {
        Time.timeScale = 1;
        GameManager.Instance.GetComponent<ScoreManager>().ResetScore();
        GameManager.Instance.GetComponent<ScenesManager>().BeginGame(0);
    }

    public void SetMusicLevelFromSlider()
    {
        Slider slider = musicSlider.GetComponent<Slider>();
        masterMixer.SetFloat("musicVol", slider.value);
        PlayerPrefs.SetFloat("musicVolume", slider.value);
    }

    public void SetEffectsLevelFromSlider()
    {
        Slider slider = effectsSlider.GetComponent<Slider>();
        masterMixer.SetFloat("effectsVol", slider.value);
        PlayerPrefs.SetFloat("effectsVolume", slider.value);
    }

    float GetMusicLevelFromMixer()
    {
        bool result = masterMixer.GetFloat(
            "musicVol",
            out float musicMixersValue
        );

        if (result)
            return musicMixersValue;
        else
            return 0;
    }

    float GetEffectsLevelFromMixer()
    {
        bool result = masterMixer.GetFloat(
            "effectsVol",
            out float effectsMixersValue
        );

        if (result)
            return effectsMixersValue;
        else
            return 0;
    }
}
