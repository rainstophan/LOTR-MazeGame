using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Keydisplay : MonoBehaviour
{
    public static int keyCount;
    public Text keyText;

    // Update is called once per frame
    void Update() {
        keyText = GetComponent<Text>();
        keyText.text = "Keys: " + keyCount + "/3";
    }
}
