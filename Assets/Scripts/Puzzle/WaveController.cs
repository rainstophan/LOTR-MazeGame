using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveController : MonoBehaviour
{
    public float height;
    public float time; // the duration of each animation

    void Start()
    {
        iTween.MoveBy(gameObject, iTween.Hash("y", height,"time", time, "easeType", "easeInOutSine", "loopType", "pingPong", "delay", .1));
    }


}
