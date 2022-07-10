using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
    public Text finalTimeText;
    private GameObject timer;

    public void EndSetup() {
        Debug.Log("endsetup fuction working");
        timer = GameObject.Find("Time");
        timer.GetComponent<UITimer>().playing = false;
        finalTimeText.text = "Your time: " + timer.GetComponent<UITimer>().TimerText.text;
    }
}
