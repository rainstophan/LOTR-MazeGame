using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCreator : MonoBehaviour
{
    //Level: easy - normal - hard -hell
    // Spawn blocks in given area;
    // Animate blocks;
    // Collision detection: block - player (sticky platform ) ; lava flow - player (back to original ); block - block
    
    // Todo:
    // Modify the lava room
    // Get 2 chances to Spawn new block by user
    // what a life
    // Audio https://www.mediacollege.com/downloads/sound-effects/movie/lordoftherings/fellowship/
    // The Fellowship Theme - Lord of the Rings | EPIC VERSION

    
	public GameObject spawnObject;
    public GameObject userSpawnObject;
	public GameObject plane;

    [SerializeField] GameObject player;

    public int spawnNumber;
    public int spawnChances = 10;

    public float minX;
    public float minZ;
    public float maxX;
    public float maxZ;

    private GameObject[] currentBlocks;

    void Start()
    {
		// plane = GameObject.FindWithTag("SpawnPlane");

        player = GameObject.Find("PlayerCapsule");

        Mesh planeMesh = plane.GetComponent<MeshFilter>().mesh;
        Bounds bounds = planeMesh.bounds;

        minX = plane.transform.position.x - plane.transform.localScale.x * bounds.size.x * 0.5f ;
        maxX = plane.transform.position.x + plane.transform.localScale.x * bounds.size.x * 0.5f ;
        minZ = plane.transform.position.z - plane.transform.localScale.z * bounds.size.z * 0.5f ;
        maxZ = plane.transform.position.z + plane.transform.localScale.z * bounds.size.z * 0.5f ;

        //Debug.Log("Z range" + minZ + " X range" + minX);
        for (int i = 0; i < spawnNumber; i++){
            objectSpawn();
        }
	}

    void Update(){
        
        // Inspect your own child!!! Fixed bug
        if(transform.childCount < spawnNumber){
            for (int i = 0; i< spawnNumber - transform.childCount; i++){
                objectSpawn();
            }
        }  


        // If user press "HJKL", spawn a new block nearby left right up down
        if(Input.GetKeyDown(KeyCode.H) && spawnChances > 0) {

            Vector3 newVec = new Vector3(player.transform.position.x + 0.8f, player.transform.position.y, player.transform.position.z);
            
            Instantiate<GameObject>(userSpawnObject, newVec, Quaternion.identity, transform);  

            spawnChances--;
            Debug.Log("Used once chance. Current Spawn Chances :" + spawnChances);
        } 
        if(Input.GetKeyDown(KeyCode.J) && spawnChances > 0) {

            Vector3 newVec = new Vector3(player.transform.position.x - 0.8f, player.transform.position.y, player.transform.position.z);
            
            Instantiate<GameObject>(userSpawnObject, newVec, Quaternion.identity, transform);  

            spawnChances--;
            Debug.Log("Used once chance. Current Spawn Chances :" + spawnChances);
        } 
        if(Input.GetKeyDown(KeyCode.K) && spawnChances > 0) {

            Vector3 newVec = new Vector3(player.transform.position.x , player.transform.position.y, player.transform.position.z + 0.8f);
            
            Instantiate<GameObject>(userSpawnObject, newVec, Quaternion.identity, transform);  

            spawnChances--;
            Debug.Log("Used once chance. Current Spawn Chances :" + spawnChances);
        } 
        if(Input.GetKeyDown(KeyCode.L) && spawnChances > 0) {

            Vector3 newVec = new Vector3(player.transform.position.x , player.transform.position.y, player.transform.position.z - 0.8f);
            
            Instantiate<GameObject>(userSpawnObject, newVec, Quaternion.identity, transform);  

            spawnChances--;
            Debug.Log("Used once chance. Current Spawn Chances :" + spawnChances);
        } 

    }
    
	/// Spawns an object at a random location on a plane
	public void objectSpawn()
	{
		Vector3 randomPosition = GetARandomPos(plane);
                                                                  
        Instantiate<GameObject>(spawnObject, randomPosition, Quaternion.identity, transform);   
	
	}

	/// Return random position on the plane
	public Vector3 GetARandomPos(GameObject plane){

        Vector3 newVec = new Vector3(Random.Range (minX, maxX),
                                    plane.transform.position.y,
                                    Random.Range (minZ, maxZ));
        return newVec;
	}

    
}
