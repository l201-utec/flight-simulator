using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

    public GameObject canvas;
    public bool gamePaused;

    void Update () {

        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown("joystick button 6"))
        {
            if (gamePaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
            
        }

        if(gamePaused && Input.GetKeyDown("joystick button 7"))
        {
            Restart();
        }
        
	}

    public void Pause()
    {
        Time.timeScale = 0f;
        canvas.SetActive(true);
        gamePaused = true;

        AudioListener.volume = 0f;
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        canvas.SetActive(false);
        gamePaused = false;

        AudioListener.volume = 1f;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        AudioListener.volume = 1f;
        canvas.SetActive(false);
        gamePaused = false;
        SceneManager.LoadScene("Gameplay");
    }
    
    public void MainMenu()
    {
        Time.timeScale = 1f;
        AudioListener.volume = 1f;
        canvas.SetActive(false);
        gamePaused = false;
        SceneManager.LoadScene("MainMenu");

    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
