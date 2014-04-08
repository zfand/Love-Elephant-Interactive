using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LustWaypoint : MonoBehaviour {

	public List<GameObject> adjacent;
	// Use this for initialization
	void Start () {
		Random.seed = (int)Time.time;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	GameObject getRandomAdjacent(){
		return adjacent[Random.Range(0, adjacent.Count - 1)];
	}
}
