using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyDoor : MonoBehaviour
{
    public void OpenDoor() {
        gameObject.SetActive(false);
    }
}
