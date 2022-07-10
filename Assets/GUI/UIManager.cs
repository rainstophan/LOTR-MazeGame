using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject endScreen;
    public GameObject startScreen;
    private bool gamePaused = false;


    // Use this for initialization
    void Start() {
        endScreen.SetActive(false);
        startScreen.SetActive(true);
        GameObject.Find("Time").GetComponent<UITimer>().playing = false;
    }
    void Update() {
        if (Input.GetKeyDown(KeyCode.P)) {
            if (gamePaused) {
                GameObject.Find("Time").GetComponent<UITimer>().playing = false;
                startScreen.SetActive(true);
                gamePaused = false;
            } else {
                GameObject.Find("Time").GetComponent<UITimer>().playing = true;
                startScreen.SetActive(false);
                gamePaused = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.R)) {
            SceneManager.LoadScene("SampleScene");
        }

        if (Input.GetKeyDown(KeyCode.Q)) {
            Application.Quit();
            //UnityEditor.EditorApplication.isPlaying = false;
        }
    }

    public void EndCheck() {
        endScreen.SetActive(true);
    }
}
