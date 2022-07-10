using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCollision : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, .01f);

        // up and left walls have the collision script
        // down and right walls have the colliders
        foreach (Collider collider in colliders) {
            if (collider.CompareTag("Wall")) {
                Destroy(gameObject);
                return;
            }
        }

        //if we add columns in the corners they need seperate handling and collider adjustments
    }
}
