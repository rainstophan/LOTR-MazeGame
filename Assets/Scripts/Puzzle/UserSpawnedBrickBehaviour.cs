using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserSpawnedBrickBehaviour : MonoBehaviour
{
 
    public GameObject plane;
    public AudioSource groundSound;

    public float minTime;
    public float maxTime;
    public float minDistance;
    public float maxDistance;
    public float pauseTime;

    private GameObject levelCreator;
    private LevelCreator lc;

    private float time;
    private float distance;
    private float height;
    

    void Start()
    {
        

        levelCreator = GameObject.Find("LevelCreator");
        lc = levelCreator.GetComponent<LevelCreator>();
        plane = lc.plane;

        Mesh planeMesh = plane.GetComponent<MeshFilter>().mesh;
        Bounds bounds = planeMesh.bounds;

        time = Random.Range(minTime, maxTime);
        distance = Random.Range(minDistance, maxDistance);
        height = Random.Range(0.1f, 0.3f);

        if (gameObject.transform.position.x - distance > lc.minX && gameObject.transform.position.x + distance < lc.maxX ){
            iTween.MoveBy(gameObject, iTween.Hash("x", distance,"time", time, "easeType", "easeInOutSine", "loopType", "pingPong", "delay", pauseTime));
        } 
        else if (gameObject.transform.position.z - distance > lc.minZ && gameObject.transform.position.z + distance < lc.maxZ ){
            iTween.MoveBy(gameObject, iTween.Hash("z", distance,"time", time, "easeType", "easeInOutSine", "loopType", "pingPong", "delay", pauseTime));
        }
        else {
            iTween.MoveBy(gameObject, iTween.Hash("y", height,"time", time, "easeType", "easeInOutSine", "loopType", "pingPong", "delay", pauseTime));
        }
    }

    
   

    private void OnCollisionEnter(Collision collision){
        
        
        if (collision.gameObject.name == "PlayerCapsule"){
            groundSound.Play();
            collision.gameObject.transform.SetParent(transform);
           
        }
    }



    private void OnCollisionExit(Collision collision){
        if (collision.gameObject.name == "PlayerCapsule"){
            collision.gameObject.transform.SetParent(null);
        }
    }


}
