using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Occlusion : MonoBehaviour
{
    private float renderDist = 16f;
    public GameObject capsul;
    private GameObject[] allObjects; 

    public void getObjects() {
        allObjects = GameObject.FindGameObjectsWithTag("Room");
    }

    // Update is called once per frame
    void Update() {
        Vector3 playr = capsul.transform.position;

        foreach (var obj in allObjects) {
            if (Vector3.Distance(playr, obj.transform.position) <= renderDist) {
                obj.SetActive(true);
            } else {
                obj.SetActive(false);
            }
        }
    }
}
