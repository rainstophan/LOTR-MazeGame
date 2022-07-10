using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITimer : MonoBehaviour
{
    public Text TimerText;
    public bool playing;
    private float Timer;

    // Update is called once per frame
    void Update() {
        if (playing) {
            Time.timeScale = 1;
            Timer += Time.deltaTime;
            int minutes = Mathf.FloorToInt(Timer / 60F);
            int seconds = Mathf.FloorToInt(Timer % 60F);
            TimerText.text = minutes.ToString("00") + ":" + seconds.ToString("00");
        }
        if (!playing) {
            Time.timeScale = 0;        }
    }
}
