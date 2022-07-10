using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBehavior : MonoBehaviour
{
    public GameObject[] walls;      // 0-3 --> up, down, right, left
    public GameObject[] doors;      // same

    // Update is called once per frame
    public void UpdateRoom(bool[] status) {
        // if true then the door is present
        for (int i = 0; i < status.Length; i++) {
            doors[i].SetActive(status[i]);
            walls[i].SetActive(!status[i]);
        }
    }
}
