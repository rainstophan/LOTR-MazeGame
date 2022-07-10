using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyHolder : MonoBehaviour
{
    public void AddKey() {
        Keydisplay.keyCount += 1;
    }

    private void OnTriggerEnter(Collider collider) {
        Key key = collider.GetComponent<Key>();
        if (collider.CompareTag("Key")) {
            AddKey();
            key.RemoveKey();
        }

        KeyDoor keyDoor = collider.GetComponent<KeyDoor>();
        if (collider.CompareTag("Level Door") && Keydisplay.keyCount >= 3) {
            Keydisplay.keyCount = 0;
            keyDoor.OpenDoor();
        }

        PortalTrigger port = collider.GetComponent<PortalTrigger>();
        if (collider.CompareTag("Portal")) {
            port.Portal();
        }
    }
}
