using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField]
    private GameObject MainMenuPanel;

    [SerializeField]
    private GameObject OptionsPanel;

    [SerializeField]
    private string GameSceneName;


    public void Start()
    {
        ExitOptions();
        FindObjectOfType<SoundManager>().PlayBGM("bg_music");
    }

    public void PlayGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(GameSceneName);
    }

    public void EnterOptions()
    {
        MainMenuPanel.SetActive(false);
        OptionsPanel.SetActive(true);
    }

    public void ExitOptions()
    {
        MainMenuPanel.SetActive(true);
        OptionsPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }


}
