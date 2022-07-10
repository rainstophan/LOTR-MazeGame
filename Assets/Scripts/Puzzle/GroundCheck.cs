using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public LayerMask lava;

    public AudioSource randomSound;
    public AudioClip[] audioClips;
    
    // public LayerMask brick;

    // Update is called once per frame
    void Update()
    {
        if(Physics.CheckSphere(gameObject.transform.position,.1f,lava)){
            Debug.Log("PlayerGround-lava collision");
            

            randomSound.clip = audioClips[Random.Range(0, audioClips.Length)];
            randomSound.Play(); 
        }

        // if(Physics.CheckSphere(gameObject.transform.position,.1f,brick)){
        //     Debug.Log("PlayerGround-brick collision");
        // }
        

    }

}
