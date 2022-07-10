using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuestMover : MonoBehaviour
{

    void Start()
    {
        iTween.MoveBy(gameObject, iTween.Hash("y", 0.2,"time", Random.Range(1.0f, 2.0f), "easeType", "easeInOutSine", "loopType", "pingPong", "delay", 0.1f));
        iTween.RotateBy(gameObject, iTween.Hash("amount", new Vector3(0,0.4f,0), "time", Random.Range(5.0f, 8.0f), "loopType", "pingPong", "delay", 0.5f));
    }

}
