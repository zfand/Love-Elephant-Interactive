using UnityEngine;
using System.Collections;

public class PlayerKeys : MonoBehaviour {

	public bool w;
	public bool g;
	public bool y;
	public bool p;
	public bool r;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void addKey(string key) {
		if (key.Contains("white")) {
			w = true;
		} else if (key.Contains("green")) {
			g = true;
		} else if (key.Contains("yellow")) {
			y = true;
		} else if (key.Contains("pink")) {
			p = true;
		} else if (key.Contains("red")) {
			r = true;
		}
	}

	public bool hasKey(string key) {
		return ((key == "white" && w) ||
		        (key == "green" && g) ||
		        (key == "yellow" && y) ||
		        (key == "pink" && p) ||
		        (key == "red" && r));
	}
}
