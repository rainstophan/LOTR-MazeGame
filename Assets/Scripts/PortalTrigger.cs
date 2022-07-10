using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTrigger : MonoBehaviour
{
    //GameOverScreen gameOverScreen;
    //UIManager manager;
    private GameObject show;
    private GameObject setTime;

    public void Portal() {
        // here we want to set the final time in the end screen
        // and then display the end screen
        show = GameObject.Find("Overlay");
        show.GetComponent<UIManager>().EndCheck();

        setTime = GameObject.Find("EndScreen");
        setTime.GetComponent<GameOverScreen>().EndSetup();
    }
}
