using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaCollsionDetector : MonoBehaviour
{
    public AudioSource randomSound;
 
    public AudioClip[] audioClips;

    [SerializeField] GameObject player;
    [SerializeField] LayerMask lava;

    private GameObject[] teleports;
    private GameObject currentTeleport;
    
    void Start(){
        player = GameObject.Find("PlayerCapsule");
        
        teleports = GameObject.FindGameObjectsWithTag("teleport");          
    }


    void Update()
    {
    
        if(Physics.CheckSphere(player.transform.position,.1f,lava)){
            Debug.Log("Player-lava collision");
            currentTeleport = FindClosestTeleport();
            // Debug.Log("currentTeleport position: " + currentTeleport.transform.position);
            iTween.MoveTo(player, currentTeleport.transform.position, 1.0f);
            // player.transform.position = currentTeleport.transform.position;

            randomSound.clip = audioClips[Random.Range(0, audioClips.Length)];
            randomSound.Play(); 

        }
 

    }

    public GameObject FindClosestTeleport()
    {
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = player.transform.position;
        foreach (GameObject go in teleports)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        return closest;
    }
   
}
