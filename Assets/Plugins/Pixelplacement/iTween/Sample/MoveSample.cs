using UnityEngine;
using System.Collections;

public class MoveSample : MonoBehaviour
{	
	void Start(){
		iTween.MoveBy(gameObject, iTween.Hash("x", 2, "easeType", iTween.EaseType.easeInOutSine, "loopType", "pingPong", "delay", .1));
	}
}

