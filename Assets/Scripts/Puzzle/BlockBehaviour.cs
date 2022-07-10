using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBehaviour : MonoBehaviour
{
    // Collision detection: block - player (sticky platform ) ; lava flow - player (back to original ); block - block (change direction or destory one of them and the other scale up)
    // Decision: when collsion happens destory objects and creat new object. So user have to watch out collison all the time and make qiuck move
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
        
       // plane = GameObject.FindWithTag("SpawnPlane");

        levelCreator = GameObject.Find("LevelCreator");
        lc = levelCreator.GetComponent<LevelCreator>();
        plane = lc.plane;

        Mesh planeMesh = plane.GetComponent<MeshFilter>().mesh;
        Bounds bounds = planeMesh.bounds;

        time = Random.Range(minTime, maxTime);
        distance = Random.Range(minDistance, maxDistance);
        height = Random.Range(0.1f, 0.3f);

        // if exceed x/z axis bound, than move up and down
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

    
    void Update()
    {
        
       // iTween.MoveFrom(gameObject, Vector3 amount, time) )
    }

    private void OnCollisionEnter(Collision collision){
        
        if (collision.gameObject.tag == "SpawnBrick"){
            Destroy(gameObject);
            // lc.objectSpawn();
            // Debug.Log("Destory object");        
        }

        if (collision.gameObject.name == "PlayerCapsule"){
            groundSound.Play();
            collision.gameObject.transform.SetParent(transform);
            // Debug.Log("Player current parent:" + collision.gameObject.transform.parent.name);
        }
    }

    // IEnumerator DestoryAndSpawn(Collision collision){
        
    //     yield return new WaitForSeconds(4);
    //     Destroy(gameObject);
    //     Debug.Log("Destory object");
        
    //     lc.objectSpawn();
    //     Debug.Log("Spawn new object");
    // }


    private void OnCollisionExit(Collision collision){
        if (collision.gameObject.name == "PlayerCapsule"){
            collision.gameObject.transform.SetParent(null);
        }
    }

}
