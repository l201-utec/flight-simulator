using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

    public GameObject canvas;
    public bool gamePaused;

    public GameObject plane;

    private CollisionDetect rest;

    private void Start()
    {
        
    }

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

        plane.GetComponent<AudioSource>().enabled = false;

    }

    public void Resume()
    {
        Time.timeScale = 1f;
        canvas.SetActive(false);
        gamePaused = false;

        plane.GetComponent<AudioSource>().enabled = true;
    }

    public void Restart()
    {
        rest = plane.GetComponent<CollisionDetect>();

        rest.Restarting();

        Time.timeScale = 1f;
        canvas.SetActive(false);
        gamePaused = false;

    }
    
    public void MainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);

    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
